using UnityEngine;
using System.Collections;

public class GUIHelper : MonoBehaviour {

    public GUIText message;

    [HideInInspector]
    public float screenWidth, screenHeight;
    [HideInInspector]
    public float txtScaledHeight, btnScaledHeight;
    [HideInInspector]
    public float txtScaledWidth, btnScaledWidth;
    
    private float stdScreenWidth, stdScreenHeight;
    
    public int screenWidthUnit = 48; // aspect ratio 16:9    
    public int screenHeightUnit = 27;
    public int txtWidthUnit = 14; //All the unit is taken from Layout.xlsx
    public int txtHeightUnit = 2;
    public int btnWidthUnit = 8;
    public int btnHeightUnit = 4;
    public int fontSizeUnit = 1;
    [HideInInspector]
    public bool guiUpdated = false;
    public float elapsedTimeDisplayedMsg = 0; //Amount of time that unchanged message remains displayed
    public float displayDuration = 10; //seconds

    public GUITexture[] guiButtons;
    public GUIText[] guiTexts;
    public GUITexture[] guiFrames;

    public GUITexture backgroundImage;
    public Color primaryColor, secondaryColor;
    public Color[] textColor, outlineColor;

	// Use this for initialization
	void Start () {
        stdScreenHeight = 1080;
        stdScreenWidth = 1920;
        screenWidth = screenHeight = 0;
        UpdateGUIElementsSize();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (message != null)
        {
            elapsedTimeDisplayedMsg += Time.deltaTime;
            if (elapsedTimeDisplayedMsg >= displayDuration)
            {
                message.text = "";
                elapsedTimeDisplayedMsg = 0;
            }
        }

        if (screenWidth != Screen.width) //screen changes size
        {
            UpdateGUIElementsSize();
        }
    }

    /// <summary>
    /// Update size of all elements in case game screen size changing
    /// </summary>
    public void UpdateGUIElementsSize()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        
        btnScaledHeight = (float)btnHeightUnit * screenHeight / screenHeightUnit;
        btnScaledWidth = (float)btnWidthUnit * screenWidth / screenWidthUnit;

        if(backgroundImage != null)
            backgroundImage.pixelInset = new Rect(0, 0, screenWidth, screenHeight);
        //Ok for mobile and device that doesn't change game's screen size
        //If it's not the case, items' fontsize will keep increasing
        

        //update size for frames and buttons
        if (guiFrames != null)
        {
            foreach (GUITexture tempFrame in guiFrames)
            {
                float tempW = tempFrame.pixelInset.width;
                float tempH = tempFrame.pixelInset.height;
                tempFrame.pixelInset = GetScaledPixelInset(tempW, tempH);
            }
        }

        if (guiButtons != null)
        {
            foreach (GUITexture tempButton in guiButtons)
            {
                float tempW = tempButton.pixelInset.width;
                float tempH = tempButton.pixelInset.height;
                tempButton.pixelInset = GetScaledPixelInset(tempW, tempH);
            }
        }

        if (guiTexts != null)
        {
            foreach (GUIText tempText in guiTexts)
            {
                int tempSize = tempText.fontSize;
                tempText.fontSize = (int)(tempSize * screenHeight / stdScreenHeight);
            }
        }

    }

    public Rect GetScaledRectFromUnit(float widthUnit, float heightUnit)
    {
        float scaledW = widthUnit * screenWidth / screenWidthUnit;
        float scaledH = heightUnit * screenHeight / screenHeightUnit;

        return new Rect(0, 0, scaledW, scaledH);
    }

    public Rect GetScaledPixelInset(float width, float height)
    {
        float scaledW = width * screenWidth / stdScreenWidth;
        float scaledH = height * screenHeight / stdScreenHeight;
        float scaledX = -scaledW / 2;
        float scaledY = -scaledH / 2;


        return new Rect(scaledX, scaledY, scaledW, scaledH);
    }

    public void SetActiveGUIElement(string name, bool isActive)
    {
        foreach (GUIText tempText in guiTexts)
        {
            if (tempText.name == name)
            {
                tempText.gameObject.SetActive(isActive);
                return;
            }
        }

        foreach (GUITexture tempButton in guiButtons)
        {
            if (tempButton.name == name)
            {
                tempButton.gameObject.SetActive(isActive);
                return;
            }
        }

        foreach (GUITexture tempFrame in guiFrames)
        {
            if (tempFrame.name == name)
            {
                tempFrame.gameObject.SetActive(isActive);
                return;
            }
        }
    }

    public bool GetButtonPress(string name)
    {
        foreach (GUITexture tempButton in guiButtons)
        {
            if(tempButton.gameObject.name == name)
                return tempButton.gameObject.GetComponent<Button_Controller>().isPressed;
        }
        return false;
    }

    /// <summary>
    /// Usually used to reset button press status
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isPressed">Usually is false to reset</param>
    public void SetButtonPress(string name, bool isPressed)
    {
        foreach (GUITexture tempButton in guiButtons)
        {
            if (tempButton.gameObject.name == name)
               tempButton.gameObject.GetComponent<Button_Controller>().isPressed = isPressed;
        }
    }

    public void ChangeTexture(string guiTexName, string texPath)
    {
        foreach (GUITexture tempTex in guiFrames)
        {
            if (tempTex.gameObject.name == guiTexName)
            {
                tempTex.texture = (Texture2D)Resources.Load(texPath);
                break;
            }
        }
    }

    public void ChangeButtonTexture(string buttonName, int index)
    {
        foreach (GUITexture tempButton in guiButtons)
        {
            if (tempButton.gameObject.name == buttonName)
            {
                tempButton.GetComponent<Button_Controller>().ChangeTexture(index);
                break;
            }
        }
    }

    public void SetText(string guiTextname, string text)
    {
        foreach (GUIText tempText in guiTexts)
        {
            if (tempText.gameObject.name == guiTextname)
            {
                tempText.text = text;
                break;
            }
        }
        if (guiTextname == "message")
            elapsedTimeDisplayedMsg = 0;
    }
}
