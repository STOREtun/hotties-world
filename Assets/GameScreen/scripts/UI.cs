using UnityEngine;
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

    public PopupController popupController;

    public Sprite celebrationSprite;

    // timer
    public Text timeText;
    public GameObject timeObject;

    // popup overlay bottom
    public GameObject bottomPopupOverlay;
    public Image bottomPopupImg;
    public Text bottomPopupText;

    // popup overlay top
    public GameObject topPopupOverlay;
    public Text topPopupText;
    public Image topPopupImage;

    public GameObject[] scoreObjs;

    public Sprite done;

    private Main main = null;

    public void Finish(){
      Color color = new Color(1, 1, 1, 1);
      currentObjectivePanel.GetComponent<Image>().color = color;
      currentObjectivePanel.GetComponent<Image>().sprite = done;
    }

    public void ShowPopupObjectiveWithText(string text, Sprite img, string _secondText = null){
		string[] texts = new string[]{ text, _secondText };
		popupController.ShowBottomPopup(texts, img);
    }
		
    public void ShowCalculatedScore(string msg, int score){
		Sprite reaction = main.level.fannyReactions[score - 1];
		bottomPopupText.text = msg;

		for(int i = 0; i < 3; i++){
			Image img = scoreObjs[i].GetComponent<Image>();
			img.enabled = true;

			if((i + 1) <= score)
				img.color = new Color(1f, 1f, 1f, 1f);
		}

//		popupController.AnimateBottomPopupWithFanny(msg, reaction);
		string[] texts = new string[]{msg};
		popupController.ShowBottomPopup(texts, reaction);
    }

    public void HideBottomPopup(){
      popupController.HideBottomPopup();
    }

    public void ToggleMenu() {
      ToggleNotebook(NotebookMode.OPTIONS_TAB);
    }

    public void ChangeObjectiveTextTo(string newText){
      notebookObjectiveDescriptionText.GetComponent<Text>().text = newText;
    }

    public void ShowMenu(bool show) {
		ShowNotebook(NotebookMode.OPTIONS_TAB, show);
    }

	#region Show Specific Menu
    public void ShowShop() {
		ShowNotebook(NotebookMode.HELP_TAB, true);
    }

	public void ShowWorldMap(){
		ShowNotebook (NotebookMode.WORLDMAP_TAB, true);
	}

	public void ShowObjectiveTab(){
		ShowNotebook (NotebookMode.OBJECTIVE_TAB, true);
	}
	#endregion

    public void ShowHUD(bool show){
		notebookHUDCanvas.SetActive(show);
    }

    public enum NotebookMode {
		CLOSED,
		OBJECTIVE_TAB, 
		HELP_TAB, 
		WORLDMAP_TAB, 
		OPTIONS_TAB
	};
    public NotebookMode notebookMode = NotebookMode.CLOSED;

    public void ToggleNotebook(NotebookMode mode) {
        ShowNotebook(mode, !notebookOverlayCanvas.activeSelf);
    }

    public void ShowNotebook(NotebookMode mode, bool show) {
      notebookMode = show ? mode : NotebookMode.CLOSED;
      notebookOverlayCanvas.SetActive(show);
      if (show) {
			//hilite/dim tabs(bookmarks)
			notebookObjectiveTabPanel.GetComponent<Image>().sprite  = (mode == NotebookMode.OBJECTIVE_TAB ? notebookObjectiveSpriteFocus  : notebookObjectiveSpriteNonFocus);
			notebookHelpTabPanel.GetComponent<Image>().sprite       = (mode == NotebookMode.HELP_TAB      ? notebookHelpSpriteFocus       : notebookHelpSpriteNonFocus);
			notebookWorldmapTabPanel.GetComponent<Image>().sprite   = (mode == NotebookMode.WORLDMAP_TAB  ? notebookWorldmapSpriteFocus   : notebookWorldmapSpriteNonFocus);

			//hide/show current tab contents
			notebookObjectiveHolderPanel.SetActive(mode == NotebookMode.OBJECTIVE_TAB);
			notebookHelpHolderPanel.SetActive(mode      == NotebookMode.HELP_TAB);
			notebookWorldmapHolderPanel.SetActive(mode  == NotebookMode.WORLDMAP_TAB);
			//notebookOptionsHolderPanel.SetActive(mode   == NotebookMode.OPTIONS_TAB);

			notebookRibbonHolderPanel.SetActive(mode      == NotebookMode.OBJECTIVE_TAB); //only show when objective tab
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
					int index = Global.instance.GetCurrentHiddenIndex();
					for (int i = 0; i <= index && i < main.level.objectiveSprites.Length; i++) {
					notebookObjectivePanels[i].GetComponent<Image>().sprite = main.level.objectiveSprites[i];
				}
					for (int i = 0; i < notebookObjectiveCheckmarkPanels.Length; i++) {
					notebookObjectiveCheckmarkPanels[i].SetActive(i <= index - 1);
				}
			}

			}else{
				notebookHUDCanvas.SetActive(true);
		}
	}

	public void PresentParentalGate(){
		ParentalController.PresentParentalGate();

		#if UNITY_EDITOR
		ShowShop();
		#endif
	}

	// Use this for initialization
	void Awake () {
    	main = FindObjectOfType<Main>();
	}		
}