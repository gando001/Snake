using UnityEngine;
using System.Collections;

public class BodyScript : MonoBehaviour {

	private int direction;

	// Use this for initialization
	void Start () 
	{
	}

	public int getDirection()
	{
		return direction;
	}

	public void setDirection(int d)
	{
		direction = d;
	}
}
