using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour {

	[SerializeField] private Sprite [] loadingFrames;

	private Image image;

	void Start () {
		image = GetComponent<Image> ();
		StartCoroutine (AnimateLoading());
	}

	IEnumerator AnimateLoading () {
		int index = 0;

		while (true) {
			if (index >= loadingFrames.Length) {
				index = 0;
			}

			image.sprite = loadingFrames [index];
			index++;

			yield return new WaitForSeconds (0.1f);
		}
	}
}