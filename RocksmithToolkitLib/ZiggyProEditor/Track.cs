using System.Collections.Generic;
using System.Xml.Serialization;

namespace RocksmithToolkitLib.ZiggyProEditor
{
    // <track name="PART REAL_GUITAR">

    [XmlRoot("track")]
    public class ZpeTrack
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray("chords")]
        [XmlArrayItem("chord")]
        public List<ZpeChord> Chords { get; set; }

        // no parallel Midi data for this so leave it null (no serializitation element)
        //<modifiers>
        //  <modifier type="Powerup" startTime="7.093596" startTick="5760" endTime="9.458128" endTick="7680" />
        //  <modifier type="Powerup" startTime="176.748767" startTick="143520" endTime="179.704432" endTick="145920" />
        //</modifiers>

        [XmlArray("tempos")]
        [XmlArrayItem("tempo")]
        public List<ZpeTempo> Tempos { get; set; }

        [XmlArray("timeSignatures")]
        [XmlArrayItem("timeSignature")]
        public List<ZpeTimeSignature> TimeSignatures { get; set; }

        [XmlArray("metaEvents")]
        [XmlArrayItem("metaEvent")]
        public List<ZpeMetaEvent> MetaEvents { get; set; }
    }
}
