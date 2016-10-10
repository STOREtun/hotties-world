using UnityEngine;
using System.Collections;

public class FingerController : MonoBehaviour {

	public GameObject textSprite;
	public GameObject fingerSprite;

	public Sprite oneClick;
	public Sprite fastClick;

	private bool animate 	= false;
	private int count 		= 0;
	private int interval 	= 10;

	public enum Tempo{FAST, SLOW};

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(animate){
			float dist 	= Vector3.Distance(fingerSprite.transform.position, textSprite.transform.position);
			float val 	= dist/400;

			if(count < interval){
				fingerSprite.transform.position += new Vector3(0.1f, -0.1f, 0);
				textSprite.transform.localScale -= new Vector3(val, val, val);
				count++;
			}else if(count < interval * 2){ // * 2 because it should animate close again at the same rate
				fingerSprite.transform.position += new Vector3(-0.1f, 0.1f, 0);
				textSprite.transform.localScale += new Vector3(val, val, val);
				count++;
			}else count = 0;
		}
	}

	// default is fast clicking
	public void begin(Tempo tempo){
		if(tempo == Tempo.SLOW){ // activate one click animation
			textSprite.GetComponent<SpriteRenderer>().sprite = oneClick;

			gameObject.SetActive(true);
			animate = true;
		}else if(tempo == Tempo.FAST){ // activate fast clicking animation
			textSprite.GetComponent<SpriteRenderer>().sprite = fastClick;

			gameObject.SetActive(true);
			animate = true;
		}
	}

	public void Stop(){
		gameObject.SetActive(false);
		animate = false;
	}
}
