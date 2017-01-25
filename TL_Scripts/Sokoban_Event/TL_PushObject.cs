using UnityEngine;

public class TL_PushObject : MonoBehaviour {

    //Variables
    public Vector3 TargetPos;
    public bool Moving;
    private TL_GridManager GridScript;

    
	void Start()
    {
        //Finds the gameobject and obtains the script
        GridScript = GameObject.Find("LevelArea").GetComponent<TL_GridManager>();

        //Set default target position
        TargetPos = transform.position;
    }

	void Update()
    {
        //Animates the moving object
        AnimateMovement();
    }

    void OnMouseDown()
    {
        //Find the PC
        GameObject PC = GameObject.FindGameObjectWithTag("PC");

        //Calculate the X and Z difference of this gameobject and the PC
        float X_Difference = Mathf.Round(transform.position.x - PC.transform.position.x);
        float Z_Difference = Mathf.Round(transform.position.z - PC.transform.position.z);

        //If the randomized values are within the boundaries of the 2D gameobject array
        if (transform.position.x + X_Difference < GridScript.ReturnLevelAreaArray().GetLength(0) && transform.position.z + Z_Difference < GridScript.ReturnLevelAreaArray().GetLength(1) && GridScript.ReturnLevelAreaArray()[(int)(transform.position.x + X_Difference), (int)(transform.position.z + Z_Difference)] == null)
        {
            //if the X and Z difference are equal accordingly to the values in the if statement
            if (X_Difference == 1f && Z_Difference == 0f || X_Difference == -1f && Z_Difference == 0f || Z_Difference == 1f && X_Difference == 0f || Z_Difference == -1f && X_Difference == 0f)
            {
                //If the gameobject isn't moving
                if (!Moving)
                {
                    //Set the target position
                    TargetPos = new Vector3(transform.position.x + X_Difference, transform.position.y, transform.position.z + Z_Difference);
                }
            }
        }
    }

    void AnimateMovement()
    {
        //If the distance between this gameobject and the target position is more than the value specified
        if (Vector3.Distance(transform.position, TargetPos) > 0.1f)
        {
            //Set the bool to true
            Moving = true;

            //Set the previous position to null in the 2D gameobject array
            GridScript.SetGOInLevelArea(null, (int) Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

            //Move towards the new position
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, 6f * Time.deltaTime);
        }
        else
        {
            //Set the bool to false
            Moving = false;

            //Set the new position
            transform.position = new Vector3 (TargetPos.x, transform.position.y, TargetPos.z);

            //Set the destination of this gameobject in the 2D gameobject array
            GridScript.SetGOInLevelArea(gameObject, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));
        }
    }

}
