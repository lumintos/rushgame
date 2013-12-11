using UnityEngine;
using System.Collections;

/// <summary>
/// Network manager. Manage Creation & Joining a room of player
/// </summary>
public class NetworkManager : MonoBehaviour {

	public User user;
	private const string gameName = "mobserv_RushGame"; //Must be unique name for our game
	public string roomName = "";
	private HostData[] hostList; //List of available rooms
	private bool refreshing;
	public GameObject playerPrefab;
	private int port = 25000;

	void OnStart()
	{
		//Find Gameobject named User that was kept from previous scene
		//then get the script component
		user = GameObject.Find("User").GetComponent<User>();
	}

	private void StartServer()
	{
		if(roomName == "")
		{
			//Set default room name
			//username is identical and so is room name
			roomName = user.username + "'s Game";
		}
		int rand = Random.Range(1, 100);
		port = port + rand; // For difference devices, no need to + rand. It's testing only on single device.
		Network.InitializeServer(10, port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameName, roomName);
	}

	void OnServerInitialized()
	{
		SpawnPlayer();
	}

	void OnGUI()
	{
		if(!Network.isClient && !Network.isServer)
		{
			if(GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();

			if(GUI.Button(new Rect(100, 250, 250, 100), "Refresh Host List"))
				RefreshHostList();

			if(hostList != null)
			{
				for(int i = 0; i < hostList.Length; i++)
				{
					if(GUI.Button(new Rect(400, 100 + (100 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}

	private void RefreshHostList()
	{
		MasterServer.RequestHostList(gameName);
	}


	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void OnConnectedToServer()
	{
		SpawnPlayer();
	}

	private void SpawnPlayer()
	{
		Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
	}
}
