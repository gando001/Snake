using UnityEngine;
using System.Collections;

public class WheelScript : MonoBehaviour {

	// bonus sprites
	public Texture2D spin_again, spin_empty, spin_slow, spin_apple, spin_double_points, spin_half_snake, spin_coin, spin_speed, spin_life, spin_opposite;

	public GameObject damper;
	public GameObject button;

	// sounds
	public AudioClip positive_sound;
	public AudioClip negative_sound;

	private GameObject snake;
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
	private TextMesh item_description;
	private SoundScript sound;
	private bool isGoodItem;

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
		damper.GetComponent<DamperScript>().reset();
		button.GetComponent<WheelButtonScript>().reset();
	}

	public void displayResult(string txt)
	{
		text = txt;
		isFinished = true;
		isGoodItem = true;

		// determine the item
		if (text == "Spin_again")
		{
			sprite = spin_again;
			description = "you have \nwon \na free spin";
		}
		else if (text == "Spin_empty")
		{
			sprite = spin_empty;
			description = "you have \nwon \nnothing";
			isGoodItem = false;
		}
		else if (text == "Spin_slow")
		{
			sprite = spin_slow;
			description = "speed \ndecrease";
		}
		else if (text == "Spin_apple")
		{
			sprite = spin_apple;
			description = "remove \napples";
			isGoodItem = false;
		}
		else if (text == "Spin_double_points")
		{
			sprite = spin_double_points;
			description = "double \npoints";
		}
		else if (text == "Spin_half_snake")
		{
			sprite = spin_half_snake;
			description = "cut \nsnake";
			isGoodItem = false;
		}
		else if (text == "Spin_coin")
		{
			sprite = spin_coin;
			description = "+1 coin";
		}
		else if (text == "Spin_speed")
		{
			sprite = spin_speed;
			description = "speed \nincrease";
			isGoodItem = false;
		}
		else if (text == "Spin_life")
		{
			sprite = spin_life;
			description = "+1 life";
		}
		else if (text == "Spin_opposite")
		{
			sprite = spin_opposite;
			description = "upside \ndown, \ngood luck!";
			isGoodItem = false;
		}

		// create a sprite and display its description
		item.sprite = Sprite.Create(sprite, new Rect(0,0,sprite.width,sprite.height), new Vector2(0.5f,0.5f));
		item_description.text = description;

		if (isGoodItem)
		{
			// play the sound
			if (sound.isSoundPlaying())
				audio.PlayOneShot(positive_sound);
		}
		else
		{
			// play the sound
			if (sound.isSoundPlaying())
				audio.PlayOneShot(negative_sound);
		}
	}

	public void setLevelSpin() 
	{
		resetWheel();
		isTryingToGetLife = true;
		isFirstSpin = true;
	}

	public void setSnake(GameObject g) {
		snake = g;
	}


	

	// Use this for initialization
	void Start () 
	{
		resetWheel();

		game = GameObject.Find("Game").GetComponent<GameScript>();
		item = GameObject.Find("Item").GetComponent<SpriteRenderer>();
		item_description = GameObject.Find("Item_description").GetComponent<TextMesh>();
		sound = GameObject.Find("Sound").GetComponent<SoundScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		getUserInput();
		handleSpinResult();
	}

	// handles all of the functionality of user input and initiating the spin
	void getUserInput()
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
			
			damper.GetComponent<DamperScript>().setStarted(); // notify the damper
			
			if (isTryingToGetLife && !isFreeSpin)
			{
				// reduce the coins count
				int num = snake.GetComponent<SnakeScript>().getCoinsCollected()-1;
				snake.GetComponent<SnakeScript>().setCoinsCollected(num);
			}
		}
	}

	// handles what the wheel finishes on
	void handleSpinResult()
	{	
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
					description = "you can \nspin again!";
					displayCurrentDescription = true; // no continue button required; user has to spin again
				}
				else if (text == "Spin_life")
				{
					// user won a life so display the button to continue
					snake.GetComponent<SnakeScript>().addLife();
					description = "phew! \nyou got \na life!";
					showContinue = true;
					isSwipe = true;
				}
				else
				{
					// wrong item selected
					if (snake.GetComponent<SnakeScript>().getCoinsCollected() > 0)
					{
						// user has more coins to try again
						description = "bad luck!\ntry again!";
						displayCurrentDescription = true; // no continue button required; user can spin again
					}
					else
					{
						// user has lost the game
						// alter the description and show a button
						description = "you lose!";
						showContinue = true;
						isSwipe = true;
					}
				}
			}
		}
		else if (displayCurrentDescription)
		{
			// keep displaying the current bonus item sprite and description
			item_description.text = description;
		}
		else if (showContinue)
		{
			// keep displaying the current bonus item sprite and description
			item_description.text = description;
			button.GetComponent<WheelButtonScript>().displaySpite();
			
			if (!isTryingToGetLife)
			{
				// normal game play
				if (button.GetComponent<WheelButtonScript>().isButtonClicked())
				{		
					// hide the wheel
					game.hideBonusWheel();
					
					// apply the bonus item
					if (text == "Spin_slow")
					{
						snake.GetComponent<SnakeScript>().slowmoSnake();
						game.setBonusSprite(item.sprite);
					}
					else if (text == "Spin_apple")
					{
						snake.GetComponent<SnakeScript>().hideApple();
						game.setBonusSprite(item.sprite);
					}
					else if (text == "Spin_double_points")
					{
						snake.GetComponent<SnakeScript>().setDoublePoints();
						game.setBonusSprite(item.sprite);
					}
					else if (text == "Spin_half_snake")
						snake.GetComponent<SnakeScript>().halfSnake();
					else if (text == "Spin_coin")
						snake.GetComponent<SnakeScript>().coinCollected();
					else if (text == "Spin_speed")
					{
						snake.GetComponent<SnakeScript>().speedSnake();
						game.setBonusSprite(item.sprite);
					}
					else if (text == "Spin_life")
						snake.GetComponent<SnakeScript>().addLife();
					else if (text == "Spin_opposite")
					{
						snake.GetComponent<SnakeScript>().setRotate();
						game.setBonusSprite(item.sprite);
					}
					
					resetWheel();
				}
			}
			else
			{
				// either the user has spun and got a life or has no more coins left to spin
				if (button.GetComponent<WheelButtonScript>().isButtonClicked())
				{	
					resetWheel();
					
					// hide the wheel
					game.hideBonusWheel();
				}
			}
		}
		else
		{
			// create the content
			description = "spin the \nwheel!";
			if (isTryingToGetLife && isFirstSpin)
				description =  "use your \ncoins to \nspin for \na life!"; // this is only required when spinning for a life the first time
			
			item.sprite = null;
			item_description.text = description;
		}
	}
}