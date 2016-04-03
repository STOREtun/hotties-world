using UnityEngine;
using System.Collections;

public class FingerController : MonoBehaviour {

	public GameObject text;
	public GameObject fingerSprite;

	private bool animate = false;
	private int count = 0;

	public int interval = 10;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if(animate){
			float dist 	= Vector3.Distance(fingerSprite.transform.position, text.transform.position);
			float val 	= dist/400;

			if(count < interval){
				fingerSprite.transform.position 				+= new Vector3(0.1f, -0.1f, 0);
				text.transform.localScale -= new Vector3(val, val, val);
				count++;
			}else if(count < interval * 2){
				fingerSprite.transform.position 				+= new Vector3(-0.1f, 0.1f, 0);
				text.transform.localScale += new Vector3(val, val, val);
				count++;
			}else count = 0;
		}
	}

	public void begin(bool oneClick = false){
		gameObject.SetActive(true);
		animate = true;
	}

	public void stop(){
		gameObject.SetActive(false);
		animate = false;
	}
}
