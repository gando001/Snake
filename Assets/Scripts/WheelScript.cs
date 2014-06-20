using UnityEngine;
using System.Collections;

public class WheelScript : MonoBehaviour {

	// bonus sprites
	public Texture2D spin_again, spin_empty, spin_slow, spin_apple, spin_double_points, spin_half_snake, spin_coin, spin_speed, spin_life, spin_opposite;

	private bool isSwipe;
	private bool isSpinning;
	private bool isFinished;
	private bool displayCurrentDescription;
	private bool showContinue;
	private string text;
	private GameScript game;
	private bool isTryingToGetLife;
	private bool isFirstSpin;
	private bool isFreeSpin;
	private SpriteRenderer item;
	private Texture2D sprite;
	private string description;
	private float x, y, w, h, itemY;
	private GUISkin skin;
	private GameObject snake;

	// speed
	private float speed;
	private const int MIN_SPEED = 500;
	private const int MAX_SPEED = 1000;

	// physics variables
	private float startY;
	private float startTime;
	private float endY;
	private float endTime;

	// completely resets the wheel
	public void resetWheel()
	{
		speed = 0;
		isSwipe = false;
		isSpinning = false;
		isFinished = false;
		isTryingToGetLife = false;
		isFirstSpin = false;
		isFreeSpin = false;
		displayCurrentDescription = false;
		showContinue = false;
		GameObject.Find("Damper").GetComponent<DamperScript>().reset();
	}

	public void displayResult(string txt)
	{
		text = txt;
		isFinished = true;

		// determine the item
		if (text == "Spin_again")
		{
			sprite = spin_again;
			description = "You have won a free spin";
		}
		else if (text == "Spin_empty")
		{
			sprite = spin_empty;
			description = "You have won nothing";
		}
		else if (text == "Spin_slow")
		{
			sprite = spin_slow;
			description = "Reduce speed";
		}
		else if (text == "Spin_apple")
		{
			sprite = spin_apple;
			description = "Remove apples";
		}
		else if (text == "Spin_double_points")
		{
			sprite = spin_double_points;
			description = "Double points";
		}
		else if (text == "Spin_half_snake")
		{
			sprite = spin_half_snake;
			description = "Cut snake in half";
		}
		else if (text == "Spin_coin")
		{
			sprite = spin_coin;
			description = "+1 coin";
		}
		else if (text == "Spin_speed")
		{
			sprite = spin_speed;
			description = "Increase speed";
		}
		else if (text == "Spin_life")
		{
			sprite = spin_life;
			description = "+1 Life";
		}
		else if (text == "Spin_opposite")
		{
			sprite = spin_opposite;
			description = "Upside down, good luck!";
		}

		// create a sprite
		item.sprite = Sprite.Create(sprite, new Rect(0,0,sprite.width,sprite.height), new Vector2(0.5f,0.5f));
	}

	public void setLevelSpin() 
	{
		resetWheel();
		isTryingToGetLife = true;
		isFirstSpin = true;
	}


	

	// Use this for initialization
	void Start () 
	{
		resetWheel();

		game = GameObject.Find("Game").GetComponent<GameScript>();
		item = GameObject.Find("Item").GetComponent<SpriteRenderer>();
		snake = GameObject.Find("Snake");
		skin = (GUISkin)Resources.Load("Skins/Wheel_Skin");

		// set up box coordinates
		Vector3 pos = Camera.main.WorldToScreenPoint(GameObject.Find("body_corner_xy").transform.position);
		x = pos.x;
		y = pos.y;
		w = 135;
		h = 128;

		description = "Spin the wheel!";
	}

