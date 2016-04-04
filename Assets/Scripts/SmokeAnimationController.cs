using UnityEngine;
using System.Collections;

public class SmokeAnimationController : MonoBehaviour {

	public GameObject smokeBackground;
	public GameObject smokePart_00;
	public GameObject smokePart_01;
	public GameObject smokePart_02;
	public GameObject smokePart_03;

	private bool animate 	= false;
	private int count 		= 0;
	private int interval 	= 10;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(animate){
			if(count < interval){
				smokePart_00.transform.position += new Vector3(0.1f, 0.1f, 0);
				smokePart_01.transform.position += new Vector3(-0.1f, 0, 0);
				smokePart_02.transform.position += new Vector3(0.1f, -0.1f, 0);
				smokePart_03.transform.position += new Vector3(0.1f, 0, 0);
				count++;
			}else if(count < interval * 2){ // * 2 because it should animate close again at the same rate
				smokePart_00.transform.position += new Vector3(-0.1f, -0.1f, 0);
				smokePart_01.transform.position += new Vector3(0.1f, 0, 0);
				smokePart_02.transform.position += new Vector3(-0.1f, 0.1f, 0);
				smokePart_03.transform.position += new Vector3(-0.1f, 0, 0);
				count++;
			}else count = 0;
		}
	}

	public void begin(){
		if(animate) return;

		fadeInSmoke();
		animate = true;

		// stop it because it needs constant clicking in order to animate
		Invoke("stop", 1);
	}

	public void stop(){
		fadeOutSmoke();
	}

	public void showBackgroundSmoke(){
		smokeBackground.SetActive(true);
		StartCoroutine(fadeInObject(smokeBackground));
	}

	public void fadeInSmoke(){
		StartCoroutine(fadeInObject(smokePart_00));
		StartCoroutine(fadeInObject(smokePart_01));
		StartCoroutine(fadeInObject(smokePart_02));
		StartCoroutine(fadeInObject(smokePart_03));
	}

	public void fadeOutSmoke(){
		StartCoroutine(fadeOutObject(smokePart_00));
		StartCoroutine(fadeOutObject(smokePart_01));
		StartCoroutine(fadeOutObject(smokePart_02));
		StartCoroutine(fadeOutObject(smokePart_03));
	}

	// fade
	public IEnumerator fadeInObject(GameObject objectToFade){
		SpriteRenderer sp = objectToFade.GetComponent<SpriteRenderer>();

		sp.color = new Color(1f, 1f, 1f, 0.1f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.2f);
		yield return new WaitForSeconds(.01f);
		sp.color = new Color(1f, 1f, 1f, 0.3f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.4f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.5f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.6f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.7f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.8f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.9f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 1f);
	}

	public IEnumerator fadeOutObject(GameObject objectToFade){
		SpriteRenderer sp = objectToFade.GetComponent<SpriteRenderer>();

		sp.color = new Color(1f, 1f, 1f, 0.9f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.8f);
		yield return new WaitForSeconds(.01f);
		sp.color = new Color(1f, 1f, 1f, 0.7f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.6f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.5f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.4f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.3f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.2f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0.1f);
		yield return new WaitForSeconds(.05f);
		sp.color = new Color(1f, 1f, 1f, 0);

		animate = false;
	}
}
