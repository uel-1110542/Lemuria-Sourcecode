using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class TL_GlobalGameManager : MonoBehaviour {
        
    public bool LoadingFlag;


    void Awake()
    {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    public void LoadGame()
    {
        //When this function being called, set the loading flag to true
        LoadingFlag = true;

        //Load the level map
        SceneManager.LoadScene("World_Map");
    }

    public void DeleteOldData()
    {
        for (int col = 0; col < 5; col++)
        {
            for (int row = 0; row < 5; row++)
            {
                //If the previous saved node properties data exists then delete it to prevent any previous data from clashing
                if (File.Exists(Application.persistentDataPath + "/SavedNodeProperties " + col + "-" + row + ".sg"))
                {
                    File.Delete(Application.persistentDataPath + "/SavedNodeProperties " + col + "-" + row + ".sg");
                }
            }
        }
        //Checks if any of the previous data exists and if it does, delete it
        if (File.Exists(Application.persistentDataPath + "/SavedNodeMap.sg"))
        {
            File.Delete(Application.persistentDataPath + "/SavedNodeMap.sg");
        }

        if (File.Exists(Application.persistentDataPath + "/SavedProgress.sg"))
        {
            File.Delete(Application.persistentDataPath + "/SavedProgress.sg");
        }

        if (File.Exists(Application.persistentDataPath + "/SavedCollectables.sg"))
        {
            File.Delete(Application.persistentDataPath + "/SavedCollectables.sg");
        }

        if (File.Exists(Application.persistentDataPath + "/SavedWorldMapData.sg"))
        {
            File.Delete(Application.persistentDataPath + "/SavedWorldMapData.sg");
        }

    }

}
