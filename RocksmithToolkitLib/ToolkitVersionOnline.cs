using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Reflection;
using System.Net;
using Newtonsoft.Json;
using RocksmithToolkitLib.DLCPackage;

namespace RocksmithToolkitLib 
{ 
    public class ToolkitVersionOnline
    {
        [JsonProperty("revision")]
        public string Revision { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("date")]
        public double UnixTimestamp { get; set; }

        [JsonIgnore]
        public DateTime Date {
            get {
                DateTime dateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(UnixTimestamp).ToLocalTime();
                return dateTime;
            }
        }

        public static bool HasNewVersion() {
            var url = GetFileUrl();

            // GET ONLINE VERSION
            var tvo = new ToolkitVersionOnline();
            var wc = new WebClient();
            var versionJson = wc.DownloadString(url);
            tvo = JsonConvert.DeserializeObject<ToolkitVersionOnline>(versionJson);

            // COMPARE ONLINE AND LOCAL
            if (ToolkitVersion.commit != "nongit")
                if (ToolkitVersion.version != String.Format("{0}-{1}", tvo.Version, tvo.Revision))
                    return true;

            return false;
        }

        public static string GetFileUrl(bool addExtension = false) {
            var lastestBuild = Convert.ToBoolean(ConfigRepository.Instance()["usebeta"]);
            var lastestReleaseUrl = ConfigRepository.Instance()["urllastestrelease"];
            var lastestBuildUrl = ConfigRepository.Instance()["urllastestbuild"];

            var fileUrl = (lastestBuild) ? lastestBuildUrl : lastestReleaseUrl;

            if (addExtension)
                if (Environment.OSVersion.Platform == PlatformID.MacOSX) {
                    fileUrl += ".tar.gz";
                } else {
                    fileUrl += ".zip";
                }

            return fileUrl;
        }
    } 
} 
