using System.Collections.Generic;
using System.Linq;

namespace RocksmithToolkitLib.Tone
{
    public enum PedalType
    {
        Amp,
        Cabinet,
        Pedal
    }

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
        public bool Bass { get; set; }
        public bool Metal { get; set; }

        public PedalType TypeEnum
        {
            get
            {
                switch (Type.ToLower())
                {
                    case "amp":
                        return PedalType.Amp;
                    case "cabinet":
                        return PedalType.Cabinet;
                    default:
                        return PedalType.Pedal;
                }
            }
        }

        public string DisplayName
        {
            get
            {
                string prefix = Bass ? "(Bass) " : Metal ? "(Metal) " : "";
                switch (TypeEnum)
                {
                    case PedalType.Amp:
                        return string.Format("{0}{1}", prefix, Name);
                    case PedalType.Cabinet:
                        return string.Format("{0}{1} {2}", prefix, Name, Category);
                    default:
                        return string.Format("{0}: {1}{2}", Category, prefix, Name);
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