using UnityEngine;
using System.Collections;

public class RoomGUI : MonoBehaviour {


    private GUIHelper guiHelper = null;

    private string username1 = "";
    private string username2 = "";
    private string waitingMsg = "Waiting for P2...";

    private Texture2D texP1, texP2;
    public Texture2D waitTex, avatarP1, avatarP2;

    public void SetUserName()
    {
        if (MultiplayerManager.Instance.PlayersList.Count == 1)
        {
            username1 = MultiplayerManager.Instance.PlayersList[0].username
                + " - LvL: " + MultiplayerManager.Instance.PlayersList[0].level;
            username2 = waitingMsg;
        }
        else
        {
            foreach (RUSHPlayer tempplayer in MultiplayerManager.Instance.PlayersList)
            {
                if (tempplayer.team == 1)
                {
                    username1 = tempplayer.username + " - LvL: " + tempplayer.level;
                }
                else
                {
                    username2 = tempplayer.username + " - LvL: " + tempplayer.level;
                }
            }
        }

    }

    public void DisplayPlayers()
    {
        if (guiHelper == null)
        {
            guiHelper = GetComponent<GUIHelper>();
            guiHelper.UpdateGUIElementsSize();
        }
        /*
        if (!guiHelper.guiUpdated)
        {
            ColoredGUISkin.Instance.UpdateGuiColors(guiHelper.primaryColor, guiHelper.secondaryColor);
            guiHelper.guiUpdated = true;
        }

        GUI.skin = ColoredGUISkin.Skin;

        GUI.skin.button.fontSize = (int)(guiHelper.btnScaledHeight * guiHelper.fontSizeUnit / (1.5f * guiHelper.btnHeightUnit));
        */

        SetUserName();
        if (MultiplayerManager.Instance.PlayersList.Count < 2)
        {
            texP2 = waitTex;
        }
        else
        {
            texP2 = avatarP2;
        }

        texP1 = avatarP1;

        DrawPlayerLeft();
        DrawPlayerRight();

        //VS text
        Rect labelRect = guiHelper.GetScaledRectFromUnit(4, 4);
        labelRect.x = 16 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        labelRect.y = 18 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = new Color(248f / 255, 160f / 255, 61f / 255, 1);
        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.wordWrap = true;
        labelStyle.fontSize = (int)guiHelper.screenHeight * guiHelper.fontSizeUnit * 2 / guiHelper.screenHeightUnit;
        GUI.Label(labelRect, "VS", labelStyle);
        
    }

    void DrawPlayerLeft()
    {
        //Player 1
        Rect playerAvatarRect = guiHelper.GetScaledRectFromUnit(14, 11);
        playerAvatarRect.x = 3 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        playerAvatarRect.y = 7 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

        RectOffset padding = GUI.skin.box.padding;
        Rect texRect = new Rect(padding.top, padding.top,
            playerAvatarRect.width - padding.left,
            playerAvatarRect.height - padding.top - padding.bottom);
        GUI.Box(playerAvatarRect, "");

        GUI.BeginGroup(playerAvatarRect);
        GUI.DrawTexture(texRect, texP1, ScaleMode.ScaleAndCrop);
        GUI.EndGroup();

        Rect usernameRect = guiHelper.GetScaledRectFromUnit(12, 2);
        usernameRect.x = 4 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        usernameRect.y = 19 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        GUIStyle usernameStyle = new GUIStyle(GUI.skin.label);
        usernameStyle.fontStyle = FontStyle.Bold;
        usernameStyle.normal.textColor = Color.white;
        usernameStyle.alignment = TextAnchor.UpperCenter;
        usernameStyle.wordWrap = true;
        usernameStyle.fontSize = (int)guiHelper.screenHeight * guiHelper.fontSizeUnit / guiHelper.screenHeightUnit;
        GUI.Label(usernameRect, username1, usernameStyle);
    }

    void DrawPlayerRight()
    {
        //Player 2
        Rect playerAvatarRect = guiHelper.GetScaledRectFromUnit(14, 11);
        playerAvatarRect.x = 19 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        playerAvatarRect.y = 7 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        
        RectOffset padding = GUI.skin.box.padding; 
        Rect texRect = new Rect(padding.top, padding.top,
            playerAvatarRect.width - padding.left,
            playerAvatarRect.height - padding.top - padding.bottom);

        GUI.Box(playerAvatarRect, "");

        GUI.BeginGroup(playerAvatarRect);
        GUI.DrawTexture(texRect, texP2, ScaleMode.ScaleAndCrop);
        GUI.EndGroup();
        
        Rect usernameRect = guiHelper.GetScaledRectFromUnit(12, 2);
        usernameRect.x = 20 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        usernameRect.y = 19 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        GUIStyle usernameStyle = new GUIStyle(GUI.skin.label);
        usernameStyle.fontStyle = FontStyle.Bold;
        usernameStyle.normal.textColor = Color.white;
        usernameStyle.alignment = TextAnchor.UpperCenter;
        usernameStyle.wordWrap = true;
        usernameStyle.fontSize = (int)guiHelper.screenHeight * guiHelper.fontSizeUnit / guiHelper.screenHeightUnit;

        GUI.Label(usernameRect, username2, usernameStyle);
        
    }

}
