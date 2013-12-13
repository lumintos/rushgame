using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	private Animator anim;
	private AnimatorStateInfo currentBaseState;
//	private PlayerHashIDs hash;

	public float turnSmoothly = 150.0f;
	public float speedDampTime = 0.1f;
	public float speedStopDampTime = 0.05f;
	public float speedMove = 5.3f;

	public CapsuleCollider col;

	void Awake() {
		anim = GetComponent<Animator>();
		col = GetComponent<CapsuleCollider>();
//		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<PlayerHashIDs>();
	}

	void FixedUpdate() {
		//get all inputs
		float h = Input.GetAxis("Horizontal");

		//get state
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation

		MovementManagement(h);
		jumpManagement();
	}

	void MovementManagement(float horizontal) {
		if (horizontal != 0.0f) {
			Rotation (horizontal);
			anim.SetFloat(PlayerHashIDs.speedFloat, speedMove, speedDampTime, Time.deltaTime);
		}
		else {
			anim.SetFloat(PlayerHashIDs.speedFloat, 0.0f, speedStopDampTime, Time.deltaTime);
		}

		//check vars
		if (gtext1 != null) {
			string str = "";
			str += "Time.deltaTime: \t" + Time.deltaTime;
			str += "\nhorizontal: \t\t" + horizontal;
			str += "\nspeed: \t\t" + anim.GetFloat(PlayerHashIDs.speedFloat);
			gtext1.text = str;
		}
	}

	void Rotation(float horizontal) {
		Vector3 targetDirection = new Vector3(0.0f, 0.0f, horizontal);
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothly * Time.deltaTime);
		rigidbody.MoveRotation(newRotation);
	}

	void jumpManagement() {
		if (currentBaseState.nameHash == PlayerHashIDs.locomotionState) {
			if (Input.GetButtonDown("Jump")) {
				anim.SetBool(PlayerHashIDs.JumpBool, true);
				//rigidbody.AddForce(new Vector3(1.0f, 200.0f, 1.0f));
			}
		}
		else if(currentBaseState.nameHash == PlayerHashIDs.jumpState)
		{
			//  ..and not still in transition..
			if(!anim.IsInTransition(0))
			{
				// reset the Jump bool so we can jump again, and so that the state does not loop 
				anim.SetBool("Jump", false);
				//col.height = 1.0f;
				//col.center.y -= 0.5f;
			}
			else {
				//col.height = 0.5f;
				//col.center.y += 0.5f;
			}

		}

	}

	//dev options:
	public GUIText gtext1 = null;
}