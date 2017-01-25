using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DN_Node_Map_Generation : MonoBehaviour 
{
    //Initializes the size of the array
    public int level_array_x = 5;
    public int level_array_y = 5;

    //Sets the starting position of the PC
    public int start_x = 3;
    public int start_y = 0;

    //Amount of node types
    public int num_danger = 3;
    public int num_story = 3;
    public int num_social = 3;
    public int num_real_danger = 3;
    public int num_safe = 3;
    public int num_mystery = 3;

    //Variables that handle randomization
    public int rng_spawn_chance = 20;
    public float rng_offset_1 = -0.5f;
    public float rng_offset_2 = 1f;

    //Serializable class
    [System.Serializable]
    public class SerializeNodeMapData
    {
        //2D int array to hold the type of node in the coresponding column and row
        public int[,] Node_Placement;

        //Contains the node columns
        public float[,] Node_XPos;

        //Contains the node rows
        public float[,] Node_ZPos;
    }
    public SerializeNodeMapData NodeMapData = new SerializeNodeMapData();
    
    public GameObject Player_Char;
    private GameObject pc;
    private GameObject pc_cur_node;

    public GameObject Collect_Manager;
    public GameObject Region_Manager;

    public GameObject node;
    private GameObject end_node;

	private int Prev_Number;
    private bool spawn_map = true;
    private GameObject[,] node_array;
    private List<GameObject> OpenList = new List<GameObject>();
    private List<GameObject> ClosedList = new List<GameObject>();
    private DN_PC_Level_Movement LevelMovementData;
    private TL_PC_World_Movement WorldMovementScript;
    private TL_SpawnPC PCSpawnScript;


    // Use this for initialization
    void Awake()
    {
        //Prevent destruction on changing scenes
        DontDestroyOnLoad(this);

        //Locates the global game manager and obtains the script
        TL_GlobalGameManager GlobalGameManagerScript = GameObject.Find("Global_GameManager").GetComponent<TL_GlobalGameManager>();

        //Finds the game manager and obtains the script
        WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

        //If the loading flag is on then load the data from their respective files
        if (GlobalGameManagerScript.LoadingFlag)
        {
            //Obtain the script from the gameobject
            LevelMovementData = GetComponent<DN_PC_Level_Movement>();
            
            //Loads the progress the player has before from the file
            LevelMovementData.LoadProgress();

            //Loads the file
            LoadSession();

            //Display the node map from the loaded file
            DisplayNodeMap();

            //Loads the node properties from the file
            LoadProperties();

            //Obtains the component from this gameobject
            PCSpawnScript = gameObject.GetComponent<TL_SpawnPC>();

            //Spawns PC based on the string in the serialized class
            PCSpawnScript.SpawnPC(WorldMovementScript.WorldMapDataClass.PC_Name);

            //Finds the gameobject based on the tag
            pc = GameObject.FindGameObjectWithTag("PC");

            //Reposition the PC based on the floats in the serialized class
            pc.transform.position = new Vector3(LevelMovementData.LevelMovementClass.X_Pos, 0.5f, LevelMovementData.LevelMovementClass.Z_Pos);

            //Search through all of the nodes in the scene and loop it with a foreach loop
            GameObject[] Nodes = GameObject.FindGameObjectsWithTag("Node");
            foreach (GameObject go in Nodes)
            {
                //If the PC's position is equal to the node then make the gameobject variable equal to that node
                if (go.transform.position.x == pc.transform.position.x && go.transform.position.z == pc.transform.position.z)
                {
                    pc_cur_node = go;
                    break;
                }
            }
            //Turn off booleans
            spawn_map = false;
            GlobalGameManagerScript.LoadingFlag = false;
        }
        else if (spawn_map && !GlobalGameManagerScript.LoadingFlag)
        {
            //inital setup of node map generation
            RestartNodeMap();

            //Fills the node placement array with zeros as default values
            for (int a = 0; a < level_array_x; a++)
            {
                for (int b = 0; b < level_array_y; b++)
                {
                    NodeMapData.Node_Placement[a, b] = 0;
                }
            }

            //start node
            NodeMapData.Node_Placement[start_x, start_y] = 0;

            //Put the node at the starting position
            node_array[start_x, start_y] = node;

            //Spawn the start node
            Spawn_AddNode(node, start_x, start_y);

            //Add the start node into the open list
            OpenList.Add(node_array[start_x, start_y]);

            //Send a message to the start node
            node_array[start_x, start_y].SendMessage("SetNodeType", "Start", SendMessageOptions.DontRequireReceiver);

            GameObject PC = GameObject.FindGameObjectWithTag("PC");
            PCSpawnScript = gameObject.GetComponent<TL_SpawnPC>();
            if (PC == null)
            {
                WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

                //inital pc pos
                pc = PCSpawnScript.SpawnPC(WorldMovementScript.WorldMapDataClass.PC_Name);
            }
            else
            {
                pc = PC;
            }

            float x = node_array[start_x, start_y].transform.position.x;
            float z = node_array[start_x, start_y].transform.position.z;
            pc.transform.position = new Vector3(x, 0.5f, z);
            pc_cur_node = node_array[start_x, start_y];

            //map generation method
            GenerateNode();
            RenderPaths();

            //Send messages to the start node to reveal the node connections and render the start node
            pc_cur_node.SendMessage("Reveal_Connection", SendMessageOptions.DontRequireReceiver);
            pc_cur_node.SendMessage("RenderNodes", SendMessageOptions.DontRequireReceiver);

            //assign the end note            
            end_node = ClosedList[ClosedList.Count - 1];
            end_node.name = "End_Node";
            end_node.GetComponent<DN_Node_Properties>().NodePropertiesClass.event_done = false;

            //searching the array for the node we want
            for (int col = 0; col < node_array.GetLength(0); col++)
            {
                for (int row = 0; row < node_array.GetLength(1); row++)
                {
                    if (node_array[col, row] == ClosedList[ClosedList.Count - 1])
                    {
                        NodeMapData.Node_Placement[col, row] = 7;
                        node_array[col, row].SendMessage("SetNodeType", "End", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            AssignNodeTypes();
            spawn_map = false;
        }
        
    }

    public void RestartNodeMap()
    {
        node_array = new GameObject[level_array_x, level_array_y];
        NodeMapData.Node_Placement = new int[level_array_x, level_array_y];
        NodeMapData.Node_XPos = new float[level_array_x, level_array_y];
        NodeMapData.Node_ZPos = new float[level_array_x, level_array_y];
    }

    public void SaveSession()
    {
        //New instance for the binary formatter
        BinaryFormatter BinaryFormat = new BinaryFormatter();

        //Creates a file in the directory specified
        FileStream FileStream = File.Create(Application.persistentDataPath + "/SavedNodeMap.sg");

        //Serializes the class
        BinaryFormat.Serialize(FileStream, NodeMapData);

        //Closes the stream
        FileStream.Close();
    }

    public void LoadSession()
    {
        //If the file exists in the specified directory
        if (File.Exists(Application.persistentDataPath + "/SavedNodeMap.sg"))
        {
            //New instance for the binary formatter
            BinaryFormatter BinaryFormat = new BinaryFormatter();

            //Opens the file in the directory
            FileStream FileStream = File.Open(Application.persistentDataPath + "/SavedNodeMap.sg", FileMode.Open);

            //Deserializes the class into the constructor
            NodeMapData = (SerializeNodeMapData) BinaryFormat.Deserialize(FileStream);

            //Closes the stream
            FileStream.Close();
        }
    }

    public void SaveProperties()
    {
        for (int col = 0; col < level_array_x; col++)
        {
            for (int row = 0; row < level_array_y; row++)
            {
                if (node_array[col, row] != null)
                {
                    //Obtains the script from the node in the array
                    DN_Node_Properties NodeProperties = node_array[col, row].GetComponent<DN_Node_Properties>();

                    //New constructor for the binary formatter
                    BinaryFormatter BinaryFormat = new BinaryFormatter();

                    //Saves the file based on the col and row
                    FileStream FileStream = File.Create(Application.persistentDataPath + "/SavedNodeProperties " + col + "-" + row + ".sg");

                    //Serializes the file into a binary format
                    BinaryFormat.Serialize(FileStream, NodeProperties.NodePropertiesClass);

                    //Closes the file stream
                    FileStream.Close();
                }
            }
        }        
    }

    public void LoadProperties()
    {        
        for (int col = 0; col < node_array.GetLength(0); col++)
        {
            for (int row = 0; row < node_array.GetLength(1); row++)
            {
                //If the node exists in the array
                if (node_array[col, row] != null)
                {
                    //If the file exists in that directory depending on the col and row
                    if (File.Exists(Application.persistentDataPath + "/SavedNodeProperties " + col + "-" + row + ".sg"))
                    {
                        //Obtains the script from the node in the array
                        DN_Node_Properties NodeProperties = node_array[col, row].GetComponent<DN_Node_Properties>();

                        //New constructor for the binary formatter
                        BinaryFormatter BinaryFormat = new BinaryFormatter();

                        //Opens the file from the directory and depending on the col and row
                        FileStream FileStream = File.Open(Application.persistentDataPath + "/SavedNodeProperties " + col + "-" + row + ".sg", FileMode.Open);

                        //Deserializes the file into the node properties class
                        NodeProperties.NodePropertiesClass = (DN_Node_Properties.SerializeNodeProperties)BinaryFormat.Deserialize(FileStream);

                        //Closes the file stream
                        FileStream.Close();
                    }
                }
            }
        }

		for (int col = 0; col < node_array.GetLength(0); col++)
		{
			for (int row = 0; row < node_array.GetLength(1); row++)
			{
				if (node_array[col, row] != null)
				{
                    //Input the col and row for the current node being loaded
                    RecreateConnections(col, row);
                }
			}
		}
        RenderPaths();

    }

    void DisplayNodeMap()
    {
        //Recreates the node array
        node_array = new GameObject[level_array_x, level_array_y];

        for (int col = 0; col < level_array_x; col++)
        {
            for (int row = 0; row < level_array_y; row++)
            {
                //If the node placements int array equals to the type of node then re-instantiate the respective nodes in their original columns and rows
                if (NodeMapData.Node_Placement[col, row] == 0 && col == 3 && row == 0)
                {
                    node_array[col, row] = (GameObject)Instantiate(node, new Vector3(NodeMapData.Node_XPos[col, row], 0f, NodeMapData.Node_ZPos[col, row]), Quaternion.identity);
                    node_array[col, row].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    node_array[col, row].SendMessage("SetNodeType", "Start", SendMessageOptions.DontRequireReceiver);
                }
                else if (NodeMapData.Node_Placement[col, row] == 1)
                {
                    node_array[col, row] = (GameObject)Instantiate(node, new Vector3(NodeMapData.Node_XPos[col, row], 0f, NodeMapData.Node_ZPos[col, row]), Quaternion.identity);
                    node_array[col, row].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    node_array[col, row].SendMessage("SetNodeType", "Danger", SendMessageOptions.DontRequireReceiver);
                }
                else if (NodeMapData.Node_Placement[col, row] == 2)
                {
                    node_array[col, row] = (GameObject)Instantiate(node, new Vector3(NodeMapData.Node_XPos[col, row], 0f, NodeMapData.Node_ZPos[col, row]), Quaternion.identity);
                    node_array[col, row].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    node_array[col, row].SendMessage("SetNodeType", "Story", SendMessageOptions.DontRequireReceiver);
                }
                else if (NodeMapData.Node_Placement[col, row] == 3)
                {
                    node_array[col, row] = (GameObject)Instantiate(node, new Vector3(NodeMapData.Node_XPos[col, row], 0f, NodeMapData.Node_ZPos[col, row]), Quaternion.identity);
                    node_array[col, row].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    node_array[col, row].SendMessage("SetNodeType", "Social", SendMessageOptions.DontRequireReceiver);
                }
                else if (NodeMapData.Node_Placement[col, row] == 4)
                {
                    node_array[col, row] = (GameObject)Instantiate(node, new Vector3(NodeMapData.Node_XPos[col, row], 0f, NodeMapData.Node_ZPos[col, row]), Quaternion.identity);
                    node_array[col, row].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    node_array[col, row].SendMessage("SetNodeType", "Real_Danger", SendMessageOptions.DontRequireReceiver);
                }
                else if (NodeMapData.Node_Placement[col, row] == 5)
                {
                    node_array[col, row] = (GameObject)Instantiate(node, new Vector3(NodeMapData.Node_XPos[col, row], 0f, NodeMapData.Node_ZPos[col, row]), Quaternion.identity);
                    node_array[col, row].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    node_array[col, row].SendMessage("SetNodeType", "Safe", SendMessageOptions.DontRequireReceiver);
                }
                else if (NodeMapData.Node_Placement[col, row] == 6)
                {
                    node_array[col, row] = (GameObject)Instantiate(node, new Vector3(NodeMapData.Node_XPos[col, row], 0f, NodeMapData.Node_ZPos[col, row]), Quaternion.identity);
                    node_array[col, row].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    node_array[col, row].SendMessage("SetNodeType", "Mystery", SendMessageOptions.DontRequireReceiver);
                }
                else if (NodeMapData.Node_Placement[col, row] == 7)
                {
                    node_array[col, row] = (GameObject)Instantiate(node, new Vector3(NodeMapData.Node_XPos[col, row], 0f, NodeMapData.Node_ZPos[col, row]), Quaternion.identity);
                    node_array[col, row].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    node_array[col, row].SendMessage("SetNodeType", "End", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    void RecreateConnections(int col, int row)
    {
		//Obtain the node properties from the parent node
		DN_Node_Properties NodeProperties = node_array[col, row].GetComponent<DN_Node_Properties>();

        for (int x = col - 1; x < col + 2; x++)
        {
            for (int z = row - 1; z < row + 2; z++)
            {
                //If it's is out of array bounds, or not empty
                if (x < 0 || x > node_array.GetLength(0)-1 || z < 0 || z > node_array.GetLength(1)-1)
				{
					//Skip to the next iteration
					continue;
				}
                //If it's adjacent
                else if (x == col + 1 && z == row || x == col - 1 && z == row || z == row + 1 && x == col || z == row - 1 && x == col)
                {
                    if (node_array[x, z] != null)
                    {
                        //Loop through the int list from the parent node
                        foreach (int index in NodeProperties.NodePropertiesClass.NodeTypeConnection)
                        {
                            //If the type of number is equal to the number in the node placement int array
                            if (index == NodeMapData.Node_Placement[x, z])
                            {
                                //Adds the parent node into the adjacent node and vice versa
                                node_array[x, z].SendMessage("AddNodeConnection", node_array[col, row], SendMessageOptions.DontRequireReceiver);
                                node_array[col, row].SendMessage("AddNodeConnection", node_array[x, z], SendMessageOptions.DontRequireReceiver);
                            }
                        }
                    }
                }
            }
        }

    }

    void AssignNodeTypes()
    {
        for (int col = 0; col < level_array_x; col++)
        {
            for (int row = 0; row < level_array_y; row++)
            {
                if (node_array[col, row] != null)
                {
                    //Obtains the script from the node in the array
                    DN_Node_Properties NodeProperties = node_array[col, row].GetComponent<DN_Node_Properties>();

                    //If the node connection integer list is empty then add in the necessary values
                    if (NodeProperties.NodePropertiesClass.NodeTypeConnection.Count == 0)
                    {
                        //Depending on the type of node which will add the number respectively into the node
                        foreach (GameObject _node in NodeProperties.NodeConnection)
                        {
                            if (_node.name == "Start")
                            {
                                NodeProperties.NodePropertiesClass.NodeTypeConnection.Add(0);
                            }
                            else if (_node.name == "Danger")
                            {
                                NodeProperties.NodePropertiesClass.NodeTypeConnection.Add(1);
                            }
                            else if (_node.name == "Story")
                            {
                                NodeProperties.NodePropertiesClass.NodeTypeConnection.Add(2);
                            }
                            else if (_node.name == "Social")
                            {
                                NodeProperties.NodePropertiesClass.NodeTypeConnection.Add(3);
                            }
                            else if (_node.name == "Real_Danger")
                            {
                                NodeProperties.NodePropertiesClass.NodeTypeConnection.Add(4);
                            }
                            else if (_node.name == "Safe")
                            {
                                NodeProperties.NodePropertiesClass.NodeTypeConnection.Add(5);
                            }
                            else if (_node.name == "Mystery")
                            {
                                NodeProperties.NodePropertiesClass.NodeTypeConnection.Add(6);
                            }
                            else if (_node.name == "End")
                            {
                                NodeProperties.NodePropertiesClass.NodeTypeConnection.Add(7);
                            }
                        }
                    }
                }
            }
        }
    }

    void RenderPaths()
    {
        for (int col = 0; col < node_array.GetLength(0); col++)
        {
            for (int row = 0; row < node_array.GetLength(1); row++)
            {
                if (node_array[col, row] != null)
                {
                    DN_Node_Properties NodePropertiesScript = node_array[col, row].GetComponent<DN_Node_Properties>();
                    NodePropertiesScript.FilterThroughList();

                    //Render the paths again
                    node_array[col, row].SendMessage("RenderPaths", SendMessageOptions.DontRequireReceiver);

                    //Reload the paths again
                    node_array[col, row].SendMessage("ReloadPaths", SendMessageOptions.DontRequireReceiver);

                    //If a path has been revealed previously then reveal the connection and render the node sprite again
                    if (NodePropertiesScript.NodePropertiesClass.paths_revealed)
                    {
                        node_array[col, row].SendMessage("Reveal_Connection", SendMessageOptions.DontRequireReceiver);
                        node_array[col, row].SendMessage("RenderNodes", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
    }

    void HideMap()
    {
        //For each of the gameobjects in the node array
        foreach (GameObject go in node_array)
        {
            //If the gameobject is not null
            if (go != null)
            {
                //Don't destroy on load
                DontDestroyOnLoad(go);

                //Deactivate the gameobject
                go.SetActive(false);
            }
        }
        //Find gameobjects based on tag
        GameObject[] paths = GameObject.FindGameObjectsWithTag("Path");

        //For each of the gameobjects in the gameobject array
        foreach (GameObject go in paths)
        {
            //Don't destroy on load
            DontDestroyOnLoad(go);

            //Deactivate the gameobject
            go.SetActive(false);
        }
        //Don't destroy the PC
        DontDestroyOnLoad(pc);

        //Deactivate the gameobject
        pc.SetActive(false);
    }
    void Return_Map()
    {
        //Activate the PC
        pc.SetActive(true);

        //Loop through the for loops
        for (int col = 0; col < node_array.GetLength(0); col++)
        {
            for (int row = 0; row < node_array.GetLength(1); row++)
            {
                //If the gameobject in the node array is not null
                if (node_array[col, row] != null)
                {
                    //Activate the gameobject
                    node_array[col, row].SetActive(true);

                    //Reload the paths again
                    node_array[col, row].SendMessage("Reload_Paths", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
    //functions for accessing the private data
    void return_nodemap(GameObject receive)
    {
        receive.SendMessage("NodeMap", node_array,SendMessageOptions.DontRequireReceiver);
    }
    void return_pc_node(GameObject receive)
    {
        receive.SendMessage("PC_Node", pc_cur_node, SendMessageOptions.DontRequireReceiver);
    }
    void set_pc_node(GameObject _node)
    {
        pc_cur_node = _node;
        pc_cur_node.SendMessage("Reveal_Connection", SendMessageOptions.DontRequireReceiver);
    }
    //used for instantiating nodes into scene and adding them to the array
    void Spawn_AddNode(GameObject go, int col, int row)
    {
        node_array[col,row]=(GameObject)Instantiate(go, new Vector3(row*3+(Random.Range(rng_offset_1, rng_offset_2)), 0, col*3+(Random.Range(rng_offset_1, rng_offset_2))), new Quaternion(90, 0, 0, 90));
		NodeMapData.Node_XPos[col, row] = node_array[col, row].transform.position.x;
		NodeMapData.Node_ZPos[col, row] = node_array[col, row].transform.position.z;
    }
    //general node generation algorithm
    void GenerateNode()
    {
        while (OpenList.Count > 0)
        {
            CheckAdjSpaces(OpenList[0]);
        }
    }

	int UniqueRandomNum(int ran_num)
	{
		//Variable for randomizing a number between the minimum and maximum number
		int Ran_Num = Random.Range(0, ran_num);

		//While the random number is the same as the previous random number, re-randomize it
		while (Ran_Num == Prev_Number)
		{
			//Re-randomzie the number again
			Ran_Num = Random.Range(0, ran_num);
		}

		//Assign the variable to the random number
		Prev_Number = Ran_Num;

		//Return the value
		return Prev_Number;
	}

    void CheckAdjSpaces(GameObject _node)
    {
        bool no_spawn = true;

        int _node_x = 0;
        int _node_y = 0;

		GameObject[] AmountOfNodes = GameObject.FindGameObjectsWithTag("Node");

        //searching the array for the node we want
        for (int col = 0; col < node_array.GetLength(0); col++)
        {
            for (int row = 0; row < node_array.GetLength(1); row++)
            {
                if (node_array[col,row] == _node)
                {
                    //get the index of the node in the array
                    _node_x = col;
                    _node_y = row;
                }
            }
        }
        //checking the adjacent nodes
        for (int col = _node_x - 1; col < _node_x + 2; col++)
        {
            for (int row = _node_y - 1; row < _node_y + 2; row++)
            {
                //if the space is out of array bounds, or space not empty
                if (col < 0 || col > node_array.GetLength(0)-1 || row < 0 || row > node_array.GetLength(1)-1 || node_array[col, row] != null)
                {
                    //skip to next iteration
                    continue;
                }
                //is adjacent
                else if (col == _node_x + 1 && row == _node_y || col == _node_x - 1 && row == _node_y || row == _node_y + 1 && col == _node_x || row == _node_y - 1 && col == _node_x)
                {
                    //if the slot is empty
                    if (node_array[col, row] == null)
                    {
                        //ensuring that the we will have more of a chance to spawn a node with less nodes in the map
						if (AmountOfNodes.GetLength(0) < 10)
                        {
                            //random chance to spawn a node if the space is empty
							no_spawn = false;
							Spawn_AddNode(node, col, row);
							OpenList.Add(node_array[col, row]);

							//calling method to add the node to list of node connections in the parent node and vice versa
							_node.SendMessage("AddNodeConnection", node_array[col, row], SendMessageOptions.DontRequireReceiver);
							node_array[col, row].SendMessage("AddNodeConnection", _node, SendMessageOptions.DontRequireReceiver);

							//deciding the type of event the node will hold
							bool decide_type = false;
							while (!decide_type)
							{
								int type = UniqueRandomNum(7);
								NodeMapData.Node_Placement[col, row] = type;
								if (type == 1 && num_danger > 0)
								{
									node_array[col, row].SendMessage("SetNodeType", "Danger", SendMessageOptions.DontRequireReceiver);
									num_danger -= 1;
									decide_type = true;
								}
								if (type == 2 && num_story > 0)
								{
									node_array[col, row].SendMessage("SetNodeType", "Story", SendMessageOptions.DontRequireReceiver);
									num_story -= 1;
									decide_type = true;
								}
								if (type == 3 && num_social > 0)
								{
									node_array[col, row].SendMessage("SetNodeType", "Social", SendMessageOptions.DontRequireReceiver);
									num_social -= 1;
									decide_type = true;
								}
								if (type == 4 && num_real_danger > 0)
								{
									node_array[col, row].SendMessage("SetNodeType", "Real_Danger", SendMessageOptions.DontRequireReceiver);
									num_real_danger -= 1;
									decide_type = true;
								}
								if (type == 5 && num_safe > 0)
								{
									node_array[col, row].SendMessage("SetNodeType", "Safe", SendMessageOptions.DontRequireReceiver);
									num_safe -= 1;
									decide_type = true;
								}
								if (type == 6 && num_mystery > 0)
								{
									node_array[col, row].SendMessage("SetNodeType", "Mystery", SendMessageOptions.DontRequireReceiver);
									num_mystery -= 1;
									decide_type = true;
								}
							}
                        }
                        //less of a chance to spawn a node
                        else
                        {
                            //random chance to spawn a node if the space is empty
                            int rng_spawn = Random.Range(0, 101);
                            if (rng_spawn < rng_spawn_chance)
                            {
                                no_spawn = false;
                                Spawn_AddNode(node, col, row);
                                OpenList.Add(node_array[col, row]);

                                //calling method to add the node to list of node connections in the parent node and vice versa
                                _node.SendMessage("AddNodeConnection", node_array[col, row], SendMessageOptions.DontRequireReceiver);
                                node_array[col, row].SendMessage("AddNodeConnection", _node, SendMessageOptions.DontRequireReceiver);

                                //deciding the type of event the node will hold
                                bool decide_type = false;
                                while (!decide_type)
                                {
									int type = UniqueRandomNum(7);
                                    NodeMapData.Node_Placement[col, row] = type;
                                    if (type == 1 && num_danger > 0)
                                    {
                                        node_array[col, row].SendMessage("SetNodeType", "Danger", SendMessageOptions.DontRequireReceiver);
                                        num_danger -= 1;
                                        decide_type = true;
                                    }
                                    if (type == 2 && num_story > 0)
                                    {
                                        node_array[col, row].SendMessage("SetNodeType", "Story", SendMessageOptions.DontRequireReceiver);
                                        num_story -= 1;
                                        decide_type = true;
                                    }
                                    if (type == 3 && num_social > 0)
                                    {
                                        node_array[col, row].SendMessage("SetNodeType", "Social", SendMessageOptions.DontRequireReceiver);
                                        num_social -= 1;
                                        decide_type = true;
                                    }
                                    if (type == 4 && num_real_danger > 0)
                                    {
                                        node_array[col, row].SendMessage("SetNodeType", "Real_Danger", SendMessageOptions.DontRequireReceiver);
                                        num_real_danger -= 1;
                                        decide_type = true;
                                    }
                                    if (type == 5 && num_safe > 0)
                                    {
                                        node_array[col, row].SendMessage("SetNodeType", "Safe", SendMessageOptions.DontRequireReceiver);
                                        num_safe -= 1;
                                        decide_type = true;
                                    }
                                    if (type == 6 && num_mystery > 0)
                                    {
                                        node_array[col, row].SendMessage("SetNodeType", "Mystery", SendMessageOptions.DontRequireReceiver);
                                        num_mystery -= 1;
                                        decide_type = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (no_spawn)
        {
            //checking the adjacent nodes
            for (int col = _node_x - 1; col < _node_x + 2; col++)
            {
                for (int row = _node_y - 1; row < _node_y + 2; row++)
                {
                    //if the space is out of array bounds, or space not empty
                    if (col < 0 || col > node_array.GetLength(0)-1 || row < 0 || row > node_array.GetLength(1)-1 || node_array[col, row] == null)
                    {
                        //skip to next iteration
                        continue;
                    }
                    //is adjacent
                    else if (col == _node_x + 1 && row == _node_y || col == _node_x - 1 && row == _node_y || row == _node_y + 1 && col == _node_x || row == _node_y - 1 && col == _node_x)
                    {
                        //if the slot has a node init
                        if (node_array[col, row] != null)
                        {
                            //random chance to spawn a node if the space is empty
                            int rng_spawn = Random.Range(0, 101);
                            if (rng_spawn <= 50)
                            {
                                //calling method to add the node to list of node connections in the parent node and vice versa                                
                                _node.SendMessage("AddNodeConnection", node_array[col, row], SendMessageOptions.DontRequireReceiver);
                                node_array[col, row].SendMessage("AddNodeConnection", _node, SendMessageOptions.DontRequireReceiver);

                                no_spawn = false;
                            }
                        }
                    }
                }
            }
        }
        //Remove the node from the open list
        OpenList.Remove(_node);

        //Add the node in the closed list
        ClosedList.Add(_node);
    }
}