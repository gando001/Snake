using UnityEngine;
using System.Collections;

public class WheelScript : MonoBehaviour {

	private bool isSwipe;
	private bool isSpinning;
	private bool isFinished;
	private string text;

	// speed
	private float speed;
	private const int MIN_SPEED = 500;
	private const int MAX_SPEED = 1000;

	// physics variables
	private float startY;
	private float startTime;
	private float endY;
	private float endTime;

	public void resetWheel()
	{
		speed = 0;
		isSwipe = false;
		isSpinning = false;
		isFinished = false;
	}

	public void displayResult(string txt)
	{
		text = txt;
		isFinished = true;
	}

	// Use this for initialization
	void Start () 
	{
		resetWheel();
	}

	void OnGUI ()
	{
		if (GUI.Button(new Rect(10,10,100,50), "Main Menu"))
		{	
			// Reload the level
			Application.LoadLevel("menu");
		}

		if (isFinished)
		{
			if (GUI.Button(new Rect(120,10,100,50), text))
			{	
				GameObject.Find("Game").GetComponent<GameScript>().hideBonusWheel();
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isSwipe && Time.timeScale != 0)
		{
			/*if (Input.touchCount > 0) 
			{
				Touch touch = Input.GetTouch(0);
				
				// user is swiping the screen
				if (touch.phase == TouchPhase.Began)
				{
					// save the y point and time when the mouse was pressed
					startY = Input.GetTouch(0).position.y;
					startTime = Time.time;
				}
				else if (touch.phase == TouchPhase.Ended)
				{
					// get the distance moved and time
					endY =  Input.GetTouch(0).position.y - startY;
					endTime = Time.time - startTime;
					
					// speed is distance divided by time
					speed = Mathf.Abs(endY/endTime);

					if (speed > MIN_SPEED)
					{
						isSwipe = true;
						isSpinning = true;
					}
				}
			}
				*/
			if(Input.GetMouseButtonDown(0))
			{
				// save the y point and time when the mouse was pressed
				startY = Input.mousePosition.y;
				startTime = Time.time;
			}
			else if(Input.GetMouseButtonUp(0))
			{
				// get the distance moved and time
				endY =  Input.mousePosition.y - startY;
				endTime = Time.time - startTime;

				// speed is distance divided by time
				speed = Mathf.Abs(endY/endTime);

				print (speed);
				if (speed > MIN_SPEED)
				{
					isSwipe = true;
					isSpinning = true;
				}
			}

			if (isSpinning)
			{
				float z = 10;
				if (endY < startY)
					z = -z;
				rigidbody.maxAngularVelocity = 100; // this allows for varying torque values
				rigidbody.AddTorque(new Vector3(0,0,z) * speed);
				isSpinning = false;

				GameObject.Find("Damper").GetComponent<DamperScript>().setStarted(); // notify the damper
			}
		}
	}
}