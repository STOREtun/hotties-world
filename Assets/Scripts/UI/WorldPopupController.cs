using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldPopupController : MonoBehaviour {

	[SerializeField] private Text myText;
	[SerializeField] private WorldScreen worldScreen;

	// Use this for initialization
	void Start () {
		SetupPopup ();
	}

	private void SetupPopup(){
		myText.text = GameConstants.POPUP_MESSAGE_WELCOME;
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