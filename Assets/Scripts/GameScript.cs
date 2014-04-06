using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	public GameObject frame;
	public GameObject apple;
	public GameObject snake;

	private GameObject[,] grid; 
	private int rows = 12;
	private int cols = 25;
	
	// Use this for initialization
	void Start () {

		grid = new GameObject[rows,cols];

		// create the level
		createLevel();

		// create the snake
		snake = Instantiate(snake) as GameObject;
		snake.GetComponent<SnakeScript>().setStartingPosition(-5,5); // randomize these
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void createLevel()
	{
		Transform parent = GameObject.Find("1 - Middleground").transform;
		float x = (float)parent.position.x;
		float y = (float)parent.position.y;
		float z = (float)parent.position.z;

		for (int i=0; i<rows; i++)
		{
			frame = Instantiate(frame) as GameObject;
			frame.transform.parent = parent;
			frame.transform.position = new Vector3(x, i - y, z);
			grid[i,0] = frame;

			frame = Instantiate(frame) as GameObject;
			frame.transform.parent = parent;
			frame.transform.position = new Vector3(cols-1+x, i - y, z);
			grid[i,cols-1] = frame;

			if (i == 0 || i == rows-1)
			{
				for (int j=0; j<cols; j++)
				{
					frame = Instantiate(frame) as GameObject;
					frame.transform.parent = parent;
					frame.transform.position = new Vector3(j + x, i - y, z);
					grid[i,j] = frame;
				}
			}
		}
	}
}
