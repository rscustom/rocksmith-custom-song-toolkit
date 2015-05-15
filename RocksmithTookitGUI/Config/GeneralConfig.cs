using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ookii.Dialogs;
using RocksmithToolkitLib;
using RocksmithToolkitLib.Extensions;
using RocksmithToolkitLib.DLCPackage;

namespace RocksmithToolkitGUI.Config
{
    public partial class GeneralConfig : UserControl
    {
        private const string MESSAGEBOX_CAPTION = "General Config";
        private bool loading = false;

        public GeneralConfig()
        {
            InitializeComponent();
            loading = true;
            try
            {
                PopulateAppIdCombo(general_defaultappid_RS2012, GameVersion.RS2012);
                PopulateAppIdCombo(general_defaultappid_RS2014, GameVersion.RS2014);
                PopulateEnumCombo(general_defaultgameversion, typeof(GameVersion));
                PopulateEnumCombo(converter_source, typeof(GamePlatform));
                PopulateEnumCombo(converter_target, typeof(GamePlatform));
                PopulateRampUp();
                PopulateConfigDDC();
                LoadAndSetupConfiguration(this.Controls);
            }
            catch { /*For mono compatibility*/ }
            loading = false;
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
            if (Directory.Exists(@".\ddc\"))
            {
                ddc_config.Items.Clear();
                foreach (var xml in Directory.EnumerateFiles(@".\ddc\", "*.cfg", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileNameWithoutExtension(xml);
                    if (name.StartsWith("user_"))
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
                    value = ((CheckBox)control).Checked.ToString();
                }
                else if (control is NumericUpDown)
                {
                    value = ((NumericUpDown)control).Value.ToString();
                }

                if (ConfigRepository.Instance().ValueChanged(key, value) && !String.IsNullOrEmpty(value))
                    ConfigRepository.Instance()[key] = value;
            }
        }

        private void closeConfigButton_Click(object sender, EventArgs e)
        {
            ((MainForm)ParentForm).ReloadControls();
        }

        private void rs1PathButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new VistaFolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                var rs1Path = fbd.SelectedPath;
                general_rs1path.Text = rs1Path;
                ConfigRepository.Instance()[general_rs1path.Name] = rs1Path;
            }
        }

        private void rs2014PathButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new VistaFolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                var rs2014Path = fbd.SelectedPath;
                general_rs2014path.Text = rs2014Path;
                ConfigRepository.Instance()[general_rs2014path.Name] = rs2014Path;
            }
        }

        private void WwisePathButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new VistaFolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;
                var wwisePath = fbd.SelectedPath;
                general_wwisepath.Text = wwisePath;
                ConfigRepository.Instance()[general_wwisepath.Name] = wwisePath;
            }
        }

    }
}
