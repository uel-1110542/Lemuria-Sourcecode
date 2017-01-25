using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DN_Level_Camera : MonoBehaviour
{
    private GameObject pc;


    // Update is called once per frame
    void Update ()
    {
        CameraLock();
	}
    void CameraLock()
    {
		if(SceneManager.GetActiveScene().name == "Level_Map")
		{
			transform.localEulerAngles = new Vector3(90f, 0f, 0f);
			pc = GameObject.FindGameObjectWithTag("PC");
			if (pc != null)
			{
				Vector3 cam_pos = new Vector3(pc.transform.position.x,2,pc.transform.position.z);
				transform.position = cam_pos;
			}

		}

    }

}