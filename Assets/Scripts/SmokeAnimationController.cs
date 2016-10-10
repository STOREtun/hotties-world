using UnityEngine;
using System.Collections;

public class SmokeAnimationController : MonoBehaviour {

	public GameObject smokeBackground;
	public GameObject smokePart_00;
	public GameObject smokePart_01;
	public GameObject smokePart_02;
	public GameObject smokePart_03;

	private bool animate = false;
	private int count = 0;
	private int interval= 10;

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

	public void Begin(){
		if(animate) return;

		FadeInSmoke();
		animate = true;

		// stop it because it needs constant clicking in order to animate
		Invoke("Stop", 1.0f);
	}

	public void Stop(){
		FadeOutSmoke();
	}

	public void ShowBackgroundSmoke(){
		smokeBackground.SetActive(true);
		StartCoroutine(FadeInObject(smokeBackground));
	}

	public void FadeInSmoke(){
		StartCoroutine(FadeInObject(smokePart_00));
		StartCoroutine(FadeInObject(smokePart_01));
		StartCoroutine(FadeInObject(smokePart_02));
		StartCoroutine(FadeInObject(smokePart_03));
	}

	public void FadeOutSmoke(){
		StartCoroutine(FadeOutObject(smokePart_00));
		StartCoroutine(FadeOutObject(smokePart_01));
		StartCoroutine(FadeOutObject(smokePart_02));
		StartCoroutine(FadeOutObject(smokePart_03));
	}

	// fade
	public IEnumerator FadeInObject(GameObject objectToFade){
		SpriteRenderer sp = objectToFade.GetComponent<SpriteRenderer>();
		sp.color = new Color (1, 1, 1, 0);

		Color c = new Color (0, 0, 0, .1f);
		for(int x = 0; x <= 10; x++){
			sp.color += c;
			yield return new WaitForSeconds(.05f);
		}
	}

	public IEnumerator FadeOutObject(GameObject objectToFade){
		SpriteRenderer sp = objectToFade.GetComponent<SpriteRenderer>();

		Color c = new Color (0, 0, 0, .1f);
		for(int x = 0; x <= 10; x++){
			sp.color -= c;
			yield return new WaitForSeconds (.05f);
		}

		animate = false;
	}
}
