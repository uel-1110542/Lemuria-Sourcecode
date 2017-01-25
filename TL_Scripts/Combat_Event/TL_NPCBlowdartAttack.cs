using UnityEngine;
using System.Collections.Generic;

public class TL_NPCBlowdartAttack : MonoBehaviour {

    //Variables
    public float NPC_Attack;
    public float NPC_AttackRate;
    public float NPC_Cooldown;

    public GameObject NPC_RangedHitbox;
    public GameObject NPC_Projectile;
    public Vector3 PC_PrevPos;

    private GameObject PC;
    private List<GameObject> RangedHitboxes = new List<GameObject>();
    private TL_NPCMovement MovementScript;
    private TL_ActiveTimeBattle CombatEventScript;


    void Start()
    {
        //Finds the gameobjects and obtains the script
        MovementScript = GetComponent<TL_NPCMovement>();

        for (int x = 1; x < 5; x++)
        {
            //Creates the marker for the NPC's ranged attack
            GameObject Ranged_HitboxClone = (GameObject)Instantiate(NPC_RangedHitbox, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
            TL_FollowChar FollowCharScript = Ranged_HitboxClone.AddComponent<TL_FollowChar>();

            //Set the variables from the script
            FollowCharScript.X_Pos = -x;
            FollowCharScript.Z_Pos = 0f;
            FollowCharScript.Character = gameObject;

            //Rotates the ranged hitbox to face upwards
            Ranged_HitboxClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }

        //Adds cooldown to prevent the NPC's ranged attack from immediately happening
        NPC_Cooldown = NPC_AttackRate + Time.realtimeSinceStartup;
    }

    void Update()
    {
        RangedAttack();
        ProjectileLength();
    }

    void ProjectileLength()
    {
        //Locates the projectile for its' position
        GameObject Blowdart = GameObject.Find("Blowdart_Projectile(Clone)");

        //If the projectile travels 4 tiles away from its' shooter then destroy the projectle
        if (Blowdart != null && Vector3.Distance(transform.position, Blowdart.transform.position) >= 4f)
        {
            Destroy(Blowdart);
        }
    }

    void RangedAttack()
    {
        //If the cooldown is less than the time since the startup
        if (NPC_Cooldown < Time.realtimeSinceStartup)
        {
            //Locates the PC for its' position
            PC = GameObject.FindGameObjectWithTag("PC");
            PC_PrevPos = PC.transform.position;

            //Spawn the projectile
            GameObject Blowdart_Projectile = (GameObject) Instantiate (NPC_Projectile, new Vector3(transform.position.x - 1f, 0.1f, transform.position.z), Quaternion.identity);

            //Obtain the rigidbody from the projectile
            Rigidbody ProjectileForce = Blowdart_Projectile.GetComponent<Rigidbody>();

            //Add force onto the projectile
            ProjectileForce.AddForce(Vector3.left * 200f);

            //Adds the cooldown
            NPC_Cooldown = NPC_AttackRate + Time.realtimeSinceStartup;
        }

    }

}
