using UnityEngine;
using System.Collections;

public class TL_AnimateSpikes : MonoBehaviour {

    private Animator Spike_Anim;

	private float RateOfFading = 0.075f;
	private float FadingCooldown;
	private float RateOfActivation = 0.025f;
    
    private Color Opacity;
    private bool DetractSpike = false;
    public bool AreSpikesActivated = false;
    public bool TriggerSpikeAnim = false;
	public int CurrentIndex;


	void Start()
	{
		//Add cooldown to prevent spikes to instantly protrude
		FadingCooldown = RateOfFading + Time.realtimeSinceStartup;

        //Obtain the animator component from this gameobject
        Spike_Anim = gameObject.GetComponent<Animator>();
    }

	void Update()
	{
		//Function for fading out spike blocks
		FadeBlock();
	}

	void FadeBlock()
	{
		//Set the color to default
		Opacity = transform.gameObject.GetComponent<SpriteRenderer>().color;

		//If the cooldown is less than the time since startup and the spike isn't triggered
		if(FadingCooldown < Time.realtimeSinceStartup && !TriggerSpikeAnim)
		{
			//Increase alpha channel
			Opacity.a += 2f * Time.deltaTime;

			//If the alpha is high enough, trigger the spike
			if(Opacity.a > 0.9f)
			{
                Spike_Anim.enabled = true;
                TriggerSpikeAnim = true;
                StartCoroutine(AnimateSpike());
			}
			//Set the opacity to show fading
			transform.gameObject.GetComponent<SpriteRenderer>().color = Opacity;

			//Adds the cooldown
			FadingCooldown = RateOfFading + Time.realtimeSinceStartup;
		}

	}

	IEnumerator AnimateSpike()
	{
        //Wait for 1 seconds
        yield return new WaitForSeconds(0.17f);

        //Enable the animation
        Spike_Anim.enabled = false;

        //Set the bool to true
        AreSpikesActivated = true;

        //Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        //Enable the animation
        Spike_Anim.enabled = true;

        //Set the trigger
        Spike_Anim.SetTrigger("RetractSpikes");

        //Set the bool to false
        AreSpikesActivated = false;

        //Wait for 1 seconds
        yield return new WaitForSeconds(0.17f);

        //Restart the animation
        Spike_Anim.SetTrigger("Restart");

        //Set the bool to false
        Spike_Anim.enabled = false;

        //Set opacity to 0
        Opacity.a = 0f;

        //Change the opacity of the color
        transform.gameObject.GetComponent<SpriteRenderer>().color = Opacity;
        
        //Wait for 5 secs
        yield return new WaitForSeconds(5f);

        //Set the bool to false
        TriggerSpikeAnim = false;
    }

    void OnTriggerStay(Collider Col)
    {
        if (Col.tag == "PC")
        {
            //Obtain the script from the collided gameobject
            TL_MoveScript MoveScript = Col.gameObject.GetComponent<TL_MoveScript>();

            //If the PC isn't moving and the spikes are activated
            if (!MoveScript.IsCharMoving && AreSpikesActivated)
            {
                //Obtain the timer script from this gameobject
                TL_Timer TimerScript = GameObject.Find("LevelArea").GetComponent<TL_Timer>();

                //Activate lose condition
                TimerScript.CheckCondition(false);
            }
        }

    }

}
