using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using TouchScript.Hit;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityConstants;

public class GameScene : MonoBehaviour {


    //main controller for Game Scene

    private Main main = null; //binding object (go through this for all references)

    //camera
    private Camera cam;
    readonly float LERP_SPEED = 0.1f; //speed to move towards new camera position
    float lerpTimer = 0; //timer 0..1 for lerp of camera movement
    Vector3 lerpPos; //current wanted position for camera (lerping to)


    private PointerEventData tmpPointer;
    private List<RaycastResult> tmpRaycastResults = new List<RaycastResult>();

    void Awake() {
        cam = Camera.main;
        main = GetComponent<Main>();
        tmpPointer = new PointerEventData(EventSystem.current);
    }

    void Start() {
        lerpPos = cam.transform.position;

        //load level prefab component
        main.loadLevel(Global.instance.currentLevelIndex);
        if (Global.instance.currentHiddenIndex < 0 || Global.instance.currentHiddenIndex >= main.level.objectiveSprites.Length)
            Global.instance.currentHiddenIndex = 0;
        main.levelObj.GetComponent<Level>().setCurrentObjective(Global.instance.currentHiddenIndex);

        //init ui stuff
        main.ui.currentObjectivePanel.GetComponent<Image>().sprite = main.level.objectiveSprites[Global.instance.currentHiddenIndex];
        main.ui.hintNumber.GetComponent<Text>().text = Global.instance.hintCount.ToString();

        //show notebook with objective
        main.ui.showNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
    }


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



    //*********************************************************************
    //***********************  input handlers  ****************************
    //*********************************************************************

