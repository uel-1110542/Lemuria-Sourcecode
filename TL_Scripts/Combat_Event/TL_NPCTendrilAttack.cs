using UnityEngine;

public class TL_NPCTendrilAttack : MonoBehaviour {

    //Variables
    public float NPC_Attack;
    public float NPC_AttackRate;
    public float NPC_Cooldown;

    public GameObject NPC_RangedHitbox;
    public GameObject NPC_Tendril;
    public Vector3 PC_PrevPos;

    private GameObject PC;
    private TL_NPCMovement MovementScript;
    private TL_ActiveTimeBattle CombatEventScript;


    void Start()
    {
        //Finds the gameobjects and obtains the script
        MovementScript = GetComponent<TL_NPCMovement>();

        //Adds cooldown to prevent the NPC's ranged attack from immediately happening
        NPC_Cooldown = NPC_AttackRate + Time.realtimeSinceStartup;
    }

    void Update()
    {
        TendrilAttack();
    }

    void TendrilAttack()
    {
        //If the cooldown is less than the time since the startup
        if (NPC_Cooldown < Time.realtimeSinceStartup)
        {
            //Destroys the previous gameobjects
            Destroy(GameObject.Find("Ranged_Hitbox(Clone)"));
            Destroy(GameObject.Find("Tendril_Platform(Clone)"));

            //Locates the PC for its' position
            PC = GameObject.FindGameObjectWithTag("PC");
            PC_PrevPos = PC.transform.position;

            //Creates the marker for the NPC's ranged attack
            GameObject Ranged_HitboxClone = (GameObject)Instantiate(NPC_RangedHitbox, new Vector3(PC_PrevPos.x, 0.1f, PC_PrevPos.z), Quaternion.identity);

            //Rotates the ranged hitbox to face upwards
            Ranged_HitboxClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

            //Spawn the tendril
            GameObject TendrilClone = (GameObject)Instantiate(NPC_Tendril, new Vector3(PC_PrevPos.x, -0.5f, PC_PrevPos.z), Quaternion.identity);
            
            //Adds the cooldown
            NPC_Cooldown = NPC_AttackRate + Time.realtimeSinceStartup;
        }

    }

}
