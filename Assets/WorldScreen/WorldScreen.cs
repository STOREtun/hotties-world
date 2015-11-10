using UnityEngine;
using System.Collections;
using System;
using TouchScript.Gestures;
using TouchScript.Hit;






public class WorldScreen : MonoBehaviour {

    private enum State { NONE, TRANSITION_ZOOM, READY };
    private State state = State.NONE;

    //camera
    private Camera cam;
    private readonly float LERP_SPEED = 0.1f; //speed to move towards new camera position
    private float lerpTimer = 0; //timer 0..1 for lerp of camera movement
    private Vector3 lerpPos; //current wanted position for camera (lerping to)

    private float originalOthographicSize = -1f;
    private readonly float MAX_ZOOM_MULTIPLIER = 2.5f;
    private readonly float LERP_ZOOM_TIME = 2f; //transition time for camera zoom effect
    private float lerpZoomTimer = 0; //timer 0..1 for lerp of camera zoom
    
    private void OnEnable() {
        //register input handlers (aka listener aka callback)
        GetComponent<PanGesture>().Panned += pannedHandler;
        //GetComponent<FlickGesture>().Flicked += flickedHandler;
        GetComponent<TapGesture>().Tapped += tappedHandler;
    }

    private void OnDisable() {
        //remove input handlers (aka listener aka callback)
        GetComponent<PanGesture>().Panned -= pannedHandler;
        //GetComponent<FlickGesture>().Flicked -= pannedHandler;
        GetComponent<TapGesture>().Tapped -= tappedHandler;
    }

    void Awake() {
        cam = Camera.main;
        originalOthographicSize = cam.orthographicSize;
    }

    void Start() {


        //inititalize location points (show/hide accordingly)
        Global global = Global.instance;
        Locations locations = GetComponent<Locations>();
        if (global.currentLevelIndex < 0) global.currentLevelIndex = 0;
        Location currentLocation = null;
        for (int i = locations.locations.Length - 1; i >= 0; i--) {
            Location location = locations.locations[i].GetComponent<Location>();
            Location.LocationState locationState = Location.LocationState.LOCKED;
            if (i > global.currentLevelIndex)
                locationState = Location.LocationState.LOCKED;
            if (i < global.currentLevelIndex)
                locationState = Location.LocationState.OPEN;
            if (i == global.currentLevelIndex) {
                locationState = Location.LocationState.CURRENT;
                currentLocation = location;
            }
            //Debug.Log("setting location " + i + " to " + locationState + " currentlevel=" + global.currentLevelIndex);
            location.state = locationState;
        }

        

        //set camera to cover whole world
        //start camera move/zoom transition to current map location
        //currentLocation = locations.locations[4].GetComponent<Location>();
        if (currentLocation) {
            state = State.TRANSITION_ZOOM;
            setLerpPos(new Vector3(currentLocation.transform.position.x, currentLocation.transform.position.y, currentLocation.transform.position.z), 3f);
        }
        //Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, -93.15f, Camera.main.transform.position.z);
        


    }



    private void pannedHandler(object sender, EventArgs e) {
        PanGesture gesture = (PanGesture)sender;
    }

    private void tappedHandler(object sender, EventArgs e) {
        TapGesture gesture = (TapGesture)sender;
        ITouchHit hit;
        gesture.GetTargetHitResult(out hit);
        Vector3 vec = Camera.main.ScreenToWorldPoint(gesture.ScreenPosition);
        Collider2D[] cols = Physics2D.OverlapPointAll(vec);
        for (int i = 0; i < cols.Length; i++) {
            Collider2D col = cols[i];
            Debug.Log("RAYHIT name=" + col.name + " tag=" + col.tag);
            if (col.gameObject.tag == UnityConstants.Tags.LOCATION_LOCATION) {
                Locations locs = GetComponent<Locations>();
                int index = Array.IndexOf(locs.locations, col.gameObject);
                Debug.Log("tapped location " + index);
                loadLevel(0);
                break;
            }
        }

        
    }

    void loadLevel(int level) {
        Debug.Log("loading level " + level);
        Global.instance.currentLevelIndex = level;
        Application.LoadLevel("Game");
    }



    void Update() {

        if (state == State.READY) {
            //camera movement 
            lerpTimer += (LERP_SPEED * Time.deltaTime);
            if (lerpTimer <= 1) {
                //Debug.Log("camelerp timer=" + lerpTimer);
                Vector3 lerp = Vector3.Lerp(cam.transform.position, lerpPos, lerpTimer);
                cam.transform.position = lerp;
            }
        }

        if (state == State.TRANSITION_ZOOM) {
            //camera zoom
            lerpZoomTimer += Time.deltaTime;
            if (lerpZoomTimer < LERP_ZOOM_TIME) {
                float zoomLerp = lerpZoomTimer / LERP_ZOOM_TIME;
                float zoomVal = zoomLerp * ((MAX_ZOOM_MULTIPLIER * originalOthographicSize) - originalOthographicSize);
                cam.orthographicSize = (MAX_ZOOM_MULTIPLIER * originalOthographicSize) - zoomVal;
                //Debug.Log("zoomval=" + zoomVal + "orthsize=" + cam.orthographicSize);
            } else {
                cam.orthographicSize = originalOthographicSize;
                state = State.READY;
            }
        }

    }


