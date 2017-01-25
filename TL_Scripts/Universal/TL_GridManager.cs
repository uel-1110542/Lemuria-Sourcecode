using UnityEngine;

public class TL_GridManager : MonoBehaviour {

    //Int variables for setting array lengths
    public int Length_X;
    public int Length_Z;

    //GameObjects
    public GameObject Platform;
    public GameObject Wall;

    //Sprites
    public Sprite Forest_Floor;
    public Sprite Forest_Wall;
    public Sprite Plains_Floor;
    public Sprite Plains_Wall;
    public Sprite Desert_Floor;
    public Sprite Desert_Wall;
    public Sprite Mountains_Floor;
    public Sprite Mountains_Wall;

    //2D Arrays for grid management
    private GameObject[,] LevelArea;
    private int[,] LevelLayout;

    //For obtaining the location of the PC in the world
    public TL_PC_World_Movement WorldMovementScript;
    


    void Awake()
    {
        //Finds the gameobject and obtains the script
        WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

        //Initialize the 2D arrays
        LevelArea = new GameObject[Length_X, Length_Z];
        LevelLayout = new int[Length_X, Length_Z];
        FillIntArray();
    }

    //Sets a gameobject within the 2D gameobject array
    public void SetGOInLevelArea(GameObject go, int x, int z)
    {
        LevelArea[x, z] = go;
    }

    //Returns a gameobject from the 2D gameobject array
    public GameObject ReturnGOInLevelArea(int x, int z)
    {
        return LevelArea[x, z];
    }

    //Returns the 2D array
    public GameObject[,] ReturnLevelAreaArray()
    {
        return LevelArea;
    }
    
    //Initialize 2D int array with all zeros as default
    void FillIntArray()
    {
        for (int x = 0; x < LevelLayout.GetLength(0); x++)
        {
            for (int z = 0; z < LevelLayout.GetLength(1); z++)
            {
                LevelLayout[x, z] = 0;
            }
        }
    }

    //Returns the 2D int array
    public int[,] ReturnIntArray()
    {
        return LevelLayout;
    }

    //Spawns a gameobjects based on X, Y and Z positions and sets it within the 2D gameobject array
    public void SpawnGameObject(GameObject go, float x, float y, float z)
    {
        LevelArea[(int) x, (int)z] = (GameObject) Instantiate(go, new Vector3(x, y, z), Quaternion.identity);
        SetBiomeSprites(LevelArea[(int)x, (int)z], WorldMovementScript.WorldMapDataClass.Region_Name);
    }

    //Changes the walls and platforms sprite based on the name of the region the PC went into
    public void SetBiomeSprites(GameObject go, string region)
    {
        if (go.name == "Platform(Clone)")
        {
            if (region == "PlainsNode(Clone)")
            {
                go.GetComponent<SpriteRenderer>().sprite = Plains_Floor;
            }
            else if (region == "ForestNode(Clone)")
            {
                go.GetComponent<SpriteRenderer>().sprite = Forest_Floor;
            }
            else if (region == "DesertNode(Clone)")
            {
                go.GetComponent<SpriteRenderer>().sprite = Desert_Floor;
            }
            else if (region == "MountainNode(Clone)")
            {
                go.GetComponent<SpriteRenderer>().sprite = Mountains_Floor;
            }
            go.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }

        if (go.name == "Wall(Clone)")
        {
            if (region == "PlainsNode(Clone)")
            {
                go.GetComponent<SpriteRenderer>().sprite = Plains_Wall;
            }
            else if (region == "ForestNode(Clone)")
            {
                go.GetComponent<SpriteRenderer>().sprite = Forest_Wall;
            }
            else if (region == "DesertNode(Clone)")
            {
                go.GetComponent<SpriteRenderer>().sprite = Desert_Wall;
            }
            else if (region == "MountainNode(Clone)")
            {
                go.GetComponent<SpriteRenderer>().sprite = Mountains_Wall;
            }
            go.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }
    }

    




}
