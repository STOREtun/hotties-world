using UnityEngine;
using TouchScript.Gestures;
using System;
using TouchScript.Hit;

public class Gestures : MonoBehaviour {


    private void OnEnable() {
        GetComponent<PanGesture>().Panned     += pannedHandler;
        GetComponent<FlickGesture>().Flicked  += flickedHandler;
        GetComponent<TapGesture>().Tapped     += tappedHandler;
    }

    private void OnDisable() {
        GetComponent<PanGesture>().Panned     -= pannedHandler;
        GetComponent<FlickGesture>().Flicked  -= pannedHandler;
        GetComponent<TapGesture>().Tapped     -= tappedHandler;
    }

    private void pannedHandler(object sender, EventArgs e) {
        PanGesture gesture = (PanGesture)sender;
        ITouchHit hit;
        gesture.GetTargetHitResult(out hit);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(gesture.ScreenPosition);
        Debug.Log("alkaskl "          + gesture.WorldTransformCenter);
        Debug.Log("PANNED worldpos="  + worldPos);
    }
    private void flickedHandler(object sender, EventArgs e) {
        Debug.Log("FLICKED");
    }
    private void tappedHandler(object sender, EventArgs e) {
        Debug.Log("TAPPED");

        TapGesture gesture = (TapGesture)sender;
        ITouchHit hit;
        gesture.GetTargetHitResult(out hit);
        Debug.Log("hit "      + hit.Transform);
        Debug.Log("gesture "  + gesture.ScreenPosition);
        Vector3 vec = Camera.main.ScreenToWorldPoint(gesture.ScreenPosition);
        Debug.Log("worldpos " + vec);


        //gameObject.transform.position = new Vector3(55, 55, 55);
        //GestureStateChangeEventArgs
    }


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}


    //private void tappedHandler(object sender, EventArgs e) {
    //    var gesture = sender as TapGesture;
    //    ITouchHit hit;
    //    gesture.GetTargetHitResult(out hit);
    //    var hit3d = hit as ITouchHit3D;
    //    if (hit3d == null) return;

    //    Color color = new Color(Random.value, Random.value, Random.value);
    //    var cube = Instantiate(CubePrefab) as Transform;
    //    cube.parent = Container;
    //    cube.name = "Cube";
    //    cube.localScale = Vector3.one * Scale * cube.localScale.x;
    //    cube.position = hit3d.Point + hit3d.Normal * .5f;
    //    cube.GetComponent<Renderer>().material.color = color;
    //}
}
