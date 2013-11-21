using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib
{
    public enum GamePlatform { Pc, Mac, XBox360, PS3, None };
    public enum GameVersion { RS2012, RS2014, None };

    public class Platform
    {
        public GamePlatform platform { get; set; }
        public GameVersion version { get; set; }

        public Platform(GamePlatform platform, GameVersion version)
        {
            this.platform = platform;
            this.version = version;
        }

        public Platform(string platform, string version)
        {
            this.platform = (GamePlatform)Enum.Parse(typeof(GamePlatform), platform);
            this.version = (GameVersion)Enum.Parse(typeof(GameVersion), version);
        }
    }
}
