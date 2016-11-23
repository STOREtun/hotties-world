using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

	public static Main instance;

	[SerializeField] public GameObject uiObj; // set by inspector
	[SerializeField] public GameObject[] levels;
	[SerializeField] public Purchaser purchaser;
	[HideInInspector] public SoundHandler soundHandler;

	[HideInInspector] public GameObject levelObj = null;
	[HideInInspector] public Level level = null;
	[HideInInspector] public UI ui = null;

    public GameObject InstantiateLevel(int levelIndex) {        
		levelObj = Instantiate(
			levels[Global.instance.currentLevelIndex], 
			levels[Global.instance.currentLevelIndex].transform.position, 
			Quaternion.identity
		) as GameObject;

        levelObj.SetActive(true);

        level = levelObj.GetComponent<Level>();

        return levelObj;
    }


	void Awake () {
        Global global = Global.instance;
        ui = uiObj.GetComponent<UI>();
    }

	void Start(){
		if(instance == null){
			instance = this;
		}

		soundHandler = GameObject.FindGameObjectWithTag("SOUNDHANDLER").GetComponent<SoundHandler>();
	}
}