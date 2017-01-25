using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class TL_MainMenu : MonoBehaviour {

	public GameObject TitleLogo;
	public GameObject TouchMessage;
	public GameObject MainMenuGroup;
	public GameObject CharacterGroup;
	public GameObject OptionsGroup;
    public GameObject DeletePrompt;
    public GameObject PrevButton;

    private Button ContinueButton;
    private Button DeleteButton;

    private Text ContinueText;
    private Text DeleteText;

	private Color Logo_Alpha;
	public bool StopFading;


    void Start()
    {
        //Finds the gameobject and obtains the script
        DN_Collectables CollectablesScript = GameObject.Find("Collectables_Manager").GetComponent<DN_Collectables>();

        //Obtain the following button and text components
        ContinueButton = MainMenuGroup.transform.FindChild("ContinueGameButton").GetComponent<Button>();
        ContinueText = MainMenuGroup.transform.FindChild("ContinueGameButton").GetComponentInChildren<Text>();

        DeleteButton = OptionsGroup.transform.FindChild("DeleteButton").GetComponent<Button>();
        DeleteText = OptionsGroup.transform.FindChild("DeleteButton").GetComponentInChildren<Text>();
        
		//Loads the collectables
		CollectablesScript.LoadCollectables();
    }

	void Update()
	{
		FadingLogo();
		TouchScreen();
		CheckPrevSaveFiles();
	}

	void CheckPrevSaveFiles()
	{
		//Disable the buttons and gray out the text if the following files in the if statement do not exist
		if (!File.Exists(Application.persistentDataPath + "/SavedNodeMap.sg") || !File.Exists(Application.persistentDataPath + "/SavedProgress.sg") || !File.Exists(Application.persistentDataPath + "/SavedCollectables.sg"))
		{            
			ContinueButton.enabled = false;
			ContinueText.color = Color.gray;

			DeleteButton.enabled = false;
			DeleteText.color = Color.gray;
		}
		else
		{
			ContinueButton.enabled = true;
			ContinueText.color = Color.black;

			DeleteButton.enabled = true;
			DeleteText.color = Color.black;
		}
	}

	void TouchScreen()
	{
		//If the player has touched the screen and the touch message is still active, disable the message and enable the main menu
		if (Input.GetMouseButtonDown(0) && TouchMessage.activeInHierarchy)
		{
            //Set the bool to true
			StopFading = true;

            //Obtain the color from the title 
			Logo_Alpha = TitleLogo.GetComponent<Image>().color;

            //Set the new color values
			TitleLogo.GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);

            //Set the message to false
			TouchMessage.SetActive(false);

            //Set the main menu to true
			MainMenuGroup.SetActive(true);
		}
	}

	void FadingLogo()
	{
        //Obtain the color from the title logo
		Logo_Alpha = TitleLogo.GetComponent<Image>().color;

        //If the bool is false
		if(!StopFading)
		{
            //Add the alpha over time with Time.deltaTime
			Logo_Alpha.a += 0.25f * Time.deltaTime;

            //Change the alpha by obtaining the color from the title logo
			TitleLogo.GetComponent<Image>().color = new Color(255f, 255f, 255f, Logo_Alpha.a);
            
            //If the alpha value has reached to max
			if(Logo_Alpha.a > 1f)
			{
                //Set the touch message to true
				TouchMessage.SetActive(true);

                //Set the bool to true
				StopFading = true;
			}
		}
	}

	public void NewGameButton()
	{
		//Hide the title logo
		TitleLogo.SetActive(false);
		
        //Deactivate the main menu buttons
		MainMenuGroup.SetActive(false);

        //Activate the character selection buttons
		CharacterGroup.SetActive(true);

        //Activate the previous button
        PrevButton.SetActive(true);
    }

	public void SelectVadinho()
	{
        //Disables the character selection
		CharacterGroup.SetActive(false);

        //Sets the PC's name to a playerpref temporary
        PlayerPrefs.SetString("PC Name", "Vadinho(Clone)");

        //Loads up the world map
		SceneManager.LoadScene("World_Map");
	}

	public void SelectZofia()
	{
        //Disables the character selection
        CharacterGroup.SetActive(false);

        //Sets the PC's name to a playerpref temporary
        PlayerPrefs.SetString("PC Name", "Zofia(Clone)");

        //Loads up the world map
        SceneManager.LoadScene("World_Map");
	}

	public void SelectWu()
	{
        //Disables the character selection
        CharacterGroup.SetActive(false);

        //Sets the PC's name to a playerpref temporary
        PlayerPrefs.SetString("PC Name", "Wu(Clone)");

        //Loads up the world map
        SceneManager.LoadScene("World_Map");
	}

	public void OptionsButton()
	{
        //Disables the character selection
        MainMenuGroup.SetActive(false);

        //Sets the player preferences to a string with the PC's name
        OptionsGroup.SetActive(true);

        //Loads up the world map
        PrevButton.SetActive(true);
    }

	public void PreviousButton()
	{
        //Enables the main menu selection and disables everything else
		TitleLogo.SetActive(true);
		MainMenuGroup.SetActive(true);
		OptionsGroup.SetActive(false);
		CharacterGroup.SetActive(false);
        DeletePrompt.SetActive(false);
        PrevButton.SetActive(false);
    }

    public void Delete()
    {
        //Turns on the delete message prompt
        DeletePrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        //Turns off the delete message prompt
        DeletePrompt.SetActive(false);
    }

}
