using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TL_Timer : MonoBehaviour {

	//Variables
	public GameObject DisplayTimer;
    public GameObject MessageBox;
    public GameObject PauseButton;
	public float Timer = 20f;

	private GameObject PC_Check;
	private string Message;
	private Text TimerDisplay;
    private Text OutcomeMessage;
        

    void Start()
	{
        //The sokoban event is a non-timed event so there's no need for the timer
        if (SceneManager.GetActiveScene().name != "Sokoban_Event")
        {
            //Finds and checks the PC if it's Wu then it subtracts the time
            PC_Check = GameObject.FindGameObjectWithTag("PC");
            if (PC_Check != null && PC_Check.name == "Wu(Clone)")
            {
                Timer -= 5f;
            }

            //Finds the UI component and displays the timer
            GameObject Timer_Clone = GameObject.Find("Camera/Canvas/Display");

            //Obtains the rect transform from the timer
            RectTransform TimerTransform = Timer_Clone.GetComponent<RectTransform>();

            //Sets new local position, rotation and scale
            TimerTransform.localPosition = new Vector3(5f, 300f, 0f);
            TimerTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
            TimerTransform.localScale = new Vector3(1f, 1f, 1f);

            //Grab the text component from the timer gameobject
            TimerDisplay = Timer_Clone.transform.FindChild("Timer").GetComponent<Text>();
        }
        //Locates the gameobject and obtains the text component
        OutcomeMessage = GameObject.Find("Camera/Canvas/Outcome_Message/Text").GetComponent<Text>();

    }

	void Update()
	{
        //The sokoban event is a non-timed event so there's no need for the timer
        if (SceneManager.GetActiveScene().name != "Sokoban_Event")
        {
            RunTimer();
        }
		
    }

	void RunTimer()
	{
        //If the message display for the event outcome is not active, run the timer
        if (!MessageBox.activeInHierarchy)
        {
            PC_Check = GameObject.FindGameObjectWithTag("PC");
            TimerDisplay.text = "Timer: " + Timer.ToString("F0");
            Timer -= Time.deltaTime;

            //When the timer runs out
            if (Timer <= 0f)
            {
                //Activate the message box
                MessageBox.SetActive(true);

                //If the PC is not dead
                if (PC_Check != null)
                {
                    //If Wu is the chosen character and the player is in the combat event
                    if (SceneManager.GetActiveScene().name == "Combat_Event" && PC_Check.name == "Wu(Clone)")
                    {
                        //Set bool to true
                        CheckCondition(true);
                    }
                    else
                    {
                        //Set timer to 0
                        Timer = 0f;

                        //Set bool to false
                        CheckCondition(false);
                    }
                    
                }

            }

        }

	}

    public void CheckCondition(bool IsEventCompleted)
    {
        //Deactivate the pause button
        PauseButton.SetActive(false);

        //Activate message box
        MessageBox.SetActive(true);

        //If the event is won or not, display the appropriate message
        if (IsEventCompleted)
        {
            //Display win message
            OutcomeMessage.text = "You Win!";
        }
        else
        {
            //Display lose message
            OutcomeMessage.text = "You Lose!";
        }
    }

    public void ReturnToLevelMap()
    {
        //If the current active scene is the combat event, award the player with a collectable
        if (SceneManager.GetActiveScene().name == "Combat_Event")
        {
            GameObject.Find("Level_Manager(Clone)").GetComponent<DN_Event_Manager>().WinState("Combat");
        }

        //Loads the level map
        GameObject.FindGameObjectWithTag("LM").GetComponent<DN_Event_Manager>().ReturnToMap();
    }

}
