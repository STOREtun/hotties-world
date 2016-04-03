using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using TouchScript.Hit;
using System;

public class Intro : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}

    private void OnEnable() {
        //register input handlers (aka listener aka callback)
        //GetComponent<PanGesture>().Panned += pannedHandler;
        //GetComponent<FlickGesture>().Flicked += flickedHandler;
        GetComponent<TapGesture>().Tapped += tappedHandler;
    }

    private void OnDisable() {
        //remove input handlers (aka listener aka callback)
        //GetComponent<PanGesture>().Panned -= pannedHandler;
        //GetComponent<FlickGesture>().Flicked -= pannedHandler;
        GetComponent<TapGesture>().Tapped -= tappedHandler;
    }

    //// Update is called once per frame
    //void Update () {
    //    if (Input.GetButtonDown("Jump"))
    //        nextScreen();
    //    if (Input.touchCount == 1) {
    //        Touch touch0 = Input.GetTouch(0);
    //        if (touch0.phase == TouchPhase.Began)
    //            nextScreen();
    //    }
    //    if (Input.GetMouseButton(0)) {
    //        nextScreen();
    //    }
    //}


    private void tappedHandler(object sender, EventArgs e) {
        nextScreen();
    }


    void nextScreen() {
        Application.LoadLevel("WorldMap");
    }

}
