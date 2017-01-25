using UnityEngine;
using UnityEngine.SceneManagement;

public class TL_RaycastSelection : MonoBehaviour {

    private GameObject SelectedGO;
    private RaycastHit CollidedRaycast;
	private RaycastHit2D CollidedRaycast2D;



	void Update()
    {
		//If the player is in the world map, use the 2D raycast selection
		if(SceneManager.GetActiveScene().name == "World_Map")
		{
			SelectWith2DRaycast();
		}
		else
		{
			SelectWithRaycast();
		}

    }

    void SelectWithRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //LayerMask is equal to 3 because it ignores raycasts from gameobject that are labelled as Ignore Raycast
            int LayerMask = 3;

            //The raycast hit is for obtaining information from the ray telling us what it hits
			RaycastHit Hit = new RaycastHit();

            //Creates the ray from the camera
            Ray Ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
            Debug.DrawRay(Ray.origin, Ray.direction * 40f, Color.red);

			if (Physics.Raycast(Ray, out Hit, 40f, LayerMask))
            {
                //If the raycast does not hit a movable object
                if (Hit.transform.tag != "MovableObj")
                {
                    //Make the gameobject variable equal to the gameobject that was hit by the raycast
                    SelectedGO = Hit.transform.gameObject;

                    //Make the Raycast hit equal to the collided raycast
                    CollidedRaycast = Hit;
                }
                
            }

        }

    }

	void SelectWith2DRaycast()
	{
		if (Input.GetMouseButtonDown(0))
		{
			//The raycast hit is for obtaining information from the ray telling us what it hits
			RaycastHit2D Hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 10f);

			//If the raycast 2D hit is not null
			if (Hit2D.collider.gameObject != null)
			{
				//Make the gameobject variable equal to the gameobject that was hit by the raycast
				SelectedGO = Hit2D.transform.gameObject;

				//Make the Raycast hit equal to the collided raycast
				CollidedRaycast2D = Hit2D;
			}

		}

	}

    //Returns the selected gameobject
    public GameObject ReturnSelectedGO()
    {
        return SelectedGO;
    }

    //Returns the gameobject that the raycast collided with
	public RaycastHit ReturnRaycastHit()
    {
        return CollidedRaycast;
    }

    //Returns the gameobject that the 2D raycast collided with
    public RaycastHit2D ReturnRaycastHit2D()
	{
		return CollidedRaycast2D;
	}

}
