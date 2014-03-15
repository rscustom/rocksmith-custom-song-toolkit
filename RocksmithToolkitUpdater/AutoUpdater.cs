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
        private WebClient webClient;
        private Stopwatch sw = new Stopwatch();
        private BackgroundWorker bWorker = new BackgroundWorker();
        string localFile = String.Empty;

        private string workDir {
            get { return Path.GetDirectoryName(Application.ExecutablePath); }
        }

        public AutoUpdater() {
            InitializeComponent();
            //System.Diagnostics.Debugger.Launch();
            UpdateToolkit();
        }

        private void UpdateToolkit() {
            var url = GetFileUrl();
            var downloadUri = new Uri(url);
            localFile = Path.Combine(workDir, Path.GetFileName(downloadUri.LocalPath));
            
            // DOWNLOADING FILE
            DownloadFile(downloadUri, localFile);
        }

        public void DownloadFile(Uri downloadUri, string location) {
            using (webClient = new WebClient()) {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                bWorker.DoWork += new DoWorkEventHandler(UnpackAndUpdate);
                bWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StartToolkitGUI);

                sw.Start();
                webClient.DownloadFileAsync(downloadUri, location);
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            // Update the progressbar percentage
            updateProgress.Value = e.ProgressPercentage;

            // Show current operation and percentage status
            ShowCurrentOperation(String.Format("Downloading new version: {0}%", e.ProgressPercentage));

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
                // Change to marquee
                updateProgress.Style = ProgressBarStyle.Marquee;
                updateProgress.Refresh();
                ShowCurrentOperation("Updating application");
                bWorker.RunWorkerAsync();
            }
        }

        private void UnpackAndUpdate(object sender, DoWorkEventArgs e)
        {
            // UNPACK
            if (!String.IsNullOrEmpty(localFile)) {
                if (File.Exists(localFile)) {
                    var output = Path.Combine(workDir, "temp");
                    ExtractFile(output);
                    MoveFiles(output, workDir); 
                    
                    // DELETE DOWNLOADED FILE
                    try
                    {
                        File.Delete(localFile);
                        localFile = null;
                        if (Directory.Exists(output))
                            Directory.Delete(output, true);
                    }
                    catch { /* Do nothing */ }
                    Thread.Sleep(1000);
                }
            }
        }

        public void ExtractFile(string output) {
            AssemblyCaller.Call(Path.Combine(workDir, "ICSharpCode.SharpZipLib.dll"), "ICSharpCode.SharpZipLib.Zip.FastZip", "ExtractZip", new Type[] { typeof(string), typeof(string), typeof(string) }, new object[] { localFile, output, null });
        }

        private void StartToolkitGUI(object sender, RunWorkerCompletedEventArgs e)
        {
            string toolkitRootPath = Path.GetDirectoryName(Application.ExecutablePath);
            
            Process updaterProcess = new Process();
            updaterProcess.StartInfo.FileName = Path.Combine(toolkitRootPath, "RocksmithToolkitGUI.exe");
            updaterProcess.StartInfo.WorkingDirectory = toolkitRootPath;
            updaterProcess.Start();
            Application.Exit();
        }

        private string GetFileUrl() {
            return (string)AssemblyCaller.Call(Path.Combine(workDir, "RocksmithToolkitLib.dll"), "RocksmithToolkitLib.ToolkitVersionOnline", "GetFileUrl", null, new object[] { true });
        }

        public void MoveFiles(string sourceDir, string destDir)
        {
            var source = new DirectoryInfo(sourceDir);

            var files = source.GetFiles("*.*", SearchOption.AllDirectories);
            var directories = source.GetDirectories();

            // CREATE DEST DIRECTORIES IF NOT EXISTS
            foreach (var dir in directories)
            {
                var od = Path.Combine(destDir, dir.Name);
                if (!Directory.Exists(od))
                    Directory.CreateDirectory(od);
            }

            // UPDATE FILES
            foreach (var file in files)
            {
                var of = Path.Combine(destDir, file.Name);
                File.Copy(file.FullName, of, true);
            }
        }

        private void ShowCurrentOperation(string message) {
            currentOperationLabel.Text = message;
            currentOperationLabel.Refresh();
        }
    }
}
