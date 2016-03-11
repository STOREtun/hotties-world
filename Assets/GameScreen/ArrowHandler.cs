using UnityEngine;
using System.Collections;

public class ArrowHandler : MonoBehaviour {

	public Transform pointTo;

	private Camera cam;
	private Vector3 screenPos;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
	}

	// Update is called once per frame
	void Update () {
		screenPos = GetComponent<Camera>().WorldToViewportPoint(transform.position);
		if(screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1){
			print("ArrowHandler, within bounds");
			return;
		}else	print("ArrowHandler, out of bounds");

		transform.Rotate(0, Time.deltaTime, 1, Space.World);
	}
}
