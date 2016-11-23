using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    public GameObject[] hungryCustomers;
    public GameObject currentHungryCustomer;
    [HideInInspector] private string currentHungryCustomerString;
    public int hungryCustomersFound;
    public bool startedCountDown;
    public float timeLeft;

    // CONSTRUCT_BUILDING
    public GameObject[] hiddenBuildingParts;
    public GameObject[] silhouettes;
    public GameObject[] buildingStages;
    //public GameObject popup;
    public FingerController finger;
    public Sprite popupConstructionImage;
    public SmokeAnimationController smokeController;
    public float timeSpend;
    public bool countUp;
    public Sprite buildSprite;
    public string buildText;

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
		finger.Stop();
		smokeController.Stop();
		arrow.enabled = false;
    }

    void Update(){
		if(startedCountDown){
			if(!main.ui.timeObject.activeInHierarchy)
				main.ui.timeObject.SetActive(true);

			timeLeft -= Time.deltaTime;

			if(timeLeft < 0){
				timeLeft = 0;
				FoundHungryAgent();
			}
			main.ui.timeText.text = timeLeft.ToString();
		}

		//if(countUp){
		//	if(!main.ui.timeObject.activeInHierarchy)
		//		main.ui.timeObject.SetActive(true);

		//	timeSpend += Time.deltaTime;
		//	main.ui.timeText.text = timeSpend.ToString();
		//}
    }

	#region Hidden Objects
	public void FoundHiddenObjective(GameObject foundObjective){
		GameObject checkmark = foundObjective.GetComponent<HiddenObject>().checkmark;
		checkmark.SetActive(true); // on map
		main.ui.objectiveCheckmarkPanels[Global.instance.currentHiddenIndex - 1].SetActive(true); // on right side ribbon

		Invoke ("PresentBottomPopup", 3.0f);
    }

    private void PresentBottomPopup(){
		List<string> texts = new List<string> ();
		Sprite img = null;

		switch(Global.instance.gameState){

		case GameState.FIND_HIDDEN_OBJECTS:
			texts.Add(objectiveDescriptionTexts [Global.instance.currentHiddenIndex]);
			img = objectiveSprites [Global.instance.currentHiddenIndex];
			break;

		case GameState.FEED_AGENTS:
			texts.Add(main.level.objectiveDescriptionFinishedText);
			break;

        case GameState.CONSTRUCT_BUILDING:
          break;

        case GameState.FINISHED:
          break;
		}

		main.ui.popupController.ShowBottomPopup (
			texts.ToArray(),
			img
		);
    }

    public void SetCurrentObjective() {
		int currentObjectiveIndex = Global.instance.currentHiddenIndex;
        // hidden objects on map
        for (int i = 0; i < hiddenObjects.Length; i++ ) {
			bool chk = currentObjectiveIndex >= i + 1;
			bool foundLastObj = currentObjectiveIndex >= hiddenObjects.Length;
		}

        // UI of hidden objects
        int sprIndex = Mathf.Clamp(currentObjectiveIndex, 0, objectiveSprites.Length-1);

        //right corner thumbnail of current objective
        //main.ui.currentObjectivePanel.GetComponent<Image>().sprite = objectiveSprites[sprIndex];

        // right corner thumbnail image of objective in bookmark list
        for (int i = 0; i <= currentObjectiveIndex && i < objectiveSprites.Length; i++) {
            main.ui.objectivePanels[i].GetComponent<Image>().sprite = objectiveSprites[i];
        }

        // right corner thumbnail image of checkmark in bookmark list
        for (int i = 0; i < main.ui.objectiveCheckmarkPanels.Length; i++) {
            main.ui.objectiveCheckmarkPanels[i].SetActive(i <= currentObjectiveIndex - 1);
        }
    }

    public int GetHiddenObjectIndex(GameObject hiddenObject) {
        for (int i = 0; i < hiddenObjects.Length; i++)
            if (hiddenObject == hiddenObjects[i])
                return i;
        return -1;
    }
	#endregion

	#region FeedAgents
    public void SpawnNextHungryCustomer(){
		if(hungryCustomersFound == 0){
			RemoveCheckmarksFromHiddenObjects ();
			StartCountDown ();
		}

		SpawnHungryCustomer();
    }

    private void StartCountDown(){
      startedCountDown = true;
      timeLeft = 60;
    }

    private void RemoveCheckmarksFromHiddenObjects(){
		for (int i = 0; i < hiddenObjects.Length; i++ ){
			GameObject checkmark = hiddenObjects[i].GetComponent<HiddenObject>().checkmark;
			checkmark.SetActive(false);
		}
    }

	IEnumerator activeCustomer;
	IEnumerator VibrateHungryCustomer (GameObject customer){
		int count = 0;
		Vector3 offset = new Vector3 (0, 0.05f, 0);

		while (true || timeLeft < 0) {
			if (count >= 10) {
				count = 0;
				offset = -offset;
			} else {
				customer.transform.position += offset;
			}

			count++;

			yield return null;
		}
	}

	public void SpawnHungryCustomer(){
		if(hungryCustomers.Length > 0){
			int index = Random.Range(0, hungryCustomers.Length - 1);
			GameObject customer = hungryCustomers[index];

	        if(currentHungryCustomer != null
	          && customer.GetComponent<SpriteRenderer>().sprite.name == currentHungryCustomerString){
					SpawnHungryCustomer(); // new customer is identical to old - try again
					return;
	        }

	        //main.ui.currentObjectivePanel.GetComponent<Image>().sprite = customer.GetComponent<SpriteRenderer>().sprite;
	        customer.SetActive(true);

	        List<GameObject> list = new List<GameObject>(hungryCustomers);
	        list.Remove(customer);
	        hungryCustomers = list.ToArray();
	        currentHungryCustomer = customer;
	        currentHungryCustomerString = customer.GetComponent<SpriteRenderer>().sprite.name;

			activeCustomer = VibrateHungryCustomer (currentHungryCustomer);
			StartCoroutine (activeCustomer);
      }else{ // found them all - done
			timeLeft = 0;
			FoundHungryAgent();
		}
    }

    public void FoundHungryAgent(GameObject customer = null){
		if(Global.instance.gameState == GameState.FEED_AGENTS){

			StopCoroutine(activeCustomer);

			if(timeLeft <= 0){ // time is up - done
				if(customer != null) customer.GetComponent<HungryCustomer>().feed();

				string[] texts = {
					buildText
				};

				main.ui.popupController.ShowBottomPopup(
					texts,
					popupConstructionImage
				);

				Global.instance.gameState = GameState.CONSTRUCT_BUILDING;
				finger.begin(FingerController.Tempo.SLOW);

				//main.ui.currentObjectivePanel.GetComponent<Image>().sprite = buildSprite;
				startedCountDown = false;
				main.ui.timeObject.SetActive (false);
			}else{ 
				hungryCustomersFound++;
				customer.GetComponent<HungryCustomer>().feed();
				SpawnHungryCustomer();
			}
		}
	}
	#endregion

	#region Construct Building
    public GameObject GetNextHiddenBuildPart(){
		foreach(GameObject obj in hiddenBuildingParts){
			if(!obj.GetComponent<HiddenBuildPart>().found){
				return obj;
			}
		}
		return null;
    }

    public void FoundPart(GameObject foundPart){
		if(foundPart.GetComponent<HiddenBuildPart>().found) return; // already found?
		foundParts++;

		foundPart.GetComponent<HiddenBuildPart>().found = true;

		// go through all the silhouettes and replace the one with matching name
		main.ui.ColorizeConstructionPart (foundPart);

		foundPart.GetComponent<HiddenBuildPart>().FlyToDestination();

		// make it possible to start the construction
		if(foundParts >= requiredParts){
			isReadyToBuild = true;
			arrow.enabled = true;
			finger.begin(FingerController.Tempo.FAST);
		}
    }

	public void InitHiddenBuildParts(bool show = true){
		foreach(GameObject obj in hiddenBuildingParts){
			obj.SetActive(show);
		}
    }

    // TODO : animate the smoke
    public void AnimateSmoke(){
		arrow.enabled = false;
		smokeController.Begin();
    }

    // TODO : make a common method to show the smoke.
    // maybe let the smoke be a part for itself, and then go through the stages
    // to build the final construction (like India right now)
    public void PrepareBuildingSmoke(){
		if(Global.instance.currentLevelIndex == 0){ // greenland
			// [1] = smoking stage
			StartCoroutine(FadeInObject(buildingStages[1]));
			buildingStages[0].gameObject.SetActive(false);
      }else{ // India
			smokeController.ShowBackgroundSmoke();
			buildingStages[1].gameObject.SetActive(false);
      }


      // animation part of smoke
      smokeController.Begin();

      // deactivate the build parts as they are used
      foreach(GameObject obj in hiddenBuildingParts){
			obj.SetActive(false);
		}
    }

    // initate the final animation between the building stages
    // TODO : same as prepareBuildingSmoke
    public IEnumerator FinishBuilding(){
		buildingStages [0].gameObject.SetActive (false);

		if(Global.instance.currentLevelIndex == 0){ // greenland
			StartCoroutine(FadeInObject(buildingStages[2]));
        	StartCoroutine(FadeOutObject(buildingStages[1]));

        	yield return new WaitForSeconds(0.5f);

        	StartCoroutine(FadeInObject(buildingStages[3]));
        	StartCoroutine(FadeOutObject(buildingStages[2]));

        	yield return new WaitForSeconds(0.5f);

        	StartCoroutine(FadeInObject(buildingStages[4]));
       		StartCoroutine(FadeOutObject(buildingStages[3]));
		}else{ // India
			buildingStages[0].gameObject.SetActive(false);
			StartCoroutine(FadeOutObject(buildingStages[2]));
			StartCoroutine(FadeInObject(buildingStages[3]));
		}
	}

    public IEnumerator FadeInObject(GameObject objectToFade){
		SpriteRenderer sp = objectToFade.GetComponent<SpriteRenderer>();

		Color c = new Color (0, 0, 0, .1f);
		for(int x = 0; x <= 10; x++){
			sp.color += c;
			yield return new WaitForSeconds(.05f);
		}
    }

    public IEnumerator FadeOutObject(GameObject objectToFade){
		SpriteRenderer sp = objectToFade.GetComponent<SpriteRenderer>();

		Color c = new Color (0, 0, 0, .1f);
		for(int x = 0; x <= 10; x++){
			sp.color -= c;
			yield return new WaitForSeconds (0.05f);
		}
    }
	#endregion
}
