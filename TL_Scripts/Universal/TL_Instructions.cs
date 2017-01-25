using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TL_Instructions : MonoBehaviour {

	//Variables
	public GameObject PauseScreen;
	public bool PauseToggle = true;
    private GameObject LevelArea;


    void Start()
    {
        //Activate/Deactivate the pause screen based on the toggle
        PauseScreen.SetActive(PauseToggle);

        //Handles turning on and off the scripts
        ToggleScripts();
    }

	void Update()
	{
        //Activate/Deactivate the pause screen based on the toggle
        PauseScreen.SetActive(PauseToggle);

        //Handles turning on and off the scripts
        ToggleScripts();
    }

	void ToggleScripts()
	{
        //Finds the level area gameobject
        LevelArea = GameObject.Find("LevelArea");

        //Find the text component from the child
        Text TitleText = PauseScreen.transform.FindChild("Title").GetComponent<Text>();
        Text InstructionsText = PauseScreen.transform.FindChild("Instructions").GetComponent<Text>();

        //Find the PC
        GameObject PC = GameObject.FindGameObjectWithTag("PC");

        //If the active scene is not the sokoban event
        if (SceneManager.GetActiveScene().name != "Sokoban_Event")
        {
            //Obtains the timer script and disables it
            TL_Timer TimerScript = LevelArea.GetComponent<TL_Timer>();
            TimerScript.enabled = !PauseToggle;
        }

        //Finds all of the gameobjects depending on the scene and disables them when the pause is toggled
        switch (SceneManager.GetActiveScene().name)
		{
            case "Combat_Event":
                //If the PC is not dead
                if (PC != null)
                {
                    //Obtain the script from the PC and switch the script on or off depending on the boolean toggle
                    TL_PCMove PCMoveScript = PC.GetComponent<TL_PCMove>();
                    PCMoveScript.enabled = !PauseToggle;
                }

                //Find the NPC
                GameObject NPC = GameObject.FindGameObjectWithTag("NPC");

                //If the NPC is not dead
                if (NPC != null)
                {
                    //Obtain the script from the NPC and switch the script on or off depending on the boolean toggle
                    TL_NPCMovement NPCMoveScript = NPC.GetComponent<TL_NPCMovement>();
                    NPCMoveScript.enabled = !PauseToggle;
                }
                //Fill in the text depending on the active scene
                TitleText.text = "Combat";
                InstructionsText.text = "- Move the character by pressing the squares adjacent from the character. \n \n - Attack the enemy by moving the hitbox inside the enemy and moving forward.";

                break;

		    case "Doppelganger_Event":
                    //Find the doppelganger
                    GameObject Doppelganger = GameObject.Find("Doppelganger(Clone)");

                    //If the doppelganger is not dead
                    if (Doppelganger != null)
                    {
                        //Obtain the script from the doppelganger and switch the script on or off depending on the boolean toggle
                        TL_MoveScript DoppelgangerMoveScript = Doppelganger.GetComponent<TL_MoveScript>();
                        DoppelgangerMoveScript.enabled = !PauseToggle;
                    }
                    //Fill in the text depending on the active scene
                    TitleText.text = "Doppelganger";
                    InstructionsText.text = "- Move the character by pressing the squares adjacent from the character. \n \n - The doppelganger will move in the opposite direction of your character. \n \n - Get the doppelganger to move into the correct flashing runes in order.";
                    break;

		    case "Sokoban_Event":
                    //If the PC is not dead
                    if (PC != null)
                    {
                        //Obtain the script from the PC and switch the script on or off depending on the boolean toggle
                        TL_MoveScript MoveScript = PC.GetComponent<TL_MoveScript>();
                        MoveScript.enabled = !PauseToggle;
                    }

                    //Find all of the movable objects
                    GameObject[] MovableObjs = GameObject.FindGameObjectsWithTag("MovableObj");

                    //Loop through the movable objects and switch them on or off depending on the boolean toggle
                    foreach (GameObject go in MovableObjs)
                    {
                        //Obtain the script from the movable object and switch the script on or off depending on the boolean toggle
                        TL_PushObject ObjScript = go.GetComponent<TL_PushObject>();
                        ObjScript.enabled = !PauseToggle;
                    }
                    //Fill in the text depending on the active scene
                    TitleText.text = "Sokoban";
                    InstructionsText.text = "- Move the character by pressing the squares adjacent from the character. \n \n - Move the boxes into the goal posts.";
                    break;

		    case "SpikeTrap_Event":
                    //Find all of the spikes
                    GameObject[] Spikes = GameObject.FindGameObjectsWithTag("Spikes");

                    //Loop through the spikes and switch them on or off depending on the boolean toggle
                    foreach (GameObject go in Spikes)
                    {
                        //Obtain the script from the spikes and switch the script on or off depending on the boolean toggle
                        TL_AnimateSpikes SpikesScript = go.GetComponent<TL_AnimateSpikes>();
                        SpikesScript.enabled = !PauseToggle;
                    }
                    //Fill in the text depending on the active scene
                    TitleText.text = "Spike Traps";
                    InstructionsText.text = "- Move the character by pressing the squares adjacent from the character. \n \n - Get to the end goal without touching the spikes. \n \n - Touching the spikes will result in failing this event.";
                    break;
		}
	}

	public void PauseButton()
	{
        //Toggles the boolean true to false and vice versa
		PauseToggle = !PauseToggle;
	}

}
