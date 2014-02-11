using UnityEngine;
using System.Collections;
using System.Xml;

public class GameController : MonoBehaviour
{

    private GUIManager guiManager;
    private bool gameEndSoundPlayed = false;
    public GUIHelper guiHelper;
    private CameraFollow camController;
    public NetworkPlayer stoneKeeper;
    private GameObject[] pauseItems;
    private GameObject[] endgameItems;
    public bool isStoneTaken;
    public GameObject goal;
    public int gameEnd; //0: playing, 1: Win, 2: Lose
    public GameObject stonePrefab;
    //public GameObject movingPlatformPrefab;
    //public Vector3[] movingPlatformPositionsAndHeights; //Position in Z always is 0, so the Z is used to store max height

    private float startTimeKeepStone, elapsedTimeKeepStone;
    private float maxTimeKeepStone;
    private bool updatedResult;
    private GameObject player = null;
    private GameObject testMultiplayer = null;
    public bool isPause = false;
    public bool isSoundEnable = true;
    public bool isQuitting = false;
    public bool isMovingPlatformStarted = false;
    public int numberOfReadyClients = 0;


    // Use this for initialization
    void Start()
    {
        gameEnd = 0;
        isStoneTaken = false;
        updatedResult = false;
        startTimeKeepStone = 0;
        elapsedTimeKeepStone = 0;
        maxTimeKeepStone = 30; // in seconds
        isMovingPlatformStarted = false;
        numberOfReadyClients = 0;
        guiManager.UpdateGUIElementsSize(new Size(Screen.width, Screen.height));

        gameEndSoundPlayed = false;
        //guiHelper.UpdateGUIElementsSize();


        testMultiplayer = GameObject.Find("Multiplayer Manager");
        if (testMultiplayer != null)
        {
            GameObject.FindGameObjectWithTag("Player").SetActive(false); //Auto remove Player Obj from scene in multiplayer mode
            player = (GameObject)MultiplayerManager.Instance.SpawnPlayer(); //And spawn new one

            if (Network.isServer)
            {
                SpawnStone();
                //StartMovingPlatform(Time.time);
                //SpawnMovingPlatform();
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            StartMovingPlatform(Time.time);
        }

        if (player != null)
        {
            foreach (Transform child in player.transform)
            {
                if (child.name == "CameraLookAt")
                {
                    camController.target = child;
                    break;
                }
            }

			if (this.isSoundEnable == true)
			{
				//InGameAudioManager.Instance.PlayStartSfx(transform.position, 0.5f);
				
				if (InGameAudioManager.Instance.BackgroundCheck() == false) // no background created
					InGameAudioManager.Instance.PlayBackground(transform.position, 0.5f);
				else
					InGameAudioManager.Instance.ContinuePlayBackgroudMusic();
			}
			// to stop playing background -> Instance.StopBackgroundMusic()
			// to pause playing background -> Instance.PauseBackgroundMusic()
        }

        if (testMultiplayer != null && Network.isClient) // Inform server that it finished loading
        {
            networkView.RPC("AddReadyClient", RPCMode.Server, null);
        }
    }

    void Awake()
    {
        guiManager = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIManager>();
        guiHelper = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIHelper>();
        camController = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        pauseItems = GameObject.FindGameObjectsWithTag("PauseItem");
        endgameItems = GameObject.FindGameObjectsWithTag("ResultItem");

        if (!goal)
            goal = GameObject.FindGameObjectWithTag("Goal");
    }


    // Update is called once per frame
    void Update()
    {
        guiManager.UpdateGUIElementsSize(new Size(Screen.width, Screen.height));
        //guiHelper.UpdateGUIElementsSize();

        // Check if any player disconnected
        if (testMultiplayer)
        {
            if (MultiplayerManager.Instance.PlayersList.Count < 2 && gameEnd == 0)
            {
                gameEnd = 3;
                MultiplayerManager.Instance.LeaveRoom(2);
            }
        }

        //WINNING CONDITION CHECKING
        //Only server can check
        if (Network.isServer)
        {
            // Start moving platforms
            
            if (!isMovingPlatformStarted)
            {
                // Only start when all clients are ready
                if(numberOfReadyClients >= Network.connections.Length - 1)
                    StartMovingPlatform(Time.time);
            }

            if (gameEnd == 0)
            {
                TriggerParent goalTrigger = goal.GetComponent<TriggerParent>();
                if (goalTrigger.collided)
                {
                    if (isStoneTaken && goalTrigger.hitObject.networkView.owner == stoneKeeper)
                    {
                        // Call RPC Display result only once
                        networkView.RPC("UpdateResult", RPCMode.AllBuffered);
                        MultiplayerManager.Instance.LeaveRoom(2); //Disconnect and unregister host for both server and client 
                    }
                }
            }

            KeepStoneTimer(isStoneTaken.ToString(), startTimeKeepStone, elapsedTimeKeepStone);
        }

        if (guiManager.GetPauseButtonPress())
        {
            isPause = true;
        }

        guiManager.ChangeStoneStatusTexture(isStoneTaken, stoneKeeper);

    }

    void FixedUpdate()
    {
        //check if player fell out of map, then respawn player
        if (player.transform.position.y < -40)
        {
            if (testMultiplayer != null)
                player.transform.position = new Vector3(MultiplayerManager.Instance.MyPlayer.team * 2, 2, 0);
            else
                player.transform.position = new Vector3(0, 2, 0);
            player.GetComponent<PlayerMovement>().ResetAllStates();
        }
    }

    //TODO: Move this part to GUIManager for consistence of code
    void OnGUI()
    {

        if (!isPause)
        {
            foreach (GameObject item in pauseItems)
            {
                //if (item.activeInHierarchy)
                item.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject item in pauseItems)
            {
                //if(!item.activeInHierarchy)
                item.SetActive(true);
            }

            if (guiHelper.GetButtonPress("SoundButton"))
            {
                guiHelper.SetButtonPress("SoundButton", false);

                isSoundEnable = !isSoundEnable;
                if (isSoundEnable)
                {
                    guiHelper.ChangeButtonTexture("SoundButton", 0);
                    // reopen or play background music			
                    if (InGameAudioManager.Instance.BackgroundCheck() == false) // no background created
                        InGameAudioManager.Instance.PlayBackground(transform.position, 0.5f);
                    else
                        InGameAudioManager.Instance.ContinuePlayBackgroudMusic();
                }
                else
                {
                    guiHelper.ChangeButtonTexture("SoundButton", 1);
                    // pause background
                    if (InGameAudioManager.Instance.BackgroundCheck() == true)
                        InGameAudioManager.Instance.PauseBackgroundMusic();
                }

            }

            if (guiHelper.GetButtonPress("QuitButton"))
            {
                guiHelper.SetButtonPress("QuitButton", false);
                MultiplayerManager.Instance.LeaveRoom(2);
                Application.LoadLevel("lobby");
            }

            if (guiHelper.GetButtonPress("RespawnButton"))
            {
                guiHelper.SetButtonPress("RespawnButton", false);
                isPause = false;
                if (testMultiplayer != null)
                    player.transform.position = new Vector3(MultiplayerManager.Instance.MyPlayer.team * 2, 2, 0);
                else
                    player.transform.position = new Vector3(0, 2, 0);
                player.GetComponent<PlayerMovement>().ResetAllStates();
            }

            if (guiHelper.GetButtonPress("ResumeButton"))
            {
                guiHelper.SetButtonPress("ResumeButton", false);
                isPause = false;
            }
        }
        /*
        if (!guiHelper.guiUpdated)
        {
            ColoredGUISkin.Instance.UpdateGuiColors(guiHelper.primaryColor, guiHelper.secondaryColor);
            guiHelper.guiUpdated = true;
        }
        */
        if (gameEnd == 0)
        {
            foreach (GameObject item in endgameItems)
            {
                item.SetActive(false);
            }
        }
        //Display for end game
        else if (gameEnd != 0)
        {
            // Close pause menu if it's opening,
            // set its active value maybe better than set bool value
            isPause = false;
            //Display result popup
            DisplayResult();
        }
    }

    [RPC]
    void AddReadyClient()
    {
        if(Network.isServer)
            numberOfReadyClients++;
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
    void StartMovingPlatform(float startingTime)
    {
        if (Network.isServer)
            networkView.RPC("StartMovingPlatform", RPCMode.OthersBuffered, startingTime);

        GameObject[] lifts = GameObject.FindGameObjectsWithTag("MovingPlatform");

        foreach (GameObject tempLift in lifts)
        {
            MoveToPoints tempComponent = tempLift.GetComponent<MoveToPoints>();
            //tempComponent.SetPositionByTime(startingTime);
            tempComponent.moveEnabled = true;
        }

        isMovingPlatformStarted = true;
    }


    /// <summary>
    /// Moving platforms need to be synchronized, so server will spawn and own the platforms. Client just update the platforms' states.
    /// </summary>
    [RPC]
    void SpawnMovingPlatform()
    {
        /*
        foreach (Vector3 posAndHeight in movingPlatformPositionsAndHeights)
        {
            Vector3 pos = new Vector3(posAndHeight.x, posAndHeight.y, 0);
            float height = posAndHeight.z;
            GameObject movingPlatform = (GameObject)Network.Instantiate(movingPlatformPrefab, pos, Quaternion.identity, 0); //This will spawn moving platforms in both server and client
            if (movingPlatform.GetComponent<MoveToPoints>().waypoints.Count > 0)
            {
                //Debug.Log("Way point");
                foreach (Transform waypoint in movingPlatform.GetComponent<MoveToPoints>().waypoints)
                {
                    if (waypoint.name == "waypoint2")
                    {
                        Vector3 tempPos = waypoint.position;
                        tempPos.y = height;
                        waypoint.position = tempPos;
                        break;
                    }
                }
            }
            else
                Debug.LogError("NO waypoint");
        }
         * */
    }

    [RPC]
    void UpdateResult()
    {
        if (Network.player == stoneKeeper)
        {
            gameEnd = 1;
        }
        else
        {
            gameEnd = 2;
        }
        if (!MultiplayerManager.Instance.PlayerName.Contains("Tester")) //If didn't login, then do not update to DB
            UpdateScoreToDB();
        else
            updatedResult = true;
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

    void DisplayResult()
    {
        foreach (GameObject item in endgameItems)
        {
            if (item.name == "ResultTitle" && gameEnd != 3)
                item.SetActive(false);
            else
                item.SetActive(true);
        }

        if (gameEnd == 1)
        {
            guiHelper.ChangeTexture("ResultFrame", "UI/frame-result-victory");
            if (gameEndSoundPlayed == false)
            {
                InGameAudioManager.Instance.StopBackgroundMusic();
                if (isSoundEnable)
                    InGameAudioManager.Instance.PlayWinSfx(transform.position, 0.5f);
                gameEndSoundPlayed = true;
            }
        }
        else if (gameEnd == 2)
        {
            guiHelper.ChangeTexture("ResultFrame", "UI/frame-result-defeat");
            if (gameEndSoundPlayed == false)
            {
                InGameAudioManager.Instance.StopBackgroundMusic();
                if (isSoundEnable)
                    InGameAudioManager.Instance.PlayLoseSfx(transform.position, 0.5f);
                gameEndSoundPlayed = true;
            }
        }
        else if (gameEnd == 3) // Player disconnected
        {
            guiHelper.ChangeTexture("ResultFrame", "UI/frame-login-body");
            if (gameEndSoundPlayed == false)
            {
                InGameAudioManager.Instance.StopBackgroundMusic();
                if (isSoundEnable)
                    InGameAudioManager.Instance.PlayLoseSfx(transform.position, 0.5f);
                gameEndSoundPlayed = true;
            }
        }


        //After Updating score to DB or when 1 player quit
        if (updatedResult || gameEnd == 3)
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
            else if (gameEnd == 3)
            {
                score = "A player has disconnected\nGame will quit";
            }
            guiManager.Text_GameResult.text = score;
            guiManager.Text_GameResult.fontSize = (int)(guiHelper.btnScaledHeight) / 2;
            guiManager.Text_GameResult.color = guiHelper.textColor[0];
            guiManager.Text_GameResultTitle.fontSize = (int)guiHelper.btnScaledHeight;

            //ShadowAndOutline.DrawOutline(txtScore, score, style, guiHelper.outlineColor[gameEnd - 1], Color.white, 4);

            GameObject.Find("ContinueButton").SetActive(true);

            if (guiHelper.GetButtonPress("ContinueButton"))
            {
                guiHelper.SetButtonPress("ContinueButton", false);
                Application.LoadLevel("lobby");
            }
        }
        else //Is updating score, wait for it
        {
            guiManager.Text_GameResult.text = "Updating Score ... ";
            guiManager.Text_GameResult.fontSize = (int)(guiHelper.btnScaledHeight) / 2;
            guiManager.Text_GameResult.color = Color.gray;
            //ShadowAndOutline.DrawOutline(txtScore, "Updating score...", style, Color.black, Color.gray, 4);
        }
    }
}
