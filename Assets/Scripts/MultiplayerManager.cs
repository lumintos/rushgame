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

    public int PlayerIndex = 2;
    public GameObject playerPrefab;
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
        PlayerName = PlayerPrefs.GetString("PlayerName");
        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate()
    {
        Instance = this;
    }

    public void CreateRoom(string roomName, int maxPlayers)
    {
        RoomName = roomName;
        MaxPlayers = maxPlayers;
        Network.InitializeServer(MaxPlayers, 25000, !Network.HavePublicAddress());
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

    //TODO: method to launch map for both client and server players


    

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
    }


    [RPC]
    void Server_PlayerJoinRequest(string playerName, NetworkPlayer view, int playerIndex)
    {
        networkView.RPC("Client_AddPlayerToList", RPCMode.All, playerName, view, playerIndex);
    }

    [RPC]
    void Server_AskPlayerToLeave()
    {
        Debug.Log("Asking...");
        if (Network.isServer)
        {
            needToLeave = false;
            Debug.Log("SV askes");
            networkView.RPC("Server_AskPlayerToLeave", RPCMode.OthersBuffered);
        }
        else
        {
            needToLeave = true;
            Debug.Log("Client answers");
        }
    }

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
            MyPlayer = tempplayer;
            PlayerIndex = MyPlayer.team;
           // GameObject play = (GameObject)Network.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, 5);            
        }
    }

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
}

[System.Serializable]
public class RUSHPlayer
{
    public string username = "";
    public NetworkPlayer networkPlayer;
    public int level = 1;
    public int team = 1; // There are 2 teams
}