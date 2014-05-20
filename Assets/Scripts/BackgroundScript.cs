using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {

	public Sprite plain;
	public Sprite bush1;
	public Sprite bush2;
	public Sprite bush3;

	public void setSprite()
	{
		// choose a random number between 1-100
		// 1-3 represents the bush 
		// 4-99 represents plain background
		int v = Random.Range(1,100);

		if (v > 3)
			this.gameObject.GetComponent<SpriteRenderer>().sprite = plain;
		else if(v == 1)
			this.gameObject.GetComponent<SpriteRenderer>().sprite = bush1;
		else if(v == 2)
			this.gameObject.GetComponent<SpriteRenderer>().sprite = bush2;
		else if(v == 3)
			this.gameObject.GetComponent<SpriteRenderer>().sprite = bush3;
	}
}
