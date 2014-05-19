using UnityEngine;
using System.Collections;

public class WheelTestScript : MonoBehaviour {

	private bool isSwipe;

	// speed
	private float speed;
	private const int MIN_SPEED = 5;
	private const int MAX_SPEED = 100;

	// physics variables
	private float startY;
	private float startTime;
	private float endY;
	private float endTime;

	// Use this for initialization
	void Start () 
	{
		speed = -1;
		isSwipe = false;
	}

	void OnGUI ()
	{
		if (GUI.Button(new Rect(10,10,100,50), "Main Menu"))
		{	
			// Reload the level
			Application.LoadLevel("menu");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isSwipe)
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
					
					// speed is distance divided by time - divide to add sensitivity
					speed = Mathf.Abs((endY/endTime)/MAX_SPEED);
					speed = Mathf.Min(MAX_SPEED, speed);

					if (speed > MIN_SPEED)
						isSwipe = true;
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

				// speed is distance divided by time - divide to add sensitivity
				speed = Mathf.Abs((endY/endTime)/MAX_SPEED);
				speed = Mathf.Min(MAX_SPEED, speed);
				print (speed);
				if (speed > MIN_SPEED)
					isSwipe = true;
			}
		}

		if (isSwipe && speed > 0)
		{
			if (endY < startY)
				transform.Rotate(Vector3.up * speed);
			else
				transform.Rotate(Vector3.down * speed);
		}
	}

	// FixedUpdate() is called at every fixed framerate frame. 
	void FixedUpdate () 
	{	
		if (isSwipe && speed > 0)
		{
			if (speed > MAX_SPEED/2)
				speed -= 5;
			else if (speed > MAX_SPEED/4)
				speed -= 2.5f;
			else if (speed > 17.5f)
				speed -= 1f;
			else if (speed > 10)
				speed -= 0.5f;
			else if (speed > 2.5f)
				speed -= 0.05f;
			else if (speed > 1)
				speed -= 0.01f;
			else
				speed -= 0.005f;
		//	print (speed);
		}
	}
}