using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TL_PC_World_Movement : MonoBehaviour {

    //Serializable Class
    [System.Serializable]
    public class SerializeWorldMovement
    {
        //String variables to hold the PC's and region's name
        public string PC_Name;
        public string Region_Name;

        //Float variables to keep track of the PC's position in the world
        public float World_X_Pos;
        public float World_Y_Pos;

        //Booleans to keep track of world map completion
        public bool ForestCompleted;
        public bool DesertCompleted;
        public bool MountainsCompleted;
        public bool PlainsCompleted;
    }
    public SerializeWorldMovement WorldMapDataClass = new SerializeWorldMovement();

    //Vector3 variable for selecting the PC's destination
    public Vector2 PC_Destination = Vector2.zero;

    //Variable to locate the PC
    public GameObject PC;

    //Boolean for checking if the player has returned from the level map or not
    public bool ReturnedFromLevel = false;

    //Boolean for checking if the PC is moving or not
    private bool PC_Moving = false;

    private TL_RaycastSelection SelectionScript;


    void Awake()
    {
        //Locates the global game manager and obtains the script
        TL_GlobalGameManager GlobalGameManagerScript = GameObject.Find("Global_GameManager").GetComponent<TL_GlobalGameManager>();

        //Obtain the script from this gameobject
        SelectionScript = GetComponent<TL_RaycastSelection>();

        //Find the PC
        PC = GameObject.FindGameObjectWithTag("PC");

        //If the boolean is true
        if (!GlobalGameManagerScript.LoadingFlag)
        {
            if (PC != null)
            {
                //Don't destroy the PC on load
                DontDestroyOnLoad(PC);

                //Set both the X and Z PC positions to the float variables in the serialized class
                WorldMapDataClass.World_X_Pos = PC.transform.position.x;
                WorldMapDataClass.World_Y_Pos = PC.transform.position.y;

                //Set the string variable to the playerprefs in the serialized class
                WorldMapDataClass.PC_Name = PlayerPrefs.GetString("PC Name");

                //Remove all playerprefs
                PlayerPrefs.DeleteAll();
            }

        }

    }

	void Update()
    {
        //If the active scene is the world map, allow these functions to persist
        if (SceneManager.GetActiveScene().name == "World_Map")
        {
            //Locate the PC with the tag
            PC = GameObject.FindGameObjectWithTag("PC");
            if (!ReturnedFromLevel)
            {
                SelectNode();
                MovePC();
            }
            else
            {
                //If the PC is null
                if (PC == null)
                {
                    //Finds the script on this gameobject and obtains the component
                    TL_SpawnPC SpawnPCScript = gameObject.GetComponent<TL_SpawnPC>();

                    //Spawn the PC
                    PC = SpawnPCScript.SpawnPC(WorldMapDataClass.PC_Name);

                    //Adjust the PC's position to the X and Z position that has been saved in the serialized class
					PC.transform.position = new Vector3(WorldMapDataClass.World_X_Pos, WorldMapDataClass.World_Y_Pos, -1f);
                }
                else
                {
                    //If the PC can be found then reactivate the PC
                    PC.SetActive(true);
                }
                //Set bool to true
                ReturnedFromLevel = false;
            }
        }
    }

    public void SaveWorldMapData()
    {
        //Variable for the binary formatter
        BinaryFormatter BinaryFormat = new BinaryFormatter();

        //Create a file stream in the specified directory with a file name
        FileStream FileStream = File.Create(Application.persistentDataPath + "/SavedWorldMapData.sg");

        //Serialize the class
        BinaryFormat.Serialize(FileStream, WorldMapDataClass);

        //Close the stream
        FileStream.Close();
    }

    public void LoadWorldMapData()
    {
        //If the file exists in that directory
        if (File.Exists(Application.persistentDataPath + "/SavedWorldMapData.sg"))
        {
            //Variable for the binary formatter
            BinaryFormatter BinaryFormat = new BinaryFormatter();

            //Open the file from the directory
            FileStream FileStream = File.Open(Application.persistentDataPath + "/SavedWorldMapData.sg", FileMode.Open);

            //Deserialize the class
            WorldMapDataClass = (SerializeWorldMovement)BinaryFormat.Deserialize(FileStream);

            //Close the stream
            FileStream.Close();
        }
    }

    void SelectNode()
    {
        if (SelectionScript.ReturnSelectedGO() != null && SelectionScript.ReturnSelectedGO().transform.tag == "Region_Node")
        {
            //Sets the region's name to a string from a serialized class
            WorldMapDataClass.Region_Name = SelectionScript.ReturnSelectedGO().transform.name;

            //Set the destination of the PC to the region node's position
			switch(SelectionScript.ReturnSelectedGO().name)
			{
			case "PlainsNode(Clone)":
				PC_Destination = new Vector2(-1.67f, 4.26f);
				break;

			case "ForestNode(Clone)":
				PC_Destination = new Vector2(4.88f, 2f);
				break;

			case "DesertNode(Clone)":
				PC_Destination = new Vector2(3f, 5f);
				break;

			case "MountainNode(Clone)":
				PC_Destination = new Vector2(2.63f, 7.45f);
				break;
			}
            //Set PC moving to true
            PC_Moving = true;

        }

    }

    void MovePC()
    {
        //While the boolean is true
        if (PC_Moving)
        {
            //If the distance between itself and the destination is not smaller than the minimum value
            if (Vector3.Distance(PC_Destination, PC.transform.position) < 0.05f)
            {
                //Set the boolean to false
                PC_Moving = false;

                //Make the PC's position equal to the destination
				PC.transform.position = new Vector3(PC_Destination.x, PC_Destination.y, -1f);

                //Set both the X and Z PC positions to the float variables in the serialized class
                WorldMapDataClass.World_X_Pos = PC.transform.position.x;
                WorldMapDataClass.World_Y_Pos = PC.transform.position.y;
                
                //Load the level map
                SceneManager.LoadScene("Level_Map");

                PC.SetActive(false);
            }
            else
            {
                //Animate the PC moving to its' destination
				PC.transform.position = Vector3.Lerp(PC.transform.position, PC_Destination, 0.05f);
            }
        }

    }

}
