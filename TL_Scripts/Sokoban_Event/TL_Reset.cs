using UnityEngine;
using UnityEngine.SceneManagement;

public class TL_Reset : MonoBehaviour {



    public void ResetScene()
    {
        //Reload the scene
        SceneManager.LoadScene("Sokoban_Event");
    }


}
