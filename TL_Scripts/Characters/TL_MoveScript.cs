using UnityEngine;

public class TL_MoveScript : MonoBehaviour {

	//Variables
	public Vector3 CurrentPos;
	public Vector3 NextPos;
	public Vector3 TargetPos;
	public float Char_Speed;
	public string Target_Name;
	public bool IsCharMoving = false;
	private Quaternion Sprite_Rotation;



	void Start()
	{
        //Sets default position
		CurrentPos = transform.position;
		NextPos = CurrentPos;
		TargetPos = CurrentPos;
	}

	void Update()
	{
        //Checks character position on the level
		CheckCharacterPos();

        //Function for animating the movement of the character
		AnimateCharacter();
	}

	void CheckCharacterPos()
	{
        //Calculate the differences of X and Z
		float dX = (TargetPos.x - CurrentPos.x);
		float dZ = (TargetPos.z - CurrentPos.z);

        //Find the angle of the direction
		float Angle = Mathf.Atan2(dX, dZ);

        //If the difference is a square away
		if(Mathf.Abs(dX) > 0.1f)
		{
			NextPos.x = CurrentPos.x + Mathf.Round(1.4f * Mathf.Sin(Angle));
		}

		if(Mathf.Abs(dZ) > 0.1f)
		{
			NextPos.z = CurrentPos.z + Mathf.Round(1.4f * Mathf.Cos(Angle));
		}
        //Update current position
		CurrentPos = NextPos;

	}

	void AnimateCharacter()
	{
        //If the distance is more than its' current position, move towards the target position
        //Only the X and Z positions are involved with moving the PC and the Y position is ignored
		if(Vector3.Distance(new Vector3(CurrentPos.x, 0.1f, CurrentPos.z), new Vector3(transform.position.x, 0.1f, transform.position.z)) > 0.1f)
		{
			IsCharMoving = true;
			transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, 0.1f, transform.position.z), new Vector3(TargetPos.x, 0.1f, TargetPos.z), Char_Speed * Time.deltaTime);
		}
		else
		{
            //If the player has reached its' destination, turn the bool off and set the transform to its' current position
			IsCharMoving = false;
			transform.position = new Vector3(CurrentPos.x, 0.1f, CurrentPos.z);
		}
	}

}
