using UnityEngine;
using System.Collections;

public static class GameConstants {

	public const string progress = "worldScreenProgress";

	#region PopupMessages in World
	public const string POPUP_MESSAGE_WELCOME = "Hello, can you help me find my missing objects?";
	public const string POPUP_MESSAGE_NEXT_LEVEL = "Well done! Now you are ready to go a new world.";
	public const string POPUP_MESSAGE_GAME_DONE = "You have cleared all available levels. Good job!";
	#endregion

	#region Tags
	public const string TAG_HUNGRY_CUSTOMER = "HUNGRY_CUSTOMER";
	public const string TAG_HIDDEN_OBJECT = "HIDDEN_OBJECT";

	public const string TAG_BUILD_AREA = "BUILD_AREA";
	public const string TAG_BUILD_PART = "BUILD_PART";
	public const string TAG_SMOKE = "SMOKE";
	#endregion
}