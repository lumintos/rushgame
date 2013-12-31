using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject mainPlayer = null;
	private Vector3 offset;
	
	/// <summary>
	/// call this function when we add new character by code	
	/// </summary>
	public void addMainPlayer(GameObject player) {
		mainPlayer = player;
		offset = this.transform.position - mainPlayer.rigidbody.position;
        offset.x = 0; // character is at horizontal center of screen
	}

	void Start() {
		if (mainPlayer != null) {
			offset = this.transform.position - mainPlayer.rigidbody.position;
		}
	}

	void LateUpdate () {
		if (mainPlayer != null) {
			this.transform.position = mainPlayer.rigidbody.position + offset;
		}
	}
}
