using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Ookii.Dialogs;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.XmlRepository;

namespace RocksmithToolkitGUI.Config
{
    public partial class GeneralConfig : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "General Config";
        private bool loading = false;

        public GeneralConfig()
        {
            InitializeComponent();
            general_defaultauthor.Validating += ValidateSortName;

            loading = true;
            try
            {
                PopulateAppIdCombo(general_defaultappid_RS2012, GameVersion.RS2012);
                PopulateAppIdCombo(general_defaultappid_RS2014, GameVersion.RS2014);
                PopulateEnumCombo(general_defaultgameversion, typeof(GameVersion));
                PopulateEnumCombo(general_defaultplatform, typeof(GamePlatform));
                PopulateEnumCombo(converter_source, typeof(GamePlatform));
                PopulateEnumCombo(converter_target, typeof(GamePlatform));
                PopulateRampUp();
                PopulateConfigDDC();
                LoadAndSetupConfiguration(this.Controls);
                // hide First Run
                if (ConfigRepository.Instance()["general_firstrun"] == "false")
                    lblFirstRun.Visible = false;
            }
            catch { /*For mono compatibility*/ }
            loading = false;
        }

        private void ValidateSortName(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
                tb.Text = tb.Text.Trim().GetValidSortableName();
        }

        private void LoadAndSetupConfiguration(ControlCollection controls)
        {
            foreach (var control in controls)
            {
                if (control is TextBox || control is CueTextBox)
                {
                    var tb = (TextBox)control;
                    tb.Text = ConfigRepository.Instance()[tb.Name];
                }
                else if (control is ComboBox)
                {
                    var cb = (ComboBox)control;
                    var value = ConfigRepository.Instance()[cb.Name];
                    if (!String.IsNullOrEmpty(cb.ValueMember))
                        cb.SelectedValue = value;
                    else
                        cb.SelectedItem = value;
                }
                else if (control is CheckBox)
                {
                    var ch = (CheckBox)control;
                    ch.Checked = ConfigRepository.Instance().GetBoolean(ch.Name);
                }
                else if (control is NumericUpDown)
                {
                    var nud = (NumericUpDown)control;
                    nud.Value = ConfigRepository.Instance().GetDecimal(nud.Name);
                }
                else if (control is GroupBox)
                    LoadAndSetupConfiguration(((GroupBox)control).Controls);
            }
        }

        private void PopulateAppIdCombo(ComboBox combo, GameVersion gameVersion)
        {
            var appIdList = SongAppIdRepository.Instance().Select(gameVersion).ToArray();
            combo.DataSource = appIdList;
            combo.DisplayMember = "DisplayName";
            combo.ValueMember = "AppId";
        }

        private void PopulateEnumCombo(ComboBox combo, Type typeEnum)
        {
            var enumList = Enum.GetNames(typeEnum).ToList<string>();
            enumList.Remove("None");
            combo.DataSource = enumList;
        }

        private void PopulateRampUp()
        {
            if (Directory.Exists(@".\ddc\"))
            {
                ddc_rampup.Items.Clear();
                foreach (var xml in Directory.EnumerateFiles(@".\ddc\", "*.xml", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileNameWithoutExtension(xml);
                    if (name.StartsWith("user_"))
                        name = name.Remove(0, 5);
                    ddc_rampup.Items.Add(name);
                    ddc_rampup.SelectedItem = ConfigRepository.Instance()[ddc_rampup.Name];
                }
            }
        }

        private void PopulateConfigDDC()
        {
            var ddcpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "ddc");
            if (Directory.Exists(ddcpath))
            {
                ddc_config.Items.Clear();
                foreach (var xml in Directory.EnumerateFiles(ddcpath, "*.cfg", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileNameWithoutExtension(xml);
                    if (name.StartsWith("user_", StringComparison.Ordinal))
                        name = name.Remove(0, 5);
                    ddc_config.Items.Add(name);
                    ddc_config.SelectedItem = ConfigRepository.Instance()[ddc_config.Name];
                }
            }
        }

        private void ConfigurationChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                Control control = (Control)sender;
                var key = control.Name;
                var value = control.Text;

                if (control is ComboBox)
                {
                    var combo = ((ComboBox)control);
                    if (!String.IsNullOrEmpty(combo.ValueMember))
                        value = combo.SelectedValue.ToString();
                    else
                        value = combo.SelectedItem.ToString();
                }
                else if (control is CheckBox)
                {
                    value = ((CheckBox)control).Checked.ToString().ToLower();
                }
                else if (control is NumericUpDown)
                {
                    value = ((NumericUpDown)control).Value.ToString(CultureInfo.InvariantCulture);
                }

                if (ConfigRepository.Instance().ValueChanged(key, value) && !String.IsNullOrEmpty(value))
                    ConfigRepository.Instance()[key] = value;
            }
        }

        private void closeConfigButton_Click(object sender, EventArgs e)
        {
            ConfigRepository.Instance()["general_firstrun"] = "false";
            ((MainForm)ParentForm).ReloadControls();
        }

        private void btnRs1Path_Click(object sender, EventArgs e)
        {
            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.SelectedPath = general_rs1path.Name;
                fbd.Description = "Select Rocksmith 2012 executable root installation folder.";

                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                var rs1Path = fbd.SelectedPath;
                general_rs1path.Text = rs1Path;
                ConfigRepository.Instance()[general_rs1path.Name] = rs1Path;
            }
        }

        private void btnRs2014Path_Click(object sender, EventArgs e)
        {
            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.SelectedPath = general_rs2014path.Name;
                fbd.Description = "Select Rocksmith 2014 executable root installation folder.";

                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                var rs2014Path = fbd.SelectedPath;
                general_rs2014path.Text = rs2014Path;
                ConfigRepository.Instance()[general_rs2014path.Name] = rs2014Path;
            }
        }

        private void btnWwisePath_Click(object sender, EventArgs e)
        {
            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.SelectedPath = general_wwisepath.Name;
                fbd.Description = "Select Wwise CLI (*.exe) installation folder.";

                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                var wwisePath = fbd.SelectedPath;
                general_wwisepath.Text = wwisePath;
                ConfigRepository.Instance()[general_wwisepath.Name] = wwisePath;
            }
        }

        private void btnProjectDir_Click(object sender, EventArgs e)
        {
            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.SelectedPath = creator_defaultproject.Name;
                fbd.Description = "Select Default Project Folder for the CDLC Creator";
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                var projectDir = fbd.SelectedPath;
                creator_defaultproject.Text = projectDir;
                ConfigRepository.Instance()[creator_defaultproject.Name] = projectDir;
            }
        }

        private void btnTonePath_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = creator_defaulttone.Name;
                ofd.Title = "Select Default Tone for the CDLC Creator";
                ofd.Filter = CurrentOFDFilter;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                var tonePath = ofd.FileName;
                creator_defaulttone.Text = tonePath;
                ConfigRepository.Instance()[creator_defaulttone.Name] = tonePath;
            }
        }

        private string CurrentOFDFilter
        {
            get
            {
                switch (general_defaultgameversion.SelectedItem.ToString())
                {
                    case "RS2014":
                        return "Rocksmith 2014 Tone Template(*.tone2014.xml)|*.tone2014.xml|All XML Files (*.xml)|*.xml";
                    default:
                        return "Rocksmith Tone Template (*.tone.xml)|*.tone.xml|All XML Files (*.xml)|*.xml";
                }
            }
        }

    }
}
