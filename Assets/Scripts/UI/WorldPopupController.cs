using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldPopupController : MonoBehaviour {

	[SerializeField] private Text myText;
	[SerializeField] private WorldScreen worldScreen;

	// Use this for initialization
	void Start () {
		int completedLevels = Global.instance.completedLevels;
		if (completedLevels <= 0) { // no levels completed
			SetupPopup (GameConstants.POPUP_MESSAGE_WELCOME);
		} else if (completedLevels >= 2) {
			SetupPopup (GameConstants.POPUP_MESSAGE_GAME_DONE);
		} else {
			Dismiss ();
		}
	}

	private void SetupPopup(string message){
		myText.text = message;
		gameObject.SetActive(true);
	}

	private void NotifyWorldScreen(){
		worldScreen.MoveCameraToCurrentLocation ();
	}

	public void Dismiss(){
		gameObject.SetActive (false);
		NotifyWorldScreen ();
	}
	
}