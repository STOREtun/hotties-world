using UnityEngine;
using System.Collections;

public class TestAnimationScript : MonoBehaviour {

	private Animator _animator;
	private int frames;

	private const int TAPPING_BUFFER = 10;

	// Use this for initialization
	void Start () {
		frames = 0;
		_animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			_animator.SetBool("isTapping", true);
			frames = 0;
		}

		if(frames > TAPPING_BUFFER){
			frames = 0;
			_animator.SetBool("isTapping", false);
		}

		frames++;
	}
}
