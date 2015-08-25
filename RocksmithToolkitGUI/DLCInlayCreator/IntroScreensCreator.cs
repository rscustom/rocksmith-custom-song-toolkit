using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.Extensions;
using SharpConfig;

namespace RocksmithToolkitGUI.DLCInlayCreator
{
    public partial class IntroScreensCreator : UserControl
    {
        private BackgroundWorker bwSaveImages = new BackgroundWorker();

        public IntroScreensCreator()
        {
            InitializeComponent();

            // SaveImages Background Worker
            bwSaveImages.DoWork += new DoWorkEventHandler(SaveImages);
            bwSaveImages.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            bwSaveImages.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ProcessCompleted);
            bwSaveImages.WorkerReportsProgress = true;
        }

        private const string MESSAGEBOX_CAPTION = "Custom Intro Screens";
        // 0 - name, 1 - size, 2- psarc image path, 3 - user image path
        private string[,] imageArray = new string[8, 4];
        private string rsDir;

        private string tmpWorkDir
        {
            get { return Path.Combine(Path.GetTempPath(), "cgm"); }
        }

        private string workDir
        {
            get { return Path.GetDirectoryName(Application.ExecutablePath); }
        }

        public void LoadIntroScreens()
        {
            txtAuthor.DoubleClick += txtAuthor_DoubleClick;
            txtSeqName.DoubleClick += txtSeqName_DoubleClick;
            txtSeqName.Leave += txtSeqName_Leave;

            if (Directory.Exists(ConfigRepository.Instance()["general_rs2014path"]))
                rsDir = ConfigRepository.Instance()["general_rs2014path"];
            else
                rsDir = Path.Combine(workDir, "cgm");

            txtAuthor.Text = ConfigRepository.Instance()["general_defaultauthor"];
            LoadImages(Path.Combine(workDir, "cgm", "current.cis"));
        }

        private void InitImageArray()
        {
            // cache.psarc cache4 images 
            imageArray[0, 0] = "Background Image";
            imageArray[0, 1] = "1280x720";
            imageArray[0, 2] = "gfxassets\\views\\introsequence_i15.dds";
            imageArray[0, 3] = null; // user image
            imageArray[1, 0] = "Ubisoft Logo Image";
            imageArray[1, 1] = "512x512";
            imageArray[1, 2] = "gfxassets\\views\\ubisoft_logo.png.dds";
            imageArray[1, 3] = null; // user image
            imageArray[2, 0] = "Studio Pedals Image";
            imageArray[2, 1] = "1280x720";
            imageArray[2, 2] = "gfxassets\\views\\intro_studio_logos.png.dds";
            imageArray[2, 3] = null; // user image
            imageArray[3, 0] = "Credits (Gibson) Image";
            imageArray[3, 1] = "824x620";
            imageArray[3, 2] = "gfxassets\\views\\introsequence_i11.dds";
            imageArray[3, 3] = null; // user image
            imageArray[4, 0] = "Lightspeed Logo Image";
            imageArray[4, 1] = "1024x256";
            imageArray[4, 2] = "gfxassets\\views\\gamebryo_logo.png.dds";
            imageArray[4, 3] = null; // user image
            // static.psarc images
            imageArray[5, 0] = "Title Background";
            imageArray[5, 1] = "1024x256";
            imageArray[5, 2] = "gfxassets\\views\\intro_sequence_bg.dds";
            imageArray[5, 3] = null; // user image
            imageArray[6, 0] = "Title RS Logo Image";
            imageArray[6, 1] = "1024x256";
            imageArray[6, 2] = "gfxassets\\views\\rocksmith_2014_logo.dds";
            imageArray[6, 3] = null; // user image
            imageArray[7, 0] = "Alt Title RS Logo";
            imageArray[7, 1] = "1024x256";
            imageArray[7, 2] = "gfxassets\\views\\rocksmith_2014_logo_graded.dds";
            imageArray[7, 3] = null; // user image
        }

