using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public class PedalRS2014
    {
        public string Type { get; set; }

        private Dictionary<string, decimal> _knobValues;
        public Dictionary<string, decimal> KnobValues {
            get {
                if (_knobValues == null)
                    _knobValues = new Dictionary<string, decimal>();

                return _knobValues;                    
            }
            set { _knobValues = value; }
        }

        public string Key { get; set; }
    }
}
