﻿using UnityEngine;

public class TL_FollowChar : MonoBehaviour {

	//Variables
	public float X_Pos;
	public float Z_Pos;
    public GameObject Character;
    public GameObject Box;



	void Update()
	{
        //Function for following the character
		FollowChar();
	}

	void FollowChar()
	{
        //If the character is not dead
        if (Character != null)
		{
            //Sets the new position based on the X and Z offsets it was given to correctly follow the character it has been assigned to
			transform.position = new Vector3(X_Pos + Character.transform.position.x, 0.15f, Z_Pos + Character.transform.position.z);
        }

	}

    void OnTriggerStay(Collider col)
    {
        //If the collider has the NPC tag
		if (col.tag == "NPC")
		{
            //Obtain the script from the PC and set the bool to true
			TL_PCMove PCScript = GameObject.FindGameObjectWithTag("PC").GetComponent<TL_PCMove>();
			PCScript.NPC_Detected = true;
		}

        //If the collided gameobject has the moveable object tag
        if (col.tag == "MovableObj")
        {
            //Make the collided gameobject equal to this gameobject variable
            Box = col.gameObject;
        }

    }

	void OnTriggerExit(Collider col)
	{
        //If the collider has the NPC tag
        if (col.tag == "NPC")
		{
            //Obtain the script from the PC and set the bool to false
            TL_PCMove PCScript = GameObject.FindGameObjectWithTag("PC").GetComponent<TL_PCMove>();
			PCScript.NPC_Detected = false;
		}

        //If the collided gameobject has the moveable object tag
        if (col.tag == "MovableObj")
        {
            //Make the gameobject variable null
            Box = null;
        }

    }

}
