using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RocksmithToolkitLib.ToolkitTone;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace pedalgen
{
    public class RsTone
    {
        public bool AllowLoop;
        public bool AllowPost; // true
        public bool AllowPre; // true
        public string AssociatedPedalKey; // "Amp_SoloMega"
        public string BlockAsset; // "urn:emergent-world:Pedal_Default"
        public string CameraLocator; // "camera_locator_cab"
        public string Category; // "57 Cone"
        public string Description; // "$[24012] This modern close back 1X12 cabinet has a tight, focused, and directional sound."
        public RsEffect[] Effects;
        public string[] ExclusiveBuild; // []
        public bool IsDLC; // false
        public bool IsPreviewOnlyItem; // false
        [JsonProperty("TonePedalRTPCName")]
        public string Key; // "Cab_1X12_MegaModern_57_Cone"
        public RsKnob[] Knobs; // []
        public string Name; // "Megan 1X12"
        public string NifAsset; // "urn:llid:3cd469ff-0000-0000-0000-000000000000"
        public string PersistentID; // "065FDEFB21754A7D851AB34A9543239F"
        public bool Rare; // false
        public string SoundBank; // "Cab_1X12_MegaModern.bnk"
        public string TonePedalRTPCName; // "Cab_1X12_MegaModern_57_Cone"
        public string Type; // "Cabinet"
        public string UnlockKey; // "Cabinet_Cab_1X12_MegaModern_57_Cone"
        public int UnlockOrder; // -1

        public ToolkitPedal ToPedal()
        {
            return new ToolkitPedal
            {
                AllowLoop = AllowLoop,
                AllowPost = AllowPost,
                AllowPre = AllowPre,
                Category = Category,
                Key = Key,
                Knobs = Knobs.Select(knob => knob.ToKnob()).ToList(),
                Name = Name,
                Type = Type,
                Bass = TonePedalRTPCName.Contains("Bass"),
                Metal = IsDLC
            };
        }
    }
    public class RsEffect
    {
        public string Name;
    }
    public class RsKnob
    {
        private static Regex BadName = new Regex(@"^\$\[\d{5}\]$");

        public string Name; // "$[24509] Sensitivity"
        [JsonProperty("TonePedalRTPCName")]
        public string Key; // "Pedal_RingMod_Sensitivity"
        public string UnitType; // "Number"
        public decimal MinValue; // 0.0
        public decimal MaxValue; // 100.0
        public decimal ValueStep; // 1.0
        public decimal DefaultValue; // 50.0
        public string NifAsset; // ""
        public string ParentLocator; // ""
        public string CameraLocator; // ""
        public int SortOrder; // 1
        public Dictionary<string, string> EnumValues; // {} I have seen both numbers and strings ("Out", "In") as keys

        public ToolkitKnob ToKnob()
        {
            var name = Name;
            if(BadName.IsMatch(name)) {
                var last_ = Key.LastIndexOf('_')+1;
                if(last_ < Key.Length && last_ > 0) {
                    name = Key.Substring(last_);
                }
            }
            return new ToolkitKnob
            {
                DefaultValue = DefaultValue,
                EnumValues = EnumValues.Select(kvp => new Tuple<string, string>(kvp.Key, kvp.Value)).ToList(),
                Key = Key,
                MaxValue = MaxValue,
                MinValue = MinValue,
                Name = name,
                UnitType = UnitType,
                ValueStep = ValueStep
            };
        }
    }
}
