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

    public int PlayerIndex = 1;
    public GameObject playerOnePrefab, playerTwoPrefab;
    public List<RUSHPlayer> PlayersList = new List<RUSHPlayer>();
    public HostData[] RoomList;
    public bool isGameStarted = false;
    public bool refreshing = false;
    public bool gameLoaded;
    public RUSHPlayer MyPlayer;
    public GameObject[] Spawnpoints;
    public int JoinedRoomFlag = 2; // 1 - Joined, 0 - Joining, -1 - Failed, other values - Idle

    public bool needToLeave = false; //Used by client when room creator left room


    void Start()
    {
        Instance = this;
        //PlayerName = PlayerPrefs.GetString("PlayerName");
        DontDestroyOnLoad(gameObject);
        //MasterServer.ipAddress = "hieurl.zapto.org";
        //MasterServer.port = 50005;
    }

    void FixedUpdate()
    {
        Instance = this;
    }

    public void CreateRoom(string roomName, int maxPlayers)
    {
        RoomName = roomName;
        MaxPlayers = maxPlayers;
        int port = 25000 + Random.Range(0, 100);
        Network.InitializeServer(MaxPlayers, port, false);
        MasterServer.RegisterHost(GameName, RoomName);
    }

    public void JoinRoom(HostData hostData)
    {
        JoinedRoomFlag = 0; // Set joining status
        Network.Connect(hostData);
    }

    public void RefreshRoomList()
    {
        MasterServer.RequestHostList(GameName);
    }

    public void LeaveRoom()
    {
        if (Network.isServer)
        {
            Server_AskPlayerToLeave();
            Network.Disconnect();
            MasterServer.UnregisterHost();
        }
        else
        {
            Network.Disconnect();
        }

        needToLeave = false;
    }

    public void LaunchGame(string mapName)
    {
        Client_LaunchGame(mapName);
    }

    

    //Events at Server
    
    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            RoomList = MasterServer.PollHostList();
            refreshing = false;
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
        foreach (RUSHPlayer tempplayer in PlayersList)
        {
            networkView.RPC("Client_AddPlayerToList", id, tempplayer.username, tempplayer.networkPlayer, tempplayer.team);
        }
    }

    //Events at Client

    void OnFailedToConnect(NetworkConnectionError error)
    {
        JoinedRoomFlag = -1;
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected");
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
        //Debug.Log("Asking...");
        if (Network.isServer)
        {
            needToLeave = false;
            //Debug.Log("SV askes");
            networkView.RPC("Server_AskPlayerToLeave", RPCMode.OthersBuffered);
        }
        else
        {
            needToLeave = true;
            //Debug.Log("Client answers");
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
    void Client_LaunchGame(string mapName)
    {
        if (Network.isServer)
            networkView.RPC("Client_LaunchGame", RPCMode.Others, mapName);
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
                   
        player = Network.Instantiate(playerPrefab, new Vector3(team * 2f, 0f, 0f), Quaternion.identity, 0);
        //player.name = team.ToString();

        return player;
    }


    /// <summary>
    /// Set user info queried from Database
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="view"></param>
    /// <param name="level"></param>
    public void SetUserInfo(string playerName, NetworkPlayer view, int level, int spirit)
    {
        MyPlayer = new RUSHPlayer();
        MyPlayer.username = playerName;
        MyPlayer.networkPlayer = view;
        MyPlayer.level = level;
        MyPlayer.spirit = spirit;
    }
}

[System.Serializable]
public class RUSHPlayer
{
    public string username = "";
    public NetworkPlayer networkPlayer;
    public int level = 1;
    public int team = 1; // There are 2 teams
    public int spirit;
}