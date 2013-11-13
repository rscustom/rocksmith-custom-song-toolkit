using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public enum ToneDescriptor {
        [Description("$[35715]BASS")]
        BASS = 35715,
        [Description("$[35716]OVERDRIVE")]
        OVERDRIVE = 35716,
        [Description("$[35719]OCTAVE")]
        OCTAVE = 35719,
        [Description("$[35720]CLEAN")]
        CLEAN = 35720,
        [Description("$[35721]ACOUSTIC")]
        ACOUSTIC = 35721,
        [Description("$[35722]DISTORTION")]
        DISTORTION = 35722,
        [Description("$[35723]CHORUS")]
        CHORUS = 35723,
        [Description("$[35724]LEAD")]
        LEAD = 35724,
        [Description("$[35725]ROTARY")]
        ROTARY = 35725,
        [Description("$[35726]REVERB")]
        REVERB = 35726,
        [Description("$[35727]TREMOLO")]
        TREMOLO = 35727,
        [Description("$[35728]VIBRATO")]
        VIBRATO = 35728,
        [Description("$[35729]FILTER")]
        FILTER = 35729,
        [Description("$[35730]PHASER")]
        PHASER = 35730,
        [Description("$[35731]FLANGER")]
        FLANGER = 35731,
        [Description("$[35732]LOW OUTPUT")]
        LOW_OUTPUT = 35732,
        [Description("$[35734]PROCESSED")]
        PROCESSED = 35734,
        [Description("$[35753]DELAY")]
        DELAY = 35753,
        [Description("$[35754]ECHO")]
        ECHO = 35754,
        [Description("$[35755]HIGH GAIN")]
        HIGH_GAIN = 35755,
        [Description("$[35756]FUZZ")]
        FUZZ = 35756
    }

    public class Tone2014
    {
        public Gear2014 GearList { get; set; }
        public bool IsCustom { get; set; }
        public decimal Volume { get; set; }
        public List<string> ToneDescriptors { get; set; }
        public string Key { get; set; }
        public string NameSeparator { get; set; }
        public string Name { get; set; }
        public decimal SortOrder { get; set; }

        public Tone2014()
        {
            GearList = new Gear2014();
            IsCustom = true;
            Volume = -12;
            ToneDescriptors = new List<string>();
            NameSeparator = " - ";
            SortOrder = 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
