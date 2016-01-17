using UnityEngine;
using System.Collections;
using Soomla.Store;


//man maa sno sig sagde slangen
public class Global  {

    public int currentLevelIndex = 0;
    public int completedLevels = 0;    // the progression of levels so far
    public int currentHiddenIndex = 0;
    public int hintCount = 0;

    public int gamelaunchcount = -1;

    public IAPManager iapManager = null;

    private const int MAX_LEVELS = 1; // number of levels in the game (starting from 0)

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
        // reset the game
        // PlayerPrefs.SetInt("completedLevels", 0);
        // PlayerPrefs.SetInt("currentlevelindex", 0);

        gamelaunchcount = PlayerPrefs.GetInt("gamelaunchcount", -1);
        gamelaunchcount++;
        //gamelaunchcount = 0; //testing...

        currentLevelIndex = PlayerPrefs.GetInt("currentlevelindex", 0);
        currentHiddenIndex = PlayerPrefs.GetInt("currenthiddenindex", 0);
        completedLevels = PlayerPrefs.GetInt("completedLevels", 0);
        hintCount = PlayerPrefs.GetInt("hintcount", 3);
        //hintCount = 9; //testing...

        PlayerPrefs.SetInt("currentlevelindex", currentLevelIndex);
        PlayerPrefs.SetInt("currenthiddenindex", currentHiddenIndex);
        PlayerPrefs.SetInt("hintcount", hintCount);
        PlayerPrefs.SetInt("completedLevels", completedLevels);

        iapManager = new IAPManager();
        iapManager.init();
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

    /*
      Maybe not the right place for manipulating data. But this way we keep custom
      manipulation of PlayerPrefs in one place.
    */
    public void updatePlayerPrefWithValue(string pref, int value){
      int currentValue = PlayerPrefs.GetInt(pref);
      PlayerPrefs.SetInt(pref, currentValue + value); // generic for integer values

      // Update local (Global) variables NO
      // hintCount = PlayerPrefs.GetInt("hintcount");
      // currentLevelIndex = PlayerPrefs.GetInt("currentlevelindex", 0);
      // currentHiddenIndex = PlayerPrefs.GetInt("currenthiddenindex", 0);
      // completedLevels = PlayerPrefs.GetInt("completedLevels", 0);
    }

    public void reloadNumberOfHints(){
      hintCount = PlayerPrefs.GetInt("hintcount", hintCount);
    }

    public void updateLevelProgression(){
      if(currentLevelIndex >= completedLevels && currentLevelIndex < MAX_LEVELS){
        updatePlayerPrefWithValue("completedLevels", 1);
        completedLevels = PlayerPrefs.GetInt("completedLevels", 0);
      }
    }


}
