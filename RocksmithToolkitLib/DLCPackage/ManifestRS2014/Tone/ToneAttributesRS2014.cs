using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitLib.DLCPackage.Manifest.Tone
{
    public class ToneAttributesRS2014
    {
        [JsonProperty("3DArtAsset")]
        public string _3DArtAsset { get; set; }
        public string AssociatedCabKey { get; set; }
        public string Category { get; set; }
        public string DefaultSkin { get; set; }
        public string Description { get; set; }
        public bool DLC { get; set; }
        public List<EffectRS2014> Effects { get; set; }
        public string Key { get; set; }
        public List<KnobRS2014> Knobs { get; set; }
        public string ManifestUrn { get; set; }
        public string Name { get; set; }
        public string PersonalityColor { get; set; }
        public string PersonalityParticle { get; set; }
        public List<string> Skins { get; set; }
        public List<string> Skins3D { get; set; }
        public string SoundBank { get; set; }
        public string Type { get; set; }
        public string PersistentID { get; set; }
    }
}
