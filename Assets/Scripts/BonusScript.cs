using UnityEngine;
using System.Collections;

public class BonusScript : MonoBehaviour {
	
	private const int TTL = 750;
	private float seconds;
	private float x,y;

	// Use this for initialization
	void Start () 
	{
		seconds = TTL;
		x = gameObject.transform.position.x;
		y = gameObject.transform.position.y;
	}
	
	// FixedUpdate() is called at every fixed framerate frame. 
	void FixedUpdate () 
	{
		// only display the coin for a limited number of frames
		if (seconds == 0)
			this.gameObject.SetActive(false);
		
		//if (seconds > 0)
			//seconds--;

		if (seconds < TTL/4)
		{
			if (seconds % 5 == 0)
			{
				if (gameObject.transform.position.z == 0)
					gameObject.transform.position = new Vector3(x,y,10);
				else 
					gameObject.transform.position = new Vector3(x,y,0);
			}
		}
	}
	
	public void reset()
	{
		seconds = TTL;
	}
}