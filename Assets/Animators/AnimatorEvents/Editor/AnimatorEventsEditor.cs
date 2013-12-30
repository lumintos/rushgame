using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CustomEditor (typeof(AnimatorEvents))]
public class AnimatorEventsEditor : Editor {
	AnimatorEvents animatorEvents;

	void OnEnable() {
		animatorEvents = target as AnimatorEvents;
		animatorEvents.animator = animatorEvents.gameObject.GetComponent<Animator>();
		if (animatorEvents.CheckRedudancy())
			return;
	}
	
	public override void OnInspectorGUI () {
		if (!Application.isPlaying) {
			if (GUILayout.Button("Update From Animator"))
				animatorEvents.layers = GetAnimatorLayers();
		}
		
		if (animatorEvents.animator == null)
			return;
		
		if (animatorEvents.layers == null)
			return;
		
		string[] layerNames = GetLayerNames(animatorEvents.animator);
	
		for (int i = 0; i < animatorEvents.layers.Length; i++) {
			
			// Draw Layer Foldout
			animatorEvents.layers[i].foldLayer = EditorGUILayout.Foldout(animatorEvents.layers[i].foldLayer, layerNames[i]);
			if (animatorEvents.layers[i].foldLayer) {
				animatorEvents.layers[i].isListening = EditorGUILayout.Toggle("Listen to Events", animatorEvents.layers[i].isListening);
				
				// Draw States Foldout
				animatorEvents.layers[i].foldStates = EditorGUILayout.Foldout(animatorEvents.layers[i].foldStates, "States(" + animatorEvents.layers[i]._stateKeys.Length.ToString() + ")");
				if (animatorEvents.layers[i].foldStates) {
					EditorGUILayout.LabelField("\t" + "Hash Name", "Unique Name");
					for (var j = 0; j < animatorEvents.layers[i]._stateKeys.Length; j++) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("\t" + animatorEvents.layers[i]._stateKeys[j].ToString(), animatorEvents.layers[i]._stateNames[j]);
						EditorGUILayout.EndHorizontal();
					}
				}
				
				//Draw Transition Foldout
				animatorEvents.layers[i].foldTransitions = EditorGUILayout.Foldout(animatorEvents.layers[i].foldTransitions, "Transitions(" + animatorEvents.layers[i]._transitionKeys.Length.ToString() + ")");
				if (animatorEvents.layers[i].foldTransitions) {
					EditorGUILayout.LabelField("\t" + "Hash Name", "Unique Name");
					for (var k = 0; k < animatorEvents.layers[i]._transitionKeys.Length; k++) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("\t" + animatorEvents.layers[i]._transitionKeys[k].ToString(), animatorEvents.layers[i]._transitionNames[k]);
						EditorGUILayout.EndHorizontal();
					}	
				}
			}	
			
		}
	}
	
	public AnimatorEventLayer[] GetAnimatorLayers() {
		List<AnimatorEventLayer> animatorLayers = new List<AnimatorEventLayer>();
		for (int i = 0; i < GetLayerCount(animatorEvents.animator); i++) {
			animatorLayers.Add (new AnimatorEventLayer (
														GetStateKeys(animatorEvents.animator, i),
														GetStateNames(animatorEvents.animator, i),
														GetTransitionKeys(animatorEvents.animator, i),
														GetTransitionNames(animatorEvents.animator, i)));
		}
		return animatorLayers.ToArray();
	}

	#region Animator Layer Methods
	
	/// <summary>
	/// Number of layers.
	/// </summary>
	/// <returns>
	/// The layer count.
	/// </returns>
	/// <param name='animator'>
	/// Animator.
	/// </param>
	private static int GetLayerCount (Animator animator) {
		//AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
		return animator.layerCount;
		//return animatorController.GetLayerCount();
	}
	
	/// <summary>
	/// Get all the layer names.
	/// </summary>
	/// <returns>
	/// The layer names.
	/// </returns>
	/// <param name='animator'>
	/// Animator.
	/// </param>
	private static string[] GetLayerNames (Animator animator) {
//		AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
		
		List<string> layerNames = new List<string>();
		
//		for (int i = 0; i < animatorController.GetLayerCount(); i++)
//			layerNames.Add(animatorController.GetLayerName(i));
		for (int i = 0; i < animator.layerCount; i++)
			layerNames.Add(animator.GetLayerName(i));

		return layerNames.ToArray();
	}
	
	#endregion
	
	#region Animator State Methods	
	private static void FromStateMachineToStateKey(StateMachine stateMachine, List<int> stateKeys) {

		for (int i = 0; i < stateMachine.stateCount; i++) {
			stateKeys.Add(stateMachine.GetState(i).uniqueNameHash);
		}

		for (int i = 0; i < stateMachine.stateMachineCount; i++) {
			FromStateMachineToStateKey(stateMachine.GetStateMachine(i), stateKeys);
		}
	}
	private static int[] GetStateKeys (Animator animator, int layer) {
		List<int> stateKeys = new List<int>();

		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		UnityEditorInternal.StateMachine stateMachine = ac.GetLayer(layer).stateMachine;

		FromStateMachineToStateKey(stateMachine, stateKeys);
		
		return stateKeys.ToArray();
	}

	private static void FromStateMachineToStateName(StateMachine stateMachine, List<string> stateNames) {
		
		for (int i = 0; i < stateMachine.stateCount; i++) {
			stateNames.Add(stateMachine.GetState(i).uniqueName);
		}
		
		for (int i = 0; i < stateMachine.stateMachineCount; i++) {
			FromStateMachineToStateName(stateMachine.GetStateMachine(i), stateNames);
		}
	}
	private static string[] GetStateNames (Animator animator, int layer) {
		List<string> stateNames = new List<string>();
		
		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		UnityEditorInternal.StateMachine stateMachine = ac.GetLayer(layer).stateMachine;

		FromStateMachineToStateName(stateMachine, stateNames);

		return stateNames.ToArray();
	}
	#endregion
	
	#region Animator Transition Methods
	private static void FromStateMachineToTransitionKey(StateMachine stateMachine, List<int> transitionKeys) {
		
		for (int i = 0; i < stateMachine.stateCount; i++) {
			Transition[] trans = stateMachine.GetTransitionsFromState(stateMachine.GetState(i));
			foreach(Transition tran in trans) {
				transitionKeys.Add(tran.uniqueNameHash);
			}
		}
		
		for (int i = 0; i < stateMachine.stateMachineCount; i++) {
			FromStateMachineToTransitionKey(stateMachine.GetStateMachine(i), transitionKeys);
		}
	}
	private static int[] GetTransitionKeys (Animator animator, int layer) {
		List<int> transitionKeys = new List<int>();
		
		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		UnityEditorInternal.StateMachine stateMachine = ac.GetLayer(layer).stateMachine;

		FromStateMachineToTransitionKey(stateMachine, transitionKeys);

		return transitionKeys.ToArray();
	}

	private static void FromStateMachineToTransitionName(StateMachine stateMachine, List<string> transitionNames) {
		
		for (int i = 0; i < stateMachine.stateCount; i++) {
			Transition[] trans = stateMachine.GetTransitionsFromState(stateMachine.GetState(i));
			foreach(Transition tran in trans) {
				transitionNames.Add(tran.uniqueName);
			}
		}
		
		for (int i = 0; i < stateMachine.stateMachineCount; i++) {
			FromStateMachineToTransitionName(stateMachine.GetStateMachine(i), transitionNames);
		}
	}
	
	private static string[] GetTransitionNames (Animator animator, int layer) {
		List<string> transitionNames = new List<string>();
		
		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		UnityEditorInternal.StateMachine stateMachine = ac.GetLayer(layer).stateMachine;

		FromStateMachineToTransitionName(stateMachine, transitionNames);
		
		return transitionNames.ToArray();
	}
	
	/// <summary>
	/// Gets the count of transitions in a layer.
	/// </summary>
	/// <returns>
	/// The transition count.
	/// </returns>
	/// <param name='animator'>
	/// Animator.
	/// </param>
	/// <param name='layer'>
	/// Layer.
	/// </param>
	public static int GetTransitionsCount (Animator animator, int layer) {
		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		UnityEditorInternal.StateMachine stateMachine = ac.GetLayer(layer).stateMachine;

		int counter = 0;
		for (int i = 0; i < stateMachine.stateCount; i++) {
			Transition[] trans = stateMachine.GetTransitionsFromState(stateMachine.GetState(i));
			counter += trans.Length;
		}

		return counter;

//		return stateMachine.transitionCount;
	}
	
	#endregion
	
	[MenuItem("Component/Miscellaneous/AnimatorEvents")]
    static void AddComponent()
    {
		if (Selection.activeGameObject != null) {
			if (Selection.activeGameObject.GetComponent<AnimatorEvents>() == null)
				Selection.activeGameObject.AddComponent(typeof(AnimatorEvents));
			else
				Debug.LogError("Can have only one AnimatorEvents");
		}
    }
}