        private void InitImagePics()
        {
            picBackground.ImageLocation = null;
            lblBackground.Visible = true;
            picCredits.ImageLocation = null;
            lblCredits.Visible = true;
            picUbi.ImageLocation = null;
            lblUbi.Visible = true;
            picLightspeed.ImageLocation = null;
            lblLightspeed.Visible = true;
            picPedals.ImageLocation = null;
            lblPedals.Visible = true;
            picTitle.ImageLocation = null;
            lblTitle.Visible = true;
        }

        private void LoadImages(string cisPath)
        {
            // init image array and clear pics
            InitImageArray();
            InitImagePics();
            if (!File.Exists(cisPath)) return;

            // background worker does not work with this method because it updates the
            // form/controls, so using alternate method to report progress
            const int numSteps = 7; // max number of times progress change is reported
            var step = (int)Math.Round(1.0 / (numSteps + 2) * 100, 0);
            var progress = 10;
            ProcessStarted(progress, "Loading images from Intro Screens Template file ...");

            // create temp folder for unzipping *.cis file
            var tmpUnzipDir = Path.Combine(tmpWorkDir, "cis_unzip");
            if (Directory.Exists(tmpUnzipDir)) DirectoryExtension.SafeDelete(tmpUnzipDir, true);
            if (!Directory.Exists(tmpUnzipDir)) Directory.CreateDirectory(tmpUnzipDir);

            var srcPath = cisPath;
            var destPath = tmpUnzipDir;
            ExternalApps.ExtractZip(srcPath, destPath);

            // Open the setup.smb INI file
            Configuration iniConfig = Configuration.LoadFromFile(Path.Combine(tmpUnzipDir, "setup.smb"));
            txtAuthor.Text = iniConfig["General"]["author"].Value;
            txtSeqName.Text = iniConfig["General"]["seqname"].Value;
            txtAuthor.ForeColor = Color.Black;
            txtSeqName.ForeColor = Color.Black;

            // Convert the dds files to png
            IEnumerable<string> ismList = Directory.EnumerateFiles(tmpUnzipDir, "*.dds");
            foreach (string imagePath in ismList)
            {
                progress += step;
                ProcessStarted(progress, "Converting images to PNG files ... " + progress);

                ExternalApps.Dds2Png(imagePath);
                for (int i = 0; i < imageArray.GetUpperBound(0); i++)
                {
                    if (Path.GetFileName(imageArray[i, 2]) == Path.GetFileName(imagePath))
                    {
                        // Load user image paths and png into picBoxes
                        imageArray[i, 3] = Path.ChangeExtension(imagePath, ".png");
                        switch (Path.GetFileName(imagePath))
                        {
                            case "introsequence_i15.dds":
                                picBackground.ImageLocation = imageArray[i, 3];
                                lblBackground.Visible = false;
                                break;
                            case "introsequence_i11.dds":
                                picCredits.ImageLocation = imageArray[i, 3];
                                lblCredits.Visible = false;
                                break;
                            case "ubisoft_logo.png.dds":
                                picUbi.ImageLocation = imageArray[i, 3];
                                lblUbi.Visible = false;
                                break;
                            case "gamebryo_logo.png.dds":
                                picLightspeed.ImageLocation = imageArray[i, 3];
                                lblLightspeed.Visible = false;
                                break;
                            case "intro_studio_logos.png.dds":
                                picPedals.ImageLocation = imageArray[i, 3];
                                lblPedals.Visible = false;
                                break;
                            case "rocksmith_2014_logo.dds":
                                picTitle.ImageLocation = imageArray[i, 3];
                                lblTitle.Visible = false;
                                break;
                        }
                    }
                }
            }
            ProcessCompleted(null, null);
        }

