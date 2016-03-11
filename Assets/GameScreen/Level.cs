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

    // CONSTRUCT_BUILDING
    public GameObject[] hiddenBuildingParts;
    // [0] = smoke/black hole (maybe 1 should be smoke depending on what is there from the start)
    // [1] = the final construction. How the world looks after
    public GameObject[] buildArea;
    public SpriteRenderer arrow;

    [HideInInspector] public int foundParts;
    private int requiredParts;

    void Start(){
      requiredParts = hiddenBuildingParts.Length;
      foundParts    = 0;
    }

    public void setCurrentObjective(int currentObjectiveIndex) {
        Main main = GameObject.Find("Main").GetComponent<Main>();
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

              main.ui.showPopupObjectiveWithText(foundText + "\n\n" + nextObjective);
            }
        }

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

    // returns true if done spawning
    public bool spawnHotdogAgent(){
      if(hotdogAgents.Length > 0){ // should stop after 6 runs … if(hotdogAgentCount <= 6)
        int index = Random.Range(0, hotdogAgents.Length - 1);
        GameObject agent = hotdogAgents[index];
        agent.SetActive(true);

        System.Collections.Generic.List<GameObject> list = new System.Collections.Generic.List<GameObject>(hotdogAgents);
        list.Remove(agent);
        hotdogAgents = list.ToArray();

        return false;
      }
      return true;
    }

    public void initHiddenBuildParts(){
      // … init the hidden building parts
      print("Level, hidden building parts has been initiated");
    }

    public void animateSmoke(){
      Animator smokeAnim = buildArea[0].GetComponent<Animator>();
      smokeAnim.SetBool("isTapping", true);
    }

    public void initConstruction(){
      if(foundParts >= requiredParts){
        // … stop smoke
        // some transistion animation
        // … activate the finalBuilding (buildArea[1].gameObject.SetActive(true))
        // some transistion animation stops
      }
    }
}
