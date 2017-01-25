using UnityEngine;
using System.Collections;

public class TL_SpecialEffect : MonoBehaviour {


    private GameObject PC;
    private float Cooldown = 2f;
    private bool IsEnemyStunned = false;
    

    void Update()
    {
        StunEffect();
    }

    void StunEffect()
    {
        //Finds the gameobject with the PC tag
        PC = GameObject.FindGameObjectWithTag("PC");

        //If the PC is Wu and the enemy is stunned
        if (PC != null && PC.name == "Wu(Clone)" && IsEnemyStunned)
        {
            //Obtain the script from the NPC
            TL_NPCMovement NPCScript = GameObject.FindGameObjectWithTag("NPC").GetComponent<TL_NPCMovement>();

            //Reduce the cooldown
            Cooldown -= Time.deltaTime;
            Debug.Log(Cooldown);

            //If the cooldown has not ran out, the enemy is still stunned
            if (Cooldown >= 0f)
            {
                Debug.Log("Stunned!");
                NPCScript.enabled = false;
            }
            else
            {
                //If the cooldown has ran out, the enemy is no longer stunned
                Debug.Log("Not Stunned!");
                NPCScript.enabled = true;
                IsEnemyStunned = false;

                //Reset the cooldown
                Cooldown = 2f;
            }
        }
                
    }

    void OnTriggerEnter(Collider Col)
    {
        //Finds the gameobject with the PC tag
        PC = GameObject.FindGameObjectWithTag("PC");

        //If the collided tag is a projectile and the PC is Vadinho
        if (Col.transform.tag == "Projectile" && PC.name == "Vadinho(Clone)")
        {
            //Disable the box collider
            transform.gameObject.GetComponent<BoxCollider>().enabled = false;

            //Destroy the projectile
            Destroy(Col.gameObject);
        }

    }

    void OnTriggerStay(Collider Col)
    {
        //If the collided tag is the NPC and the PC's name is Wu and the enemy is not stunned
        if (Col.transform.tag == "NPC" && PC.name == "Wu(Clone)" && !IsEnemyStunned)
        {
            //Stun the enemy and set the bool to true
            IsEnemyStunned = true;
        }
    }

}
