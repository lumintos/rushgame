using UnityEngine;
using System.Collections;

public class Lobby : MonoBehaviour {
    
    //private User user, user2;
    private GUIHelper guiHelper;
    private HostData[] roomList;
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
        guiHelper.UpdateGUIElementsSize();
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
        guiHelper.UpdateGUIElementsSize();
        
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

        if (guiHelper.message != "")//There is error message
        {
            DrawMessage();
        }
        
        DrawFriendsList();
        DrawUserInfo();

        //Room functions buttons

        if ((!createRoom && !joinRoom) || (joinRoom && MultiplayerManager.Instance.JoinedRoomFlag == 0)) //Not create and not join room, or in joining process)
        {
            if (joinRoom) // Second case
            {
                guiHelper.message = "Connecting ...";
                elapsedTimeDisplayedMsg = 0;
            }

            DrawRoomList();
        }        
        else if (joinRoom && MultiplayerManager.Instance.JoinedRoomFlag == -1) //Failed to join
        {
            guiHelper.message = "Room does not exist!";
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
        //TODO: Query from Database, parse returned result into user info and pass to MultiplayerManager
        int dummyLevel = 10;
        int dummySpirit = 1000;
        MultiplayerManager.Instance.SetUserInfo(MultiplayerManager.Instance.PlayerName, Network.player, dummyLevel, dummySpirit);
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
                guiHelper.message = w.text;
                elapsedTimeDisplayedMsg = 0;
            }
        }
        else
        {
            guiHelper.message = "ERROR: " + w.error + "\n";
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
        guiHelper.message = "New List";
        elapsedTimeDisplayedMsg = 0;
    }

    /// <summary>
    /// Friends List and refresh button
    /// </summary>
    void DrawFriendsList()
    {
        Rect friendGroupRect = guiHelper.GetScaledRectFromUnit(10, 17);
        friendGroupRect.x = 37 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        friendGroupRect.y = 5 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        GUI.Box(friendGroupRect, "Friends List");

        Rect btnTemptRect = guiHelper.GetScaledRectFromUnit(8, 3);
        btnTemptRect.x = 38 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        btnTemptRect.y = 23 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

        if (GUI.Button(btnTemptRect, "Refresh Friend List"))
        {
            guiHelper.message = "Refreshing List ... ";
            elapsedTimeDisplayedMsg = 0;
            WWWForm form = new WWWForm();
            form.AddField("username", MultiplayerManager.Instance.MyPlayer.username);
            WWW w = new WWW("http://84.101.189.177:25500/getfriends.php", form);
            StartCoroutine(getFriendsRequest(w));
        }
    }

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
            GUI.Label(msgRect, guiHelper.message, msgStyle);
        }

    }

    void DrawUserInfo()
    {

        //Bounding box
        Rect userinfoGroupRect = guiHelper.GetScaledRectFromUnit(11, 3);
        userinfoGroupRect.x = 36 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        userinfoGroupRect.y = 0.75f * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        
        GUI.Box(userinfoGroupRect, "");//, GUI.skin.box);

        //Username
        Rect userinfoRect = guiHelper.GetScaledRectFromUnit(6, 2);
        userinfoRect.x = 37 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        userinfoRect.y = 1 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

        GUIStyle tempLabelStyle = new GUIStyle(GUI.skin.label);
        tempLabelStyle.normal.textColor = Color.white;
        tempLabelStyle.fontSize = (int)guiHelper.screenHeight * guiHelper.fontSizeUnit / guiHelper.screenHeightUnit;
        GUI.Label(userinfoRect, MultiplayerManager.Instance.PlayerName, tempLabelStyle);

        //Spirit and other info
        userinfoRect.y = 2 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        tempLabelStyle.fontStyle = FontStyle.Italic;
        tempLabelStyle.fontSize = (int)guiHelper.screenHeight * guiHelper.fontSizeUnit / guiHelper.screenHeightUnit;
        GUI.Label(userinfoRect, "Spirit: " + MultiplayerManager.Instance.MyPlayer.spirit, tempLabelStyle);

        //Logout button
        Rect btnLogoutRect = guiHelper.GetScaledRectFromUnit(2.5f, 2.5f);
        btnLogoutRect.x = 44 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        btnLogoutRect.y = 1 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

        if (GUI.Button(btnLogoutRect, "Logout"))
        {
            guiHelper.message = "Logging out ... ";
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

    void DoWindowRoomList(int windowsID)
    {
        if (MultiplayerManager.Instance.RoomList != null
            && MultiplayerManager.Instance.RoomList.Length > 0) // There is a list
        {
            if (roomList == null) // Our list is refreshing
            {
                roomList = MultiplayerManager.Instance.RoomList;

                //Then we must choose new rooms to display
                int startRoomIndex = Random.Range(0, roomList.Length - 1); //choose 1 random room from list to start
                for (int i = 0; i < roomList.Length && i < maxRoomsDisplayed; i++)
                {
                    displayedRoomIndex[i] = startRoomIndex % roomList.Length;
                    startRoomIndex++;
                }
            }       
        }
        else
            roomList = null;

        if (roomList != null)
        {
            Rect roomListRect = guiHelper.GetScaledRectFromUnit(28, 14);
            roomListRect.x = 3 * guiHelper.screenWidth / guiHelper.screenWidthUnit; //in relation to the parent window
            roomListRect.y = 2 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

            //GUILayout.BeginVertical();
            //scrollVector = GUILayout.BeginScrollView(scrollVector);

            for (int i = 0; i < roomList.Length && i < maxRoomsDisplayed; i++)
            {
                Rect buttonRect = guiHelper.GetScaledRectFromUnit(12, 5);
                buttonRect.x = (3 + (i%2) * (12 + 4))  * guiHelper.screenWidth / guiHelper.screenWidthUnit;
                buttonRect.y = (2 + (i / 2) * (5 + 2)) * guiHelper.screenHeight / guiHelper.screenHeightUnit;

                if (GUI.Button(buttonRect, roomList[displayedRoomIndex[i]].gameName))
                {
                    roomName = roomList[displayedRoomIndex[i]].gameName;
                    MultiplayerManager.Instance.JoinRoom(roomList[i]);
                    joinRoom = true;
                }
            }
            //GUILayout.EndScrollView();
            //GUILayout.EndVertical();
        }
    }

    void DrawRoomList() 
    {
        Rect roomGroupRect = guiHelper.GetScaledRectFromUnit(34, 17);
        roomGroupRect.x = 1 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        roomGroupRect.y = 5 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

        GUI.Window(0, roomGroupRect, DoWindowRoomList, "Rooms List");
        
        Rect btnTemptRect = guiHelper.GetScaledRectFromUnit(8, 3);
        btnTemptRect.y = 23 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        btnTemptRect.x = 9 * guiHelper.screenWidth / guiHelper.screenWidthUnit;

        

        if (GUI.Button(btnTemptRect, "Refresh Room List"))
        {
            MultiplayerManager.Instance.RefreshRoomList();
            roomList = null;
            //displayedRoomIndex[0] = -1;
            guiHelper.message = "New room list";
            elapsedTimeDisplayedMsg = 0;
        }


        btnTemptRect.x = 19 * guiHelper.screenWidth / guiHelper.screenWidthUnit;

        if (GUI.Button(btnTemptRect, "Create Room"))
        {
            //call network manager to register room here
            roomName = MultiplayerManager.Instance.PlayerName + "'s Room";
            //MultiplayerManager.Instance.MyPlayer.team = 1;
            //networkManager.StartServer(roomName);
            MultiplayerManager.Instance.CreateRoom(roomName, 2);
            guiHelper.message = "New room created ... ";
            elapsedTimeDisplayedMsg = 0;
            joinedPlayer = 1;
            createRoom = true;
        }
    }

    void DoWindowSingleRoom(int windowsID)
    {
        roomGUI.DisplayPlayers();
    }

    void DrawSingleRoom()
    {
        if (MultiplayerManager.Instance.JoinedRoomFlag == 1) // If first frame after joining room
        {
            guiHelper.message = "Joined Room";
            elapsedTimeDisplayedMsg = 0;
            MultiplayerManager.Instance.JoinedRoomFlag = 2; // then reset to Idle, this help message to disappear
        }

        Rect roomGroupRect = guiHelper.GetScaledRectFromUnit(34, 17);
        roomGroupRect.x = 1 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        roomGroupRect.y = 5 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

        GUI.Window(0, roomGroupRect, DoWindowSingleRoom, roomName);

        Rect btnTemptRect = guiHelper.GetScaledRectFromUnit(8, 3);
        btnTemptRect.y = 23 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        btnTemptRect.x = 9 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        

        if (GUI.Button(btnTemptRect, "Back"))
        {
            joinedPlayer = 0;
            roomName = "Rooms";
            createRoom = false;
            joinRoom = false;
            guiHelper.message = "Backed to room list";
            elapsedTimeDisplayedMsg = 0;
            MultiplayerManager.Instance.LeaveRoom(0);
            MultiplayerManager.Instance.RefreshRoomList();
        }

        btnTemptRect.x = 19 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        if (Network.isServer)
        {
            if (GUI.Button(btnTemptRect, "Start Game"))
            {
                //if (joinedPlayer < 2)
                if (MultiplayerManager.Instance.PlayersList.Count < 2)
                {
                    guiHelper.message = "There must be 2 players to start game!";
                    elapsedTimeDisplayedMsg = 0;
                }
                else
                {
                    guiHelper.message = "Starting ... ";
                    elapsedTimeDisplayedMsg = 0;
                    MultiplayerManager.Instance.LaunchGame("map");
                }
            }
        }
        else
        {
            GUI.Label(btnTemptRect, "Waiting for host to start...");
        }


        // If this is Player 2, check if room creator has left
        // If room creator has left, leave room, too.
        if (joinRoom && MultiplayerManager.Instance.needToLeave)
        {
            guiHelper.message = "Disconnected from room's host";
            elapsedTimeDisplayedMsg = 0;
            joinRoom = false; // reset to join other rooms
            MultiplayerManager.Instance.needToLeave = false;
            MultiplayerManager.Instance.RefreshRoomList();
            return;
        }
    }
}
