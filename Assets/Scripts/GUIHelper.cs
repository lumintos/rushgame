using UnityEngine;
using System.Collections;

public class GUIHelper : MonoBehaviour {

    public string message = "";

    [HideInInspector]
    public float screenWidth, screenHeight;
    [HideInInspector]
    public float txtScaledHeight, btnScaledHeight;
    [HideInInspector]
    public float txtScaledWidth, btnScaledWidth;

    
    public int screenWidthUnit = 48; // aspect ratio 16:9    
    public int screenHeightUnit = 27;
    public int txtWidthUnit = 14; //All the unit is taken from Layout.xlsx
    public int txtHeightUnit = 2;
    public int btnWidthUnit = 8;
    public int btnHeightUnit = 4;
    public int fontSizeUnit = 1;
    [HideInInspector]
    public bool guiUpdated = false;

    public GUITexture backgroundImage;
    public Color primaryColor, secondaryColor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Update size of all elements in case game screen size changing
    /// </summary>
    public void UpdateGUIElementsSize()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        //Calculate real size of textbox and button
        txtScaledHeight = (float)txtHeightUnit * screenHeight / screenHeightUnit;
        txtScaledWidth = (float)txtWidthUnit * screenWidth / screenWidthUnit;

        btnScaledHeight = (float)btnHeightUnit * screenHeight / screenHeightUnit;
        btnScaledWidth = (float)btnWidthUnit * screenWidth / screenWidthUnit;

        if(backgroundImage)
            backgroundImage.pixelInset = new Rect(0, 0, screenWidth, screenHeight);
    }

    public Rect GetScaledRectFromUnit(float widthUnit, float heightUnit)
    {
        float scaledW = widthUnit * screenWidth / screenWidthUnit;
        float scaledH = heightUnit * screenHeight / screenHeightUnit;

        return new Rect(0, 0, scaledW, scaledH);
    }
}
