using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private GUIManager guiManager;
	
	// Use this for initialization
	void Start () {
		
		guiManager.UpdateGUIElementsSize(new Size(Screen.width,Screen.height));
	}
	
	void Awake(){
		
		guiManager = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIManager>();
	}
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
