using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RocksmithToolkitUpdater
{
    public partial class AutoUpdater : Form
    {
        WebClient webClient;
        Stopwatch sw = new Stopwatch();
        string localFile = String.Empty;

        public AutoUpdater() {
            InitializeComponent();
            UpdateToolkit();
        }

        private void UpdateToolkit() {
            var url = GetFileUrl();
            var downloadUri = new Uri(url);
            localFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileName(downloadUri.LocalPath));
            
            // DOWNLOADING FILE
            DownloadFile(downloadUri, localFile);
        }

        public void DownloadFile(Uri downloadUri, string location) {
            using (webClient = new WebClient()) {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                sw.Start();
                webClient.DownloadFileAsync(downloadUri, location);
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            // Update the progressbar percentage
            updateProgress.Value = e.ProgressPercentage;

            // Show current operation and percentage status
            currentOperationLabel.Text = String.Format("Downloading new version: {0}%", e.ProgressPercentage);

            // Calculate download speed
            labelSpeed.Text = String.Format("Speed: {0:0.00} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds));

            // Data have been downloaded so far and the total size of the file we are currently downloading
            labelDownloaded.Text = String.Format("Downloaded: {0:0.00} MB's / Total: {1:0.00} MB's", (e.BytesReceived / 1024d / 1024d), (e.TotalBytesToReceive / 1024d / 1024d));
        }

        private void Completed(object sender, AsyncCompletedEventArgs e) {
            // Reset the stopwatch.
            sw.Reset();

            if (e.Cancelled == true) {
                MessageBox.Show("Download has been canceled.");
            } else {
                UnpackAndUpdate();
            }
        }

        private void UnpackAndUpdate() {
            updateProgress.Style = ProgressBarStyle.Continuous;
            updateProgress.Refresh();
            
            // UNPACK
            if (!String.IsNullOrEmpty(localFile)) {
                if (File.Exists(localFile)) {
                    currentOperationLabel.Text = "Extracting downloaded file.";
                    ExtractFile();
                    Thread.Sleep(1000);
                    
                    // DELETE DOWNLOADED FILE
                    currentOperationLabel.Text = "Deleting temp files.";
                    File.Delete(localFile);
                    localFile = null;
                }
            }

            Application.Exit();
        }

        public void ExtractFile() {
            var output = Path.GetDirectoryName(Application.ExecutablePath);
            AssemblyCaller.Call(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ICSharpCode.SharpZipLib.dll"), "ICSharpCode.SharpZipLib.Zip.FastZip", "ExtractZip", new Type[] { typeof(string), typeof(string), typeof(string) }, new object[] { localFile, output, null });
        }

        private void AutoUpdater_FormClosing(object sender, FormClosingEventArgs e) {
            // OPEN TOOLKIT AND EXIT AUTO UPDATER
            StartToolkitGUI();
        }

        private void StartToolkitGUI() {
            string toolkitRootPath = Path.GetDirectoryName(Application.ExecutablePath);
            
            Process updaterProcess = new Process();
            updaterProcess.StartInfo.FileName = Path.Combine(toolkitRootPath, "RocksmithToolkitGUI.exe");
            updaterProcess.StartInfo.WorkingDirectory = toolkitRootPath;
            updaterProcess.Start();
        }

        private string GetFileUrl() {
            return (string)AssemblyCaller.Call(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "RocksmithToolkitLib.dll"), "RocksmithToolkitLib.ToolkitVersionOnline", "GetFileUrl", null, new object[] { true });
        }
    }
}
