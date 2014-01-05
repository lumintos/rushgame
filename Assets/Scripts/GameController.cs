using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private GUIManager guiManager;
    public GUIHelper guiHelper;
    private CameraController camController;
    public NetworkPlayer stoneKeeper;
    public bool isStoneTaken;
    public GameObject goal;
    public int gameEnd; //0: playing, 1: Win, 2: Lose
    public GameObject stonePrefab;

    private float startTimeKeepStone, elapsedTimeKeepStone;
    private float maxTimeKeepStone;
    GameObject testMultiplayer = null;

	// Use this for initialization
	void Start () {
        gameEnd = 0;
        isStoneTaken = false;
        startTimeKeepStone = 0;
        elapsedTimeKeepStone = 0;
        maxTimeKeepStone = 30; // in seconds
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
        
        if(Network.isServer)
            SpawnStone(stonePrefab.transform.position, stonePrefab.transform.rotation);
        
        if(player != null)
            camController.addMainPlayer(player);
	}
	
	void Awake(){		
		guiManager = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIManager>();
        guiHelper = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIHelper>();
        camController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        if (!goal)
            goal = GameObject.FindGameObjectWithTag("Goal");
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
            if (gameEnd == 0)
            {
                TriggerParent goalTrigger = goal.GetComponent<TriggerParent>();
                if (goalTrigger.collided)
                {
                    if (isStoneTaken && goalTrigger.hitObject.networkView.owner == stoneKeeper)
                    {
                        //TODO: call RPC Display result only once
                        networkView.RPC("DisplayResult", RPCMode.AllBuffered);
                        MultiplayerManager.Instance.LeaveRoom(2); //Disconnect and unregister host for both server and client 
                    }
                }
            }

            KeepStoneTimer(isStoneTaken.ToString(), startTimeKeepStone, elapsedTimeKeepStone);
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
        
        //TODO: Display stone's status here


        //Display for end game
        if (gameEnd != 0)
        {
            Rect txtTemptRect = guiHelper.GetScaledRectFromUnit(24, 8);
            txtTemptRect.x = 12 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
            txtTemptRect.y = 4 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
            Vector2 direction = new Vector2(1, 1);
            GUIContent content = new GUIContent(guiManager.GameResult.text);
            GUIStyle style = GUI.skin.label;
            style.fontSize = (int)(guiHelper.btnScaledHeight);
            style.alignment = TextAnchor.MiddleCenter;

            string text = "";
            if (gameEnd == 1)
                text = "VICTORY";
            else if (gameEnd == 2)
                text = "DEFEAT";

            ShadowAndOutline.DrawOutline(txtTemptRect, text, style, guiHelper.outlineColor[gameEnd - 1], guiHelper.textColor[gameEnd - 1], 4); // When game ended, gameEnd > 0


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
    void KeepStoneTimer(string isTaken, float startTime, float elapsedTime)
    {
        if (Network.isServer) //server has previlege to control timer
        {
            if (!isStoneTaken)
                return;
            if (startTimeKeepStone == 0) //Start timer
                startTimeKeepStone = Time.time;

            elapsedTimeKeepStone += Time.deltaTime;

            if (elapsedTimeKeepStone > maxTimeKeepStone)
            {
                //TODO: respawn stone at a specific position
                SpawnStone(stonePrefab.transform.position, stonePrefab.transform.rotation);
                isStoneTaken = false;
                startTimeKeepStone = 0;
                elapsedTimeKeepStone = 0;
            }
            networkView.RPC("KeepStoneTimer", RPCMode.OthersBuffered, isStoneTaken.ToString(), startTimeKeepStone, elapsedTimeKeepStone);
        }
        else if (Network.isClient) //client just update information
        {
            startTimeKeepStone = startTime;
            elapsedTimeKeepStone = elapsedTime;
            isStoneTaken = bool.Parse(isTaken);
        }
    }

    [RPC]
    void SpawnStone(Vector3 position, Quaternion quarternion)
    {
        Network.Instantiate(stonePrefab, position, quarternion, 0); //This will spawn magical stone in both server and client
    }

    [RPC]
    void DisplayResult()
    {
        //Since client does not check winning condition, it does not know game ended
        //So we update for it here because result is only displayed once the game ended

        if (Network.player == stoneKeeper)
        {
            //TODO: Display Popup result
            gameEnd = 1;
        }
        else
        {
            gameEnd = 2;
            //Debug.Log("DEFEAT");
        } 
    }

    
}
