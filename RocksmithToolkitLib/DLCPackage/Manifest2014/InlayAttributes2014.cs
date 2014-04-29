using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class InlayAttributes2014 {
        private static readonly string URN_TEMPLATE = "urn:{0}:{1}:{2}";
        
        public string DecorativeInlays { get; set; }
        public bool DLC { get; set; }
        public string LocName { get; set; }
        public string ManifestUrn { get; set; }
        public string Name { get; set; }
        public string PreviewArt { get; set; }
        public string PersistentID { get; set; }

        public InlayAttributes2014() {}

        public InlayAttributes2014(DLCPackageData info)
        {
            var dlcName = info.Inlay.DLCSixName;

            DLC = true;
            LocName = Name = info.Name;
            DecorativeInlays = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.GamebryoSceneGraph.GetDescription(), dlcName);
            ManifestUrn = String.Format(URN_TEMPLATE, TagValue.Database.GetDescription(), TagValue.JsonDB.GetDescription(), String.Format("dlc_guitar_{0}", dlcName));
            PreviewArt = String.Format(URN_TEMPLATE, TagValue.Image.GetDescription(), TagValue.DDS.GetDescription(), String.Format("reward_inlay_{0}", dlcName));
            PersistentID = info.Inlay.Id.ToString().Replace("-", "").ToUpper();
        }
    }
}
