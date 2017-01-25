using UnityEngine;
using UnityEngine.SceneManagement;

public class TL_SpawnPC : MonoBehaviour {

    //GameObject PC's
    public GameObject Vadinho;
    public GameObject Zofia;
    public GameObject Wu;
    public Vector3 SpawningPos;
    GameObject PC_Clone;

    
    public GameObject SpawnPC(string PC_Name)
    {
        //Obtains the PC name from playerprefs and instantiates the PC based on the name
        switch (PC_Name)
        {
            case "Vadinho(Clone)":
                PC_Clone = (GameObject)Instantiate(Vadinho, SpawningPos, Quaternion.identity);
                break;

            case "Zofia(Clone)":
                PC_Clone = (GameObject)Instantiate(Zofia, SpawningPos, Quaternion.identity);
                break;

            case "Wu(Clone)":
                PC_Clone = (GameObject)Instantiate(Wu, SpawningPos, Quaternion.identity);
                break;
        }
		//If the player is in the world map, reset the rotation to zero
		if(SceneManager.GetActiveScene().name != "World_Map")
		{
            PC_Clone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		}

		//Return the gameobject
        return PC_Clone;
    }

}
