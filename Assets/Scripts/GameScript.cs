using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class GameScript : MonoBehaviour {
	
	public GameObject background;
	public GameObject frame;
	public GameObject apple;
	public GameObject snake;
	public GameObject coin;
	public GameObject bonus;
	public GameObject bonus_wheel;
	public GameObject teleporter_red;
	public GameObject teleporter_green;
	public GameObject teleporter_blue;

	public const bool REAL_DEVICE = false;

	// level values
	public const int EMPTY = 0;
	public const int FRAME = 1;
	public const int SNAKE = 2;
	public const int TELEPORTER = 3;

	// direction values
	public const int LEFT = 4;
	public const int RIGHT = 2;
	public const int UP = 1;
	public const int DOWN = 3;

	// game state keys
	private const string LEVEL = "LEVEL";
	private const string SCORE = "SCORE";
	private const string SPEED = "SPEED";
	private const string COINS = "COINS";
	private const string LIVES = "LIVES";

	// game logic
	private int[,] grid; 
	private ArrayList empty_spaces;
	private const int rows = 13;
	private const int cols = 25;
	private int level;
	private int direction;
	private bool userWin;
	private bool gameOver;
	private int currentScore;
	private float currentSpeed;
	private int currentCoins;
	private int currentLives;
	private int visibleScore;
	private Transform bk_parent;

	// HUD variables
	private float hudWidth;
	private float hudX;
	private float hudHeight;
	private float hudY;
	public Texture2D coin_sprite;
	public Texture2D life_sprite;
	
	// pause/play variables
	private bool paused;

	// GUI skins
	private GUISkin skin_normal;
	private GUISkin skin_pause;
	private GUISkin skin_play;

	public void updateGrid(int row, int col, int value)
	{
		if (grid[row,col] != TELEPORTER)
		{
			// only update grid if we are not dealing with a teleporter 

			if (value == SNAKE)
			{
				// update the snake head position in the grid
				grid[row, col] = SNAKE;

				// remove the empty space as it is no longer empty
				empty_spaces.Remove(new Vector2(row, col));
			}
			else
			{
				// update the grid with a new empty position as the tail has moved
				grid[row, col] = EMPTY;

				// store the empty space as a vector (row,col) since the tail moved
				empty_spaces.Add(new Vector2(row, col));
			}
		}
	}

	public void moveApple()
	{
		// get a random empty space from the array 0 - count
		Vector2 space = getRandomEmptySpace();

		// set the apple to the random position
		apple.transform.position = new Vector3((space.y+apple.transform.parent.position.x), (space.x+apple.transform.parent.position.y), apple.transform.parent.position.z);
	}

	public bool isValidMove(int row, int col)
	{
		// returns true if the given row and col in the grid is empty or a teleporter
		if (grid[row,col] == EMPTY || grid[row,col] == TELEPORTER)
			return true;
		return false;
	}

	public void setGameOver(bool v)
	{
		gameOver = v;
	}

	public bool isGameOver()
	{
		return gameOver;
	}

	// displays the coin in the level
	public void displayCoin()
	{
		if (coin.activeSelf == false)
		{
			// set the coin to active and reset its values
			coin.SetActive(true);
			coin.GetComponent<CoinScript>().reset();
			
			// get a random empty space from the array 0 - count
			Vector2 space = getRandomEmptySpace();
			
			// set the coin to the random position
			coin.transform.position = new Vector3((space.y+coin.transform.parent.position.x), (space.x+coin.transform.parent.position.y), coin.transform.parent.position.z);
		}
	}

	// displays the bonus pick up in the level
	public void displayBonusPickUp()
	{
		if (bonus.activeSelf == false)
		{
			// set the bonus to active and reset its values
			bonus.SetActive(true);
			bonus.GetComponent<BonusScript>().reset();
			
			// get a random empty space from the array 0 - count
			Vector2 space = getRandomEmptySpace();
			
			// set the bonus to the random position
			bonus.transform.position = new Vector3((space.y+bonus.transform.parent.position.x), (space.x+bonus.transform.parent.position.y), bonus.transform.parent.position.z);
		}
	}

	// return the rotation based on the given direction
	public Vector3 getRotation(int direction)
	{
		int z = 0;
		if (direction == GameScript.LEFT)
			z = 0;
		else if (direction == GameScript.RIGHT)
			z = 180;
		else if (direction == GameScript.UP)
			z = 270;
		else
			z = 90;

		return new Vector3(0,0,z);
	}

	// animates the score changes displayed on the HUD using iTween's ValueTo
	public void animateHUDScore() 
	{	
		iTween.ValueTo (gameObject, iTween.Hash( "from", visibleScore,  "to" , snake.GetComponent<SnakeScript>().getScore(), "onupdate" , "ChangeVisibleScore","time" , 0.5 ));
	}

	public bool isCoinLevel()
	{
		if (level > 2)
			return true;
		return false;
	}

	public bool isPickUpLevel()
	{
		if (level > 4)
			return true;
		return false;
	}

	public void displayBonusWheel()
	{
		// freeze the snake
		snake.GetComponent<SnakeScript>().setBonusWheelShowing(true);
		
		// freeze any coin
		coin.GetComponent<CoinScript>().setFreeze(true);

		// remove the background as it slows the wheel rotation
		bk_parent.gameObject.SetActive(false);

		// display the wheel
		bonus_wheel.SetActive(true);
	}

	public void hideBonusWheel() 
	{
		bonus_wheel.SetActive(false);

		// display the background
		bk_parent.gameObject.SetActive(true);

		Invoke("resume", 2);
	}




	// Use this for initialization
	void Start () 
	{
		// load the current game state
		loadGame();

		// create the game grid
		grid = new int[rows,cols];
		empty_spaces = new ArrayList();

		gameOver = false;
		userWin = false;
		paused = false;

		// skins
		skin_normal = (GUISkin)Resources.Load("Skins/skin_normal");
		skin_pause = (GUISkin)Resources.Load("Skins/skin_pause");
		skin_play = (GUISkin)Resources.Load("Skins/skin_play");
		
		// HUD values
		hudWidth = Screen.width/4;
		hudX = hudWidth/2;
		hudY = GameObject.Find("Middleground").transform.position.y+rows;
		hudHeight = Screen.height/rows;

		// create the level
		createLevel();

		if (isCoinLevel())
		{
			// create the coin(s)
			createCoins();
		}

		if (isPickUpLevel())
		{
			// create the bonus pick ups
			createPickUps();
		}
		
		// create the snake
		snake = Instantiate(snake) as GameObject;
		snake.GetComponent<SnakeScript>().setGameScript(this);
		snake.name = "Snake";
		setUpSnake();
		
		// create the apple
		apple = Instantiate(apple) as GameObject;
		apple.transform.parent = GameObject.Find("Foreground").transform;
		apple.name = "Apple";
		moveApple();
	}

	// Update is called once per frame
	void Update () 
	{
		if (isGameOver())
		{
			// game is over so check whether the user won or lost
			if (snake.GetComponent<SnakeScript>().isLevelPassed())
			{
				// user has passed this level
				userWin = true;
			}
			else
			{
				// user failed this level
				userWin = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// user pressed the go back button so load the menu
			Application.LoadLevel("menu");
		}
	}   

	void OnGUI ()
	{
		// draws the HUD
		drawHUD();

		// draws the pause/play menu
		drawPause();

		// draw the menu
		drawMenu();
	}



	// loads the level from a file
	void createLevel()
	{
		// the level will be a child of the middle ground
		Transform parent = GameObject.Find("Middleground").transform;
		float x = (float)parent.position.x;
		float y = (float)parent.position.y;
		float z = (float)parent.position.z;

		// the backround or empty objects will be a child of the background
		bk_parent = GameObject.Find("Background").transform;

		// read all of the lines into an array
		TextAsset level_file = (TextAsset)Resources.Load("Levels/level_"+level);
		string[] lines = level_file.text.Split("\n"[0]);
	
		// store the teleporters while we find its pair
		Hashtable teleporters = new Hashtable();                 
		GameObject current_teleporter = null;

		int current_row = 0;
		int val = 0;
		foreach(string l in lines)
		{
			// split the line by comma to get each grid component 
			string[] line = l.Split(',');
			int current_col = 0;
			foreach(string c in line)
			{
				val = int.Parse(c);
				if (val == FRAME)
				{
					// add a frame 
					frame = Instantiate(frame) as GameObject;
					frame.transform.parent = parent;
					frame.transform.position =  new Vector3(current_col + x, current_row + y, z);
					frame.name = "Frame";
					grid[current_row, current_col] = FRAME;
				}
				else if (val == EMPTY)
				{
					// add a background element
					background = Instantiate(background) as GameObject;
					background.transform.parent = bk_parent;
					background.transform.position =  new Vector3(current_col + bk_parent.position.x, current_row + bk_parent.position.y, bk_parent.position.z);
					background.name = "Background";
					background.gameObject.GetComponent<BackgroundScript>().setSprite();

					// store the empty space as a vector (row,col)
					empty_spaces.Add(new Vector2(current_row, current_col));
					grid[current_row, current_col] = EMPTY;
				}
				else
				{
					// teleporters

					// get the id and direction from the number - XXX color, id (color+id), direction
					int id = int.Parse(val.ToString().Substring(0,2));
					int dir = int.Parse(val.ToString().Substring(2,1));

					if (val >= 311 && val <= 394)
					{
						// red
						current_teleporter = Instantiate(teleporter_red) as GameObject;
					}		
					else if (val >= 411 && val <= 494)
					{
						// green
						current_teleporter = Instantiate(teleporter_green) as GameObject;
					}
					else
					{
						// blue
						current_teleporter = Instantiate(teleporter_blue) as GameObject;
					}
					
					// set some common properties
					current_teleporter.transform.parent = parent;
					current_teleporter.transform.position =  new Vector3(current_col + x, current_row + y, z);
					current_teleporter.GetComponent<TeleporterScript>().setRowAndCol(current_row, current_col);
					current_teleporter.name = "Teleporter";
					grid[current_row, current_col] = TELEPORTER;
					current_teleporter.transform.rotation = Quaternion.Euler(getRotation(dir));
					current_teleporter.GetComponent<TeleporterScript>().setDirection(dir);

					// determine whether we have found a pairing teleporter
					if (!teleporters.Contains(id))
					{
						// new teleporter
						teleporters[id] = current_teleporter;
					}
					else
					{
						// found the second teleporter of the pair
						GameObject pair = (GameObject)teleporters[id];

						// make these reference each other
						pair.GetComponent<TeleporterScript>().setPair(current_teleporter);
						current_teleporter.GetComponent<TeleporterScript>().setPair(pair);

						// remove the teleporter from the table
						teleporters.Remove(id);
					}
				}

				current_col++;
			}
			current_row++;
		}
	}

	// sets up the snake at a starting position
	void setUpSnake()
	{
		// choose a random direction
		direction = Random.Range(1,5);

		// find a valid space based on the direction
		bool valid = false;
		Vector2 space = getRandomEmptySpace();
		int tail_index = 0;
		int valid_move_index = 0;
		while (!valid)
		{
			// need to check the space to see if there is room for the tail 
			// and whether the user has at least 1 move in the direction before setting the snake to it
			if (direction == LEFT)
			{
				// - the column on the right needs to be empty too for the tail
				// - the column on the left needs to be empty 
				tail_index = empty_spaces.IndexOf(new Vector2(space.x, space.y+1));
				valid_move_index = empty_spaces.IndexOf(new Vector2(space.x, space.y-1));
			}
			else if (direction == RIGHT)
			{
				// - the column on the left needs to be empty too for the tail
				// - the column on the right needs to be empty 
				tail_index = empty_spaces.IndexOf(new Vector2(space.x, space.y-1));
				valid_move_index = empty_spaces.IndexOf(new Vector2(space.x, space.y+1));
			}
			else if (direction == UP)
			{
				// - the row down needs to be empty too for the tail
				// - the row above needs to be empty 
				tail_index = empty_spaces.IndexOf(new Vector2(space.x-1, space.y));
				valid_move_index = empty_spaces.IndexOf(new Vector2(space.x+1, space.y));
			}
			else
			{
				// - the row above needs to be empty too for the tail
				// - the row down needs to be empty 
				tail_index = empty_spaces.IndexOf(new Vector2(space.x+1, space.y));
				valid_move_index = empty_spaces.IndexOf(new Vector2(space.x-1, space.y));
			}

			// ensure that both spaces are available
			if (tail_index != -1 && valid_move_index != -1)
				valid = true;
			
			if (!valid)
			{
				// get a new random empty space from the array 0 - count
				space = getRandomEmptySpace();
			}
		}

		// found a valid tail space
		Vector2 tail_space = (Vector2)empty_spaces[tail_index];
		
		// remove the valid spaces from empty spaces
		empty_spaces.Remove(space);
		empty_spaces.Remove(tail_space);

		// update the grid
		updateGrid((int)space.x, (int)space.y, GameScript.SNAKE);
		updateGrid((int)tail_space.x, (int)tail_space.y, GameScript.SNAKE);

		// set the snake head and tail to the spaces
		snake.GetComponent<SnakeScript>().setHeadStartingPosition((int)space.x, (int)space.y, direction);
		snake.GetComponent<SnakeScript>().setTailStartingPosition((int)tail_space.x, (int)tail_space.y);

		// set the values stored in the game state
		snake.GetComponent<SnakeScript>().setScore(currentScore);
		snake.GetComponent<SnakeScript>().setSpeed(currentSpeed);
		snake.GetComponent<SnakeScript>().setCoinsCollected(currentCoins);
		snake.GetComponent<SnakeScript>().setLives(currentLives);

		// randomly choose how many coins this level will have (0-3)
		snake.GetComponent<SnakeScript>().setNumberOfCoins(Random.Range(0,4));

		// randomly choose how many pick ups this level will have (0-4)
		snake.GetComponent<SnakeScript>().setNumberOfBonusPickUps(Random.Range(0,5));
	}

	// returns a random empty space from the grid
	Vector2 getRandomEmptySpace()
	{
		// ensure the space is actually empty
		Vector2 space = (Vector2)empty_spaces[Random.Range(0, empty_spaces.Count)];
		Transform parent = GameObject.Find("Foreground").transform;
		Vector3 pos;
		bool valid = false;
		while (!valid)
		{
			pos = new Vector3((space.y+parent.position.x), (space.x+parent.position.y), parent.position.z);
			valid = true;

			// check if an apple is at this position
			if (apple.transform.position == pos)
				valid = false;

			// check if a coin is at this position
			if (isCoinLevel() && coin.transform.position == pos)
				valid = false;

			// check if a bonus pick up is at this position
			if (isPickUpLevel() && bonus.transform.position == pos)
				valid = false;

			if (!valid){
				space = (Vector2)empty_spaces[Random.Range(0, empty_spaces.Count)]; print ("s");}
		}
		
		return space;
	}

	// create the coins for the level
	void createCoins()
	{
		coin = Instantiate(coin) as GameObject;
		coin.transform.parent = GameObject.Find("Foreground").transform;
		coin.name = "Coin";
		coin.SetActive(false);
		this.displayCoin();
	}

	// create the bonus pick ups for the level
	void createPickUps()
	{
		bonus = Instantiate(bonus) as GameObject;
		bonus.transform.parent = GameObject.Find("Foreground").transform;
		bonus.name = "Bonus";
		bonus.SetActive(false);
		this.displayBonusPickUp();

		bonus_wheel = Instantiate(bonus_wheel) as GameObject;
		bonus_wheel.transform.parent = GameObject.Find("Foreground").transform;
		bonus_wheel.name = "Bonus Wheel";
		bonus_wheel.SetActive(false);
	}




	// loads the game state as defined by the saved state
	void loadGame()
	{
		if (!PlayerPrefs.HasKey(LEVEL))
		{
			// new game save the default game state
			setUpInitialGameState();
		}

		// get the current game state
		level = PlayerPrefs.GetInt(LEVEL);
		currentScore = PlayerPrefs.GetInt(SCORE);
		currentSpeed = PlayerPrefs.GetFloat(SPEED);
		currentCoins = PlayerPrefs.GetInt(COINS);
		currentLives = PlayerPrefs.GetInt(LIVES);

		visibleScore = currentScore;

		level = 16;
	}
	
	// saves the game state 
	void saveGame()
	{
		// store the game state
		if (userWin)
		{
			PlayerPrefs.SetInt(LEVEL, level);
			PlayerPrefs.SetInt(SCORE, snake.GetComponent<SnakeScript>().getScore());
			PlayerPrefs.SetFloat(SPEED, snake.GetComponent<SnakeScript>().getSpeed());
			PlayerPrefs.SetInt(COINS, snake.GetComponent<SnakeScript>().getCoinsCollected());
			PlayerPrefs.SetInt(LIVES, snake.GetComponent<SnakeScript>().getLives());
		}
		else
		{
			setUpInitialGameState();
		}
	}

	// sets up a default game with the default values
	void setUpInitialGameState()
	{
		PlayerPrefs.SetInt(LEVEL, 1);
		PlayerPrefs.SetInt(SCORE, 0);
		PlayerPrefs.SetFloat(SPEED, 0.5f);
		PlayerPrefs.SetInt(COINS, 0);
		PlayerPrefs.SetInt(LIVES, 0);
	}

	void resume()
	{	
		// unfreeze the snake
		snake.GetComponent<SnakeScript>().setBonusWheelShowing(false);
		
		// unfreeze any coin
		coin.GetComponent<CoinScript>().setFreeze(false);
	}



	
	// draws the HUD
	void drawHUD()
	{
		GUI.skin = skin_normal;

		// draw the level box in the centre left
		GUI.Box(new Rect(hudX,hudY,hudWidth,hudHeight), "Level "+level);

		// draw the bonus pick up box in the centre
		GUI.Box(new Rect(hudX+hudWidth,hudY,hudWidth,hudHeight), "");

		// add the coins if any are collected
		if (snake != null)
		{
			float w = hudWidth/6;
			float x = hudX+hudWidth;

			// display the bonus item if there is one
			if (snake.GetComponent<SnakeScript>().hasSnakeGotBonusItem())
			{
				GUI.Box(new Rect(x,hudY,w,hudHeight), coin_sprite);
				x += w;
				GUI.Box(new Rect(x,hudY,w,hudHeight), snake.GetComponent<SnakeScript>().getBonusSeconds()+"");
			}

			// display the lives
			x += w;
			int num = snake.GetComponent<SnakeScript>().getLives();
			GUI.Box(new Rect(x,hudY,w,hudHeight), life_sprite);
			x += w;
			GUI.Box(new Rect(x,hudY,w,hudHeight), "x"+num);
			
			// display the coins
			x += w;
			num = snake.GetComponent<SnakeScript>().getCoinsCollected();
			GUI.Box(new Rect(x,hudY,w,hudHeight), coin_sprite);
			x += w;
			GUI.Box(new Rect(x,hudY,w,hudHeight), "x"+num);
		}

		// draw the score box in the centre right
		GUI.Box(new Rect(hudX+(2*hudWidth),hudY,hudWidth,hudHeight), "Score "+visibleScore);
	}

	// draws the pause/play menu
	void drawPause()
	{
		if (!isGameOver())
		{
			// draw a button at the top right corner
			float w = hudWidth/3;
			float x = Screen.width-w;
			Rect rect = new Rect(x,hudY,hudHeight,hudHeight);
			if (!paused)
			{
				// game is in play mode so display pause sprite
				GUI.skin = skin_pause;
				if (GUI.Button(rect,""))
				{
					paused = true;
					Time.timeScale = 0f;
				}
			}
			else
			{
				GUI.skin = skin_play;
				if (GUI.Button(rect,""))
				{
					paused = false;
					Time.timeScale = 1.0f;
				}
			}
		}
	}

	// draws the menu once the level has finished
	void drawMenu()
	{
		if (isGameOver())
		{
			GUI.skin = skin_normal;
			if (userWin)
			{
				// display the scoreboard
				float x = Screen.width/2;
				float y = Screen.height/2;
				float w = hudWidth/2;
				float h = hudHeight;
				
				if (GUI.Button(new Rect(x,y,w,h), "Next"))
				{
					// increment the level
					level++;

					// increase the speed every level
					currentSpeed -= 0.01f;

					// save the game state
					saveGame();

					// Load the level
					Application.LoadLevel("level");
				}
			}
			else
			{
				// give the user options to retry or go to the main menu
				float x = Screen.width/2;
				float y = Screen.height/2;
				float w = hudWidth/2;
				float h = hudHeight;
			
				if (GUI.Button(new Rect(x-w-(w/4),y,w,h), "Retry"))
				{
					// save the game state
					saveGame();

					// Reload the level
					Application.LoadLevel("level");
				}
				
				if (GUI.Button(new Rect(x+(w/4),y,w,h), "Main Menu"))
				{
					// save the game state
					saveGame();

					// Reload the level
					Application.LoadLevel("menu");
				}
			}
		}
	}

	// Changes the currently visible score on the HUD. Called every time iTween changes my
	// visibleScore variable
	void ChangeVisibleScore (int i) {
		visibleScore = i;
	}
}