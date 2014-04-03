using UnityEngine;
using System.Collections;

public class SnakeScript : MonoBehaviour {

	// initial speed values
	public Vector2 speed = new Vector2(2,2);
	public Transform tail;
	public Transform body;

	// private variables
	private Vector2 direction;
	private Vector2 movement;
	private int score;
	private int score_limit;
	private int body_parts;

	// Use this for initialization
	void Start () 
	{
		score = 0;
		score_limit = 15;
		body_parts = 0;

		// need to randomize the direction and initial values
		direction = new Vector2(1, 0); 
		float startX = 0;
		float startY = 0;

		// create the tail and assign a position
		var tailTransform = Instantiate(tail) as Transform;
	
		// based on the initial direction the tail position needs to be adjusted
		float x = startX-(this.gameObject.renderer.bounds.size.x*this.gameObject.renderer.transform.localScale.x); // get the render bounds x position (sprite bounds x) and multiply
		// by the scale of 0.25 and adjust as required
		tailTransform.position = new Vector3(x, startY); // based on the initial direction the tail position needs to be adjusted
		tail = tailTransform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// get the keyboard values and calculate the movement
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");

		// determine the direction the user wants to go
		// need to remove the going left then able to go right and top -> bottom
		if (inputX < 0)
		{
			direction.x = -1;
			direction.y = 0;
		}
		else if (inputX > 0)
		{
			direction.x = 1;
			direction.y = 0;
		}
		else if (inputY < 0)
		{
			direction.x = 0;
			direction.y = -1;
		}
		else if (inputY > 0)
		{
			direction.x = 0;
			direction.y = 1; // for diagnoal movement remove the = 0 part from each if statement
		}

		movement = new Vector2 (speed.x * direction.x, speed.y * direction.y);
	}

	void FixedUpdate(){
	
		// FixedUpdate() is called at every fixed framerate frame. 
		// You should use this method over Update() when dealing with physics ("RigidBody" and forces).
		rigidbody2D.velocity = movement;
		tail.rigidbody2D.velocity = movement;
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		// OnTriggerEnter2D(Collider2D otherCollider) is invoked when another 
		// collider marked as a "Trigger" is touching this object collider.

		// determine the other collider
		if (otherCollider.gameObject.name == "Frame" || otherCollider.gameObject.name == "Body") 
		{
			// game over as snake hit the frame or itself

		}
		else if (otherCollider.gameObject.name == "Apple")
		{
			// remove the apple
			Destroy(otherCollider.gameObject);

			// increment the snake
			incrementSnake();
		}
	}

	void incrementSnake()
	{
		score++;
		if (score == score_limit)
		{
			// game over the user has passed this level
		}
		else
		{
			// add a new body to the snake
			addBody();

			// add more apples
		}
	}

	void addBody()
	{
		// increment the counter
		body_parts++;

		// add the bodies

		// adjust the tail
	}

	void addBodies()
	{
		// loop through adding bodies to the snake
		for(int i=0; i<this.body_parts; i++)
		{

		}
	}
}