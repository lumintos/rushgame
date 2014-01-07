using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

public class Lobby : MonoBehaviour {
    
    //private User user, user2;
    private GUIHelper guiHelper;
    public List<HostData> roomList;
    private RoomGUI roomGUI;

    private bool createRoom , joinRoom;
    public int joinedPlayer;
    private string roomName; //Display for both room list view or user's room view
    public float elapsedTimeDisplayedMsg = 0; //Amount of time that unchanged message remains displayed

    private Vector2 scrollVector;
    private int maxRoomsDisplayed = 4; //not all rooms will be displayed in the list
    private int[] displayedRoomIndex;


	// Use this for initialization
	void Start () {
        displayedRoomIndex = new int[maxRoomsDisplayed];

        guiHelper = GetComponent<GUIHelper>();
        guiHelper.SetText("message", "Welcome");
        //guiHelper.UpdateGUIElementsSize();
        roomGUI = GetComponent<RoomGUI>();
        createRoom = false;
        joinRoom = false;
        joinedPlayer = 0;
        roomName = "Rooms";
        elapsedTimeDisplayedMsg = 0;
        GetUserInfo();
        MultiplayerManager.Instance.RefreshRoomList();
	}
	
	// Update is called once per frame
	void Update () {
        //Update size of all elements in case game screen size changing
        //This is not the case on mobile devices, so comment it
        //guiHelper.UpdateGUIElementsSize();
        
	}

    void OnGUI()
    {
        if (!guiHelper.guiUpdated)
        {
            ColoredGUISkin.Instance.UpdateGuiColors(guiHelper.primaryColor, guiHelper.secondaryColor);
            guiHelper.guiUpdated = true;
        }

        GUI.skin = ColoredGUISkin.Skin;

        GUI.skin.button.fontSize = (int)(guiHelper.btnScaledHeight * guiHelper.fontSizeUnit / (1.5f * guiHelper.btnHeightUnit));
                       
        DrawUserInfo();
        DrawFriendsList();

        //Room functions buttons
        if ((!createRoom && !joinRoom) || (joinRoom && MultiplayerManager.Instance.JoinedRoomFlag == 0)) //Not create and not join room, or in joining process)
        {
            if (joinRoom) // Second case
            {
                guiHelper.SetText("message", "Connecting ...");
                elapsedTimeDisplayedMsg = 0;
            }

            DrawRoomList();
        }        
        else if (joinRoom && MultiplayerManager.Instance.JoinedRoomFlag == -1) //Failed to join
        {
            guiHelper.SetText("message", "Room does not exist!");
            elapsedTimeDisplayedMsg = 0;
            roomName = "Rooms";
            MultiplayerManager.Instance.RefreshRoomList();
            joinRoom = false;
            MultiplayerManager.Instance.JoinedRoomFlag = 2;
            DrawRoomList();
        }
        else if( createRoom || (joinRoom && MultiplayerManager.Instance.JoinedRoomFlag > 0)) //Creator or Joined successfully
        {
            DrawSingleRoom();
        }
    }

    void GetUserInfo()
    {
        string url = "http://hieurl.zapto.org/~hieu/rushgame/Server/php/user.php?action=query_score&username=" + MultiplayerManager.Instance.PlayerName;
        WWW w = new WWW(url);
        StartCoroutine(queryInfoRequest(w));
    }

    IEnumerator queryInfoRequest(WWW w)
    {
        yield return w;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(w.text);

        XmlNode codeNode = doc.DocumentElement.SelectSingleNode("/response/code");
        string code = codeNode.InnerText;

        if (code == "OK")
        {
            XmlNode spiritNode = doc.DocumentElement.SelectSingleNode("/response/user_score/spirit");
            int spirit = int.Parse(spiritNode.InnerText);

            XmlNode maxSpiritNode = doc.DocumentElement.SelectSingleNode("/response/user_score/max_spirit");
            int maxSpirit = int.Parse(maxSpiritNode.InnerText);

            MultiplayerManager.Instance.SetUserInfo(MultiplayerManager.Instance.PlayerName, Network.player, maxSpirit, spirit);
        }
        else
        {
            guiHelper.SetText("message", "Cannot retrieve user's information");
        }        
    }

    /// <summary>
    /// Update to database that this user logged out
    /// </summary>
    IEnumerator logoutRequest(WWW w)
    {
        //user.username = "";
        //user.friendsList.Clear();
        MultiplayerManager.Instance.MyPlayer = null;
        //Back to login scene
        Application.LoadLevel("login");

        yield return w;
        if (w.error == null)
        {
            if (w.text == "Logged out")
            {
                //Reset user
                //user.username = ""; 
                //user.friendsList.Clear();
                //Back to login scene
                Application.LoadLevel("login");
            }
            else
            {
                guiHelper.SetText("message", w.text);
                elapsedTimeDisplayedMsg = 0;
            }
        }
        else
        {
            guiHelper.SetText("message", "ERROR: " + w.error + "\n");
            elapsedTimeDisplayedMsg = 0;
        }
    }

