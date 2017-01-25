using UnityEngine;
using UnityEngine.UI;

public class TL_SaveButton : MonoBehaviour {

    

    void Start()
    {
        //Obtain the button component from this gameobject
        Button CollectablesButton = GetComponent<Button>();

        //Find the collectables manager and add the listener
        CollectablesButton.onClick.AddListener(delegate { GameObject.Find("Collectables_Manager").GetComponent<DN_Collectables>().SaveButton(); });
    }

}
