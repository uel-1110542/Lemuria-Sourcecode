using UnityEngine;
using System.Collections;

public class TL_MoveOffset : MonoBehaviour {

    //Variables
    public float X_Offset;
    public float Z_Offset;


    //A function with float parameters to set the offset of the gameobject
    public void SetOffset(float x, float z)
    {
        X_Offset = x;
        Z_Offset = z;
    }
    

}
