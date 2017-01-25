using UnityEngine;
using UnityEngine.UI;

public class TL_LoadGameButton : MonoBehaviour {


    
    void Start()
    {
        //Obtain the button component from this gameobject
        Button LoadButton = GetComponent<Button>();

        //Find the global game manager and add the listener
        LoadButton.onClick.AddListener(delegate { GameObject.Find("Global_GameManager").GetComponent<TL_GlobalGameManager>().LoadGame(); });
    }

}
