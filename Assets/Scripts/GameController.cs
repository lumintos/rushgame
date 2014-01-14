using UnityEngine;
using System.Collections;
using System.Xml;

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
    private bool updatedResult;
    GameObject testMultiplayer = null;

	// Use this for initialization
	void Start () {
        gameEnd = 0;
        isStoneTaken = false;
        updatedResult = false;
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
            SpawnStone();
        
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
                        //networkView.RPC("DisplayResult", RPCMode.AllBuffered);
                        //MultiplayerManager.Instance.LeaveRoom(2); //Disconnect and unregister host for both server and client 
                    }
                }
            }

            KeepStoneTimer(isStoneTaken.ToString(), startTimeKeepStone, elapsedTimeKeepStone);
        }

        guiManager.ChangeStoneStatusTexture(isStoneTaken, stoneKeeper);

	}

    //TODO: Move this part to GUIManager for consistence of code
    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 200, 100), "QUIT"))
        {
            MultiplayerManager.Instance.LeaveRoom(2);
            Application.LoadLevel("lobby");
        }

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
            Rect txtTemptRect = guiHelper.GetScaledRectFromUnit(40, 23);
            txtTemptRect.x = 4 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
            txtTemptRect.y = 2 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

            GUI.BeginGroup(txtTemptRect, "", GUI.skin.box);

            //GUI.Box(txtTemptRect, "");
            
            txtTemptRect = guiHelper.GetScaledRectFromUnit(20, 7);
            txtTemptRect.x = 10 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
            txtTemptRect.y = 1 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
            Vector2 direction = new Vector2(1, 1);
            GUIContent content = new GUIContent(guiManager.Text_GameResult.text);
            GUIStyle style = GUI.skin.label;
            style.fontSize = (int)(guiHelper.btnScaledHeight);
            style.alignment = TextAnchor.MiddleCenter;

            string text = "";
            if (gameEnd == 1)
                text = "VICTORY";
            else if (gameEnd == 2)
                text = "DEFEAT";

            ShadowAndOutline.DrawOutline(txtTemptRect, text, style, guiHelper.outlineColor[gameEnd - 1], guiHelper.textColor[gameEnd - 1], 4); // When game ended, gameEnd > 0

            Rect txtScore = guiHelper.GetScaledRectFromUnit(40, 4);
            txtScore.x = 0 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
            txtScore.y = 9 * guiHelper.screenHeight / guiHelper.screenHeightUnit;
            style.fontSize /= 2;

            Rect btnTemptRect = guiHelper.GetScaledRectFromUnit(8, 4);
            btnTemptRect.x = 16 * guiHelper.screenWidth / guiHelper.screenWidthUnit;
            btnTemptRect.y = 16 * guiHelper.screenHeight / guiHelper.screenHeightUnit;

            if (updatedResult)
            {
                string score = "";
                if (gameEnd == 1)
                {
                    score = "Spirit: " + MultiplayerManager.Instance.MyPlayer.spirit
                        + "(+ " + GameConstants.bonusSpirit + ")"
                        + " / " + MultiplayerManager.Instance.MyPlayer.maxSpirit;
                }
                else if (gameEnd == 2)
                {
                    score = "Spirit: " + MultiplayerManager.Instance.MyPlayer.spirit
                        + "(- " + GameConstants.bonusSpirit + ")"
                        + " / " + MultiplayerManager.Instance.MyPlayer.maxSpirit;
                }

                ShadowAndOutline.DrawOutline(txtScore, score, style, guiHelper.outlineColor[gameEnd - 1], Color.white, 4);

                if (GUI.Button(btnTemptRect, "Continue"))
                {
                    Application.LoadLevel("lobby");
                }
            }
            else
            {
                ShadowAndOutline.DrawOutline(txtScore, "Updating score...", style, Color.black, Color.gray, 4);
            }

            GUI.EndGroup();
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
                SpawnStone();
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

        if (isStoneTaken)
        {
            int minutes = (int)(GameConstants.stoneTimer - elapsedTimeKeepStone) / 60;
            int seconds = (int)(GameConstants.stoneTimer - elapsedTimeKeepStone) % 60;
            guiManager.Text_Timer.text = minutes.ToString() + " : " + seconds.ToString();
        }
        else
        {
            guiManager.Text_Timer.text = "-- : --";
        }
    }

    [RPC]
    void SpawnStone()
    {
        Network.Instantiate(stonePrefab, stonePrefab.transform.position, stonePrefab.transform.rotation, 0); //This will spawn magical stone in both server and client
    }

    [RPC]
    void DisplayResult()
    {
            //TODO: Display Popup result
        if (Network.player == stoneKeeper)
        {
            gameEnd = 1;
        }
        else
        {
            gameEnd = 2;
        }

        UpdateScoreToDB();
    }

    void UpdateScoreToDB()
    {
        string result = (gameEnd == 1 ? "win" : "lose");
        WWWForm form = new WWWForm();
        form.AddField("username", MultiplayerManager.Instance.PlayerName);
        form.AddField("alter_spirit", GameConstants.bonusSpirit);
        form.AddField("match_result", result);
        form.AddField("alter_max_spirit", 0);
        string url = "http://hieurl.zapto.org/~hieu/rushgame/Server/php/user.php?action=update_match_score";
        WWW w = new WWW(url, form);
        StartCoroutine(setScoreRequest(w));
    }

    IEnumerator setScoreRequest(WWW w)
    {
        yield return w;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(w.text);

        XmlNode codeNode = doc.DocumentElement.SelectSingleNode("/response/code");
        string code = codeNode.InnerText;

        if (code == "OK")
        {
            updatedResult = true;
        }
        else
        {
            updatedResult = false;
        }
    }
}
