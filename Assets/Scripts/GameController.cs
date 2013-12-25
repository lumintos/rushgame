using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private GUIManager guiManager;
    private CameraController camController;
	
	// Use this for initialization
	void Start () {
        guiManager.UpdateGUIElementsSize(new Size(Screen.width, Screen.height));
        GameObject player = (GameObject) MultiplayerManager.Instance.SpawnPlayer();
        camController.addMainPlayer(player);
	}
	
	void Awake(){
		
		guiManager = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIManager>();
        camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
	}
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