        private void SaveImages(object sender, DoWorkEventArgs e)
        {
            // method using Background Worker to report progress
            var savePath = e.Argument as string;
            const int numSteps = 7; // max number of times progress change is reported
            var step = (int)Math.Round(1.0 / (numSteps + 2) * 100, 0);
            var progress = 0;

            // Create temp folder for zipping files
            var tmpZipDir = Path.Combine(tmpWorkDir, "cis_zip");
            if (Directory.Exists(tmpZipDir)) DirectoryExtension.SafeDelete(tmpZipDir, true);
            Directory.CreateDirectory(tmpZipDir);

            // convert user png images to dds images
            for (int i = 0; i < imageArray.GetUpperBound(0); i++)
            {
                progress += step;
                bwSaveImages.ReportProgress(progress, "Converting user PNG images ... " + i.ToString());

                if (imageArray[i, 3] != null) // user has specified a replacement image
                {
                    // CRITICAL PATH AND ARGS
                    var srcPath = imageArray[i, 3];
                    var destPath = Path.Combine(tmpZipDir, Path.GetFileName(imageArray[i, 2]));
                    ExternalApps.Png2Dds(srcPath, destPath, ImageHandler.Size2IntX(imageArray[i, 1]), ImageHandler.Size2IntY(imageArray[i, 1]));
                }
            }
            progress += step;
            bwSaveImages.ReportProgress(progress, "Saving Intro Screens Template file ... ");

            // Write ini file setup.smb
            var iniFile = Path.Combine(tmpZipDir, "setup.smb");
            Configuration iniCFG = new Configuration();
            iniCFG["General"].Add(new Setting("author", String.IsNullOrEmpty(txtAuthor.Text) ? "CSC" : txtAuthor.Text));
            iniCFG["General"].Add(new Setting("seqname", String.IsNullOrEmpty(txtSeqName.Text) ? "SystemSaved" : txtSeqName.Text));
            iniCFG["General"].Add(new Setting("cscvers", ToolkitVersion.version));
            iniCFG["General"].Add(new Setting("modified", DateTime.Now.ToShortDateString()));
            iniCFG.Save(iniFile);

            // Zip intro sequence *.cis file
            ExternalApps.InjectZip(tmpZipDir, savePath, false, true);
        }

        private void UpdateCache()
        {
            // unpack cache.psarc 
            const int numSteps = 8; // max number of times ReportProgress will be called
            var step = (int)Math.Round(1.0 / (numSteps + 2) * 100, 0);
            int progress = 0;
            progress += step;
            ProcessStarted(progress, "Unpacking cache file ... ");

            var srcPath = Path.Combine(rsDir, "cache.psarc");
            var destPath = Path.Combine(rsDir, "cache.psarc.org");
            // backup the original cache.psarc and never overwrite it
            if (!File.Exists(destPath)) File.Copy(srcPath, destPath, false);

            var tmpCisDir = Path.Combine(tmpWorkDir, "cis_cache");
            if (Directory.Exists(tmpCisDir)) DirectoryExtension.SafeDelete(tmpCisDir, true);
            // CRITCAL PATH
            Directory.CreateDirectory(Path.Combine(tmpCisDir, "cache4\\gfxassets\\views"));
            destPath = tmpCisDir;
            Packer.Unpack(srcPath, destPath);
            // ExternalApps.UnpackPsarc(srcPath, destPath, DLCInlayCreator.GlobalTitlePlatform);

            // convert user png images to dds images
            for (int i = 0; i < 5; i++)
            {
                progress += step;
                ProcessStarted(progress, "Convertng user PNG images ...");

                if (imageArray[i, 3] != null) // user has specified a replacement image
                {
                    // CRITICAL PATH AND ARGS
                    srcPath = imageArray[i, 3];
                    destPath = Path.Combine(tmpCisDir, "cache4\\gfxassets\\views", Path.GetFileName(imageArray[i, 2]));
                    ExternalApps.Png2Dds(srcPath, destPath, ImageHandler.Size2IntX(imageArray[i, 1]), ImageHandler.Size2IntY(imageArray[i, 1]));
                }
            }

            // update user images to zip file
            progress += step;
            ProcessStarted(progress, "Injecting user images ...");
            // SUPER CRITICAL PATH AND ARGS
            var rootDir = string.Format("cache_{0}", DLCInlayCreator.GlobalTitlePlatform);
            srcPath = Path.Combine(tmpCisDir, "cache4\\gfxassets");
            destPath = Path.Combine(tmpCisDir, rootDir, "cache4.7z");
            ExternalApps.InjectZip(srcPath, destPath, true);

            // repack cache.psarc
            progress += step;
            ProcessStarted(progress, "Repacking cache file ...");
            srcPath = Path.Combine(tmpCisDir, rootDir);
            destPath = Path.Combine(rsDir, "cache.psarc");
            Packer.Pack(srcPath, destPath);
            // ExternalApps.RepackPsarc(srcPath, destPath, DLCInlayCreator.GlobalTitlePlatform);

            ProcessCompleted(null, null);
        }

