using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	MovementController movement = new MovementController();

	private Animator anim;
	private AnimatorStateInfo currentBaseState;

	public float turnSmoothly = 150.0f;
	public float speedDampTime = 0.1f;
	public float speedStopDampTime = 0.05f;
	public float speedMove = 5.3f;

	void Awake() {
		anim = GetComponent<Animator>();

		movement.initMovement(this.gameObject, anim);
		movement.gtext1 = gtext1;
	}

	void FixedUpdate() {
		//get all inputs
		float h = Input.GetAxis("Horizontal");
		float hInt = 0.0f;
		if (h > 0.0f) {
			hInt = 1.0f;
		}
		else if (h < 0.0f) {
			hInt = -1.0f;
		}
		movement.updateMovement(hInt);
	}

	//dev options:
	public GUIText gtext1 = null;
}