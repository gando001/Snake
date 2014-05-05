using UnityEngine;
using System.Collections;

public class AppleScript : MonoBehaviour {

	private int score;

	// Use this for initialization
	void Start () 
	{
		score = 50;
	}
	
	// Update is called once per frame
	void Update () {
		// rotate at 90 degrees per second
		transform.Rotate(Vector3.up * Time.deltaTime*90);
	}

	public int getScoreValue()
	{
		return score;
	}
}
