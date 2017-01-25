using UnityEngine;

[System.Serializable]
public class DN_Item_Properties : MonoBehaviour
{
    public string item_type;
    private bool is_collected = false;

    void Start()
    {
       // Collection_State();
    }
    //for managing the state of the collectable object to be showed when collection book is opened
    void Collection_State()
    {
        if (is_collected)
        {
            //using set active is only temporary
            gameObject.SetActive(true);
        }
        else if (!is_collected)
        {
            //using set active is only temporary
            gameObject.SetActive(false);
        }
    }
    public bool Get_Collected_State()
    {
        return is_collected;
    }
    public bool Same_Type(string collect_type)
    {
        if (item_type == collect_type)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Now_Collected()
    {
        is_collected = true;
    }
}
