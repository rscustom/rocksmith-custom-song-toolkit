using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithDLCCreator.Tone
{
    public class Pedal
    {
        public string PedalKey { get; set; }
        public Dictionary<string,float> KnobValues { get; set; }
        public Pedal()
        {
            KnobValues = new Dictionary<string, float>();
        }
    }
}
