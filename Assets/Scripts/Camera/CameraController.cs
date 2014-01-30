using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject mainPlayer = null;
	private Vector3 offset;
	GameController gameController;
	
	/// <summary>
	/// call this function when we add new character by code	
	/// </summary>
	public void addMainPlayer(GameObject player) {
		mainPlayer = player;
		offset = this.transform.position - mainPlayer.rigidbody.position;
        offset.x = 0; // character is at horizontal center of screen
	}

	void Start() {
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		if (gameController.isSoundEnable == true)
		{
			InGameAudioManager.Instance.PlayStartSfx(transform.position, 1.0f);

			if (InGameAudioManager.Instance.BackgroundCheck() == false) // no background created
				InGameAudioManager.Instance.PlayBackground(transform.position, 1.0f);
			else
				InGameAudioManager.Instance.ContinuePlayBackgroudMusic();
		}
		// to stop playing background -> Instance.StopBackgroundMusic()
		// to pause playing background -> Instance.PauseBackgroundMusic()
		if (mainPlayer != null) {
			offset = this.transform.position - mainPlayer.rigidbody.position;
		}
	}

	void LateUpdate () {
		if (mainPlayer != null) {
			this.transform.position = mainPlayer.rigidbody.position + offset;
		}

		if (gameController.isSoundEnable == true)
		{
			// reopen or play background music			
			if (InGameAudioManager.Instance.BackgroundCheck() == false) // no background created
				InGameAudioManager.Instance.PlayBackground(transform.position, 1.0f);
			else
				InGameAudioManager.Instance.ContinuePlayBackgroudMusic();
		}
		else
		{
			if (InGameAudioManager.Instance.BackgroundCheck() == true) // 
				InGameAudioManager.Instance.PauseBackgroundMusic();
		}
	}
}
