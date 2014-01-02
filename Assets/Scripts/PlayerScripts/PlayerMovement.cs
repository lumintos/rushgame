using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	//MovementController movement = new MovementController();

	private Animator anim;
	private AnimatorStateInfo currentBaseState;

//	private GameObject obj;
//	private Animator anim;
//	private AnimatorStateInfo currentBaseState;
	
	//rotate
	public float turnSmoothly = 1500.0f;
	
	//move
	public float velocityFactor = 3.0f; // this factor let's velocity * orientation (in range [-1; 1]) increase faster to maximum speed
	private float velocity = 0.0f;
	public float velocityMaximum = 8.0f;//5.3f;
	
	//jump
	public float jumpForce = 17.0f;
	public float jumpForceReduce = 0.7f;// reduce force for every physic frame
	private int jumpCount = 0;
	public int jumpCountMaximum = 2;
	private float jumpMove = 0.0f;
	private bool jumpButtonLock = false;//only unlock when release then re-press/touch jump button
	private bool IsKeyboardInput = true;//there are 2 types of input: by keys or by touch-button
	
	//pre-define only for this particular scene
	public Vector3 Vector3Forward { get { return new Vector3(1.0f, 0, 0); } }

	//control events for current animator
	AnimatorEvents animatorEvents;

	private GUIManager guiManager;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private Vector3 _destination;
    private bool IsJump;
    private float moveDirection = 0;



	void Awake() {
		anim = GetComponent<Animator>();

		guiManager = GameObject.FindGameObjectWithTag(Tags.gui).GetComponent<GUIManager>();
		//guiManager.SetMaxHP(MaxHP);
		//movement.initMovement(this.gameObject, anim);

		//control events for current animator
		animatorEvents = GetComponent<AnimatorEvents>();
	}

	void FixedUpdate() {
        GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (gameController.gameEnd)
            return;

        //States in server is the correct one for all network player (regardless networkView), all clients must follow
        if (Network.isServer)
            networkView.RPC("CorrectSyncedMovement", RPCMode.OthersBuffered, rigidbody.position);

        //Input only for network player of owner
        if (networkView.isMine)
        {
            //get all inputs
			//orientation, works with float range [-1.0f, 1.0f]
            float h = Input.GetAxis("Horizontal");
            float hInt = Mathf.Clamp(h + guiManager.GetInputGUI_h(), -1.0f, 1.0f);
            moveDirection = hInt;

			//only set IsJump = true when that button is release and re-press again
			if ((Input.GetButtonUp("Jump") && IsKeyboardInput) //if input from keyboard
			    || (guiManager.GetInputGUI_v() == 0.0f && !IsKeyboardInput)) //if input from touch-button 
			{
				jumpButtonLock = false;
			}
            //jump
            IsJump = false;
            if ((Input.GetButtonDown("Jump") || guiManager.GetInputGUI_v() != 0.0f)
			&& !jumpButtonLock)
            {
				IsJump = true;
				jumpButtonLock = true;
				IsKeyboardInput = Input.GetButtonDown("Jump") ? true : false;
            }
            
            //movement.updateMovement(hInt, IsJump);
			this.updateMovement(hInt, IsJump);
            //Call object instance in other game instances to perform exact movement
            networkView.RPC("MoveCommands", RPCMode.OthersBuffered, hInt, IsJump);            

        }
        /* else
        {
            //if (Network.isClient)
            //    SyncedMovement();
            if(Network.isServer)
                networkView.RPC("CorrectSyncedMovement", RPCMode.Others, rigidbody.position);
        } */
	}

	void OnGUI()
	{
		//guiManager.UpdateHP(HP,-1);// negative is left HP, positive is right HP, depend on which side player is.
		guiManager.UpdateTouchInput();
	}
    /*
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        //char animation = 'x'; // idle
        //bool isJump = false;
        //float direction = 0;
        if (stream.isWriting)
        {
            syncPosition = rigidbody.position;
            stream.Serialize(ref syncPosition);

            syncVelocity = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);


            //stream.Serialize(ref animation);

            //isJump = IsJump;
            //stream.Serialize(ref isJump);

            //direction = moveDirection;
            //stream.Serialize(ref direction);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            //stream.Serialize(ref animation);
            //stream.Serialize(ref isJump);
            //stream.Serialize(ref direction);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = rigidbody.position;

            //IsJump = isJump;
            //moveDirection = direction;
            
            //if (animation == 'a')
            //    syncAnimation = "run";
        }
    }

    
    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;
        //rigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
        //Assume no lag, no delay in network
        float h = (syncStartPosition - syncEndPosition).x;
        h = Mathf.Clamp(h, -1.0f, 1.0f);
        movement.updateMovement(h, IsJump);
        //if(rigidbody.position != syncEndPosition)
        //    rigidbody.position = Vector3.Lerp(rigidbody.position, syncEndPosition, syncTime / syncDelay);
    }
    */
    [RPC]
    private void MoveCommands(float horizontal, bool isJump)
    {
        //movement.updateMovement(horizontal, isJump);
		this.updateMovement(horizontal, isJump);
    }

    [RPC]
    private void CorrectSyncedMovement(Vector3 position)
    {
        //Each 3 seconds, the client must correst it's world state regarding to host's world state (only if the client's state is wrong)
        if (rigidbody.position != position)
        {
            syncTime += Time.deltaTime;
        }
        else
            syncTime = 0;

        if (syncTime >= 3) //3 seconds
        {
            if (rigidbody.position != position)
            {
                //rigidbody.position = Vector3.Lerp(rigidbody.position, position, Time.deltaTime);
                rigidbody.position = position;
            }
            syncTime = 0;
        }
    }

	//------------------------------------------------------------------------------
	//control events of current animator
	void OnEnable(){
		//EventtriggersfromAnimatorEvents
		animatorEvents.OnStateChanged += OnStateChanged;
		animatorEvents.OnTransition += OnTransition;
	}
	
	void OnDisable(){
		animatorEvents.OnStateChanged -= OnStateChanged;
		animatorEvents.OnTransition -= OnTransition;
	}
	//------------------------------------------------------------------------------
	/// <summary>
	/// Implemented by an animation event plugin. 
	/// If there is any change in state, this function will be called
	/// </summary>
	/// <param name="layer">Layer.</param>
	/// <param name="previous">Previous state machine</param>
	/// <param name="current">Current state machine</param>
	void OnStateChanged(int layer, AnimatorStateInfo previous,AnimatorStateInfo current){
		//This displays the State Info of previous and currentstates.
		//Debug.Log("State changed from" + previous + "to" + current);

		//AnimatorEvents returns a much friendly way than hash names
		//Debug.Log("State changed to" + animatorEvents.layers[layer].GetStateName(current.nameHash));
		 
		if(current.nameHash == PlayerHashIDs.jumpState) {
			//reset JumpBool in animator, in order to re-jump in jump or fall state
			anim.SetBool(PlayerHashIDs.JumpBool, false);
		}
		else if (current.nameHash == PlayerHashIDs.doubleJumpState) {
			//reset DoubleJump, so DoubleJump > FallState, FallState doesn't go back to DoubleState
			anim.SetBool(PlayerHashIDs.IsDoubleJump, false);
		}
		else if (current.nameHash == PlayerHashIDs.fallState) {
			//JumpState/DoubleJumpState > FallState: update down-force
			//this.rigidbody.velocity = this.rigidbody.velocity + Vector3.down * this.rigidbody.velocity.y;
			this.rigidbody.AddForce(Vector3.down * this.rigidbody.velocity.y, ForceMode.VelocityChange);

			Vector3 force = Vector3.zero;
			force += Vector3.down * jumpForce;
			force += Vector3Forward * jumpMove;
			print("jumpForce" + jumpForce + "jumpMove " + jumpMove);
			print("force "+ force);

			this.rigidbody.AddForce(force, ForceMode.VelocityChange);
			//this.rigidbody.velocity = new Vector3(jumpMove, -1.0f * jumpHeight, this.rigidbody.velocity.z);
		}
		else if (previous.nameHash == PlayerHashIDs.landState
		         && (current.nameHash == PlayerHashIDs.locomotionState
					|| current.nameHash == PlayerHashIDs.idleState)) {
			//after FallState: reset JumpingProcess
			this.jumpStateReset();
		}
		else if (previous.nameHash == PlayerHashIDs.fallState && current.nameHash == PlayerHashIDs.locomotionState) {
			this.jumpStateReset();
		}
	}
	
	void OnTransition(int layer, AnimatorTransitionInfo transitionInfo){
//		Debug.Log("Transition from"+ animatorEvents.layers[layer].GetTransitionName(transitionInfo.nameHash));
	}

	public void updateMovement(float horizontal, bool IsJump) {
		//get all inputs
		//get state
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
		
		MovementManagement(horizontal);
		jumpManagement(horizontal, IsJump);
		//		if (this.rigidbody.velocity.y > 0.1f || this.rigidbody.velocity.z > 0.1f) {
		//			print ("velocity: " + this.rigidbody.velocity + ", hInt " + horizontal);
		//		}
	}
	
	void MovementManagement(float orientation) {
		Rotation (orientation);
		if (orientation != 0.0f) {
			this.velocity = Mathf.Clamp(velocityFactor * velocityMaximum * orientation, -velocityMaximum, velocityMaximum);
		}
		else {
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
		//this.rigidbody.velocity +=  this.Vector3Forward * this.velocity;
		this.rigidbody.AddForce(this.Vector3Forward * this.velocity, ForceMode.VelocityChange);
		anim.SetFloat(PlayerHashIDs.speedFloat, Mathf.Abs(velocity));
		//changing the whole rigidbody by chaging the velocity, depend on mass
		//this.rigidbody.AddForce(Vector3.forward * orientation * (velocity * this.rigidbody.mass), ForceMode.Impulse);
		//this.rigidbody.AddForce(this.Vector3Forward * orientation * (velocity * this.rigidbody.mass) * 50.0f, ForceMode.Force);
		//don't know why we need alot of force to move character
		
		//with the velocity, its almost the same, I think
		//this.rigidbody.velocity = Vector3.forward * velocity * horizontal;
		//addForce applies into the value of rigid.velocity. Checked
		//print ("velocity: " + this.rigidbody.velocity);
	}

	/// <summary>
	/// Rotate character when orientation from negative to positive and vice versa
	/// </summary>
	/// <param name="orientation">Orientation.</param>
	void Rotation(float orientation) {
		if (orientation == 0.0f) return;
		Vector3 targetDirection = new Vector3(orientation, 0.0f, 0.0f);
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		
		Quaternion newRotation = Quaternion.Lerp(this.rigidbody.rotation, targetRotation, turnSmoothly * Time.deltaTime);
		this.rigidbody.MoveRotation(newRotation);
	}
	
	//---------------------------------------------
	//manage jumpState
	void jumpStateEnter() {
		anim.SetBool(PlayerHashIDs.JumpBool, true);

		//enable double jump animation
		if (jumpCount == 1) {
			print("double jump!");
			anim.SetBool(PlayerHashIDs.IsDoubleJump, true);
		}

		jumpCount++;
		jumpMove = velocity;

		Vector3 force = Vector3.zero;
		force += Vector3.up * jumpForce;
		force += Vector3Forward * jumpMove;
		
		//jumpForce = force for destroying gravity + force depend on vlocity and mass
		//idle, jump at force 5.0f, walk/run jump at force up to 5 + 5.3/2
		//jumpForce = 9.8f + 50.0f + this.rigidbody.mass * velocity / 2.0f;
		//this.rigidbody.velocity = new Vector3(this.rigidbody.velocity.x, jumpHeight, this.rigidbody.velocity.z);
		this.rigidbody.AddForce(force, ForceMode.VelocityChange);
	}
	
	void jumpStateReset() {
		anim.SetBool(PlayerHashIDs.JumpBool, false);
		anim.SetBool(PlayerHashIDs.FallToLandBool, false);
		anim.SetBool(PlayerHashIDs.IsDoubleJump, false);
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
			//check double jump
			if (IsJump && jumpCount < jumpCountMaximum) {
				this.jumpStateEnter();
				//anim.ForceStateNormalizedTime(0.0f); //function deprecated 
				anim.SetTarget(AvatarTarget.Root, 0.0f);
			}
			this.rigidbody.AddForce(Vector3.down * jumpForceReduce, ForceMode.VelocityChange);
			print("velocity " + this.rigidbody.velocity);
		}
		else if (currentBaseState.nameHash == PlayerHashIDs.fallState) {
			//check double jump
			if (IsJump && jumpCount < jumpCountMaximum) {
				//reset falling force
				//this.rigidbody.velocity = new Vector3(this.rigidbody.velocity.x, 0.0f, this.rigidbody.velocity.z);
				this.jumpStateEnter();
				print ("press jump in falling state");
			}
			
			// Raycast down from the center of the character.. 
			Ray ray = new Ray(this.transform.position + Vector3.up, -Vector3.up);
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
			
			//this.rigidbody.AddForce(Vector3.up * 0.0f, ForceMode.Impulse);
			print("velocity " + this.rigidbody.velocity);
		}
	}

}