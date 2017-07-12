using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitLib
{
    public class ToolkitVersionOnline
    {
        [JsonProperty("revision")] // github 4 byte rev/commit
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
            var url = String.Format("{0}/{1}", GetFileUrl(), ToolkitVersion.AssemblyConfiguration);
            Console.WriteLine("Current version url: " + url);

            try
            {
                // No TLS 1.2 in WinXp, or before IE8 browser if OS is newer than WinXP 
                // Automatic updates do not work in WinXP (Win10 TLS 1.2 must be manually activated)
                var versionJson = new WebClient().DownloadString(url);
                // test string for when no internet connection exists
                //var versionJson = "{\"version\":\"2.7.1.0\",\"date\":1470934174,\"update\":true,\"commits\":[\"2016-08-11:AppVeyour build failed so recommitting\",\"2016-08-11: Commit for Beta Version 2.7.1.0\"],\"revision\":\"7f8f5233\"}";
                return JsonConvert.DeserializeObject<ToolkitVersionOnline>(versionJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("versionJson Error: " + ex.Message);
            }

            return null;
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

        // TODO: impliment TLS check
        //public static bool IsTlsCompat(string appName)
        //{
        //    // requires Net 4.5, or Win7 and IE8
        //    // use TLS 1.2 protocol if available
        //    // ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        //    // check system config, some websites e.g. https://www.rscustom.net/ require TSL 1.2 compatible browswer
        //    var errMsg = String.Empty;
        //    var ieVers = SysExtensions.GetBrowserVersion(SysExtensions.GetInternetExplorerVersion());
        //    if (ieVers < 8.0)
        //        errMsg = "Internet Explorer 8 or greater is required";

        //    var sysVers = SysExtensions.MajorVersion + (double)SysExtensions.MinorVersion / 10;
        //    if (sysVers < 6.1)
        //        errMsg = !String.IsNullOrEmpty(errMsg) ? errMsg + Environment.NewLine + "and OS Windows 7 or greater is required" : "OS Windows 7 or greater is required";

        //    if (!String.IsNullOrEmpty(errMsg))
        //    {
        //        errMsg = errMsg + Environment.NewLine + "to download " + appName;
        //        BetterDialog2.ShowDialog(errMsg, "Incompatible System Configuration", null, null, "Ok", Bitmap.FromHicon(SystemIcons.Warning.Handle), "Warning", 150, 150);
        //        return false;
        //    }

        //    return true;
        //}
    }
}