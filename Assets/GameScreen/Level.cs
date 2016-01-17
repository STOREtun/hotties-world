using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Level : MonoBehaviour {

    public GameObject[] hiddenObjects;

    public Sprite[] objectiveSprites;

    public string[] objectiveDescriptionTexts;
    public string[] objectiveFoundTexts;
    public string objectiveDescriptionFinishedText;
    public string objectiveCaptionText;
    public string levelText;

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
}
