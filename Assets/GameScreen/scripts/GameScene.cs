using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using TouchScript.Hit;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityConstants;
using SimpleJSON;

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

    // FEED_AGENTS
    private GameObject draggableHo;

    // CONSTRUCT_BUILDING
    private int tapsOnSmoke;
    private const int REQUIRED_TAPS = 12;
    bool shouldInitBuildParts = true;

    void Awake() {
      cam = Camera.main;
      main = GetComponent<Main>();
      tmpPointer = new PointerEventData(EventSystem.current);
    }

    void Start() {
      // PlayerPrefs.DeleteAll();
      //Global.instance.updateLevelData();
      lerpPos = cam.transform.position;

      //load level prefab component
      main.loadLevel(Global.instance.currentLevelIndex);
      if (Global.instance.currentHiddenIndex < 0 || Global.instance.currentHiddenIndex >= main.level.objectiveSprites.Length)
          Global.instance.currentHiddenIndex = 0;

      main.levelObj.GetComponent<Level>().setCurrentObjective();

      //init ui stuff
      main.ui.currentObjectivePanel.GetComponent<Image>().sprite = main.level.objectiveSprites[Global.instance.currentHiddenIndex];

      // init hint number
      main.ui.hintNumber.GetComponent<Text>().text = Global.instance.hintCount.ToString();

      // data for the state CONSTRUCT_BUILDING
      tapsOnSmoke = 0;

      main.ui.showPopupObjectiveWithText(
        main.level.levelText,
        main.level.fannyReactions[0],
        main.level.objectiveDescriptionTexts[Global.instance.currentHiddenIndex]
      );

      // Soomla store event listeners. TODO : delete soomla
      // StoreEvents.OnMarketPurchase += onMarketPurchase;
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
        // TODO check if in menu! Disable panned
        PanGesture gesture = (PanGesture) sender;

        //check for UI objects
        // tmpPointer.position = gesture.ScreenPosition; //gui is in screen pos, not world pos
        // tmpRaycastResults.Clear(); //reset previous results
        // EventSystem.current.RaycastAll(tmpPointer, tmpRaycastResults);

        // is below used?
        foreach (RaycastResult res in tmpRaycastResults) {
            if (res.gameObject.name == "ObjectiveScrollHolderPanel") {
                ScrollRect scrollRect = res.gameObject.GetComponent<ScrollRect>();
                scrollRect.verticalNormalizedPosition -= 0.1f;

                Transform[] comps = res.gameObject.GetComponentsInChildren<Transform>();
                foreach (Transform t in comps) {
                    if (t.gameObject.name == "Objective001001Panel") {
                        t.gameObject.SetActive(false);
                    }
                }

                float totalHeight = 0;
                foreach (Transform t in comps) {
                    RectTransform rectTransform = t.gameObject.GetComponent<RectTransform>();
                    if (rectTransform && t.gameObject.name.StartsWith("Objective001") && t.gameObject.activeInHierarchy) {
                        //Debug.Log("Ucrap this is active: " + t.gameObject.name);
                        totalHeight += rectTransform.rect.height;
                    }
                }

                foreach (Transform t in comps) {
                    if (t.gameObject.name == "ObjectiveScrollPanel") {
                        RectTransform rectTransform = t.gameObject.GetComponent<RectTransform>();
                        //Debug.Log("old height=" + rectTransform.rect.height + " new height=" + totalHeight);
                    }
                }
            }
        }


        //game
        Vector3 worldPos      = gesture.WorldTransformCenter;
        Vector3 prevWorldPos  = gesture.PreviousWorldTransformCenter;
        Vector3 newWorldPos   = lerpPos - (worldPos - prevWorldPos);

        setLerpPos(newWorldPos, 0.1f);
    }

    private void tappedHandler(object sender, EventArgs e) {
        TapGesture gesture = (TapGesture) sender;

        //check for UI objects
        tmpPointer.position = gesture.ScreenPosition; //gui is in screen pos, not world pos
        tmpRaycastResults.Clear(); //reset previous results

        EventSystem.current.RaycastAll(tmpPointer, tmpRaycastResults);
        if(tmpRaycastResults.Count <= 0){ //If no ui object was hit - close all menu elements and show the gamescene
          main.ui.showMenu(false);
          main.ui.showShop(false);
          main.ui.showHUD(true);
          // return;
        }

        foreach (RaycastResult res in tmpRaycastResults) {
            // yet another workaround .. so apparantly raycast masking (see UIButtonRaycastMask) on gui doesnt work for EventSystem.current.RaycastAll
            UIButtonRaycastMask uiButtonRaycastMask = res.gameObject.GetComponent<UIButtonRaycastMask>();
            bool hitAccepted = true;
            if (uiButtonRaycastMask && !uiButtonRaycastMask.IsRaycastLocationValid(gesture.ScreenPosition, Camera.main))
                hitAccepted = false;
            if (hitAccepted) {
                // Debug.Log("RAYUIHIT name=" + res.gameObject.name + " tag=" + res.gameObject.tag);
                string pressedObject = res.gameObject.name;
                //print("GameScene, pressed: " + pressedObject);
                if(pressedObject == "CurrentObjectivePanel"){
                  main.ui.showNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
                  return;
                }

                if (pressedObject == "Menu") { // for some reason it does not reach the 'MenuButton', but Menu works just fine
                    main.ui.showNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
                    return;
                }else if(res.gameObject.tag == UnityConstants.Tags.OVERLAY_BTN_CLOSE){
                  if(Global.instance.gameState == Global.GameState.FINSISHED)
                    Application.LoadLevel(0); // go back to world menu

                  if(Global.instance.gameState == Global.GameState.FIND_HIDDEN_OBJECTS){
                    main.level.setCurrentObjective();
                  }

                  if(Global.instance.gameState == Global.GameState.FEED_AGENTS){
                    // initiate countdown if not already initiated
                    if(!main.level.startedCountDown){
                      main.level.startCountDown();
                    }
                  }
                  main.ui.hideBottomPopup();
                }else if(res.gameObject.tag == UnityConstants.Tags.OVERLAY_BTN_NEXT){
                    main.ui.nextButtonPressed();
                }else if (pressedObject == "SmallHintPack") {
                  main.purchaser.buyConsumable("smallHintPack");
                  Invoke("updateHintNumber", 3);
                  //print("GameScene, TODO: buy small hint pack");
                    // main.ui.showMenu(false);
                    // main.ui.showShop(true);
                    // return;
                }else if(pressedObject == "BigHintPack"){
                  main.purchaser.buyConsumable("bigHintPack");
                  Invoke("updateHintNumber", 3);
                  //print("GameScene, TODO: buy big hint pack");
                }else if (pressedObject == "BuyButton") {
                  Global.instance.iapManager.buyItem(res.gameObject.tag, "not_used"); //Payload (second argument) should not be used like this
                }else if (pressedObject == "CurrentObjectivePanel") {
                    //open objective overlay
                    //main.ui.toggleNotebook(UI.NotebookMode.OBJECTIVE_TAB);
                    //if (Global.instance.currentHiddenIndex >= 0 && Global.instance.currentHiddenIndex < main.level.objectiveDescriptionTexts.Length) {
                    //    main.ui.notebookObjectiveDescriptionText.GetComponent<Text>().text = main.level.objectiveDescriptionTexts[Global.instance.currentHiddenIndex];
                    //}
                }else if(res.gameObject.name == "QuitButton"){
                  Application.LoadLevel("WorldMap");
                }else if (pressedObject == "HintPanel") {
                  //if (Global.instance.currentHiddenIndex >= 0 && Global.instance.currentHiddenIndex < main.level.hiddenObjects.Length) {
                      if (Global.instance.hintCount > 0) {
                          // depending on the gamestate the hints should function differently
                          GameObject hiddenGameObj = null;
                          print("GameScene, pressed hintpanel: " + Global.instance.gameState);
                          switch(Global.instance.gameState){
                            case Global.GameState.FIND_HIDDEN_OBJECTS:
                              hiddenGameObj = main.level.hiddenObjects[Global.instance.currentHiddenIndex];
                            break;

                            case Global.GameState.FEED_AGENTS:
                              hiddenGameObj = main.level.currentHungryCustomer;
                            break;

                            case Global.GameState.CONSTRUCT_BUILDING:
                              // if we are ready to build the hint is deactivated
                              if(main.level.isReadyToBuild) hiddenGameObj = null;

                              hiddenGameObj = main.level.getNextHiddenBuildPart();
                            break;
                          }

                          if(hiddenGameObj != null){
                            setLerpPos(hiddenGameObj.transform.position);
                            Global.instance.hintCount--;
                            Global.instance.updatePlayerPrefWithInt("hintCount", -1);
                            main.ui.hintNumber.GetComponent<Text>().text = Global.instance.hintCount.ToString();
                          }

                      } else {
                          // TODO : open shop to get more hints
                          Debug.Log("GameScene, open shop to get more hints");
                      }
                  //}
                }

                //notebook ui hits
                if (res.gameObject == main.ui.notebookObjectiveTabPanel) {
                    main.ui.showNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
                } else if (res.gameObject == main.ui.notebookHelpTabPanel) {
                    main.ui.showNotebook(UI.NotebookMode.HELP_TAB, true);
                } else if (res.gameObject == main.ui.notebookWorldmapTabPanel) {
                    main.ui.showNotebook(UI.NotebookMode.WORLDMAP_TAB, true);
                    main.ui.WorldMapText.GetComponent<Text>().text = main.level.levelText;
                } else if (res.gameObject == main.ui.notebookOptionsTabPanel) {
                    main.ui.showNotebook(UI.NotebookMode.OPTIONS_TAB, true);
                }else if(res.gameObject == main.ui.closeMenuButton){
                  main.ui.showNotebook(UI.NotebookMode.CLOSED, false);
                }

                if (main.ui.notebookMode == UI.NotebookMode.OBJECTIVE_TAB) {
                    string text = "";
                    int index = -1;
                    int currentHiddenIndex = Global.instance.getCurrentHiddenIndex();
                    if(pressedObject == "ObjectiveBig1Panel"){ // this could be generalized
                      index = 0;
                      text = main.level.objectiveDescriptionTexts[index];
                      if(currentHiddenIndex >= index){
                        main.ui.changeObjectiveTextTo(text);
                      }
                    }else if(pressedObject == "ObjectiveBig2Panel"){
                      index = 1;
                      text = main.level.objectiveDescriptionTexts[index];
                      if(currentHiddenIndex >= index){
                        main.ui.changeObjectiveTextTo(text);
                      }
                    }else if(pressedObject == "ObjectiveBig3Panel"){
                      index = 2;
                      text = main.level.objectiveDescriptionTexts[index];
                      if(currentHiddenIndex >= index){
                        main.ui.changeObjectiveTextTo(text);
                      }
                    }else if(pressedObject == "ObjectiveBig4Panel"){
                      index = 3;
                      text = main.level.objectiveDescriptionTexts[index];
                      if(currentHiddenIndex >= index){
                        main.ui.changeObjectiveTextTo(text);
                      }
                    }else if(pressedObject == "ObjectiveBig5Panel"){
                      index = 4;
                      text = main.level.objectiveDescriptionTexts[index];
                      if(currentHiddenIndex >= index){
                        main.ui.changeObjectiveTextTo(text);
                      }
                    }

                    if (res.gameObject == main.ui.notebookButtonPanel) {
                        if (currentHiddenIndex >= main.level.objectiveSprites.Length) {
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
                        main.ui.iapCanvas.SetActive(true);
                        main.ui.toggleMenu();
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

    //check if we hit objective object
    ITouchHit hit;
    gesture.GetTargetHitResult(out hit);
    Vector3 vec = Camera.main.ScreenToWorldPoint(gesture.ScreenPosition);

    Collider2D[] cols = Physics2D.OverlapPointAll(vec);

    // initial state: FIND_HIDDEN_OBJECTS
		if(Global.instance.gameState == Global.GameState.FIND_HIDDEN_OBJECTS){
        for (int i = 0; i < cols.Length; i++) {

            //Debug.Log("RAYHITGAME name=" + cols[i].name + " tag=" + cols[i].tag);
            if (cols[i].gameObject.tag == UnityConstants.Tags.HIDDEN_OBJECT) {
                GameObject hiddenObject = cols[i].gameObject;
                Level level = main.levelObj.GetComponent<Level>();
                int hiddenObjectIndex = level.getHiddenObjectIndex(hiddenObject);

                if (Global.instance.currentHiddenIndex == hiddenObjectIndex) {
                    Global.instance.updateCurrentHiddenIndex();

                    // check if we found all the hidden objects
                    if (Global.instance.currentHiddenIndex >= level.hiddenObjects.Length){
                      main.ui.popupController.animateTopPopup(
                        main.level.objectiveFoundTexts[Global.instance.currentHiddenIndex - 1],
                        main.level.hiddenObjectsReactions[Global.instance.currentHiddenIndex - 1]
                      );
                      main.ui.objectiveCheckmarkPanels[Global.instance.currentHiddenIndex - 1].SetActive(true);

                      // found the last hidden object and move on to next part of the level
                      Global.instance.changeGameState(Global.GameState.FEED_AGENTS);
                      // present next state to the player and clear the small ribbons to the right

                      main.level.prepareFeedingState();

                      main.ui.popupController.animateBottomPopup(
                        main.level.objectiveDescriptionFinishedText,
                        main.level.fannyReactions[0]
                      );
                    }else{ // there are still objectives to be found - present the next
                      // check the checkmark on the map
                      main.level.foundHiddenObjective(cols[i].gameObject);

                      // initiate the found reaction
                      main.ui.popupController.animateTopPopup(
                        main.level.objectiveFoundTexts[Global.instance.currentHiddenIndex - 1],
                        main.level.hiddenObjectsReactions[Global.instance.currentHiddenIndex - 1]
                      );
                    }
                break;
              }
            }
          }

        // second state FEED_AGENTS (hungry customers)
        }else if(Global.instance.gameState == Global.GameState.FEED_AGENTS){
          for (int i = 0; i < cols.Length; i++){
            GameObject clickedObj = cols[i].gameObject;
            if(clickedObj.tag == UnityConstants.Tags.HOTDOG_AGENT){
              main.level.foundHungryAgent(clickedObj);
            }
          }

        // third and final state: CONSTRUCT_BUILDING
        }else if(Global.instance.gameState == Global.GameState.CONSTRUCT_BUILDING){
          for (int i = 0; i < cols.Length; i++){
            GameObject clickedObj = cols[i].gameObject;

            // click on build area to be presented for building parts (silhouette)
            if(clickedObj.tag == UnityConstants.Tags.BUILD_AREA && !main.level.isReadyToBuild){
              // present popup with silhouttes for hidden parts
              main.level.popup.SetActive(!main.level.popup.activeSelf);

              // initiate the hidden parts once
              if(shouldInitBuildParts){
                main.level.initHiddenBuildParts();
                shouldInitBuildParts = false;

                main.level.startTimerForConstruction();

                // hide the arrow and finger
                main.level.arrow.enabled = false;
                main.level.finger.stop();
              }

            }else if(clickedObj.tag == UnityConstants.Tags.HIDDEN_BUILD_PART){
              // notify level that we found a part
              main.level.foundPart(clickedObj);

            // we need to be ready before activating smoke (= found all parts)
            }else if(clickedObj.tag == UnityConstants.Tags.BUILD_AREA){
              main.level.prepareBuildingSmoke();
            }else if(clickedObj.tag == UnityConstants.Tags.SMOKE && main.level.isReadyToBuild){
              // tapping on area generate smoke animation
              main.level.animateSmoke();
              main.level.popup.SetActive(false);

              // increment tapping counter
              tapsOnSmoke++;
              print("GameScene, tapping++ " + tapsOnSmoke);

              // enough taps has been done do complete construction
              if(tapsOnSmoke >= REQUIRED_TAPS){
                main.level.finger.stop();
                main.level.countUp = false;
                StartCoroutine(main.level.finishBuilding());
                Global.instance.changeGameState(Global.GameState.FINSISHED);

                main.ui.finish();


                // StartCoroutine(main.level.fadeOutObject(main.level.smoke));
                // StartCoroutine(main.level.fadeInObject(main.level.finalBuilding));

                // score is based on foundHungryCustomers and spendTime

                Invoke("initCalculationOfScore", 3);
              }
            }
          }
        }
    }

    private void initCalculationOfScore(){
      int customersFound  = main.level.hungryCustomersFound;
      int timeToBuild     = (int) main.level.timeSpend;

      int score = timeToBuild - customersFound;

      calculateScoreBasedOnTime(score);
      Debug.Log("GameScene, score " + score);
    }

    private void calculateScoreBasedOnTime(float time){
      // default is one golden hotdog stand
      string msg  = "Good job!";
      int score   = 1;

      if(time < 50){
        msg   = "Good job!";
        score = 3;
      }else if(time < 70){
        msg   = "Good job!";
        score = 2;
      }

      Global.instance.saveScoreAndCompleteLevel(score);
      main.ui.showCalculatedScore(msg, score);
    }

    //*********************************************************************
    //*********************************  Frame ****************************
    //*********************************************************************

    void Update() {
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
        lerp.x    = Mathf.Clamp(lerp.x, mincamx, maxcamx);
        lerp.y    = Mathf.Clamp(lerp.y, mincamy, maxcamy);
        lerpPos   = lerp;
        lerpPos.z = cam.transform.position.z;

        lerpTimer = 0;
    }


    /** Soomla store events
      They are kept here so IAPManager does not have to interact with UI elements.

      onMarketPurchase handles the aftermath of a purchase. For some reason the ToString method does not
      directly match the string ids and therefore the contains method was used.
      Otherwise the implementation would use a switch case
    */
    // public void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra) {
    //   Debug.Log("GameScene, we are detecting a buy from within GameScene!");
    //
    //   JSONObject item = pvi.toJSONObject();
    //   string itemIDString = item["itemId"].ToString();
    //
    //   if(itemIDString.Contains("hints_small_id")){
    //     Global.instance.updatePlayerPrefWithInt("hintcount", 5);
    //   }else if(itemIDString.Contains("hints_large_id")){
    //     Global.instance.updatePlayerPrefWithInt("hintcount", 20);
    //   }
    //
    //   main.ui.hintNumber.GetComponent<Text>().text = Global.instance.hintCount.ToString();
    //   main.ui.showShop(false);
    //   main.ui.showHUD(true);
    // }

    public void updateHintNumber(){
      main.ui.hintNumber.GetComponent<Text>().text = Global.instance.hintCount.ToString();
    }
}
