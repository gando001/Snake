using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {

	public Sprite plain;
	public Sprite bush;

	public void setSprite()
	{
		// choose a random number between 1-40
		// 1 represents the bush 
		// 1+ represents plain background
		int v = Random.Range(1,40);

		if (v > 1)
			this.gameObject.GetComponent<SpriteRenderer>().sprite = plain;
		else if(v == 1)
			this.gameObject.GetComponent<SpriteRenderer>().sprite = bush;
	}
}
