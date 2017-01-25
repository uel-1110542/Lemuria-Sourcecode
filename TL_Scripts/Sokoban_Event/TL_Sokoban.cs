using UnityEngine;
using System.Collections.Generic;

public class TL_Sokoban : MonoBehaviour {

    //Variables
    public GameObject MovableObj;
    public GameObject GoalPoint;
    private bool[] AllPlacedCorrectly = new bool[3];

    //Vector3 List
    public List<Vector3> GoalPos = new List<Vector3>();

    //Int variables
    private int Prev_Num;
    private int Ran_X;
    private int Ran_Z;

    //Scripts
    private TL_GridManager GridScript;


    void Start()
    {
        //Obtains the scripts from this gameobject
        GridScript = GetComponent<TL_GridManager>();
        TL_SpawnPC PCSpawnScript = GetComponent<TL_SpawnPC>();

        //Spawns the PC based on a string in the serialized class
        PCSpawnScript.SpawnPC(GridScript.WorldMovementScript.WorldMapDataClass.PC_Name);
        
        //Spawn platforms
        CreatePlatforms();

        for (int x = 0; x < GridScript.ReturnLevelAreaArray().GetLength(0); x++)
        {
            for (int z = 0; z < GridScript.ReturnLevelAreaArray().GetLength(1); z++)
            {
                //Create a border of walls
                if (x < 1 || x > 12 || z < 1 || z > 6)
                {
                    GridScript.SpawnGameObject(GridScript.Wall, x, 0f, z);
                }
            }
        }
        //Create the level
        CreateLevel();

        //Spawn the goal points
        SpawnGoalPoints();

        //Spawn the movable objects
        SpawnMovableObjs();
    }

    void Update()
    {
        CheckGoalPlacement();
    }

    void CheckGoalPlacement()
    {
        //Find all of the goal points in the scene
        GameObject[] GoalPoints = GameObject.FindGameObjectsWithTag("Goal");
                
        for (int i = 0; i < GoalPoints.GetLength(0); i++)
        {
            //Obtain the script from all of the goal points
            TL_CheckPlacement GoalScript = GoalPoints[i].GetComponent<TL_CheckPlacement>();

            //Set the bool to the bool from the script
            AllPlacedCorrectly[i] = GoalScript.CorrectPlace;
        }

        //If all of the bools from the array are true
        if (AllPlacedCorrectly[0] && AllPlacedCorrectly[1] && AllPlacedCorrectly[2])
        {
            //Obtain the timer script from this gameobject
            TL_Timer TimerScript = GetComponent<TL_Timer>();

            //Activate the win condition
            TimerScript.CheckCondition(true);

            //Finds the level manager to access the function
            GameObject.Find("Level_Manager(Clone)").GetComponent<DN_Event_Manager>().WinState("Sokoban");
        }
    }

    void CreatePlatforms()
    {
        for (int x = 0; x < GridScript.ReturnLevelAreaArray().GetLength(0); x++)
        {
            for (int z = 0; z < GridScript.ReturnLevelAreaArray().GetLength(1); z++)
            {
                //Spawn the platform
                GameObject PlatformClone = (GameObject) Instantiate(GridScript.Platform, new Vector3(x, 0f, z), Quaternion.identity);

                //Set the rotation
                PlatformClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                //Change the sprite of the gameobject accordingly to the region selected
                GridScript.SetBiomeSprites(PlatformClone, GridScript.WorldMovementScript.WorldMapDataClass.Region_Name);
            }

        }

    }

    void CreateLevel()
    {
        for (int x = 2; x < GridScript.ReturnLevelAreaArray().GetLength(0); x += 3)
        {
            for (int z = 2; z < GridScript.ReturnLevelAreaArray().GetLength(1); z += 3)
            {
                //Randomize a value with the function
                int Ran_Int = ReRandomize(6);

                //Choose a template
                ChooseTemplate(Ran_Int, x, z);
            }
        }
    }

    int ReRandomize(int ran_num)
    {
        int Ran_Num = Random.Range(1, ran_num);

        //While the random number is the same as the previous random number, re-randomize it
        while (Ran_Num == Prev_Num)
        {
            Ran_Num = Random.Range(1, ran_num);
        }

        //Assign the variable to the random number
        Prev_Num = Ran_Num;
        return Prev_Num;
    }

