using UnityEngine;
using System.Collections;

public class Button_Controller : MonoBehaviour
{

    public string button_name;
    public GUITexture guitex;
    public Texture2D normalTex;
    public Texture2D pressedTex;
    public Texture2D[] allNormalTex;
    public Texture2D[] allPressedTex;

    [HideInInspector]
    public bool isTouchDown, isPressed;

    // Use this for initialization
    void Start()
    {
        isTouchDown = false;
        isPressed = false;
        if (allNormalTex.Length > 0 && allPressedTex.Length > 0)
        {
            normalTex = allNormalTex[0];
            pressedTex = allPressedTex[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        GUI.depth = 0;
        isPressed = false;
        bool hitTest = false;
        foreach (Touch touch in Input.touches)
        {
            hitTest = guitex.HitTest(touch.position);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                if (hitTest)
                {
                    guitex.texture = pressedTex;
                    isTouchDown = true;
                }
                else
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        guitex.texture = normalTex;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (hitTest)
                {
                    guitex.texture = normalTex;
                    if (isTouchDown)
                        isPressed = true;
                    isTouchDown = false;
                }
            }
        }
        hitTest = guitex.HitTest(Input.mousePosition);
        if (hitTest)
        {
            if (Input.GetMouseButton(0))
            {
                guitex.texture = pressedTex;
                isTouchDown = true;
                isPressed = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                guitex.texture = normalTex;
                if (isTouchDown)
                    isPressed = true;
                isTouchDown = false;
            }
        }
    }

    /// <summary>
    /// For button that have many textures regarding to different contexts.
    /// </summary>
    /// <param name="index">Index 0 is the default texture</param>
    public void ChangeTexture(int index)
    {
        normalTex = allNormalTex[index];
        pressedTex = allPressedTex[index];

        //Most of the time, this function is call after the button is release, 
        //the texture set at release does not correspond to the new status of the button, we need to update
        if (isTouchDown) //just in case if there is button change status on touch down instead of press
            guitex.texture = pressedTex;
        else
            guitex.texture = normalTex;
    }
}
