using UnityEngine;
using System.Collections;

/* Handles the entire game, menus and gameplay */
public class GameManager : MonoBehaviour {

	private int level;
	public static GameManager Instance;
	
	public void incrementLevel()
	{
		level++;
	}

	public int getLevel()
	{
		return level;
	}

	// Use this for initialization
	void Start () 
	{
		level = 1;
	}
	
	void Awake()
	{
		if(Instance)
		{
				DestroyImmediate(gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}

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
	}
}