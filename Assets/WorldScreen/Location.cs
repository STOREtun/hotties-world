using UnityEngine;
using System.Collections;

public class Location : MonoBehaviour {

    public enum Score{ZERO, ONE, TWO, THREE};
    public Score score = Score.ZERO;
    public GameObject[] golden;

    public enum LocationState {LOCKED, OPEN, CURRENT};
    private LocationState pstate = LocationState.LOCKED;

    public LocationState state {
        get {
            return pstate;
        }
        set {
            //show/hide location gameobjects based on new state
            Transform goLocked = Global.findGameObjectWithTag(this.gameObject, UnityConstants.Tags.LOCATION_LOCKED);
            if (goLocked) goLocked.gameObject.SetActive(value == LocationState.LOCKED);
            else Debug.Log("GameObject for location locked not found");

            Transform goOpen = Global.findGameObjectWithTag(this.gameObject, UnityConstants.Tags.LOCATION_OPEN);
            if (goOpen) goOpen.gameObject.SetActive(value == LocationState.OPEN);
            else Debug.Log("GameObject for location open not found");

            Transform goCurrent = Global.findGameObjectWithTag(this.gameObject, UnityConstants.Tags.LOCATION_CURRENT);
            if (goCurrent) goCurrent.gameObject.SetActive(value == LocationState.CURRENT);
            else Debug.Log("GameObject for location current not found");

            pstate = value;
        }
    }


}
