//Author: Giang
//TODO: refactor all GUI functions by using GUiHelper's ones
using UnityEngine;
using System.Collections;
using System.Xml;

public class Login : MonoBehaviour {

	public static string username = "", email = "";
	private string password = "", rePassword = "";
	
	private bool register = false;

	private float screenWidth, screenHeight;
    private float stdScreenWidth, stdScreenHeight;

	private int screenWidthUnit = 48; // aspect ratio 16:9
	private int screenHeightUnit = 27;
    public float txtWidthUnit = 20;
    public float txtHeightUnit = 2.5f;
	private float fontSizeUnit = 1.5f;

    public GUITexture loginButton;
    public GUITexture registerButton;
    public GUITexture backButton;
    public GUIText lbUsername, lbPassword, lbEmail, lbRetypePassword;
    public GUITexture groupBox;
    public GUIText msg;

//	public GUIStyle lbGUIStyle;
//	public GUIStyle txtGUIStyle;
//	public GUIStyle btnGUIStyle;
	public GUITexture backgroundImage;
	//public GUISkin myskin;

	public Color primaryColor, secondaryColor;

	private bool guiUpdated = false;

    void Start()
    {
        stdScreenHeight = 540;
        stdScreenWidth = 960;
        screenWidth = screenHeight = 0;
        UpdateGUIElementsSize();
    }

	void Update()
	{
        if (screenWidth != Screen.width) //screen changes size
        {
            UpdateGUIElementsSize();
        }
	}

	private void OnGUI()
	{
		if(!guiUpdated)
		{
			ColoredGUISkin.Instance.UpdateGuiColors(primaryColor, secondaryColor);
			guiUpdated = true;
		}
		
		GUI.skin = ColoredGUISkin.Skin;	
		
		GUI.skin.textField.fontSize = (int) (screenHeight * fontSizeUnit/ screenHeightUnit);
		//GUI.skin.label.fontSize = (int) (screenHeight * fontSizeUnit / screenHeightUnit);
		//GUI.skin.button.fontSize = (int) (btnHeightUnit * screenHeight / screenHeightUnit);

        if (!register)
        {
            backButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(true);
            groupBox.pixelInset = GetScaledPixelInset(456, 342);
            lbUsername.transform.position = new Vector3(0.3f, 0.71f, 0f);
            lbPassword.transform.position = new Vector3(0.3f, 0.53f, 0f);
            lbRetypePassword.gameObject.SetActive(false);
            lbEmail.gameObject.SetActive(false);

            Rect txtUsernameRect = GetScaledRect(txtWidthUnit, txtHeightUnit);
            txtUsernameRect.y = 9.5f * screenHeight / screenHeightUnit;
            txtUsernameRect.x = 14 * screenWidth / screenWidthUnit;
            username = GUI.TextField(txtUsernameRect, username);

            Rect txtPasswordRect = GetScaledRect(txtWidthUnit, txtHeightUnit);
            txtPasswordRect.y = 14.5f * screenHeight / screenHeightUnit;
            txtPasswordRect.x = 14 * screenWidth / screenWidthUnit;
            password = GUI.PasswordField(txtPasswordRect, password, "*"[0]);

            if (loginButton.GetComponent<Button_Controller>().isPressed)
            {
                msg.text = "";
                loginButton.GetComponent<Button_Controller>().isPressed = false;

                if (username == "" || password == "")
                    msg.text = "Please enter all the fields \n";
                else
                {
                    msg.text = "Connecting ... ";
                    WWWForm form = new WWWForm();
                    form.AddField("username", username);
                    form.AddField("password", password);
                    //WWW w = new WWW("http://84.101.189.177:25500/login.php", form);
                    WWW w = new WWW(@"http://hieurl.zapto.org/~hieu/rushgame/Server/php/user.php?action=login", form);
                    StartCoroutine(loginRequest(w));
                }
            }

            if (registerButton.GetComponent<Button_Controller>().isPressed)
            {
                registerButton.GetComponent<Button_Controller>().isPressed = false;
                register = true;
                //keep username but remove password
                password = "";
                msg.text = "";
            }
        }
        else
        {
            loginButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            groupBox.pixelInset = GetScaledPixelInset(720, 342);
            lbUsername.transform.position = new Vector3(0.15f, 0.71f, 0f);
            lbPassword.transform.position = new Vector3(0.15f, 0.53f, 0f);
            lbRetypePassword.gameObject.SetActive(true);
            lbEmail.gameObject.SetActive(true);

            Rect txtUsernameRect = GetScaledRect(txtWidthUnit * 0.8f, txtHeightUnit);
            txtUsernameRect.y = 9.5f * screenHeight / screenHeightUnit;
            txtUsernameRect.x = 7 * screenWidth / screenWidthUnit;
            username = GUI.TextField(txtUsernameRect, username);

            Rect txtPasswordRect = GetScaledRect(txtWidthUnit * 0.8f, txtHeightUnit);
            txtPasswordRect.y = 14.5f * screenHeight / screenHeightUnit;
            txtPasswordRect.x = 7 * screenWidth / screenWidthUnit;
            password = GUI.PasswordField(txtPasswordRect, password, "*"[0]);

            Rect txtEmailRect = GetScaledRect(txtWidthUnit * 0.8f, txtHeightUnit);
            txtEmailRect.y = 9.5f * screenHeight / screenHeightUnit;
            txtEmailRect.x = 25 * screenWidth / screenWidthUnit;
            email = GUI.TextField(txtEmailRect, email);

            Rect txtRePasswordRect = GetScaledRect(txtWidthUnit * 0.8f, txtHeightUnit);
            txtRePasswordRect.y = 14.5f * screenHeight / screenHeightUnit;
            txtRePasswordRect.x = 25 * screenWidth / screenWidthUnit;
            rePassword = GUI.PasswordField(txtRePasswordRect, rePassword, "*"[0]);

            if (backButton.GetComponent<Button_Controller>().isPressed)
            {
                backButton.GetComponent<Button_Controller>().isPressed = false;
                register = false;
                //reset all fields except username
                //username = "";
                password = "";
                email = "";
                rePassword = "";
                msg.text = "";
            }

            if (registerButton.GetComponent<Button_Controller>().isPressed)
            {
                msg.text = "";
                registerButton.GetComponent<Button_Controller>().isPressed = false;

                if (username == "" || email == "" || password == "")
                    msg.text = "Please enter all the fields \n";
                else
                {
                    if (password == rePassword)
                    {
                        msg.text = "Registering ... ";
                        WWWForm form = new WWWForm();
                        form.AddField("username", username);
                        form.AddField("email", email);
                        form.AddField("password", password);
                        //WWW w = new WWW("http://84.101.189.177:25500/register.php", form);
                        WWW w = new WWW("http://hieurl.zapto.org/~hieu/rushgame/Server/php/user.php?action=register", form);
                        StartCoroutine(registerRequest(w));
                    }
                    else
                        msg.text = "Your Password does not match \n";
                }
            }           
        }
	
	}

