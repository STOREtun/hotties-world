using UnityEngine;
using System.Collections;

public class CameraTestController : MonoBehaviour {

	private const int MIN = 2;
	private const int MAX = 20;

	private bool zoom;

	// Use this for initialization
	void Start () {
		Camera.main.orthographicSize = MIN;
		zoom = true;
	}

	// Update is called once per frame
	void Update () {
		 if(Camera.main.orthographicSize <= MAX && zoom){
			 Camera.main.orthographicSize += 0.05f;
		 }else zoom = false;

		 if(!zoom && Camera.main.orthographicSize >= MIN){
			 Camera.main.orthographicSize -= 0.05f;
		 }else zoom = true;
	}
}
