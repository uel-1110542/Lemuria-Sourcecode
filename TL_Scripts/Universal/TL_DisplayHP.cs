using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TL_DisplayHP : MonoBehaviour {

    //Variables
    public GameObject HealthBar;
    public List<GameObject> NPC_Quantity = new List<GameObject>();
    public List<GameObject> NPC_HealthBars = new List<GameObject>();
    private GameObject[] NPCs;
    

    void Start()
    {
        //Find all of the NPCs
        NPCs = GameObject.FindGameObjectsWithTag("NPC");

        //Instantiates the health bar
        CreateHealthBars(HealthBar, "HealthBar", "Untagged");

        //Loop through the for loop to check for any additional NPCs to create health bars for
        for (int i = 0; i < NPCs.GetLength(0); i++)
        {
            CreateHealthBars(HealthBar, "EnemyHealthBar_" + (i + 1).ToString(), "NPC_HPBar");

            //Add current NPCs in the gameobject list
            NPC_Quantity.Add(NPCs[i]);
        }

        //Find the gameobjects with the NPC_HPBar
        GameObject[] NPC_HPBars = GameObject.FindGameObjectsWithTag("NPC_HPBar");

        //For each of the gameobjects from the NPC_HPBars, add them to the gameobject list
        foreach (GameObject go in NPC_HPBars)
        {
            NPC_HealthBars.Add(go);
        }
    }

	void Update()
    {
        //Find all of the NPCs
        NPCs = GameObject.FindGameObjectsWithTag("NPC");

        //Finds gameobjects with the tag required
        GameObject PC = GameObject.FindGameObjectWithTag("PC");        

        //Function for managing the health bars
        HealthBarManager(PC, GameObject.Find("Camera/Canvas/HealthBar"), 0.3f);

        //Function for checking the state of NPCs
        CheckNPCs();
    }

    void CheckNPCs()
    {
        //Loop through the list
        for (int i = 0; i < NPC_Quantity.Count; i++)
        {
            //If the NPC in the list is still alive
            if (NPC_Quantity[i] != null)
            {
                //Manage the health bar for that NPC
                HealthBarManager(NPC_Quantity[i], NPC_HealthBars[i], 0.65f);
            }
            else
            {
                //Remove both the health bar and the NPC from the list and destroy the health bar for that dead NPC
                NPC_Quantity.RemoveAt(i);
                Destroy(NPC_HealthBars[i]);
                NPC_HealthBars.RemoveAt(i);                
            }
        }

        //Loop through the list
        for (int i = NPCs.GetLength(0)-1; i > 0; i--)
        {
            if (!NPC_Quantity.Contains(NPCs[i]))
            {
                CreateHealthBars(HealthBar, "EnemyHealthBar_" + (i + 1).ToString(), "NPC_HPBar");
                NPC_Quantity.Add(NPCs[i]);
                NPC_HealthBars.Add(GameObject.Find("EnemyHealthBar_" + (i + 1).ToString()));
            }
        }
    }

    void CreateHealthBars(GameObject HP_Bar, string HP_Name, string HP_Tag)
    {
        //Instantiates the healthbar background
        GameObject HealthBarClone = (GameObject)Instantiate(HP_Bar, Vector3.zero, Quaternion.identity);

        //Set name for the health bar
        HealthBarClone.name = HP_Name;

        //Set tag for the health bar
        HealthBarClone.tag = HP_Tag;

        //Sets the parent and resets local position and rotation
        HealthBarClone.transform.SetParent(Camera.main.transform.FindChild("Canvas"));
        HealthBarClone.transform.localPosition = Vector3.zero;
        HealthBarClone.transform.localScale = new Vector3(1f, 1f, 1f);
        HealthBarClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        HealthBarClone.transform.SetSiblingIndex(2);
    }

    void HealthBarManager(GameObject Character, GameObject HP_Bar, float Y_Pos)
    {
        //Find the canvas and obtain the rect transform
        RectTransform CanvasRect = GameObject.Find("Camera/Canvas").GetComponent<RectTransform>();

        //If the character isn't dead
        if (Character != null)
        {
            //Obtains the script from the character
            TL_CharStats CharacterScript = Character.GetComponent<TL_CharStats>();

            //Calculates the current and max health into a %
            float CurrentHPPercent = (CharacterScript.CurrentHealth / CharacterScript.MaxHealth) - 0.25f;
            
            //Obtains the gameobject from the hierarchy
            GameObject RedHealthBar = GameObject.Find("Camera/Canvas/" + HP_Bar.name + "/RedHealthBar");

            //Convert the PC's position from world space to viewport space
            Vector2 CharViewportPos = Camera.main.WorldToViewportPoint(Character.transform.position);

            //Calculate the X and Y of the screen position from the viewport space and canvas size
            Vector2 ScreenPosForHealthBar = new Vector2((CharViewportPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f), (CharViewportPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * Y_Pos));

            //Set the pivot position to the converted Vector2
            HP_Bar.GetComponent<RectTransform>().anchoredPosition = ScreenPosForHealthBar;

            //Sets the X scale to the percentage of the Character's health
            RedHealthBar.transform.localScale = new Vector3(CurrentHPPercent, 0.7f, 1f);

            //Finds the gameobject from the hierarchy
            GameObject HealthNumberDisplay = GameObject.Find("Camera/Canvas/" + HP_Bar.name + "/HealthNumber");

            //Obtains the text component from the gameobject
            Text CharHealthDisplay = HealthNumberDisplay.GetComponent<Text>();

            //Sets the text to display the character's max and current health
            CharHealthDisplay.text = CharacterScript.CurrentHealth.ToString("F0") + " / " + CharacterScript.MaxHealth.ToString("F0");
        }
        
    }

}
