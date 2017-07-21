using UnityEngine;
using System.Collections;

public class TL_PCMove : MonoBehaviour {

	//Variables
    public GameObject Doppelganger;
	public bool NPC_Detected = false;
    public bool GrabbedByNPC = false;
    private GameObject NPC;
    private GameObject Closest_NPC;
    private GameObject LevelArea;

    private TL_MoveScript MoveScript;
	private TL_MoveScript NPCMoveScript;
	private TL_CharStats CharacterScript;
    private TL_GridManager GridScript;
    private TL_RaycastSelection SelectionScript;
    private int MovementPresses;



    void Start()
	{
        //Find the level area
        LevelArea = GameObject.Find("LevelArea");

        //Obtain the script from the gameobject this script is attached to
        MoveScript = GetComponent<TL_MoveScript>();

        //Obtain the scripts from the level area
        GridScript = LevelArea.GetComponent<TL_GridManager>();
        SelectionScript = LevelArea.GetComponent<TL_RaycastSelection>();

        //Set default rotation
        transform.eulerAngles = new Vector3(90f, 0f, 0f);
    }

	void Update()
	{
		MoveCharacter();
    }

	void MoveCharacter()
	{
        //If the raycast is the movement boxes
        if (SelectionScript.ReturnSelectedGO() != null)
        {
            //If the selected gameobject has the tag Offset
            if (SelectionScript.ReturnSelectedGO().transform.tag == "Offset")
            {
                //Calculate the distance between where the player is clicking and the PC's position
                float MoveDist = (Mathf.Round(transform.position.x) - Mathf.Round(SelectionScript.ReturnSelectedGO().transform.position.x));

                //Find all the NPCs
                GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");
                
                //Loop through all of the NPCs in the scene
                foreach (GameObject go in NPCs)
                {
                    //Calculate the X and Z distance of the PC and the closest NPC
                    float X_Dist = (Mathf.Round(transform.position.x) - Mathf.Round(go.transform.position.x));
                    float Z_Dist = (Mathf.Round(transform.position.z) - Mathf.Round(go.transform.position.z));
                    
                    //If the distance of X is more than 1, flip the sprite, if not, don't flip it
                    if (X_Dist >= 1)
                    {
                        GetComponent<SpriteRenderer>().flipX = true;
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                    }

                    //If the PC is Zofia
                    if (transform.name == "Zofia(Clone)")
                    {
                        //If the player has clicked in front of the PC and the NPC
                        if (MoveDist == -1 && X_Dist == -2 && Z_Dist == 0 || MoveDist == 1 && X_Dist == 2 && Z_Dist == 0)
                        {
                            //If the NPC is not dead
                            if (go != null)
                            {
                                //Attack the NPC and deal damage
                                AttackNPC(go);
                            }
                        }
                        else
                        {
                            //Update the movement grid
                            UpdateMovementGrid();
                        }
                    }
                    else if (transform.name == "Vadinho(Clone)")        //If the PC is Vadinho
                    {
                        //If the player has clicked in front of or behind the PC and the NPC
                        if (MoveDist == 1 && X_Dist == 1 && Z_Dist >= -1 && Z_Dist <= 1 || 
                            MoveDist == -1 && X_Dist == -1 && Z_Dist >= -1 && Z_Dist <= 1)
                        {
                            //If the NPC is not dead
                            if (go != null)
                            {
                                //Attack the NPC and deal damage
                                AttackNPC(go);
                            }
                        }
                        else
                        {
                            //Update the movement grid
                            UpdateMovementGrid();
                        }                        
                    }                    
                }
            }

            if (SelectionScript.ReturnSelectedGO().transform.name != "Wall(Clone)" && !MoveScript.IsCharMoving)
            {
                //Finds the doppelganger gameobject
                Doppelganger = GameObject.Find("Doppelganger(Clone)");

                //If the doppelganger has been found for the event
                if (Doppelganger != null)
                {
                    //Grabs the component from the doppelganger
                    NPCMoveScript = Doppelganger.GetComponent<TL_MoveScript>();

                    //Invert movement for the doppelganger
                    InvertMovement(Mathf.Round(SelectionScript.ReturnRaycastHit().point.x), Mathf.Round(SelectionScript.ReturnRaycastHit().point.z));
                }
                else
                {
                    //Round up to the nearest number to prevent numbers with decimal points
                    MoveScript.TargetPos.x = Mathf.Round(SelectionScript.ReturnRaycastHit().point.x);
                    MoveScript.TargetPos.z = Mathf.Round(SelectionScript.ReturnRaycastHit().point.z);
                }
            }
        }
    }

