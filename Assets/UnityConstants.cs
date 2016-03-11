// This file is auto-generated. Modifications are not saved.
// UnityConstantsGenerator.cs modified by Janus Kjempff
//    - documentation parts removed for readability.

namespace UnityConstants
{
    public static class Tags
    {
        public const string Untagged                = "Untagged";
        public const string Respawn                 = "Respawn";
        public const string Finish                  = "Finish";
        public const string EditorOnly              = "EditorOnly";
        public const string MainCamera              = "MainCamera";
        public const string Player                  = "Player";
        public const string GameController          = "GameController";
        public const string LOCATION_LOCKED         = "LOCATION_LOCKED";
        public const string LOCATION_OPEN           = "LOCATION_OPEN";
        public const string LOCATION_CURRENT        = "LOCATION_CURRENT";
        public const string LOCATION_WORLDMAP       = "LOCATION_WORLDMAP";
        public const string HIDDEN_OBJECT           = "HIDDEN_OBJECT";
        public const string LOCATION_LOCATION       = "LOCATION_LOCATION";
        public const string HIDDEN_OBJECT_CHECKMARK = "HIDDEN_OBJECT_CHECKMARK";
        public const string HOTDOG_AGENT            = "HOTDOG_AGENT";
        public const string SMOKE                   = "SMOKE";
    }

    public static class SortingLayers
    {
        public const int Default = 0;
    }

    public static class Layers
    {
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int Ignore_Raycast = 2;
        public const int Water = 4;
        public const int UI = 5;
        public const int TouchInputLayer = 8;

        public const int DefaultMask = 1 << 0;
        public const int TransparentFXMask = 1 << 1;
        public const int Ignore_RaycastMask = 1 << 2;
        public const int WaterMask = 1 << 4;
        public const int UIMask = 1 << 5;
        public const int TouchInputLayerMask = 1 << 8;
    }

    public static class Scenes
    {
        public const int Intro = 0;
        public const int Game = 1;
        public const int WorldMap = 2;
    }
}
