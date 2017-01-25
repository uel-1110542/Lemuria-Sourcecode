using UnityEngine;

public class TL_NPCRangedAttack : MonoBehaviour {

    //Variables
    public float NPC_Attack;
    public float NPC_AttackRate;
    public float NPC_Cooldown;

    public GameObject NPC_RangedHitbox;
    public GameObject NPC_Tendril;
    public GameObject NPC_Acid;
    public GameObject NPC_Blowdart;
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
        if (Projectile != null && Vector3.Distance(PC_PrevPos, Projectile.transform.position) < 0.5f)
        {
            Destroy(Projectile);
        }
    }

    void RangedAttack()
    {
        //If the cooldown is less than the time since the startup
        if (NPC_Cooldown < Time.realtimeSinceStartup)
        {
            //Finds all of the ranged hitboxes
            GameObject[] Ranged_Hitboxes = GameObject.FindGameObjectsWithTag("NPC_Hitbox");

            //Destroys the previous gameobjects
            foreach (GameObject go in Ranged_Hitboxes)
            {
                Destroy(go);
            }
            Destroy(GameObject.Find("Tendril_Platform(Clone)"));

            //Locates the PC for its' position
            PC = GameObject.FindGameObjectWithTag("PC");
            PC_PrevPos = PC.transform.position;

            //Creates the marker for the NPC's ranged attack
            GameObject Ranged_HitboxClone;

            if (gameObject.name == "Midget_Blowdart(Clone)")
            {
                for (int x = 1; x < 5; x++)
                {
                    Ranged_HitboxClone = (GameObject) Instantiate (NPC_RangedHitbox, new Vector3(transform.position.x - x, 0.1f, transform.position.z), Quaternion.identity);

                    //Rotates the ranged hitbox to face upwards
                    Ranged_HitboxClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                }
                //Spawn the projectile
                GameObject Blowdart_Projectile = (GameObject) Instantiate (NPC_Blowdart, new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z), Quaternion.identity);

                //Obtain the rigidbody from the projectile
                Rigidbody ProjectileForce = Blowdart_Projectile.GetComponent<Rigidbody>();

                //Add force onto the projectile
                ProjectileForce.AddForce(Vector3.left * 100f);

            }
            else
            {
                //Spawn the ranged hitbox
                Ranged_HitboxClone = (GameObject) Instantiate (NPC_RangedHitbox, new Vector3(PC_PrevPos.x, 0.1f, PC_PrevPos.z), Quaternion.identity);

                //Rotates the ranged hitbox to face upwards
                Ranged_HitboxClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }            

            //If the NPC is the Forest Spirit
            if (gameObject.name == "Forest_Spirit(Clone)")
            {
                //Spawn the tendril
                GameObject TendrilClone = (GameObject) Instantiate (NPC_Tendril, new Vector3(PC_PrevPos.x, -0.5f, PC_PrevPos.z), Quaternion.identity);
            }

            //If the NPC is the Pitcher Snake
            if (gameObject.name == "Pitcher_Snake(Clone)")
            {
                //Spawn the projectile
                GameObject SpitProjectileClone = (GameObject) Instantiate (NPC_Acid, transform.position, Quaternion.identity);

                //Obtain the rigidbody from the projectile
                Rigidbody ProjectileForce = SpitProjectileClone.GetComponent<Rigidbody>();

                //Calculate the distance
                Vector3 Distance = PC.transform.position - transform.position;

                //Face towards at the PC
                SpitProjectileClone.transform.LookAt(PC_PrevPos);

                //Add force onto the projectile
                ProjectileForce.AddForce(Distance * 50f);
            }

            //Adds the cooldown
            NPC_Cooldown = NPC_AttackRate + Time.realtimeSinceStartup;
        }
    }

}
