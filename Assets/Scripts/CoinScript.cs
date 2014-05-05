using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

	private const int TTL = 1000;
	private int score;
	private float seconds;

	// Use this for initialization
	void Start () 
	{
		score = 50;
		seconds = TTL;
	}
	
	// FixedUpdate() is called at every fixed framerate frame. 
	void FixedUpdate () 
	{
		// only display the coin for a limited number of frames
		if (seconds == 0)
			this.gameObject.SetActive(false);

		if (seconds > 0)
			seconds--;
	}

	public int getScoreValue()
	{
		return score;
	}

	public void reset()
	{
		seconds = TTL;
	}
}
