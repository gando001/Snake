using UnityEngine;
using System.Collections;

public class DamperScript : MonoBehaviour {

	// sounds
	public AudioClip damper_sound;

	private string selected_item;
	private bool spinStarted;
	private float time;
	private GameObject wheel;
	private GameScript game;
	private SoundScript sound;

	public void setStarted()
	{
		spinStarted = true;
	}

	public void reset()
	{
		selected_item = "";
		spinStarted = false;
		time = -1;
	}


	// Use this for initialization
	void Start () 
	{
		reset();
		wheel = GameObject.Find("Wheel");
		game = GameObject.Find("Game").GetComponent<GameScript>();
		sound = GameObject.Find("Sound").GetComponent<SoundScript>();
	}

	// Update is called once per frame
	void Update () 
	{
		if (!game.isPaused())
		{
			if (spinStarted)
			{
				time--;
		
				if (time == 0)
				{
					wheel.GetComponent<WheelScript>().displayResult(selected_item);
					spinStarted = false;
				}
			}
		}
	}

	void OnTriggerStay(Collider otherCollider)
	{	
		// determine the other collider
		selected_item = otherCollider.gameObject.name;
	}

	void OnTriggerExit(Collider otherCollider)
	{
		// reset the timer after each exit
		// the chosen item will not have its time reset
		time = 300;
	}

	void OnCollisionEnter(Collision otherCollider)
	{
		// play the sound
		if (sound.isSoundPlaying())
			audio.PlayOneShot(damper_sound);
	}
}