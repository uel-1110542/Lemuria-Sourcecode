using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DN_PC_Level_Movement : MonoBehaviour
{
    public GameObject Player_Char;
    public bool NodeSelection = false;
    public bool SkipEvent = false;

    private bool is_moving = false;
    private bool called_event = false;

    private Vector3 pc_destination = Vector3.zero;
    private string event_name;

    private GameObject[,] node_array;
    private GameObject pc_cur_node;

    [System.Serializable]
    public class SerializeLevelMovement
    {
        public float X_Pos;
        public float Z_Pos;
    }
    public SerializeLevelMovement LevelMovementClass = new SerializeLevelMovement();
    private DN_Node_Map_Generation node_script;
    private TL_RaycastSelection SelectionScript;
    public int Global_XPos;
    public int Global_ZPos;


    void Start()
    {
        node_script = GameObject.FindGameObjectWithTag("LM").GetComponent<DN_Node_Map_Generation>();
        node_script.SendMessage("return_nodemap", gameObject, SendMessageOptions.DontRequireReceiver);
        SelectionScript = GetComponent<TL_RaycastSelection>();

        GameObject PC = GameObject.FindGameObjectWithTag("PC");
        if (PC != null)
        {
            LevelMovementClass.X_Pos = PC.transform.position.x;
            LevelMovementClass.Z_Pos = PC.transform.position.z;
        }        
    }
	
	void Update ()
    {
        //Highlight adjacent nodes
        HighlightNodes();

        //Checks the node array
        CheckNodeArray();

        //Handles PC movement
        PC_Movement();
    }

    public void SaveProgress()
    {
        BinaryFormatter BinaryFormat = new BinaryFormatter();
        FileStream FileStream = File.Create(Application.persistentDataPath + "/SavedProgress.sg");
        BinaryFormat.Serialize(FileStream, LevelMovementClass);
        FileStream.Close();
    }

    public void LoadProgress()
    {
        if (File.Exists(Application.persistentDataPath + "/SavedProgress.sg"))
        {
            BinaryFormatter BinaryFormat = new BinaryFormatter();
            FileStream FileStream = File.Open(Application.persistentDataPath + "/SavedProgress.sg", FileMode.Open);
            LevelMovementClass = (SerializeLevelMovement) BinaryFormat.Deserialize(FileStream);
            FileStream.Close();
        }
    }

    void PC_Movement()
    {
        if (!is_moving)
        {
            Player_Char = GameObject.FindGameObjectWithTag("PC");

            if (SelectionScript.ReturnSelectedGO() != null && SelectionScript.ReturnSelectedGO().transform.tag == "Node" && !NodeSelection)
            {
                Player_Char = GameObject.FindGameObjectWithTag("PC");
                node_script.SendMessage("return_nodemap", gameObject, SendMessageOptions.DontRequireReceiver);
                node_script.SendMessage("return_pc_node", gameObject, SendMessageOptions.DontRequireReceiver);

                //searching the array for the node we want
                for (int col = 0; col < node_array.GetLength(0); col++)
                {
                    for (int row = 0; row < node_array.GetLength(1); row++)
                    {
                        if (node_array[col, row] == pc_cur_node)
                        {
                            DN_Node_Properties connections = node_array[col, row].GetComponent<DN_Node_Properties>();
                            connections.NodePropertiesClass.event_done = true;
                            if (connections.NodeConnection.Contains(SelectionScript.ReturnSelectedGO().transform.gameObject))
                            {
                                //Gray out visited nodes
                                SpriteRenderer Node_Sprite = SelectionScript.ReturnSelectedGO().transform.gameObject.GetComponent<SpriteRenderer>();
                                Node_Sprite.color = Color.grey;

                                //Set the col and row to the global X and Z positions
                                Global_XPos = col;
                                Global_ZPos = row;

                                //Send message to the set PC node function
                                node_script.SendMessage("set_pc_node", SelectionScript.ReturnSelectedGO().transform.gameObject, SendMessageOptions.DontRequireReceiver);

                                //Set the X and Z variables to the X and Z positions of the raycast hit gameobject
                                float x = SelectionScript.ReturnSelectedGO().transform.position.x;
                                float z = SelectionScript.ReturnSelectedGO().transform.position.z;

                                //Set PC destination
                                pc_destination = new Vector3(x, 0.5f, z);

                                //Set the X and Z variables from the serialized class to x and z local variables
                                LevelMovementClass.X_Pos = x;
                                LevelMovementClass.Z_Pos = z;

                                //Set event name to the raycast hit gameobject name
                                event_name = SelectionScript.ReturnSelectedGO().transform.gameObject.name;

                                //Turn the boolean to true
                                is_moving = true;

                                //Set the boolean from the serialized class to true
                                called_event = SelectionScript.ReturnSelectedGO().transform.GetComponent<DN_Node_Properties>().NodePropertiesClass.event_done;
                            }
                        }
                    }
                }
            }
            else if (NodeSelection)
            {
                //If the node selection boolean is true then select the adjacent nodes with the global X and Z positions and the raycast hit gameobject
                SelectAdjacentNodes(Global_XPos, Global_ZPos, SelectionScript.ReturnSelectedGO().transform.gameObject);
            }
        }
        else if (is_moving)
        {
            //If the distance between the PC's destination and the PC is less than the value set in the if statement
            if (Vector3.Distance(pc_destination, Player_Char.transform.position) < 0.05f)
            {
                //Set the bool to false
                is_moving = false;

                //Find the gameobject and obtain the script
                DN_Event_Manager event_caller = GameObject.FindGameObjectWithTag("LM").GetComponent<DN_Event_Manager>();

                //Set the PC's position to its' destination
                Player_Char.transform.position = pc_destination;

                //Set the variables in the serialized class to the PC's X and Z positions
                LevelMovementClass.X_Pos = Player_Char.transform.position.x;
                LevelMovementClass.Z_Pos = Player_Char.transform.position.z;

                //If the event is not called
                if (!called_event)
                {
                    if (event_name == "End")
                    {
                        //Destroy the nodes, paths and the PC within the current scene
                        foreach (GameObject node in GameObject.FindGameObjectsWithTag("Node"))
                        {
                            Destroy(node);
                        }
                        foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                        {
                            Destroy(path);
                        }
                        //Destroy the PC
                        Destroy(Player_Char);

                        //Destroy the level manager for the current node map
                        Destroy(GameObject.FindGameObjectWithTag("LM"));
                    }
                    else
                    {
                        //Call the function to hide the map
                        node_script.SendMessage("HideMap", SendMessageOptions.DontRequireReceiver);
                    }
                    //Call the event
                    event_caller.CallEvent(event_name);
                }
            }
            else
            {
                //Animate PC movement
                Player_Char.transform.position = Vector3.Lerp(Player_Char.transform.position, pc_destination, 0.05f);
            }
        }
    }

    void CheckNodeArray()
    {
        //If the PC is not moving
        if (!is_moving)
        {
            //Locates the gameobject based on the tag
            GameObject PC = GameObject.FindGameObjectWithTag("PC");

            //Loops through 2 for loops
            for (int col = 0; col < node_array.GetLength(0); col++)
            {
                for (int row = 0; row < node_array.GetLength(1); row++)
                {
                    //If the gameobject in the 2D array and the PC is not null
                    if (node_array[col, row] != null && PC != null)
                    {
                        //If the X and Z node positions are the same as the PC's
                        if (node_array[col, row].transform.position.x == PC.transform.position.x && node_array[col, row].transform.position.z == PC.transform.position.z)
                        {
                            //Set the global variables to the col and row
                            Global_XPos = col;
                            Global_ZPos = row;
                        }
                    }
                }
            }
        }        
    }

    void HighlightNodes()
    {
        //Loop through the for loops to detect adjacent nodes
        for (int x = Global_XPos - 1; x < Global_XPos + 2; x++)
        {
            for (int z = Global_ZPos - 1; z < Global_ZPos + 2; z++)
            {
                //If the loops go outside the boundaries of the array, skip the iteration
                if (x < 0 || x > node_array.GetLength(0) - 1 || z < 0 || z > node_array.GetLength(1) - 1)
                {
                    continue;
                }
                else if (x == Global_XPos + 1 && z == Global_ZPos || x == Global_XPos - 1 && z == Global_ZPos || z == Global_ZPos + 1 && x == Global_XPos || z == Global_ZPos - 1 && x == Global_XPos)
                {
                    //If a node is adjacent to the PC and the nodes aren't the start and the end
                    if (node_array[x, z] != null && node_array[x, z].name != "Start" && node_array[x, z].name != "End")
                    {
                        //Obtain the node properties script from the adjacent node
                        DN_Node_Properties NodePropertiesScript = node_array[x, z].GetComponent<DN_Node_Properties>();

                        //Obtain the node sprite from the adjacent node
                        SpriteRenderer Node_Sprite = node_array[x, z].transform.gameObject.GetComponent<SpriteRenderer>();

                        //If the node selection is true then highlight it
                        if (NodeSelection)
                        {
                            Node_Sprite.color = Color.cyan;
                            break;
                        }
                        else if (NodePropertiesScript.NodePropertiesClass.paths_revealed)
                        {
                            //If the paths are revealed and the node selection is false then change the color to gray
                            Node_Sprite.color = Color.gray;
                            break;
                        }
                        else
                        {
                            //If neither boolean is true then revert the color to its' original state
                            Node_Sprite.color = new Color(255f, 255f, 255f, 255f);
                            break;
                        }
                    }
                }
            }
        }
    }

    void SelectAdjacentNodes(int Cur_XPos, int Cur_ZPos, GameObject SelectedNode)
    {
        //Loop through the for loops to detect adjacent nodes
        for (int x = Cur_XPos - 1; x < Cur_XPos + 2; x++)
        {
            for (int z = Cur_ZPos - 1; z < Cur_ZPos + 2; z++)
            {
                //If the loops go outside the boundaries of the array, skip the iteration
                if (x < 0 || x > node_array.GetLength(0) - 1 || z < 0 || z > node_array.GetLength(1) - 1)
                {
                    continue;
                }
                else if (x == Cur_XPos + 1 && z == Cur_ZPos || x == Cur_XPos - 1 && z == Cur_ZPos || z == Cur_ZPos + 1 && x == Cur_XPos || z == Cur_ZPos - 1 && x == Cur_XPos)
                {
                    //If a node is adjacent to the PC and the nodes aren't the start and the end
                    if (node_array[x, z] != null && node_array[x, z].name != "Start" && node_array[x, z].name != "End")
                    {
                        //Obtain the node properties script from the adjacent node
                        DN_Node_Properties NodePropertiesScript = node_array[x, z].GetComponent<DN_Node_Properties>();

                        //If the selected node is equal to an adjacent node
                        if (SelectedNode == node_array[x, z])
                        {
                            //If the node selection bool is true and the skip event bool is false
                            if (NodeSelection && !SkipEvent)
                            {
                                //Render the node
                                NodePropertiesScript.RenderNodes();

                                //Turn node selection off
                                NodeSelection = false;
                                break;
                            }
                            else if (NodeSelection && SkipEvent)
                            {
                                //If the node selection and skip event bool are true then set the adjacent node's event done bool to true
                                NodePropertiesScript.NodePropertiesClass.event_done = true;

                                //Turn both booleans to false
                                SkipEvent = false;
                                NodeSelection = false;
                                break;
                            }
                        }
                        else
                        {
                            //Turn node selection off
                            NodeSelection = false;
                            break;
                        }
                    }
                }
            }
        }
    }

    void NodeMap(GameObject[,] _array)
    {
        node_array = _array;
    }
    void PC_Node(GameObject _node)
    {
        pc_cur_node = _node;
    }

}