    private void pannedHandler(object sender, EventArgs e) {
        PanGesture gesture = (PanGesture)sender;

        //check for UI objects
        tmpPointer.position = gesture.ScreenPosition; //gui is in screen pos, not world pos
        tmpRaycastResults.Clear(); //reset previous results
        EventSystem.current.RaycastAll(tmpPointer, tmpRaycastResults);
        foreach (RaycastResult res in tmpRaycastResults) {
            Debug.Log("RAYUIPAN name=" + res.gameObject.name + " tag=" + res.gameObject.tag);
            //manually scrolling UI ScrollRect.. and it works! whawhawha something that works for a change, yay
            if (res.gameObject.name == "ObjectiveScrollHolderPanel") {
                ScrollRect scrollRect = res.gameObject.GetComponent<ScrollRect>();
                scrollRect.verticalNormalizedPosition -= 0.1f;

                //string strt = Tags.hiddenobject;

                //GameObject[] comps = res.gameObject.GetComponents<GameObject>();
                Transform[] comps = res.gameObject.GetComponentsInChildren<Transform>();
                //foreach (GameObject o in comps) {
                foreach (Transform t in comps) {
                    //Debug.Log("test: min=" + o.GetComponent<Renderer>().bounds.min.y + " max=" + o.GetComponent<Renderer>().bounds.max.y);
//                    Debug.Log(t.gameObject.name);
//                    Debug.Log(t.gameObject.transform.localPosition); //use scrolling to component position

                    if (t.gameObject.name == "Objective001001Panel") {
                        t.gameObject.SetActive(false);
                    }

                    //RectTransform rectTransform = t.gameObject.GetComponent<RectTransform>();
                    //if (rectTransform)
                    //    Debug.Log(rectTransform.rect.height);
                }

                float totalHeight = 0;
                foreach (Transform t in comps) {
                    RectTransform rectTransform = t.gameObject.GetComponent<RectTransform>();
                    if (rectTransform && t.gameObject.name.StartsWith("Objective001") && t.gameObject.activeInHierarchy) {
                        Debug.Log("Ucrap this is active: " + t.gameObject.name);
                        totalHeight += rectTransform.rect.height;
                    }
                }

                foreach (Transform t in comps) {
                    if (t.gameObject.name == "ObjectiveScrollPanel") {
                        RectTransform rectTransform = t.gameObject.GetComponent<RectTransform>();
                        Debug.Log("old height=" + rectTransform.rect.height + " new height=" + totalHeight);
                    }
                }
            }
        }


        //game
        Vector3 worldPos = gesture.WorldTransformCenter;
        Vector3 prevWorldPos = gesture.PreviousWorldTransformCenter;
        Vector3 newWorldPos = lerpPos - (worldPos - prevWorldPos);
        //Debug.Log("PANNED to=" + newWorldPos);
        setLerpPos(newWorldPos, 0.1f);
    }
//    private void flickedHandler(object sender, EventArgs e) {
//        FlickGesture gesture = (FlickGesture)sender;
//        ITouchHit hit;
//        gesture.GetTargetHitResult(out hit);
//        Vector3 worldPos = cam.ScreenToWorldPoint(gesture.ScreenPosition);
////        setLerpPos(worldPos);
//        Debug.Log("FLICKED to=" + worldPos);
//    }
    private void tappedHandler(object sender, EventArgs e) {
        Debug.Log("TAPPED");
        TapGesture gesture = (TapGesture)sender;

        //check for UI objects
        tmpPointer.position = gesture.ScreenPosition; //gui is in screen pos, not world pos
        tmpRaycastResults.Clear(); //reset previous results
        EventSystem.current.RaycastAll(tmpPointer, tmpRaycastResults);
        foreach (RaycastResult res in tmpRaycastResults) {
            //yet another workaround .. so apparantly raycast masking (see UIButtonRaycastMask) on gui doesnt work for EventSystem.current.RaycastAll
            UIButtonRaycastMask uiButtonRaycastMask = res.gameObject.GetComponent<UIButtonRaycastMask>();
            bool hitAccepted = true;
            if (uiButtonRaycastMask && !uiButtonRaycastMask.IsRaycastLocationValid(gesture.ScreenPosition, Camera.main))
                hitAccepted = false;
            if (hitAccepted) {
                Debug.Log("RAYUIHIT name=" + res.gameObject.name + " tag=" + res.gameObject.tag);
                if (res.gameObject.name == "MenuButton") {
                    main.ui.showShop(false);
                    main.ui.toggleMenu();
                    return;
                }
                if (res.gameObject.name == "ShopButton") {
                    main.ui.showMenu(false);
                    main.ui.showShop(true);
                    return;
                }
                if (res.gameObject.name == "BuyButton") {
                    IAPItem iapItem = res.gameObject.GetComponentInParent<IAPItem>();
                    Global.instance.iapManager.initiatePurchase(iapItem.iapIdentifierString);
                }
                if (res.gameObject.name == "CurrentObjectivePanel") {
                    //open objective overlay
                    main.ui.toggleNotebook(UI.NotebookMode.OBJECTIVE_TAB);
                    //if (Global.instance.currentHiddenIndex >= 0 && Global.instance.currentHiddenIndex < main.level.objectiveDescriptionTexts.Length) {
                    //    main.ui.notebookObjectiveDescriptionText.GetComponent<Text>().text = main.level.objectiveDescriptionTexts[Global.instance.currentHiddenIndex];
                    //}
                }
                if (res.gameObject.name == "HintPanel") {
                    if (Global.instance.currentHiddenIndex >= 0 && Global.instance.currentHiddenIndex < main.level.hiddenObjects.Length) {
                        if (Global.instance.hintCount > 0) {
                            GameObject hiddenGameObj = main.level.hiddenObjects[Global.instance.currentHiddenIndex];
                            setLerpPos(hiddenGameObj.transform.position);
                            Global.instance.hintCount--;
                            main.ui.hintNumber.GetComponent<Text>().text = Global.instance.hintCount.ToString();
                        } else {
                            Debug.Log("TODO: open shop to get more hints");
                        }
                    }
                }
                //notebook ui hits
                if (res.gameObject == main.ui.notebookObjectiveTabPanel) {
                    main.ui.showNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
                } else if (res.gameObject == main.ui.notebookHelpTabPanel) {
                    main.ui.showNotebook(UI.NotebookMode.HELP_TAB, true);
                } else if (res.gameObject == main.ui.notebookWorldmapTabPanel) {
                    main.ui.showNotebook(UI.NotebookMode.WORLDMAP_TAB, true);
                } else if (res.gameObject == main.ui.notebookOptionsTabPanel) {
                    main.ui.showNotebook(UI.NotebookMode.OPTIONS_TAB, true);
                }
                if (main.ui.notebookMode == UI.NotebookMode.OBJECTIVE_TAB) {
                    if (res.gameObject == main.ui.notebookButtonPanel) {
                        if (Global.instance.currentHiddenIndex >= main.level.objectiveSprites.Length) {
                            //all objectives found
                            Application.LoadLevel("WorldMap");
                        } else {
                            main.ui.showNotebook(UI.NotebookMode.CLOSED, false);
                        }
                    }
                } else if (main.ui.notebookMode == UI.NotebookMode.HELP_TAB) {
                    if (res.gameObject == main.ui.notebookButtonPanel)
                        main.ui.showNotebook(UI.NotebookMode.CLOSED, false);
                } else if (main.ui.notebookMode == UI.NotebookMode.WORLDMAP_TAB) {
                    if (res.gameObject == main.ui.notebookButtonPanel)
                        Application.LoadLevel("WorldMap");
                } else if (main.ui.notebookMode == UI.NotebookMode.OPTIONS_TAB) {
                    if (res.gameObject.name == "ShopPanel") {
                        Debug.Log("TODO: open shop");
                    }
                    if (res.gameObject.name == "QuitPanel") {
                        Application.LoadLevel("WorldMap");
                    }
                    if (res.gameObject.name == "MusicTogglePanel") {
                        Debug.Log("TODO: music toggle");
                    }
                    if (res.gameObject.name == "SoundEffectsTogglePanel") {
                        Debug.Log("TODO: sound effx toggle");
                    }
                    if (res.gameObject == main.ui.notebookButtonPanel)
                        main.ui.showNotebook(UI.NotebookMode.CLOSED, false);
                }
            }


        }


        //check if we hit hidden object
        ITouchHit hit;
        gesture.GetTargetHitResult(out hit);
        Vector3 vec = Camera.main.ScreenToWorldPoint(gesture.ScreenPosition);

        Collider2D[] cols = Physics2D.OverlapPointAll(vec);
        for (int i = 0; i < cols.Length; i++) {
            //Debug.Log("RAYHITGAME name=" + cols[i].name + " tag=" + cols[i].tag);
            if (cols[i].gameObject.tag == UnityConstants.Tags.HIDDEN_OBJECT) {
                GameObject hiddenObject = cols[i].gameObject;
                Level level = main.levelObj.GetComponent<Level>();
                int hiddenObjectIndex = level.getHiddenObjectIndex(hiddenObject);
                //Debug.Log("Found" + hiddenObject);
                if (Global.instance.currentHiddenIndex == hiddenObjectIndex) {
                    Global.instance.currentHiddenIndex++;
                    main.levelObj.GetComponent<Level>().setCurrentObjective(Global.instance.currentHiddenIndex);
                    if (Global.instance.currentHiddenIndex >= level.hiddenObjects.Length) {
                        Debug.Log("TODO: Found all hidden objects .. doing uhm.. what next?");
                    } else {
                        Debug.Log("Found hidden object, next hiddenObjectIndex=" + Global.instance.currentHiddenIndex);
                    }
                    break;
                }
            }
        }

    }



    //*********************************************************************
    //*********************************  Frame ****************************
    //*********************************************************************





    void Update() {
        //if (Input.GetMouseButtonUp(1)) {
        //    Vector3 lerp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    setLerpPos(lerp, 0);
        //}


        //camera movement
        lerpTimer += (LERP_SPEED * Time.deltaTime);
        if (lerpTimer <= 1) {
            Vector3 lerp = Vector3.Lerp(cam.transform.position, lerpPos, lerpTimer);
            cam.transform.position = lerp;
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
        foreach (SpriteRenderer sr in main.levelObj.GetComponentsInChildren<SpriteRenderer>()) {
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
