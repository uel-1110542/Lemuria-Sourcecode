using UnityEngine;

public class TL_CheckPlacement : MonoBehaviour {

    //Variable
    public bool CorrectPlace;
    private TL_PushObject MovableObjScript;



    void OnTriggerStay(Collider obj)
    {
        //If the tag of the object is a movable object
        if (obj.tag == "MovableObj")
        {
            //Obtain the script from the collided object
            MovableObjScript = obj.gameObject.GetComponent<TL_PushObject>();

            //If the moved object is not moving, set the bool to true
            if (!MovableObjScript.Moving)
            {
                CorrectPlace = true;
            }
        }
    }

    void OnTriggerExit(Collider obj)
    {
        //If the tag of the object is a movable object
        if (obj.tag == "MovableObj")
        {
            //Set the bool to false
            CorrectPlace = false;
        }
    }

}
