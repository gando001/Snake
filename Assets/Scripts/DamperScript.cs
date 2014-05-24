using UnityEngine;
using System.Collections;

public class DamperScript : MonoBehaviour {

	private string selected_item;
	private bool spinFinished;

	// Use this for initialization
	void Start () {

		selected_item = "";
		spinFinished = false;
	}

	// Update is called once per frame
	void Update () 
	{
		if (spinFinished)
			print ("Item you got is "+selected_item);
	}

	void OnTriggerStay(Collider otherCollider)
	{
		// OnTriggerStay(Collider otherCollider) is invoked when another 
		// collider marked as a "Trigger" is touching this object collider.
		
		// determine the other collider
		print(selected_item+":"+otherCollider.gameObject.name);
		if (otherCollider.gameObject.name != selected_item)
			selected_item = otherCollider.gameObject.name;
		else
			spinFinished = true;
	}
}