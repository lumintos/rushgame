using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// User keeps username and friendslist of that user
/// </summary>

public class User : MonoBehaviour {

	public string username = "";
    public int level = 1;
    public int points = 100;
    public int playerIndex;
	public List<string> friendsList;
	public bool needToDestroy = false;

	// Use this for initialization
	void Start () {
		friendsList = new List<string>();
        NetworkView networkView = GetComponent<NetworkView>();

        if (networkView != null && networkView.isMine)
        {
            if (username == "")
            {
                User myinfo = GameObject.FindGameObjectWithTag("UserInfo").GetComponent<User>();
                UpdateInfo(myinfo);

                this.gameObject.tag = Tags.player;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        
	}

	void Awake()
	{
		//Keep this object while loading from Login scene to Lobby Scene
		//but destroy it when we enter Map (playing) scene to prevent conflict info between players
		if(!needToDestroy)
			DontDestroyOnLoad(this);
	}

    private void UpdateInfo(User myinfo)
    {
       // object[] rpcparams = new object[1];

       // rpcparams[0] = myinfo.username;
       // rpcparams[1] = myinfo.level;

        level = myinfo.level;
        points = myinfo.points;
        friendsList = new List<string>(myinfo.friendsList);
        myinfo.needToDestroy = true;

        SetMyInfo(myinfo.username);

    }

    [RPC] private void SetMyInfo(string name)
    {
        username = name;

        if (Network.isClient)
            playerIndex = 2;
        else
            playerIndex = 1;

        if (networkView != null && networkView.isMine)
        {
            networkView.RPC("SetMyInfo", RPCMode.OthersBuffered, username);
        }
    }
}
