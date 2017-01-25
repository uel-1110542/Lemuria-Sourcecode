using UnityEngine;
using UnityEngine.SceneManagement;

public class TL_Regions_Manager : MonoBehaviour
{
    //Variables
    public GameObject PC;
    public GameObject Scene_Camera;
    public GameObject DesertNode;
    public GameObject MountainNode;
    public GameObject ForestNode;
    public GameObject PlainsNode;
    public GameObject Collect_Manager;
    
    private bool game_paused = false;
    private GameObject LevelMap;   
    private int[,] WorldMapLayout;
	private TL_SpawnPC SpawnPCScript;
    private TL_GlobalGameManager GlobalGameManagerScript;



    void Awake()
    {
        //Finds the gameobject and obtains the script
        TL_PC_World_Movement WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

        GlobalGameManagerScript = GameObject.Find("Global_GameManager").GetComponent<TL_GlobalGameManager>();
        if (GlobalGameManagerScript.LoadingFlag)
        {
            //Loads the world map data
            WorldMovementScript.LoadWorldMapData();
            SceneManager.LoadScene("Level_Map");
        }

        //If there is more than 1 type of this instance, destroy it otherwise don't
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        //If the camera gameobject does not exist in the scene
        if (!GameObject.Find("Camera(Clone)") || !GameObject.FindGameObjectWithTag("MainCamera"))
        {
			GameObject cm = (GameObject)Instantiate(Scene_Camera, new Vector3(3f, 5f, -3f), transform.rotation);
        }

        //Finds the PC based on the tag
		GameObject PC = GameObject.FindGameObjectWithTag("PC");

        //Obtain the script from this gameobject
		SpawnPCScript = gameObject.GetComponent<TL_SpawnPC>();

        //If the gameobject is null then spawn the PC and if not re-position the current PC
		if (PC == null)
		{
			PC = SpawnPCScript.SpawnPC(PlayerPrefs.GetString("PC Name"));
		}
		else
		{
            PC.transform.position = SpawnPCScript.SpawningPos;
		}
		//Spawn the node biomes
		SpawnObject(DesertNode, new Vector3(3f, 5f, 0f));
		SpawnObject(ForestNode, new Vector3(3f, 5f, 0.5f));
		SpawnObject(MountainNode, new Vector3(3f, 5f, 0f));
		SpawnObject(PlainsNode, new Vector3(3f, 5f, 0f));

    }

	void SpawnObject(GameObject go, Vector3 NodePos)
	{
		GameObject ObjClone = (GameObject) Instantiate(go, NodePos, Quaternion.identity);
	}

    public bool Pause_Game()
    {
        if (game_paused)
        {
            game_paused = false;
        }
        else if (!game_paused)
        {
            game_paused = true;
        }
        return game_paused;
    }

}
