using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour {

	public static string username = "", realname = "";
	private string password = "", rePass = "", message = "";
	
	private bool register = false;

	private float screenWidth, screenHeight;
	private float optimalWidth = 1600;
	private float optimalHeight = 900;
	private float screenScale;
	private float optimalItemHeightScale = 0.125f;
	private float optimalItemWidthScale = 0.5f;
	public float optimalItemHeight;
	public float optimalItemWidth;

	void OnStart()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		screenScale = screenHeight / optimalHeight;
		optimalItemHeight = optimalItemHeightScale * screenHeight;
		optimalItemWidth = optimalItemWidthScale * screenWidth;
	}

	void Rect getScaledRect(float w, float h)
	{
		float scaledW = w;
		float scaledH = h;
		float top = 0, left = 0;

		return new Rect(left, top, scaledW, scaledH);
	}

	void OnGui()
	{
		if (message != "")
			GUI.Box

	}

	IEnumerator loginRequest(WWW w)
	{
		yield return w;
		if (w.error == null)
		{
			if (w.text == "Logged in")
			{
				Application.LoadLevel("map");
			}
			else
				message += w.text;
		}
		else
		{
			message += "ERROR: " + w.error + "\n";
		}
	}
	
	IEnumerator registerRequest(WWW w)
	{
		yield return w;
		if (w.error == null)
		{
			message += w.text;
		}
		else
		{
			message += "ERROR: " + w.error + "\n";
		}
	}
}
