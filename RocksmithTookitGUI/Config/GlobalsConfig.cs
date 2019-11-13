using NLog;
namespace RocksmithToolkitGUI.Config
{
    public static class GlobalsConfig
    {
        public static string DefaultToneFile { get; set; }
        public static string DefaultProjectFolder { get; set; }
        public static Logger Log { get; set; }
    }

}

// CODE GRAVE YARD

// override destination platform using GeneralConfig settings
// var defaultGameVersion = (GameVersion)Enum.Parse(typeof(GameVersion), ConfigRepository.Instance()["general_defaultgameversion"]);
// var defaultPlatform = (GamePlatform)Enum.Parse(typeof(GamePlatform), ConfigRepository.Instance()["general_defaultplatform"]);
// var destPlatform = new Platform(defaultPlatform, defaultGameVersion);

