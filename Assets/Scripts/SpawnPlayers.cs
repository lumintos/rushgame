using UnityEngine;
using System.Collections;

public class SpawnPlayers : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MultiplayerManager.Instance.SpawnPlayer();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
