using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Tone
{
    public class Tone
    {
        public string BlockAsset { get; set; }
        public string Description { get; set; }
        public List<object> ExclusiveBuild { get; set; }
        public bool IsDLC { get; set; }
        public bool IsPreviewOnlyItem { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public Dictionary<string, Pedal> PedalList { get; set; }
        public string PersistentID { get; set; }
        public string UnlockKey { get; set; }
        public decimal Volume { get; set; }

        public Tone()
        {
            PedalList = new Dictionary<string, Pedal>();
            ExclusiveBuild = new List<object>();
            UnlockKey = "";
            IsDLC = true;
            IsPreviewOnlyItem = false;
            Volume = -12;
            Description = "$[-1] ";
        }
    }
}
