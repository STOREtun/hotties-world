﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/** Could remove iapCanvas, it is not used since the shop was moved into the
notebook
*/

public class UI : MonoBehaviour {
    public GameObject currentObjectivePanel; //set by inspector
    public GameObject[] objectivePanels; //set by inspector
    public GameObject[] objectiveCheckmarkPanels; //set by inspector
    public GameObject hintNumber; //inspector

    public GameObject menuPanel;
    public GameObject iapCanvas;

    public GameObject notebookOverlayCanvas;
    public GameObject notebookObjectiveDescriptionText;
    public GameObject notebookHUDCanvas;

    public GameObject notebookObjectiveTabPanel;
    public GameObject notebookHelpTabPanel;
    public GameObject notebookWorldmapTabPanel;
    public GameObject notebookOptionsTabPanel;
    public GameObject closeMenuButton;
    public Sprite notebookObjectiveSpriteFocus;
    public Sprite notebookObjectiveSpriteNonFocus;
    public Sprite notebookHelpSpriteFocus;
    public Sprite notebookHelpSpriteNonFocus;
    public Sprite notebookWorldmapSpriteFocus;
    public Sprite notebookWorldmapSpriteNonFocus;
    public Sprite notebookOptionsSpriteFocus;
    public Sprite notebookOptionsSpriteNonFocus;

    public GameObject notebookObjectiveHolderPanel;
    public GameObject notebookHelpHolderPanel;
    public GameObject notebookWorldmapHolderPanel;
    public GameObject notebookOptionsHolderPanel;

    public GameObject WorldMapText;

    public GameObject notebookRibbonHolderPanel;
    public GameObject notebookCaptionText;

    public GameObject[] notebookObjectivePanels;
    public GameObject[] notebookObjectiveCheckmarkPanels;

    public GameObject notebookButtonPanel;
    public GameObject notebookButtonText;

    public GameObject popupOverlay;
    public GameObject popupText;

    private Main main = null;

    public void showPopupObjectiveWithText(string text){
      showMenu(false);
      popupText.GetComponent<Text>().text = text;
      popupOverlay.SetActive(true);
    }

    public void hidePopupObjective(){
      popupOverlay.SetActive(false);
    }

    public void toggleMenu() {
      //showMenu(!menuPanel.activeSelf);
      toggleNotebook(NotebookMode.OPTIONS_TAB);
    }

    public void changeObjectiveTextTo(string newText){
      notebookObjectiveDescriptionText.GetComponent<Text>().text = newText;
    }

    //Could show latest menu instead of always showing the options
    public void showMenu(bool show) {
      showNotebook(NotebookMode.OPTIONS_TAB, show);
    }

    public void showShop(bool show) {
      iapCanvas.SetActive(show);
    }

    public void showHUD(bool show){
      notebookHUDCanvas.SetActive(show);
    }

    public enum NotebookMode { CLOSED, OBJECTIVE_TAB, HELP_TAB, WORLDMAP_TAB, OPTIONS_TAB };
    public NotebookMode notebookMode = NotebookMode.CLOSED;

