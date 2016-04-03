using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {

	// top
	public Transform 	top_animateFrom;
	public Transform 	top_animateTo;
	public GameObject top_background;
	public Image 			topPopupImage;
	public Text 			topPopupText;

	// bottom
	public Transform 	bottom_animateFrom;
	public Transform 	bottom_animateTo;
	public GameObject bottom_background;
	public Image 			bottomPopupImg;
	public GameObject bottomPopupButton_close;
	public GameObject bottomPopupButton_next;
	public Text 			bottomPopupText;

	private string 		secondText;

	private bool playBottomPopup = false,
	playTopPopup = false,
	reverseTopPopup,
	reverseBottomPopup = false;
	private int speed;


	// Use this for initialization
	void Start () {
		speed	= 1000;
	}

	// Update is called once per frame
	void Update () {
		// bottom popup
		if(playBottomPopup){ // play bottom
			float step = (speed * 1.5f) * Time.deltaTime;
	    bottom_background.transform.position = Vector3.MoveTowards(bottom_background.transform.position, bottom_animateTo.position, step);
			float dist = Vector3.Distance(bottom_background.transform.position, bottom_animateTo.position);

			if(dist == 0){
				playBottomPopup = false;
			}
		}else if(reverseBottomPopup){ // reverse bottom
			float step = (speed * 1.5f) * Time.deltaTime;
	    bottom_background.transform.position = Vector3.MoveTowards(bottom_background.transform.position, bottom_animateFrom.position, step);
			float dist = Vector3.Distance(bottom_background.transform.position, bottom_animateFrom.position);

			if(dist == 0){
				reverseBottomPopup = false;
			}
		}

		// top popup
		if(playTopPopup){ // play top
			float topStep = speed * Time.deltaTime;
	    top_background.transform.position = Vector3.MoveTowards(top_background.transform.position, top_animateTo.position, topStep);
			float topDist = Vector3.Distance(top_background.transform.position, top_animateTo.position);

			if(topDist == 0){
				playTopPopup = false;
				Invoke("startReverseTopPopup", 1.5f);
			}
		}else if(reverseTopPopup){ // play top
			float _topStep = speed * Time.deltaTime;
	    top_background.transform.position = Vector3.MoveTowards(top_background.transform.position, top_animateFrom.position, _topStep);
			float _topDist = Vector3.Distance(top_background.transform.position, top_animateFrom.position);

			if(_topDist == 0){
				reverseTopPopup = false;
			}
		}
	}

	/** animates the popup from bottom into view
			this popup does not reverseTopPopup animate
			it must be clicked away
	*/
	public void animateBottomPopup(string text, Sprite img){
		if(img != null) bottomPopupImg.sprite = img;
		bottomPopupText.text = text;
		bottomPopupButton_close.SetActive(true);
		bottomPopupButton_next.SetActive(false);

		playBottomPopup = true;
	}

	public void animateBottomPopupWithSecondText(string text, string _secondText, Sprite img){
		if(img != null) bottomPopupImg.sprite = img;
		bottomPopupText.text = text;
		secondText = _secondText;
		bottomPopupButton_close.SetActive(false);
		bottomPopupButton_next.SetActive(true);

		playBottomPopup = true;
	}

	public void hideBottomPopup(){
		playBottomPopup = false;
		reverseBottomPopup = true;
	}

	public void presentSecondText(){
		if(secondText != null){
			bottomPopupText.text = secondText;
			bottomPopupButton_next.SetActive(false);
			bottomPopupButton_close.SetActive(true);
		}
	}

	public void animateTopPopup(string text, Sprite img){
		if(img != null) topPopupImage.sprite = img;
		topPopupText.text = text;
		playTopPopup = true;
	}

	void startReverseTopPopup(){
		reverseTopPopup = true;
	}
}
