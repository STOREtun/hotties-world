using UnityEngine;
using System.Collections;

public enum GameState{
	FIND_HIDDEN_OBJECTS,
	FEED_AGENTS,
	CONSTRUCT_BUILDING,
	FINISHED
}

public class Global  {

    public int currentLevelIndex;
    public int completedLevels;    // the progression of levels so far
    public int currentHiddenIndex;
    public int hintCount;

    public int gamelaunchcount;

    public IAPManager iapManager = null;

	public GameState gameState;

    protected Global() {} //guarantee singleton
    private static Global _instance = null;
    public static Global instance {
        get {
            if (_instance == null) {
                _instance = new Global();
                _instance.init();
            }
            return _instance;
        }
    }

    private void init() {
		gamelaunchcount = PlayerPrefs.GetInt("gamelaunchcount", -1);
        gamelaunchcount++;
        //gamelaunchcount = 0; //testing...

        //default values
        currentHiddenIndex  = 0;
		gameState           = GameState.FIND_HIDDEN_OBJECTS; // GameState.CONSTRUCT_BUILDING;

        currentLevelIndex   = PlayerPrefs.GetInt("currentlevelindex", 0);
        completedLevels     = PlayerPrefs.GetInt("completedLevels", 0);
        hintCount           = PlayerPrefs.GetInt("hintCount", 3);

        PlayerPrefs.SetInt("currentLevelIndex", currentLevelIndex);
        //PlayerPrefs.SetInt("currentHiddenIndex", currentHiddenIndex);
        PlayerPrefs.SetInt("hintCount", hintCount);
        PlayerPrefs.SetInt("completedLevels", completedLevels);
    }

    private GameState convertStringToGameState(string _state){
      GameState convertState = GameState.FIND_HIDDEN_OBJECTS;

      // switch case does not work since gameState.ToString() != GameState.anystate
      if(_state.Contains("FIND_HIDDEN_OBJECTS")){
        convertState = GameState.FIND_HIDDEN_OBJECTS;
      }else if(_state.Contains("FEED_AGENTS")){
        convertState = GameState.FEED_AGENTS;
      }else if(_state.Contains("CONSTRUCT_BUILDING")){
        convertState = GameState.CONSTRUCT_BUILDING;
      }else if(_state.Contains("FINSISHED")){
        convertState = GameState.FINISHED;
      }

      return convertState;
    }


    public static Transform findGameObjectWithTag(GameObject parent, string strTag) {
        foreach (Transform o in parent.transform) {
            if (o.gameObject.tag == strTag)
                return o;

        }
        return null;
    }

    public void Reset(){
      currentHiddenIndex = 0;
      gameState = GameState.FIND_HIDDEN_OBJECTS;
    }


    public void UpdateCurrentHiddenIndex(){
		currentHiddenIndex++;
    }

	/// <summary>
	/// Changes the game state and configures the game to thew new state.
	/// </summary>
	/// <param name="newState">New state.</param>
    public void ChangeGameState(GameState newState){
		gameState = newState;
    }

    public int GetCurrentHiddenIndex(){
      return currentHiddenIndex;
    }

    public void updatePlayerPrefWithInt(string pref, int value){
      int currentValue = PlayerPrefs.GetInt(pref);
      PlayerPrefs.SetInt(pref, currentValue + value);
    }

    public void updatePlayerPrefWithString(string pref, string value){
      PlayerPrefs.SetString(pref, value);
    }

    public void updateHintcountWith(int count){
      PlayerPrefs.SetInt("hintCount", hintCount + count);
      hintCount = PlayerPrefs.GetInt("hintCount", hintCount);
    }

    public void reloadNumberOfHints(){
      hintCount = PlayerPrefs.GetInt("hintCount", hintCount);
    }

    /** returns score if the level progress is stored in PlayerPrefs.
        otherwise return 0
    */
    public int GetScoreForLevel(int level){
      return PlayerPrefs.GetInt("levelScore_" + level, 0);
    }

    public void SaveScoreAndCompleteLevel(int score){
      int currentScore = PlayerPrefs.GetInt("levelScore_" + currentLevelIndex, 0);
      if(score > currentScore){ // better score, therefore we update it
          PlayerPrefs.SetInt("levelScore_" + currentLevelIndex, score);
      }

      // only increment if is first time playing the level
      if(currentLevelIndex >= completedLevels){
        completedLevels++;
        PlayerPrefs.SetInt("completedLevels", completedLevels);
      }
    }
}
