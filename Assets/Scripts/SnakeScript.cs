﻿using UnityEngine;
using System.Collections;

public class SnakeScript : MonoBehaviour {

	// initial speed values
	public float speed;
	public Transform tail;
	public Transform body;

	// private variables
	private int score;
	private int score_limit;
	private int body_parts;
	private bool gameOver;
	private bool left;
	private bool right;
	private bool up;
	private bool down;
	private float lastUpdate;
	private Transform parent;
	private int row;
	private int col;
	private bool eaten;

	// Use this for initialization
	void Start () 
	{
		score = 0;
		score_limit = 15;
		body_parts = 0;
		gameOver = false;
		lastUpdate = 0;

		// set the snakes parent and position
		parent = GameObject.Find("2 - Foreground").transform;	
		transform.parent = parent;
		transform.position = new Vector3(col+transform.parent.position.x, row+transform.parent.position.y, (float)parent.position.z);

		// create the tail
		tail = Instantiate(tail) as Transform;
		tail.parent = parent;
		tail.name = "tail";
		tail.position = transform.position;
	}

	public bool isGameOver()
	{
		return gameOver;
	}
	
	public void setStartingPosition(int r, int c)
	{
		row = r;
		col = c;

		// need to randomize the direction and initial starting value
		right = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// get the keyboard values and calculate the movement
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");
		
		// determine the direction the user wants to go
		// need to remove the going left then able to go right and top -> bottom
		if (!right && inputX < 0)
		{
			// left
			left = true;
			right = false;	
			down = false;
			up = false;
		}
		else if (!left && inputX > 0)
		{
			// right
			left = false;
			right = true;	
			down = false;
			up = false;
		}
		else if (!down && inputY < 0)
		{
			// up
			left = false;
			right = false;	
			down = false;
			up = true;
		}
		else if (!up && inputY > 0)
		{
			// down	
			left = false;
			right = false;	
			down = true;
			up = false;
		}
	}

	void FixedUpdate(){
	
		// FixedUpdate() is called at every fixed framerate frame. 
		// You should use this method over Update() when dealing with physics ("RigidBody" and forces).

		if (Time.time - lastUpdate >= 0.5)
		{
			if (body_parts > 0)
			{
				Vector3 last_pos = GameObject.Find("body"+body_parts).transform.position;
				if (eaten && (Mathf.Abs(tail.position.x-last_pos.x) == 1 || Mathf.Abs(tail.position.y-last_pos.y) == 1))
				{
					// don't update the tails position this time to allow for new body part to be inserted
					eaten = false;
				}
				else if (eaten && body_parts > 1)
				{
					// adding first body part - make tail follow the new body part
					tail.position = GameObject.Find("body"+(body_parts-1)).transform.position;
				}
				else
				{
					// set the tail to the previous last bodies position
					tail.position = GameObject.Find("body"+body_parts).transform.position;
				}

				// set the position of each body to the body before it
				int i = body_parts;
				if (eaten)
					i--; // skip the last body
				while (i>1)
				{
					GameObject.Find("body"+i).transform.position = GameObject.Find("body"+(i-1)).transform.position; 
					i--;
				}

				// set the first body to the heads position
				GameObject.Find("body1").transform.position = transform.position;
			}
			else
			{
				// set the tail to the heads position - no bodies tail follows head
				tail.position = transform.position;
			}

			// continue in the same direction
			if (left)
				col--;
			else if (right)
				col++;
			else if (down)
				row++;
			else if(up)
				row--;

			// update the head position
			transform.position = new Vector3(col+transform.parent.position.x, row+transform.parent.position.y);

			lastUpdate = Time.time;
		}
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		// OnTriggerEnter2D(Collider2D otherCollider) is invoked when another 
		// collider marked as a "Trigger" is touching this object collider.

		// determine the other collider
		if (otherCollider.gameObject.name == "Frame" || otherCollider.gameObject.name == "Body") 
		{
			// game over as snake hit the frame or itself
			gameOver = true;
		}
		else if (otherCollider.gameObject.name == "Apple")
		{
			// move the apple
			//Destroy(otherCollider.gameObject);

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
			gameOver = true;
		}
		else
		{
			// add a new body to the snake
			body_parts++;
			body = Instantiate(body) as Transform;
			body.parent = parent;
			body.name = "body"+body_parts;
			body.position = transform.position; // set the position to the head
			eaten = true;
		}
	}
}