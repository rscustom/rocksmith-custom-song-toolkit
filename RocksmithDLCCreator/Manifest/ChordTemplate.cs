using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithDLCCreator.Manifest
{
    public class ChordTemplate
    {
        public int ChordId { get; set; }
        public string ChordName { get; set; }
        public List<int> Fingers { get; set; }
        public List<int> Frets { get; set; }
    }
}
