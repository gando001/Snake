using UnityEngine;
using System.Collections;

public class MenuButtonScript : MonoBehaviour {

	private GameScript game;

	// Use this for initialization
	void Start () {
		game = GameObject.Find("Game").GetComponent<GameScript>();
	}
	
	void Update()
	{
		if (GameScript.REAL_DEVICE)
		{
			if (Input.touchCount > 0) 
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Ended && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
				{
					// save the game state
					game.saveGame();

					// hide the banner ad
					Camera.main.GetComponent<GoogleMobileAdsScript>().HideBanner();
					
					// Reload the level
					Application.LoadLevel("menu");
				}
			}
		}
		else
		{
			if(Input.GetMouseButtonUp(0) && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
			{
				// save the game state
				game.saveGame();

				// hide the banner ad
				Camera.main.GetComponent<GoogleMobileAdsScript>().HideBanner();
				
				// load the main menu
				Application.LoadLevel("menu");
			}
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