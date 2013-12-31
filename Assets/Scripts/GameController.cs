using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private GUIManager guiManager;
    private CameraController camController;
    public GameObject magicalStone;
	
	// Use this for initialization
	void Start () {
        guiManager.UpdateGUIElementsSize(new Size(Screen.width, Screen.height));
        GameObject player = (GameObject) MultiplayerManager.Instance.SpawnPlayer();
        camController.addMainPlayer(player);
        if (!magicalStone)
            magicalStone = GameObject.FindGameObjectWithTag("KeyItem");
	}
	
	void Awake(){		
		guiManager = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIManager>();
        camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
	}
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
