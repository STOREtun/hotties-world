using UnityEngine;
using System.Collections;

public class PopupController : MonoBehaviour {

	public Transform animateFrom;
	public Transform animateTo;

	public GameObject background;

	private bool play = false,
	reverse,
	dontReverse;
	private int speed;


	// Use this for initialization
	void Start () {
		speed	= 500;
	}

	// Update is called once per frame
	void Update () {
		if(play){
			float step = speed * Time.deltaTime;
	    background.transform.position = Vector3.MoveTowards(background.transform.position, animateTo.position, step);
			float dist = Vector3.Distance(background.transform.position, animateTo.position);

			if(dist == 0 && !dontReverse){
				play = false;
				Invoke("reverseAnimation", 5);
			}
		}else if(reverse){
			float step = speed * Time.deltaTime;
	    background.transform.position = Vector3.MoveTowards(background.transform.position, animateFrom.position, step);
			float dist = Vector3.Distance(background.transform.position, animateFrom.position);
			if(dist == 0) reverse = false;
		}
	}


	/** animates the popup from top into view
	*/
	public void animatePopup(bool _dontReverse = false){
		// animate the popup
		play = true;
		dontReverse = _dontReverse;
	}

	void reverseAnimation(){
		reverse = true;
	}
}
