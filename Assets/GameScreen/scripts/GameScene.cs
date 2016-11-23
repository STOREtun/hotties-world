using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using UnityConstants;
using TouchScript.Hit;
using System.Collections;
using TouchScript.Gestures;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour {
    private Main main = null; 

	readonly float LERP_SPEED = 0.1f; 
    
	private float lerpTimer = 0; 
    
	private Vector3 lerpPos;
	private Camera cam;
    private PointerEventData tmpPointer;
    private List<RaycastResult> tmpRaycastResults = new List<RaycastResult>();

    // FEED_AGENTS
    private GameObject draggableHo;

    // CONSTRUCT_BUILDING
    private int tapsOnSmoke;
    private const int REQUIRED_TAPS = 12;
    bool shouldInitBuildParts;

    void Awake() {
		cam = Camera.main;
		main = GetComponent<Main>();
		tmpPointer = new PointerEventData(EventSystem.current);
    }

    void Start() {
		shouldInitBuildParts = true;
		lerpPos = cam.transform.position;

		// load level prefab component
		main.InstantiateLevel(Global.instance.currentLevelIndex);
		if (Global.instance.currentHiddenIndex < 0 || Global.instance.currentHiddenIndex >= main.level.objectiveSprites.Length)
			Global.instance.currentHiddenIndex = 0;

		main.level.SetCurrentObjective();

		//init ui stuff
		//main.ui.currentObjectivePanel.GetComponent<Image>().sprite = 
			//main.level.objectiveSprites[Global.instance.currentHiddenIndex];

		// init hint number
		main.ui.hintNumber.GetComponent<Text>().text = 
			Global.instance.hintCount.ToString();

		// data for the state CONSTRUCT_BUILDING
		tapsOnSmoke = 0;

		main.ui.ShowPopupObjectiveWithText(
			main.level.levelText,
			null, // main.level.fannyReactions[0],
			main.level.objectiveDescriptionTexts[Global.instance.currentHiddenIndex]
		);

	}


    private void OnEnable() {
        GetComponent<PanGesture>().Panned += PannedHandler;
        GetComponent<TapGesture>().Tapped += TappedHandler;
    }

    private void OnDisable() {
        GetComponent<PanGesture>().Panned -= PannedHandler;
        GetComponent<TapGesture>().Tapped -= TappedHandler;
    }

    //*********************************************************************
    //***********************  input handlers  ****************************
    //*********************************************************************

    private void PannedHandler(object sender, EventArgs e) {
        PanGesture gesture = (PanGesture) sender;

//        // is below used?
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
                        totalHeight += rectTransform.rect.height;
                    }
                }
            }
        }


        //game
        Vector3 worldPos = gesture.WorldTransformCenter;
        Vector3 prevWorldPos = gesture.PreviousWorldTransformCenter;
        Vector3 newWorldPos = lerpPos - (worldPos - prevWorldPos);

        SetLerpPos(newWorldPos, 0.1f);
    }

    private void TappedHandler(object sender, EventArgs e) {
		TapGesture gesture = (TapGesture)sender;

		ITouchHit hit;
		gesture.GetTargetHitResult (out hit);

		Vector3 vec = Camera.main.ScreenToWorldPoint (gesture.ScreenPosition);
		Collider2D[] cols = Physics2D.OverlapPointAll (vec);
		foreach (BoxCollider2D col in cols) {
			switch (Global.instance.gameState) {

			case GameState.FIND_HIDDEN_OBJECTS:
				if (col.gameObject.tag == GameConstants.TAG_HIDDEN_OBJECT) {
					FoundHiddenObject (col.gameObject);
				}
				break;

			case GameState.FEED_AGENTS:
				if(col.gameObject.tag == GameConstants.TAG_HUNGRY_CUSTOMER){
					main.level.FoundHungryAgent (col.gameObject);
				}
				break;

			case GameState.CONSTRUCT_BUILDING:
				HandleConstructionClick (col.gameObject);
				break;

			default:
				break;
			}
		}
	}

	private void HandleConstructionClick(GameObject col){
		if(col.gameObject.tag == GameConstants.TAG_BUILD_AREA){

			//main.level.popup.SetActive(!main.level.popup.activeSelf);
			main.ui.ShowConstructionPopup ();

			if(shouldInitBuildParts){
				shouldInitBuildParts = false;

				main.soundHandler.PlaySound (Sound.PRESENT_POPUP);

				main.level.InitHiddenBuildParts();

				// hide the arrow and finger
				main.level.arrow.enabled = false;
				main.level.finger.Stop();
			}
		}else if(col.gameObject.tag == GameConstants.TAG_BUILD_PART && !main.level.isReadyToBuild){
			main.level.FoundPart (col.gameObject);
			main.soundHandler.PlaySound (Sound.CLICK_FOUND_OBJECT);
		}else if(col.gameObject.tag == GameConstants.TAG_SMOKE && main.level.isReadyToBuild){
			main.level.AnimateSmoke();
			//main.level.popup.SetActive(false);
			main.ui.HideConstructionPopup ();
			main.level.InitHiddenBuildParts (false);
			// set the items to false

			// increment tapping counter
			tapsOnSmoke++;

			// enough taps has been done do complete construction
			if (tapsOnSmoke >= REQUIRED_TAPS) {
				main.soundHandler.PlaySound (Sound.CONSTRUCTION_DONE);

				main.level.finger.Stop ();
				main.level.countUp = false;

				StartCoroutine (main.level.FinishBuilding ());

				Global.instance.ChangeGameState (GameState.FINISHED);

				main.ui.Finish ();

				Invoke ("InitCalculationOfScore", 3);
			}else main.soundHandler.PlaySound (Sound.CONSTRUCTION_CLICK);
		}
	}

	/// <summary>
	/// Method for HIDDEN_OBJECTS game state.
	/// Handles a click on a hidden objects
	/// </summary>
	/// <param name="hiddenObject">Hidden object.</param>
	private void FoundHiddenObject(GameObject hiddenObject){
		int hiddenObjectIndex = main.level.GetHiddenObjectIndex(hiddenObject);
		int globalIndex = Global.instance.currentHiddenIndex;

		if(globalIndex >= main.level.hiddenObjects.Length - 1){
			Global.instance.ChangeGameState (GameState.FEED_AGENTS);
		}

		if (globalIndex == hiddenObjectIndex) { // check if we pressed the correct hidden object
			main.soundHandler.PlaySound (Sound.CLICK_FOUND_OBJECT);

			Global.instance.UpdateCurrentHiddenIndex ();

			// check the checkmark on the map
			main.level.FoundHiddenObjective (hiddenObject);

			// initiate the found reaction
			main.ui.popupController.AnimateTopPopup (
				main.level.objectiveFoundTexts [Global.instance.currentHiddenIndex - 1],
				main.level.hiddenObjectsReactions [Global.instance.currentHiddenIndex - 1]
			);
		}
	}

	private void InitCalculationOfScore(){
		int customersFound = main.level.hungryCustomersFound;
		int timeToBuild = (int) main.level.timeSpend;

		int score = timeToBuild - customersFound;

		CalculateScoreBasedOnTime(score);
	}

	private void CalculateScoreBasedOnTime(float time){
		// default is one golden hotdog stand
		string msg = "Hooray. Thanks for your help. Press OK to return to world map. Play again to optimize your score or wait for more cities to open.";
		int score = 1;

		if(time < 50){
			score = 3;
		}else if(time < 70){
			score = 2;
		}

		Global.instance.SaveScoreAndCompleteLevel(score);
		main.ui.ShowCalculatedScore(msg, score);
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

	void SetLerpPos(Vector3 lerp, float newLerpTimer) {
		SetLerpPos(lerp);
		if (newLerpTimer >= 0 && newLerpTimer < 0.95)
			lerpTimer = newLerpTimer;
	}

	void SetLerpPos(Vector3 lerp) {
		float minx = float.MaxValue;
		float maxx = float.MinValue;
		float miny = float.MaxValue;
		float maxy = float.MinValue;

		// find level bounds
		foreach (SpriteRenderer sr in main.levelObj.GetComponentsInChildren<SpriteRenderer>()) {
			minx = Mathf.Min(minx, sr.bounds.min.x);
			maxx = Mathf.Max(maxx, sr.bounds.max.x);
			miny = Mathf.Min(miny, sr.bounds.min.y);
			maxy = Mathf.Max(maxy, sr.bounds.max.y);
		}

		// find cam bounds for level
		float vertExtent = cam.orthographicSize;
		float horzExtent = vertExtent * Screen.width / Screen.height;

		float mincamx = minx + horzExtent;
		float maxcamx = maxx - horzExtent;
		float mincamy = miny + vertExtent;
		float maxcamy = maxy - vertExtent;

		// adjust and set lerp
		lerp.x    = Mathf.Clamp(lerp.x, mincamx, maxcamx);
		lerp.y    = Mathf.Clamp(lerp.y, mincamy, maxcamy);
		lerpPos   = lerp;
		lerpPos.z = cam.transform.position.z;

		lerpTimer = 0;
	}

	public void HintPressed(){
		// if (Global.instance.currentHiddenIndex >= 0 && Global.instance.currentHiddenIndex < main.level.hiddenObjects.Length) {
		if (Global.instance.hintCount > 0) {
			// depending on the gamestate the hints should function differently
			GameObject hiddenGameObj = null;
			switch(Global.instance.gameState){

			case GameState.FIND_HIDDEN_OBJECTS:
				hiddenGameObj = main.level.hiddenObjects[Global.instance.currentHiddenIndex];
				break;

			case GameState.FEED_AGENTS:
				hiddenGameObj = main.level.currentHungryCustomer;
				break;

			case GameState.CONSTRUCT_BUILDING:
				// if we are ready to build the hint is deactivated
				if(main.level.isReadyToBuild)
					hiddenGameObj = null;

				hiddenGameObj = main.level.GetNextHiddenBuildPart();
				break;
			}

			if(hiddenGameObj != null){
				SetLerpPos(hiddenGameObj.transform.position);
				Global.instance.hintCount--;
				Global.instance.updatePlayerPrefWithInt("hintCount", -1);
				main.ui.hintNumber.GetComponent<Text>().text = Global.instance.hintCount.ToString();
			}

		} else {
			// TODO : open shop to get more hints
			Debug.Log("GameScene, open shop to get more hints");
		}
	}



//	// check if we found all the hidden objects
//	if (Global.instance.currentHiddenIndex >= level.hiddenObjects.Length) {
//		main.ui.popupController.AnimateTopPopup (
//			main.level.objectiveFoundTexts [Global.instance.currentHiddenIndex - 1],
//			main.level.hiddenObjectsReactions [Global.instance.currentHiddenIndex - 1]
//		);
//		main.ui.objectiveCheckmarkPanels [Global.instance.currentHiddenIndex - 1].SetActive (true);
//
//		// found the last hidden object and move on to next part of the level
//		Global.instance.changeGameState (Global.GameState.FEED_AGENTS);
//
//		// present next state to the player and clear the small ribbons to the right
//		main.level.PrepareFeedingState ();
//
//		// dont present fanny reaction. The image is not scaled correctly as it is
//		string[] texts = new string[] {
//			main.level.objectiveDescriptionFinishedText
//		};
//		main.ui.popupController.ShowBottomPopup (
//			texts,
//			null
//		);
//	} else { // there are still objectives to be found - present the next


//
//        // check for UI objects
//        tmpPointer.position = gesture.ScreenPosition; // gui is in screen pos, not world pos
//        tmpRaycastResults.Clear(); // reset previous results
//
//        EventSystem.current.RaycastAll(tmpPointer, tmpRaycastResults);
//
//        foreach (RaycastResult res in tmpRaycastResults) {
//            // yet another workaround .. so apparantly raycast masking (see UIButtonRaycastMask) on gui doesnt work for EventSystem.current.RaycastAll
//            UIButtonRaycastMask uiButtonRaycastMask = res.gameObject.GetComponent<UIButtonRaycastMask>();
//            bool hitAccepted = true;
//            if (uiButtonRaycastMask && !uiButtonRaycastMask.IsRaycastLocationValid(gesture.ScreenPosition, Camera.main))
//                hitAccepted = false;
//            if (hitAccepted) {
//                string pressedObject = res.gameObject.name;
//                if(pressedObject == "CurrentObjectivePanel"){
//                  main.ui.ShowNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
//                  return;
//                }
//
//                if (pressedObject == "Menu") { // for some reason it does not reach the 'MenuButton', but Menu works just fine
//                    main.ui.ShowNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
//                    return;
//                }else if(res.gameObject.tag == UnityConstants.Tags.OVERLAY_BTN_CLOSE){
//					if (Global.instance.gameState == Global.GameState.FINSISHED)
//						SceneManager.LoadScene (SceneConstants.WORLD_SCENE);
//
//                  	if(Global.instance.gameState == Global.GameState.FIND_HIDDEN_OBJECTS){
//						main.level.SetCurrentObjective();
//                  	}
//
//                  	if(Global.instance.gameState == Global.GameState.FEED_AGENTS){
//                    // initiate countdown if not already initiated
//                    	if(!main.level.startedCountDown){
//                      		main.level.StartCountDown();
//                    	}
//					}
//
//					main.ui.HideBottomPopup();
//
//					}else if(res.gameObject.tag == UnityConstants.Tags.OVERLAY_BTN_NEXT){
//						main.ui.NextButtonPressed();
//					}else if (pressedObject == "SmallHintPack") {
//						main.purchaser.BuyConsumable("smallHintPack");
//					}else if(pressedObject == "BigHintPack"){
//						main.purchaser.BuyConsumable("bigHintPack");
//					}else if(pressedObject == "RestorePurchase"){
//						main.purchaser.RestorePurchases();
//					}else if (pressedObject == "BuyButton") {
//						Global.instance.iapManager.BuyItem(res.gameObject.tag, "not_used"); // Payload (second argument) should not be used like this
//					}else if (pressedObject == "CurrentObjectivePanel") {
//                }else if(res.gameObject.name == "QuitButton"){
//					SceneManager.LoadScene (SceneConstants.WORLD_SCENE);
//                }else if (pressedObject == "HintPanel") {
//                    // if (Global.instance.currentHiddenIndex >= 0 && Global.instance.currentHiddenIndex < main.level.hiddenObjects.Length) {
//                    if (Global.instance.hintCount > 0) {
//                        // depending on the gamestate the hints should function differently
//                        GameObject hiddenGameObj = null;
//                        switch(Global.instance.gameState){
//                          case Global.GameState.FIND_HIDDEN_OBJECTS:
//                            hiddenGameObj = main.level.hiddenObjects[Global.instance.currentHiddenIndex];
//                          break;
//
//                          case Global.GameState.FEED_AGENTS:
//                            hiddenGameObj = main.level.currentHungryCustomer;
//                          break;
//
//                          case Global.GameState.CONSTRUCT_BUILDING:
//                            // if we are ready to build the hint is deactivated
//                            if(main.level.isReadyToBuild) hiddenGameObj = null;
//
//                            hiddenGameObj = main.level.getNextHiddenBuildPart();
//                          break;
//                        }
//
//                        if(hiddenGameObj != null){
//                          SetLerpPos(hiddenGameObj.transform.position);
//                          Global.instance.hintCount--;
//                          Global.instance.updatePlayerPrefWithInt("hintCount", -1);
//                          main.ui.hintNumber.GetComponent<Text>().text = Global.instance.hintCount.ToString();
//                        }
//
//                      } else {
//                        // TODO : open shop to get more hints
//						Debug.Log("GameScene, open shop to get more hints");
//					}
//				}
//
//                //notebook ui hits
//                if (res.gameObject == main.ui.notebookObjectiveTabPanel) {
//                  main.ui.ShowNotebook(UI.NotebookMode.OBJECTIVE_TAB, true);
//                } else if (res.gameObject == main.ui.notebookHelpTabPanel) {
//                  // activate ParentelController
//                  main.ui.PresentParentalGate();
//                  //main.ui.ShowNotebook(UI.NotebookMode.HELP_TAB, true);
//                } else if (res.gameObject == main.ui.notebookWorldmapTabPanel) {
//                  main.ui.ShowNotebook(UI.NotebookMode.WORLDMAP_TAB, true);
//                  main.ui.WorldMapText.GetComponent<Text>().text = main.level.levelText;
//                } else if (res.gameObject == main.ui.notebookOptionsTabPanel) {
//                  main.ui.ShowNotebook(UI.NotebookMode.OPTIONS_TAB, true);
//                }else if(res.gameObject == main.ui.closeMenuButton){
//                  main.ui.ShowNotebook(UI.NotebookMode.CLOSED, false);
//                }
//
//                if (main.ui.notebookMode == UI.NotebookMode.OBJECTIVE_TAB) {
//                    string text = "";
//                    int index = -1;
//                    int currentHiddenIndex = Global.instance.GetCurrentHiddenIndex();
//                    if(pressedObject == "ObjectiveBig1Panel"){ // this could be generalized
//                      index = 0;
//                      text = main.level.objectiveDescriptionTexts[index];
//                      if(currentHiddenIndex >= index){
//                        main.ui.ChangeObjectiveTextTo(text);
//                      }
//                    }else if(pressedObject == "ObjectiveBig2Panel"){
//                      index = 1;
//                      text = main.level.objectiveDescriptionTexts[index];
//                      if(currentHiddenIndex >= index){
//                        main.ui.ChangeObjectiveTextTo(text);
//                      }
//                    }else if(pressedObject == "ObjectiveBig3Panel"){
//                      index = 2;
//                      text = main.level.objectiveDescriptionTexts[index];
//                      if(currentHiddenIndex >= index){
//                        main.ui.ChangeObjectiveTextTo(text);
//                      }
//                    }else if(pressedObject == "ObjectiveBig4Panel"){
//                      index = 3;
//                      text = main.level.objectiveDescriptionTexts[index];
//                      if(currentHiddenIndex >= index){
//                        main.ui.ChangeObjectiveTextTo(text);
//                      }
//                    }else if(pressedObject == "ObjectiveBig5Panel"){
//                      index = 4;
//                      text = main.level.objectiveDescriptionTexts[index];
//                      if(currentHiddenIndex >= index){
//                        main.ui.ChangeObjectiveTextTo(text);
//                      }
//                    }
//
//                    if (res.gameObject == main.ui.notebookButtonPanel) {
//                        if (currentHiddenIndex >= main.level.objectiveSprites.Length) {
//							SceneManager.LoadScene (SceneConstants.WORLD_SCENE);
//                        } else {
//                            main.ui.ShowNotebook(UI.NotebookMode.CLOSED, false);
//                        }
//                    }
//                } else if (main.ui.notebookMode == UI.NotebookMode.HELP_TAB) {
//                    if (res.gameObject == main.ui.notebookButtonPanel)
//                        main.ui.ShowNotebook(UI.NotebookMode.CLOSED, false);
//                } else if (main.ui.notebookMode == UI.NotebookMode.WORLDMAP_TAB) {
//                    if (res.gameObject == main.ui.notebookButtonPanel)
//						SceneManager.LoadScene (SceneConstants.WORLD_SCENE);
//                } else if (main.ui.notebookMode == UI.NotebookMode.OPTIONS_TAB) {
//                    if (res.gameObject.name == "ShopPanel") {
//                        main.ui.iapCanvas.SetActive(true);
//                        main.ui.ToggleMenu();
//                    }
//                    if (res.gameObject.name == "QuitPanel") {
//						SceneManager.LoadScene (SceneConstants.WORLD_SCENE);
//                    }
//                    if (res.gameObject.name == "MusicTogglePanel") {
//                        Debug.Log("TODO: music toggle");
//                    }
//                    if (res.gameObject.name == "SoundEffectsTogglePanel") {
//                        Debug.Log("TODO: sound effx toggle");
//                    }
//                    if (res.gameObject == main.ui.notebookButtonPanel)
//                        main.ui.ShowNotebook(UI.NotebookMode.CLOSED, false);
//                }
//            }
//        }
//			
//		//check if we hit objective object
//		ITouchHit hit;
//		gesture.GetTargetHitResult(out hit);
//		Vector3 vec = Camera.main.ScreenToWorldPoint(gesture.ScreenPosition);
//		Collider2D[] cols = Physics2D.OverlapPointAll(vec);
//
//		// initial state: FIND_HIDDEN_OBJECTS
//		if(Global.instance.gameState == Global.GameState.FIND_HIDDEN_OBJECTS){
//			for (int i = 0; i < cols.Length; i++) {
//
//            //Debug.Log("RAYHITGAME name=" + cols[i].name + " tag=" + cols[i].tag);
//            if (cols[i].gameObject.tag == UnityConstants.Tags.HIDDEN_OBJECT) {
//                GameObject hiddenObject = cols[i].gameObject;
//                Level level = main.levelObj.GetComponent<Level>();
//                int hiddenObjectIndex = level.GetHiddenObjectIndex(hiddenObject);
//
//                if (Global.instance.currentHiddenIndex == hiddenObjectIndex) {
//                    Global.instance.UpdateCurrentHiddenIndex();
//
//                    // check if we found all the hidden objects
//						if (Global.instance.currentHiddenIndex >= level.hiddenObjects.Length){
//							main.ui.popupController.AnimateTopPopup(
//								main.level.objectiveFoundTexts[Global.instance.currentHiddenIndex - 1],
//								main.level.hiddenObjectsReactions[Global.instance.currentHiddenIndex - 1]
//							);
//							main.ui.objectiveCheckmarkPanels[Global.instance.currentHiddenIndex - 1].SetActive(true);
//
//                      // found the last hidden object and move on to next part of the level
//                      		Global.instance.changeGameState(Global.GameState.FEED_AGENTS);
//                      // present next state to the player and clear the small ribbons to the right
//							main.level.PrepareFeedingState();
//
//                      // dont present fanny reaction. The image is not scaled correctly as it is
//							string[] texts = new string[] {
//								main.level.objectiveDescriptionFinishedText
//							};
//							main.ui.popupController.ShowBottomPopup(
//								texts,
//								null
//							);
//						}else{ // there are still objectives to be found - present the next
//                      // check the checkmark on the map
//							main.level.FoundHiddenObjective(cols[i].gameObject);
//
//                      // initiate the found reaction
//							main.ui.popupController.AnimateTopPopup(
//								main.level.objectiveFoundTexts[Global.instance.currentHiddenIndex - 1],
//								main.level.hiddenObjectsReactions[Global.instance.currentHiddenIndex - 1]
//							);
//						}
//						break;
//					}
//				}
//			}
//
//        // second state FEED_AGENTS (hungry customers)
//        }else if(Global.instance.gameState == Global.GameState.FEED_AGENTS){
//          for (int i = 0; i < cols.Length; i++){
//            GameObject clickedObj = cols[i].gameObject;
//            if(clickedObj.tag == UnityConstants.Tags.HOTDOG_AGENT){
//              main.level.FoundHungryAgent(clickedObj);
//            }
//          }
//
//        // third and final state: CONSTRUCT_BUILDING
//        }else if(Global.instance.gameState == Global.GameState.CONSTRUCT_BUILDING){
//          for (int i = 0; i < cols.Length; i++){
//            GameObject clickedObj = cols[i].gameObject;
//
//            // click on build area to be presented for building parts (silhouette)
//            if(clickedObj.tag == UnityConstants.Tags.BUILD_AREA && !main.level.isReadyToBuild){
//              // present popup with silhouttes for hidden parts
//              main.level.popup.SetActive(!main.level.popup.activeSelf);
//
//              // initiate the hidden parts once
//              if(shouldInitBuildParts){
//                main.level.InitHiddenBuildParts();
//                shouldInitBuildParts = false;
//
//                main.level.StartTimerForConstruction();
//
//                // hide the arrow and finger
//                main.level.arrow.enabled = false;
//                main.level.finger.Stop();
//              }
//
//            }else if(clickedObj.tag == UnityConstants.Tags.HIDDEN_BUILD_PART){
//              // notify level that we found a part
//              main.level.FoundPart(clickedObj);
//
//            // we need to be ready before activating smoke (= found all parts)
//            }else if(clickedObj.tag == UnityConstants.Tags.BUILD_AREA){
//              main.level.PrepareBuildingSmoke();
//            }else if(clickedObj.tag == UnityConstants.Tags.SMOKE && main.level.isReadyToBuild){
//              // tapping on area generate smoke animation
//              main.level.AnimateSmoke();
//              main.level.popup.SetActive(false);
//
//              // increment tapping counter
//              tapsOnSmoke++;
//              print("GameScene, tapping++ " + tapsOnSmoke);
//
//              // enough taps has been done do complete construction
//              if(tapsOnSmoke >= REQUIRED_TAPS){
//                main.level.finger.Stop();
//                main.level.countUp = false;
//                StartCoroutine(main.level.finishBuilding());
//                Global.instance.changeGameState(Global.GameState.FINSISHED);
//
//                main.ui.Finish();
//
//                Invoke("initCalculationOfScore", 3);
//              }
//            }
//          }
//        }
//    }

    
}