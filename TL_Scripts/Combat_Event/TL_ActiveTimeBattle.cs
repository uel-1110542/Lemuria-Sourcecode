using UnityEngine;
using System.Collections.Generic;

public class TL_ActiveTimeBattle : MonoBehaviour {

	//Variables
	public GameObject PC;
	public GameObject NPC;

    public GameObject HitBox;
	public GameObject Special_HitBox;
	public GameObject Melee_Hitbox;
	public GameObject Enemy2_Hitbox;
	public GameObject MovementSquare;
	private GameObject[,] LevelMovement;

	public bool BattleStart = false;
	public int NPC_XPos;
	public int NPC_ZPos;
	public int X_Bounds;
	public int Z_Bounds;
	public Sprite Hitbox;

	public List<GameObject> Special_Hitboxes = new List<GameObject>();
	public List<GameObject> AttackHitboxes = new List<GameObject>();

    private float X_Offset;
    private float Z_Offset;

    private TL_FollowChar BoxScript;
	private TL_NPCBehaviour NPC_Script;
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
        
        //Function for initializing the layout of the level
        InitializeLayout();

        //Function for instantiating the hitboxes
		InitializeHitBoxes();
        InitializeNPCHitboxes();

        gameObject.GetComponent<TL_DisplayHP>().enabled = true;
	}

    //Sets the gameobject in the array specified by X and Z
    public void SetGOInLevelMovement(GameObject go, int x, int z)
    {
        LevelMovement[x, z] = go;
    }

    //Returns the gameobject from the 2D array
    public GameObject ReturnGOInLevelMovement(int x, int z)
    {
        return LevelMovement[x, z];
    }

    //Returns the 2D gameobject array
    public GameObject[,] ReturnLevelMovementArray()
    {
        return LevelMovement;
    }

    void Update()
	{
        //Locates the gameobjects
        NPC = GameObject.FindGameObjectWithTag("NPC");

        //Checks the winning condition
        CheckWinCondition();

        //Checks if the colliders are adjacent to the PC and out of bounds
        ColliderManager();

        //Checks if the special hitboxes are out of bounds
		UpdateHitboxes();

        //Cheat Function
        InstantKO();
    }

    //Cheat Function
    void InstantKO()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(GameObject.FindGameObjectWithTag("NPC"));
        }
    }

	void CheckWinCondition()
	{
        //Finds the NPC gameobject
		GameObject NPC_Clone = GameObject.FindGameObjectWithTag("NPC");

        //If the NPC is dead
		if(NPC_Clone == null)
		{
            //Obtain the timer script from this gameobject
            TL_Timer TimerScript = GetComponent<TL_Timer>();

            //Activate win condition
            TimerScript.CheckCondition(true);
        }
    }

	void InitializeLayout()
	{
        //Find the PC
        PC = GameObject.FindGameObjectWithTag("PC");

        //Obtains the script from the PC gameobject
        TL_MoveScript MoveScript = PC.GetComponent<TL_MoveScript>();

        //Obtain the script from this gameobject
        GridScript = gameObject.GetComponent<TL_GridManager>();

        //Disables the script
        MoveScript.enabled = false;

        //Sets the PC's X and Z position into the gameobject array
        GridScript.SetGOInLevelArea(PC, (int)Mathf.Round(PC.transform.position.x), (int)Mathf.Round(PC.transform.position.z));

        //Spawns NPC
        GridScript.SpawnGameObject(NPC, 5, 0.1f, 1);
        
        //Initialize the 2D gameobject array for checking movement
        LevelMovement = new GameObject[GridScript.Length_X, GridScript.Length_Z];

        //Sets default rotation
        GridScript.ReturnLevelAreaArray()[5, 1].transform.eulerAngles = new Vector3(90f, 0f, 0f);

        //If the NPC has the movement script
        if (GridScript.ReturnLevelAreaArray()[5, 1].GetComponent<TL_NPCBehaviour>() != null)
        {
            //Obtains the script from the NPC
            NPC_Script = GridScript.ReturnLevelAreaArray()[5, 1].GetComponent<TL_NPCBehaviour>();

            //Enables the script
            NPC_Script.enabled = true;
        }

        for (int x = 0; x < LevelMovement.GetLength(0); x++)
		{
			for(int z = 0; z < LevelMovement.GetLength(1); z++)
			{
                //Spawn the platform
                GameObject PlatformClone = Instantiate (GridScript.Platform, new Vector3(x, 0, z), Quaternion.identity);

                //Change the sprite according to the region selected
                GridScript.SetBiomeSprites(PlatformClone, WorldMovementScript.WorldMapDataClass.Region_Name);

                //Instantiates the movement squares and sets it to the 2D gameobject array depending on the for loops
                LevelMovement[x, z] = Instantiate (MovementSquare, new Vector3(x, 0.1f, z), Quaternion.identity);

                //Obtains the script from the 2D gameobject array
                TL_MoveOffset OffsetScript = LevelMovement[x, z].GetComponent<TL_MoveOffset>();

                //Set the offset
				OffsetScript.SetOffset(x, z);

                //Obtains the box collider component from the gameobject array
                BoxCollider MoveCol = LevelMovement[x, z].GetComponent<BoxCollider>();

                //Sets the collider to false
                MoveCol.enabled = false;
			}
		}
        //Starts the battle so set bool to true
		BattleStart = true;

	}

	void InitializeHitBoxes()
	{
		//Special hitboxes
		GameObject Special_HitboxClone;

        //Obtains the gameobject from the name tags
		PC = GameObject.FindGameObjectWithTag("PC");		

        //Variable for the instantiated hitboxes
        GameObject HitboxClone;        

        //Goes through the switch case statements depending on the PC's name
        switch (PC.name)
		{
		    case "Vadinho(Clone)":
                //Sets the X and Z offsets
			    X_Offset = 1f;
			    Z_Offset = 1f;

                //Instantiates the special hitbox
			    Special_HitboxClone = Instantiate (Special_HitBox, new Vector3 (1f, 0.15f, 1f), Quaternion.identity);

                //Enables the box collider
                Special_HitboxClone.GetComponent<BoxCollider>().enabled = false;

                //Obtains the script from the special hitbox
                BoxScript = Special_HitboxClone.GetComponent<TL_FollowChar>();

                //Assigns the character variable as the PC
			    BoxScript.Character = PC;

			    //Instantiates the hitboxes with a for loop
			    for (int i = -1; i < 2; i++)
			    {
				    HitboxClone = Instantiate(HitBox, new Vector3(PC.transform.position.x + X_Offset, 0.15f, PC.transform.position.z + i), Quaternion.identity);
				    HitboxClone.transform.localEulerAngles = new Vector3 (90f, 0f, 0f);

                    //Obtain the script from the special hitbox
                    BoxScript = HitboxClone.GetComponent<TL_FollowChar>();

                     //Sets the X and Z offsets for the hitboxes
                    BoxScript.X_Pos = X_Offset;
				    BoxScript.Z_Pos = i;

                    //Assigns the character variable as the PC
                    BoxScript.Character = PC;
			    }

                for (int i = -1; i < 2; i++)
                {
                    HitboxClone = Instantiate(HitBox, new Vector3(PC.transform.position.x - X_Offset, 0.15f, PC.transform.position.z + i), Quaternion.identity);
                    HitboxClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                    //Obtain the script from the special hitbox
                    BoxScript = HitboxClone.GetComponent<TL_FollowChar>();

                    //Sets the X and Z offsets for the hitboxes
                    BoxScript.X_Pos = -X_Offset;
                    BoxScript.Z_Pos = i;

                    //Assigns the character variable as the PC
                    BoxScript.Character = PC;
                }
                break;

		    case "Zofia(Clone)":
                //Sets the X and Z offsets
                X_Offset = 1f;
			    Z_Offset = 2f;

                //Instantiates the hitbox
                HitboxClone = Instantiate(HitBox, new Vector3(PC.transform.position.x + 2f, 0.15f, PC.transform.position.z), Quaternion.identity);
			    HitboxClone.transform.localEulerAngles = new Vector3 (90f, 0f, 0f);
                
                //Enables the box collider
                BoxScript = HitboxClone.GetComponent<TL_FollowChar>();

                //Sets the X and Z offsets for the hitboxes
                BoxScript.X_Pos = 2f;
                BoxScript.Z_Pos = 0f;

                //Assigns the character variable as the PC
                BoxScript.Character = PC;

                HitboxClone = Instantiate(HitBox, new Vector3(PC.transform.position.x - 2f, 0.15f, PC.transform.position.z), Quaternion.identity);
                HitboxClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                //Enables the box collider
                BoxScript = HitboxClone.GetComponent<TL_FollowChar>();

                //Sets the X and Z offsets for the hitboxes
                BoxScript.X_Pos = -2f;
                BoxScript.Z_Pos = 0f;

                //Assigns the character variable as the PC
                BoxScript.Character = PC;

                //Instantiates the special hitboxes
                for (int i = -1; i < 2; i++)
			    {
				    Special_HitboxClone = Instantiate (Special_HitBox, new Vector3 (PC.transform.position.x + X_Offset, PC.transform.position.y, PC.transform.position.z + i), Quaternion.identity);

                    //Disable the box collider
                    Special_HitboxClone.GetComponent<BoxCollider>().enabled = false;

                    //Obtain the script from the special hitbox
                    BoxScript = Special_HitboxClone.GetComponent<TL_FollowChar>();

                    //Sets the X and Z offsets for the hitboxes
                    BoxScript.X_Pos = X_Offset;
				    BoxScript.Z_Pos = i;

                    //Assigns the character variable as the PC
                    BoxScript.Character = PC;
			    }

                for (int i = -1; i < 2; i++)
                {
                    Special_HitboxClone = Instantiate(Special_HitBox, new Vector3(PC.transform.position.x - X_Offset, PC.transform.position.y, PC.transform.position.z + i), Quaternion.identity);

                    //Disable the box collider
                    Special_HitboxClone.GetComponent<BoxCollider>().enabled = false;

                    //Obtain the script from the special hitbox
                    BoxScript = Special_HitboxClone.GetComponent<TL_FollowChar>();

                    //Sets the X and Z offsets for the hitboxes
                    BoxScript.X_Pos = -X_Offset;
                    BoxScript.Z_Pos = i;

                    //Assigns the character variable as the PC
                    BoxScript.Character = PC;
                }
                break;

		    case "Wu(Clone)":
                //Sets the X and Z offsets
                X_Offset = 0;
			    Z_Offset = 1;

                //Instantiates the hitbox
                Special_HitboxClone = Instantiate(Special_HitBox, new Vector3(PC.transform.position.x, PC.transform.position.y, PC.transform.position.z + Z_Offset), Quaternion.identity);
            
                //Instantiates the special hitbox
                Special_HitboxClone.GetComponent<BoxCollider>().enabled = false;
            
                //Obtain the script from the special hitbox
			    BoxScript = Special_HitboxClone.GetComponent<TL_FollowChar>();

                //Sets the X and Z offsets for the hitboxes
                BoxScript.X_Pos = X_Offset;
			    BoxScript.Z_Pos = Z_Offset;
            
                //Assigns the character variable as the PC
                BoxScript.Character = PC;
			    break;

		    default:
			    break;
		}
        //Finds all of the gameobjects the named tags
        GameObject[] Hitboxes = GameObject.FindGameObjectsWithTag("PC_Hitbox");

        //Add all of the gameobjects into the list
		foreach (GameObject go in Hitboxes)
		{
			AttackHitboxes.Add(go);
		}

        //Finds all of the gameobjects the named tags
        GameObject[] Special_Hitbox = GameObject.FindGameObjectsWithTag("Special");

        //Add all of the gameobjects into the list
        foreach (GameObject go in Special_Hitbox)
		{
			Special_Hitboxes.Add(go);
		}

	}

    public void InitializeNPCHitboxes()
    {
        GameObject Enemy_Hitbox;
        switch (NPC.name)
        {
            case "Enemy1(Clone)":
                //Sets the X and Z offsets
                X_Offset = 0;
                Z_Offset = -1f;

                for (int x = -1; x < 2; x++)
                {
                    //If X is not 0
                    if (x != 0)
                    {
                        //Spawn the enemy hitbox
                        Enemy_Hitbox = Instantiate(Melee_Hitbox, new Vector3(transform.position.x + x, 0f, NPC.transform.position.z + 1f), Quaternion.identity);
                        Enemy_Hitbox.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                        //Obtain the enemy hitbox
                        BoxScript = Enemy_Hitbox.GetComponent<TL_FollowChar>();

                        //Sets the X and Z offsets for the enemy hitboxes
                        BoxScript.X_Pos = x;
                        BoxScript.Z_Pos = Z_Offset + 1f;

                        //Assigns the character variable as the PC
                        BoxScript.Character = NPC;
                    }
                }

                for (int z = -1; z < 2; z++)
                {
                    if (z != 0)
                    {
                        //Spawn the enemy hitbox
                        Enemy_Hitbox = Instantiate(Melee_Hitbox, new Vector3(transform.position.x + X_Offset, 0f, transform.position.z + z), Quaternion.identity);
                        Enemy_Hitbox.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                        //Obtain the enemy hitbox
                        BoxScript = Enemy_Hitbox.GetComponent<TL_FollowChar>();

                        //Sets the X and Z offsets for the enemy hitboxes
                        BoxScript.X_Pos = X_Offset;
                        BoxScript.Z_Pos = z;

                        //Assigns the character variable as the PC
                        BoxScript.Character = NPC;
                    }
                }
                break;
        }
    }

	void UpdateHitboxes()
	{
        //Finds the PC and NPC
		GameObject PC = GameObject.FindGameObjectWithTag("PC");
		GameObject NPC = GameObject.FindGameObjectWithTag("NPC");

        //If the NPC is not dead
        if (NPC != null)
		{
            //Use gameobject array to find gameobjects with the tag
			GameObject[] Enemy_Hitboxes = GameObject.FindGameObjectsWithTag("NPC_Hitbox");

            //Calculate both the X and Z differences between the PC and the NPC
			float X_Difference = Mathf.Round(PC.transform.position.x) - Mathf.Round(NPC.transform.position.x);
			float Z_Difference = Mathf.Round(PC.transform.position.z) - Mathf.Round(NPC.transform.position.z);
            
            //Render the sprite for the hitboxes
            foreach (GameObject go in Enemy_Hitboxes)
            {
                //If the X or Z difference is either 1 or -1
                if (X_Difference == 1f || X_Difference == -1f && Z_Difference == 1f || Z_Difference == -1f)
                {
                    go.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    go.GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            //Render the sprite for the hitboxes
            foreach (GameObject go in AttackHitboxes)
            {
                if (X_Difference == -1f)
                {
                    go.GetComponent<SpriteRenderer>().enabled = true;

                }
                else
                {
                    //Render the sprites null for the hitboxes
                    go.GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            foreach (GameObject go in Enemy_Hitboxes)
            {
                //If the X and Z position of the special hitboxes ever go out of bounds, obtain the script and disable it
                if (go.transform.position.x < 0f || go.transform.position.x > X_Bounds || go.transform.position.z < 0f || go.transform.position.z > Z_Bounds)
                {
                    go.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    //If the X and Z positions of the special hitboxes are within bounds, obtain the script and enable it
                    go.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
		}

        //Add all of the gameobjects into the list
        foreach (GameObject go in Special_Hitboxes)
		{
            //Variable for the script
			TL_FollowChar SpecialBoxScript;

            //If the X and Z position of the special hitboxes ever go out of bounds, obtain the script and disable it
			if (go.transform.position.x < 0f || go.transform.position.x > X_Bounds || go.transform.position.z < 0f || go.transform.position.z > Z_Bounds)
			{
				SpecialBoxScript = go.GetComponent<TL_FollowChar>();
				SpecialBoxScript.enabled = false;
			}
			else
			{
                //If the X and Z positions of the special hitboxes are within bounds, obtain the script and enable it
				SpecialBoxScript = go.GetComponent<TL_FollowChar>();
				SpecialBoxScript.enabled = true;
			}
		}
	}

    void ColliderManager()
    {
        //If the NPC isn't dead
        if (NPC != null)
        {
            //Obtain the script from the NPC
            NPC_Script = NPC.GetComponent<TL_NPCBehaviour>();
        }

        for (int x = 0; x < LevelMovement.GetLength(0); x++)
        {
            for (int z = 0; z < LevelMovement.GetLength(1); z++)
            {
                //Variable for the box collider for later use
                BoxCollider BoxCol;

                //Rounds up the X value in the for loop and Z value in the PC position and calculates both the differences of X and Z
                float X_Difference = Mathf.Round(LevelMovement[x, z].transform.position.x) - Mathf.Round(PC.transform.position.x);
                float Z_Difference = Mathf.Round(LevelMovement[x, z].transform.position.z) - Mathf.Round(PC.transform.position.z);

                //If the differences of X and Z are either -1 or 1 which makes it horizontal for X and vertical for Z
                if (X_Difference == -1f && Z_Difference == 0f || X_Difference == 1f && Z_Difference == 0f || X_Difference == 0f && Z_Difference == -1f || X_Difference == 0f && Z_Difference == 1f)
                {
                    //Obtain the component from the gameobject array and round off the x and z
                    BoxCol = LevelMovement[x, z].GetComponent<BoxCollider>();

                    //Enable the collider
                    BoxCol.enabled = true;
                }
                else
                {
                    //Obtain the component from the gameobject array and round off the x and z
                    BoxCol = LevelMovement[x, z].GetComponent<BoxCollider>();

                    //Enable the collider
                    BoxCol.enabled = false;
                }

            }

        }

    }

}
