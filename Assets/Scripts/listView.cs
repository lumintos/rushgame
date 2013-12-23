using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class listView : MonoBehaviour
{

    Vector2 scrollPosition = Vector2.zero;
    private bool hasUpdatedGui = false;
    public List<Color> primaryColors;
    public List<Color> secondaryColors;

    void OnGUI()
    {
        string[] listItems =
    {
    "Hello world,",
    "this",
    "is a",
    "very",
    "very",
    "very",
    "very",
    "very",
    "very",
    "long",
    "list.",
    };
        if (!hasUpdatedGui)
        {
            ColoredGUISkin.Instance.UpdateGuiColors(primaryColors[0], secondaryColors[0]);
            hasUpdatedGui = true;
        }
        GUI.skin = ColoredGUISkin.Skin;


        GUILayout.BeginArea(new Rect(0f, 0f, 300f, 200f), GUI.skin.window);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
       // GUILayout.BeginVertical(GUI.skin.box);
        int i = 0;
        foreach (string item in listItems)
        {
            //GUILayout.Button(item, GUI.skin.button, GUILayout.Height(30), GUILayout.Width(90));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(item, GUI.skin.button, GUILayout.Height(30), GUILayout.Width(90)))
                Debug.Log(item);
            if (GUILayout.Button(item, GUI.skin.button, GUILayout.Height(30), GUILayout.Width(90)))
                Debug.Log(item);
            if (GUILayout.Button(item, GUI.skin.button, GUILayout.Height(30), GUILayout.Width(90)))
                Debug.Log(item);
            GUILayout.EndHorizontal();
            i++;
        }

       // GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}