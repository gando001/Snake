using UnityEngine;
using System.Collections;

public class SnakeScript : MonoBehaviour {

	// initial speed values
	public Transform tail;
	public Transform body;
	public Transform scoreText;
	public Sprite body_corner;
	public Sprite body_normal;

	// snake logic
	private float speed;
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
	private int tail_direction;

	// coins
	private int number_of_coins;
	private int coin_body_index;
	private int coins_collected;

	// bonus pick ups
	private int number_of_pick_ups;
	private int bonus_body_index;
	private int bonus_collected;
	private bool bonusWheelShowing;
	private bool hasBonusItem;
	private int bonusSeconds;
	private float originalSpeed;
	private bool bonusSlow;
	private bool bonusSpeed;
	private bool bonusDoublePoints;
	private bool bonusRemoveApple;
	private bool bonusRotate;
	private const int TTL = 90;

	// lives
	private int lives;
	
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

	public bool isLevelPassed(){
		return levelPassed;
	}
	
	public bool isEaten(){
		return eaten;
	}

	public int getScore(){
		return score;
	}

	public void setScore(int s){
		score = s;
	}

	public float getSpeed(){
		if (bonusSlow || bonusSpeed)
			return originalSpeed;
		else
			return speed;
	}

	public void setSpeed(float s){
		speed = s;
	}

	// sets how many times to display a coin for this level
	public void setNumberOfCoins(int v){
		number_of_coins = v;
	}

	// sets the number of coins the user has already collected
	public void setCoinsCollected(int c){
		coins_collected = c;
	}

	public void coinCollected(){
		coins_collected++;
	}

	public int getCoinsCollected(){
		return coins_collected;
	}

	public void setNumberOfBonusPickUps(int v){
		number_of_pick_ups = v;
	}

	public void setBonusWheelShowing(bool v){
		bonusWheelShowing = v;
	}

	public bool isBonusWheelShowing(){
		return bonusWheelShowing;
	}

	// sets the number of lives the user has
	public void setLives(int v){
		lives = v;
	}

	public void addLife(){
		lives++;
	}

	public void loseLife(){
		lives--;
	}

	public int getLives(){
		return lives;
	}

	// halves the length of the snake
	public void halfSnake()
	{
		if (body_parts > 0)
		{
			// set half of the snake bodies to inactive
			// by changing their z position
			int length = body_parts/2;
			GameObject current;
			for (int i=body_parts; i>length; i--)
			{
				current = GameObject.Find("body"+i);
				Vector3 cur = current.transform.position;
				current.transform.position = new Vector3(cur.x, cur.y, 15);

				// set the tail to this body's position
				updateGridFromTail();
				tail.position = cur;
				tail_direction = current.GetComponent<BodyScript>().getDirection();
				tail.transform.rotation = Quaternion.Euler(gameScript.getRotation(tail_direction));
			}

			body_parts = length;
		}
	}

	// slows the snake down
	public void slowmoSnake()
	{
		originalSpeed = speed;
		setSpeed(Mathf.Min(0.28f, originalSpeed+0.05f));

		bonusSlow = true;
		hasBonusItem = true;
		bonusSeconds = TTL;

		InvokeRepeating ("BonusCountdown", 1.0f, 1.0f);
	}

	// speeds the snake up
	public void speedSnake()
	{
		originalSpeed = speed;
		setSpeed(Mathf.Max(0.0f, originalSpeed-0.05f));

		bonusSpeed = true;
		hasBonusItem = true;
		bonusSeconds = TTL;

		InvokeRepeating ("BonusCountdown", 1.0f, 1.0f);
	}

	public void setDoublePoints(){
		bonusDoublePoints = true;
		hasBonusItem = true;
		bonusSeconds = TTL;

		InvokeRepeating ("BonusCountdown", 1.0f, 1.0f);
	}

	// hides the apple for the remove apple bonus item
	public void hideApple(){
		gameScript.apple.SetActive(false);
		bonusRemoveApple = true;
		hasBonusItem = true;
		bonusSeconds = TTL;

		InvokeRepeating ("BonusCountdown", 1.0f, 1.0f);
	}

	// rotates the world
	public void setRotate(){
		Camera.main.GetComponent<CameraScript>().setUpsideDown();
		bonusRotate = true;
		hasBonusItem = true;
		bonusSeconds = TTL;

		InvokeRepeating ("BonusCountdown", 1.0f, 1.0f);
	}

