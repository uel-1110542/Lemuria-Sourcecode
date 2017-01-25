﻿using UnityEngine;

public class TL_NPCMovement : MonoBehaviour {

	//Variables	
	private float RateOfMove = 1.5f;
	private float MoveCooldown = 1.5f;
    private float RateOfStateChange = 5f;
    private float StateChangeCooldown = 5f;
	private float Min_Range;
	private float Max_Range;
    public float PrevHP;

    private GameObject PC;
    private TL_ActiveTimeBattle CombatManagerScript;
    private TL_CharStats CharacterScript;
    private TL_GridManager GridScript;

    public string NPC_State = "Move";
    public GameObject MergedEnemy;
    


	void Start ()
	{
        //Find the level area gameobject
        GameObject LevelArea = GameObject.Find("LevelArea");

        //Obtain the script from the level area
        CombatManagerScript = LevelArea.GetComponent<TL_ActiveTimeBattle>();
        GridScript = LevelArea.GetComponent<TL_GridManager>();

        //Obtain the script from this gameobject
        CharacterScript = gameObject.GetComponent<TL_CharStats>();

        //Set the variable to its' default value
        PrevHP = CharacterScript.CurrentHealth;

        //Adds the cooldown so the NPC doesn't react immediately
        MoveCooldown = RateOfMove + Time.realtimeSinceStartup;
        StateChangeCooldown = RateOfStateChange + Time.realtimeSinceStartup;
    }

    void Update()
    {
        //Function for moving the NPC
		Move();

        //Function for checking health
        CheckHealth();
    }

    void CheckHealth()
    {
        //if this gameobject is the pixie
        if (gameObject.name == "Pixie(Clone)")
        {
            //Obtain the script from this gameobject
            CharacterScript = gameObject.GetComponent<TL_CharStats>();

            //If the previous HP value is not the same as its' current health
            if (PrevHP != CharacterScript.CurrentHealth)
            {
                //Set the state to teleport to prevent any other actions
                NPC_State = "Teleport";

                //Randomize both the X and Z values
                int Ran_X = Random.Range(0, GridScript.ReturnLevelAreaArray().GetLength(0));
                int Ran_Z = Random.Range(0, GridScript.ReturnLevelAreaArray().GetLength(1));

                //if the X and Z values are null in the 2D gameobject array
                if (GridScript.ReturnLevelAreaArray()[Ran_X, Ran_Z] == null)
                {
                    //Set the previous position in the array null
                    GridScript.SetGOInLevelArea(null, (int)transform.position.x, (int)transform.position.z);

                    //Teleport this gameobject
                    transform.position = new Vector3(Ran_X, 0.1f, Ran_Z);

                    //Set the new position in the array as this gameobject
                    GridScript.SetGOInLevelArea(gameObject, Ran_X, Ran_Z);
                }
                //Set the state to move
                NPC_State = "Move";

                //Update the previous value with its' current health
                PrevHP = CharacterScript.CurrentHealth;
            }

        }
        
    }

    //Sets current state
    public void SetState(string state)
    {
        NPC_State = state;
    }

    //Returns current state
    public string ReturnState()
    {
        return NPC_State;
    }

