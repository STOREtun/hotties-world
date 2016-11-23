using UnityEngine;
using System.Collections;

public class SoundHandler : MonoBehaviour{

	[SerializeField] private AudioSource backgroundMusic;
	[SerializeField] private AudioSource sfxSource;
	[SerializeField] private AudioSource sfxSource_02;

	[SerializeField] private AudioClip presentSound;
	[SerializeField] private AudioClip foundObjectSound;

	[SerializeField] private AudioClip buildClip;
	[SerializeField] private AudioClip buildClickClip;

	[SerializeField] private AudioClip finishedClip;

	void Start () {
		DontDestroyOnLoad (gameObject);

		backgroundMusic.Play ();
	}

	public void PlaySound (Sound sound){
		AudioClip clip = null;

		switch (sound) {

		case Sound.PRESENT_POPUP:
			bool gameFinished = Global.instance.gameState == GameState.FINISHED;
			clip = gameFinished ? finishedClip : presentSound;

			break;

		case Sound.CLICK_FOUND_OBJECT:
			clip = foundObjectSound;
			break;

		case Sound.CONSTRUCTION_CLICK:
			clip = buildClickClip;
			break;

		case Sound.CONSTRUCTION_DONE:
			PlayFinalConstructionSound ();
			break;
		}

		if (clip != null) {
			sfxSource.clip = clip;
			sfxSource.Play ();
		}
	}

	void PlayFinalConstructionSound () {
		sfxSource_02.clip = buildClip;
		sfxSource_02.Play ();
	}
}

public enum Sound { 
	PRESENT_POPUP,
	CLICK_FOUND_OBJECT,
	CONSTRUCTION_CLICK,
	CONSTRUCTION_DONE
}