    IEnumerator loginRequest(WWW w)
    {
        yield return w;
        //Debug.Log(w.text);
        if (w.error == null)
        {            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(w.text);
            XmlNode code = doc.DocumentElement.SelectSingleNode("/response/code");

            if (code.InnerText == "Success")
            {
                //((User)GameObject.Find("User").GetComponent("User")).username = username; //store username for later scenes
                MultiplayerManager.Instance.PlayerName = username;
                PlayerPrefs.SetString("PlayerName", MultiplayerManager.Instance.PlayerName);
                Application.LoadLevel("lobby");
            }
            else
            {
                msg.text = "Wrong username or password";
            }
        }
        else
        {
            msg.text = "Cannot connect to server";
        }
    }
	
	IEnumerator registerRequest(WWW w)
	{
		yield return w;
		if (w.error == null)
		{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(w.text);
            XmlNode code = doc.DocumentElement.SelectSingleNode("/response/code");

            if (code.InnerText == "OK")
            {
                //((User)GameObject.Find("User").GetComponent("User")).username = username; //store username for later scenes
                MultiplayerManager.Instance.PlayerName = username;
                PlayerPrefs.SetString("PlayerName", MultiplayerManager.Instance.PlayerName);
				Application.LoadLevel("lobby");
			}
			else if(code.InnerText == "Fail")
			{
				msg.text = "Cannot create user";
			}
            else if (code.InnerText == "User_Exists")
            {
                msg.text = "Username is already taken";
            }
		}
		else
		{
            msg.text = "Cannot connect to server";
		}
	}	
	
    //Just scale width, height; X & Y will be set after that
	private Rect GetScaledRect(float widthUnit, float heightUnit)
	{
		float scaledW = widthUnit * screenWidth / screenWidthUnit;
		float scaledH = heightUnit * screenHeight / screenHeightUnit;

		return new Rect(0, 0, scaledW, scaledH);
	}

    private Rect GetScaledPixelInset(float width, float height)
    {
        float scaledW = width * screenWidth / stdScreenWidth;
        float scaledH = height * screenHeight / stdScreenHeight;
        float scaledX = -scaledW / 2;
        float scaledY = -scaledH / 2;

        return new Rect(scaledX, scaledY, scaledW, scaledH);
    }

    private void UpdateGUIElementsSize()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        GUIText[] guiTexts = GameObject.FindObjectsOfType<GUIText>();
        //Ok for mobile and device that doesn't change game's screen size
        //If it's not the case, items' fontsize will keep increasing
        foreach (GUIText tempText in guiTexts)
        {
            int tempSize = tempText.fontSize;
            tempText.fontSize = (int)(tempSize * screenHeight / stdScreenHeight);
        }

        GUITexture[] guiTextures = GameObject.FindObjectsOfType<GUITexture>();
        foreach (GUITexture tempTexture in guiTextures)
        {
            float tempW = tempTexture.pixelInset.width;
            float tempH = tempTexture.pixelInset.height;
            tempTexture.pixelInset = GetScaledPixelInset(tempW, tempH);
        }

        backgroundImage.pixelInset = new Rect(0, 0, screenWidth, screenHeight);
    }
}
