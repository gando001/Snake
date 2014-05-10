﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class GameScript : MonoBehaviour {
	
	public GameObject background;
	public GameObject frame;
	public GameObject apple;
	public GameObject snake;
	public GameObject coin;

	// level values
	public const int EMPTY = 0;
	public const int FRAME = 1;
	public const int SNAKE = 2;

	// direction values
	public const int LEFT = 1;
	public const int RIGHT = 2;
	public const int UP = 3;
	public const int DOWN = 4;

	// game state keys
	private const string LEVEL = "LEVEL";
	private const string SCORE = "SCORE";
	private const string SPEED = "SPEED";

	// game logic
	private int[,] grid; 
	private ArrayList empty_spaces;
	private ArrayList frames;
	private const int rows = 13;
	private const int cols = 25;
	private int level;
	private int currentScore;
	private int direction;
	private bool userWin;
	private bool gameOver;
	private float currentSpeed;

	// HUD variables
	private float hudWidth;
	private float hudX;
	private float hudHeight;
	private float hudY;

	// pause/play variables
	private bool paused;

	// GUI skins
	private GUISkin skin_normal;
	private GUISkin skin_pause;
	private GUISkin skin_play;

	// coin variables
	public Texture2D coin_sprite;

	public void updateGrid(int row, int col, int value)
	{
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

	public void moveApple()
	{
		// get a random empty space from the array 0 - count
		Vector2 space = getRandomEmptySpace();

		// set the apple to the random position
		apple.transform.position = new Vector3((space.y+apple.transform.parent.position.x), (space.x+apple.transform.parent.position.y), apple.transform.parent.position.z);
	}

	public bool isGridEmpty(int row, int col)
	{
		if (grid[row,col] == EMPTY)
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




	// Use this for initialization
	void Start () 
	{
		// load the current game state
		loadGame();

		// create the game grid
		grid = new int[rows,cols];
		empty_spaces = new ArrayList();
		frames = new ArrayList();

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

		// create the background image
		createBackground();

		// create the level
		createLevel();

		// create the coin(s)
		createCoins();
		
		// create the snake
		snake = Instantiate(snake) as GameObject;
		snake.GetComponent<SnakeScript>().setGameScript(this);
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

		// read all of the lines into an array
		TextAsset level_file = (TextAsset)Resources.Load("Levels/level_"+level);
		string[] lines = level_file.text.Split("\n"[0]);
	
		int current_row = 0;
		foreach(string l in lines)
		{
			// split the line by comma to get each grid component 
			string[] line = l.Split(',');
			int current_col = 0;
			foreach(string c in line)
			{
				if (int.Parse(c) == FRAME)
				{
					// add a frame 
					frame = Instantiate(frame) as GameObject;
					frame.transform.parent = parent;
					frame.transform.localScale = new Vector3(0.25f, 0.25f, 0);
					frame.transform.position =  new Vector3(current_col + x, current_row + y, z);
					frame.name = "Frame";
					grid[current_row, current_col] = FRAME;
					frames.Add(frame);
				}
				else
				{
					// store the empty space as a vector (row,col)
					empty_spaces.Add(new Vector2(current_row, current_col));
					grid[current_row, current_col] = EMPTY;
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

		// set the snake score saved from the game state
		snake.GetComponent<SnakeScript>().setScore(currentScore);

		// set the snakes speed saved from the game state
		snake.GetComponent<SnakeScript>().setSpeed(currentSpeed);

		// randomly choose how many coins this level will have (1-3)
		snake.GetComponent<SnakeScript>().setNumberOfCoins(Random.Range(1,4));
	}

	// returns a random empty space from the grid
	Vector2 getRandomEmptySpace()
	{
		return (Vector2)empty_spaces[Random.Range(0, empty_spaces.Count)];
	}

	// creates the background image
	void createBackground()
	{
		// make a 4x1 grid of the background image
		Transform parent = GameObject.Find("Background").transform;
		float x = parent.position.x - 7.5f;
		float y = parent.position.y;
		float z = parent.position.z;
		
		for (int i=0; i<3; i++)
		{
			background = Instantiate(background) as GameObject;
			background.transform.parent = parent;
			background.transform.name = "background"+(i+1);
			background.transform.localScale = new Vector3(1.5f, 2.75f, 0);
			background.transform.position =  new Vector3(x, y, z);
			x += 7.5f;
		}
	}

	// create the coins for the level
	void createCoins()
	{
		coin = Instantiate(coin) as GameObject;
		coin.transform.parent = GameObject.Find("Foreground").transform;
		coin.name = "Coin";
		coin.SetActive(false);
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
			int num = snake.GetComponent<SnakeScript>().getCoinsCollected();
			if (num > 0)
			{
				float w = hudWidth/5;
				float x = hudX+(2*hudWidth)-w;
				for (int i=0; i<num; i++)
				{
					GUI.Box(new Rect(x,hudY,w,hudHeight), coin_sprite);
					x -= w;
				}
			}
		}

		// draw the score box in the centre right
		GUI.Box(new Rect(hudX+(2*hudWidth),hudY,hudWidth,hudHeight), "Score "+snake.GetComponent<SnakeScript>().getScore());
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

					// increase the speed every 5 levels
					if (level % 5 == 0)
						currentSpeed -= 0.05f;

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
}