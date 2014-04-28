using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Ookii.Dialogs;
using X360.STFS;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.DLCPackage.Manifest;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Sng;
using RocksmithToolkitLib.Ogg;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitGUI.DLCConverter
{
    public partial class DLCConverter : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "DLC Converter";

        public string AppId
        {
            get { return AppIdTB.Text; }
            set { AppIdTB.Text = value; }
        }

        public Platform SourcePlatform {
            get {
                if (platformSourceCombo.Items.Count > 0)
                    return new Platform(platformSourceCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                else
                    return new Platform(GamePlatform.None, GameVersion.None);
            }
        }

        public Platform TargetPlatform {
            get {
                if (platformTargetCombo.Items.Count > 0)
                    return new Platform(platformTargetCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                else
                    return new Platform(GamePlatform.None, GameVersion.None);
            }

        }

        public DLCConverter()
        {
            InitializeComponent();
        }

        private void DLCConverter_Load(object sender, EventArgs e) {
            try {
                // Fill source combo
                
                var sourcePlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
                sourcePlatform.Remove("None");
                platformSourceCombo.DataSource = sourcePlatform;
                platformSourceCombo.SelectedItem = ConfigRepository.Instance()["converter_source"];

                // Fill target combo
                var targetPlatform = Enum.GetNames(typeof(GamePlatform)).ToList<string>();
                targetPlatform.Remove("None");
                platformTargetCombo.DataSource = targetPlatform;
                platformTargetCombo.SelectedItem = ConfigRepository.Instance()["converter_target"];

                // Fill App ID
                PopulateAppIdCombo(GameVersion.RS2014); //Supported game version
                AppIdVisibilty();
            } catch { /*For mono compatibility*/ }
        }

        private void PopulateAppIdCombo(GameVersion gameVersion)
        {
            appIdCombo.Items.Clear();
            foreach (var song in SongAppIdRepository.Instance().Select(gameVersion))
                appIdCombo.Items.Add(song);

            var songAppId = SongAppIdRepository.Instance().Select(ConfigRepository.Instance()["general_defaultappid_RS2014"], gameVersion);
            appIdCombo.SelectedItem = songAppId;
            AppId = songAppId.AppId;
        }

        private void AppIdVisibilty() {
            if (platformTargetCombo.SelectedItem != null)
            {
                var target = new Platform(platformTargetCombo.SelectedItem.ToString(), GameVersion.RS2014.ToString());
                appIdCombo.Enabled = !target.IsConsole;
                AppIdTB.Enabled = !target.IsConsole;
            }
        }

        private void platformTargetCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppIdVisibilty();
        }

        private void appIdCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (appIdCombo.SelectedItem != null)
                AppId = ((SongAppId)appIdCombo.SelectedItem).AppId;
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            // VALIDATIONS
            if (SourcePlatform.Equals(TargetPlatform)) {
                MessageBox.Show("The source and target platform should be different.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // GET FILES
            string[] sourcePackages;

            using (var ofd = new OpenFileDialog()) {
                ofd.Title = "Select one DLC for platform conversion";
                ofd.Multiselect = true;
                switch (SourcePlatform.platform) {
                    case GamePlatform.Pc:
                    case GamePlatform.Mac:
                        ofd.Filter = "PC or Mac Rocksmith 2014 DLC (*.psarc)|*.psarc";
                        break;
                    case GamePlatform.XBox360:
                        ofd.Filter = "XBox 360 Rocksmith 2014 DLC (*.)|*.*";
                        break;
                    case GamePlatform.PS3:
                        ofd.Filter = "PS3 Rocksmith 2014 DLC (*.edat)|*.edat";
                        break;
                    default:
                        MessageBox.Show("The converted audio on Wwise 2013 for target platform should be selected.", MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                sourcePackages = ofd.FileNames;
            }

            // SOURCE            
            StringBuilder errorsFound = new StringBuilder();

            foreach (var sourcePackage in sourcePackages)
            {
                if (!sourcePackage.IsValidPSARC())
                {
                    errorsFound.AppendLine(String.Format("File '{0}' isn't valid. File extension was changed to '.invalid'", Path.GetFileName(sourcePackage)));

                    return;
                }

                var alertMessage = String.Format("Source package '{0}' seems to be not {1} platform, the conversion can't be work.", Path.GetFileName(sourcePackage), SourcePlatform);
                var haveCorrectName = Path.GetFileNameWithoutExtension(sourcePackage).EndsWith(SourcePlatform.GetPathName()[2]);
                if (SourcePlatform.platform == GamePlatform.PS3)
                    haveCorrectName = Path.GetFileNameWithoutExtension(sourcePackage).EndsWith(SourcePlatform.GetPathName()[2] + ".psarc");

                if (!haveCorrectName)
                {
                    errorsFound.AppendLine(alertMessage);
                    if (MessageBox.Show(alertMessage + Environment.NewLine + "Force try to convert this package?", MESSAGEBOX_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        continue;
                }

                // CONVERT
                var output = DLCPackageConverter.Convert(sourcePackage, SourcePlatform, TargetPlatform, AppId);
                if (!String.IsNullOrEmpty(output))
                    errorsFound.AppendLine(output);
            }

            if (errorsFound.Length <= 0)
                MessageBox.Show(String.Format("DLC was converted from '{0}' to '{1}'.", SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show(String.Format("DLC was converted from '{0}' to '{1}' with erros. See below: " + Environment.NewLine + errorsFound.ToString(), SourcePlatform.platform, TargetPlatform.platform), MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
