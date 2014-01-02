using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private GUIManager guiManager;
    private CameraController camController;
    public MagicalStone magicalStone;
    public NetworkPlayer stoneKeeper;
    public GameObject goal;
    public bool gameEnd;

	// Use this for initialization
	void Start () {
        gameEnd = false;
        guiManager.UpdateGUIElementsSize(new Size(Screen.width, Screen.height));
        GameObject player = (GameObject) MultiplayerManager.Instance.SpawnPlayer();
        camController.addMainPlayer(player);
	}
	
	void Awake(){		
		guiManager = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIManager>();
        camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        if (!goal)
            goal = GameObject.FindGameObjectWithTag("Goal");

        if (!magicalStone)
            magicalStone = GameObject.FindGameObjectWithTag("KeyItem").GetComponent<MagicalStone>();
	}
	
	
	// Update is called once per frame
	void Update () 
    {
        //WINNING CONDITION CHECKING
        //Only server can check
        if (Network.isServer)
        {
            if (!gameEnd)
            {
                TriggerParent goalTrigger = goal.GetComponent<TriggerParent>();
                if (goalTrigger.collided)
                {
                    if (goalTrigger.hitObject.networkView.owner == stoneKeeper)
                    {
                        gameEnd = true;
                        //TODO: call RPC Display result only once
                        networkView.RPC("DisplayResult", RPCMode.AllBuffered);
                    }
                }
            }
        }
	}

    [RPC]
    void DisplayResult()
    {
        //Since client does not check winning condition, it does not know game ended
        //So we update for it here because result is only displayed once the game ended
        if (Network.isClient)
        {
            gameEnd = true;
        }

        if (Network.player == stoneKeeper)
        {
            //TODO: Display Popup result
            guiManager.GameResult.text = "VICTORY";
            //Debug.Log("VICTORY");
        }
        else
        {
            guiManager.GameResult.text = "DEFEAT";
            //Debug.Log("DEFEAT");
        }
    }

    
}
