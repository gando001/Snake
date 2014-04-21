using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public GameObject frame;
	public GameObject apple;
	public GameObject snake;

	// level values
	public const int EMPTY = 0;
	public const int FRAME = 1;
	public const int SNAKE = 2;
	public int DIRECTION;

	// camera and scale values
	public const float CAM_WIDTH = 1017;
	public const float CAM_HEIGHT = 481;
	public float SCALE_X;
	public float SCALE_Y;

	// game logic
	private int[,] grid; 
	private ArrayList empty_spaces;
	private int rows = 12;
	private int cols = 25;
	private int level = 0;
	
	// Use this for initialization
	void Start () {

		grid = new int[rows,cols];
		empty_spaces = new ArrayList();

		// set up the scale
		SCALE_X = Screen.width/CAM_WIDTH;
		SCALE_Y = Screen.height/CAM_HEIGHT;

		// create the background image

		// create the level
		createLevel();

		// create the snake
		createSnake();

		// create an apple
		createApple();
	}

	public void updateGrid(int row, int col, int value)
	{
		if (value == SNAKE)
		{
			// update the snake head position in the grid
			grid[row, col] = SNAKE;

			// remove the empty space
			empty_spaces.Remove(new Vector2(row, col));
		}
		else
		{
			// update the tail position
			grid[row, col] = EMPTY;

			// store the empty space as a vector (row,col)
			empty_spaces.Add(new Vector2(row, col));
		}
	}

	public void moveApple()
	{
		// get a random empty space from the array 0 - count
		Vector2 space = getRandomEmptySpace();
		apple.transform.position = this.getScaledPostion((space.y+apple.transform.parent.position.x), (space.x+apple.transform.parent.position.y), apple.transform.parent.position.z);
	}

	public Vector3 getScaledPostion(float x, float y, float z)
	{
		return new Vector3(x*SCALE_X,y*SCALE_Y,z);
	}

	// Update is called once per frame
	void Update () 
	{
		if (snake.GetComponent<SnakeScript>().isGameOver())
		{
			// game is over check whether the user won or lost
			Destroy(snake);
			Destroy(this);
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
		DIRECTION = Random.Range(1,5);

		// find a valid space based on the direction
		bool valid = false;
		Vector2 space = getRandomEmptySpace();
		int index = 0;
		while (!valid)
		{
			// need to check the space for the tail before setting the snake to it
			if (DIRECTION == 1)
			{
				// left - the column on the right needs to be empty too
				index = empty_spaces.IndexOf(new Vector2(space.x, space.y+1));
				if (index != -1)
					valid = true;
			}
			else if (DIRECTION == 2)
			{
				// right - the column on the left needs to be empty too
				index = empty_spaces.IndexOf(new Vector2(space.x, space.y-1));
				if (index != -1)
					valid = true;
			}
			else if (DIRECTION == 3)
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

		// set the snake head and tail to the spaces
		snake.GetComponent<SnakeScript>().setHeadStartingPosition((int)space.x, (int)space.y);
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

	Vector2 getRandomEmptySpace()
	{
		return (Vector2)empty_spaces[Random.Range(0, empty_spaces.Count)];
	}
}
