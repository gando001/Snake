using UnityEngine;
using System.Collections;

public class SnakeScript : MonoBehaviour {

	// initial speed values
	public float speed;
	public Transform tail;
	public Transform body;
	public Sprite body_corner_1;
	public Sprite body_corner_2;
	public Sprite body_corner_3;
	public Sprite body_corner_4;
	public Sprite body_normal;

	// snake logic
	private int score;
	private int body_limit;
	private int body_parts;
	private bool levelPassed;
	private bool left;
	private bool right;
	private bool up;
	private bool down;
	private bool getUserInput;
	private float lastUpdate;
	private Transform parent;
	private int row;
	private int col;
	private bool eaten;
	private GameScript gameScript;
	private ArrayList bodies;
	private int direction;
	
	public void setHeadStartingPosition(int r, int c, int direction)
	{
		row = r;
		col = c;
		this.direction = direction;

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
		transform.position = new Vector3(col+transform.parent.position.x, row+transform.parent.position.y, (float)parent.position.z);
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		transform.rotation = Quaternion.Euler(new Vector3(0,0,getRotation(direction)));
	}

	public void setTailStartingPosition(int r, int c)
	{
		// create the tail
		tail = Instantiate(tail) as Transform;
		tail.parent = parent;
		tail.name = "tail";
		tail.position = new Vector3(c+transform.parent.position.x, r+transform.parent.position.y, (float)parent.position.z);
		tail.localScale = new Vector3(tail.localScale.x, tail.localScale.y, tail.localScale.z);
		updateSprites();
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
		getUserInput = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*// user input
		if (Input.touchCount > 0) 
		{
			Touch touch = Input.GetTouch(0);

			// user input is swiping the screen
			if (touch.phase == TouchPhase.Moved && getUserInput)
			{
				Vector2 deltaPostion = Input.GetTouch(0).deltaPosition;

				// determine the direction the user wants to go
				float dx = deltaPostion.x;
				float dy = deltaPostion.y;
				float delta = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));

				if (delta == Mathf.Abs(dx))
				{
					// user chose a horizontal movement
					if (!right && dx < 0)
					{
						// left
						left = true;
						right = false;	
						down = false;
						up = false;
						direction = GameScript.LEFT;
					}
					else if (!left && dx > 0)
					{
						// right
						left = false;
						right = true;	
						down = false;
						up = false;
						direction = GameScript.RIGHT;
					}
				}
				else
				{
					// user chose a vertical movement
					if (!down && dy > 0)
					{
						// up
						left = false;
						right = false;	
						down = false;
						up = true;
						direction = GameScript.UP;
					}
					else if (!up && dy < 0)
					{
						// down	
						left = false;
						right = false;	
						down = true;
						up = false;
						direction = GameScript.DOWN;
					}
				}
				getUserInput =false;
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
				direction = GameScript.LEFT;
			}
			else if (!left && inputX > 0)
			{
				// right
				left = false;
				right = true;	
				down = false;
				up = false;
				direction = GameScript.RIGHT;
			}
			else if (!down && inputY > 0)
			{
				// up
				left = false;
				right = false;	
				down = false;
				up = true;
				direction = GameScript.UP;
			}
			else if (!up && inputY < 0)
			{
				// down	
				left = false;
				right = false;	
				down = true;
				up = false;
				direction = GameScript.DOWN;
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
			else if (up)
				row++;
			else if(down)
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
				transform.position = new Vector3(col+transform.parent.position.x, row+transform.parent.position.y, transform.position.z);
				transform.rotation = Quaternion.Euler(new Vector3(0,0,getRotation(direction)));

				// update the grid
				gameScript.updateGrid(row, col, GameScript.SNAKE);

				updateSprites();
				
				lastUpdate = Time.time;
				getUserInput = true;
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
		body.localScale = new Vector3(body.localScale.x, body.localScale.y, body.localScale.z);
	//	bodies.Add(body);
	}

