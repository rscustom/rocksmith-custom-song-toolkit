using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XmlRepository;
using System.Collections.Generic;

namespace RocksmithToolkitLib
{
    public class ToolkitVersionOnline
    {
        [JsonProperty("revision")] // github 4 byte rev/commit
        public string Revision { get; set; }

        [JsonProperty("version")]
        public string OnlineVersion { get; set; }

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
        /// Get the latest online version info
        /// </summary>
        /// <param name="gitSubversion">Get specified version info and force rollback</param>
        /// <returns></returns>
        public static ToolkitVersionOnline GetVersionInfo(string gitSubversion = "") // fe47c38
        {
            // TODO: impliment TLS check (see way below)
            // No TLS 1.2 in WinXp, or before IE8 browser if OS is newer than WinXP 
            // Automatic updates do not work in WinXP (Win10 TLS 1.2 must be manually activated)
            var versionOnline = new ToolkitVersionOnline();
            var versionInfoUrl = String.Empty;
            if (!String.IsNullOrEmpty(gitSubversion))
                versionInfoUrl = String.Format("{0}/{1}", GetFileUrl(), gitSubversion);
            else
                versionInfoUrl = GetFileUrl(); // latest online version (default)

            var versionInfoJson = String.Empty;
            if (!GeneralExtension.IsInDesignMode)
            {
                try
                {
                    versionInfoJson = new WebClient().DownloadString(versionInfoUrl);
                }
                catch (Exception ex)
                {
                    versionOnline.CommitMessages = new string[3];
                    versionOnline.CommitMessages[0] = "<WARNING> Could not retreive online version info ...";
                    versionOnline.CommitMessages[1] = "<WARNING> Check your internet connection ...";
                    versionOnline.CommitMessages[2] = " - " + ex.Message;
                    versionOnline.UpdateAvailable = true;
                    return versionOnline;
                }
            }
            else // use some dumby JSON data for debugging 
                versionInfoJson = "{\"version\":\"2.7.1.0\",\"date\":1470934174,\"update\":true,\"commits\":[\"<README> For Developer Use Only ...\",\"<README> This is dumby data for debugging ...\",\"2016-08-11:AppVeyour build failed so recommitting\",\"2016-08-11: Commit for Beta Version 2.7.1.0\"],\"revision\":\"7f8f5233\"}";

            versionOnline = JsonConvert.DeserializeObject<ToolkitVersionOnline>(versionInfoJson);

            var commitMessagesList = new List<string>();
            if (String.IsNullOrEmpty(gitSubversion))
            {
                // update if git subversions are different
                var versionInstalled = ToolkitVersion.AssemblyInformationVersion;
                if (!versionInstalled.Equals(versionOnline.Revision, StringComparison.OrdinalIgnoreCase))
                {
                    if (!GeneralExtension.IsInDesignMode)
                    {
                        commitMessagesList.Add("<README> Sucessfully retrieved latest online version info  ...");
                        commitMessagesList.Add("<README> An update is ready for download and installation  ...");
                    }

                    versionOnline.UpdateAvailable = true;
                }
                else
                    versionOnline.UpdateAvailable = false;
            }
            else // forced update (custom rollback) to a specific version
            {
                commitMessagesList.Add("<README> This is a custom rollback installation ...");
                versionOnline.UpdateAvailable = true;
            }

            if (versionOnline.CommitMessages != null)
                for (int i = 0; i < versionOnline.CommitMessages.Length; i++)
                    commitMessagesList.Add(versionOnline.CommitMessages[i]);

            versionOnline.CommitMessages = commitMessagesList.ToArray();
            return versionOnline;
        }

        public static string GetFileUrl(bool addExtension = false)
        {
            // now forcing use of latest build (beta) in GeneralConfig
            var useBeta = ConfigRepository.Instance().GetBoolean("general_usebeta");
            var lastestReleaseUrl = ConfigRepository.Instance()["general_urllastestrelease"];
            var lastestBetaUrl = ConfigRepository.Instance()["general_urllastestbeta"];

            var fileUrl = useBeta ? lastestBetaUrl : lastestReleaseUrl;

            if (!addExtension)
                return fileUrl;

            // downloader is currently disabled for Mac Mono
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                return fileUrl + ".tar.gz";

            return fileUrl + ".zip";
        }

        // TODO: impliment TLS check
        //public static bool IsTlsCompat(string appName)
        //{
        //    // requires Net 4.5, or Win7 and IE8
        //    // use TLS 1.2 protocol if available
        //    // ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

        //    // check system config, some websites e.g. https://www.rscustom.net/ require TLS 1.2 compatible browswer
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