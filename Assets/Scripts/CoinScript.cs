using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

	private const int TTL = 1000;
	private int score;
	private float seconds;
	private float x,y;
	private bool isFreeze;

	// Use this for initialization
	void Start () 
	{
		score = 50;
		seconds = TTL;

		x = gameObject.transform.position.x;
		y = gameObject.transform.position.y;
	}
	
	// FixedUpdate() is called at every fixed framerate frame. 
	void FixedUpdate () 
	{
		if (!isFreeze)
		{
			// only display the coin for a limited number of frames
			if (seconds == 0)
				this.gameObject.SetActive(false);

			if (seconds > 0)
				seconds--;

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
	}

	public int getScoreValue()
	{
		return score;
	}

	public void reset()
	{
		seconds = TTL;
	}

	public void setFreeze(bool v)
	{
		isFreeze = v;
	}
}
