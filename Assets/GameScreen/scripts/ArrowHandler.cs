using UnityEngine;
using System.Collections;

public class ArrowHandler : MonoBehaviour {

	public Transform pointTo;

	private Camera cam;
	private Vector3 screenPos;

	// test variables
	private float journeyLength;
	private float startTime;
	private const float speed = 0.2f;

	private bool stop = false;

	// Use this for initialization
	void Start () {
		cam = Camera.main;

		journeyLength = Vector3.Distance(transform.position, pointTo.position);
		startTime = Time.time;

		// for some reason this fixes the rotation problem
		transform.rotation = new Quaternion(0, 0, 0, 0);
	}

	// TODO : the arrow needs to be rotated 180 degrees
	// TODO : stop the arrow when within distance (x) of target area
	// Update is called once per frame
	void Update () {
		float distance = Vector3.Distance(pointTo.position, transform.position);

		// rotate the arrow to look at target
		Quaternion rotation = Quaternion.LookRotation (pointTo.position - transform.position, transform.TransformDirection(Vector3.up));
    transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

		screenPos = cam.WorldToViewportPoint(transform.position);
		float xAxis = screenPos.x;
		float yAxis = screenPos.y;

		// move the arrow towards the building area
		if(xAxis >= 0.2f && xAxis <= 0.8f
		&& yAxis >= 0.3f && yAxis <= 1f){
			// in bounds of screen
			float distCovered = (Time.time - startTime) * speed;
      float fracJourney = (distCovered / journeyLength) / 10;
			if(distance >= 10)	transform.position = Vector3.Lerp(transform.position, pointTo.position, fracJourney);
		}else{
			// out of bounds - correcting the position of the arrow
			Vector3 newPos = Vector3.zero;

			// x: 0 … 1
			// y: 1
			//		…
			//		0
			if(xAxis <= 0.1f){
				newPos = Vector3.right;
			}else if(xAxis >= 0.8f){
				newPos = Vector3.left;
			}else if(yAxis <= 0.2f){
				newPos = Vector3.up;
			}else	if(yAxis >= 0.8f){
				newPos = Vector3.down;
			}
			transform.position = Vector3.Lerp(transform.position, transform.position + newPos, speed);

			// deactivate arrow if within range of pointTo (target)
			// …
		}
	}

	public void deactivateArrow(){
		gameObject.SetActive(false);
	}
}
