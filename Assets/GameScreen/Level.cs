using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Level : MonoBehaviour {

    public GameObject[] hiddenObjects;

    public Sprite[] objectiveSprites;

    public string[] objectiveDescriptionTexts;
    public string objectiveDescriptionFinishedText;
    public string objectiveCaptionText;

    public void setCurrentObjective(int currentObjectiveIndex) {
        for (int i = 0; i < hiddenObjects.Length; i++ ) {
            bool chk = currentObjectiveIndex >= i + 1;
            //Debug.Log("index=" + i + " curObjIndex=" + currentObjectiveIndex + " check=" + chk);
            GameObject checkmark = hiddenObjects[i].GetComponent<HiddenObject>().checkmark;
            checkmark.SetActive(chk);
        }

        //set ui stuff
        Main main = GameObject.Find("Main").GetComponent<Main>();
        int sprIndex = Mathf.Clamp(currentObjectiveIndex, 0, objectiveSprites.Length-1);
        main.ui.currentObjectivePanel.GetComponent<Image>().sprite = objectiveSprites[sprIndex];


        for (int i = 0; i <= currentObjectiveIndex && i < objectiveSprites.Length; i++) {
            main.ui.objectivePanels[i].GetComponent<Image>().sprite = objectiveSprites[i];
        }

        for (int i = 0; i < main.ui.objectiveCheckmarkPanels.Length; i++) {
            main.ui.objectiveCheckmarkPanels[i].SetActive(i <= currentObjectiveIndex - 1);
        }

    }

    public int getHiddenObjectIndex(GameObject hiddenObject) {
        for (int i = 0; i < hiddenObjects.Length; i++)
            if (hiddenObject == hiddenObjects[i])
                return i;
        return -1;
    }

    //public GameObject[] levels;


    //public GameObject loadLevel(int levelIndex) {
    //    if (levelIndex < 0 || levelIndex >= levels.Length)
    //        Debug.LogError("Tried to load an unknown level " + levelIndex);
    //    GameObject levelObj = GetComponent<Main>().levelObj = Instantiate(levels[Global.instance.currentLevelIndex], levels[Global.instance.currentLevelIndex].transform.position, Quaternion.identity) as GameObject;
    //    levelObj.SetActive(true);
    //    return levelObj;
    //}




    //private Camera cam;

    //GameObject level;

    //readonly float LERP_SPEED = 0.1f;
    //float lerpTimer = 0;
    //Vector3 lerpPos;
    //void Start () {
    //    cam = Camera.main;
    //    lerpPos = cam.transform.position;

    //    if (Global.instance.currentLevelIndex < 0 || Global.instance.currentLevelIndex >= levels.Length)
    //        Debug.LogError("Tried to load an unknown level " + Global.instance.currentLevelIndex);

    //    //Vector3 curcampos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
    //    //GameObject o = Instantiate(levels[Global.instance.currentLevelIndex], curcampos, Quaternion.identity) as GameObject;
    //    level = Instantiate(levels[Global.instance.currentLevelIndex], levels[Global.instance.currentLevelIndex].transform.position, Quaternion.identity) as GameObject;


    //}



    //void Update() {
    //    if (Input.GetMouseButtonUp(1)) {
    //        lerpTimer = 0;
    //        Vector3 lerp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        setLerpPos(lerp);
    //    }


    //    //camera movement
    //    lerpTimer += (LERP_SPEED * Time.deltaTime);
    //    if (lerpTimer < 1) {
    //        Vector3 lerp = Vector3.Lerp(cam.transform.position, lerpPos, lerpTimer);
    //        cam.transform.position = lerp;
    //    }
    //}

    //void setLerpPos(Vector3 lerp) {
    //    //set and clamp lerp position so camera will be within level bounds

    //    //find level bounds
    //    float minx = float.MaxValue;
    //    float maxx = 0;
    //    float miny = float.MaxValue;
    //    float maxy = 0;
    //    foreach (SpriteRenderer sr in level.GetComponentsInChildren<SpriteRenderer>()) {
    //        minx = Mathf.Min(minx, sr.bounds.min.x);
    //        maxx = Mathf.Max(maxx, sr.bounds.max.x);
    //        miny = Mathf.Min(miny, sr.bounds.min.y);
    //        maxy = Mathf.Max(maxy, sr.bounds.max.y);
    //    }
    //    //Debug.Log("minx=" + minx + "maxx=" + maxx + "miny=" + miny + "maxy=" + maxy);

    //    //find cam bounds for level
    //    float vertExtent = cam.orthographicSize;
    //    float horzExtent = vertExtent * Screen.width / Screen.height;
    //    float mincamx = minx + horzExtent;
    //    float maxcamx = maxx - horzExtent;
    //    float mincamy = miny + vertExtent;
    //    float maxcamy = maxy - vertExtent;
    //    //Debug.Log("mincamx=" + mincamx + "maxcamx=" + maxcamx + "mincamy=" + mincamy + "maxcamy=" + maxcamy);

    //    //adjust and set lerp
    //    lerp.x = Mathf.Clamp(lerp.x, mincamx, maxcamx);
    //    lerp.y = Mathf.Clamp(lerp.y, mincamy, maxcamy);
    //    lerpPos = lerp;
    //    lerpPos.z = cam.transform.position.z;
    //}

}
