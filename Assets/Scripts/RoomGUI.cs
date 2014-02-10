using UnityEngine;
using System.Collections;

public class RoomGUI : MonoBehaviour {


    private GUIHelper guiHelper = null;

    private string username1 = "";
    private string username2 = "";
    private string waitingMsg = "Waiting for P2";

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

        //Player 1
        if (username1 == MultiplayerManager.Instance.PlayerName)
        {
            guiHelper.ChangeTexture("PlayerOneFrame", "UI/frame-ninja-player-host-avatar");
        }
        else
        {
            guiHelper.ChangeTexture("PlayerOneFrame", "UI/frame-ninja-player-guest-avatar");
        }

        //Player 2        
        if (username2 != waitingMsg)
        {
            if (username2 == MultiplayerManager.Instance.PlayerName)
            {
                guiHelper.ChangeTexture("PlayerTwoFrame", "UI/frame-astro-player-host-avatar");
            }
            else
            {
                guiHelper.ChangeTexture("PlayerTwoFrame", "UI/frame-astro-player-guest-avatar");
            }
        }
        else
        {
            guiHelper.ChangeTexture("PlayerTwoFrame", "UI/frame-player-waiting-avatar2"); //Wait for other player to join
        }
    }
}
