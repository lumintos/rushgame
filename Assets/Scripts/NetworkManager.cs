using UnityEngine;
using System.Collections;

/// <summary>
/// Network manager. Manage Creation & Joining a room of player
/// </summary>
public class NetworkManager : MonoBehaviour {

	public User myself;
	private const string gameName = "mobserv_RushGame"; //Must be unique name for our game
	public string roomName = "";
	private HostData[] hostList; //List of available rooms
	private bool refreshing = false;
	public GameObject playerPrefab;
	private int port = 25000;

	void Start()
	{
		//Find Gameobject named User that was kept from previous scene
		//then get the script component
        myself = (User)GameObject.Find("User").GetComponent("User");
	}

    /// <summary>
    /// Register for a game room
    /// </summary>
    /// <param name="_roomName">Name of the room, must be unique logically</param>
	public void StartServer(string _roomName)
	{
        roomName = _roomName;
		int rand = Random.Range(1, 100);
		port = port + rand; // For difference devices, no need to + rand. It's testing only on single device.
		Network.InitializeServer(10, port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameName, roomName);
	}

	void OnServerInitialized()
	{
		SpawnPlayer(1);
	}

    public HostData[] GetRoomList()
    {
        RefreshHostList();
        while (refreshing)
            continue;

        return hostList;
    }

	private void RefreshHostList()
    {
        refreshing = true;
        MasterServer.RequestHostList(gameName);
	}


	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            hostList = MasterServer.PollHostList();
            refreshing = false;
        }
	}

	public void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void OnConnectedToServer()
	{
		SpawnPlayer(2);
	}

	private void SpawnPlayer(int userIndex)
	{
		Network.Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
        
	}
}
