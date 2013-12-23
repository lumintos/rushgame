using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	MovementController movement = new MovementController();

	private Animator anim;
	private AnimatorStateInfo currentBaseState;
	private GUIManager guiManager;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private Vector3 _destination;
    private string syncAnimation = "";
    private bool IsJump;

	public float turnSmoothly = 150.0f;
	public float speedDampTime = 0.1f;
	public float speedStopDampTime = 0.05f;
	public float speedMove = 5.3f;

	void Awake() {
        //TODO: make camera move along with player

		anim = GetComponent<Animator>();

		guiManager = GameObject.FindGameObjectWithTag(Tags.gui).GetComponent<GUIManager>();
		//guiManager.SetMaxHP(MaxHP);
		movement.initMovement(this.gameObject, anim);
		movement.gtext1 = gtext1;
	}

	void FixedUpdate() {
        if (networkView.isMine)
        {
            //get all inputs
            //orientation
            float h = Input.GetAxis("Horizontal");
            float hInt = Mathf.Clamp(h + guiManager.GetInputGUI_h(), -1.0f, 1.0f);

            //hInt only have 3 values: 0, -1 and 1
            /*	float hInt = 0.0f;
                if (h > 0.0f) {
                    hInt = 1.0f;
                }
                else if (h < 0.0f) {
                    hInt = -1.0f;
                }*/

            //jump
            IsJump = false;
            if (Input.GetButtonDown("Jump") || guiManager.GetInputGUI_v() != 0.0f)
            {
                IsJump = true;
            }
            
            movement.updateMovement(hInt, IsJump);
        }
        else
        {
            SyncedMovement();
        } 
	}

	void OnGUI()
	{
		//guiManager.UpdateHP(HP,-1);// negative is left HP, positive is right HP, depend on which side player is.
		guiManager.UpdateTouchInput();
	}

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        //char animation = 'x'; // idle
        bool isJump = false;
        if (stream.isWriting)
        {
            syncPosition = rigidbody.position;
            stream.Serialize(ref syncPosition);

            syncVelocity = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);


            //stream.Serialize(ref animation);

            isJump = IsJump;
            stream.Serialize(ref isJump);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            //stream.Serialize(ref animation);
            stream.Serialize(ref isJump);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = rigidbody.position;

            IsJump = isJump;
            
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
    }


	//dev options:
	public GUIText gtext1 = null;
}