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
		apple.transform.position = new Vector3(space.y+apple.transform.parent.position.x, space.x+apple.transform.parent.position.y, apple.transform.parent.position.z);
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
		string[] lines = System.IO.File.ReadAllLines(@"Assets/Levels/level_"+level+".txt");

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
					frame.transform.position = new Vector3(current_col + x, current_row + y, z);
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

		// get a random empty space from the array 0 - count
		Vector2 space = getRandomEmptySpace();

		// need to check the space before setting the snake at it

		// remove the space from empty spaces

		// set the grid value to OPCCUPIED

		snake.GetComponent<SnakeScript>().setStartingPosition((int)space.x, (int)space.y); // randomize these
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
