using System;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Hit;
using System.Collections;
using TouchScript.Gestures;
using UnityEngine.SceneManagement;

public class WorldScreen : MonoBehaviour {

	[SerializeField] private SpriteRenderer worldMapRenderer;

    private enum State {
		NONE, 
		TRANSITION_ZOOM,
		READY,
		WAITING
	};
    private State state = State.NONE;

	private readonly float LERP_SPEED = 0.1f;
    private float lerpTimer = 0;
    private float originalOthographicSize = -1f;
    private float lerpZoomTimer = 0; 

    private bool loading;

	private Camera cam;
	private Vector3 lerpPos; 
	private Location currentLocation;

	private readonly float MAX_ZOOM_MULTIPLIER = 2.5f;
	private readonly float LERP_ZOOM_TIME = 2f; //transition time for camera zoom effect

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
		global.Reset(); // reset currentIndex and gamestate

		Locations locations = GetComponent<Locations>();
		if (global.completedLevels < 0) global.completedLevels = 0;
			//print("WorldScreen, currentIndex: " + global.currentLevelIndex);
			for (int i = locations.locations.Length - 1; i >= 0; i--) {
			Location location = locations.locations[i].GetComponent<Location>();
			Location.LocationState locationState = Location.LocationState.LOCKED;

			if (i > global.completedLevels)
				locationState = Location.LocationState.LOCKED;

			if (i < global.completedLevels){
				locationState = Location.LocationState.OPEN;

				// setting score for already completed level
				int score = Global.instance.getScoreForLevel(i);
				if(score > 0){
					for(int x = 0; x < 3; x++){
						if(x < score){
						SpriteRenderer sp = location.golden[x].GetComponent<SpriteRenderer>();
						sp.color = new Color(1, 1, 1, 1);
						}
					}
				}
			}

			if (i == global.completedLevels) {
				locationState   = Location.LocationState.CURRENT;
				currentLocation = location;
			}

			location.state = locationState;

			state = State.WAITING;
		}
	}

	public void MoveCameraToCurrentLocation (){
		state = State.READY;
		if (currentLocation) {
			GameObject currentObj = GameObject.FindWithTag("LOCATION_CURRENT");
			Vector3 currentObjectivePos = currentObj.transform.position;
			SetLerpPos(currentObjectivePos, 3f);
		}else{
			SetLerpPos(cam.transform.position, 1f);
		}
	}

    private void pannedHandler(object sender, EventArgs e) {
		if(loading) return; 

		PanGesture gesture = (PanGesture)sender;
		Vector3 worldPos = gesture.WorldTransformCenter;
		Vector3 prevWorldPos = gesture.PreviousWorldTransformCenter;
		Vector3 newWorldPos = lerpPos - (worldPos - prevWorldPos);
		//Debug.Log("PANNED to=" + newWorldPos);
		SetLerpPos(newWorldPos, 0.1f);
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

			if (col.gameObject.tag == UnityConstants.Tags.LOCATION_LOCATION) {
				Locations locs = GetComponent<Locations>();
				int index = Array.IndexOf(locs.locations, col.gameObject);

				// checking whether the locked_flag is active or not = level open/closed
				foreach(Transform child in col.gameObject.transform){
					string childName = child.gameObject.name;
					bool isLocked = child.gameObject.activeSelf;
					if(childName == "location_locked" && !isLocked){
			  			StartCoroutine(LoadLevel(index));
			  			break;
					}
				}
			}
		}
	}

    private IEnumerator LoadLevel(int level){
		loading = true;

		Color c = new Color (0.1f, 0.1f, 0.1f, 0);
		for(int x = 0; x < 10; x++){
			worldMapRenderer.color -= c;
			yield return new WaitForSeconds (0.05f);
		}

		Global.instance.currentLevelIndex = level;
		SceneManager.LoadScene ("Game");
//		Application.LoadLevel("Game");
    }


    void Update() {
        if (state == State.READY) {
            lerpTimer += (LERP_SPEED * Time.deltaTime);
            if (lerpTimer <= 1) {
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
            } else {
                cam.orthographicSize = originalOthographicSize;
                state = State.READY;
            }
        }

    }


    //*********************************************************************
    //*******************************  other  *****************************
    //*********************************************************************

    void SetLerpPos(Vector3 lerp, float newLerpTimer) {
        SetLerpPos(lerp);
        if (newLerpTimer >= 0 && newLerpTimer < 0.95)
            lerpTimer = newLerpTimer;
    }

    void SetLerpPos(Vector3 lerp) {
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
