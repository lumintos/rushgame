using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private GUIManager guiManager;
    public GUIHelper guiHelper;
    private CameraController camController;
    public MagicalStone magicalStone;
    public NetworkPlayer stoneKeeper;
    public GameObject goal;
    public bool gameEnd;

    GameObject testMultiplayer = null;

	// Use this for initialization
	void Start () {
        gameEnd = false;
        guiManager.UpdateGUIElementsSize(new Size(Screen.width, Screen.height));
        guiHelper.UpdateGUIElementsSize();

        testMultiplayer = GameObject.Find("Multiplayer Manager");
        GameObject player = null;
        if (testMultiplayer != null)
        {
            GameObject.FindGameObjectWithTag("Player").SetActive(false); //Auto remove Player Obj from scene in multiplayer mode
            player = (GameObject)MultiplayerManager.Instance.SpawnPlayer(); //And spawn new one
        }
        else
        {            
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if(player != null)
            camController.addMainPlayer(player);
	}
	
	void Awake(){		
		guiManager = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIManager>();
        guiHelper = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIHelper>();
        camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        if (!goal)
            goal = GameObject.FindGameObjectWithTag("Goal");

        if (!magicalStone)
            magicalStone = GameObject.FindGameObjectWithTag("KeyItem").GetComponent<MagicalStone>();
	}
	
	
	// Update is called once per frame
	void Update () 
    {
        guiManager.UpdateGUIElementsSize(new Size(Screen.width, Screen.height));
        guiHelper.UpdateGUIElementsSize();
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
                        MultiplayerManager.Instance.LeaveRoom(2); //Disconnect and unregister host for both server and client 
                    }
                }
            }
        }
	}

    //TODO: Move this part to GUIManager for consistence of code
    void OnGUI()
    {
        if (!guiHelper.guiUpdated)
        {
            ColoredGUISkin.Instance.UpdateGuiColors(guiHelper.primaryColor, guiHelper.secondaryColor);
            guiHelper.guiUpdated = true;
        }

        GUI.skin = ColoredGUISkin.Skin;

        GUI.skin.button.fontSize = (int)(guiHelper.btnScaledHeight * guiHelper.fontSizeUnit / guiHelper.btnHeightUnit);

        if (gameEnd)
        {
            Rect btnTemptRect = guiHelper.GetScaledRectFromUnit(8, 4);
            btnTemptRect.x = 20 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
            btnTemptRect.y = 16 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

            if (GUI.Button(btnTemptRect, "Continue"))
            {   
                Application.LoadLevel("lobby");
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
