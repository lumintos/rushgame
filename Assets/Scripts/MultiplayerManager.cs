using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance;

    public string PlayerName;

    private string GameName = "mobserv-RUSH Game";
    private string RoomName = "";
    private int MaxPlayers = 2;
    private int lastLevelPrefix = 0;

    public int PlayerIndex = 1;
    public GameObject playerOnePrefab, playerTwoPrefab;
    public List<RUSHPlayer> PlayersList = new List<RUSHPlayer>();
    public List<HostData> RoomList = null;
    public bool isGameStarted = false;
    public bool refreshing = false;
    public bool gameLoaded;
    public RUSHPlayer MyPlayer;
    public GameObject[] Spawnpoints;
    public int JoinedRoomFlag = 2; // 1 - Joined, 0 - Joining, -1 - Failed, other values - Idle
    public HostData roomToJoin;
    public string roomToJoinIP;
    public bool needToLeave = false; //Used by client when room creator left room


    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //MasterServer.ipAddress = "192.168.1.109";
        //MasterServer.port = 23466;
    }

    void FixedUpdate()
    {
        Instance = this;
    }

    public void CreateRoom(string roomName, int maxPlayers)
    {
        RoomName = roomName;
        MaxPlayers = maxPlayers;
        int port = 25000;// +Random.Range(0, 100);
        Network.InitializeServer(MaxPlayers, port, false);
        MasterServer.RegisterHost(GameName, RoomName);
    }

    public void JoinRoom(HostData hostData)
    {
        roomToJoin = hostData;
        int i = 1;
        roomToJoinIP = roomToJoin.ip[0];

        while (i < roomToJoin.ip.Length)
        {
            roomToJoinIP += "." + roomToJoin.ip[i];
            i++;
        }
        //Debug.Log("Room IP: " + roomToJoinIP);
        JoinedRoomFlag = 0; // Set joining status
        MasterServer.RequestHostList(GameName);
        //Network.Connect(hostData);
    }

    public void RefreshRoomList()
    {
        MasterServer.RequestHostList(GameName);
    }

    /// <summary>
    /// Use when user leaves lobby room or current game. 
    /// </summary>
    /// <param name="flag">
    /// Flag determines user leaves lobby room (0) or a running game (1) or an ended game (2)</param>
    public void LeaveRoom(int flag)
    {
        int timeout = 200;
        if (flag == 2) //End game
            timeout = 3000; //3 seconds

        if (Network.isServer)
        {
            if (flag == 0 || flag == 1) //Host leaves room or running game, everyone must leave
                Server_AskPlayerToLeave();
            Network.Disconnect(timeout);
            MasterServer.UnregisterHost();
        }
        else
        {
            Network.Disconnect(timeout);
        }

        needToLeave = false;
    }

    public void LaunchGame(string mapName)
    {
        Client_LaunchGame(mapName, lastLevelPrefix + 1);
    }

    

    //Events at Server
    
    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            int i = 0;
            RoomList = null;
            foreach (HostData room in MasterServer.PollHostList())
            {
                //Debug.Log("Room has: " + room.connectedPlayers.ToString());
                if (room.connectedPlayers == 1) //Only display waiting room
                {
                    if (RoomList == null)
                        RoomList = new List<HostData>();
                    RoomList.Add(room);
                }
            }
            if (JoinedRoomFlag == 0)
            {
                //is checking existence of a room to join room
                //Debug.Log("Checking room");
                foreach (HostData room in MasterServer.PollHostList())
                {
                    string checkIP = room.ip[0];
                    i = 1;
                    while (i < room.ip.Length)
                    {
                        checkIP += "." + room.ip[i];
                        i++;
                    }

                    //Debug.Log("Check IP: " + checkIP);
                    //Debug.Log("Port: " + room.port);
                    if (checkIP == roomToJoinIP && room.port == roomToJoin.port && room.connectedPlayers == 1)
                    {
                        Network.Connect(roomToJoin);
                        return;
                    }
                } 
                JoinedRoomFlag = -1;
            }
        }
    }

    void OnServerInitialized()
    {
        PlayerIndex = 1; // Room Creator
        Server_PlayerJoinRequest(PlayerName, Network.player, PlayerIndex);
    }

    void OnPlayerDisconnected(NetworkPlayer id)
    {
        networkView.RPC("Client_RemovePlayer", RPCMode.All, id);
        //TODO: stop game with win result to the player left in room
    }

    void OnPlayerConnected(NetworkPlayer id)
    {
        Debug.Log(Network.connections.Length.ToString());
        if (Network.connections.Length < 2)
        {
            foreach (RUSHPlayer tempplayer in PlayersList)
            {
                networkView.RPC("Client_AddPlayerToList", id, tempplayer.username, tempplayer.networkPlayer, tempplayer.team);
            }
        }
        else //Ask recently join player to leave
        {
            networkView.RPC("Server_AskPlayerToLeave", id);
        }
    }

    //Events at Client

    void OnFailedToConnect(NetworkConnectionError error)
    {
        JoinedRoomFlag = -1;
        Debug.Log(error);
    }

    void OnConnectedToServer()
    {
        //Debug.Log("Connected");
        JoinedRoomFlag = 1;
        PlayerIndex = 2; // Not Room Creator
        networkView.RPC("Server_PlayerJoinRequest", RPCMode.Server, PlayerName, Network.player, PlayerIndex);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        PlayersList.Clear();
        //TODO: in case of network connection, stop game with win result to the player left in room
    }


    [RPC]
    void Server_PlayerJoinRequest(string playerName, NetworkPlayer view, int playerIndex)
    {
        networkView.RPC("Client_AddPlayerToList", RPCMode.All, playerName, view, playerIndex);
    }

    /// <summary>
    /// When host leaves room, other player must leave, too.
    /// </summary>
    [RPC]
    void Server_AskPlayerToLeave()
    {
        if (Network.isServer)
        {
            needToLeave = false;
            networkView.RPC("Server_AskPlayerToLeave", RPCMode.OthersBuffered);
        }
        else
        {
            needToLeave = true;
            Network.Disconnect();
        }
    }

    /// <summary>
    /// Add player to list at each client, including host
    /// </summary>
    /// <param name="playerName">Username of player</param>
    /// <param name="view">Network view ID of player</param>
    /// <param name="playerIndex">Team of player</param>
    [RPC]
    void Client_AddPlayerToList(string playerName, NetworkPlayer view, int playerIndex)
    {
        RUSHPlayer tempplayer = new RUSHPlayer();
        tempplayer.username = playerName;
        tempplayer.networkPlayer = view;
        tempplayer.team = playerIndex;
        PlayersList.Add(tempplayer);
        if (Network.player == view) //Same as networkView.isMine == true
        {
            //MyPlayer = tempplayer;
            MyPlayer.team = playerIndex;
            PlayerIndex = playerIndex;
        }
    }


    /// <summary>
    /// A player has left room, must be removed from list at other client
    /// </summary>
    /// <param name="view"></param>
    [RPC]
    void Client_RemovePlayer(NetworkPlayer view)
    {
        RUSHPlayer temppl = null;
        foreach (RUSHPlayer pl in PlayersList)
        {
            if (pl.networkPlayer == view)
            {
                temppl = pl;
            }
        }
        if (temppl != null)
        {
            PlayersList.Remove(temppl);
        }
    }

    /// <summary>
    /// Both host and client will launch a given map
    /// </summary>
    /// <param name="mapName">Name of Unity scene</param>
    [RPC]
    void Client_LaunchGame(string mapName, int levelPrefix)
    {
        if (Network.isServer)
            networkView.RPC("Client_LaunchGame", RPCMode.OthersBuffered, mapName, levelPrefix);

        lastLevelPrefix = levelPrefix;
        Application.LoadLevel(mapName);
    }

    /// <summary>
    /// Spawn players at different position based on team index
    /// </summary>
    public Object SpawnPlayer()
    {
        GameObject playerPrefab = null;
        Object player = null;
        int team = 1;

        foreach (RUSHPlayer tempplayer in PlayersList)
        {
            if (PlayerName == tempplayer.username)
            {
                team = tempplayer.team;
                break;
            }
        }

        if (team == 1)
            playerPrefab = playerOnePrefab;
        else
            playerPrefab = playerTwoPrefab;
                   
        player = Network.Instantiate(playerPrefab, new Vector3(team * 2f, 2f, 0f), playerPrefab.transform.rotation, 0);
        //player.name = team.ToString();

        return player;
    }


    /// <summary>
    /// Set user info queried from Database
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="view"></param>
    /// <param name="level"></param>
    public void SetUserInfo(string playerName, NetworkPlayer view, int max_spirit, int spirit)
    {
        MyPlayer = new RUSHPlayer();
        MyPlayer.username = playerName;
        MyPlayer.networkPlayer = view;
        MyPlayer.maxSpirit = max_spirit;
        MyPlayer.spirit = spirit;
    }
}

[System.Serializable]
public class RUSHPlayer
{
    public string username = "";
    public NetworkPlayer networkPlayer;
    public int maxSpirit = 1;
    public int team = 1; // There are 2 teams
    public int spirit;
}