	void updateSprites()
	{
		// rotates the sprites as required
		if (body_parts == 0)
		{
			// tail follows head
			tail.transform.rotation = Quaternion.Euler(getRotation(tail.transform.position, transform.position));
		}
		else
		{
			// update the body parts
			GameObject current = GameObject.Find("body"+body_parts);
			Vector3 rot = getRotation(current.transform.position, transform.position);
			int z = Mathf.RoundToInt(current.transform.rotation.eulerAngles.z);

			if (z != (int)rot.z && current.gameObject.GetComponent<SpriteRenderer>().sprite == body_normal)
			{
				// this body is on a corner so change its sprite
				current.gameObject.GetComponent<SpriteRenderer>().sprite = getCornerSprite(z, (int)rot.z);
				current.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
			}
			else
			{
				current.gameObject.GetComponent<SpriteRenderer>().sprite = body_normal;
				current.transform.rotation = Quaternion.Euler(rot);
			}

			for (int i=body_parts; i>1; i--)
			{
				current = GameObject.Find("body"+(i-1));
				rot = getRotation(current.transform.position, GameObject.Find("body"+i).transform.position);
				z = Mathf.RoundToInt(current.transform.rotation.eulerAngles.z);

				if (z != (int)rot.z && current.gameObject.GetComponent<SpriteRenderer>().sprite == body_normal)
				{
					// this body is on a corner so change its sprite
					current.gameObject.GetComponent<SpriteRenderer>().sprite = getCornerSprite(z, (int)rot.z);
					current.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
				}
				else
				{
					current.gameObject.GetComponent<SpriteRenderer>().sprite = body_normal;
					current.transform.rotation = Quaternion.Euler(rot);
				}
			}

			tail.transform.rotation = Quaternion.Euler(getRotation(tail.transform.position, GameObject.Find("body1").transform.position));
		}
	}

	Vector3 getRotation(Vector3 current, Vector3 future)
	{
		// returns a Vector for the rotation the sprite should apply based on its
		// current and future positions

		int curRow = Mathf.RoundToInt(current.y-transform.parent.position.y);
		int curCol = Mathf.RoundToInt(current.x-transform.parent.position.x);
		
		// determine the change in direction
		int newRow = Mathf.RoundToInt(future.y-transform.parent.position.y);
		int z = 0;
		if (newRow != curRow)
		{
			// vertical movement
			if (newRow > curRow)
			{
				// up
				z = 270;
			}
			else
			{
				// down
				z = 90;
			}
		}
		else
		{
			int newCol = Mathf.RoundToInt(future.x-transform.parent.position.x);
			// horizontal movement
			if (newCol > curCol)
			{
				// right
				z = 180;
			}
			else
			{
				// left
				z = 0;
			}
		}

		return new Vector3(0,0,z);
	}

	int getRotation(int direction)
	{
		if (direction == GameScript.LEFT)
			return 0;
		else if (direction == GameScript.RIGHT)
			return 180;
		else if (direction == GameScript.UP)
			return 270;
		else
			return 90;
	}

	Sprite getCornerSprite(int curZ, int newZ)
	{
		// returns the sprite required based on the given values
		if ((curZ == 180 && newZ == 270) || (curZ == 90 && newZ == 0))
		{
			// right -> up or down -> left
			return body_corner_1;
		}
		else if ((curZ == 0 && newZ == 270) || (curZ == 90 && newZ == 180))
		{
			// left -> up or down -> right
			return body_corner_2;
		}
		else if ((curZ == 0 && newZ == 90) || (curZ == 270 && newZ == 180))
		{
			// left -> down or up -> right
			return body_corner_3;
		}
		else
			return body_corner_4; // right -> down or up -> left
	}

	void updateGridFromTail()
	{
		// update the grid based on the tail position before it moves
		int row = Mathf.RoundToInt(tail.position.y-transform.parent.position.y);
		int col = Mathf.RoundToInt(tail.position.x-transform.parent.position.x);
		gameScript.updateGrid(row, col, GameScript.EMPTY);
	}
}