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
        public float Volume { get; set; }

        public Tone()
        {
            PedalList = new Dictionary<string, Pedal>();
            ExclusiveBuild = new List<object>();
            UnlockKey = "";
            IsDLC = true;
            IsPreviewOnlyItem = false;
            Volume = -21;
            Description = "$[-1] ";
        }

        public static Tone CreateDefaultTone()
        {
            var tone = new Tone();

            var postPedal3 = new Pedal { PedalKey = "Pedal_EQ7" };
            postPedal3.KnobValues.Add("Pedal_EQ7_1600", -1);
            postPedal3.KnobValues.Add("Pedal_EQ7_800", 1);
            postPedal3.KnobValues.Add("Pedal_EQ7_200", 2);
            postPedal3.KnobValues.Add("Pedal_EQ7_3200", -3);
            postPedal3.KnobValues.Add("Pedal_EQ7_100", 1);
            postPedal3.KnobValues.Add("Pedal_EQ7_400", 1);
            postPedal3.KnobValues.Add("Pedal_EQ7_6400", 1);
            tone.PedalList.Add("PostPedal3", postPedal3);

            var cabinet = new Pedal() { PedalKey = "Cab_4X12_Ranger_Tube_Cone" };
            tone.PedalList.Add("Cabinet", cabinet);

            var amp = new Pedal() { PedalKey = "Amp_MegaDuel" };
            amp.KnobValues.Add("Amp_MegaDuel_Low", 70);
            amp.KnobValues.Add("Amp_MegaDuel_Mid", 40);
            amp.KnobValues.Add("Amp_MegaDuel_Hi", 65);
            amp.KnobValues.Add("Amp_MegaDuel_Gain", 88);
            tone.PedalList.Add("Amp", amp);

            return tone;
        }
    }
}
