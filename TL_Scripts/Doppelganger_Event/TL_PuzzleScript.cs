using UnityEngine;
using System.Collections.Generic;

public class TL_PuzzleScript : MonoBehaviour {

	//Lists
	public List<GameObject> FlashingCubes = new List<GameObject>();
	public List<GameObject> Sequence = new List<GameObject>();
    public List<GameObject> Cubes = new List<GameObject>();
    public List<int> RandomNum = new List<int>();
	public List<int> DupedNumbers = new List<int>();

    //Colors
	public Color[] FlashingColors = new Color[4];
	public Color DefaultColor;
    private Color ChangingColor;
    
    //Floats
    public float Cooldown = 1f;
    private float RateOfFlashing = 1.5f;
    
    //GameObjects
    public GameObject FlashingCube;
    public GameObject Doppelganger;

    //Sprites
    public Sprite VadinhoSprite;
    public Sprite ZofiaSprite;
    public Sprite WuSprite;
    public Sprite IdleRune;
    public Sprite CorrectRune;
    public Sprite IncorrectRune;

    //Ints
    public int Index = 0;
    public int SequenceIndex = 0;
    private int RandomIndex;
    private int Increment = 0;
    private int[,] LevelAreaLayout;

    //Bool
    public bool IsSequenceFinished = false;

    //Scripts
    public TL_Instructions InstructionsScript;
    private TL_SpawnPC PCSpawnScript;
    private TL_PC_World_Movement WorldMovementScript;
    private TL_GridManager GridScript;


    void Start()
	{
        //Finds the gameobject and obtains the script
        WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

        //Obtains the script from this gameobject
        PCSpawnScript = gameObject.GetComponent<TL_SpawnPC>();

        //Spawns the PC based on a string in the serialized class
        PCSpawnScript.SpawnPC(WorldMovementScript.WorldMapDataClass.PC_Name);

        //Obtain the script from this gameobject
        GridScript = GetComponent<TL_GridManager>();

        //Function for initializing the layout
        InitializeLayout();

        //Adds to the cooldown to prevent instant start
		Cooldown += Time.realtimeSinceStartup;
	}

    void InitializeLayout()
    {
        for (int x = 0; x < GridScript.ReturnLevelAreaArray().GetLength(0); x++)
        {
            for (int z = 0; z < GridScript.ReturnLevelAreaArray().GetLength(1); z++)
            {
                GameObject PlatformClone;
                //If X is more than 2
                if (x > 2f)
                {
                    //Spawn the platform based on X and Z values
                    if (GridScript.ReturnLevelAreaArray()[x, z] == null)
                    {
                        GridScript.SpawnGameObject(GridScript.Platform, x, 0, z);
                    }

                    //Spawn the flashing cubes based on X and Z values
                    if (x == 3 && z == 1 || x == 5 && z == 1 || x == 4 && z == 0 || x == 4 && z == 2)
                    {
                        //Spawn the flashing cube
                        GridScript.SpawnGameObject(FlashingCube, x, 0.15f, z);

                        //Rotate the cube
                        GridScript.ReturnGOInLevelArea(x, z).transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                        //Add the flashing cubes into a list
                        FlashingCubes.Add(GridScript.ReturnGOInLevelArea(x, z));
                    }
                    else if (x == 4 && z == 1)
                    {
                        //Spawn the doppelganger
                        GameObject Doppelganger_Clone = (GameObject)Instantiate(Doppelganger, new Vector3(x, 0.1f, z), Quaternion.identity);

                        //Set default rotation
                        Doppelganger_Clone.transform.eulerAngles = new Vector3(90f, 0f, 0f);

                        //Switch case statement for the player prefs of the PC's name
                        switch (WorldMovementScript.WorldMapDataClass.PC_Name)
                        {
                            case "Vadinho(Clone)":
                                //Set doppelganger sprite to Vadinho's sprite
                                Doppelganger_Clone.GetComponent<SpriteRenderer>().sprite = VadinhoSprite;
                                break;

                            case "Zofia(Clone)":
                                //Set doppelganger sprite to Zofia's sprite
                                Doppelganger_Clone.GetComponent<SpriteRenderer>().sprite = ZofiaSprite;
                                break;

                            case "Wu(Clone)":
                                //Set doppelganger sprite to Wu's sprite
                                Doppelganger_Clone.GetComponent<SpriteRenderer>().sprite = WuSprite;
                                break;
                        }
                        //Spawn the platform based on X and Z values
                        PlatformClone = (GameObject)Instantiate(GridScript.Platform, new Vector3(x, 0, z), Quaternion.identity);

                        //Set default rotation
                        PlatformClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                        //Set the appropriate biome sprite
                        GridScript.SetBiomeSprites(PlatformClone, WorldMovementScript.WorldMapDataClass.Region_Name);
                    }
                }
                else
                {
                    if (GridScript.ReturnLevelAreaArray()[x, z] == null)
                    {
                        //Spawn the platform based on X and Z values
                        PlatformClone = (GameObject)Instantiate(GridScript.Platform, new Vector3(x, 0, z), Quaternion.identity);

                        //Set default rotation
                        PlatformClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                        //Set the appropriate biome sprite
                        GridScript.SetBiomeSprites(PlatformClone, WorldMovementScript.WorldMapDataClass.Region_Name);
                    }
                }
            }
        }

    }