    /// <summary>
    /// Get friends list of this user
    /// </summary>
    /// <param name="w"></param>
    /// <returns></returns>
    IEnumerator getFriendsRequest(WWW w)
    {
        yield return w;
        guiHelper.message.text = "New List";
        elapsedTimeDisplayedMsg = 0;
    }

    /// <summary>
    /// Friends List and refresh button
    /// </summary>
    void DrawFriendsList()
    {
        if (guiHelper.GetButtonPress("ButtonRefreshFriendsList"))
        {
            guiHelper.SetButtonPress("ButtonRefreshFriendsList", false);
            guiHelper.SetText("message", "Refreshing List ... ");
            elapsedTimeDisplayedMsg = 0;
            WWWForm form = new WWWForm();
            form.AddField("username", MultiplayerManager.Instance.MyPlayer.username);
            WWW w = new WWW("http://84.101.189.177:25500/getfriends.php", form);
            StartCoroutine(getFriendsRequest(w));
        }
    }

    /*
    /// <summary>
    /// Display message
    /// </summary>
    void DrawMessage()
    {
        elapsedTimeDisplayedMsg += Time.deltaTime;
        if (elapsedTimeDisplayedMsg < 10)
        {
            Rect msgRect = guiHelper.GetScaledRectFromUnit(10, 3);
            msgRect.x = 19 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
            msgRect.y = 1 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
            GUIStyle msgStyle = new GUIStyle(GUI.skin.label);
            msgStyle.fontStyle = FontStyle.Italic;
            msgStyle.normal.textColor = Color.white;
            msgStyle.alignment = TextAnchor.UpperCenter;
            msgStyle.wordWrap = true;
            msgStyle.fontSize = (int)guiHelper.screenHeight * guiHelper.fontSizeUnit / guiHelper.screenHeightUnit;
            GUI.Label(msgRect, guiHelper.message.text, msgStyle);
        }
    }
    */

    void DrawUserInfo()
    {
        //Set username and spirit
        guiHelper.SetText("username", MultiplayerManager.Instance.PlayerName);

        string spirit = MultiplayerManager.Instance.MyPlayer.spirit.ToString()
                        + "/" + MultiplayerManager.Instance.MyPlayer.maxSpirit.ToString();
        guiHelper.SetText("spiritNumber", spirit);

        //Logout button
        if (guiHelper.GetButtonPress("ButtonLogout"))
        {
            guiHelper.SetButtonPress("ButtonLogout", false);
            guiHelper.SetText("message", "Logging out ... ");
            elapsedTimeDisplayedMsg = 0;

            if (createRoom || joinRoom)
            {
                joinRoom = false;
                createRoom = false;
                PlayerPrefs.DeleteKey("PlayerName");
                MultiplayerManager.Instance.LeaveRoom(0);
            }

            WWWForm form = new WWWForm();
            form.AddField("username", MultiplayerManager.Instance.PlayerName);
            WWW w = new WWW("http://84.101.189.177:25500/logout.php", form);
            StartCoroutine(logoutRequest(w));
        }
    }


    void DrawRoomList()
    {
        guiHelper.ChangeTexture("FrameRoom", "UI/title-frame-room-list2");
        guiHelper.SetActiveGUIElement("ButtonRefreshRoomsList", true);
        guiHelper.SetActiveGUIElement("ButtonCreateRoom", true);
        guiHelper.SetActiveGUIElement("ButtonStartGame", false);
        guiHelper.SetActiveGUIElement("ButtonBack", false);
        guiHelper.SetActiveGUIElement("PlayerOneFrame", false);
        guiHelper.SetActiveGUIElement("PlayerOneName", false);
        guiHelper.SetActiveGUIElement("PlayerTwoFrame", false);
        guiHelper.SetActiveGUIElement("PlayerTwoName", false);
        guiHelper.SetActiveGUIElement("VSTexture", false);

        if (MultiplayerManager.Instance.RoomList != null
            && MultiplayerManager.Instance.RoomList.Count > 0) // There is a list
        {
            if (roomList == null) // Our list is refreshing
            {
                roomList = MultiplayerManager.Instance.RoomList;

                //Then we must choose new rooms to display
                int startRoomIndex = Random.Range(0, roomList.Count - 1); //choose 1 random room from list to start
                for (int i = 0; i < roomList.Count && i < maxRoomsDisplayed; i++)
                {
                    displayedRoomIndex[i] = startRoomIndex % roomList.Count;
                    startRoomIndex++;
                }
            }
        }
        else
            roomList = null;

        //Arrange rooms list
        if (roomList != null)
        {
            for (int i = 0; i < roomList.Count && i < maxRoomsDisplayed; i++)
            {
                Rect buttonRect = guiHelper.GetScaledRectFromUnit(12, 5);
                buttonRect.x = (4 + (i % 2) * (12 + 4)) * guiHelper.screenWidth / guiHelper.screenWidthUnit;
                buttonRect.y = (8 + (i / 2) * (5 + 2)) * guiHelper.screenHeight / guiHelper.screenHeightUnit;
                GUI.skin.button.fontSize = (int)(guiHelper.message.fontSize * 1.5f);

                if (GUI.Button(buttonRect, roomList[displayedRoomIndex[i]].gameName))
                {
                    if (MultiplayerManager.Instance.MyPlayer.spirit < GameConstants.minSpiritJoinRoom)
                    {
                        guiHelper.SetText("message", "Not enough Spirit to join room");
                        elapsedTimeDisplayedMsg = 0;
                    }
                    else
                    {
                        roomName = roomList[displayedRoomIndex[i]].gameName;
                        MultiplayerManager.Instance.JoinRoom(roomList[i]);
                        joinRoom = true;
                    }
                }
            }
        }

        if (guiHelper.GetButtonPress("ButtonRefreshRoomsList"))
        {
            guiHelper.SetButtonPress("ButtonRefreshRoomsList", false);

            MultiplayerManager.Instance.RefreshRoomList();
            roomList = null;
            //displayedRoomIndex[0] = -1;
            guiHelper.SetText("message", "New room list");
            elapsedTimeDisplayedMsg = 0;
            return;
        }


        if (guiHelper.GetButtonPress("ButtonCreateRoom"))
        {
            guiHelper.SetButtonPress("ButtonCreateRoom", false);
            if (MultiplayerManager.Instance.MyPlayer.spirit < GameConstants.minSpiritJoinRoom)
            {
                guiHelper.SetText("message", "Not enough Spirit to create room");
                elapsedTimeDisplayedMsg = 0;
            }
            else
            {
                //call network manager to register room here
                roomName = MultiplayerManager.Instance.PlayerName + "'s Room";
                //MultiplayerManager.Instance.MyPlayer.team = 1;
                //networkManager.StartServer(roomName);
                MultiplayerManager.Instance.CreateRoom(roomName, 2);
                guiHelper.SetText("message", "New room created ... ");
                elapsedTimeDisplayedMsg = 0;
                joinedPlayer = 1;
                createRoom = true;
            }
            return;
        }
    }


