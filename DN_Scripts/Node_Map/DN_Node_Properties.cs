using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DN_Node_Properties : MonoBehaviour
{
    public List<Sprite> node_sprites = new List<Sprite>();
    private SpriteRenderer spriteRenderer;
    private Sprite node_sprite;

    public GameObject path;
    public List<GameObject> NodeConnection = new List<GameObject>();
    public List<GameObject> path_lines = new List<GameObject>();

    //Serializable class
    [System.Serializable]
    public class SerializeNodeProperties
    {
        public List<int> NodeTypeConnection;
        public bool paths_revealed = false;
        public bool event_done = false;
        public string st_node_type;
    }
    public SerializeNodeProperties NodePropertiesClass = new SerializeNodeProperties();    


    void Awake()
    {
        //Find the gameobject and obtain the component
        TL_PC_World_Movement WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

        //Initialize a new integer list for the node types
        NodePropertiesClass.NodeTypeConnection = new List<int>();

        //Obtain the component for the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Set the node sprites respective to the region name
        switch (WorldMovementScript.WorldMapDataClass.Region_Name)
        {
            case "DesertNode(Clone)":
                spriteRenderer.sprite = Search_sprite("Unknown_3");
                break;

            case "ForestNode(Clone)":
                spriteRenderer.sprite = Search_sprite("Unknown_1");
                break;

            case "PlainsNode(Clone)":
                spriteRenderer.sprite = Search_sprite("Unknown_2");
                break;

            case "MountainNode(Clone)":
                spriteRenderer.sprite = Search_sprite("Unknown_4");
                break;
        }
        
    }

    void AddNodeConnection(GameObject _node)
    {
        //Add the node into the list
        NodeConnection.Add(_node);

        //Using distinct to prevent duplicated gameobjects in a list
        NodeConnection = NodeConnection.Distinct().ToList();
    }

    public void FilterThroughList()
    {
        //Loop through the int list
        for (int i = 0; i < NodePropertiesClass.NodeTypeConnection.Count; i++)
        {
            //If the int list only has 1 element, remove the rest except one from the gameobject list
            if (NodePropertiesClass.NodeTypeConnection.Count == 1)
            {
                NodeConnection.RemoveRange(1, NodeConnection.Count-1);
            }
            //otherwise filter through the lists and remove any inconsistencies, for example if the first element is 0 and
            //it doesn't contain the Start node, remove it
            else if (NodeConnection.Count > NodePropertiesClass.NodeTypeConnection.Count)
            {
                if (NodeConnection[i].name == "Start" && NodePropertiesClass.NodeTypeConnection[i] != 0 || NodeConnection[i].name == "Danger" && NodePropertiesClass.NodeTypeConnection[i] != 1 || NodeConnection[i].name == "Story" && NodePropertiesClass.NodeTypeConnection[i] != 2 || NodeConnection[i].name == "Social" && NodePropertiesClass.NodeTypeConnection[i] != 3 || NodeConnection[i].name == "Real_Danger" && NodePropertiesClass.NodeTypeConnection[i] != 4 || NodeConnection[i].name == "Safe" && NodePropertiesClass.NodeTypeConnection[i] != 5 || NodeConnection[i].name == "Mystery" && NodePropertiesClass.NodeTypeConnection[i] != 6 || NodeConnection[i].name == "End" && NodePropertiesClass.NodeTypeConnection[i] != 7)
                {
                    NodeConnection.RemoveAt(i);
                }
            }
        }

    }

    public void RenderNodes()
    {
        //Obtain the component from this gameobject
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Set the sprite
        spriteRenderer.sprite = node_sprite;
    }
    void Reveal_Connection()
    {
        //For each gameobject in the gameobject list, send the message to the gameobject
        foreach (GameObject _node in NodeConnection)
        {
            _node.SendMessage("RenderNodes", SendMessageOptions.DontRequireReceiver);
        }

        //For each gameobject in the gameobject list, set the bool to true from the serialized class
        foreach (GameObject _path in path_lines)
        {
            NodePropertiesClass.paths_revealed = true;

            //Activate the gameobject path
            _path.SetActive(true);
        }
    }
    void Reload_Paths()
    {
        //If the paths have been revealed
        if (NodePropertiesClass.paths_revealed)
        {
            //For each gameobject in the gameobject list, activate the gameobject
            foreach (GameObject _path in path_lines)
            {
                _path.SetActive(true);
            }
        }
    }
    void RenderPaths()
    {
        //For each gameobject in the gameobject list
        foreach (GameObject _node in NodeConnection)
        {
            //Create the gameobject containing the line renderer
            GameObject path_line = (GameObject)Instantiate(path, transform.position, transform.rotation);

            //Obtain the line renderer component from the created gameobject
            LineRenderer line_render = path_line.GetComponent<LineRenderer>();

            //Set the color to black
            line_render.startColor = Color.black;
            line_render.endColor = Color.black;

            //Set the width of the paths
            line_render.startWidth = 0.1f;
            line_render.endWidth = 0.1f;

            //Create a Vector3 array
            Vector3[] point = new Vector3[2];

            //First element contains the position connecting to another gameobject
			point[0] = new Vector3(_node.transform.position.x, -0.1f, _node.transform.position.z);

            //Second element contains this gameobject's position
			point[1] = new Vector3(transform.position.x, -0.1f, transform.position.z);

            //Set the parent to this gameobject
            path_line.transform.SetParent(gameObject.transform);

            //Set the positions based from the Vector3 array
            line_render.SetPositions(point);

            //Add the created gameobject containing the line renderer
            path_lines.Add(path_line);

            //Deactivate the gameobject
            path_line.SetActive(false);
        }
    }
    void SetNodeType(string node_type)
    {
        //If the node type equals to the coresponding string then search for the node sprite
        if (node_type == "Danger")
        {
            node_sprite = Search_sprite(node_type);
        }
        if (node_type == "Story")
        {
            NodePropertiesClass.event_done = true;
            node_sprite = Search_sprite(node_type);
        }
        if (node_type == "Social")
        {
            NodePropertiesClass.event_done = true;
            node_sprite = Search_sprite(node_type);
        }
        if (node_type == "Real_Danger")
        {
            node_sprite = Search_sprite(node_type);
        }
        if (node_type == "Safe")
        {
            //NodePropertiesClass.event_done = true;
            node_sprite = Search_sprite(node_type);
        }
        if (node_type == "Mystery")
        {
            node_sprite = Search_sprite(node_type);
        }
        if (node_type == "Boss")
        {
            node_sprite = Search_sprite(node_type);
        }
        if (node_type == "End")
        {
            node_sprite = Search_sprite("Start");
        }
        if (node_type == "Start")
        {
            NodePropertiesClass.event_done = true;
            node_sprite = Search_sprite(node_type);
        }
        if (node_type == "Unknown")
        {
            node_sprite = Search_sprite(node_type);
        }
        //Set the string from the serialized class to the node type
        NodePropertiesClass.st_node_type = node_type;

        //Set the name of the gameobject from the string in the serialized class
        gameObject.name = NodePropertiesClass.st_node_type;
    }
    Sprite Search_sprite(string node_type)
    {
        //Variable for returning the sprite
        Sprite sprite_output = node_sprite;

        //For each of the node sprites in the list
        foreach (Sprite sprite in node_sprites)
        {
            //If the name of the sprite is equal to the one in the node sprite list
            if (sprite.name == node_type)
            {
                //Make the sprite output variable equal to the sprite from the foreach loop
                sprite_output = sprite;
            }
        }
        //Return the sprite
        return sprite_output;
    }
}