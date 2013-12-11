using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// User keeps username and friendslist of that user
/// </summary>

public class User : MonoBehaviour {

	public string username = "TestUser";
	public List<string> friendsList;
	public bool needToDestroy = false;

	// Use this for initialization
	void Start () {
		friendsList = new List<string>();
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
}
