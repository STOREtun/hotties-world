using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PopupController : MonoBehaviour {

	// top
	[SerializeField] private Transform top_animateFrom;
	[SerializeField] private Transform top_animateTo;
	[SerializeField] private GameObject top_background;
	[SerializeField] private Image topPopupImage;
	[SerializeField] private Text topPopupText;

	// bottom
	[SerializeField] private GameObject bottomPopup;
	[SerializeField] private GameObject bottom_background;
	[SerializeField] private Image fannyReaction;
	[SerializeField] private GameObject bottomPopupButton_close;
	[SerializeField] private GameObject bottomPopupButton_next;
	[SerializeField] private Text bottomPopupText;
	[SerializeField] public Image bottomPopupImg;

	private int speed;

	private string[] texts;

	// Use this for initialization
	void Start () {
		speed = 1500;
	}

	/// <summary>
	/// Show the buttom popup
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="img">Image.</param>
	public void ShowBottomPopup(string[] texts, Sprite img){
		Main.instance.soundHandler.PlaySound (Sound.PRESENT_POPUP);

		bottomPopup.SetActive (true);
		fannyReaction.gameObject.SetActive(false);

		if(img != null){
			bottomPopupImg.gameObject.SetActive(true);
			bottomPopupImg.sprite = img;
		}else bottomPopupImg.gameObject.SetActive(false);

		bool twoTexts = texts.Length > 1;

		bottomPopupButton_close.SetActive(!twoTexts);
		bottomPopupButton_next.SetActive(twoTexts);

		bottomPopupText.text = texts[0];

		this.texts = texts;
	}

	public void AnimateBottomPopupWithFanny(string text, Sprite reaction){
		bottomPopupImg.gameObject.SetActive(false);
		fannyReaction.gameObject.SetActive(true);
		fannyReaction.sprite = reaction;

		bottomPopupText.text = text;
		bottomPopupButton_close.SetActive(true);
		bottomPopupButton_next.SetActive(false);
	}

	#region Buttons
	public void HideBottomPopup(){
		bottomPopup.SetActive (false);

		switch (Global.instance.gameState) {

		case GameState.FIND_HIDDEN_OBJECTS:
			Main.instance.level.SetCurrentObjective ();
			break;
		case GameState.FEED_AGENTS:
			Main.instance.level.SpawnNextHungryCustomer ();
			break;
		case GameState.CONSTRUCT_BUILDING:
			Main.instance.level.arrow.enabled = true;
			break;
		case GameState.FINISHED:
			SceneManager.LoadScene ("WorldMap");
			break;

		}
	}

	public void NextText(){
		bottomPopupText.text = texts [1];
		bottomPopupButton_close.SetActive(true);
		bottomPopupButton_next.SetActive(false);
	}
	#endregion

	#region Animating
	public void AnimateTopPopup(string text, Sprite img){
		if(img != null) topPopupImage.sprite = img;
		topPopupText.text = text;

		StartCoroutine (ShowTopPopup());
	}

	private IEnumerator ShowTopPopup(){
		float distance = Vector3.Distance (
			top_background.transform.position, 
			top_animateTo.position
		);

		while(distance > 0.1f){
			float step = speed * Time.deltaTime;
			top_background.transform.position = Vector3.MoveTowards(
				top_background.transform.position, 
				top_animateTo.position, 
				step
			);

			distance = Vector3.Distance (
				top_background.transform.position, 
				top_animateTo.position
			);

			yield return null;
		}

		StartCoroutine (HideTopPopup(1.5f));
	}

	private IEnumerator HideTopPopup(float startDelay){
		yield return new WaitForSeconds (startDelay);

		float distance = Vector3.Distance (
			top_background.transform.position, 
			top_animateFrom.position
		);

		while(distance > 0.1f){
			float step = speed * Time.deltaTime;
			top_background.transform.position = Vector3.MoveTowards(
				top_background.transform.position, 
				top_animateFrom.position, 
				step
			);

			distance = Vector3.Distance (
				top_background.transform.position, 
				top_animateFrom.position
			);

			yield return null;
		}
	}
	#endregion
}