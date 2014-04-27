using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	private float centreX, centreY, buttonWidth, buttonHeight;

	// Use this for initialization
	void Start () 
	{
		centreX = Screen.width/2;
		centreY = Screen.height/2;
		buttonWidth = Screen.width/6;
		buttonHeight = buttonWidth/2;
	}

	void OnGUI ()
	{
		// give the user options for the main menu
		if (GUI.Button(new Rect(centreX-buttonWidth/2, centreY-buttonHeight, buttonWidth, buttonHeight), "New Game"))
		{	
			// delete any saved game data to start a new game
			PlayerPrefs.DeleteAll();

			// Load the level
			Application.LoadLevel("level");
		}
		
		if (GUI.Button(new Rect(centreX-buttonWidth/2, centreY, buttonWidth, buttonHeight), "Help"))
		{	
			// Load the level
			Application.LoadLevel("help");
		}

		if (GUI.Button(new Rect(centreX-buttonWidth/2, centreY+buttonHeight, buttonWidth, buttonHeight), "Quit"))
		{	
			// quit the application
			Application.Quit();
		}
	}
}
