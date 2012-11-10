using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithDLCCreator
{
    public class SoundBank : ElementFile
    {
        public SoundBank()
        {
            LLID = IdGenerator.LLID();
        }
        public string LLID { get; private set; }
    }
}
