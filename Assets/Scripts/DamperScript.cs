using UnityEngine;
using System.Collections;

public class DamperScript : MonoBehaviour {

	private string selected_item;
	private bool spinStarted;
	private float time;

	public void setStarted()
	{
		spinStarted = true;
	}

	// Use this for initialization
	void Start () {

		selected_item = "";
		spinStarted = false;
		time = -1;
	}

	// Update is called once per frame
	void Update () 
	{
		if (spinStarted)
		{
			time--;

			if (time == 0)
			{
				print ("Item you got is "+selected_item);
				handleItem();
				spinStarted = false;
			}
		}
	}

	void OnTriggerStay(Collider otherCollider)
	{	
		// determine the other collider
		selected_item = otherCollider.gameObject.name;
	}

	void OnTriggerExit(Collider otherCollider)
	{
		// reset the timer after each exit
		// the chosen item will not have its time reset
		time = 100;
	}

	void handleItem()
	{	
		// handle the various wheel items
		if (selected_item == "Spin_again")
		{
			// reset the damper and wheel
			Start ();
			GameObject.Find("Wheel").GetComponent<WheelScript>().resetWheel();
		}
		else if (selected_item == "Spin_empty")
		{
			GameObject.Find("Wheel").GetComponent<WheelScript>().displayResult("You won nothing");
		}
	}
}