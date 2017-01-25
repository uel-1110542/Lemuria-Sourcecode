using UnityEngine;

public class TL_WorldBackgroundManager : MonoBehaviour {

	//Variables
	public Sprite PlainsBG;
	public Sprite DesertBG;
	public Sprite ForestBG;
	public Sprite MountainsBG;
	private TL_PC_World_Movement WorldMovementScript;


	void Awake()
	{
        //Find the game manager and obtain the script
		WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

        //Select the image based on the selected region by their name
        SelectBG(WorldMovementScript.WorldMapDataClass.Region_Name);
	}

	void SelectBG(string region)
	{
        //The switch case statement will obtain the sprite renderer sprite and match the sprites accordingly to the world region it was selected
        switch (region)
		{
			case "ForestNode(Clone)":
				GetComponent<SpriteRenderer>().sprite = ForestBG;
				break;

			case "DesertNode(Clone)":
				GetComponent<SpriteRenderer>().sprite = DesertBG;
				break;

			case "PlainsNode(Clone)":
				GetComponent<SpriteRenderer>().sprite = PlainsBG;
				break;

			case "MountainNode(Clone)":
				GetComponent<SpriteRenderer>().sprite = MountainsBG;
				break;
		}
	}

}
