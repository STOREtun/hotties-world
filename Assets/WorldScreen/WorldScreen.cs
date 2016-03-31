using UnityEngine;
using System.Collections;
using System;
using TouchScript.Gestures;
using TouchScript.Hit;






public class WorldScreen : MonoBehaviour {

    public SpriteRenderer worldMapRenderer;

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

    private bool loading;

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
      loading = false;

      //inititalize location points (show/hide accordingly)
      Global global = Global.instance;
      global.reset(); // reset currentIndex and gamestate

      Locations locations = GetComponent<Locations>();
      if (global.completedLevels < 0) global.completedLevels = 0;
      Location currentLocation = null;
      //print("WorldScreen, currentIndex: " + global.currentLevelIndex);
      for (int i = locations.locations.Length - 1; i >= 0; i--) {
          Location location = locations.locations[i].GetComponent<Location>();
          Location.LocationState locationState = Location.LocationState.LOCKED;

          if (i > global.completedLevels)
              locationState = Location.LocationState.LOCKED;

          if (i < global.completedLevels)
              locationState = Location.LocationState.OPEN;

          if (i == global.completedLevels) {
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
        state = State.READY;
        //state = State.TRANSITION_ZOOM;
        GameObject currentObj = GameObject.FindWithTag("LOCATION_CURRENT");
        Vector3 currentObjectivePos = currentObj.transform.position;
        //print("WorldScreen, currentObjective.childCount: " + );
        setLerpPos(currentObjectivePos, 3f);
      }
    }



    private void pannedHandler(object sender, EventArgs e) {
      if(loading) return; // dont let user manipulate screen if loading

      PanGesture gesture = (PanGesture)sender;
      Vector3 worldPos = gesture.WorldTransformCenter;
      Vector3 prevWorldPos = gesture.PreviousWorldTransformCenter;
      Vector3 newWorldPos = lerpPos - (worldPos - prevWorldPos);
      //Debug.Log("PANNED to=" + newWorldPos);
      setLerpPos(newWorldPos, 0.1f);
    }

    private void tappedHandler(object sender, EventArgs e) {
      if(loading) return; // dont let user manipulate screen if loading

      TapGesture gesture = (TapGesture)sender;
      ITouchHit hit;
      gesture.GetTargetHitResult(out hit);
      Vector3 vec = Camera.main.ScreenToWorldPoint(gesture.ScreenPosition);
      Collider2D[] cols = Physics2D.OverlapPointAll(vec);
      for (int i = 0; i < cols.Length; i++) {
        Collider2D col = cols[i];
        // Debug.Log("RAYHIT name=" + col.name + " tag=" + col.tag);
        if (col.gameObject.tag == UnityConstants.Tags.LOCATION_LOCATION) {
          Locations locs = GetComponent<Locations>();
          int index = Array.IndexOf(locs.locations, col.gameObject);

          // checking whether the locked_flag is active or not = level open/closed
          foreach(Transform child in col.gameObject.transform){
            string childName = child.gameObject.name;
            bool isLocked = child.gameObject.activeSelf;
            if(childName == "location_locked" && !isLocked){
              // loadLevel(index);
              StartCoroutine(loadLevel(1));
              break;
            }
          }
        }
      }
    }

    private IEnumerator loadLevel(int level){
      loading = true;

      worldMapRenderer.color = new Color(0.9f, 0.9f, 0.9f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.8f, 0.8f, 0.8f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.7f, 0.7f, 0.7f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.6f, 0.6f, 0.6f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.4f, 0.4f, 0.4f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.2f, 0.2f, 0.2f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.1f, 0.1f, 0.1f, 1);
      yield return new WaitForSeconds(0.05f);
      worldMapRenderer.color = new Color(0.0f, 0.0f, 0.0f, 1);

      Global.instance.currentLevelIndex = level;
      Application.LoadLevel("Game");
    }


    void Update() {
        //print("WorldScreen, lerpPos: " + lerpPos);
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