    void Move()
    {
		//Randomly selects which axis to move in
		int RanNum = Random.Range(0, 2);

        //Finds the PC gameobject
		GameObject PC = GameObject.FindGameObjectWithTag("PC");

        if (MoveCooldown < Time.realtimeSinceStartup)
        {
            //Variable for obtaining the script
            TL_MoveOffset OffsetScript;

            //Calculate the distance of X between the PC and itself
			float X_Dist = Mathf.Round(PC.transform.position.x) - Mathf.Round(transform.position.x);

            //Variable for obtaining the sprite renderer
			SpriteRenderer EnemySprite;

            //If the distance of X is more than 1, flip the sprite, if not, don't flip it
			if (X_Dist >= 1f)
			{
				EnemySprite = GetComponent<SpriteRenderer>();
				EnemySprite.flipX = true;
			}
			else
			{
				EnemySprite = GetComponent<SpriteRenderer>();
				EnemySprite.flipX = false;
			}

            if (NPC_State == "Move")
            {
                //Rounds up the X value in the for loop and Z value in the PC position and calculates both the differences of X and Z
                float X_Difference = Random.Range(-1, 2) + Mathf.Round(transform.position.x);
                float Z_Difference = Random.Range(-1, 2) + Mathf.Round(transform.position.z);

                switch (RanNum)
                {
                    case 0:
                        if (X_Difference > -1 && X_Difference < GridScript.ReturnLevelAreaArray().GetLength(0) && GridScript.ReturnGOInLevelArea((int)X_Difference, (int)Mathf.Round(transform.position.z)) == null)
                        {
                            //Set previous position in the grid as null
                            GridScript.SetGOInLevelArea(null, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

                            //Obtain the script from the level area where the NPC is about to move to
                            OffsetScript = CombatManagerScript.ReturnGOInLevelMovement((int)X_Difference, (int)Mathf.Round(transform.position.z)).GetComponent<TL_MoveOffset>();

                            //Move towards the new position
                            transform.position = Vector3.MoveTowards(transform.position, new Vector3(OffsetScript.X_Offset, transform.position.y, OffsetScript.Z_Offset), 5f);

                            //Set the new position in the grid by this gameobject
                            GridScript.SetGOInLevelArea(transform.gameObject, (int) Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

                            //Adds the cooldown based on the value of the variable plus the realtimeSinceStartup
                            MoveCooldown = RateOfMove + Time.realtimeSinceStartup;
                        }
                        break;

                    case 1:
                        if (Z_Difference > -1f && Z_Difference < GridScript.ReturnLevelAreaArray().GetLength(1) && GridScript.ReturnGOInLevelArea((int)Mathf.Round(transform.position.x), (int)Z_Difference) == null)
                        {
                            //Set previous position in the grid as null
                            GridScript.SetGOInLevelArea(null, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

                            //Obtain the script from the level area where the NPC is about to move to
                            OffsetScript = CombatManagerScript.ReturnGOInLevelMovement((int)Mathf.Round(transform.position.x), (int)Z_Difference).GetComponent<TL_MoveOffset>();

                            //Move towards the new position
                            transform.position = Vector3.MoveTowards(transform.position, new Vector3(OffsetScript.X_Offset, transform.position.y, OffsetScript.Z_Offset), 5f);

                            //Set the new position in the grid by this gameobject
                            GridScript.SetGOInLevelArea(transform.gameObject, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

                            //Adds the cooldown based on the value of the variable plus the realtimeSinceStartup
                            MoveCooldown = RateOfMove + Time.realtimeSinceStartup;
                        }
                        break;
                }

                //If the cooldown for changing states is less than the time since startup
                if (StateChangeCooldown < Time.realtimeSinceStartup)
                {
                    //Set state to chase
                    SetState("Chase");

                    //Adds the cooldown based on the value of the variable plus the realtimeSinceStartup
                    StateChangeCooldown = RateOfStateChange + Time.realtimeSinceStartup;
                }
            }
            else if (NPC_State == "Chase")
            {
                //Calculate the distance of X and Z between itself and the target
                float X_Target = 0;
                float Z_Target = 0;

                //If this gameobject is the plume
                if (gameObject.name == "Plume(Clone)")
                {
                    //Find another plume
                    GameObject Merger = GameObject.Find("Plume(Clone)");

                    //If the other plume isn't dead
                    if (Merger != null)
                    {
                        //Calculate the distance between the target and itself
                        X_Target = Mathf.Round(Merger.transform.position.x) - Mathf.Round(transform.position.x);
                        Z_Target = Mathf.Round(Merger.transform.position.z) - Mathf.Round(transform.position.z);
                    }                    
                }
                else
                {
                    //Calculate the distance between the target and itself
                    X_Target = Mathf.Round(PC.transform.position.x) - Mathf.Round(transform.position.x);
                    Z_Target = Mathf.Round(PC.transform.position.z) - Mathf.Round(transform.position.z);
                }
                
                //If the target is adjacent to this gameobject
                if (X_Target == -1f && Z_Target == 0 || X_Target == 1f && Z_Target == 0 || X_Target == 0f && Z_Target == -1f || X_Target == 0f && Z_Target == 1f)
                {
                    //If this gameobject is the midget grappler
                    if (transform.gameObject.name == "Midget_Grappler(Clone)")
                    {
                        //Set bool to true
                        NPC_State = "Grab";
                    }
                    else if (transform.gameObject.name == "Plume(Clone)")
                    {
                        //Spawn the merged plume
                        GameObject MergedClone = (GameObject) Instantiate (MergedEnemy, transform.position, Quaternion.identity);

                        //Set initial rotation
                        MergedClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

                        //Destroy both the other plume and itself
                        Destroy(GameObject.Find("Plume(Clone)"));
                        Destroy(gameObject);
                    }
                }                

                switch (RanNum)
                {
                    case 0:
                        if (X_Target >= 1 && GridScript.ReturnGOInLevelArea((int)transform.position.x + 1, (int)Mathf.Round(transform.position.z)) == null)
                        {
                            //Set previous position in the grid as null
                            GridScript.SetGOInLevelArea(null, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

                            //Step upwards by 1 square
                            X_Target = transform.position.x + 1f;

                            //Obtain the script from the level area where the NPC is about to move to
                            OffsetScript = CombatManagerScript.ReturnGOInLevelMovement((int)Mathf.Round(X_Target), (int)Mathf.Round(transform.position.z)).GetComponent<TL_MoveOffset>();

                            //Move towards the new position
                            transform.position = Vector3.MoveTowards(transform.position, new Vector3(OffsetScript.X_Offset, transform.position.y, OffsetScript.Z_Offset), 5f);

                            //Set the new position in the grid by this gameobject
                            GridScript.SetGOInLevelArea(transform.gameObject, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));
                        }
                        else if (X_Target <= -1 && GridScript.ReturnGOInLevelArea((int)Mathf.Round(transform.position.x - 1), (int)Mathf.Round(transform.position.z)) == null)
                        {
                            //Set previous position in the grid as null
                            GridScript.SetGOInLevelArea(null, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

                            //Step downwards by 1 square
                            X_Target = transform.position.x - 1f;

                            //Obtain the script from the level area where the NPC is about to move to
                            OffsetScript = CombatManagerScript.ReturnGOInLevelMovement((int)Mathf.Round(X_Target), (int)Mathf.Round(transform.position.z)).GetComponent<TL_MoveOffset>();

                            //Move towards the new position
                            transform.position = Vector3.MoveTowards(transform.position, new Vector3(OffsetScript.X_Offset, transform.position.y, OffsetScript.Z_Offset), 5f);

                            //Set the new position in the grid by this gameobject
                            GridScript.SetGOInLevelArea(transform.gameObject, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));
                        }
                        else if (X_Target == 0)
                        {
                            //If the NPC is on the same X position as its' target, change it to move the NPC on its' Z position
                            RanNum = 1;
                        }
                        break;

                    case 1:
                        if (Z_Target >= 1 && GridScript.ReturnGOInLevelArea((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z + 1)) == null)
                        {
                            //Set previous position in the grid as null
                            GridScript.SetGOInLevelArea(null, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

                            //Step towards the right by 1 square
                            Z_Target = transform.position.z + 1f;

                            //Obtain the script from the level area where the NPC is about to move to
                            OffsetScript = CombatManagerScript.ReturnGOInLevelMovement((int)Mathf.Round(transform.position.x), (int)Mathf.Round(Z_Target)).GetComponent<TL_MoveOffset>();

                            //Move towards the new position
                            transform.position = Vector3.MoveTowards(transform.position, new Vector3(OffsetScript.X_Offset, transform.position.y, OffsetScript.Z_Offset), 5f);

                            //Set the new position in the grid by this gameobject
                            GridScript.SetGOInLevelArea(transform.gameObject, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(Z_Target));
                        }
                        else if (Z_Target <= -1 && GridScript.ReturnGOInLevelArea((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z - 1)) == null)
                        {
                            //Set previous position in the grid as null
                            GridScript.SetGOInLevelArea(null, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

                            //Step towards the left by 1 square
                            Z_Target = transform.position.z - 1f;

                            //Obtain the script from the level area where the NPC is about to move to
                            OffsetScript = CombatManagerScript.ReturnGOInLevelMovement((int)Mathf.Round(transform.position.x), (int)Mathf.Round(Z_Target)).GetComponent<TL_MoveOffset>();

                            //Move towards the new position
                            transform.position = Vector3.MoveTowards(transform.position, new Vector3(OffsetScript.X_Offset, transform.position.y, OffsetScript.Z_Offset), 5f);

                            //Set the new position in the grid by this gameobject
                            GridScript.SetGOInLevelArea(transform.gameObject, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(Z_Target));
                        }
                        else if (Z_Target == 0)
                        {
                            //If the NPC is on the same Z position as its' target, change it to move the NPC on its' X position
                            RanNum = 0;
                        }
                        break;
                }

                //Adds the cooldown based on the value of the variable plus the realtimeSinceStartup
                MoveCooldown = RateOfMove + Time.realtimeSinceStartup;

            }

        }               

    }

}
