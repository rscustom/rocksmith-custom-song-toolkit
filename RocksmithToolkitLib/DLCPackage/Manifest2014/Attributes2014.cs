using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class Attributes2014 : AttributesHeader2014
    {
        public SongArrangementProperties2014 ArrangementProperties { get; set; }
        public int ArrangementSort { get; set; }
        public int ArrangementType { get; set; }
        public string BlockAsset { get; set; }
        public Dictionary<string, Dictionary<string, object>> Chords { get; set; } //Problem in 3rd sublevel that can be a list or not
        public List<ChordTemplate> ChordTemplates { get; set; }
        public List<float> DynamicVisualDensity { get; set; }
        public string FullName { get; set; }
        public string LastConversionDateTime { get; set; }
        public int MasterID_PS3 { get; set; }
        public int MasterID_Xbox360 { get; set; }
        public int MaxPhraseDifficulty { get; set; }
        public List<PhraseIteration2014> PhraseIterations { get; set; }
        public List<Phrase> Phrases { get; set; }
        public string PreviewBankPath { get; set; }
        public int RelativeDifficulty { get; set; }
        public float Score_MaxNotes { get; set; }
        public float Score_PNV { get; set; }
        public List<Section> Sections { get; set; }
        public string ShowlightsXML { get; set; }
        public string SongAsset { get; set; }
        public float SongAverageTempo { get; set; }
        public string SongBank { get; set; }
        public string SongEvent { get; set; }
        public float SongOffset { get; set; }
        public int SongPartition { get; set; }
        public string SongXml { get; set; }
        public int TargetScore { get; set; }
        public Dictionary<string, Dictionary<string, object>> Techniques { get; set; } //Problem in 3rd sublevel that can be a list or not
        public string Tone_A { get; set; }
        public string Tone_B { get; set; }
        public string Tone_Base { get; set; }
        public string Tone_C { get; set; }
        public string Tone_D { get; set; }
        public string Tone_Multiplayer { get; set; }
        public List<ToneRS2014> Tones { get; set; }
        public string InputEvent { get; set; } //Vocals only

        [JsonIgnore]
        private Dictionary<string, string> SectionUINames { get; set; }

        public Attributes2014(Arrangement arrangement, AggregateGraph2014 aggregateGraph) {
            #region Section Variables
            var section = new Dictionary<string, string>();
            section.Add("fadein", "$[34276] Fade In [1]");
            section.Add("fadeout", "$[34277] Fade Out [1]");
            section.Add("buildup", "$[34278] Buildup [1]");
            section.Add("chorus", "$[34279] Chorus [1]");
            section.Add("hook", "$[34280] Hook [1]");
            section.Add("head", "$[34281] Head [1]");
            section.Add("bridge", "$[34282] Bridge [1]");
            section.Add("breakdown", "$[34284] Breakdown [1]");//Where is 34283 ???
            section.Add("interlude", "$[34285] Interlude [1]");
            section.Add("intro", "$[34286] Intro [1]");
            section.Add("melody", "$[34287] Melody [1]");
            section.Add("modbridge", "$[34288] Modulated Bridge [1]");
            section.Add("modchorus", "$[34289] Modulated Chorus [1]");
            section.Add("modverse", "$[34290] Modulated Verse [1]");
            section.Add("outro", "$[34291] Outro [1]");
            section.Add("postbrdg", "$[34292] Post Bridge [1]");
            section.Add("postchorus", "$[34293] Post Chorus [1]");
            section.Add("postvs", "$[34294] Post Verse [1]");
            section.Add("prebrdg", "$[34295] Pre Bridge [1]");
            section.Add("prechorus", "$[34296] Pre Chorus [1]");
            section.Add("preverse", "$[34297] Pre Verse [1]");
            section.Add("riff", "$[34298] Riff [1]");//Where is 34299 ???
            section.Add("solo", "$[34300] Solo [1]");
            section.Add("transition", "$[34301] Transition [1]");
            section.Add("vamp", "$[34302] Vamp [1]");
            section.Add("variation", "$[34303] Variation [1]");
            section.Add("verse", "$[34304] Verse [1]");
            section.Add("noguitar", "$[6091] No Guitar [1]");
            section.Add("silence", "$[6092] Silence [1]");//Not found in RS2014
            section.Add("ambient", "$[6011] Ambient [1]");//Not found in RS2014
            #endregion
            SectionUINames = section;

            PersistentID = IdGenerator.IdString().ToUpper();
            //TODO: WORKING
        }
    }
}
