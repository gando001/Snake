using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public GameObject background;
	public GameObject frame;
	public GameObject apple;
	public GameObject snake;

	// level values
	public const int EMPTY = 0;
	public const int FRAME = 1;
	public const int SNAKE = 2;

	// game logic
	private int[,] grid; 
	private ArrayList empty_spaces;
	private const int rows = 13;
	private const int cols = 25;
	private int level = 0;
	private int direction;

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
		apple.transform.position = this.getScaledPostion((space.y+apple.transform.parent.position.x), (space.x+apple.transform.parent.position.y), apple.transform.parent.position.z);
	}

	// Returns the given position scaled to the current screens width and height
	public Vector3 getScaledPostion(float x, float y, float z)
	{
		return new Vector3(x,y,z);
	}

	// Returns the column and row for the given scaled position
	public Vector2 getColAndRow(float x, float y)
	{
		return new Vector2(x, y);
	}

	public bool isGridEmpty(int row, int col)
	{
		if (grid[row,col] == EMPTY)
			return true;
		return false;
	}



	// Use this for initialization
	void Start () {
		
		// create the game grid
		grid = new int[rows,cols];
		empty_spaces = new ArrayList();
		
		// set up the orthographic camera size based on the screen resolution
		float res = (float)Screen.width/Screen.height;
		float size = 0;
		if (res < 1.4)
			size = 9.5f;
		else if (res < 1.6)
			size = 8.5f;
		else if (res < 1.7)
			size = 7.9f;
		else
			size = 7.45f;
		Camera.main.orthographicSize = size;
		
		// create the background image
		createBackground();
		
		// create the level
		createLevel();
		
		// create the snake
		createSnake();
		
		// create an apple
		createApple();
	}

	// Update is called once per frame
	void Update () 
	{
		if (snake.GetComponent<SnakeScript>().isGameOver())
		{
			// game is over so check whether the user won or lost
			if (snake.GetComponent<SnakeScript>().isLevelPassed())
			{
				// user has passed this level
			}
			else
			{
				// user failed this level

				// display the flashing snake for 3 seconds then display the menu options
			}
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
					frame.transform.localScale = this.getScaledPostion(0.25f, 0.25f, 0);
					frame.transform.position =  this.getScaledPostion(current_col + x, current_row + y, z);
					frame.name = "Frame";
					grid[current_row, current_col] = FRAME;
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

	// creates the snake and chooses a starting position
	void createSnake()
	{
		snake = Instantiate(snake) as GameObject;
		snake.GetComponent<SnakeScript>().setGameScript(this);

		// choose a random direction
		direction = Random.Range(1,5);

		// find a valid space based on the direction
		bool valid = false;
		Vector2 space = getRandomEmptySpace();
		int index = 0;
		while (!valid)
		{
			// need to check the space for the tail before setting the snake to it
			if (direction == 1)
			{
				// left - the column on the right needs to be empty too
				index = empty_spaces.IndexOf(new Vector2(space.x, space.y+1));
				if (index != -1)
					valid = true;
			}
			else if (direction == 2)
			{
				// right - the column on the left needs to be empty too
				index = empty_spaces.IndexOf(new Vector2(space.x, space.y-1));
				if (index != -1)
					valid = true;
			}
			else if (direction == 3)
			{
				// up - the row down needs to be empty too
				index = empty_spaces.IndexOf(new Vector2(space.x+1, space.y));
				if (index != -1)
					valid = true;
			}
			else
			{
				// down - the row up needs to be empty too
				index = empty_spaces.IndexOf(new Vector2(space.x-1, space.y));
				if (index != -1)
					valid = true;
			}

			if (!valid)
			{
				// get a random empty space from the array 0 - count
				space = getRandomEmptySpace();
			}
		}

		// found a valid tail space
		Vector2 tail_space = (Vector2)empty_spaces[index];
		
		// remove the valid spaces from empty spaces
		empty_spaces.Remove(space);
		empty_spaces.Remove(tail_space);

		// update the grid
		updateGrid((int)space.x, (int)space.y, GameScript.SNAKE);
		updateGrid((int)tail_space.x, (int)tail_space.y, GameScript.SNAKE);

		// set the snake head and tail to the spaces
		snake.GetComponent<SnakeScript>().setHeadStartingPosition((int)space.x, (int)space.y, direction);
		snake.GetComponent<SnakeScript>().setTailStartingPosition((int)tail_space.x, (int)tail_space.y);
	}

	// creates the apple
	void createApple()
	{
		apple = Instantiate(apple) as GameObject;
		apple.transform.parent = GameObject.Find("Foreground").transform;
		apple.name = "Apple";
		moveApple();
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
			background.transform.localScale = this.getScaledPostion(1.5f, 2.75f, 0);
			background.transform.position =  this.getScaledPostion(x, y, z);
			x += 7.5f;
		}
	}
}
