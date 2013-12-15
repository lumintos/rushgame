using UnityEngine;
using System.Collections;

public class PlayerHashIDs : MonoBehaviour 
{
	public static int idleState 			= Animator.StringToHash("Base Layer.Idle");
	public static int locomotionState		= Animator.StringToHash("Base Layer.Locomotion");
	public static int jumpState				= Animator.StringToHash("Base Layer.Jump");

	public static int speedFloat			= Animator.StringToHash("speed");
	public static int JumpBool				= Animator.StringToHash("Jump");
}
