using UnityEngine;

public class TL_CharStats : MonoBehaviour {

	//Variables
	public float MaxHealth;
	public float CurrentHealth;
	public int AttackValue;
	public int DefendValue;
	public int DamageReceived;
	private TL_CharStats CharacterScript;
	private Quaternion OriginalRotation;


	void Start()
	{
        //Assign current health to max health as the default value
		CurrentHealth = MaxHealth;
	}
    
	public void ReceiveDamage(GameObject go, int dmg)
	{
        //If the gameobject is still alive
		if(go != null)
		{
            //Obtain the script from the gameobject parameter
			CharacterScript = go.GetComponent<TL_CharStats>();

            //If the defense value is more than the damage
			if(DefendValue > dmg)
			{
				CharacterScript.CurrentHealth -= 1f;
			}
			else
			{
                //Reduce the current health based on the outcome of the damage
				CharacterScript.CurrentHealth -= (dmg - DefendValue);
			}

            //If the character is dead
			if(CharacterScript.CurrentHealth <= 0)
			{
                //If the PC is not null
				if(go != GameObject.FindGameObjectWithTag("PC"))
				{
                    //Destroy PC
					Destroy(go);
				}
				else
				{
                    //If the character's health is less than or equal to 0
					if(CharacterScript.CurrentHealth <= 0f)
					{
                        //Sets current health to default value
						CharacterScript.CurrentHealth = CharacterScript.MaxHealth;

                        //Loads the level map
                        GameObject.FindGameObjectWithTag("LM").GetComponent<DN_Event_Manager>().ReturnToMap();
                    }
                }

			}

		}

	}

}