    void UpdateMovementGrid()
    {
        //Obtain the script from the movement box
        TL_MoveOffset MoveOffsetScript = SelectionScript.ReturnSelectedGO().GetComponent<TL_MoveOffset>();

        if (GridScript.ReturnGOInLevelArea((int)Mathf.Round(SelectionScript.ReturnSelectedGO().transform.position.x), (int)Mathf.Round(SelectionScript.ReturnSelectedGO().transform.position.z)) == null)
        {
            //Sets its' previous position in the grid to be null
            GridScript.SetGOInLevelArea(null, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));

            //Set the target X and Z positions to the X and Z positions obtained from the grid offsets
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveOffsetScript.X_Offset, 0.1f, MoveOffsetScript.Z_Offset), 5f);

            //Assigns its' current position after moving into the grid
            GridScript.SetGOInLevelArea(transform.gameObject, (int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.z));
        }
    }

    void AttackNPC(GameObject go)
    {
        //If the PC is not grabbed by the NPC
        if (!GrabbedByNPC)
        {
            //If the closest NPC is not dead and the player has pressed the mouse down
            if (Input.GetMouseButtonDown(0))
            {
                //Obtain the sprite renderer from the closest NPC
                SpriteRenderer NPC_Sprite = go.GetComponent<SpriteRenderer>();

                //Start the corountine for flashing the sprite
                StartCoroutine(SpriteFlash(NPC_Sprite));

                //A variable to store the value of the PC attack value
                int PC_Attack = GetComponent<TL_CharStats>().AttackValue;

                //Obtains the script from the NPC gameobject
                CharacterScript = go.GetComponent<TL_CharStats>();

                //Input the gameobject receiving the damage with the function
                CharacterScript.ReceiveDamage(go, PC_Attack);
            }
        }
    }

    public IEnumerator SpriteFlash(SpriteRenderer Sprite)
    {
        //Check throughout every sprite flash if the character is still alive and if not then stop the coroutine
        if (Sprite == null)
        {
            StopCoroutine(SpriteFlash(Sprite));
            NPC_Detected = false;
        }
        else
        {
            Sprite.color = new Color(255f, 255f, 255f, 0f);
        }
        
        yield return new WaitForSeconds(0.15f);

        if (Sprite == null)
        {
            StopCoroutine(SpriteFlash(Sprite));
            NPC_Detected = false;
        }
        else
        {
            Sprite.color = new Color(255f, 255f, 255f, 255f);
        }
        
        yield return new WaitForSeconds(0.15f);

        if (Sprite == null)
        {
            StopCoroutine(SpriteFlash(Sprite));
            NPC_Detected = false;
        }
        else
        {
            Sprite.color = new Color(255f, 255f, 255f, 0f);
        }
        
        yield return new WaitForSeconds(0.15f);

        if (Sprite == null)
        {
            StopCoroutine(SpriteFlash(Sprite));
            NPC_Detected = false;
        }
        else
        {
            Sprite.color = new Color(255f, 255f, 255f, 255f);
        }        
        yield return new WaitForSeconds(0.15f);
        StopCoroutine(SpriteFlash(Sprite));
    }

    void InvertMovement(float x, float z)
    {
        if (MoveScript.TargetPos.x > x)         //If X is more than the target position of X
        {
            //Move the PC to the left and move the doppelganger to the right
            MoveScript.TargetPos.x -= 1f;
            NPCMoveScript.TargetPos.x += 1f;
        }
        else if(MoveScript.TargetPos.x < x)     //If X is less than the target position of X
        {
            //Move the PC to the right and move the doppelganger to the left
            MoveScript.TargetPos.x += 1f;
            NPCMoveScript.TargetPos.x -= 1f;
        }
        else if (MoveScript.TargetPos.z > z)    //If Z is more than the target position of Z
        {
            //Move the PC upwards and move the doppelganger downwards
            MoveScript.TargetPos.z -= 1f;
            NPCMoveScript.TargetPos.z += 1f;
        }
        else if (MoveScript.TargetPos.z < z)    //If Z is less than the target position of Z
        {
            //Move the PC downwards and move the doppelganger upwards
            MoveScript.TargetPos.z += 1f;
            NPCMoveScript.TargetPos.z -= 1f;
        }

    }

}
