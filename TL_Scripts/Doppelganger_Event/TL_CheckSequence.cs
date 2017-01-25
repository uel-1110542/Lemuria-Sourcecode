using UnityEngine;
using System.Collections.Generic;

public class TL_CheckSequence : MonoBehaviour {

    //Variables
    public List<GameObject> Cubes = new List<GameObject>();
    public int Index = 0;
    private TL_PuzzleScript PuzzleScript;
    

	void Start()
    {
        //Obtain the script from this gameobject
        PuzzleScript = GetComponent<TL_PuzzleScript>();
	}

    public void CheckSequence()
    {
        //If the list in the sequence is the same as the one the player selects
        if (PuzzleScript.Sequence[Index] == Cubes[Index])
        {
            if (Index == 3)
			{
                //Loads the level map
                GameObject.FindGameObjectWithTag("LM").GetComponent<DN_Event_Manager>().ReturnToMap();

                //Finds the level manager to access the function
                GameObject.Find("Level_Manager(Clone)").GetComponent<DN_Event_Manager>().WinState("Doppelganger");
            }
        }
        else
        {
            //Clear the gameobject list
			Cubes.Clear();

            //Loads the level map
            GameObject.FindGameObjectWithTag("LM").GetComponent<DN_Event_Manager>().ReturnToMap();
        }

    }

}
