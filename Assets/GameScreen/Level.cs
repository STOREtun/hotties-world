using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Level : MonoBehaviour {

    // HIDDEN_OBJECT
    public GameObject[] hiddenObjects;

    public Sprite[] objectiveSprites;

    public string[] objectiveDescriptionTexts;
    public string[] objectiveFoundTexts;
    public string objectiveDescriptionFinishedText;
    public string objectiveCaptionText;
    public string levelText;

    // FEED_AGENTS
    public GameObject[] hotdogAgents;
    private int hotdogsLeft;

    // CONSTRUCT_BUILDING
    public GameObject[] hiddenBuildingParts;
    public GameObject[] silhouettes;
    public GameObject[] buildingStages;
    public GameObject popup;

    [HideInInspector] public bool isReadyToBuild;
    public SpriteRenderer arrow;

    private const int TAPPING_BUFFER  = 20;

    [HideInInspector] public int foundParts;
    private int requiredParts;


    void Start(){
      requiredParts   = hiddenBuildingParts.Length;
      foundParts      = 0;
      isReadyToBuild  = false;
      hotdogsLeft     = 5;
    }

    void Update(){
      if(Global.instance.gameState == Global.GameState.CONSTRUCT_BUILDING){
        // TODO : stop the smoke animation
      }
    }

    public void setCurrentObjective(int currentObjectiveIndex) {
        Main main = GameObject.Find("Main").GetComponent<Main>();

        // hidden objects on map
        for (int i = 0; i < hiddenObjects.Length; i++ ) {
            bool chk = currentObjectiveIndex >= i + 1;
            bool foundLastObj = currentObjectiveIndex >= hiddenObjects.Length;
            //Debug.Log("index=" + i + " curObjIndex=" + currentObjectiveIndex + " check=" + chk);
            GameObject checkmark = hiddenObjects[i].GetComponent<HiddenObject>().checkmark;
            checkmark.SetActive(chk);

            if(chk){
              string foundText = objectiveFoundTexts[i];
              string nextObjective;
              if(foundLastObj){
                nextObjective = objectiveDescriptionFinishedText;
              }else nextObjective = objectiveDescriptionTexts[i + 1];

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

    // remove the images of the hidden objects from previous gamestate
    public void prepareFeedingState(){
      Main main = GameObject.Find("Main").GetComponent<Main>();
      // // small ribbons on the right on the HUD
      for (int i = 0; i < hiddenObjects.Length; i++ ){
        GameObject checkmark = hiddenObjects[i].GetComponent<HiddenObject>().checkmark;
        checkmark.SetActive(false);
      }

      main.ui.showPopupObjectiveWithText(objectiveDescriptionFinishedText);
    }

    // returns true if done spawning
    // TODO : do not spawn two of the same customers in a row
    public void spawnHotdogAgent(){
      Main main = GameObject.Find("Main").GetComponent<Main>();
      if(hotdogAgents.Length > 0){ // should stop after 6 runs … if(hotdogAgentCount <= 6)
        int index = Random.Range(0, hotdogAgents.Length - 1);
        GameObject customer = hotdogAgents[index];
        main.ui.currentObjectivePanel.GetComponent<Image>().sprite = customer.GetComponent<SpriteRenderer>().sprite;
        print("Level, spawning customer: " + customer);
        customer.SetActive(true);

        System.Collections.Generic.List<GameObject> list = new System.Collections.Generic.List<GameObject>(hotdogAgents);
        list.Remove(customer);
        hotdogAgents = list.ToArray();
      }
    }

    /** remove a hotdog from the right side HUD because it was used to feed
        a hungry customer
        TODO : rotate the arrow correctly
    */
    public void foundHungryAgent(GameObject customer){
      Main main = GameObject.Find("Main").GetComponent<Main>();

      // check if are in correct state
      if(Global.instance.gameState == Global.GameState.FEED_AGENTS){
        if(hotdogsLeft <= 0){ // last hotdog
          customer.GetComponent<HungryCustomer>().feed();
          main.ui.showPopupObjectiveWithText("YAY! YOU FOUND AND FED ALL THE HUNGRY CUSTOMERS! NOW IT IS TIME TO BUILD");
          Global.instance.gameState = Global.GameState.CONSTRUCT_BUILDING;
          arrow.enabled = true;
        }else{ // if not last remove one from bottom (-1 because the HUD only contains 5)
          main.ui.objectiveCheckmarkPanels[hotdogsLeft - 1].SetActive(true);
          hotdogsLeft--;

          customer.GetComponent<HungryCustomer>().feed();

          // spawn new
          spawnHotdogAgent();
        }
      }
    }

    // TODO : move the part next to building area - maybe animate it in a flying manner?
    public void foundPart(string partName){
      foundParts++;

      // go through all the silhouettes and replace the one with matching name
      foreach(GameObject obj in silhouettes){
        if(obj.name == partName){
          // replace with full sprite instead of deactivate
          obj.SetActive(false);
        }
      }

      // make it possible to start the construction
      // how to show that?
      if(foundParts >= requiredParts){
        isReadyToBuild = true;
        // StartCoroutine(fadeInObject(smoke));
      }
    }

    public void initHiddenBuildParts(){
      foreach(GameObject obj in hiddenBuildingParts){
        obj.SetActive(true);
      }
    }

    public void animateSmoke(){
      // TODO : animate the smoke
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
