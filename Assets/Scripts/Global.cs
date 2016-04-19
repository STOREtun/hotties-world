using UnityEngine;
using System.Collections;


//man maa sno sig sagde slangen
public class Global  {

    public int currentLevelIndex;
    public int completedLevels;    // the progression of levels so far
    public int currentHiddenIndex;
    public int hintCount;

    public int gamelaunchcount;

    public IAPManager iapManager = null;

    private const int MAX_LEVELS = 1; // should get this number from main.levels.count

    // the three minigames
    public enum GameState{
      FIND_HIDDEN_OBJECTS,
      FEED_AGENTS,
      CONSTRUCT_BUILDING,
      FINSISHED
    }
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

    // TODO : dont delete all PlayerPrefs
    private void init() {
        // reset the game
        // PlayerPrefs.DeleteAll();
        // PlayerPrefs.SetInt("completedLevels", 0);
        // PlayerPrefs.SetInt("currentlevelindex", 0);

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

        // iapManager = new IAPManager();
        // iapManager.init();
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
        convertState = GameState.FINSISHED;
      }

      return convertState;
    }


    public static Transform findGameObjectWithTag(GameObject parent, string strTag) {
        foreach (Transform o in parent.transform) {
//        foreach (GameObject o in parent.GetComponents<GameObject>()) {
//            if (o.tag == strTag)
//                return o.transform;
            if (o.gameObject.tag == strTag)
                return o;

        }
        return null;
    }

    public void reset(){
      currentHiddenIndex = 0;
      gameState = GameState.FIND_HIDDEN_OBJECTS;
    }


    public void updateCurrentHiddenIndex(){
      currentHiddenIndex++;
    }

    public void changeGameState(GameState newState){
      gameState = newState;
    }

    public int getCurrentHiddenIndex(){
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
      updatePlayerPrefWithInt("hintCount", hintCount + count);
      reloadNumberOfHints();
    }

    public void reloadNumberOfHints(){
      hintCount = PlayerPrefs.GetInt("hintCount", hintCount);
    }

    /** returns score if the level progress is stored in PlayerPrefs.
        otherwise return 0
    */
    public int getScoreForLevel(int level){
      return PlayerPrefs.GetInt("levelScore_"+currentLevelIndex, 0);
    }

    public void saveScoreAndCompleteLevel(int score){
      int currentScore = PlayerPrefs.GetInt("levelScore_"+currentLevelIndex, 0);
      if(score > currentScore){ // better score, therefore we update it
          PlayerPrefs.SetInt("levelScore_"+currentLevelIndex, score);
      }

      // only increment if is first time playing the level
      if(currentLevelIndex >= completedLevels){
        completedLevels++;
        PlayerPrefs.SetInt("completedLevels", completedLevels);
      }
    }

    /** Old levelData - the need to save progress is no longer needed

    public JSONObject levelData;
    levelData = new JSONObject();

    public void updateLevelDataInPlayerPrefs(){
      PlayerPrefs.SetString("levelData_"+currentLevelIndex, levelData.ToString());
    }

    // update the current level json object to match levelID
    public void updateLevelData(){
       string _data = PlayerPrefs.GetString("levelData_"+currentLevelIndex, "empty");

       // _data is empty if it is the first time loading this level
       if(_data == "empty"){
         JSONObject newLevel = new JSONObject();

         newLevel.AddField("levelID", currentLevelIndex);
         newLevel.AddField("currentHiddenIndex", 0);
         newLevel.AddField("gameState", GameState.FIND_HIDDEN_OBJECTS.ToString());

         levelData = newLevel;
         PlayerPrefs.SetString("levelData_"+currentLevelIndex, levelData.ToString());
       }else{
         // load the data instead of creating it
         levelData = new JSONObject(_data);

         // update local level variables
         gameState          = convertStringToGameState(levelData.GetField("gameState").ToString());
         currentHiddenIndex = int.Parse(levelData.GetField("currentHiddenIndex").ToString());
       }
    }
    */
}
