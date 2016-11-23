using UnityEngine;
using System.Collections;

public class LayerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().sortingOrder = 5;
	}

	// Update is called once per frame
	void Update () {

	}
}
