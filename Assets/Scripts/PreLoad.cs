using UnityEngine;
using System.Collections;

public class PreLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LoadScene(0);
	}

    /// <summary>
    /// Flexible loading chosen scene before build
    /// </summary>
    /// <param name="flag">0 for normal, require login; 1 for test mode, no login</param>
    void LoadScene(int flag)
    {
        switch (flag)
        {
            case 0: //normal, login and play
                {
                    Application.LoadLevel("login");
                    break;
                }
            case 1: //test, not login, use dummy account
                {
                    string username = "Tester";
                    username += Random.Range(0, 10);
                    MultiplayerManager.Instance.PlayerName = username;
                    PlayerPrefs.SetString("PlayerName", MultiplayerManager.Instance.PlayerName);
                    Application.LoadLevel("lobby");
                    break;
                }
            default: break;
        }
    }
}
