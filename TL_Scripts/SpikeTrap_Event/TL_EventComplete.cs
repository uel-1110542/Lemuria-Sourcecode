using UnityEngine;

public class TL_EventComplete : MonoBehaviour {

    

    void OnTriggerStay(Collider Col)
    {
        //Obtains the script from the collided gameobject
        TL_MoveScript MoveScript = Col.gameObject.GetComponent<TL_MoveScript>();

        //If the collided tag is the PC and the PC is not moving
        if (Col.tag == "PC" && !MoveScript.IsCharMoving)
        {
            //Obtain the timer script from this gameobject
            TL_Timer TimerScript = GameObject.Find("LevelArea").GetComponent<TL_Timer>();

            //Activate win condition
            TimerScript.CheckCondition(true);

            //Locates the level manager and obtains the script to use a function
            GameObject.Find("Level_Manager(Clone)").GetComponent<DN_Event_Manager>().WinState("Spike_Trap");
        }
    }

}
