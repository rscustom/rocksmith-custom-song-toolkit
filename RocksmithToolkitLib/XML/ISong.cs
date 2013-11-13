using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

namespace RocksmithToolkitLib.Xml
{
    public interface ISongChordTemplate
    {
        string ChordName { get; set; }
        Int32 Fret0 { get; set; }
        Int32 Fret1 { get; set; }
        Int32 Fret2 { get; set; }
        Int32 Fret3 { get; set; }
        Int32 Fret4 { get; set; }
        Int32 Fret5 { get; set; }
        Int32 Finger0 { get; set; }
        Int32 Finger1 { get; set; }
        Int32 Finger2 { get; set; }
        Int32 Finger3 { get; set; }
        Int32 Finger4 { get; set; }
        Int32 Finger5 { get; set; }
    }
}
