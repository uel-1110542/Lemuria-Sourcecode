using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DN_Collectables : MonoBehaviour {

    //Refers to the current instance
    private int len_x = 5;
    private int len_y = 2;
    public int Current_Page = 1;

    //list holding collectables
    public List<GameObject> Sprite_UI = new List<GameObject>();
    public List<GameObject> Collectable_Items_Desert = new List<GameObject>();
    public List<GameObject> Collectable_Items_Forest = new List<GameObject>();
    public List<GameObject> Collectable_Items_Plains = new List<GameObject>();
    public List<GameObject> Collectable_Items_Mountain = new List<GameObject>();

    //Navigation buttons for the collectables book
    public GameObject NextButton;
    public GameObject PrevButton;
	public GameObject ExitButton;

    GameObject NextButtonClone;
    GameObject PrevButtonClone;

    private bool Toggle = false;
    private bool open_coll_book = false;
    private GameObject collection_background;
    private GameObject slot_cursor;
	private GameObject CollectablesManager;

    //actual array where the information is stored if a collectable is collected or not
    [System.Serializable]
    public class SerializeArrays
    {
        public int[,] Collectable_Array_Desert;
        public int[,] Collectable_Array_Forest;
        public int[,] Collectable_Array_Plains;
        public int[,] Collectable_Array_Mountains;
    }
    SerializeArrays ItemArrays = new SerializeArrays();
    private DN_PC_Level_Movement PC_LevelMovementScript;
    private DN_Node_Map_Generation NodeMapGenerationScript;
    private TL_GlobalGameManager GlobalGameManagerScript;
    private TL_PC_World_Movement WorldMovementScript;



    void Awake()
    {
        //If there is more than 1 type of this instance, destroy it otherwise don't
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        ItemArrays.Collectable_Array_Desert = new int[len_x, len_y];
        ItemArrays.Collectable_Array_Forest = new int[len_x, len_y];
        ItemArrays.Collectable_Array_Plains = new int[len_x, len_y];
        ItemArrays.Collectable_Array_Mountains = new int[len_x, len_y];

		//providing the arrays with lengths and filling them out with zero (aka they are empty)
		FillArray_Zero(ItemArrays.Collectable_Array_Desert);
		FillArray_Zero(ItemArrays.Collectable_Array_Forest);
		FillArray_Zero(ItemArrays.Collectable_Array_Plains);
		FillArray_Zero(ItemArrays.Collectable_Array_Mountains);
    }

	public void SaveButton()
	{
        //Find the level manager
        GameObject LevelManager = GameObject.Find("Level_Manager(Clone)");

        //Obtain the script from the managers
        NodeMapGenerationScript = LevelManager.GetComponent<DN_Node_Map_Generation>();
		GlobalGameManagerScript = GameObject.Find("Global_GameManager").GetComponent<TL_GlobalGameManager>();
		WorldMovementScript = GameObject.Find("Game_Manager").GetComponent<TL_PC_World_Movement>();

		//Deletes the previous data first before saving
		GlobalGameManagerScript.DeleteOldData();

		//Saves the following data for their respective serialized classes
		WorldMovementScript.SaveWorldMapData();
		NodeMapGenerationScript.SaveProperties();
		NodeMapGenerationScript.SaveSession();
        PC_LevelMovementScript = LevelManager.GetComponent<DN_PC_Level_Movement>();            
		PC_LevelMovementScript.SaveProgress();
		SaveCollectables();

        //Search through all of the nodes in the scene and loop it with a foreach loop
        GameObject[] Nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach (GameObject go in Nodes)
        {
            Destroy(go);
        }

        //Destroy the PC and the level manager
        Destroy(GameObject.FindGameObjectWithTag("PC"));
        Destroy(LevelManager);

        //Load the start screen
        SceneManager.LoadScene("Start_Game");
	}

	public void CollectablesButton()
	{
		Toggle = !Toggle;
	}

    public void SaveCollectables()
    {
        BinaryFormatter BinaryFormat = new BinaryFormatter();
        FileStream FileStream = File.Create(Application.persistentDataPath + "/SavedCollectables.sg");
        BinaryFormat.Serialize(FileStream, ItemArrays);
        FileStream.Close();
    }

    public void LoadCollectables()
    {
        if (File.Exists(Application.persistentDataPath + "/SavedCollectables.sg"))
        {
            BinaryFormatter BinaryFormat = new BinaryFormatter();
            FileStream FileStream = File.Open(Application.persistentDataPath + "/SavedCollectables.sg", FileMode.Open);
            ItemArrays = (SerializeArrays)BinaryFormat.Deserialize(FileStream);
            FileStream.Close();
        }
    }
    
	// Update is called once per frame
	void Update ()
    {
        Collectable_Book();
        if (Input.GetKeyDown("i"))
        {
            TestAddItem();
        }
    }
    public void Collectable_Book()
    {
        if (Toggle && !open_coll_book)
        {
            foreach (GameObject canvas_ui in Sprite_UI)
            {
                if (canvas_ui.name.Contains("Canvas_Collectable"))
                {
                    GameObject image = (GameObject)Instantiate(canvas_ui, transform.position, Quaternion.identity);
                }
                if (canvas_ui.name.Contains("Book_Background"))
                {
                    open_coll_book = true;
                    collection_background = (GameObject)Instantiate(canvas_ui, canvas_ui.transform.position, Quaternion.identity);
                    collection_background.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Collectable").transform,false);
                    RectTransform rect_trans = (RectTransform)collection_background.transform;
                    rect_trans.sizeDelta = new Vector2(Screen.width, Screen.height);
                }
                if (canvas_ui.name.Contains("Empty_Slot_Button"))
                {
                    for (int col = 0; col < len_x; col++)
                    {
                        for (int row = 0; row < len_y; row++)
                        {
                            GameObject image = (GameObject)Instantiate(canvas_ui, transform.position, Quaternion.identity);
                            image.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Collectable").transform, false);

                            RectTransform rect_trans = (RectTransform)image.transform;
                            //slot size
                            rect_trans.sizeDelta = new Vector2(Slot_Size().x, Slot_Size().y);
                            //slot position
                            rect_trans.localPosition = new Vector2(LeftSide_Slot_Pos(col, row).x, LeftSide_Slot_Pos(col, row).y);

                            //providing the slots with a clickabble function
                            Button btn = image.GetComponent<Button>();
                            btn.onClick.RemoveAllListeners();
                            //we are providing our own scripted function here (in this case "Book_Functions")
                            //btn.onClick.AddListener(delegate { Book_Functions((int)Slot_Pos((int)btn.transform.position.x, (int)btn.transform.position.y).x, (int)Slot_Pos((int)btn.transform.position.x, (int)btn.transform.position.y).y); });
                        }
                    }

					for (int col = 0; col < len_x; col++)
					{
						for (int row = 0; row < len_y; row++)
						{
							GameObject image = (GameObject)Instantiate(canvas_ui, transform.position, Quaternion.identity);
							image.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Collectable").transform, false);

							RectTransform rect_trans = (RectTransform)image.transform;
							//slot size
							rect_trans.sizeDelta = new Vector2(Slot_Size().x, Slot_Size().y);
							//slot position
							rect_trans.localPosition = new Vector2(RightSide_Slot_Pos(col, row).x, RightSide_Slot_Pos(col, row).y);

							//providing the slots with a clickabble function
							Button btn = image.GetComponent<Button>();
							btn.onClick.RemoveAllListeners();
							//we are providing our own scripted function here (in this case "Book_Functions")
							//btn.onClick.AddListener(delegate { Book_Functions((int)Slot_Pos((int)btn.transform.position.x, (int)btn.transform.position.y).x, (int)Slot_Pos((int)btn.transform.position.x, (int)btn.transform.position.y).y); });
						}
					}
                }
            }
			NextButtonClone = (GameObject)Instantiate(NextButton, new Vector3(Screen.width * 0.4f, 5f, 0f), Quaternion.identity);
            NextButtonClone.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Collectable").transform, false);

			PrevButtonClone = (GameObject)Instantiate(PrevButton, new Vector3(Screen.width * -0.4f, 5f, 0f), Quaternion.identity);
            PrevButtonClone.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Collectable").transform, false);

			GameObject ExitButtonClone = (GameObject)Instantiate(ExitButton, new Vector3(Screen.width * 0.35f, Screen.height * 0.35f, 0f), Quaternion.identity);
			ExitButtonClone.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Collectable").transform, false);
            
            Button NextUIButton = NextButtonClone.GetComponent<Button>();
            NextUIButton.onClick.AddListener(delegate { GameObject.Find("Collectables_Manager").GetComponent<DN_Collectables>().Increment(); });

            Button PrevUIButton = PrevButtonClone.GetComponent<Button>();
            PrevUIButton.onClick.AddListener(delegate { GameObject.Find("Collectables_Manager").GetComponent<DN_Collectables>().Decrement(); });

            Button ExitUIButton = ExitButtonClone.GetComponent<Button>();
			ExitUIButton.onClick.AddListener(delegate { GameObject.Find("Collectables_Manager").GetComponent<DN_Collectables>().CloseCollectablesBook(); });
            
            //the first thing you see when you load up the collectable book will be the common collectables
            //you will have to do the functions where you can switch to different "sets" of collectables
            //such as a "next button" that will switch it from common to desert for example 
            UpdateCollectablesPage();
        }

    }

	public void OpenCollectablesBook()
	{
        //Set toggle to true
		Toggle = true;
	}

	public void CloseCollectablesBook()
	{
        //Turn off booleans and destroy the canvas for the collectable
		Toggle = false;
		open_coll_book = false;
		Destroy(GameObject.FindGameObjectWithTag("Canvas_Collectable"));
	}

    void UpdateCollectablesPage()
    {
        //If the current page is 1 then display the desert and forest items
        if (Current_Page == 1)
        {
            //Deactivate the previous button
            PrevButtonClone.SetActive(false);

            //Activate the next button
            NextButtonClone.SetActive(true);

            //Remove the mountains and plains collectables
            RemoveOtherCollectables(Collectable_Items_Mountain);
            RemoveOtherCollectables(Collectable_Items_Plains);

            //Display the desert and forest collectables
			CycleThroughCollectibles(Collectable_Items_Desert, ItemArrays.Collectable_Array_Desert, Collectable_Items_Forest, ItemArrays.Collectable_Array_Forest);
        }
        else if (Current_Page == 2)     //If the current page is 2 then display the plain and mountain items
        {
            //Activate the previous button
            PrevButtonClone.SetActive(true);

            //Deactivate the next button
            NextButtonClone.SetActive(false);

            //Remove the forest and desert collectables
            RemoveOtherCollectables(Collectable_Items_Forest);
            RemoveOtherCollectables(Collectable_Items_Desert);

            //Display the plains and mountains collectables
			CycleThroughCollectibles(Collectable_Items_Plains, ItemArrays.Collectable_Array_Plains, Collectable_Items_Mountain, ItemArrays.Collectable_Array_Mountains);
        }

	}

    void RemoveOtherCollectables(List<GameObject> RegionItems)
    {
        //Find all of the collectables and destroy them
        foreach (GameObject go in RegionItems)
        {
            Destroy(GameObject.Find(go.name + "(Clone)"));
        }
    }

    public void Increment()
    {
        //Increments the index for the current page
		Current_Page++;

        //If the index goes beyond 2 pages, set it to 2
		if (Current_Page > 2)
		{
			Current_Page = 2;
		}
		else
		{
            //Update the current collectables page
			UpdateCollectablesPage();
		}        
    }

    public void Decrement()
    {
        //Decrements the index for the current page
        Current_Page--;

        //If the index goes below 1 page, set it to 1
        if (Current_Page < 1)
		{
			Current_Page = 1;
		}
		else
		{
            //Update the current collectables page
            UpdateCollectablesPage();
		}
        
    }

    //Function that cycles through the collectibles list with a parameter list
    //that holds different sets of lists to display different types of collectibles
    //Stores temporary information of where the collectible is collected
    void CycleThroughCollectibles(List<GameObject> Left_Placeholder_Items_List, int[,] Left_Placeholder_Array, List<GameObject> Right_Placeholder_Items_List, int[,] Right_Placeholder_Array)
    {        
        int left_index = 0;
        int right_index = 0;

        //instantiate the collectables too
        for (int col = 0; col < len_x; col++)
        {
            for (int row = 0; row < len_y; row++)
            {
                if (Left_Placeholder_Items_List.Count <= (col + row))
                {
                    break;
                }
                
                //checking if the "collectable" has been collected before or not
                if (Left_Placeholder_Array[col, row] == 1)//is collected
                {
                    //getting the game collectable using "i" as the index to go through the common collectables list
                    GameObject item_image = Left_Placeholder_Items_List[left_index];

                    //increase "i" here to be ready for the next iteration of the loop
                    left_index++;

                    //where we then begin instantiating the collectable image with the same size and position as the slots
                    GameObject image = (GameObject)Instantiate(item_image, transform.position, Quaternion.identity);
                    image.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Collectable").transform, false);

                    //getting the rect transform of the "collectable"
                    RectTransform rect_trans = (RectTransform)image.transform;

                    //slot size
                    rect_trans.sizeDelta = new Vector2(Slot_Size().x, Slot_Size().y);

                    //slot position
                    rect_trans.localPosition = new Vector2(LeftSide_Slot_Pos(col, row).x, LeftSide_Slot_Pos(col, row).y);
                }
                
            }

        }

        if (Right_Placeholder_Items_List != null)
        {
            for (int col = 0; col < len_x; col++)
            {
                for (int row = 0; row < len_y; row++)
                {
                    if (Right_Placeholder_Items_List.Count <= (col + row))
                    {
                        break;
                    }

                    //checking if the "collectable" has been collected before or not
                    if (Right_Placeholder_Array[col, row] == 1)//is collected
                    {
                        //getting the game collectable using "i" as the index to go through the common collectables list
                        GameObject item_image = Right_Placeholder_Items_List[right_index];

                        //increase "i" here to be ready for the next iteration of the loop
                        right_index++;

                        //where we then begin instantiating the collectable image with the same size and position as the slots
                        GameObject image = (GameObject)Instantiate(item_image, transform.position, Quaternion.identity);
                        image.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_Collectable").transform, false);

                        //getting the rect transform of the "collectable"
                        RectTransform rect_trans = (RectTransform)image.transform;

                        //slot size
                        rect_trans.sizeDelta = new Vector2(Slot_Size().x, Slot_Size().y);

                        //slot position
                        rect_trans.localPosition = new Vector2(RightSide_Slot_Pos(col, row).x, RightSide_Slot_Pos(col, row).y);
                    }
                }
            }
        }
    }

    Vector2 Slot_Size()
    {
        float size_x = Screen.height * (len_x * 0.021f);
        float size_y = size_x;
        Vector2 slotsize = new Vector2(size_x, size_y);
        return slotsize;
    }
    Vector2 LeftSide_Slot_Pos(int left_col, int left_row)
    {
        float left_pos_x = 0;
        float left_pos_y = 0;

        //magic number to further modify the scaling as we please - worked out from trial and error - should really change this static magic number into a more abstract magic argument
        float mod = 0.127f;
        //maths equations for positioning the objects to the screen (currently only works for specifically 5 x 2 grids)
        /*if (col < len_x / 2 && row < len_y / 2 || col >= len_x / 2 && row < len_y / 2)
        {

            pos_x = -(col * (Screen.height * mod)) + (Screen.height * 0.25f);
            pos_y = (row * (Screen.width * mod)) - (Screen.width * 0.35f);
        }
        if (col < len_x / 2 && row >= len_y / 2 || col >= len_x / 2 && row >= len_y / 2)
        {
            pos_x = -(col * (Screen.height * mod)) + (Screen.height * 0.25f);
            pos_y = (row * (Screen.width * mod)) - (Screen.width * 0.35f);
        }*/
        if (left_col < len_x / 2 && left_row < len_y / 2 || left_col >= len_x / 2 && left_row < len_y / 2)
        {
            left_pos_x = -(left_col * (Screen.height * mod)) + (Screen.height * 0.2f);
            left_pos_y = (left_row * (Screen.width * mod)) - (Screen.width * 0.3f);
        }
        if (left_col < len_x / 2 && left_row >= len_y / 2 || left_col >= len_x / 2 && left_row >= len_y / 2)
        {
            left_pos_x = -(left_col * (Screen.height * mod)) + (Screen.height * 0.2f);
            left_pos_y = (left_row * (Screen.width * mod)) - (Screen.width * 0.3f);
        }
        Vector2 left_slotpos = new Vector2(left_pos_y, left_pos_x);
        return left_slotpos;
    }
    Vector2 RightSide_Slot_Pos(int right_col, int right_row)
    {
        float right_pos_x = 0;
        float right_pos_y = 0;

        //magic number to further modify the scaling as we please - worked out from trial and error - should really change this static magic number into a more abstract magic argument
        float mod = 0.127f;
        //maths equations for positioning the objects to the screen (currently only works for specifically 5 x 2 grids)
        if (right_col < len_x / 2 && right_row < len_y / 2 || right_col >= len_x / 2 && right_row < len_y / 2)
        {
            right_pos_x = -(right_col * (Screen.height * mod)) + (Screen.height * 0.2f);
            right_pos_y = (right_row * (Screen.width * mod)) - (Screen.width * -0.117f);
        }
        if (right_col < len_x / 2 && right_row >= len_y / 2 || right_col >= len_x / 2 && right_row >= len_y / 2)
        {
            right_pos_x = -(right_col * (Screen.height * mod)) + (Screen.height * 0.2f);
            right_pos_y = (right_row * (Screen.width * mod)) - (Screen.width * -0.117f);
        }
        Vector2 right_slotpos = new Vector2(right_pos_y, right_pos_x);
        return right_slotpos;
    }
    void Book_Functions(int col, int row)
    {
        //add mouse over functionality to provide information for the collectables (currently does nothing,)
        RectTransform rt = (RectTransform)slot_cursor.transform;
        rt.localPosition = gameObject.transform.position;
    }
    public void AddCollectable(string region_item_list, string collectable_type)
    {
        int rng = Random.Range(0, 49);
        ////pointing towards the correct lists and then searching through that list to turn on the collectable
        if (rng < 50)
        {
            if (region_item_list == "DesertNode(Clone)")
            {
                //desert item
                SortThroughItemList(collectable_type, Collectable_Items_Desert);
                print("Received = " + region_item_list + " and type = " + collectable_type);
            }
            if (region_item_list == "ForestNode(Clone)")
            {
                //forest item
                SortThroughItemList(collectable_type, Collectable_Items_Forest);
                print("Received = " + region_item_list + " and type = " + collectable_type);
            }
            if (region_item_list == "PlainsNode(Clone)")
            {
                //plains item
                SortThroughItemList(collectable_type, Collectable_Items_Plains);
                print("Received = " + region_item_list + " and type = " + collectable_type);
            }
            if (region_item_list == "MountainNode(Clone)")
            {
                //mountain item
                SortThroughItemList(collectable_type, Collectable_Items_Mountain);
                print("Received = " + region_item_list + " and type = " + collectable_type);
            }

        }

    }
    //TEST FUNCTION ONLY
    void TestAddItem()
    {
        AddCollectable("ForestNode(Clone)", "Combat");
        AddCollectable("MountainNode(Clone)", "Combat");
		AddCollectable("PlainsNode(Clone)", "Combat");
		AddCollectable("DesertNode(Clone)", "Combat");
    }
    void SortThroughItemList(string collectable_type , List<GameObject>Collectable_List)
    {
        //temporary list to hold items that have yet to be "collected"
        List<GameObject> Available_Items = new List<GameObject>();

        //looping through the provided collectable list to find out which items are already "collected"
        foreach (GameObject item in Collectable_List)
        {
            DN_Item_Properties dn_item_property = item.GetComponent<DN_Item_Properties>();
            //access item_properties of the collectable and checking if it is the same "type" we want
            if (dn_item_property.Same_Type(collectable_type))
            { 
                //for finding the index in the list of the collected item
                //using "i" to find the index of the "item" in the collectable list
                int i = Collectable_List.IndexOf(item);

                //we will be using "j" for keeping within the range of len_x (col)
                int j = 1;
                //find out "item"'s position in the array
                while (j <= len_x)
                {
                    if (i < len_y * j)
                    {
                        //"k" will be used for the "row" in the array
                        int k = 0;
                        //"k" is calculated to be the "row"
                        k = len_y - ((len_y * j) - i);

                        //print("i = " + i + "; j = " + j + "; k = " + k);
                        //then put those values into the respective array by checking what collectable list we were using
                        //REMEMBER - j was initalised as 1 for mathematical operations, if you want to use it as a replacement for "col", minus 1 away from it
                        if (Collectable_List == Collectable_Items_Desert)
                        {
                            if (ItemArrays.Collectable_Array_Desert[j - 1, k] == 0)
                            {
                                //add the item to the temp list
                                Available_Items.Add(item);                                
                            }
                        }
                        if (Collectable_List == Collectable_Items_Forest)
                        {
                            if (ItemArrays.Collectable_Array_Forest[j - 1, k] == 0)
                            {
                                //add the item to the temp list
                                Available_Items.Add(item);
                            }
                        }
                        if (Collectable_List == Collectable_Items_Mountain)
                        {
                            if (ItemArrays.Collectable_Array_Mountains[j - 1, k] == 0)
                            {
                                //add the item to the temp list
                                Available_Items.Add(item);
                            }
                        }
                        if (Collectable_List == Collectable_Items_Plains)
                        {
                            if (ItemArrays.Collectable_Array_Plains[j - 1, k] == 0)
                            {
                                //add the item to the temp list
                                Available_Items.Add(item);
                            }
                        }
                        break;
                    }
                    j++;
                }
            }
        }
        //if there was an item we could add
        if (Available_Items.Count > 0)
        {            
            //randomly set item collection state to true;
            int rng = Random.Range(0, Available_Items.Count - 1);
            //using "rng" as a reference point, we go through the collectable list again for item that has been "collected"
            int i = Collectable_List.IndexOf(Available_Items[rng]);
            //we will be using "j" for keeping within the range of len_x amd to act as the "col" in the array
            int j = 1;
            //"k" will be used for the "row" in the array
            int k = 0;
            //find out "item"'s position in the array
            while (j <= len_x)
            {
                if (i < len_y * j)
                {
                    //"k" is calculated to be the "row" (the minus one is because "zero" is the first index)
                    k = len_y - ((len_y * j) - i);
                    //then put those values into the respective array by checking what collectable list we were using
                    //REMEMBER - j was initalised as 1 for mathematical operations, if you want to use it as a replacement for "col", minus 1 away from it
                    if (Collectable_List == Collectable_Items_Desert)
                    {
                        ItemArrays.Collectable_Array_Desert[j - 1, k] = 1;
                    }
                    if (Collectable_List == Collectable_Items_Forest)
                    {
                        ItemArrays.Collectable_Array_Forest[j - 1, k] = 1;
                    }
                    if (Collectable_List == Collectable_Items_Mountain)
                    {
                        ItemArrays.Collectable_Array_Mountains[j - 1, k] = 1;
                    }
                    if (Collectable_List == Collectable_Items_Plains)
                    {
                        ItemArrays.Collectable_Array_Plains[j - 1, k] = 1;
                    }
                    //break out of the loop
                    break;
                }
                j++;
            }
        }
    }

	public void ResetSerializedArrays()
	{
		ItemArrays = new SerializeArrays();
		ItemArrays.Collectable_Array_Desert = new int[len_x, len_y];
		ItemArrays.Collectable_Array_Forest = new int[len_x, len_y];
		ItemArrays.Collectable_Array_Plains = new int[len_x, len_y];
		ItemArrays.Collectable_Array_Mountains = new int[len_x, len_y];
	}

    void FillArray_Zero(int[,] array)
    {
        array = new int[len_x, len_y];
        for (int col = 0; col < len_x; col++)
        {
            for (int row = 0; row < len_y; row++)
            {
                array[col, row] = 0;
            }
        }
    }

}
