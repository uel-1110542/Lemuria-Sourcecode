using UnityEngine;
using System.Collections.Generic;

public class TL_Hitboxes : MonoBehaviour {

    //Variables
    public int X_Bounds;
    public int Z_Bounds;
    public GameObject HitBox;
	public GameObject Special_HitBox;
    public GameObject Enemy1_Hitbox;
    public GameObject Enemy2_Hitbox;
    private GameObject PC;
    private GameObject NPC;
    private float X_Offset;
    private float Z_Offset;
    public List<GameObject> Special_Hitboxes = new List<GameObject>();
    public List<GameObject> AttackHitboxes = new List<GameObject>();
    private TL_FollowChar BoxScript;


    
    void Start()
    {
		InitializeHitBoxes();
	}

    void Update()
    {
        UpdateHitboxes();
    }

    void InitializeHitBoxes()
	{
		//Special hitboxes
		GameObject Special_HitboxClone;

        //Find the PC
		PC = GameObject.FindGameObjectWithTag ("PC");

        //Switch case statement to check which PC has been chosen
		switch (PC.name)
        {
            case "Vadinho(Clone)":
                //Set both X and Z offsets
			    X_Offset = 1;
			    Z_Offset = 1;

                //Instantiate the special hitboxes
			    Special_HitboxClone = (GameObject)Instantiate (Special_HitBox, new Vector3 (1f, 0.25f, 1f), Quaternion.identity);

                //Enable the hitbox
			    Special_HitboxClone.GetComponent<BoxCollider>().enabled = false;

                //Obtain the script from the hitbox
                BoxScript = Special_HitboxClone.GetComponent<TL_FollowChar>();

                //Make the gameobject variable equal to the PC
                BoxScript.Character = PC;

                //For loop for setting the offsets
                for (int i = -1; i < 2; i++)
                {
                    //Create the hitbox
                    GameObject HitboxClone = (GameObject)Instantiate(HitBox, new Vector3(PC.transform.position.x + i, 0.25f, PC.transform.position.z + Z_Offset), Quaternion.identity);

                    //Obtain the script from the hitbox
                    BoxScript = HitboxClone.GetComponent<TL_FollowChar>();

                    //Set the X and Z offsets based on the value in the for loop
					BoxScript.X_Pos = i;
                    BoxScript.Z_Pos = 1;

                    //Make the gameobject variable equal to the PC
                    BoxScript.Character = PC;
                }
                break;

            case "Zofia(Clone)":
                //Set both X and Z offsets
                X_Offset = 0;
			    Z_Offset = 2;

                //For loop for setting the offsets
                for (int i = -1; i < 2; i++)
			    {
                    //Instantiate the special hitboxes
                    Special_HitboxClone = (GameObject)Instantiate (Special_HitBox, new Vector3 (PC.transform.position.x + i, PC.transform.position.y, PC.transform.position.z + 1), Quaternion.identity);

                    //Enable the special hitbox
                    Special_HitboxClone.GetComponent<BoxCollider>().enabled = false;

                    //Obtain the script from the hitbox
                    BoxScript = Special_HitboxClone.GetComponent<TL_FollowChar>();

                    //Set the X and Z offsets based on the value in the for loop
                    BoxScript.X_Pos = i;
				    BoxScript.Z_Pos = 1;

                    //Make the gameobject variable equal to the PC
                    BoxScript.Character = PC;
			    }
                break;

            case "Wu(Clone)":
                //Set both X and Z offsets
                X_Offset = 0;
                Z_Offset = 1;

                //Instantiate the special hitboxes
                Special_HitboxClone = (GameObject)Instantiate(Special_HitBox, new Vector3(PC.transform.position.x, PC.transform.position.y, PC.transform.position.z + Z_Offset), Quaternion.identity);

                //Enable the special hitbox
                Special_HitboxClone.GetComponent<BoxCollider>().enabled = false;

                //Obtain the script from the hitbox
                BoxScript = Special_HitboxClone.GetComponent<TL_FollowChar>();

                //Set the X and Z offsets based on the value in the for loop
                BoxScript.X_Pos = X_Offset;
                BoxScript.Z_Pos = Z_Offset;

                //Make the gameobject variable equal to the PC
                BoxScript.Character = PC;
                break;

            default:
                break;                
		}

        NPC = GameObject.FindGameObjectWithTag("NPC");
        if (NPC != null)
        {
            switch (NPC.name)
            {
                case "Enemy1(Clone)":
                    //Set both X and Z offsets
                    X_Offset = 0;
                    Z_Offset = -1f;

                    //Create the hitbox
                    GameObject Enemy1_HitboxClone = (GameObject)Instantiate(Enemy1_Hitbox, new Vector3(transform.position.x + X_Offset, 0.25f, transform.position.z + Z_Offset), Quaternion.identity);

                    //Enable the hitbox
                    BoxScript = Enemy1_HitboxClone.GetComponent<TL_FollowChar>();

                    //Set the X and Z offsets based on the value in the for loop
                    BoxScript.X_Pos = X_Offset;
                    BoxScript.Z_Pos = Z_Offset;

                    //Make the gameobject variable equal to the NPC
                    BoxScript.Character = NPC;
                    break;

                case "Enemy2(Clone)":
                    //Set both X and Z offsets
                    X_Offset = 0;
                    Z_Offset = 0;

                    for (int x = -1; x < 2; x++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            if (x != 0 || z != 0)
                            {
                                //Create the hitbox
                                GameObject Enemy2_HitboxClone = (GameObject)Instantiate(Enemy2_Hitbox, new Vector3(NPC.transform.position.x + x, 0.25f, NPC.transform.position.z + z), Quaternion.identity);

                                //Obtain the script from the hitbox
                                BoxScript = Enemy2_HitboxClone.GetComponent<TL_FollowChar>();

                                //Set the X and Z offsets based on the value in the for loop
                                BoxScript.X_Pos = x;
                                BoxScript.Z_Pos = z;

                                //Make the gameobject variable equal to the NPC
                                BoxScript.Character = NPC;
                            }
                        }
                    }
                    break;

                case "Enemy3(Clone)":
                    //Set both X and Z offsets
                    X_Offset = 0;
                    Z_Offset = 0;

                    for (int x = -1; x < 2; x++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            if (x != 0 && z != 0)
                            {
                                //Create the hitbox
                                GameObject Enemy3_HitboxClone = (GameObject)Instantiate(Enemy2_Hitbox, new Vector3(NPC.transform.position.x + x, 0.25f, NPC.transform.position.z + z), Quaternion.identity);

                                //Obtain the script from the hitbox
                                BoxScript = Enemy3_HitboxClone.GetComponent<TL_FollowChar>();

                                //Set the X and Z offsets based on the value in the for loop
                                BoxScript.X_Pos = x;
                                BoxScript.Z_Pos = z;

                                //Make the gameobject variable equal to the NPC
                                BoxScript.Character = NPC;
                            }
                        }
                    }
                    break;
            }
        }
        //Find all of the gameobjects with the hitbox tag
        GameObject[] Hitboxes = GameObject.FindGameObjectsWithTag("Hitbox");

        //Add the hitboxes into the gameobject list
        foreach (GameObject go in Hitboxes)
        {
            AttackHitboxes.Add(go);
        }

        //Find all of the gameobjects with the special tag
        GameObject[] Special_Hitbox = GameObject.FindGameObjectsWithTag("Special");

        //Add the special hitboxes into the gameobject list
        foreach (GameObject go in Special_Hitbox)
        {
            Special_Hitboxes.Add(go);
        }
                
    }

    void UpdateHitboxes()
    {
        foreach (GameObject go in Special_Hitboxes)
        {
            TL_FollowChar SpecialBoxScript;

            //If the X and Z positions of the special hitboxes go outside the boundaries
            if (go.transform.position.x < 0f || go.transform.position.x > X_Bounds || go.transform.position.z < 0f || go.transform.position.z > Z_Bounds)
            {
                //Obtain the script from the hitbox
                SpecialBoxScript = go.GetComponent<TL_FollowChar>();

                //Disable the hitbox
                SpecialBoxScript.enabled = false;
            }
            else
            {
                //Obtain the script from the hitbox
                SpecialBoxScript = go.GetComponent<TL_FollowChar>();

                //Enable the hitbox
                SpecialBoxScript.enabled = true;
            }
        }

    }

}
