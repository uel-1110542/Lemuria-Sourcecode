using UnityEngine;

public class TL_AddSequence : MonoBehaviour {

    //Variables
    public Sprite IdleRune;
    public Sprite IncorrectRune;
    public bool OccupiedSquare = false;
    private TL_PuzzleScript SequenceScript;
    private TL_MoveScript MoveScript;



    void Update()
    {
        //Function for checking doppelganger position
        CheckPosition();
    }

    void CheckPosition()
    {
        //Finds the doppelganger
        GameObject Doppelganger = GameObject.Find("Doppelganger(Clone)");

        //Finds the gameobject required and obtains the scripts
        SequenceScript = GameObject.Find("LevelArea").GetComponent<TL_PuzzleScript>();
        MoveScript = GameObject.Find("Doppelganger(Clone)").GetComponent<TL_MoveScript>();

        //If the doppelganger is on the flashing platform and the doppelganger stopped moving, set the bool to true
        if (!MoveScript.IsCharMoving && !OccupiedSquare && transform.position.x == Doppelganger.transform.position.x && transform.position.z == Doppelganger.transform.position.z)
        {
            //Adds this gameobject into the list
            SequenceScript.Cubes.Add(transform.gameObject);

            //Checks the sequence
            SequenceScript.CheckSequence();

            //Sets bool to true
            OccupiedSquare = true;
        }
    }

    void OnTriggerExit(Collider Col)
    {
        //Obtain the script from the gameobject
        SequenceScript = GameObject.Find("LevelArea").GetComponent<TL_PuzzleScript>();

        //If the doppelganger is off the collider, set the bool to false
        if (Col.transform.name == "Doppelganger(Clone)")
        {
            GetComponent<SpriteRenderer>().sprite = IdleRune;

            //Sets bool to false
            OccupiedSquare = false;

            //Increments index for the next order of sequence
            SequenceScript.Index++;
        }
    }

}
