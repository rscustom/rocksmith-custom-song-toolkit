using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;

namespace RocksmithToolkitLib.ToolkitTone
{
    public enum PedalType {
        Amp,
        Cabinet,
        Pedal,
        Rack
    }

    public class ToolkitPedal
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Key { get; set; }
        public IList<ToolkitKnob> Knobs { get; set; }
        public bool Bass { get; set; }

        #region Not used for RS2014
        public bool AllowLoop { get; set; }
        public bool AllowPost { get; set; }
        public bool AllowPre { get; set; }
        public bool Metal { get; set; }
        #endregion
        
        public PedalType TypeEnum
        {
            get{
               switch (Type.ToLower())
               {
                    case "amp":
                    case "amps":
                        return PedalType.Amp;
                    case "cabinet":
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
            get {
                string prefix = Bass ? "(Bass) " : Metal ? "(Metal) " : "";
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

        public dynamic MakePedalSetting(GameVersion gameVersion)
        {
            if (gameVersion == GameVersion.RS2014)
            {
                return new DLCPackage.Manifest.Tone.Pedal2014
                {
                    PedalKey = Key,
                    KnobValues = Knobs.ToDictionary(k => k.Key, k => k.DefaultValue),
                    Category = Category,
                    Type = Type
                };
            }
            else {
                return new Pedal
                {
                    PedalKey = Key,
                    KnobValues = Knobs.ToDictionary(k => k.Key, k => (decimal)k.DefaultValue)
                };
            }
        }

        public static IList<ToolkitPedal> LoadFromResource(GameVersion gameVersion)
        {
            var pedalsJson = Encoding.ASCII.GetString((gameVersion != GameVersion.RS2012) ? Properties.Resources.pedals2014 : Properties.Resources.pedals);
            return JsonConvert.DeserializeObject<IList<ToolkitPedal>>(pedalsJson);
        }
    }
}