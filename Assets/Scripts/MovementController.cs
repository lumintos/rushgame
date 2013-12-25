using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

	private GameObject obj;
	private Animator anim;
	private AnimatorStateInfo currentBaseState;

	//rotate
	public float turnSmoothly = 1500.0f;

	//move
	public float velocityFactor = 3.0f; // this factor let's velocity * orientation (in range [-1; 1]) increase faster to maximum speed
	private float velocity = 0.0f;
	private float velocityMaximum = 5.3f;

	//jump
	public float jumpHeight = 9.0f;
	private int jumpCount = 0;
	public int jumpCountMaximum = 2;

	//pre-define only for this particular scene
	public Vector3 Vector3Forward { get { return new Vector3(1.0f, 0, 0); } }

	public void initMovement(GameObject objInit, Animator animInit) {
		obj = objInit;
		anim = animInit;
	}

	public void updateMovement(float horizontal, bool IsJump) {
		//reverse orientation because x-axis in this scene
		horizontal = -horizontal;

		//get all inputs
		//get state
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation

		MovementManagement(horizontal);
		jumpManagement(horizontal, IsJump);
//		if (obj.rigidbody.velocity.y > 0.1f || obj.rigidbody.velocity.z > 0.1f) {
//			print ("velocity: " + obj.rigidbody.velocity + ", hInt " + horizontal);
//		}
		print ("velocity: " + obj.rigidbody.velocity + ", hInt " + horizontal);
	}

	void MovementManagement(float orientation) {
		Rotation (orientation);
		if (orientation != 0.0f) {
			this.velocity = Mathf.Clamp(velocityFactor * velocityMaximum * orientation, -velocityMaximum, velocityMaximum);
			//anim.SetFloat(PlayerHashIDs.speedFloat, velocity);//, speedDampTime, Time.deltaTime);
		}
		else {
			//anim.SetFloat(PlayerHashIDs.speedFloat, velocity);//0.0f, speedStopDampTime, Time.deltaTime);
			//not set velocity to zero immediately, but slow it down a bit
			//It's solve the problem: when we change the orientation, 
			//	there is 1 frame that the orientation becomes 0, 
			//	character's state change to idle before back to locomotion
			if (Mathf.Abs(velocity) < 0.05) {
				velocity = 0.0f;
			}
			else {
				velocity /= 2.0f; // around 5 physic frames
			}
		}

		//if set value into velocity, the value will reset each frame
		obj.rigidbody.velocity +=  this.Vector3Forward * this.velocity;
		anim.SetFloat(PlayerHashIDs.speedFloat, Mathf.Abs(velocity));
		//changing the whole rigidbody by chaging the velocity, depend on mass
		//obj.rigidbody.AddForce(Vector3.forward * orientation * (velocity * obj.rigidbody.mass), ForceMode.Impulse);
		//obj.rigidbody.AddForce(this.Vector3Forward * orientation * (velocity * obj.rigidbody.mass) * 50.0f, ForceMode.Force);
		//don't know why we need alot of force to move character

		//with the velocity, its almost the same, I think
		//obj.rigidbody.velocity = Vector3.forward * velocity * horizontal;
		//addForce applies into the value of rigid.velocity. Checked
		//print ("velocity: " + obj.rigidbody.velocity);
	}
	
	void Rotation(float orientation) {
		Vector3 targetDirection = new Vector3(orientation, 0.0f, 0.0f);
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		
		Quaternion newRotation = Quaternion.Lerp(obj.rigidbody.rotation, targetRotation, turnSmoothly * Time.deltaTime);
		obj.rigidbody.MoveRotation(newRotation);
	}

	//---------------------------------------------
	//manage jumpState
	void jumpStateEnter() {
		anim.SetBool(PlayerHashIDs.JumpBool, true);
		jumpCount++;

		//jumpForce = force for destroying gravity + force depend on vlocity and mass
		//idle, jump at force 5.0f, walk/run jump at force up to 5 + 5.3/2
		//jumpForce = 9.8f + 50.0f + obj.rigidbody.mass * velocity / 2.0f;
		obj.rigidbody.velocity = new Vector3(obj.rigidbody.velocity.x, jumpHeight, obj.rigidbody.velocity.z);
	}
	
	void jumpStateReset() {
		anim.SetBool(PlayerHashIDs.JumpBool, false);
		anim.SetBool(PlayerHashIDs.FallToLandBool, false);
		jumpCount = 0;
	}
	//---------------------------------------------

	void jumpManagement(float orientation, bool IsJump) {
		//three basic steps for jumping process
		//step 1: jump with a vector-up-force and vector-forward-force, controlled by orientation, in 1 second
		//step 2: fall down with a raycast, change to landing state (FallToLand = true) when almost ground
		//step 3: do the animation, reset variables (jumpCount = 0, FallToLand = false)

		if (currentBaseState.nameHash == PlayerHashIDs.locomotionState
		    || currentBaseState.nameHash == PlayerHashIDs.idleState) {
			if (IsJump) {
				this.jumpStateEnter();
			}
		}
		else if(currentBaseState.nameHash == PlayerHashIDs.jumpState)
		{
			anim.SetBool(PlayerHashIDs.JumpBool, false);
			//jumpForce -= 0.07f * 50.0f;//stronger force, but decreasing over time
			//check double jump
			if (IsJump && jumpCount < jumpCountMaximum) {
				this.jumpStateEnter();
			}

			//obj.rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Force);
			//obj.rigidbody.AddForce(Vector3.up * jumpForce / 50.0f, ForceMode.Impulse);
		}
		else if (currentBaseState.nameHash == PlayerHashIDs.fallState) {
			obj.rigidbody.velocity = new Vector3(obj.rigidbody.velocity.x, -1.0f*jumpHeight, obj.rigidbody.velocity.z);
			//check double jump
			if (IsJump && jumpCount < jumpCountMaximum) {
				this.jumpStateEnter();
			}

			// Raycast down from the center of the character.. 
			Ray ray = new Ray(obj.transform.position + Vector3.up, -Vector3.up);
			RaycastHit hitInfo = new RaycastHit();
			
			if (Physics.Raycast(ray, out hitInfo))
			{
				if (hitInfo.distance < 1.2f) {//this value may change depend on character's center
					anim.SetBool(PlayerHashIDs.FallToLandBool, true);
				}
				// ..if distance to the ground is more than 1.75, use Match Target
//				if (hitInfo.distance > 1.75f)
//				{
//					// MatchTarget allows us to take over animation and smoothly transition our character towards a location - the hit point from the ray.
//					// Here we're telling the Root of the character to only be influenced on the Y axis (MatchTargetWeightMask) and only occur between 0.35 and 0.5
//					// of the timeline of our animation clip
//					anim.MatchTarget(hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.35f, 0.5f);
//				}
			}

			//obj.rigidbody.AddForce(Vector3.up * 0.0f, ForceMode.Impulse);
		}
		else if (currentBaseState.nameHash == PlayerHashIDs.landState) {
			this.jumpStateReset();
		}
	}
}
