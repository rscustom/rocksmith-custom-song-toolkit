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
        /// based on current installed revision get the latest online revision
        /// <para>server will determine if local revision should be updated</para>
        /// </summary>
        /// <returns></returns>
        public static ToolkitVersionOnline Load(string versionInfoUrl = "")
        {
            // No TLS 1.2 in WinXp, or before IE8 browser if OS is newer than WinXP 
            // Automatic updates do not work in WinXP (Win10 TLS 1.2 must be manually activated)

            var versionInfoJson = String.Empty;
            var toolkitVersionOnline = new ToolkitVersionOnline();

            if (String.IsNullOrEmpty(versionInfoUrl))
                versionInfoUrl = String.Format("{0}/{1}", GetFileUrl(), ToolkitVersion.AssemblyInformationVersion);

            try
            {
                // normal operation
                if (!GeneralExtensions.IsInDesignMode)
                {
                    versionInfoJson = new WebClient().DownloadString(versionInfoUrl);
                    toolkitVersionOnline = JsonConvert.DeserializeObject<ToolkitVersionOnline>(versionInfoJson);

                    //  recommend update to latest revision under special conditions
                    var useBeta = ConfigRepository.Instance().GetBoolean("general_usebeta");

                    if ((!useBeta && ToolkitVersion.AssemblyConfiguration == "BETA") ||
                        (useBeta && ToolkitVersion.AssemblyConfiguration != "BETA") ||
                        String.IsNullOrEmpty(toolkitVersionOnline.Revision))
                    {
                        versionInfoJson = new WebClient().DownloadString(GetFileUrl());
                        toolkitVersionOnline = JsonConvert.DeserializeObject<ToolkitVersionOnline>(versionInfoJson);
                        toolkitVersionOnline.CommitMessages = new string[2];
                        toolkitVersionOnline.CommitMessages[0] = "<WARNING>: Special conditions were detected ...";
                        toolkitVersionOnline.CommitMessages[1] = "Recommend installing the latest online version.";
                        toolkitVersionOnline.UpdateAvailable = true;
                    }
                    else
                    {
                        var installedRevision = ToolkitVersion.AssemblyInformationVersion;
                        // update available
                        if (!installedRevision.Contains(toolkitVersionOnline.Revision))
                            toolkitVersionOnline.UpdateAvailable = true;
                        else
                            toolkitVersionOnline.UpdateAvailable = false;
                    }
                }
                else // dumby JSON data for debugging 
                {
                    versionInfoJson = "{\"version\":\"2.7.1.0\",\"date\":1470934174,\"update\":true,\"commits\":[\"2016-08-11:AppVeyour build failed so recommitting\",\"2016-08-11: Commit for Beta Version 2.7.1.0\"],\"revision\":\"7f8f5233\"}";
                    toolkitVersionOnline = JsonConvert.DeserializeObject<ToolkitVersionOnline>(versionInfoJson);

                    var installedRevision = ToolkitVersion.AssemblyInformationVersion;
                    if (!installedRevision.Contains(toolkitVersionOnline.Revision))
                        toolkitVersionOnline.UpdateAvailable = true;
                    // alt debugging option
                    // toolkitVersionOnline.UpdateAvailable = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("VersionInfoUrl Load Error: " + ex.Message);
            }

            return toolkitVersionOnline;
        }

        public static string GetFileUrl(bool addExtension = false)
        {
            var useBeta = ConfigRepository.Instance().GetBoolean("general_usebeta");
            var lastestReleaseUrl = ConfigRepository.Instance()["general_urllastestrelease"];
            var lastestBetaUrl = ConfigRepository.Instance()["general_urllastestbeta"];

            var fileUrl = useBeta ? lastestBetaUrl : lastestReleaseUrl;

            if (!addExtension)
                return fileUrl;

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