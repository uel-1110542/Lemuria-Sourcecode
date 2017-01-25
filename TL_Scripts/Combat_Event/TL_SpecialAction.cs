using UnityEngine;
using System.Collections;

public class TL_SpecialAction : MonoBehaviour {

    //Variables
    private GameObject PC;
    private float Cooldown;
    private bool ButtonPressed = false;
    

    void Update()
    {
        //Function for setting the special effect
        SpecialEffect();
    }

	public void ActivateSpecial()
    {
        //The button is pressed so the bool is true
        ButtonPressed = true;

        //Finds the PC gameobject
        PC = GameObject.FindGameObjectWithTag("PC");

        //If the PC isn't dead
        if (PC != null)
        {
            //If the name of the PC is either Vadinho or Wu
            if (PC.name == "Vadinho(Clone)" || PC.name == "Wu(Clone)")
            {
                //Find the gameobject with the tag
                GameObject HitBox = GameObject.FindGameObjectWithTag("Special");

                //Enable the box collider
                HitBox.GetComponent<BoxCollider>().enabled = true;
            }
            else if (PC.name == "Zofia(Clone)")     //If the PC's name is Zofia
            {
                //Obtain the move script from the PC
                TL_MoveScript MoveScript = PC.GetComponent<TL_MoveScript>();

                //If the PC's new Z position is still within boundaries
                if ((MoveScript.CurrentPos.z - 2f) >= 0)
                {
                    //Set new PC position
                    PC.transform.position = new Vector3(MoveScript.CurrentPos.x, MoveScript.CurrentPos.y, MoveScript.CurrentPos.z - 2f);

                    //Set new Z position for the target position
                    MoveScript.TargetPos.z -= 2f;
                }
            }

        }       

	}

    void SpecialEffect()
    {
        //If the PC isn't dead
        if (PC != null)
        {
            //Find all of the gameobjects with the tag called Special
            GameObject[] Special_Hitbox = GameObject.FindGameObjectsWithTag("Special");

            //If the PC's name is Zofia
            if (PC.name == "Zofia(Clone)")
            {
                //If the button is pressed and the cooldown is less than the startup time
                if (ButtonPressed && Cooldown < Time.realtimeSinceStartup)
                {
                    //Activate the box collider from the list and deactivate the script that follows the PC
                    foreach (GameObject go in Special_Hitbox)
                    {
                        go.GetComponent<BoxCollider>().enabled = true;
                        go.GetComponent<TL_FollowChar>().enabled = false;
                    }
                    //Sets bool to false
                    ButtonPressed = false;

                    //Increases the cooldown
                    Cooldown = 1f + Time.realtimeSinceStartup;
                }
                else
                {
                    //Deactivate the box collider from the list and activate the script that follows the PC
                    foreach (GameObject go in Special_Hitbox)
                    {
                        go.GetComponent<BoxCollider>().enabled = false;
                        go.GetComponent<TL_FollowChar>().enabled = true;
                    }
                }
            }
            else if (PC.name == "Wu(Clone)")        //If the PC's name is Wu
            {
                //If the button is pressed and the cooldown is less than the startup time
                if (ButtonPressed && Cooldown < Time.realtimeSinceStartup)
                {
                    //Activate the box collider from the list
                    foreach (GameObject go in Special_Hitbox)
                    {
                        go.GetComponent<BoxCollider>().enabled = true;
                    }
                    //Sets bool to false
                    ButtonPressed = false;

                    //Increases the cooldown
                    Cooldown = 1f + Time.realtimeSinceStartup;
                }
            }
        }

    }

}
