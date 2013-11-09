using System.Collections.Generic;
using System.Linq;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using Newtonsoft.Json;
using System.Text;

namespace RocksmithToolkitLib.ToolkitTone
{
    public class ToolkitPedalRS2014 : IToolkitPedal
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Key { get; set; }
        public IList<ToolkitKnob> Knobs { get; set; }
        public bool Bass { get; set; }

        public PedalType TypeEnum
        {
            get
            {
                switch (Type.ToLower())
                {
                    case "amps":
                        return PedalType.Amp;
                    case "cabinets":
                        return PedalType.Cabinet;
                    case "racks":
                        return PedalType.Rack;
                    default:
                        return PedalType.Pedal;
                }
            }
        }

        public string DisplayName
        {
            get
            {
                string prefix = (Bass) ? "(Bass) " : "";
                switch (TypeEnum)
                {
                    case PedalType.Amp:
                        return string.Format("{0}{1}", prefix, Name);
                    case PedalType.Cabinet:
                    case PedalType.Rack:
                        return string.Format("{0}{1} {2}", prefix, Name, Category);
                    default:
                        return string.Format("{0}: {1}{2}", Category, prefix, Name);
                }
            }
        }

        public PedalRS2014 MakePedalSetting()
        {
            return new PedalRS2014
            {
                Key = Key,
                KnobValues = Knobs.ToDictionary(k => k.Key, k => k.DefaultValue)
            };
        }

        public static IList<ToolkitPedalRS2014> LoadFromResource()
        {
            var pedalsJson = Encoding.ASCII.GetString(Properties.Resources.pedals2014);
            return JsonConvert.DeserializeObject<IList<ToolkitPedalRS2014>>(pedalsJson);
        }
    }
}