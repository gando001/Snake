using UnityEngine;
using System.Collections;

public class SoundScript : MonoBehaviour {

	public Sprite sound_sprite;
	public Sprite mute_sprite;
	private bool sound;

	public void setSoundPlaying(bool v) {
		sound = v;

		if (isSoundPlaying())
			GetComponent<SpriteRenderer>().sprite = sound_sprite;
		else
			GetComponent<SpriteRenderer>().sprite = mute_sprite;
	}

	public bool isSoundPlaying() {
		return sound;
	}

	// Use this for initialization
	void Start () {
		setSoundPlaying(true);
	}

	void Update()
	{
		if (GameScript.REAL_DEVICE)
		{
			if (Input.touchCount > 0) 
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Ended && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
					setSoundPlaying(!isSoundPlaying());
			}
		}
		else
		{
			if(Input.GetMouseButtonUp(0) && isTouched(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
				setSoundPlaying(!isSoundPlaying());
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