    void DrawSingleRoom()
    {
        Debug.Log("FLAAAAG: " + MultiplayerManager.Instance.JoinedRoomFlag.ToString());
        guiHelper.ChangeTexture("FrameRoom", "UI/frame-room");
        guiHelper.SetActiveGUIElement("ButtonRefreshRoomsList", false);
        guiHelper.SetActiveGUIElement("ButtonCreateRoom", false);
        guiHelper.SetActiveGUIElement("ButtonStartGame", false);
        guiHelper.SetActiveGUIElement("ButtonBack", true);
        guiHelper.SetActiveGUIElement("PlayerOneFrame", true);
        guiHelper.SetActiveGUIElement("PlayerOneName", true);
        guiHelper.SetActiveGUIElement("PlayerTwoFrame", true);
        guiHelper.SetActiveGUIElement("PlayerTwoName", true);
        guiHelper.SetActiveGUIElement("VSTexture", true);

        if (MultiplayerManager.Instance.JoinedRoomFlag == 1) // If first frame after joining room
        {
            guiHelper.SetText("message", "Joined Room");
            elapsedTimeDisplayedMsg = 0;
            MultiplayerManager.Instance.JoinedRoomFlag = 2; // then reset to Idle, this help message to disappear
        }

        roomGUI.DisplayPlayers();

        if (guiHelper.GetButtonPress("ButtonBack"))
        {
            guiHelper.SetButtonPress("ButtonBack", false);
            joinedPlayer = 0;
            roomName = "Rooms";
            createRoom = false;
            joinRoom = false;
            guiHelper.SetText("message", "Backed to room list");
            elapsedTimeDisplayedMsg = 0;
            MultiplayerManager.Instance.LeaveRoom(0);
            MultiplayerManager.Instance.RefreshRoomList();
            return;
        }

        if (Network.isServer)
        {
            guiHelper.SetActiveGUIElement("ButtonStartGame", true);

            if (guiHelper.GetButtonPress("ButtonStartGame"))
            {
                guiHelper.SetButtonPress("ButtonStartGame", false);
                //if (joinedPlayer < 2)
                if (MultiplayerManager.Instance.PlayersList.Count < 2)
                {
                    guiHelper.SetText("message", "There must be 2 players to start game!");
                    elapsedTimeDisplayedMsg = 0;
                }
                else
                {
                    guiHelper.SetText("message", "Starting ... ");
                    elapsedTimeDisplayedMsg = 0;
                    MultiplayerManager.Instance.LaunchGame("map");
                }

                return;
            }
        }
        else
        {
            guiHelper.SetText("message", "Waiting for host to start game");
            elapsedTimeDisplayedMsg = 0;
        }


        // If this is Player 2, check if room creator has left
        // If room creator has left, leave room, too.
        if (joinRoom && MultiplayerManager.Instance.needToLeave)
        {
            guiHelper.SetText("message", "Disconnected from host");
            elapsedTimeDisplayedMsg = 0;
            joinRoom = false; // reset to join other rooms
            MultiplayerManager.Instance.needToLeave = false;
            MultiplayerManager.Instance.RefreshRoomList();
            return;
        }
    }
}
