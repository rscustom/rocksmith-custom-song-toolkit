using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MiscUtil.Conversion;

namespace RocksmithToolkitLib
{
    public enum GamePlatform { Pc, Mac, XBox360, PS3, None };
    public enum GameVersion { RS2012, RS2014, None };

    public class Platform
    {
        public GamePlatform platform { get; set; }
        public GameVersion version { get; set; }
        public bool IsConsole { get { return (platform == GamePlatform.XBox360 || platform == GamePlatform.PS3); } }

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

        public override bool Equals(object obj)
        {
            var o = obj as Platform;
            return o != null && o.platform == this.platform && o.version == this.version;
        }

        public override int GetHashCode()
        {
            return this.platform.GetHashCode() + this.version.GetHashCode();
        }

        public EndianBitConverter GetBitConverter
        {
            get{
                switch (platform)
                {
                    case GamePlatform.Pc:
                    case GamePlatform.Mac:
                        return EndianBitConverter.Little;
                    case GamePlatform.XBox360:
                    case GamePlatform.PS3:
                        return EndianBitConverter.Big;
                    default:
                        return EndianBitConverter.Little;
                }
            }
        }

        public override string ToString()
        {
            return String.Format("{0}_{1}", version, platform);
        }
    }
}
