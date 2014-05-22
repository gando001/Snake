using UnityEngine;
using System.Collections;

public class ScoreTextScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		// animate the score by raising it vertically
		iTween.ValueTo(gameObject, iTween.Hash("from", Vector2.zero, "to", new Vector2(0,30), "onupdate", "raiseScore", "oncomplete", "reset", "time", 0.5f));
	}
	
	void raiseScore(Vector2 v)
	{
		// raise the gui text
		gameObject.guiText.pixelOffset = v;
	}
	
	void reset()
	{
		// set the gameobject to inactive
		gameObject.SetActive(false);
	}
}
