using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.XML;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.ToolkitTone;
using System.Text.RegularExpressions;
using RocksmithToolkitLib;

public class RsToneRS2014
{
    [JsonProperty("3DArtAsset")]
    public string _3DArtAsset { get; set; }
    public string AssociatedCabKey { get; set; }
    public string Category { get; set; }
    public string DefaultSkin { get; set; }
    public string Description { get; set; }
    public bool DLC { get; set; }
    public List<Rs2014Effect> Effects { get; set; }
    public string Key { get; set; }
    public List<Rs2014Knob> Knobs { get; set; }
    public string ManifestUrn { get; set; }
    public string Name { get; set; }
    public string PersonalityColor { get; set; }
    public string PersonalityParticle { get; set; }
    public List<string> Skins { get; set; }
    public List<string> Skins3D { get; set; }
    public string SoundBank { get; set; }
    public string SpawnPoint { get; set; }
    public string Type { get; set; }
    public string PersistentID { get; set; }

    public RsToneRS2014() {
        Knobs = new List<Rs2014Knob>();
    }

    public ToolkitPedal ToPedal()
    {
        var tkPedal = new ToolkitPedal();
        tkPedal.Category = Category;
        tkPedal.Key = Key;
        if (Knobs != null)
            tkPedal.Knobs = (from k in Knobs
                             where k.Name != null
                             select k).Select(knob => knob.ToToolkitKnob()).ToList();
        tkPedal.Name = Name;
        tkPedal.Type = Type;
        tkPedal.Bass = Key.Contains("Bass");
        return tkPedal;
    }
}

public class Rs2014Effect
{
    public string Name { get; set; }
    public string Name_MacOS { get; set; }
}

public class Rs2014Knob
{
    public string RTPC { get; set; }
    public string Base { get; set; }
    public int Type { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public string Name { get; set; }
    public string UnitType { get; set; }
    public float DefaultValue { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public float ValueStep { get; set; }
    public string Ring { get; set; }
    public int Index { get; set; }
    public Dictionary<string, string> EnumValues { get; set; }

    public ToolkitKnob ToToolkitKnob()
    {
        var tKnob = new ToolkitKnob();
        tKnob.DefaultValue = DefaultValue;
        if (EnumValues != null)
            tKnob.EnumValues = EnumValues.Select(kvp => new Tuple<string, string>(kvp.Key, kvp.Value)).ToList();
        tKnob.Key = RTPC;
        tKnob.MaxValue = MaxValue;
        tKnob.MinValue = MinValue;
        tKnob.Name = Name;
        tKnob.UnitType = UnitType;
        tKnob.ValueStep = ValueStep;
        tKnob.Index = Index;
        return tKnob;
    }
}
