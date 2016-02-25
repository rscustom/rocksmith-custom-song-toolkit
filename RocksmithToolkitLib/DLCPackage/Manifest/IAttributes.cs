using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public interface IAttributes
    {
        List<ChordTemplate> ChordTemplates { get; set; }
        List<Section> Sections { get; set; }
        List<PhraseIteration> PhraseIterations { get; set; }
        List<Phrase> Phrases { get; set; }
        List<float> DynamicVisualDensity { get; set; }
        float Score_MaxNotes { get; set; }
        float Score_PNV { get; set; }
        int TargetScore { get; set; }
    }
}
