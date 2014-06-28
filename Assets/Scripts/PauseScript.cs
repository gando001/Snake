using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {

	public Sprite pause_sprite;
	public AudioClip button_sound;

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
					// play the sound
					if (GameObject.Find("Sound").GetComponent<SoundScript>().isSoundPlaying())
						audio.PlayOneShot(button_sound);

					game.setPause(!game.isPaused());
				}
			}
		}
		else
		{
			if(Input.GetMouseButtonUp(0) && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
			{
				// play the sound
				if (GameObject.Find("Sound").GetComponent<SoundScript>().isSoundPlaying())
					audio.PlayOneShot(button_sound);

				game.setPause(!game.isPaused());
			}
		}

		if (game.isPaused())
			GetComponent<SpriteRenderer>().sprite = null;
		else
			GetComponent<SpriteRenderer>().sprite = pause_sprite;
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