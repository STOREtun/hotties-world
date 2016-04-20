using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Level : MonoBehaviour {

    // HIDDEN_OBJECT
    public GameObject[] hiddenObjects;
    public Sprite[] hiddenObjectsReactions;

    public Sprite[] objectiveSprites;

    public string[] objectiveDescriptionTexts;
    public string[] objectiveFoundTexts;
    public string   objectiveDescriptionFinishedText;
    public string   objectiveCaptionText;
    public string   levelText;

    // FEED_AGENTS
    public GameObject[] hotdogAgents;
    public GameObject currentHungryCustomer;
    [HideInInspector] private string currentHungryCustomerString;
    public int hungryCustomersFound;
    public bool startedCountDown;
    public float timeLeft;

    // CONSTRUCT_BUILDING
    public GameObject[] hiddenBuildingParts;
    public GameObject[] silhouettes;
    public GameObject[] buildingStages;
    public GameObject popup;
    public FingerController finger;
    public Sprite popupConstructionImage;
    public SmokeAnimationController smokeController;
    public float timeSpend;
    public bool countUp;
    public Sprite buildSprite;

    public Sprite[] fannyReactions;

    [HideInInspector] public bool isReadyToBuild;
    public SpriteRenderer arrow;

    private const int TAPPING_BUFFER  = 20;

    [HideInInspector] public int foundParts;
    private int requiredParts;

    private Main main;

    private GUIStyle guiStyle;

    void Awake(){
      main = GameObject.Find("Main").GetComponent<Main>();
    }

    void Start(){
      // FEED_AGENTS
      startedCountDown  = false;
      hungryCustomersFound = 0;

      // CONSTRUCT_BUILDING
      countUp           = false;
      timeSpend         = 0;
      requiredParts     = hiddenBuildingParts.Length;
      foundParts        = 0;
      isReadyToBuild    = false;

      guiStyle = new GUIStyle();
      guiStyle.fontSize = 60;
      guiStyle.normal.textColor = Color.white;

      // make sure there is no animation on start of level
      finger.stop();
      smokeController.stop();
      arrow.enabled = false;

      // arrow.enabled = true;
    }

    void Update(){
      if(startedCountDown){
        if(!main.ui.timeObject.activeInHierarchy) main.ui.timeObject.SetActive(true);
        timeLeft -= Time.deltaTime;
        if(timeLeft < 0){
          timeLeft = 0;
          foundHungryAgent();
        }
        main.ui.timeText.text = timeLeft.ToString();
      }

      if(countUp){
        if(!main.ui.timeObject.activeInHierarchy) main.ui.timeObject.SetActive(true);
        timeSpend += Time.deltaTime;
        main.ui.timeText.text = timeSpend.ToString();
      }
    }

    void OnGUI(){
      // if(startedCountDown){
      //   GUI.Box(new Rect(10, 10, 70, 70), "");
      //   GUI.Label(new Rect(10, 10, 100, 100), ((int) timeLeft).ToString(), guiStyle);
      // }else if(countUp){
      //   GUI.Box(new Rect(10, 10, 80, 70), "");
      //   GUI.Label(new Rect(10, 10, 100, 100), ((int) timeSpend).ToString(), guiStyle);
      // }
    }


    // FIND_HIDDEN_OBJECTS

    /** check checkmarks on both map and right side ribbon
    */
    public void foundHiddenObjective(GameObject foundObjective){
      GameObject checkmark = foundObjective.GetComponent<HiddenObject>().checkmark;
      checkmark.SetActive(true); // on map
      main.ui.objectiveCheckmarkPanels[Global.instance.currentHiddenIndex - 1].SetActive(true); // on right side ribbon

      Invoke("presentBottomPopup", 3);
    }

    void presentBottomPopup(){
      switch(Global.instance.gameState){
        case Global.GameState.FIND_HIDDEN_OBJECTS:
          main.ui.popupController.animateBottomPopup(
            objectiveDescriptionTexts[Global.instance.currentHiddenIndex],
            objectiveSprites[Global.instance.currentHiddenIndex]
          );
          break;

        case Global.GameState.FEED_AGENTS:
          break;

        case Global.GameState.CONSTRUCT_BUILDING:
          break;

        case Global.GameState.FINSISHED:
          break;
      }
    }

    public void setCurrentObjective() {
        int currentObjectiveIndex = Global.instance.currentHiddenIndex;
        // hidden objects on map
        for (int i = 0; i < hiddenObjects.Length; i++ ) {
          bool chk = currentObjectiveIndex >= i + 1;
          bool foundLastObj = currentObjectiveIndex >= hiddenObjects.Length;
          //Debug.Log("index=" + i + " curObjIndex=" + currentObjectiveIndex + " check=" + chk);
          // GameObject checkmark = hiddenObjects[i].GetComponent<HiddenObject>().checkmark;
          // checkmark.SetActive(chk);

          if(chk){
            string foundText = objectiveFoundTexts[i];
            string nextObjective;
            if(foundLastObj){
              nextObjective = objectiveDescriptionFinishedText;
            }else{
              nextObjective = objectiveDescriptionTexts[i + 1];
            }
          }
        }

        // UI of hidden objects
        int sprIndex = Mathf.Clamp(currentObjectiveIndex, 0, objectiveSprites.Length-1);

        //right corner thumbnail of current objective
        main.ui.currentObjectivePanel.GetComponent<Image>().sprite = objectiveSprites[sprIndex];

        // right corner thumbnail image of objective in bookmark list
        for (int i = 0; i <= currentObjectiveIndex && i < objectiveSprites.Length; i++) {
            main.ui.objectivePanels[i].GetComponent<Image>().sprite = objectiveSprites[i];
        }

        // right corner thumbnail image of checkmark in bookmark list
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


    // FEED_AGENTS

    // remove the images of the hidden objects from previous gamestate
    public void prepareFeedingState(){
      // // small ribbons on the right on the HUD
      Invoke("removeCheckmarksFromHiddenObjects", 3);

      //main.ui.showPopupObjectiveWithText(objectiveDescriptionFinishedText);
      spawnHotdogAgent();
    }

    public void startCountDown(){
      startedCountDown = true;
      timeLeft = 60;
    }

    void removeCheckmarksFromHiddenObjects(){
      for (int i = 0; i < hiddenObjects.Length; i++ ){
        GameObject checkmark = hiddenObjects[i].GetComponent<HiddenObject>().checkmark;
        checkmark.SetActive(false);
      }
    }

    public void spawnHotdogAgent(){
      // Main main = GameObject.Find("Main").GetComponent<Main>();
      if(hotdogAgents.Length > 0){ // should stop after 6 runs … if(hotdogAgentCount <= 6)
        int index = Random.Range(0, hotdogAgents.Length - 1);
        GameObject customer = hotdogAgents[index];

        if(currentHungryCustomer != null
          && customer.GetComponent<SpriteRenderer>().sprite.name == currentHungryCustomerString){
            spawnHotdogAgent(); // new customer is identical to old - try again
            return;
        }

        main.ui.currentObjectivePanel.GetComponent<Image>().sprite = customer.GetComponent<SpriteRenderer>().sprite;
        customer.SetActive(true);

        System.Collections.Generic.List<GameObject> list = new System.Collections.Generic.List<GameObject>(hotdogAgents);
        list.Remove(customer);
        hotdogAgents = list.ToArray();
        currentHungryCustomer = customer;
        currentHungryCustomerString = customer.GetComponent<SpriteRenderer>().sprite.name;
      }else{ // found them all
        timeLeft = 0;
        foundHungryAgent();
      }
    }

    /** remove a hotdog from the right side HUD because it was used to feed
        a hungry customer
        TODO : rotate the arrow correctly
    */
    public void foundHungryAgent(GameObject customer = null){
      // Main main = GameObject.Find("Main").GetComponent<Main>();

      // check if are in correct state
      if(Global.instance.gameState == Global.GameState.FEED_AGENTS){
        if(timeLeft <= 0){ // time is up
          if(customer != null) customer.GetComponent<HungryCustomer>().feed();

          main.ui.popupController.animateBottomPopup(
            "Good job! Now it is time to build! How fast can you find the parts for the building? The timer will start when you click the building area.",
            popupConstructionImage
          );
          Global.instance.gameState = Global.GameState.CONSTRUCT_BUILDING;
          arrow.enabled = true;
          finger.begin(FingerController.Tempo.SLOW);

          // set up the building image - the hammer
          main.ui.currentObjectivePanel.GetComponent<Image>().sprite = buildSprite;
          startedCountDown = false;
        }else{ // if not last remove one from bottom (-1 because the HUD only contains 5)
          //main.ui.objectiveCheckmarkPanels[hotdogsLeft - 1].SetActive(true);
          hungryCustomersFound++;

          // change image
          customer.GetComponent<HungryCustomer>().feed();
          // spawn new
          spawnHotdogAgent();
        }
      }
    }

    // CONSTRUCT_BUILDING

    public void startTimerForConstruction(){
      countUp = true;
    }

    public GameObject getNextHiddenBuildPart(){
      foreach(GameObject obj in hiddenBuildingParts){
        if(!obj.GetComponent<HiddenBuildPart>().found){
          return obj;
        }
      }

      return null;
    }

    public void foundPart(GameObject foundPart){
      if(foundPart.GetComponent<HiddenBuildPart>().found) return; // already found?
      foundParts++;

      foundPart.GetComponent<HiddenBuildPart>().found = true;

      // go through all the silhouettes and replace the one with matching name
      foreach(GameObject obj in silhouettes){
        if(obj.name == foundPart.name){
          // replace with full sprite
          obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
      }

      foundPart.GetComponent<HiddenBuildPart>().flyToDestination();

      // make it possible to start the construction
      if(foundParts >= requiredParts){
        isReadyToBuild = true;
        finger.begin(FingerController.Tempo.FAST);
      }
    }

    public void initHiddenBuildParts(){
      // main.ui.currentObjectivePanel.GetComponent<Image>().sprite = hiddenBuildingParts[foundParts].GetComponent<SpriteRenderer>().sprite;
      // main.ui.currentObjectivePanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);
      foreach(GameObject obj in hiddenBuildingParts){
        obj.SetActive(true);
      }
    }

    // TODO : animate the smoke
    public void animateSmoke(){
      smokeController.begin();
    }

    // TODO : make a common method to show the smoke.
    // maybe let the smoke be a part for itself, and then go through the stages
    // to build the final construction (like India right now)
    public void prepareBuildingSmoke(){
      if(Global.instance.currentLevelIndex == 0){ // greenland
        // [1] = smoking stage
        StartCoroutine(fadeInObject(buildingStages[1]));
        buildingStages[0].gameObject.SetActive(false);
      }else{ // India
        smokeController.showBackgroundSmoke();
        buildingStages[1].gameObject.SetActive(false);
      }


      // animation part of smoke
      smokeController.begin();

      // deactivate the build parts as they are used
      foreach(GameObject obj in hiddenBuildingParts){
        obj.SetActive(false);
      }
    }

    // initate the final animation between the building stages
    // TODO : same as prepareBuildingSmoke
    public IEnumerator finishBuilding(){
      if(Global.instance.currentLevelIndex == 0){ // greenland
        StartCoroutine(fadeInObject(buildingStages[2]));
        StartCoroutine(fadeOutObject(buildingStages[1]));

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(fadeInObject(buildingStages[3]));
        StartCoroutine(fadeOutObject(buildingStages[2]));

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(fadeInObject(buildingStages[4]));
        StartCoroutine(fadeOutObject(buildingStages[3]));

      }else{ // India
        buildingStages[0].gameObject.SetActive(false);
        StartCoroutine(fadeOutObject(buildingStages[2]));
        StartCoroutine(fadeInObject(buildingStages[3]));
      }
    }

    public IEnumerator fadeInObject(GameObject objectToFade){
      SpriteRenderer sp = objectToFade.GetComponent<SpriteRenderer>();

      sp.color = new Color(1f, 1f, 1f, 0.1f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.2f);
      yield return new WaitForSeconds(.01f);
      sp.color = new Color(1f, 1f, 1f, 0.3f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.4f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.5f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.6f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.7f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.8f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.9f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 1f);
    }

    public IEnumerator fadeOutObject(GameObject objectToFade){
      SpriteRenderer sp = objectToFade.GetComponent<SpriteRenderer>();

      sp.color = new Color(1f, 1f, 1f, 0.9f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.8f);
      yield return new WaitForSeconds(.01f);
      sp.color = new Color(1f, 1f, 1f, 0.7f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.6f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.5f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.4f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.3f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.2f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0.1f);
      yield return new WaitForSeconds(.05f);
      sp.color = new Color(1f, 1f, 1f, 0);
    }
}