        private void UpdateStatic()
        {
            // unpack static.psarc
            const int numSteps = 5; // max number of times ReportProgress will be called
            var step = (int)Math.Round(1.0 / (numSteps + 2) * 100, 0);
            var progress = 0;
            progress += step;
            ProcessStarted(progress, "Unpacking static file ... ");

            var srcPath = Path.Combine(rsDir, "static.psarc");
            var destPath = Path.Combine(rsDir, "static.psarc.org");
            // backup the original static.psarc and never overwrite it
            if (!File.Exists(destPath)) File.Copy(srcPath, destPath, false);

            var tmpModDir = Path.Combine(tmpWorkDir, "cis_static");
            if (Directory.Exists(tmpModDir)) DirectoryExtension.SafeDelete(tmpModDir, true);
            Directory.CreateDirectory(tmpModDir);

            destPath = tmpModDir;
            // Packer.Unpack(srcPath, destPath); // not working here
            ExternalApps.UnpackPsarc(srcPath, destPath, DLCInlayCreator.GlobalTitlePlatform);

            // convert user png images to dds images
            progress += step * 3;
            ProcessStarted(progress, "Convertng user PNG images ...");

            // CRITICAL PATH AND ARGS
            var rootDir = string.Format("static_{0}", DLCInlayCreator.GlobalTitlePlatform);
            srcPath = imageArray[6, 3];
            destPath = Path.Combine(tmpModDir, rootDir, "gfxassets\\views", Path.GetFileName(imageArray[6, 2]));
            ExternalApps.Png2Dds(srcPath, destPath, ImageHandler.Size2IntX(imageArray[6, 1]), ImageHandler.Size2IntY(imageArray[6, 1]));

            // repack static.psarc
            progress += step;
            ProcessStarted(progress, "Repacking static file ...");
            srcPath = Path.Combine(tmpModDir, rootDir);
            destPath = Path.Combine(rsDir, "static.psarc");
            // Packer.Pack(srcPath, destPath);  // not working
            ExternalApps.RepackPsarc(srcPath, destPath, DLCInlayCreator.GlobalTitlePlatform);

            ProcessCompleted(null, null);
        }

        private bool UserChangedCache()
        {
            for (int i = 0; i < 5; i++)
            {
                if (imageArray[i, 3] != null) return true;
            }
            return false;
        }

        private bool UserChangedStatic()
        {
            return imageArray[6, 3] != null;
        }

