using System.Xml.Serialization;

namespace RocksmithToolkitGUI.ZpeConverter.ZiggyProEditor
{
    //<metaEvents>
    //    <metaEvent startTime="0" startTick="0" metaType="TrackName" text="PART REAL_GUITAR_22" />
    //    <metaEvent startTime="2.364532" startTick="1920" metaType="Text" text="[idle]" />
    //    <metaEvent startTime="4.4334975" startTick="3600" metaType="Text" text="[play]" />
    //    <metaEvent startTime="140.24630425" startTick="113880" metaType="Text" text="[idle]" />
    //    <metaEvent startTime="142.1674865" startTick="115440" metaType="Text" text="[play]" />
    //    <metaEvent startTime="203.940885" startTick="165600" metaType="Text" text="[idle]" />
    //  </metaEvents>

    [XmlType("metaEvent")]
    public class ZpeMetaEvent
    {
        [XmlAttribute("startTime")]
        public float StartTime { get; set; }

        [XmlAttribute("startTick")]
        public float StartTick { get; set; }

        [XmlAttribute("metaType")]
        public string MetaType { get; set; }

        [XmlAttribute("text")]
        public string Text { get; set; }
    }
}