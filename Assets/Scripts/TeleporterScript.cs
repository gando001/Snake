using UnityEngine;
using System.Collections;

public class TeleporterScript : MonoBehaviour {

	private int direction;
	private int row;
	private int col;
	private GameObject pair;

	public void setDirection(int d)
	{
		direction = d;
	}

	public int getDirection()
	{
		// return the opposite direction to get to the exit cell
		if (direction == GameScript.LEFT)
			return GameScript.RIGHT;
		else if (direction == GameScript.RIGHT)
			return GameScript.LEFT;
		else if (direction == GameScript.UP)
			return GameScript.DOWN;
		else
			return GameScript.UP;
	}

	public void setRowAndCol(int r, int c)
	{
		row = r;
		col = c;
	}

	// returns the exit cell grid position (row, col) that this teleporter exits to
	public Vector2 getExit()
	{
		if (direction == GameScript.LEFT)
			return new Vector2(row,col+1); // move to the right cell since this teleporter is on the exits left
		else if (direction == GameScript.RIGHT)
			return new Vector2(row,col-1);
		else if (direction == GameScript.UP)
			return new Vector2(row-1,col);
		else
			return new Vector2(row+1,col);
	}

	public void setPair(GameObject p)
	{
		pair = p;
	}

	public GameObject getPair()
	{
		return pair;
	}
}
