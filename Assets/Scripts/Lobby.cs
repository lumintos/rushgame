using UnityEngine;
using System.Collections;

public class Lobby : MonoBehaviour {
    
    private User user, user2;
    private GUIHelper guiHelper;
    private HostData[] roomList;
    private NetworkManager networkManager;
    private RoomGUI roomGUI;

    private bool creatRoom = false, joinRoom = false;
    public int joinedPlayer = 0;
    private string roomName = "Rooms"; //Display for both room list view or user's room view
    public float elapsedTimeDisplayedMsg = 0; //Amount of time that unchanged message remains displayed

	// Use this for initialization
	void Start () {
        guiHelper = GetComponent<GUIHelper>();
        guiHelper.UpdateGUIElementsSize();
        user = GameObject.Find("User").GetComponent<User>();
        user2 = null; //for testing only
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        roomGUI = GetComponent<RoomGUI>();
	}
	
	// Update is called once per frame
	void Update () {
        //Update size of all elements in case game screen size changing
        //This is not the case on mobile devices, so comment it
        guiHelper.UpdateGUIElementsSize();
        if (user2 == null || user2.username == user.username)
        {
            foreach (GameObject tempGO in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (tempGO != null && user.username != tempGO.GetComponent<User>().username)
                {
                    user2 = tempGO.GetComponent<User>();
                    break;
                }
            }
        }

        joinedPlayer = GameObject.FindGameObjectsWithTag("Player").GetLength(0);
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

        Rect roomGroupRect = guiHelper.GetScaledRectFromUnit(34, 17);
        roomGroupRect.x = 1 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        roomGroupRect.y = 5 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        GUI.Box(roomGroupRect, roomName);

        //Friend List & button
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
            form.AddField("username", user.username);
            WWW w = new WWW("http://84.101.189.177:25500/getfriends.php", form);
            StartCoroutine(getFriendsRequest(w));
        }


        //User info
        Rect userinfoRect = guiHelper.GetScaledRectFromUnit(6, 2);
        userinfoRect.x = 37 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        userinfoRect.y = 1 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

        GUIStyle tempLabelStyle = new GUIStyle(GUI.skin.label);
        tempLabelStyle.normal.textColor = Color.white;
        tempLabelStyle.fontSize = (int)guiHelper.screenHeight * guiHelper.fontSizeUnit / guiHelper.screenHeightUnit;
        GUI.Label(userinfoRect, user.username, tempLabelStyle);

        userinfoRect.y = 2 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
        tempLabelStyle.fontStyle = FontStyle.Italic;
        tempLabelStyle.fontSize = (int)guiHelper.screenHeight * guiHelper.fontSizeUnit / guiHelper.screenHeightUnit;
        GUI.Label(userinfoRect, "Level: " + user.level, tempLabelStyle);

        //Logout button
        Rect btnLogoutRect = guiHelper.GetScaledRectFromUnit(3, 3);
        btnLogoutRect.x = 44 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
        btnLogoutRect.y = 1 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

        if (GUI.Button(btnLogoutRect, "Logout"))
        {
            guiHelper.message = "Logging out ... ";
            elapsedTimeDisplayedMsg = 0;
            WWWForm form = new WWWForm();
            form.AddField("username", user.username);
            WWW w = new WWW("http://84.101.189.177:25500/logout.php", form);
            StartCoroutine(logoutRequest(w));            
        }

        //Room functions buttons
        btnTemptRect.x = 9 * guiHelper.screenWidth / guiHelper.screenWidthUnit;

        if (!creatRoom && !joinRoom)
        {
            //TODO: Room list is displayed here
            if (roomList != null)
            {
                for (int i = 0; i < roomList.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (100 * i), 300, 100), roomList[i].gameName))
                    {
                        roomName = roomList[i].gameName;
                        user.playerIndex = 2;
                        networkManager.JoinServer(roomList[i]);
                        joinRoom = true;
                    }
                }
            }

            if (GUI.Button(btnTemptRect, "Refresh Room List"))
            {
                roomList = networkManager.GetRoomList();
                guiHelper.message = "New room list";
                elapsedTimeDisplayedMsg = 0;
            }


            btnTemptRect.x = 19 * guiHelper.screenWidth / guiHelper.screenWidthUnit;

            if (GUI.Button(btnTemptRect, "Create Room"))
            {
                //call network manager to register room here
                roomName = user.username + "'s Room";
                user.playerIndex = 1;
                networkManager.StartServer(roomName);
                guiHelper.message = "New room created ... ";
                elapsedTimeDisplayedMsg = 0;
                joinedPlayer = 1;
                creatRoom = true;
            }
        }
        else
        {
            //TODO: Display 2 players here
            roomGUI.SetUserName(user, user2);
            roomGUI.DisplayPlayers();

            if (GUI.Button(btnTemptRect, "Back"))
            {
                joinedPlayer = 0;
                roomName = "Rooms";
                creatRoom = false;
                joinRoom = false;
                guiHelper.message = "Backed to room list";
                elapsedTimeDisplayedMsg = 0;
                Network.Disconnect();
                
                if (creatRoom) //this is room creator
                {
                    MasterServer.UnregisterHost();
                }
            }

            btnTemptRect.x = 19 * guiHelper.screenWidth / guiHelper.screenWidthUnit;

            if (GUI.Button(btnTemptRect, "Start Game"))
            {
                if (joinedPlayer < 2)
                {
                    guiHelper.message = "There must be 2 players to start game!";
                    elapsedTimeDisplayedMsg = 0;
                }
                else
                {
                    guiHelper.message = "Starting ... ";
                    elapsedTimeDisplayedMsg = 0;
                    Application.LoadLevel("map");
                }
            }
        }



    }


    /// <summary>
    /// Update to database that this user logged out
    /// </summary>
    IEnumerator logoutRequest(WWW w)
    {
        user.username = "";
        user.friendsList.Clear();
        //Back to login scene
        Application.LoadLevel("login");

        yield return w;
        if (w.error == null)
        {
            if (w.text == "Logged out")
            {
                //Reset user
                user.username = ""; 
                user.friendsList.Clear();
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

    IEnumerator getFriendsRequest(WWW w)
    {
        yield return w;
        guiHelper.message = "New List";
        elapsedTimeDisplayedMsg = 0;
    }
}
