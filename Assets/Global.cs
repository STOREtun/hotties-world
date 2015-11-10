using UnityEngine;
using System.Collections;


//man maa sno sig sagde slangen
public class Global  {

    public int currentLevelIndex = 0;
    public int currentHiddenIndex = 0;
    public int hintCount = 0;

    public int gamelaunchcount = -1;

    public IAPManager iapManager = null;

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
        gamelaunchcount = 0; //testing...

        currentLevelIndex = PlayerPrefs.GetInt("currentlevelindex", 0);
        currentHiddenIndex = PlayerPrefs.GetInt("currenthiddenindex", 0);
        hintCount = PlayerPrefs.GetInt("hintcount", 3);
        hintCount = 9; //testing...

        iapManager = new IAPManager();
        iapManager.init();


        //PlayerPrefs.SetInt("currentlevelindex", currentLevelIndex);
        //PlayerPrefs.SetInt("currenthiddenindex", currentHiddenIndex);
        //PlayerPrefs.SetInt("hintcount", hintCount);
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


}
