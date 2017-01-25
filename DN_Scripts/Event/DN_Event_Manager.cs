using UnityEngine;
using UnityEngine.SceneManagement;

public class DN_Event_Manager : MonoBehaviour
{
    private DN_Collectables dn_collect_class;
    private DN_Node_Map_Generation dn_node_map_class;
    private TL_PC_World_Movement WorldMovementScript;


    void Start ()
    {
        //Locates the gameobject and then obtains the script
        dn_collect_class = GameObject.FindGameObjectWithTag("CM").GetComponent<DN_Collectables>();
        dn_node_map_class = GameObject.FindGameObjectWithTag("LM").GetComponent<DN_Node_Map_Generation>();
        WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();
    }
    public void ReturnToMap()
    {
        //Loads the level map
        SceneManager.LoadScene("Level_Map");

        //Turn nodes and paths back on
        dn_node_map_class.SendMessage("Return_Map",SendMessageOptions.DontRequireReceiver);
    }
    public void WinState(string event_type)
    {
        string item_list = WorldMovementScript.WorldMapDataClass.Region_Name;
		dn_collect_class.AddCollectable(item_list, event_type);
    }
    public void CallEvent(string event_name)
    {
        if (event_name == "Safe")
        {
            SceneManager.LoadScene("Sokoban_Event");
        }
        if (event_name == "Danger")
        {
            int rng = Random.Range(0, 101);
            if (rng < 50)
            {
                SceneManager.LoadScene("Combat_Event");
            }
            else
            {
                SceneManager.LoadScene("Doppelganger_Event");
            }
        }
        if (event_name == "Mystery")
        {
            int rng = Random.Range(0, 99);
            if (rng < 49)
            {
                SceneManager.LoadScene("SpikeTrap_Event");
            }
            else if (rng >= 49)
            {
                SceneManager.LoadScene("Doppelganger_Event");
            }
        }
        if (event_name == "Real_Danger")
        {
            SceneManager.LoadScene("Combat_Event");
        }
        if (event_name == "Social")
        {

        }
        if (event_name == "Story")
        {

        }
        if (event_name == "End")
        {
            //Switch case statement for checking which node biome is completed
			switch (WorldMovementScript.WorldMapDataClass.Region_Name)
			{
			case "ForestNode(Clone)":
                WorldMovementScript.WorldMapDataClass.ForestCompleted = true;
				break;

			case "MountainNode(Clone)":
                WorldMovementScript.WorldMapDataClass.MountainsCompleted = true;
				break;

			case "PlainsNode(Clone)":
                WorldMovementScript.WorldMapDataClass.PlainsCompleted = true;
				break;

			case "DesertNode(Clone)":
                WorldMovementScript.WorldMapDataClass.DesertCompleted = true;
				break;
			}
            //Turn on bool to indicate that the PC has returned from the level
            WorldMovementScript.ReturnedFromLevel = true;

            //Load the world map
            SceneManager.LoadScene("World_Map");

        }
    }
}