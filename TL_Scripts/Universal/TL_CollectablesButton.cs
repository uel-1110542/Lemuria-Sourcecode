using UnityEngine;
using UnityEngine.UI;

public class TL_CollectablesButton : MonoBehaviour {



	void Start()
	{
        //obtain the button component from this gameobject
		Button CollectablesButton = GetComponent<Button>();

        //Find the gameobject and assign the function onto this button
		CollectablesButton.onClick.AddListener(delegate { GameObject.Find("Collectables_Manager").GetComponent<DN_Collectables>().CollectablesButton(); });
	}

}
