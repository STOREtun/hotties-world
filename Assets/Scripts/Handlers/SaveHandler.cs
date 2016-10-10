using UnityEngine;
using System.Collections;

public static class SaveHandler {

	#region WorldScreen
	// ...
	#endregion

	#region Generics
	public static void SaveInt(string key, int value){
		PlayerPrefs.SetInt (key, value);
	}

	public static int GetInt(string key){
		return PlayerPrefs.GetInt (key, 0);
	}
	#endregion
}