    void ChooseTemplate(int choice, int x, int z)
    {
        switch (choice)
        {
            //3x3 Box Template with a wall in the middle
            case 1:
                GridScript.SpawnGameObject(GridScript.Wall, x, 0f, z);
                break;

            //3x3 Box Template with a wall in the top-left
            case 2:
                GridScript.SpawnGameObject(GridScript.Wall, x - 1, 0f, z + 1);
                break;

            //3x3 Box Template with a wall in the top-right
            case 3:
                GridScript.SpawnGameObject(GridScript.Wall, x + 1, 0f, z + 1);
                break;

            //3x3 Box Template with a wall in the bottom-left
            case 4:
                GridScript.SpawnGameObject(GridScript.Wall, x - 1, 0f, z - 1);
                break;

            //3x3 Box Template without a wall in the bottom-right
            case 5:
                GridScript.SpawnGameObject(GridScript.Wall, x + 1, 0f, z - 1);
                break;
        }
        
    }

    void SpawnGoalPoints()
    {
        //While there are less than 3 goal points
        while (GameObject.FindGameObjectsWithTag("Goal").GetLength(0) < 3)
        {
            //Randomize the values based on the length of the array
            Ran_X = ReRandomize(GridScript.ReturnLevelAreaArray().GetLength(0) - 1);
            Ran_Z = ReRandomize(GridScript.ReturnLevelAreaArray().GetLength(1) - 1);

            //If the randomed values are within the level area
            if (Ran_X - 1 > 0 || Ran_X + 1 < GridScript.ReturnLevelAreaArray().GetLength(0) - 1 || Ran_Z - 1 > 0 || Ran_Z + 1 < GridScript.ReturnLevelAreaArray().GetLength(1) - 1)
            {
                //If the grid space in the array is clear in a cross shaped pattern in x 3x3
                if (GridScript.ReturnLevelAreaArray()[Ran_X + 1, Ran_Z] == null && GridScript.ReturnLevelAreaArray()[Ran_X - 1, Ran_Z] == null && GridScript.ReturnLevelAreaArray()[Ran_X, Ran_Z] == null && GridScript.ReturnLevelAreaArray()[Ran_X, Ran_Z + 1] == null && GridScript.ReturnLevelAreaArray()[Ran_X, Ran_Z - 1] == null)
                {
                    //If the list doesn't contain this position
                    if (!GoalPos.Contains(new Vector3 (Ran_X, 0f, Ran_Z)))
                    {
                        //Spawn the goal point
                        GameObject GoalClone = (GameObject)Instantiate(GoalPoint, new Vector3(Ran_X, 0f, Ran_Z), Quaternion.identity);

                        //Set the rotation
                        GoalClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                        //Add the position to the list
                        GoalPos.Add(GoalClone.transform.position);
                    }
                }
            }
        }
    }

    void SpawnMovableObjs()
    {
        //While there are less than 3 movable objects
        while (GameObject.FindGameObjectsWithTag("MovableObj").GetLength(0) < 3)
        {
            //Randomize the values based on the length of the array
            Ran_X = ReRandomize(GridScript.ReturnLevelAreaArray().GetLength(0) - 1);
            Ran_Z = ReRandomize(GridScript.ReturnLevelAreaArray().GetLength(1) - 1);

            //If the randomed values are within the level area
            if (Ran_X - 1 > 0 || Ran_X + 1 < GridScript.ReturnLevelAreaArray().GetLength(0) - 1 || Ran_Z - 1 > 0 || Ran_Z + 1 < GridScript.ReturnLevelAreaArray().GetLength(1) - 1)
            {
                //If the grid space in the array is clear in a cross-shaped pattern in a 3x3
                if (GridScript.ReturnLevelAreaArray()[Ran_X, Ran_Z] == null && GridScript.ReturnLevelAreaArray()[Ran_X + 1, Ran_Z] == null && GridScript.ReturnLevelAreaArray()[Ran_X - 1, Ran_Z] == null && GridScript.ReturnLevelAreaArray()[Ran_X, Ran_Z + 1] == null && GridScript.ReturnLevelAreaArray()[Ran_X, Ran_Z - 1] == null)
                {
                    //If the list doesn't contain this position or the starting position
                    if (!GoalPos.Contains(new Vector3(Ran_X, 0f, Ran_Z)) && Ran_X != 6 && Ran_Z != 2)
                    {
                        //Spawn the movable object
                        GameObject MovableObjClone = (GameObject)Instantiate(MovableObj, new Vector3(Ran_X, 0.5f, Ran_Z), Quaternion.identity);

                        //Set the spawned gameobject in the 2D gameobject array
                        GridScript.SetGOInLevelArea(MovableObjClone, Ran_X, Ran_Z);
                    }                        
                }
            }
        }
    }

}
