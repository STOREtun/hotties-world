using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

    //Working around the Unity module concept and trying to get a more object oriented
    //way of getting an organized code base, less spaghetti coding, less outside factors
    //(inspector changes) to effect and break already tested code.
    //Also trying to get nearer a MVC (Model View Controller) approach, as much as Unity
    //will allow it.
    //-- Theory in progress.. --
    //
    //1.  All scenes has a Main (MonoBehaviour). Main holds references to all objects that needs to be
    //    referenced by the game logic. Main can hold "Model" "View" and "Controller" objects.
    //1.1 Main must always be first in execution order so it can initialize whatever needed.
    //    Main acts as an alternative to a singleton/global, being specific to a scene.
    //    Globals/singletons may still need to exist, but should be referenced through Main.
    //    "Model" objects CAN NOT reference Main, as they must be platform independent.
    //1.2 Do NOT use main for handling any frame logic, Main is purely for use as a type safe
    //    object holder and parameterized helper functions needed by more than one script
    //
    //2.  Avoid referencing objects by string, use strong typing (if you do not understand
    //    type safe or why it is a extremely important concept, start learning NOW).
    //
    //3. MVC.
    //Finding a method to implement a more MVC (http://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller)
    //way of working. This is clashing a bit with the way Unity wants us to work, but it
    //will not cause any fundamental problems.. -- theory in progess... --
    //Model: Game data and game logic independent of presentation. DO NOT MAKE THIS A
    //       UNITY SCRIPT, IT IS AN INDEPENDENT OBJECT THAT IN THEORY SHOULD BE ABLE TO
    //       COMPILE IN ANY NON UNITY c# PROJECT. Example: Player object
    //       holding data about progress, highscore, level, inventory and logic to manipulate these.
    //View: Gameobjects, prefabs and other visual stuff
    //Controller: Unity Scripts that are the glue between Model and View, typically
    //            handles input and connects screen logic with game logic.
    //            Example: Screen script that handles input, networking, loading, etc



    public GameObject[] levels; //set by inspector

    public GameObject loadLevel(int levelIndex) {
        if (levelIndex < 0 || levelIndex >= levels.Length)
            Debug.LogError("Tried to load an unknown level " + levelIndex);
        levelObj = Instantiate(levels[Global.instance.currentLevelIndex], levels[Global.instance.currentLevelIndex].transform.position, Quaternion.identity) as GameObject;
        levelObj.SetActive(true);

        level = levelObj.GetComponent<Level>();

        return levelObj;
    }

    [HideInInspector]
    public GameObject levelObj = null;

    [HideInInspector]
    public Level level = null;

    public GameObject uiObj; //set by inspector

    [HideInInspector]
    public UI ui = null;

    //[HideInInspector]
    //public GameScene gameSceneScr = null;

	void Awake () {
        Global global = Global.instance; //might aswell get globals loaded if it is not yet
//        gameSceneScr = GetComponent<GameScene>();
        ui = uiObj.GetComponent<UI>();
    }

}
