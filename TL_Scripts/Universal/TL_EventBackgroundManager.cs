using UnityEngine;

public class TL_EventBackgroundManager : MonoBehaviour {

    //Variables
    public Sprite PlainsImage;
    public Sprite ForestImage;
    public Sprite DesertImage;
    public Sprite MountainsImage;
    private TL_PC_World_Movement WorldMovementScript;


    void Awake()
    {
        //Find the game manager and obtain the script
        WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

        //Select the image based on the selected region by their name
        SelectImage(WorldMovementScript.WorldMapDataClass.Region_Name);
    }


    void SelectImage(string region)
    {
        //The switch case statement will obtain the sprite renderer sprite and match the sprites accordingly to the world region it was selected
        switch (region)
        {
            case "ForestNode(Clone)":
                GetComponent<SpriteRenderer>().sprite = ForestImage;
                break;

            case "DesertNode(Clone)":
                GetComponent<SpriteRenderer>().sprite = DesertImage;
                break;

            case "PlainsNode(Clone)":
                GetComponent<SpriteRenderer>().sprite = PlainsImage;
                break;

            case "MountainNode(Clone)":
                GetComponent<SpriteRenderer>().sprite = MountainsImage;
                break;
        }
    }


}
