using UnityEngine;

public class TL_AttackChar : MonoBehaviour {

    //Variables
    public float AttackRate = 1f;
    public float AttackCooldown = 1f;
    private GameObject PC;
    private GameObject NPC;
    private TL_CharStats CharacterScript;
    private TL_NPCMovement NPCScript;



    void Start()
    {
        //Adds the cooldowns so the NPC doesn't react immediately
        AttackCooldown = AttackRate + Time.realtimeSinceStartup;
    }
    
    void OnTriggerStay(Collider Col)
    {
        //If the collided gameobject is the PC and the cooldown is less than the time since startup
        if (Col.transform.tag == "PC" && AttackCooldown < Time.realtimeSinceStartup)
        {
            //Finds the NPC gameobject by the tag
            NPC = GameObject.FindGameObjectWithTag("NPC");
			if (NPC != null)
			{
				//Obtains the script from the NPC
				NPCScript = NPC.GetComponent<TL_NPCMovement>();
                
                //Sets the state to Attack
                NPCScript.SetState("Attack");

				//Obtains the script from the NPC again
				CharacterScript = NPC.transform.gameObject.GetComponent<TL_CharStats>();

				//Sets the variable to the attack value of the NPC
				int NPC_Attack = CharacterScript.AttackValue;

				//Obtains the script from the collided gameobject which is the PC
				CharacterScript = Col.transform.gameObject.GetComponent<TL_CharStats>();
                TL_PCMove PCScript = Col.transform.gameObject.GetComponent<TL_PCMove>();

                //Start the coroutine function
                SpriteRenderer PC_Sprite = Col.gameObject.GetComponent<SpriteRenderer>();
                StartCoroutine(PCScript.SpriteFlash(PC_Sprite));

                //Pass through the values of the gameobject and the NPC attack to the PC
                CharacterScript.ReceiveDamage(Col.transform.gameObject, NPC_Attack);
                
                //Adds the cooldown
                AttackCooldown = AttackRate + Time.realtimeSinceStartup;
			}
            
        }

    }

    void OnTriggerExit(Collider Col)
    {
        //If the collided gameobject is the PC
        if (Col.transform.tag == "PC")
        {
            //Obtain the script from the NPC
            NPCScript = GameObject.FindGameObjectWithTag("NPC").GetComponent<TL_NPCMovement>();

            //Set the state to move
            NPCScript.SetState("Move");
        }
    }

}
