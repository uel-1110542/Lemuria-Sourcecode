using UnityEngine;

public class TL_SpikeEvent : MonoBehaviour {

    //Variables
    public GameObject Collider;
    public GameObject Spike;

    public bool EventStart = false;
    public int NPC_XPos;
    public int NPC_ZPos;

    private GameObject PC;
    private int BacktrackedIndex;
    private int RandomIndex;
    private TL_CharStats CharacterScript;
    private TL_MoveScript MoveScript;
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

        PC = GameObject.FindGameObjectWithTag("PC");

        //Obtain the script for the level grid
        GridScript = GetComponent<TL_GridManager>();

        //Function for initializing the layout of the level
        InitializeLayout();
    }

    void Update()
    {
        Escape();
    }

    //Cheat Function
    void Escape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PC = GameObject.FindGameObjectWithTag("PC");
            MoveScript = PC.GetComponent<TL_MoveScript>();
            MoveScript.enabled = false;
            PC.transform.position = new Vector3(14f, 0.1f, 4f);
        }
    }

    void InitializeLayout()
    {
        //Creates walls in the maze
        CreateWalls();

        //The depth first search hollows out the walls in the grid to form a maze
        DepthFirstSearch(0, 4);

        //PC position
        GridScript.ReturnLevelAreaArray()[0, 4] = PC;

        //Exit position        
        GridScript.SpawnGameObject(GridScript.Platform, 14, 0, 4);

        //Set color to the exit platform
        Color ExitColor = Color.cyan;
        GridScript.ReturnGOInLevelArea(14, 4).GetComponent<SpriteRenderer>().material.color = ExitColor;

        //Add the box collider onto the exit platform
        BoxCollider ExitCollider = GridScript.ReturnGOInLevelArea(14, 4).AddComponent<BoxCollider>();

        //Set default size and center
        ExitCollider.size = new Vector3(1f, 1f, 0.5f);
        ExitCollider.center = new Vector3(0f, 0f, -0.25f);

        //Set the trigger
        ExitCollider.isTrigger = true;

        //Add the event complete script
        GridScript.ReturnGOInLevelArea(14, 4).AddComponent<TL_EventComplete>();

        //Ignore raycasting for this gameobject
        GridScript.ReturnGOInLevelArea(14, 4).layer = 2;

        //Function for generating spikes
        GenerateSpikes();

        //Start the event
        EventStart = true;
    }

    //Function for spawning object into the scene and into the gameobject array
    void SpawnObjects(GameObject go, int x, float y, int z)
    {
        GridScript.ReturnLevelAreaArray()[x, z] = (GameObject)Instantiate(go, new Vector3(x, y, z), Quaternion.identity);

        //If the gameobject name in the array is the spike then saet initial rotation
        if (GridScript.ReturnLevelAreaArray()[x, z].name == "Spikes(Clone)")
        {
            GridScript.ReturnLevelAreaArray()[x, z].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }
    }

    void CreateWalls()
    {
        for (int x = 0; x < GridScript.ReturnLevelAreaArray().GetLength(0); x++)
        {
            for (int z = 0; z < GridScript.ReturnLevelAreaArray().GetLength(1); z++)
            {
                //If the x and z positions are on the starting position then skip the iteration
                if (x == 0 && z == 4)
                {
                    continue;
                }
                else
                {
                    //If not then spawn the wall
                    GridScript.SpawnGameObject(GridScript.Wall, x, 0, z);
                }
            }
        }
    }

    void DepthFirstSearch(int x, int z)
    {
        for (int j = 0; j < 8; j++)
        {
            //Variable for random directions
            int Ran_Dir = Random.Range(1, 5);
            
            //Switch case statement to check which of the 4 directions the algorithm will take
            switch (Ran_Dir)
            {
                //Up Direction
                case 1:
                    //If the next checked direction goes outside the boundaries of the array then skip the iteration
                    if (z + 2 > GridScript.ReturnLevelAreaArray().GetLength(1) - 1)
                    {
                        continue;
                    }
                    //If the next checked direction has a wall
                    else if (GridScript.ReturnLevelAreaArray()[x, z + 2] != null)
                    {
                        //Destroy the walls with a for loop
                        for (int i = 1; i < 3; i++)
                        {
                            //If the for loop goes outside the array length then break the loop
                            if (z + i > GridScript.ReturnLevelAreaArray().GetLength(1) - 1)
                            {
                                break;
                            }
                            else
                            {
                                //Destroy the walls and set it to null in the gameobject array
                                Destroy(GridScript.ReturnLevelAreaArray()[x, z + i]);
                                GridScript.ReturnLevelAreaArray()[x, z + i] = null;
                            }
                        }
                        //Recall the function again
                        DepthFirstSearch(x, z + 2);
                    }
                    //If the adjacent directions checked is within the boundaries of the array
                    else if (x - 1 > 0 && x + 1 < GridScript.ReturnLevelAreaArray().GetLength(0) - 1 && z - 1 > 0 && z + 1 < GridScript.ReturnLevelAreaArray().GetLength(1) - 1)
                    {
                        //If the left, top and right walls are detected
                        if (GridScript.ReturnLevelAreaArray()[x - 1, z] != null && GridScript.ReturnLevelAreaArray()[x, z + 1] != null && GridScript.ReturnLevelAreaArray()[x + 1, z] != null)
                        {
                            //Destroy the walls and set it to null in the gameobject array
                            Destroy(GridScript.ReturnLevelAreaArray()[x, z + 1]);
                            GridScript.ReturnLevelAreaArray()[x, z + 1] = null;
                        }
                    }
                    break;

                //Right Direction
                case 2:
                    //If the next checked direction goes outside the boundaries of the array then skip the iteration
                    if (x + 2 > GridScript.ReturnLevelAreaArray().GetLength(0) - 1)
                    {
                        continue;
                    }
                    else if (GridScript.ReturnLevelAreaArray()[x + 2, z] != null)
                    {
                        //Destroy the walls with a for loop
                        for (int i = 1; i < 3; i++)
                        {
                            //If the for loop goes outside the array length then break the loop
                            if (x + i > GridScript.ReturnLevelAreaArray().GetLength(0) - 1)
                            {
                                break;
                            }
                            else
                            {
                                //Destroy the walls and set it to null in the gameobject array
                                Destroy(GridScript.ReturnLevelAreaArray()[x + i, z]);
                                GridScript.ReturnLevelAreaArray()[x + i, z] = null;
                            }
                        }
                        //Recall the function again
                        DepthFirstSearch(x + 2, z);
                    }
                    //If the adjacent directions checked is within the boundaries of the array
                    else if (x - 1 > 0 && x + 1 < GridScript.ReturnLevelAreaArray().GetLength(0) - 1 && z - 1 > 0 && z + 1 < GridScript.ReturnLevelAreaArray().GetLength(1) - 1)
                    {
                        //If the top, right and bottom walls are detected
                        if (GridScript.ReturnLevelAreaArray()[x, z + 1] != null && GridScript.ReturnLevelAreaArray()[x + 1, z] != null && GridScript.ReturnLevelAreaArray()[x, z - 1] != null)
                        {
                            //Destroy the walls and set it to null in the gameobject array
                            Destroy(GridScript.ReturnLevelAreaArray()[x + 1, z]);
                            GridScript.ReturnLevelAreaArray()[x + 1, z] = null;
                        }
                    }
                    break;

                //Down Direction
                case 3:
                    //If the next checked direction goes outside the boundaries of the array then skip the iteration
                    if (z - 2 < 0)
                    {
                        continue;
                    }
                    else if (GridScript.ReturnLevelAreaArray()[x, z - 2] != null)
                    {
                        //Destroy the walls with a for loop
                        for (int i = 1; i < 3; i++)
                        {
                            //If the for loop goes outside the array length then break the loop
                            if (z - i < 0)
                            {
                                break;
                            }
                            else
                            {
                                //Destroy the walls and set it to null in the gameobject array
                                Destroy(GridScript.ReturnLevelAreaArray()[x, z - i]);
                                GridScript.ReturnLevelAreaArray()[x, z - i] = null;
                            }
                        }
                        //Recall the function again
                        DepthFirstSearch(x, z - 2);
                    }
                    //If the adjacent directions checked is within the boundaries of the array
                    else if (x - 1 > 0 && x + 1 < GridScript.ReturnLevelAreaArray().GetLength(0) - 1 && z - 1 > 0 && z + 1 < GridScript.ReturnLevelAreaArray().GetLength(1) - 1)
                    {
                        //If the left, down and right walls are detected
                        if (GridScript.ReturnLevelAreaArray()[x - 1, z] != null && GridScript.ReturnLevelAreaArray()[x, z - 1] != null && GridScript.ReturnLevelAreaArray()[x + 1, z] != null)
                        {
                            //Destroy the walls and set it to null in the gameobject array
                            Destroy(GridScript.ReturnLevelAreaArray()[x, z - 1]);
                            GridScript.ReturnLevelAreaArray()[x, z - 1] = null;
                        }
                    }
                    break;

                //Left Direction
                case 4:
                    //If the next checked direction goes outside the boundaries of the array then skip the iteration
                    if (x - 2 < 0)
                    {
                        continue;
                    }
                    else if (GridScript.ReturnLevelAreaArray()[x - 2, z] != null)
                    {
                        //Destroy the walls with a for loop
                        for (int i = 1; i < 3; i++)
                        {
                            //If the for loop goes outside the array length then break the loop
                            if (x - i < 0)
                            {
                                break;
                            }
                            else
                            {
                                //Destroy the walls and set it to null in the gameobject array
                                Destroy(GridScript.ReturnLevelAreaArray()[x - i, z]);
                                GridScript.ReturnLevelAreaArray()[x - i, z] = null;
                            }
                        }
                        //Recall the function again
                        DepthFirstSearch(x - 2, z);
                    }
                    //If the adjacent directions checked is within the boundaries of the array
                    else if (x - 1 > 0 && x + 1 < GridScript.ReturnLevelAreaArray().GetLength(0) - 1 && z - 1 > 0 && z + 1 < GridScript.ReturnLevelAreaArray().GetLength(1) - 1)
                    {
                        //If the down, left and top walls are detected
                        if (GridScript.ReturnLevelAreaArray()[x, z - 1] != null && GridScript.ReturnLevelAreaArray()[x - 1, z] != null && GridScript.ReturnLevelAreaArray()[x, z + 1] != null)
                        {
                            //Destroy the walls and set it to null in the gameobject array
                            Destroy(GridScript.ReturnLevelAreaArray()[x - 1, z]);
                            GridScript.ReturnLevelAreaArray()[x - 1, z] = null;
                        }
                    }
                    break;
            }
        }
    }

    void GenerateSpikes()
    {
        for (int x = 0; x < GridScript.ReturnLevelAreaArray().GetLength(0); x++)
        {
            for (int z = 0; z < GridScript.ReturnLevelAreaArray().GetLength(1); z++)
            {
                GameObject PlatformClone;

                //Variable for adding a random Z position
                int Ran_Chance = Random.Range(0, 4);

                //If the divided number returns no remainder
                if (Ran_Chance % 3 == 0)
                {
                    //If the Z position in the gameobject array is null and it is not the spawning point of the PC or the exit
                    if (GridScript.ReturnLevelAreaArray()[x, z] == null)
                    {
                        //Spawns spikes
                        SpawnObjects(Spike, x, 0f, z);

                        //Spawning the platform beneath the spike
                        PlatformClone = (GameObject)Instantiate(GridScript.Platform, new Vector3(x, 0f, z), Quaternion.identity);

                        //Set default rotation
                        PlatformClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                        //Set the appropriate biome sprites
                        GridScript.SetBiomeSprites(PlatformClone, WorldMovementScript.WorldMapDataClass.Region_Name);
                    }
                }

                //If the X and Z from the for loops return the PC or null
                if (GridScript.ReturnLevelAreaArray()[x, z] == PC || GridScript.ReturnLevelAreaArray()[x, z] == null)
                {
                    //Spawn the platform
                    PlatformClone = (GameObject)Instantiate(GridScript.Platform, new Vector3(x, 0f, z), Quaternion.identity);

                    //Set default rotation
                    PlatformClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                    //Set the appropriate biome sprites
                    GridScript.SetBiomeSprites(PlatformClone, WorldMovementScript.WorldMapDataClass.Region_Name);
                }
            }
        }
    }

    int Randomizer(int ran_num)
    {
        //Randomizes the number
        int Ran_Num = Random.Range(0, ran_num);

        //While the randomed number is still the same as the previously randomed number
        while (Ran_Num == RandomIndex)
        {
            //Re-randomize the number
            Ran_Num = Random.Range(0, ran_num);
        }
        //Set randomized number to previous number
        RandomIndex = Ran_Num;

        //Return the value
        return RandomIndex;
    }

}
