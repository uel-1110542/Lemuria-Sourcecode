using UnityEngine;

public class TL_NPCAcidAttack : MonoBehaviour {

    //Variables
    public float NPC_Attack;
    public float NPC_AttackRate;
    public float NPC_Cooldown;

    public GameObject NPC_RangedHitbox;
    public GameObject NPC_Projectile;
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
        RangedAttack();
        CheckProjectiles();
    }

    void CheckProjectiles()
    {
        //Locates the projectile for its' position
        GameObject Projectile = GameObject.Find("Spit_Projectile(Clone)");

        //If the projectile reaches its' target then destroy the projectile
        if (Projectile != null && Vector3.Distance(PC_PrevPos, Projectile.transform.position) < 1f)
        {
            Destroy(Projectile);
        }
    }

    void RangedAttack()
    {
        //If the cooldown is less than the time since the startup
        if (NPC_Cooldown < Time.realtimeSinceStartup)
        {
            //Destroys the previous ranged hitbox
            Destroy(GameObject.Find("Ranged_Hitbox(Clone)"));

            //Locates the PC for its' position
            PC = GameObject.FindGameObjectWithTag("PC");
            PC_PrevPos = PC.transform.position;

            //Creates the marker for the NPC's ranged attack
            GameObject Ranged_HitboxClone = (GameObject)Instantiate(NPC_RangedHitbox, new Vector3(PC_PrevPos.x, 0.1f, PC_PrevPos.z), Quaternion.identity);

            //Rotates the ranged hitbox to face upwards
            Ranged_HitboxClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

            //Calculate the distance
            Vector3 Distance = PC.transform.position - transform.position;

            //Spawn the projectile while facing towards at the PC
            GameObject SpitProjectileClone = (GameObject)Instantiate(NPC_Projectile, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.LookRotation(Distance));

            //Obtain the rigidbody from the projectile
            Rigidbody ProjectileForce = SpitProjectileClone.GetComponent<Rigidbody>();

            //Set initial velocity to 0
            ProjectileForce.velocity = Vector3.zero;

            //Add force onto the projectile
            ProjectileForce.AddForce(Distance.normalized * 200f);

            //Adds the cooldown
            NPC_Cooldown = NPC_AttackRate + Time.realtimeSinceStartup;
        }

    }

}
