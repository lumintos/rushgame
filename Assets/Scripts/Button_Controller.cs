using UnityEngine;
using System.Collections;

public class Button_Controller : MonoBehaviour
{

    public string button_name;
    public GUITexture guitex;
    public Texture2D normalTex;
    public Texture2D pressedTex;
    [HideInInspector]
    public bool isTouchDown, isPressed;

    // Use this for initialization
    void Start()
    {
        isTouchDown = false;
        isPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
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
}
