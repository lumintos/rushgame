using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour {

	public static string username = "", email = "";
	private string password = "", rePassword = "", message = "";
	
	private bool register = false;

	private float screenWidth, screenHeight;
	private float txtScaledHeight, btnScaledHeight;
	private float txtScaledWidth, btnScaledWidth;

	private int screenWidthUnit = 48; // aspect ratio 16:9
	private int screenHeightUnit = 27;
	private int txtWidthUnit = 14;
	private int txtHeightUnit = 2;
	private int btnWidthUnit = 8;
	private int btnHeightUnit = 4;
	private int fontSizeUnit = 1;

//	public GUIStyle lbGUIStyle;
//	public GUIStyle txtGUIStyle;
//	public GUIStyle btnGUIStyle;
	public GUITexture backgroundImage;
	//public GUISkin myskin;

	public Color primaryColor, secondaryColor;

	private bool guiUpdated = false;

	void Update()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;

		//Calculate real size of textbox and button
		txtScaledHeight = (float) txtHeightUnit * screenHeight / screenHeightUnit;
		txtScaledWidth = (float) txtWidthUnit * screenWidth / screenWidthUnit;

		btnScaledHeight = (float) btnHeightUnit * screenHeight / screenHeightUnit;
		btnScaledWidth = (float) btnWidthUnit * screenWidth / screenWidthUnit;

//		Debug.Log(screenHeight + " - " + screenWidth);
//		Debug.Log(txtScaledHeight + " - " + txtScaledWidth);
//		Debug.Log(btnScaledHeight + " - " + btnScaledWidth);

		backgroundImage.pixelInset = new Rect(0, 0, screenWidth, screenHeight);

	}

	private void OnGUI()
	{
		if(!guiUpdated)
		{
			ColoredGUISkin.Instance.UpdateGuiColors(primaryColor, secondaryColor);
			guiUpdated = true;
		}
		
		GUI.skin = ColoredGUISkin.Skin;

		Rect btnLoginRect = GetScaledRect(btnScaledWidth, btnScaledHeight);
		btnLoginRect.x = 15 * screenWidth / screenWidthUnit;
		btnLoginRect.y = 19 * screenHeight / screenHeightUnit;
		
		Rect btnRegisterRect = GetScaledRect(btnScaledWidth, btnScaledHeight);
		btnRegisterRect.x = 25 * screenWidth / screenWidthUnit;
		btnRegisterRect.y = 19 * screenHeight / screenHeightUnit;		
		
		GUI.skin.label.fontSize = (int) screenHeight * fontSizeUnit / screenHeightUnit;
		GUI.skin.textField.fontSize = (int) txtScaledHeight * fontSizeUnit/ txtHeightUnit;
		GUI.skin.button.fontSize = (int) btnScaledHeight * fontSizeUnit/ btnHeightUnit;
	
		if(message != "")//There is error message
		{
			Rect msgRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			msgRect.x = 17 * screenWidth / screenWidthUnit;
			msgRect.y = 17 * screenHeight / screenHeightUnit;
			GUIStyle msgStyle = new GUIStyle(GUI.skin.label);
			msgStyle.fontStyle = FontStyle.Italic;
			msgStyle.normal.textColor = Color.white;
			msgStyle.alignment = TextAnchor.UpperCenter;
			GUI.Label(msgRect, message, msgStyle);
		}


		if(!register)
		{
			Rect lbUsernameRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			lbUsernameRect.y = 7 * screenHeight / screenHeightUnit; //constants from Layout.xlsx in References directory
			lbUsernameRect.x = 17 * screenWidth / screenWidthUnit;
			GUI.Label(lbUsernameRect, "Username");

			Rect txtUsernameRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			txtUsernameRect.y = 9 * screenHeight / screenHeightUnit; 
			txtUsernameRect.x = 17 * screenWidth / screenWidthUnit;
			username = GUI.TextField(txtUsernameRect, username);


			Rect lbPasswordRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			lbPasswordRect.y = 12 * screenHeight / screenHeightUnit; 
			lbPasswordRect.x = 17 * screenWidth / screenWidthUnit;
			GUI.Label(lbPasswordRect, "Password");

			Rect txtPasswordRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			txtPasswordRect.y = 14 * screenHeight / screenHeightUnit;
			txtPasswordRect.x = 17 * screenWidth / screenWidthUnit;
			password = GUI.PasswordField(txtPasswordRect, password, "*"[0]);


			//GUILayout.BeginHorizontal();

			if (GUI.Button(btnLoginRect, "Login"))
			{
				message = "";
				
				if (username == "" || password == "")
					message = "Please enter all the fields \n";
				else
				{
					message = "Connecting ... ";
					WWWForm form = new WWWForm();
					form.AddField("username", username);
					form.AddField("password", password);
					WWW w = new WWW("http://192.168.1.109:80/login.php", form);
					StartCoroutine(loginRequest(w));
				}
			}
			
			if (GUI.Button(btnRegisterRect, "Register"))
			{
				register = true;
				//keep username but remove password
				password = "";
				message = "";
			}
			
			//GUILayout.EndHorizontal();
		}
		else
		{
			Rect lbUsernameRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			lbUsernameRect.y = 7 * screenHeight / screenHeightUnit; //constants from Layout.xlsx in References directory
			lbUsernameRect.x = 9 * screenWidth / screenWidthUnit;
			GUI.Label(lbUsernameRect, "Username");
			
			Rect txtUsernameRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			txtUsernameRect.y = 9 * screenHeight / screenHeightUnit; 
			txtUsernameRect.x = lbUsernameRect.x;
			username = GUI.TextField(txtUsernameRect, username);
			
			Rect lbPasswordRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			lbPasswordRect.y = 12 * screenHeight / screenHeightUnit; 
			lbPasswordRect.x = lbUsernameRect.x;
			GUI.Label(lbPasswordRect, "Password");
			
			Rect txtPasswordRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			txtPasswordRect.y = 14 * screenHeight / screenHeightUnit;
			txtPasswordRect.x = lbUsernameRect.x;
			password = GUI.PasswordField(txtPasswordRect, password, "*"[0]);

			Rect lbEmailRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			lbEmailRect.y = 7 * screenHeight / screenHeightUnit; 
			lbEmailRect.x = 26 * screenWidth / screenWidthUnit;
			GUI.Label(lbEmailRect, "Email");
			
			Rect txtEmailRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			txtEmailRect.y = 9 * screenHeight / screenHeightUnit; 
			txtEmailRect.x = lbEmailRect.x;
			email = GUI.TextField(txtEmailRect, email);
			
			Rect lbRePasswordRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			lbRePasswordRect.y = 12 * screenHeight / screenHeightUnit; 
			lbRePasswordRect.x = lbEmailRect.x;
			GUI.Label(lbRePasswordRect, "Re-type Password");
			
			Rect txtRePasswordRect = GetScaledRect(txtScaledWidth, txtScaledHeight);
			txtRePasswordRect.y = 14 * screenHeight / screenHeightUnit;
			txtRePasswordRect.x = lbEmailRect.x;
			rePassword = GUI.PasswordField(txtRePasswordRect, rePassword, "*"[0]);

			GUILayout.BeginHorizontal();
			
			if (GUI.Button(btnLoginRect, "Back"))
			{
				register = false;
				//reset all fields except username
				//username = "";
				password = "";
				email = "";
				rePassword = "";
				message = "";
			}
			
			if (GUI.Button(btnRegisterRect, "Register"))
			{
				message = "";
				
				if (username == "" || email == "" || password == "")
					message = "Please enter all the fields \n";
				else
				{
					if (password == rePassword)
					{
						message = "Connecting ... ";
						WWWForm form = new WWWForm();
						form.AddField("username", username);
						form.AddField("email", email);
						form.AddField("password", password);
                        //WWW w = new WWW("http://84.101.189.177:25500/register.php", form);
                        WWW w = new WWW("http://192.168.1.109:80/register.php", form);
						StartCoroutine(registerRequest(w));
					}
					else
						message = "Your Password does not match \n";
				}
			}
			
			GUILayout.EndHorizontal();

		}

	
	}

	IEnumerator loginRequest(WWW w)
	{
		yield return w;
		if (w.error == null)
		{
			if (w.text == "Logged in")
            {
                //((User)GameObject.Find("User").GetComponent("User")).username = username; //store username for later scenes
                MultiplayerManager.Instance.PlayerName = username;
                PlayerPrefs.SetString("PlayerName", MultiplayerManager.Instance.PlayerName);
				Application.LoadLevel("lobby");
			}
			else
			{
				message = w.text;
			}
		}
		else
		{
			message = "ERROR: " + w.error + "\n";
		}
	}
	
	IEnumerator registerRequest(WWW w)
	{
		yield return w;
		if (w.error == null)
		{
			if(w.text == "Succesfully Created User!")
			{
                ((User)GameObject.Find("User").GetComponent("User")).username = username; //store username for later scenes
				Application.LoadLevel("lobby");
			}
			else
			{
				message = w.text;
			}
		}
		else
		{
			message = "ERROR: " + w.error + "\n";
		}
	}	
	
	private Rect GetScaledRect(float width, float height)
	{
		float scaledW = width;
		float scaledH = height;
		float top = 0, left = (screenWidth - width) / 2;

		return new Rect(left, top, scaledW, scaledH);
	}
}
