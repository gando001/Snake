using UnityEngine;
using System.Collections;

public class SnakeScript : MonoBehaviour {

	// initial speed values
	public float speed;
	public Transform tail;
	public Transform body;

	// snake logic
	private int score;
	private int body_limit;
	private int body_parts;
	private bool levelPassed;
	private bool left;
	private bool right;
	private bool up;
	private bool down;
	private float lastUpdate;
	private Transform parent;
	private int row;
	private int col;
	private bool eaten;
	private GameScript gameScript;
	private ArrayList bodies;
	
	public void setHeadStartingPosition(int r, int c, int direction)
	{
		row = r;
		col = c;

		if (direction == GameScript.LEFT)
			left = true;
		else if (direction == GameScript.RIGHT)
			right = true;
		else if (direction == GameScript.UP)
			up = true;
		else
			down = true;

		// set the snakes parent, position and scale
		parent = GameObject.Find("Foreground").transform;	
		transform.parent = parent;
		transform.position = gameScript.getScaledPostion(col+transform.parent.position.x, row+transform.parent.position.y, (float)parent.position.z);
		transform.localScale = gameScript.getScaledPostion(transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

	public void setTailStartingPosition(int r, int c)
	{
		// create the tail
		tail = Instantiate(tail) as Transform;
		tail.parent = parent;
		tail.name = "tail";
		tail.position = gameScript.getScaledPostion(c+transform.parent.position.x, r+transform.parent.position.y, (float)parent.position.z);
		tail.localScale = gameScript.getScaledPostion(tail.localScale.x, tail.localScale.y, tail.localScale.z);
	}

	public void setGameScript(GameScript gs)
	{
		// used to get access to the grid
		gameScript = gs;
	}

	public bool isLevelPassed()
	{
		return levelPassed;
	}
	
	public bool isEaten()
	{
		return eaten;
	}

	public int getScore()
	{
		return score;
	}

	public void setScore(int s)
	{
		score = s;
	}



	// Use this for initialization
	void Start () 
	{
		score = 0;
		body_limit = 50;
		body_parts = 0;
		lastUpdate = 0;
		bodies = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*// user input
		if (Input.touchCount > 0) 
		{
			Touch touch = Input.GetTouch(0);

			// user input is swiping the screen
			if (touch.phase == TouchPhase.Moved)
			{
				Vector2 deltaPostion = Input.GetTouch(0).deltaPosition;

				// determine the direction the user wants to go
				float dx = deltaPostion.x;
				float dy = deltaPostion.y;
				float delta = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
				float inputX = 0;
				float inputY = 0;
				if (delta == Mathf.Abs(dx))
				{
					// user chose a horizontal movement
					inputX = dx;
				}
				else
				{
					// user chose a vertical movement
					inputY = dy;
				}

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
		}*/

		// Use for testing in Unity not on device
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

		// update when it is time to and the game isn't over
		if (Time.time - lastUpdate >= speed && !gameScript.isGameOver())
		{
			// continue in the same direction
			if (left)
				col--;
			else if (right)
				col++;
			else if (down)
				row++;
			else if(up)
				row--;

			// only move the snake if the new cell is empty
			if (gameScript.isGridEmpty(row,col))
			{
				if (isEaten())
				{
					// create the new body at the heads position
					createBody();

					// do not move the snake
					eaten = false;
				}
				else if (body_parts > 0)
				{
					// make tail follow the first body part
					updateGridFromTail();
					tail.position = GameObject.Find("body1").transform.position;

					// set the position of each body to the body in front of it
					for (int i=1; i<body_parts; i++)
					{
						GameObject.Find("body"+i).transform.position = GameObject.Find("body"+(i+1)).transform.position; 
					}

					// set the latest body added to the heads position
					GameObject.Find("body"+body_parts).transform.position = transform.position;
				}
				else
				{	
					// set the tail to the heads position - no bodies tail follows head
					updateGridFromTail();
					tail.position = transform.position;
				}
	
				// update the head position
				transform.position = gameScript.getScaledPostion(col+transform.parent.position.x, row+transform.parent.position.y, transform.position.z);

				// update the grid
				gameScript.updateGrid(row, col, GameScript.SNAKE);
			}
			else
			{
				// snake tried to move to a non-empty cell
				gameScript.setGameOver(true);
				levelPassed = false;

				// vibrate the device
				Handheld.Vibrate ();

				// flash the snake
				InvokeRepeating("flashSnake", 0, 0.25f);
			}

			lastUpdate = Time.time;
		}
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		// OnTriggerEnter2D(Collider2D otherCollider) is invoked when another 
		// collider marked as a "Trigger" is touching this object collider.

		// determine the other collider
		if (otherCollider.gameObject.name == "Apple")
		{
			// increment the snake
			incrementSnake();
		}
	}

	void flashSnake()
	{
		// make the snake flash by setting the game objects of the snake to active and inactive
		if (this.transform.gameObject.activeSelf)
			this.transform.gameObject.SetActive(false);
		else
			this.transform.gameObject.SetActive(true);

		foreach(Transform body in bodies)
		{
			if (body.gameObject.activeSelf)
				body.gameObject.SetActive(false);
			else
				body.gameObject.SetActive(true);
		}

		if (tail.transform.gameObject.activeSelf)
			tail.transform.gameObject.SetActive(false);
		else
			tail.transform.gameObject.SetActive(true);
	}

	void incrementSnake()
	{
		score += 10;
		body_parts++;
		if (body_parts == body_limit)
		{
			// game won the user has passed this level
			gameScript.setGameOver(true);
			levelPassed = true;
		}
		else
		{
			// add a new body to the snake
			eaten = true;
			gameScript.moveApple();
		}
	}

	void createBody()
	{
		// create a new body prefab
		body = Instantiate(body) as Transform;
		body.parent = parent;
		body.name = "body"+body_parts;
		body.position = transform.position;
		body.localScale = gameScript.getScaledPostion(body.localScale.x, body.localScale.y, body.localScale.z);
		bodies.Add(body);
	}

	void updateGridFromTail()
	{
		// update the grid based on the tail position before it moves
		Vector2 cr = gameScript.getColAndRow(tail.position.x, tail.position.y);
		int row = Mathf.RoundToInt(cr.y-transform.parent.position.y);
		int col = Mathf.RoundToInt(cr.x-transform.parent.position.x);
		gameScript.updateGrid(row, col, GameScript.EMPTY);
	}
}