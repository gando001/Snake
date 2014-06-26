using UnityEngine;
using System.Collections;

public class WheelButtonScript : MonoBehaviour {

	public Sprite sprite;
	private bool isClicked;

	// Use this for initialization
	void Start () {
		isClicked = false;
	}

	public bool isButtonClicked() {
		return isClicked;
	}

	public void displaySpite() {
		GetComponent<SpriteRenderer>().sprite = sprite;
	}

	public void reset() {
		isClicked = false;
		GetComponent<SpriteRenderer>().sprite = null;
	}
	
	void Update()
	{
		if (GameScript.REAL_DEVICE)
		{
			if (Input.touchCount > 0) 
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Ended && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
					isClicked = true;
			}
		}
		else
		{
			if(Input.GetMouseButtonUp(0) && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
				isClicked = true;
		}
	}
	
	// checks whether the given touch position is within the sprite renderer bounds
	bool isTouched(Vector3 touch)
	{
		return (renderer.bounds.min.x < touch.x &&
		        renderer.bounds.max.x > touch.x &&
		        renderer.bounds.min.y < touch.y &&
		        renderer.bounds.max.y > touch.y);
	}
}