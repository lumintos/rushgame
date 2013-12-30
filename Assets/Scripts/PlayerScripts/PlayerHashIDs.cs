using UnityEngine;
using System.Collections;

public class PlayerHashIDs : MonoBehaviour 
{
	public static int idleState 			= Animator.StringToHash("Base Layer.Idle");
	public static int locomotionState		= Animator.StringToHash("Base Layer.Locomotion");
	public static int jumpState				= Animator.StringToHash("JumpProcess.Jump");
	public static int fallState				= Animator.StringToHash("JumpProcess.Fall");
	public static int landState				= Animator.StringToHash("JumpProcess.Land");
	public static int doubleJumpState		= Animator.StringToHash("JumpProcess.DoubleJump");

	public static int speedFloat			= Animator.StringToHash("speed");
	public static int JumpBool				= Animator.StringToHash("Jump");
	public static int FallToLandBool		= Animator.StringToHash("FallToLand");
	public static int IsDoubleJump			= Animator.StringToHash("IsDoubleJump");
}