    //*********************************************************************
    //*******************************  other  *****************************
    //*********************************************************************

    void setLerpPos(Vector3 lerp, float newLerpTimer) {
        setLerpPos(lerp);
        if (newLerpTimer >= 0 && newLerpTimer < 0.95)
            lerpTimer = newLerpTimer;
    }

    void setLerpPos(Vector3 lerp) {
        //set and clamp lerp position so camera will be within level bounds

        //find level bounds
        float minx = float.MaxValue;
        float maxx = float.MinValue;
        float miny = float.MaxValue;
        float maxy = float.MinValue;
        GameObject worldMap = GameObject.FindGameObjectWithTag(UnityConstants.Tags.LOCATION_WORLDMAP);
        foreach (SpriteRenderer sr in worldMap.GetComponentsInChildren<SpriteRenderer>()) {
            minx = Mathf.Min(minx, sr.bounds.min.x);
            maxx = Mathf.Max(maxx, sr.bounds.max.x);
            miny = Mathf.Min(miny, sr.bounds.min.y);
            maxy = Mathf.Max(maxy, sr.bounds.max.y);
        }
        //Debug.Log("minx=" + minx + "maxx=" + maxx + "miny=" + miny + "maxy=" + maxy);

        //find cam bounds for level
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        float mincamx = minx + horzExtent;
        float maxcamx = maxx - horzExtent;
        float mincamy = miny + vertExtent;
        float maxcamy = maxy - vertExtent;
        //Debug.Log("mincamx=" + mincamx + "maxcamx=" + maxcamx + "mincamy=" + mincamy + "maxcamy=" + maxcamy);

        //adjust and set lerp
        lerp.x = Mathf.Clamp(lerp.x, mincamx, maxcamx);
        lerp.y = Mathf.Clamp(lerp.y, mincamy, maxcamy);
        lerpPos = lerp;
        lerpPos.z = cam.transform.position.z;

        lerpTimer = 0;
    }

}










//public class World : MonoBehaviour {

//    public GameObject[] locations;

//    int unlockedLocationIndex = 0;

//    private void OnEnable() {
//        //register input handlers (aka listener aka callback)
//        GetComponent<TapGesture>().Tapped += tappedHandler;
//    }

//    private void OnDisable() {
//        //remove input handlers (aka listener aka callback)
//        GetComponent<TapGesture>().Tapped -= tappedHandler;
//    }

//    void Start () {
//        for (int i = 0; i < locations.Length; i++) {
//            //Debug.Log(locations[i]);
//            foreach (Transform t in locations[i].GetComponentInChildren<Transform>()) {
//                bool unlocked = unlockedLocationIndex <= i;
//                if (t.name == "location_dot") t.gameObject.SetActive(unlocked);
//                if (t.name == "location_locked") t.gameObject.SetActive(!unlocked);
//                //Debug.Log(t.gameObject.name);
//            }

            
//        }


//    }

//    //void Update() {
//    //    Collider2D col = null; //YAY a nullable type !!
//    //    if (Input.touchCount == 1) {
//    //        Touch touch0 = Input.GetTouch(0);
//    //        if (touch0.phase == TouchPhase.Began) {
//    //            col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
//    //        }
//    //    }
//    //    if (Input.GetMouseButtonUp(0)) {
//    //        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//    //        Vector2 touchPos = new Vector2(pos.x, pos.y);
//    //        col = Physics2D.OverlapPoint(touchPos);
//    //    }
//    //    if (col && col.gameObject.name == "Location") {
//    //        loadLevel(col.gameObject.GetComponent<Location>().locationIndex);
//    //    }
//    //}

//    private void tappedHandler(object sender, EventArgs e) {
//        TapGesture gesture = (TapGesture)sender;
//        ITouchHit hit;
//        gesture.GetTargetHitResult(out hit);
//        Vector2 vec = Camera.main.ScreenToWorldPoint(gesture.ScreenPosition);
//        Collider2D col = Physics2D.OverlapPoint(vec);
//        if (col && col.gameObject.name == "Location") {
//            loadLevel(0);
//        }
//    }

//    void loadLevel(int level) {
//        Debug.Log("loading level " + level);
//        Global.instance.currentLevelIndex = level;
//        Application.LoadLevel("Game");
//    }


//    //void hittest___() {
//    //    //hit testing with ray (needs a collider)
//    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//    //    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
//    //    if (hit) {
//    //        Debug.Log("hit");
//    //    }

//    //    //hit testing with Overlap check (needs a collider)
//    //    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//    //    Vector2 touchPos = new Vector2(pos.x, pos.y);
//    //    Collider2D col = Physics2D.OverlapPoint(touchPos);
//    //    if (col) {
//    //        Debug.Log(col);
//    //    }
//    //}

//}


