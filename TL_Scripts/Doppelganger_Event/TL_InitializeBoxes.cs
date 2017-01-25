using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TL_InitializeBoxes : MonoBehaviour {

	//Variables
	public GameObject Box;
	public float X_Pos_Bounds;
	public float Z_Pos_Bounds;
	private GameObject PC;
    private List<GameObject> PC_Boxes = new List<GameObject>();
    private TL_GridManager GridManagerScript;



    void Start()
	{
        //Variable for the follow character script
        TL_FollowChar ColliderScript;

        //Finds the gameobject with the PC tag
        PC = GameObject.FindGameObjectWithTag("PC");

        //Finds the doppelganger gameobject
        GameObject Doppelganger = GameObject.Find("Doppelganger(Clone)");

        //Find the level area gameobject and obtain the script
        GridManagerScript = GameObject.Find("LevelArea").GetComponent<TL_GridManager>();

        //Instantiates the move boxes in a for loop
        for (int i = 0; i < 4; i++)
		{
			GameObject ColliderClone = (GameObject) Instantiate(Box, new Vector3(0f, 2f, 0f), Quaternion.identity);

            //Sets default rotation
            ColliderClone.transform.eulerAngles = new Vector3(90f, 0f, 0f);

            //Adds the gameobjects in a list
            PC_Boxes.Add(ColliderClone);
        }

        //Finds gameobjects with the tag
		GameObject[] Colliders = GameObject.FindGameObjectsWithTag("MoveBox");

        //Set the X position for the box colliders
		for(int x = -1; x < 2; x++)
		{
            if (x == -1)
            {
                Colliders[x + 1].transform.eulerAngles = new Vector3(90f, 90f, 0f);
            }
            else if (x == 1)
            {
                Colliders[x + 1].transform.eulerAngles = new Vector3(90f, -90f, 0f);
            }
            //Obtains the component from the script and sets the X offset
            ColliderScript = Colliders[x+1].GetComponent<TL_FollowChar>();
			ColliderScript.X_Pos = x;
		}

        //Set the Z position for the box colliders
        for (int z = -1; z < 2; z++)
		{
            if (z == -1)
            {
                Colliders[z + 2].transform.eulerAngles = new Vector3(90f, 0f, 0f);
            }
            else if (z == 1)
            {
                Colliders[z + 2].transform.eulerAngles = new Vector3(90f, 180f, 0f);
            }
            //Obtains the component from the script and sets the Z offset
            ColliderScript = Colliders[z+2].GetComponent<TL_FollowChar>();
			ColliderScript.Z_Pos = z;
		}

	}

	void Update()
	{
		CheckBoxes();
	}

	void CheckBoxes()
	{
		foreach(GameObject go in PC_Boxes)
		{
            //Obtains the script from the gameobject
            TL_FollowChar PCBoxScript = go.GetComponent<TL_FollowChar>();

            //Assigns the character variable as the PC
            PCBoxScript.Character = PC;

            //Variable for obtaining the box collider in the list
            BoxCollider BoxCol = go.GetComponent<BoxCollider>();

            //If the X and Z position of the special hitboxes ever go out of bounds, obtain the script and disable it
            if (go.transform.position.x < 0f || go.transform.position.z < 0f || go.transform.position.x >= X_Pos_Bounds || go.transform.position.z >= Z_Pos_Bounds)
            {
                BoxCol.enabled = false;
            }
            else
            {
                //If the X and Z positions of the special hitboxes are within bounds, obtain the script and enable it
                BoxCol.enabled = true;
            }

        }

	}

}
