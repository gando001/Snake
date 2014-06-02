using UnityEngine;
using System.Collections;

public class DamperScript : MonoBehaviour {

	private string selected_item;
	private bool spinStarted;
	private float time;
	private GameObject wheel;
	private GameObject snake;

	public void setStarted()
	{
		spinStarted = true;
	}

	// Use this for initialization
	void Start () {

		selected_item = "";
		spinStarted = false;
		time = -1;
		wheel = GameObject.Find("Wheel");
		snake = GameObject.Find("Snake");
	}

	// Update is called once per frame
	void Update () 
	{
		if (spinStarted)
		{
			time--;

			if (time == 0)
			{
				print ("Item you got is "+selected_item);
				handleItem();
				spinStarted = false;
			}
		}
	}

	void OnTriggerStay(Collider otherCollider)
	{	
		// determine the other collider
		selected_item = otherCollider.gameObject.name;
	}

	void OnTriggerExit(Collider otherCollider)
	{
		// reset the timer after each exit
		// the chosen item will not have its time reset
		time = 200;
	}

	void handleItem()
	{	
		// handle the various wheel items
		string txt = "";
		if (selected_item == "Spin_again")
		{
			// reset the damper and wheel
			Start ();
			wheel.GetComponent<WheelScript>().resetWheel();
		}
		else if (selected_item == "Spin_empty")
		{
			txt = "You won nothing";
		}
		else if (selected_item == "Spin_slow")
		{
			snake.GetComponent<SnakeScript>().slowmoSnake();
			txt = "slowmo";
		}
		else if (selected_item == "Spin_apple")
		{
			snake.GetComponent<SnakeScript>().hideApple();
			txt = "remove apple";	
		}
		else if (selected_item == "Spin_double_points")
		{
			snake.GetComponent<SnakeScript>().setDoublePoints();
			txt = "double points";	
		}
		else if (selected_item == "Spin_half_snake")
		{
			snake.GetComponent<SnakeScript>().halfSnake();
			txt = "1/2 snake";
		}
		else if (selected_item == "Spin_coin")
		{
			snake.GetComponent<SnakeScript>().coinCollected();
			txt = "+1 coin";
		}
		else if (selected_item == "Spin_speed")
		{
			snake.GetComponent<SnakeScript>().speedSnake();
			txt = "speed";	
		}
		else if (selected_item == "Spin_life")
		{
			snake.GetComponent<SnakeScript>().addLife();
			txt = "+1 life";
		}
		else if (selected_item == "Spin_opposite")
		{
			snake.GetComponent<SnakeScript>().setRotate();
			txt = "opposite";
		}
		
		wheel.GetComponent<WheelScript>().displayResult(txt);
	}
}