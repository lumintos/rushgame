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
		//orientation
		float h = Input.GetAxis("Horizontal");
		//hInt only have 3 values: 0, -1 and 1
		float hInt = 0.0f;
		if (h > 0.0f) {
			hInt = 1.0f;
		}
		else if (h < 0.0f) {
			hInt = -1.0f;
		}

		//jump
		bool IsJump = false;
		if (Input.GetButtonDown("Jump")) {IsJump = true;}

		movement.updateMovement(hInt, IsJump);
	}

	//dev options:
	public GUIText gtext1 = null;
}