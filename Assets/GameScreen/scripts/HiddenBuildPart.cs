using UnityEngine;
using System.Collections;

public class HiddenBuildPart : MonoBehaviour {

	public Transform destination;

	public bool found;

	private bool move;
	private int speed = 50;

	// Use this for initialization
	void Start () {
		move = false;
		found = false;
	}

	// Update is called once per frame
	void Update () {

		if(move){
			float step = speed * Time.deltaTime;
	    transform.position = Vector3.MoveTowards(transform.position, destination.position, step);

			float dist = Vector3.Distance(transform.position, destination.position);
			if(dist == 0) move = false;
		}
	}

	public void FlyToDestination(){
		move = true;
	}
}
