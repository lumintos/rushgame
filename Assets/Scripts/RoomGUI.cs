using UnityEngine;
using System.Collections;

public class RoomGUI : MonoBehaviour {


    private GUIHelper guiHelper = null;

    private string username1 = "";
    private string username2 = "";
    private string waitingMsg = "Waiting for P2";

    public Texture2D waitTex, avatarP1, avatarP2;

    public void SetUserName()
    {
        if (MultiplayerManager.Instance.PlayersList.Count == 1)
        {
            username1 = MultiplayerManager.Instance.PlayersList[0].username;
                //+ " - LvL: " + MultiplayerManager.Instance.PlayersList[0].level;
            username2 = waitingMsg;
        }
        else
        {
            foreach (RUSHPlayer tempplayer in MultiplayerManager.Instance.PlayersList)
            {
                if (tempplayer.team == 1)
                {
                    username1 = tempplayer.username;// +" - LvL: " + tempplayer.level;
                }
                else
                {
                    username2 = tempplayer.username;// +" - LvL: " + tempplayer.level;
                }
            }
        }

        guiHelper.SetText("PlayerOneName", username1);
        guiHelper.SetText("PlayerTwoName", username2);

    }

    public void DisplayPlayers()
    {
        if (guiHelper == null)
        {
            guiHelper = GetComponent<GUIHelper>();
           // guiHelper.UpdateGUIElementsSize();
        }

        SetUserName();
        Texture2D texP1, texP2;
        if (MultiplayerManager.Instance.PlayersList.Count < 2)
        {
            texP2 = waitTex;
        }
        else
        {
            texP2 = avatarP2;
        }

        texP1 = avatarP1;

        //Player 1
        if (username1 == MultiplayerManager.Instance.PlayerName)
        {
            guiHelper.ChangeTexture("PlayerOneFrame", "UI/frame-player-waiting-avatar");
        }
        else
        {
            guiHelper.ChangeTexture("PlayerOneFrame", "UI/frame-player-waiting-avatar2");
        }

        //Player 2        
        if (username2 != waitingMsg)
        {
            if (username2 == MultiplayerManager.Instance.PlayerName)
            {
                guiHelper.ChangeTexture("PlayerTwoFrame", "UI/frame-player-waiting-avatar");
            }
            else
            {
                guiHelper.ChangeTexture("PlayerTwoFrame", "UI/frame-player-waiting-avatar2");
            }
        }
        else
        {
            guiHelper.ChangeTexture("PlayerTwoFrame", "UI/frame-player-waiting-avatar2"); //There will be another texture for waiting
        }
    }



}