	// removes any applied timed bonus items
	public void removeAppliedBonusItems()
	{
		if (bonusSlow || bonusSpeed)
			setSpeed(originalSpeed);
		else if (bonusRemoveApple)
		{
			gameScript.moveApple();
			gameScript.apple.SetActive(true);
		}
		else if (bonusRotate)
			Camera.main.GetComponent<CameraScript>().setNormal();

		hasBonusItem = false;
		bonusSlow = false;
		bonusSpeed = false;
		bonusDoublePoints = false;
		bonusRemoveApple = false;
		bonusRotate = false;
	}

	public bool hasSnakeGotBonusItem(){
		return hasBonusItem;
	}

	public int getBonusSeconds(){
		return bonusSeconds;
	}
	
	


	// Use this for initialization
	void Start () 
	{
		score = 0;
		body_limit = 30;
		body_parts = 0;
		lastUpdate = 0;
		bodies = new ArrayList();
		getUserInput = true;

		// bonus
		bonusWheelShowing = false;
		hasBonusItem = false;
		bonusSlow = false;
		bonusSpeed = false;
		bonusDoublePoints = false;
		bonusRemoveApple = false;
		bonusRotate = false;

		// set up the coins if required
		if (gameScript.isCoinLevel() && number_of_coins > 0)
		{
			coins_collected = 0;
			setCoinBodyIndex();
		}

		// set up the bonus wheel if required
		if (gameScript.isPickUpLevel() && number_of_pick_ups > 0)
		{
			bonus_collected = 0;
			setBonusBodyIndex();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// user input
		if (GameScript.REAL_DEVICE)
		{
			if (Input.touchCount > 0 && !isBonusWheelShowing()) 
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

					if (delta == Mathf.Abs(dx))
					{
						// user chose a horizontal movement
						if (!right && dx < 0)
						{
							// left
							setLeft();
						}
						else if (!left && dx > 0)
						{
							// right
							setRight();
						}
					}
					else
					{
						// user chose a vertical movement
						if (!down && dy > 0)
						{
							// up
							setUp();
						}
						else if (!up && dy < 0)
						{
							// down	
							setDown();
						}
					}
				}
			}
		}
		else
		{
			if (!isBonusWheelShowing())
			{
				// Use for testing in Unity not on device
				// get the keyboard values and calculate the movement
				float inputX = Input.GetAxis ("Horizontal");
				float inputY = Input.GetAxis ("Vertical");
				
				// determine the direction the user wants to go
				// need to remove the going left then able to go right and top -> bottom
				if (!right && inputX < 0)
				{
					// left
					setLeft();	
				}
				else if (!left && inputX > 0)
				{
					// right
					setRight();
				}
				else if (!down && inputY > 0)
				{
					// up
					setUp();
				}
				else if (!up && inputY < 0)
				{
					// down	
					setDown();
				}
			}
		}
	}

	void FixedUpdate(){
	
		// FixedUpdate() is called at every fixed framerate frame. 
		// You should use this method over Update() when dealing with physics ("RigidBody" and forces).

		// update when it is time to and the game isn't over
		if (Time.time - lastUpdate >= speed && !gameScript.isGameOver() && !isBonusWheelShowing())
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

			// only move the snake if the new cell is empty or a teleporter
			if (gameScript.isValidMove(row,col))
			{
				if (isEaten())
				{
					// create the new body at the latest bodies position
					createBody();

					eaten = false;
				}
				else 
				{
					// only move the tail if we haven't eaten an apple
					updateGridFromTail();
					if (body_parts == 0)
					{	
						// set the tail to the heads position - no bodies so tail follows head
						tail.position = transform.position;
					}
					else
					{
						// make tail follow the latest body part
						tail.position = GameObject.Find("body"+body_parts).transform.position;
					}
				}
	
				if (body_parts > 0)
				{
					// set the position of each body to the body in front of it (has a lower number)
					for (int i=body_parts; i>1; i--)
					{
						GameObject.Find("body"+i).transform.position = GameObject.Find("body"+(i-1)).transform.position; 
					}

					// set the first body added to the heads position
					GameObject.Find("body1").transform.position = transform.position;
				}
	
				updateHead();

				updateSprites();
				
				lastUpdate = Time.time;
				getUserInput = true;
			}
			else
			{
				// snake tried to move to a non-empty cell
				endGame();
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
			incrementScore(true, GameObject.Find("Apple").GetComponent<AppleScript>().getScoreValue());

			// increment the snake
			incrementSnake();
		}
		else if (otherCollider.gameObject.name == "Coin")
		{
			incrementScore(false, GameObject.Find("Coin").GetComponent<CoinScript>().getScoreValue());

			coinCollected();

			// remove the coin
			gameScript.coin.SetActive(false);
		}
		else if (otherCollider.gameObject.name == "Bonus")
		{	
			bonus_collected++;

			// remove the bonus pick up
			gameScript.bonus.SetActive(false);

			// show the bonus wheel
			gameScript.displayBonusWheel();
		}
		else if (otherCollider.gameObject.name == "Teleporter")
		{
			if (direction == otherCollider.gameObject.GetComponent<TeleporterScript>().getFacingDirection())
			{
				// the snake is entering the correct face of the teleporter

				// head is on the teleporter so update the grid to set it back to a teleporter
				gameScript.updateGrid(row, col, GameScript.TELEPORTER);

				// get this teleporters matching teleporter so we can get its exit cell
				Vector2 exit = otherCollider.gameObject.GetComponent<TeleporterScript>().getPair().GetComponent<TeleporterScript>().getExit();
				row = (int)exit.x;
				col = (int)exit.y;

				updateHead();
			}
			else if (!gameScript.isGameOver())
			{
				// the snake hit the teleporter from a side
				endGame();
			}
		}
	}

	void updateHead()
	{
		// update the head position
		transform.position = new Vector3(col+transform.parent.position.x, row+transform.parent.position.y, transform.position.z);
		
		// update the grid
		gameScript.updateGrid(row, col, GameScript.SNAKE);
	}
	
	void createBody()
	{
		string name = "body"+body_parts;
		if (GameObject.Find(name) != null)
		{
			// this body has already been created but set to inactive due to half snake bonus item
			body = GameObject.Find(name).transform;
		}
		else
		{
			// create a new body prefab
			body = Instantiate(body) as Transform;
			body.parent = parent;
			body.name = name;
			bodies.Add(body);
		}
		
		// add the body to the end of the snake before the tail
		if (body_parts > 1)
		{
			// add the body after the last body
			GameObject prev = GameObject.Find("body"+(body_parts-1));
			body.position = prev.transform.position;
			body.rotation = prev.transform.rotation;
			
			// add corner sprite if required
			int dir = prev.GetComponent<BodyScript>().getDirection();
			if (dir != tail_direction)
			{
				// since the tail hasn't been moved and the rest of the snake has and
				// the sprites have been updated then prev has its new direction 
				// we set this body to the tail direction to force the sprite to be
				// drawn as a corner
				body.GetComponent<BodyScript>().setDirection(tail_direction);
			}
			else
				body.GetComponent<BodyScript>().setDirection(dir);
		}
		else
		{
			// first body so it uses the head properties
			body.position = transform.position;
			body.GetComponent<BodyScript>().setDirection(tail_direction);
		}
	}

	void incrementSnake()
	{
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
			
			// check if its time to show the coin
			if (body_parts == coin_body_index)
			{
				// display the coin
				gameScript.displayCoin();
				
				number_of_coins--;
				if (number_of_coins > 0)
					setCoinBodyIndex();
			}

			// check if its time to show the pick up
			if (body_parts == bonus_body_index)
			{
				// display the pick up
				gameScript.displayBonusPickUp();

				number_of_pick_ups--;
				if (number_of_pick_ups > 0)
					setBonusBodyIndex();
			}
		}
	}

	void updateSprites()
	{
		// rotates the sprites as required
		if (body_parts == 0)
		{
			// tail follows head
			tail_direction = direction;
			tail.transform.rotation = Quaternion.Euler(gameScript.getRotation(tail_direction));
		}
		else
		{
			// tail follows the latest body part
			GameObject current = GameObject.Find("body"+body_parts);
			tail_direction = current.GetComponent<BodyScript>().getDirection();
			tail.transform.rotation = Quaternion.Euler(gameScript.getRotation(tail_direction));
			
			// update the body parts
			GameObject next = null;
			int currentDirection;
			int nextDirection;
			int rotation;
			for (int i=body_parts; i>0; i--)
			{
				// determine if the current body needs to change its default sprite
				current = GameObject.Find("body"+i);
				currentDirection = current.GetComponent<BodyScript>().getDirection();
				
				if (i == 1)
				{
					// reached the first body part so it follows the head
					next = this.gameObject;
					nextDirection = direction;
				}
				else
				{
					next =  GameObject.Find("body"+(i-1));
					nextDirection = next.GetComponent<BodyScript>().getDirection();
				}
				
				if (currentDirection != nextDirection)
				{
					// this body needs to change direction so it is on a corner => change its sprite
					rotation = getCorner(currentDirection, nextDirection);
					current.transform.rotation = Quaternion.Euler(new Vector3(0,0,rotation));
					current.gameObject.GetComponent<SpriteRenderer>().sprite = body_corner;
					current.GetComponent<BodyScript>().setDirection(nextDirection);
				}
				else
				{
					// the directions are the same so leave as a normal body but change its rotation
					current.gameObject.GetComponent<SpriteRenderer>().sprite = body_normal;
					current.transform.rotation = Quaternion.Euler(gameScript.getRotation(currentDirection));
				}
			}
		}
		
		// update the heads rotation
		transform.rotation = Quaternion.Euler(gameScript.getRotation(direction));
	}

	// ends the game
	void endGame()
	{
		gameScript.setGameOver(true);
		levelPassed = false;
		
		// vibrate the device
		Handheld.Vibrate ();
	}

	// randomly choose when to display the coins based on the snake body length
	void setCoinBodyIndex()
	{
		coin_body_index = Random.Range(Mathf.Max(10,body_parts),body_limit+1);
	}

	// randomly choose when to display the bonus based on the snake body length
	void setBonusBodyIndex()
	{
		bonus_body_index = Random.Range(Mathf.Max(10,body_parts),body_limit+1);
	}

	int getCorner(int currentDirection, int newDirection)
	{
		// returns the rotation value of the z axis required for a body part on the corner
		if ((currentDirection == GameScript.RIGHT && newDirection == GameScript.UP) || (currentDirection == GameScript.DOWN && newDirection == GameScript.LEFT))
		{
			return 0;
		}
		else if ((currentDirection == GameScript.RIGHT && newDirection == GameScript.DOWN) || (currentDirection == GameScript.UP && newDirection == GameScript.LEFT))
		{
			return 90;
		}
		else if ((currentDirection == GameScript.LEFT && newDirection == GameScript.DOWN) || (currentDirection == GameScript.UP && newDirection == GameScript.RIGHT))
		{
			return 180;
		}
		else
			return 270;
	}

	void updateGridFromTail()
	{
		// update the grid based on the tail position before it moves
		int row = Mathf.RoundToInt(tail.position.y-transform.parent.position.y);
		int col = Mathf.RoundToInt(tail.position.x-transform.parent.position.x);
		gameScript.updateGrid(row, col, GameScript.EMPTY);
	}

	void incrementScore(bool isApple, int v)
	{
		// increment the score
		if (bonusDoublePoints)
			v = v*2;
		score += v;

		// animate the HUD score
		gameScript.animateHUDScore();

		// create the score object where the apple was
		scoreText = Instantiate(scoreText) as Transform;
		scoreText.parent = parent;
		scoreText.name = "scoreText";
		scoreText.guiText.text = "+"+v;
		scoreText.position = Camera.main.WorldToViewportPoint(new Vector3(col+transform.parent.position.x, row+transform.parent.position.y, (float)parent.position.z));
		scoreText.gameObject.SetActive(true);
	}

	void setLeft()
	{
		left = true;
		right = false;	
		down = false;
		up = false;
		direction = GameScript.LEFT;
	}

	void setRight()
	{
		left = false;
		right = true;	
		down = false;
		up = false;
		direction = GameScript.RIGHT;
	}

	void setUp()
	{
		left = false;
		right = false;	
		down = false;
		up = true;
		direction = GameScript.UP;
	}

	void setDown()
	{
		left = false;
		right = false;	
		down = true;
		up = false;
		direction = GameScript.DOWN;
	}

	void BonusCountdown () 
	{
		if (--bonusSeconds == 0)
		{
			removeAppliedBonusItems();
			CancelInvoke ("Countdown");
		}
	}
}