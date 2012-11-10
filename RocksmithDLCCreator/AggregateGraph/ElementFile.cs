﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocksmithDLCCreator
{
    public class ElementFile : Element
    {
        public string File { get; set; }
        public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(File); } }
    }
}