    public void toggleNotebook(NotebookMode mode) {
        showNotebook(mode, !notebookOverlayCanvas.activeSelf);
    }
    public void showNotebook(NotebookMode mode, bool show) {
        popupOverlay.SetActive(false); // make sure popup is closed

        notebookMode = show ? mode : NotebookMode.CLOSED;
        notebookOverlayCanvas.SetActive(show);
        if (show) {
            //hilite/dim tabs(bookmarks)
            notebookObjectiveTabPanel.GetComponent<Image>().sprite = (mode == NotebookMode.OBJECTIVE_TAB ? notebookObjectiveSpriteFocus : notebookObjectiveSpriteNonFocus);
            notebookHelpTabPanel.GetComponent<Image>().sprite = (mode == NotebookMode.HELP_TAB ? notebookHelpSpriteFocus : notebookHelpSpriteNonFocus);
            notebookWorldmapTabPanel.GetComponent<Image>().sprite = (mode == NotebookMode.WORLDMAP_TAB ? notebookWorldmapSpriteFocus : notebookWorldmapSpriteNonFocus);
            notebookOptionsTabPanel.GetComponent<Image>().sprite = (mode == NotebookMode.OPTIONS_TAB ? notebookOptionsSpriteFocus : notebookOptionsSpriteNonFocus);

            //hide/show current tab contents
            notebookObjectiveHolderPanel.SetActive(mode == NotebookMode.OBJECTIVE_TAB);
            notebookHelpHolderPanel.SetActive(mode == NotebookMode.HELP_TAB);
            notebookWorldmapHolderPanel.SetActive(mode == NotebookMode.WORLDMAP_TAB);
            notebookOptionsHolderPanel.SetActive(mode == NotebookMode.OPTIONS_TAB);

            notebookRibbonHolderPanel.SetActive(mode == NotebookMode.OBJECTIVE_TAB); //only show when objective tab
            notebookCaptionText.GetComponent<Text>().text = main.level.objectiveCaptionText;

            notebookButtonPanel.SetActive(false);
            notebookHUDCanvas.SetActive(false);

            if (mode == NotebookMode.OBJECTIVE_TAB) {
                //print("UI, " + Global.instance.currentHiddenIndex + " >= 0 " + Global.instance.currentHiddenIndex + " < " + main.level.objectiveDescriptionTexts.Length);
                if (Global.instance.currentHiddenIndex >= 0 && Global.instance.currentHiddenIndex < main.level.objectiveDescriptionTexts.Length) {
                    //rule: when index in range, it means player is still searching for hidden objects
                    string str = main.level.objectiveDescriptionTexts[Global.instance.currentHiddenIndex].Replace("\\n", "\n"); //yet another Unity workaround (inspector escapes the backslash, so we need to unescape it)
                    notebookObjectiveDescriptionText.GetComponent<Text>().text = str;
                } else {
                    //rule: when index is > array, it means player has found last hidden object
                    string str = main.level.objectiveDescriptionFinishedText.Replace("\\n", "\n"); //yet another Unity workaround (inspector escapes the backslash, so we need to unescape it)
                    notebookObjectiveDescriptionText.GetComponent<Text>().text = str;
                }
                //set big ribbon objectives
                for (int i = 0; i <= Global.instance.currentHiddenIndex && i < main.level.objectiveSprites.Length; i++) {
                    notebookObjectivePanels[i].GetComponent<Image>().sprite = main.level.objectiveSprites[i];
                }
                for (int i = 0; i < notebookObjectiveCheckmarkPanels.Length; i++) {
                    notebookObjectiveCheckmarkPanels[i].SetActive(i <= Global.instance.currentHiddenIndex - 1);
                }
                // notebookButtonText.GetComponent<Text>().text = (Global.instance.currentHiddenIndex >= main.level.objectiveSprites.Length ? "FINISH" : "PLAY");
                // notebookButtonPanel.SetActive(true);
             } //else if (mode == NotebookMode.HELP_TAB) {
            //     notebookButtonText.GetComponent<Text>().text = "CLOSE";
            //     notebookButtonPanel.SetActive(true);
            // } else if (mode == NotebookMode.WORLDMAP_TAB) {
            //     notebookButtonText.GetComponent<Text>().text = "RETURN";
            //     notebookButtonPanel.SetActive(true);
            // } else if (mode == NotebookMode.OPTIONS_TAB) {
            //     notebookButtonText.GetComponent<Text>().text = "CLOSE";
            //     notebookButtonPanel.SetActive(true);
            // }

        }else{
          if(!iapCanvas.activeSelf){
            notebookHUDCanvas.SetActive(true);
          }
        }

    }

	// Use this for initialization
	void Awake () {
    main = FindObjectOfType<Main>();
	}

	// Update is called once per frame
	void Update () {
	}
}
