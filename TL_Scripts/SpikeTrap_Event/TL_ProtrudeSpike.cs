using UnityEngine;
using System.Collections;

public class TL_ProtrudeSpike : MonoBehaviour {

	//Variables
	private float RateOfFading = 0.075f;
	private float FadingCooldown;
	private Color StartingColor;
	private Color Opacity;
	private GameObject Spike;
	private bool TriggerSpike = false;
    

	void Start()
	{
        //Add cooldown to prevent spikes to instantly protrude
        FadingCooldown += Time.realtimeSinceStartup;

        //Finds the child gameobject
        Spike = transform.FindChild("Spike").gameObject;

        //Obtains the color component from the renderer
		StartingColor = transform.gameObject.GetComponent<Renderer>().material.color;
	}

	void Update()
	{
        //Function for fading out spike blocks
		FadeBlock();
	}

	void FadeBlock()
	{
        //Set the color to default
		Opacity = transform.gameObject.GetComponent<Renderer>().material.color;

        //If the cooldown is less than the time since startup and the spike isn't triggered
		if(FadingCooldown < Time.realtimeSinceStartup && !TriggerSpike)
		{
            //Reduce opacity
			Opacity.r -= 2f * Time.deltaTime;
			Opacity.b -= 2f * Time.deltaTime;
			Opacity.g -= 2f * Time.deltaTime;

            //If the opacity is low enough, trigger the spike
			if(Opacity.r < 0.4f)
			{
				TriggerSpike = true;
			}
            //Set the opacity to show fading
			transform.gameObject.GetComponent<Renderer>().material.color = Opacity;

            //Adds the cooldown
			FadingCooldown = RateOfFading + Time.realtimeSinceStartup;	
		}
        //Start the co-routine function
		StartCoroutine(Wait());
	}

	IEnumerator Wait()
	{
		if(TriggerSpike)
		{
            //Protrude the spike
            Spike.transform.localPosition = Vector3.MoveTowards(Spike.transform.localPosition, new Vector3(Spike.transform.localPosition.x, 0.5f, Spike.transform.localPosition.z), 10f * Time.deltaTime);

            //Wait for 3 seconds
            yield return new WaitForSeconds(3);

            //Retract the spike
            Spike.transform.localPosition = Vector3.MoveTowards(Spike.transform.localPosition, new Vector3(Spike.transform.localPosition.x, -0.5f, Spike.transform.localPosition.z), 10f * Time.deltaTime);

            //Assign the default color
            Opacity = StartingColor;

            //Set the color to default with a variable
            transform.gameObject.GetComponent<Renderer>().material.color = Opacity;

            //Set the bool to false
            TriggerSpike = false;
		}
	}

}
