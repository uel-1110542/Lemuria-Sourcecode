using UnityEngine;

public class TL_Raycast : MonoBehaviour {

	//Variables
    public GameObject PC;
    public int Index = 0;
	private float DistanceToGround;
	private TL_MoveScript MoveScript;
	private TL_CheckSequence SequenceScript;


	void Start ()
	{
        //Sets the float variable to the extents of the box collier
		DistanceToGround = GetComponent<Collider>().bounds.extents.y;

        //Obtains the script from this gameobject
		MoveScript = GetComponent<TL_MoveScript>();
	}

	void Update()
	{
        //Casts a ray onto the ground
		RaycastGround();
	}

	void RaycastGround()
	{
        //Find the gameobject
		GameObject Sequence = GameObject.Find("Level");

        //Set the raycast hit variable
		RaycastHit RayHit;

        //Initialize a new ray
		Ray Ray = new Ray(transform.position, -Vector3.up);        
		Debug.DrawRay(Ray.origin, Ray.direction, Color.green);

        //If the raycast hits the ground
		if(Physics.Raycast(Ray, out RayHit, (DistanceToGround + 2f)))
		{
			if(Sequence != null)
			{
				//If the player lands on a tile, add the gameobject which the raycast hits and check the sequence
				SequenceScript = Sequence.GetComponent<TL_CheckSequence>();

                //if the raycast hit gameobject still exists and it is the flashing cube and the PC is not moving
                if (RayHit.transform.gameObject != null && RayHit.collider != null && !MoveScript.IsCharMoving && RayHit.transform.gameObject.name == "Flashing_Cube(Clone)")
                {
                    //Add the raycast hit gameobject
                    SequenceScript.Cubes.Add(RayHit.transform.gameObject);

                    //Check the sequence
                    SequenceScript.CheckSequence();

                    //Increment the index
                    Index++;
                }                
			}
		}

	}

}