	void OnGUI ()
	{
		GUI.skin = skin;

		if (isFinished)
		{
			if (!isTryingToGetLife)
			{
				// normal game play mode 

				// reset the damper and wheel to reset all the flags
				resetWheel();

				// apply the item
				if (text == "Spin_again")
					displayCurrentDescription = true; // no continue button required; user can spin again
				else
				{
					showContinue = true; // normal wheel so have a button to continue and apply the item
					isSwipe = true; // stops the user from spinning again
				}
			}
			else
			{
				// reset the damper and wheel to reset all the flags
				resetWheel();
				isTryingToGetLife = true;

				// user is using coins to get a life
				if (text == "Spin_again")
				{
					displayCurrentDescription = true; // no continue button required; user can spin again
					isFreeSpin = true;
				}
				else if (text == "Spin_coin")
				{ 
					// increase the coin count
					snake.GetComponent<SnakeScript>().coinCollected();

					// alter the description
					description += "\nSpin again!";
					displayCurrentDescription = true; // no continue button required; user can spin again
				}
				else if (text == "Spin_life")
				{
					// user won a life so display the button to continue
					snake.GetComponent<SnakeScript>().addLife();
					description += "\nPhew! You got a life!";
					showContinue = true;
					isSwipe = true;
				}
				else
				{
					// wrong item selected
					if (snake.GetComponent<SnakeScript>().getCoinsCollected() > 0)
					{
						// user has more coins to try again
						description += "Try again!";
						displayCurrentDescription = true; // no continue button required; user can spin again
					}
					else
					{
						// user has lost the game
						// alter the description and show a button
						description = "You lose!";
						showContinue = true;
						isSwipe = true;
					}
				}
			}
		}
		else if (displayCurrentDescription)
		{
			// keep displaying the current bonus item sprite and description
			GUI.Box (new Rect(x,y,w,h), description);
		}
		else if (showContinue)
		{
			// keep displaying the current bonus item sprite and description
			GUI.Box (new Rect(x,y,w,h), description);

			if (!isTryingToGetLife)
			{
				// normal game play
				if (GUI.Button(new Rect(x,y+h,w,50), "Continue"))
				{	
					// apply the bonus item
					if (text == "Spin_slow")
					{
						snake.GetComponent<SnakeScript>().slowmoSnake();
						game.setBonusSprite(spin_slow);
					}
					else if (text == "Spin_apple")
					{
						snake.GetComponent<SnakeScript>().hideApple();
						game.setBonusSprite(spin_apple);
					}
					else if (text == "Spin_double_points")
					{
						snake.GetComponent<SnakeScript>().setDoublePoints();
						game.setBonusSprite(spin_double_points);
					}
					else if (text == "Spin_half_snake")
						snake.GetComponent<SnakeScript>().halfSnake();
					else if (text == "Spin_coin")
						snake.GetComponent<SnakeScript>().coinCollected();
					else if (text == "Spin_speed")
					{
						snake.GetComponent<SnakeScript>().speedSnake();
						game.setBonusSprite(spin_speed);
					}
					else if (text == "Spin_life")
						snake.GetComponent<SnakeScript>().addLife();
					else if (text == "Spin_opposite")
					{
						snake.GetComponent<SnakeScript>().setRotate();
						game.setBonusSprite(spin_opposite);
					}
					
					// hide the wheel
					game.hideBonusWheel();
				}
			}
			else
			{
				// either the user has spun and got a life or has no more coins left to spin
				if (GUI.Button(new Rect(x,y+h,w,50), "Continue"))
				{	
					// hide the wheel
					game.hideBonusWheel();
				}
			}
		}
		else
		{
			// create the content
			if (isTryingToGetLife && isFirstSpin)
				description =  "Use your coins to spin the wheel for a life!"; // this is only required when spinning for a life the first time

			GUIContent c = new GUIContent(description);

			item.sprite = null;
		
			GUIStyle style = new GUIStyle();
			style.wordWrap = true;
			style.fontSize = 18;

			// calculate the height
			float h = style.CalcHeight(c,w);

			// display the label
			GUI.Box (new Rect(x,y,w,h), c);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GameScript.REAL_DEVICE)
		{
			if (!isSwipe && Time.timeScale != 0)
			{
				if (Input.touchCount > 0) 
				{
					Touch touch = Input.GetTouch(0);
					
					// user is swiping the screen
					if (touch.phase == TouchPhase.Began)
					{
						// save the y point and time when the screen was touched
						startY = Input.GetTouch(0).position.y;
						startTime = Time.time;
					}
					else if (touch.phase == TouchPhase.Ended)
					{
						// get the distance moved and time
						endY =  Input.GetTouch(0).position.y - startY;
						endTime = Time.time - startTime;
						
						// speed is distance divided by time
						speed = Mathf.Abs(endY/endTime);

						if (speed > MIN_SPEED)
						{
							isSwipe = true;
							isSpinning = true;
						}
					}
				}
			}
		}
		else
		{
			if (!isSwipe && Time.timeScale != 0)
			{
				if(Input.GetMouseButtonDown(0))
				{
					// save the y point and time when the mouse was pressed
					startY = Input.mousePosition.y;
					startTime = Time.time;
				}
				else if(Input.GetMouseButtonUp(0))
				{
					// get the distance moved and time
					endY =  Input.mousePosition.y - startY;
					endTime = Time.time - startTime;

					// speed is distance divided by time
					speed = Mathf.Abs(endY/endTime);

					if (speed > MIN_SPEED)
					{
						isSwipe = true;
						isSpinning = true;
					}
				}
			}
		}

		if (isSpinning)
		{
			float z = 10;
			if (endY < startY)
				z = -z;
			rigidbody.maxAngularVelocity = 100; // this allows for varying torque values
			rigidbody.AddTorque(new Vector3(0,0,z) * speed);
			GameObject.Find("Inner Wheel").rigidbody.AddTorque(new Vector3(0,0,-z) * speed);

			isSpinning = false;

			GameObject.Find("Damper").GetComponent<DamperScript>().setStarted(); // notify the damper

			if (isTryingToGetLife && !isFreeSpin)
			{
				// reduce the coins count
				int num = snake.GetComponent<SnakeScript>().getCoinsCollected()-1;
				snake.GetComponent<SnakeScript>().setCoinsCollected(num);
			}
		}
	}
}