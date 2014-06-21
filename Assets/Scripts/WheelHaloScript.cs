using UnityEngine;
using System.Collections;

public class WheelHaloScript : MonoBehaviour {

	private float duration = 1.0F;
	private Color color0 = Color.black;
	private Color color1 = Color.white;
	private float lastUpdate;

	// Update is called once per frame
	void Update () 
	{
		// automatically change the light colour
		float t = Mathf.PingPong(Time.time, duration) / duration;
		light.color = Color.Lerp(color0, color1, t);

	}
}
