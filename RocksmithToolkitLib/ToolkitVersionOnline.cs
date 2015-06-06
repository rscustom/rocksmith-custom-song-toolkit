using System;
using System.Net;
using Newtonsoft.Json;

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

        [JsonProperty("update")]
        public bool UpdateAvailable { get; set; }

        [JsonProperty("commits")]
        public string[] CommitMessages { get; set; }

        [JsonIgnore]
        public DateTime Date
        {
            get
            {
                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(UnixTimestamp).ToLocalTime();
                return dateTime;
            }
        }
        /// <summary>
        /// Get online version info based on current build
        /// </summary>
        /// <returns></returns>
        public static ToolkitVersionOnline Load()
        {
            var url = String.Format("{0}/{1}", GetFileUrl(), ToolkitVersion.commit);
            var versionJson = new WebClient().DownloadString(url);

            return JsonConvert.DeserializeObject<ToolkitVersionOnline>(versionJson);
        }

        public static string GetFileUrl(bool addExtension = false)
        {
            var lastestBuild = ConfigRepository.Instance().GetBoolean("general_usebeta");
            var lastestReleaseUrl = ConfigRepository.Instance()["general_urllastestrelease"];
            var lastestBuildUrl = ConfigRepository.Instance()["general_urllastestbuild"];

            var fileUrl = lastestBuild ? lastestBuildUrl : lastestReleaseUrl;

            if (!addExtension) return fileUrl;
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return fileUrl + ".tar.gz";
            }
            return fileUrl + ".zip";
        }
    }
}