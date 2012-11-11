using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithDLCCreator.Tone
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
            var pedal = new Pedal() { PedalKey = "Pedal_EQ7" };
            pedal.KnobValues.Add("Pedal_EQ7_1600", -1);
            pedal.KnobValues.Add("Pedal_EQ7_800", 1);
            pedal.KnobValues.Add("Pedal_EQ7_200", 2);
            pedal.KnobValues.Add("Pedal_EQ7_3200", -3);
            pedal.KnobValues.Add("Pedal_EQ7_100", 1);
            pedal.KnobValues.Add("Pedal_EQ7_400", 1);
            pedal.KnobValues.Add("Pedal_EQ7_6400", 1);
            PedalList.Add("PostPedal3", pedal);
            pedal = new Pedal() { PedalKey = "Cab_4X12_Ranger_Tube_Cone" };
            PedalList.Add("Cabinet", pedal);
            pedal = new Pedal() { PedalKey = "Amp_MegaDuel" };
            pedal.KnobValues.Add("Amp_MegaDuel_Low", 70);
            pedal.KnobValues.Add("Amp_MegaDuel_Mid", 40);
            pedal.KnobValues.Add("Amp_MegaDuel_Hi", 65);
            pedal.KnobValues.Add("Amp_MegaDuel_Gain", 88);
            PedalList.Add("Amp", pedal);
            Volume = -21;
            Description = "$[-1] ";
            IsDLC = true;
            IsPreviewOnlyItem = false;
            UnlockKey = "";
            ExclusiveBuild = new List<object>();
        }
    }
}
