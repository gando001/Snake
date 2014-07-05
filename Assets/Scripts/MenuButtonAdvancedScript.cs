using UnityEngine;
using System.Collections;

public class MenuButtonAdvancedScript : MonoBehaviour {

	public const int PAUSED = 1;
	public const int WON = 2;
	public const int LOST = 3;
	public const int RETRY = 4;

	public Sprite play_sprite, retry_sprite;
	public AudioClip button_sound;

	private int state;
	private GameScript game;
	private SoundScript sound;

	// Use this for initialization
	void Start () {
		game = GameObject.Find("Game").GetComponent<GameScript>();
		sound = GameObject.Find("Sound").GetComponent<SoundScript>();
		state = 0;
	}
	
	void Update()
	{
		if (GameScript.REAL_DEVICE)
		{
			if (Input.touchCount > 0) 
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Ended && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
					buttonHit();
			}
		}
		else
		{
			if(Input.GetMouseButtonUp(0) && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
				buttonHit();
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

	// changes the state of this button based on the given value
	public void setState(int v)
	{
		state = v;
	
		// alter the sprite
		if (state == PAUSED || state == WON)
			GetComponent<SpriteRenderer>().sprite = play_sprite;
		else
			GetComponent<SpriteRenderer>().sprite = retry_sprite;
	}

	// executes the relevant functionality based on the buttons current state
	void buttonHit()
	{
		// play the sound
		if (sound.isSoundPlaying())
			audio.PlayOneShot(button_sound);
		
		if (state == PAUSED)
			game.setPause(false);
		else if (state == WON)
			game.levelPassed();
		else if (state == LOST)
			game.levelLost();
		else if (state == RETRY)
			game.levelRetry();



		// hide the banner ad
		Camera.main.GetComponent<GoogleMobileAdsScript>().HideBanner();
	}
}