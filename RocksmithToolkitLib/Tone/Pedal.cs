using System.Collections.Generic;
using System.Linq;

namespace RocksmithToolkitLib.Tone
{
    public class Pedal
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Key { get; set; }
        public IList<Knob> Knobs { get; set; }
        public bool AllowLoop { get; set; }
        public bool AllowPost { get; set; }
        public bool AllowPre { get; set; }

        public string DisplayName
        {
            get
            {
                switch (Type.ToLower())
                {
                    case "amp":
                        return Name;
                    case "cabinet":
                        return string.Format("{0} {1}", Name, Category);
                    default:
                        return string.Format("{0}: {1}", Category, Name);
                }
            }
        }

        public DLCPackage.Tone.Pedal MakePedalSetting()
        {
            return new DLCPackage.Tone.Pedal
            {
                PedalKey = Key,
                KnobValues = Knobs.ToDictionary(k => k.Key, k => k.DefaultValue)
            };
        }
    }
}