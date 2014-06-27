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
	public GameObject menu;
	public GameObject foreground;
	public GameObject middleground;

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
	private Transform bk_parent;
	private bool bonus_wheel_pause;

	// HUD variables
	private int visibleScore;
	public GameObject bonus_item;
	public TextMesh hud_level;
	public TextMesh hud_lives;
	public TextMesh hud_coins;
	public TextMesh hud_bonus;
	public TextMesh hud_score;
	
	// pause/play variables
	private bool paused;

	// score board variables
	private bool startScoreBoardAnimation;
	private int scoreBoardScore;
	private int scoreBoardLives;
	private int scoreBoardCoins;
	public TextMesh menu_level_num;
	public TextMesh menu_score_num;
	public TextMesh menu_lives_num;
	public TextMesh menu_coins_num;
	public GameObject menu_button;

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

		if (gameOver)
			startScoreBoardAnimation = true;
	}

	public bool isGameOver(){
		return gameOver;
	}

	public void setPause(bool v){
		paused = v;

		if (isPaused())
		{
			Time.timeScale = 0f;
			
			if (bonus_wheel.activeSelf) {
				bonus_wheel.SetActive(false);
				bonus_wheel_pause = true;
			}
		}
		else
		{
			Time.timeScale = 1.0f;
			menu.SetActive(false);

			if (bonus_wheel_pause) {
				bonus_wheel.SetActive(true);
				bonus_wheel_pause = false;
			}
		}
	}
	
	public bool isPaused(){
		return paused;
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
		middleground.SetActive(false);
		foreground.SetActive(false);

		// display the wheel
		bonus_wheel.SetActive(true);

		// set a reference between the wheel and snake objects
		GameObject.Find("Wheel").GetComponent<WheelScript>().setSnake(snake);
	}

	public void hideBonusWheel() 
	{
		bonus_wheel.SetActive(false);

		// display the background
		bk_parent.gameObject.SetActive(true);
		middleground.SetActive(true);
		foreground.SetActive(true);

		// unfreeze the snake
		snake.GetComponent<SnakeScript>().setBonusWheelShowing(false);

		// do not move the snake until user gives in put
		snake.GetComponent<SnakeScript>().setStarted(false);
		
		// unfreeze any coin
		coin.GetComponent<CoinScript>().setFreeze(false);
	}

	public void setBonusSprite(Sprite s){
		bonus_item.GetComponent<SpriteRenderer>().sprite = s;
	}

	public void levelPassed()
	{
		// increment the level
		level++;
		
		// increase the speed every second level
		if (level % 2 == 0)
			currentSpeed -= 0.01f;

		// save the game state
		saveGame();
		
		// Load the level
		Application.LoadLevel("level");
	}

	public void levelRetry()
	{
		// set up the game state for saving
		userWin = true; 
		snake.GetComponent<SnakeScript>().loseLife();
		
		// save the game state
		saveGame();
		
		// Reload the level
		Application.LoadLevel("level");
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
		bonus_wheel_pause = false;
		setPause(false);

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

			// draw the menu
			// only display the menu if the bonus wheel is not showing; i.e: user is trying to use coins on the wheel
			if (!bonus_wheel.activeSelf)
			{
				// remove any bonus items
				snake.GetComponent<SnakeScript>().removeAppliedBonusItems();
				
				if (!userWin && snake.GetComponent<SnakeScript>().getCoinsCollected() > 0)
				{
					// user has lost the level but has coins for the bonus wheel
					displayBonusWheel();
					
					// notify the wheel that this is for a level retry - need this hack to delay the call
					StartCoroutine(resetWheel());
				}
				else
				{
					// user has either won the level, lost but has lives or lost the game
					// so draw the score board
					drawScoreBoard();
				}
			}
		}
		else if (isPaused())
		{
			// game paused so set the score board values since we are not animating the values
			scoreBoardScore = snake.GetComponent<SnakeScript>().getScore();
			scoreBoardLives = snake.GetComponent<SnakeScript>().getLives();
			scoreBoardCoins = snake.GetComponent<SnakeScript>().getCoinsCollected();
			
			// draw the score board
			drawScoreBoard();
		}

		// HUD
		hud_level.text = ""+level;
		hud_lives.text = ""+snake.GetComponent<SnakeScript>().getLives();
		hud_coins.text = ""+snake.GetComponent<SnakeScript>().getCoinsCollected();
		hud_score.text = visibleScore+"";
			
		// display the bonus item if there is one
		if (snake.GetComponent<SnakeScript>().hasSnakeGotBonusItem())
			hud_bonus.text = snake.GetComponent<SnakeScript>().getBonusSeconds()+"";
		else
		{
			setBonusSprite(null);
			hud_bonus.text = "";
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// user pressed the go back button so save the game state
			saveGame();

			// load the main menu
			Application.LoadLevel("menu");
		}
	}   

	// called when the home button is pressed
	void OnApplicationPause(bool paused) 
	{
		if (paused)
		{
			setPause(true);

			// save the game state
			saveGame();
		}
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

		// delays restoring the snake values from saved game state
		StartCoroutine(restoreSnakeValuesFromGameState());
	
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

			if (!valid)
				space = (Vector2)empty_spaces[Random.Range(0, empty_spaces.Count)];
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
	}

	// create the bonus pick ups for the level
	void createPickUps()
	{
		bonus = Instantiate(bonus) as GameObject;
		bonus.transform.parent = GameObject.Find("Foreground").transform;
		bonus.name = "Bonus";
		bonus.SetActive(false);

		bonus_wheel = Instantiate(bonus_wheel) as GameObject;
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
	}
	
	// saves the game state 
	public void saveGame()
	{
		// store the game state when the user passes a level or decides to leave a level before winning or losing
		if (userWin || !isGameOver())
		{
			PlayerPrefs.SetInt(LEVEL, level);
			PlayerPrefs.SetInt(SCORE, snake.GetComponent<SnakeScript>().getScore());
			PlayerPrefs.SetFloat(SPEED, currentSpeed);
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
		PlayerPrefs.SetFloat(SPEED, 0.28f);
		PlayerPrefs.SetInt(COINS, 0);
		PlayerPrefs.SetInt(LIVES, 0);
	}
	
	// draws a score board of the current level state
	void drawScoreBoard()
	{
		// show the banner ad
		Camera.main.GetComponent<GoogleMobileAdsScript>().ShowBanner();

		if (startScoreBoardAnimation && !isPaused())
		{
			// only animate once and if not paused
			animateScoreBoard();
			startScoreBoardAnimation = false;
		}

		menu.SetActive(true);
		menu_level_num.text = level+"";
		menu_score_num.text = scoreBoardScore+"";
		menu_lives_num.text = scoreBoardLives+"";
		menu_coins_num.text = scoreBoardCoins+"";

		if (isPaused())
		{
			// display the play button
			menu_button.GetComponent<MenuButtonAdvancedScript>().setState(MenuButtonAdvancedScript.PAUSED);
		}
		else
		{
			// game must be over here
			if (userWin)
			{
				// user wins the level
				menu_button.GetComponent<MenuButtonAdvancedScript>().setState(MenuButtonAdvancedScript.WON);
			}
			else
			{
				// user loses the level
				if (snake.GetComponent<SnakeScript>().getLives() > 0)
				{
					// user has lives to retry
					menu_button.GetComponent<MenuButtonAdvancedScript>().setState(MenuButtonAdvancedScript.RETRY);
				}
				else
				{
					// user loses the level
					menu_button.SetActive(false);
					Vector3 pos = GameObject.Find("Menu_button").transform.position;
					GameObject.Find("Menu_button").transform.position = new Vector3(0, pos.y, pos.z);
				}
			}
		}
	}

	// animates the component values of the score board
	void animateScoreBoard()
	{
		iTween.ValueTo (gameObject, iTween.Hash( "from", 0,  "to" , visibleScore+1000, "onupdate" , "animateScoreBoardScore","time" , 1 ));
		iTween.ValueTo (gameObject, iTween.Hash( "from", 0,  "to" , snake.GetComponent<SnakeScript>().getLives()+1000, "onupdate" , "animateScoreBoardLives","time" , 1, "delay", 1 ));
		iTween.ValueTo (gameObject, iTween.Hash( "from", 0,  "to" , snake.GetComponent<SnakeScript>().getCoinsCollected()+1000, "onupdate" , "animateScoreBoardCoins","time" , 1, "delay", 2 ));
	}

	// Changes the currently visible score on the HUD. Called every time iTween changes my
	// visibleScore variable
	void ChangeVisibleScore (int i) {
		visibleScore = i;
	}

	void animateScoreBoardScore (int i) {
		scoreBoardScore = i;
	}

	void animateScoreBoardLives (int i) {
		scoreBoardLives = i;
	}

	void animateScoreBoardCoins (int i) {
		scoreBoardCoins = i;
	}

	IEnumerator resetWheel() 
	{
		// resets the wheel after waiting a little while
		yield return new WaitForSeconds(0);
		GameObject.Find("Wheel").GetComponent<WheelScript>().setLevelSpin();
	}

	IEnumerator restoreSnakeValuesFromGameState() 
	{
		// restores the snake values after waiting a little while
		yield return new WaitForSeconds(0);

		// set the values stored in the game state
		snake.GetComponent<SnakeScript>().setScore(currentScore);
		snake.GetComponent<SnakeScript>().setSpeed(currentSpeed);
		snake.GetComponent<SnakeScript>().setCoinsCollected(currentCoins);
		snake.GetComponent<SnakeScript>().setLives(currentLives);
	}
}