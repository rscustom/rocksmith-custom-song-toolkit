using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithDLCCreator.Manifest
{
    public class Section
    {
        public string Name { get; set; }
        public string UIName { get; set; }
        public int Number { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public int StartPhraseIterationIndex { get; set; }
        public int EndPhraseIterationIndex { get; set; }
        public bool IsSolo { get; set; }
    }
}