	void Update()
	{
        //If the pause toggle is set to false
        if (!InstructionsScript.PauseToggle)
        {
            StartSequence();
        }

        //Find the timer gameobject
        GameObject Timer = GameObject.Find("Camera/Canvas/Display");

        //Obtain the script from itself
        TL_Timer TimerScript = GetComponent<TL_Timer>();

        //Obtain the script from the PC
        TL_PCMove PCScript = GameObject.FindGameObjectWithTag("PC").GetComponent<TL_PCMove>();

        //If the sequence is finished
        if (IsSequenceFinished)
        {
            //Activate the timer and the following scripts
            Timer.SetActive(true);
            PCScript.enabled = true;
            TimerScript.enabled = true;
        }
        else
        {
            //Deactivate the timer and the following scripts
            Timer.SetActive(false);
            PCScript.enabled = false;
            TimerScript.enabled = false;
        }
	}

    public void CheckSequence()
    {
        //Obtain the timer script from this gameobject
        TL_Timer TimerScript = GetComponent<TL_Timer>();

        //If the list that tracks the sequence is the same as the list that records the player's choice
        if (Sequence[Index] == Cubes[Index])
        {
            Sequence[Index].GetComponent<SpriteRenderer>().color = Color.cyan;
            
            //If the index for the last element in the list is 3
            if (Index == 3)
            {
                //Activate win condition
                TimerScript.CheckCondition(true);
                GameObject.Find("Level_Manager(Clone)").GetComponent<DN_Event_Manager>().WinState("Doppelganger");
            }
        }
        else
        {
            //Display the incorrect rune
            Cubes[Index].GetComponent<SpriteRenderer>().sprite = IncorrectRune;
            
            //Activate lose condition
            TimerScript.CheckCondition(false);
        }

    }

	void StartSequence()
	{
		if(Increment < 4 && !IsSequenceFinished)
		{
			if(Cooldown < Time.realtimeSinceStartup)
			{
                //Randomizes the number
                Randomizer(4);

                //Adds the sequence in a list
                Sequence.Add(FlashingCubes[RandomIndex].gameObject);

                //Flashes the cubes in a sequence
				FlashingSequence();

                //Increase the count
				Increment++;

                //Adds the cooldown
				Cooldown = RateOfFlashing + Time.realtimeSinceStartup;
			}
		}
		else
		{
            //Extend the cooldown once to set the default color of the cube
			if(Cooldown < Time.realtimeSinceStartup)
			{
                //Set default color
				SetDefaultColor();

                //Adds the cooldown
				Cooldown = RateOfFlashing + Time.realtimeSinceStartup;
			}
            //Sequence is finished so set bool to true
			IsSequenceFinished = true;
		}

	}

    int Randomizer(int ran_num)
    {
        int Ran_Num = Random.Range(0, ran_num);

        //While the random number is the same as the previous random number, re-randomize it
        while (Ran_Num == RandomIndex)
        {
            Ran_Num = Random.Range(0, ran_num);
        }

        //Assign the variable to the random number
        RandomIndex = Ran_Num;
        return RandomIndex;
    }

    void FlashingSequence()
	{
        //Set default color
        SetDefaultColor();

        //Set the colors from the list
        ChangingColor = FlashingColors[RandomIndex];

        //Assign the material from the list to the color variable
		FlashingCubes[RandomIndex].GetComponent<SpriteRenderer>().color = ChangingColor;
	}

	void SetDefaultColor()
	{
        //Variable that holds the default color
        ChangingColor = DefaultColor;

		//Loops through the list of gameobjects and if the sprite renderer isn't the default color then change it back
		for(int i = 0; i < Sequence.Count; i++)
		{
			if(Sequence[i].GetComponent<SpriteRenderer>().color != DefaultColor)
			{
				Sequence[i].GetComponent<SpriteRenderer>().color = DefaultColor;
			}
		}

	}

}