        private string picClick(string imageName, string imageSize)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a " + imageSize + " (PNG Image) to replace the " + imageName;
                ofd.Filter = "PNG files|*.png";
                ofd.FilterIndex = 1;
                // ofd.InitialDirectory = Path.Combine(workDir, "cgm");
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return null;
                return ofd.FileName;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // modify intro screens
            if (!UserChangedCache() && !UserChangedStatic()) return;
            if (DLCInlayCreator.GlobalTitlePlatform == null)
            {
                MessageBox.Show("Please select platform.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(Path.Combine(rsDir, "cache.psarc")) || !File.Exists(Path.Combine(rsDir, "static.psarc")))
            {
                MessageBox.Show("Please select the Rocksmith 2014 root folder location in the" + Environment.NewLine +
                    "Configuration toolbar that contains the cache.psarc and static.psarc files." + Environment.NewLine +
                "Click on the File toolbar and Restart for your configuration to take effect.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // create user intro screen sequence
            if (MessageBox.Show("Please be patient ..." + Environment.NewLine + "This process can take up to 3 minutes to complete.", MESSAGEBOX_CAPTION, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                return;
            if (UserChangedCache()) UpdateCache();
            if (UserChangedStatic()) UpdateStatic();

            if (File.Exists(Path.Combine(workDir, "cgm", "current.cis")))
                File.Delete(Path.Combine(workDir, "cgm", "current.cis"));

            // Start SaveImages BackgroundWorker
            ProcessStarted();
            if (!bwSaveImages.IsBusy)
                bwSaveImages.RunWorkerAsync(Path.Combine(workDir, "cgm", "current.cis"));
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string cisPath = String.Empty;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a Custom Intro Screens template";
                ofd.Filter = "CIS file (*.cis)|*.cis";
                ofd.FilterIndex = 1;
                ofd.InitialDirectory = Path.Combine(workDir, "cgm");
                ofd.CheckPathExists = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;
                cisPath = ofd.FileName;
            }
            LoadImages(cisPath);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning this will remove any user image modifications and restore all originals.\n" + Environment.NewLine + "\rAre you sure you want to proceed?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            // restore the original cache.psarc
            ProcessStarted(20, "Restoring original cache file ...");
            var srcPath = Path.Combine(rsDir, "cache.psarc");
            var destPath = Path.Combine(rsDir, "cache.psarc.org");

            if (!File.Exists(destPath))
                MessageBox.Show("Can't find cache.psarc.org file.", MESSAGEBOX_CAPTION + " ... Oops", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                File.Copy(destPath, srcPath, true);

            // restore the original static.psarc
            ProcessStarted(60, "Restoring original static file ...");
            srcPath = Path.Combine(rsDir, "static.psarc");
            destPath = Path.Combine(rsDir, "static.psarc.org");

            if (!File.Exists(destPath))
                MessageBox.Show("Can't find static.psarc.org file.", MESSAGEBOX_CAPTION + " ... Oops", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                File.Copy(destPath, srcPath, true);

            if (File.Exists(Path.Combine(workDir, "cgm", "current.cis")))
                File.Delete(Path.Combine(workDir, "cgm", "current.cis"));

            InitImageArray();
            InitImagePics();
            Refresh();
            ProcessCompleted(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!UserChangedCache() & !UserChangedStatic()) return;

            var savePath = String.Empty;
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a location to store the custom intro sequence template (*.cis) file.";
                fbd.SelectedPath = Path.Combine(workDir, "cgm");
                if (fbd.ShowDialog() != DialogResult.OK) return;
                // name the new *.cis file here
                var SeqName = String.IsNullOrEmpty(txtSeqName.Text) ? "SystemSaved" : txtSeqName.Text;
                var AuthorName = String.IsNullOrEmpty(txtAuthor.Text) ? "CSC" : txtAuthor.Text;
                savePath = Path.Combine(fbd.SelectedPath, SeqName.Replace(" ", "_") + "_" + AuthorName.Replace(" ", "_") + ".cis");
            }

            // check if *.cis file already exists
            if (File.Exists(savePath))
            {
                if (MessageBox.Show(Path.GetFileNameWithoutExtension(savePath) + " already exists at that location." + Environment.NewLine + "Do you want to overwrite it?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) return;
                File.Delete(savePath);
            }

            // Start SaveImages BackgroundWorker
            ProcessStarted();
            if (!bwSaveImages.IsBusy)
                bwSaveImages.RunWorkerAsync(savePath);
        }

        private void lblBackground_Click(object sender, EventArgs e)
        {
            picBackground_Click(null, null);
        }

        private void lblCredits_Click(object sender, EventArgs e)
        {
            picCredits_Click(null, null);
        }

        private void lblLightspeed_Click(object sender, EventArgs e)
        {
            picLightspeed_Click(null, null);
        }

        private void lblPedals_Click(object sender, EventArgs e)
        {
            picPedals_Click(null, null);
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            picTitle_Click(null, null);
        }

        private void lblUbi_Click(object sender, EventArgs e)
        {
            picUbi_Click(null, null);
        }

        private void picBackground_Click(object sender, EventArgs e)
        {
            imageArray[0, 3] = picClick(imageArray[0, 0], imageArray[0, 1]);
            picBackground.Image = ImageHandler.PreSizeImage(imageArray[0, 3], imageArray[0, 1]);
            lblBackground.Visible = imageArray[0, 3] == null;

            // upadate background image in other images
            picUbi.BackgroundImage = picBackground.Image;
            picPedals.BackgroundImage = picBackground.Image;
            picCredits.BackgroundImage = picBackground.Image;
            picLightspeed.BackgroundImage = picBackground.Image;
            picTitle.BackgroundImage = picBackground.Image;
        }

        private void picCredits_Click(object sender, EventArgs e)
        {
            imageArray[3, 3] = picClick(imageArray[3, 0], imageArray[3, 1]);
            picCredits.Image = ImageHandler.PreSizeImage(imageArray[3, 3], imageArray[3, 1]);
            lblCredits.Visible = imageArray[3, 3] == null;
        }

        private void picLightspeed_Click(object sender, EventArgs e)
        {
            imageArray[4, 3] = picClick(imageArray[4, 0], imageArray[4, 1]);
            picLightspeed.Image = ImageHandler.PreSizeImage(imageArray[4, 3], imageArray[4, 1]);
            lblLightspeed.Visible = imageArray[4, 3] == null;
        }

        private void picPedals_Click(object sender, EventArgs e)
        {
            imageArray[2, 3] = picClick(imageArray[2, 0], imageArray[2, 1]);
            picPedals.Image = ImageHandler.PreSizeImage(imageArray[2, 3], imageArray[2, 1]);
            lblPedals.Visible = imageArray[2, 3] == null;
        }

        private void picTitle_Click(object sender, EventArgs e)
        {
            imageArray[6, 3] = picClick(imageArray[6, 0], imageArray[6, 1]);
            picTitle.Image = ImageHandler.PreSizeImage(imageArray[6, 3], imageArray[6, 1]);
            lblTitle.Visible = imageArray[6, 3] == null;
        }

        private void picUbi_Click(object sender, EventArgs e)
        {
            imageArray[1, 3] = picClick(imageArray[1, 0], imageArray[1, 1]);
            picUbi.Image = ImageHandler.PreSizeImage(imageArray[1, 3], imageArray[1, 1]);
            lblUbi.Visible = imageArray[1, 3] == null;
        }

        private void txtAuthor_DoubleClick(object sender, EventArgs e)
        {
            // allow user input
            txtAuthor.ForeColor = Color.Black;
            txtAuthor.SelectAll();
        }

        private void txtSeqName_Leave(object sender, EventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Text = textbox.Text.Trim().GetValidName(false, false, false, false);
        }

        private void txtSeqName_DoubleClick(object sender, EventArgs e)
        {
            // allow user input 
            txtSeqName.ForeColor = Color.Black;
            txtSeqName.SelectAll();
        }

        private void helpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // inlay creator help link
            string link = "http://goo.gl/pJxMuz";
            Process.Start(link);
        }

        public void IntroScreensCreator_Dispose(object sender, EventArgs e)
        {
            if (Directory.Exists(tmpWorkDir))
                DirectoryExtension.SafeDelete(tmpWorkDir);
        }

        ////
        //// Background worker and progress bar methods follow
        ////

        private void ProcessStarted(int progress = 0, string process = "Starting process ...")
        {
            Application.DoEvents();
            Cursor.Current = Cursors.WaitCursor;
            pbarStatus.Style = ProgressBarStyle.Continuous;
            pbarStatus.Value = progress;
            pbarStatus.Visible = lblStatus.Visible = true;
            pbarStatus.Refresh();
            UpdateLblStatus(process);
            btnGenerate.Enabled = false;
            btnLoad.Enabled = false;
            btnRestore.Enabled = false;
            btnSave.Enabled = false;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= pbarStatus.Maximum)
                pbarStatus.Value = e.ProgressPercentage;
            else
                pbarStatus.Value = pbarStatus.Maximum;

            UpdateLblStatus(e.UserState as string);
        }

        private void UpdateLblStatus(string message)
        {
            lblStatus.Text = message;
            lblStatus.Refresh();
        }

        private void ProcessCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.DoEvents();
            pbarStatus.Value = pbarStatus.Maximum;
            UpdateLblStatus("All done ...");
            Thread.Sleep(1500); // dispaly message
            pbarStatus.Visible = lblStatus.Visible = false;
            btnGenerate.Enabled = true;
            btnLoad.Enabled = true;
            btnRestore.Enabled = true;
            btnSave.Enabled = true;
            Cursor.Current = Cursors.Default;

        }

    }
}
