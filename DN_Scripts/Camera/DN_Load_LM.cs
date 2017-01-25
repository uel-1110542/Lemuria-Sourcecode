using UnityEngine;

public class DN_Load_LM : MonoBehaviour
{
    public GameObject level_manager;


    // Use this for initialization
    void Awake()
    {
        if (GameObject.FindGameObjectWithTag("LM") == null)
        {
            GameObject level_man = (GameObject)Instantiate(level_manager,transform.position,transform.rotation);
        }
    }
}