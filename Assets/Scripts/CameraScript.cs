using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
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