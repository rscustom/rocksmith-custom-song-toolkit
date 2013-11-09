using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Tone;
using RocksmithToolkitLib.ToolkitTone;
using Pedal = RocksmithToolkitLib.ToolkitTone.ToolkitPedal;
using System.Text.RegularExpressions;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ToneControl : UserControl
    {
        private bool _RefreshingCombos = false;

        private Tone tone;
        public Tone Tone
        {
            set
            {
                tone = value;
                if (value != null)
                    RefreshControls();
            }
            get
            {
                return tone;
            }
        }

        public ToneControl()
        {
            InitializeComponent();
            InitializeTextBoxes();
            InitializeComboBoxes(ToolkitPedal.LoadFromResource());
        }

        public void RefreshControls()
        {
            _RefreshingCombos = true;
            toneNameBox.Text = tone.Name ?? "";
            volumeBox.Value = tone.Volume;

            UpdateComboSelection(ampBox, ampKnobButton, "Amp");
            UpdateComboSelection(cabinetBox, cabinetKnobButton, "Cabinet");

            UpdateComboSelection(prePedal1Box, prePedal1KnobButton, "PrePedal1");
            UpdateComboSelection(prePedal2Box, prePedal2KnobButton, "PrePedal2");
            UpdateComboSelection(prePedal3Box, prePedal3KnobButton, "PrePedal3");

            UpdateComboSelection(loopPedal1Box, loopPedal1KnobButton, "LoopPedal1");
            UpdateComboSelection(loopPedal2Box, loopPedal2KnobButton, "LoopPedal2");
            UpdateComboSelection(loopPedal3Box, loopPedal3KnobButton, "LoopPedal3");

            UpdateComboSelection(postPedal1Box, postPedal1KnobButton, "PostPedal1");
            UpdateComboSelection(postPedal2Box, postPedal2KnobButton, "PostPedal2");
            UpdateComboSelection(postPedal3Box, postPedal3KnobButton, "PostPedal3");
            _RefreshingCombos = false;
        }

        private void UpdateComboSelection(ComboBox box, Control knobSelectButton, string pedalSlot)
        {
            box.SelectedItem = tone.PedalList.ContainsKey(pedalSlot)
                ? box.Items.OfType<Pedal>().First(p => p.Key == tone.PedalList[pedalSlot].PedalKey)
                : null;
            knobSelectButton.Enabled = tone.PedalList.ContainsKey(pedalSlot) ? tone.PedalList[pedalSlot].KnobValues.Any() : false;
        }

        private void InitializeTextBoxes()
        {
            toneNameBox.TextChanged += (sender, e) =>
                tone.Name = toneNameBox.Text;
            volumeBox.ValueChanged += (sender, e) =>
                tone.Volume = volumeBox.Value;
        }

        private void InitializeComboBoxes(IList<Pedal> allPedals)
        {
            var amps = allPedals
                .Where(p => p.TypeEnum == PedalType.Amp)
                .OrderBy(p => p.DisplayName)
                .ToArray();
            var cabinets = allPedals
                .Where(p => p.TypeEnum == PedalType.Cabinet)
                .OrderBy(p => p.DisplayName)
                .ToArray();
            var prePedals = allPedals
                .Where(p => p.TypeEnum == PedalType.Pedal && p.AllowPre)
                .OrderBy(p => p.DisplayName)
                .ToArray();
            var loopPedals = allPedals
                .Where(p => p.TypeEnum == PedalType.Pedal && p.AllowLoop)
                .OrderBy(p => p.DisplayName)
                .ToArray();
            var postPedals = allPedals
                .Where(p => p.TypeEnum == PedalType.Pedal && p.AllowPost)
                .OrderBy(p => p.DisplayName)
                .ToArray();

            InitializePedalSelect(ampBox, ampKnobButton, "Amp", amps, false);
            InitializePedalSelect(cabinetBox, cabinetKnobButton, "Cabinet", cabinets, false);

            InitializePedalSelect(prePedal1Box, prePedal1KnobButton, "PrePedal1", prePedals, true);
            InitializePedalSelect(prePedal2Box, prePedal2KnobButton, "PrePedal2", prePedals, true);
            InitializePedalSelect(prePedal3Box, prePedal3KnobButton, "PrePedal3", prePedals, true);

            InitializePedalSelect(loopPedal1Box, loopPedal1KnobButton, "LoopPedal1", loopPedals, true);
            InitializePedalSelect(loopPedal2Box, loopPedal2KnobButton, "LoopPedal2", loopPedals, true);
            InitializePedalSelect(loopPedal3Box, loopPedal3KnobButton, "LoopPedal3", loopPedals, true);

            InitializePedalSelect(postPedal1Box, postPedal1KnobButton, "PostPedal1", postPedals, true);
            InitializePedalSelect(postPedal2Box, postPedal2KnobButton, "PostPedal2", postPedals, true);
            InitializePedalSelect(postPedal3Box, postPedal3KnobButton, "PostPedal3", postPedals, true);
        }

        private void InitializePedalSelect(ComboBox box, Control knobSelectButton, string pedalSlot, Pedal[] pedals, bool allowNull)
        {
            knobSelectButton.Enabled = false;
            knobSelectButton.Click += (sender, e) =>
            {
                var pedal = tone.PedalList[pedalSlot];
                using (var form = new ToneKnobForm())
                {
                    form.Init(pedal, pedals.Single(p => p.Key == pedal.PedalKey).Knobs);
                    form.ShowDialog();
                }
            };

            box.DisplayMember = "DisplayName";
            if (allowNull)
                box.Items.Add(string.Empty);
            box.Items.AddRange(pedals);
            box.SelectedValueChanged += (sender, e) =>
            {
                if (_RefreshingCombos)
                {
                    return;
                }
                var pedal = box.SelectedItem as Pedal;
                if (pedal == null)
                {
                    tone.PedalList.Remove(pedalSlot);
                    knobSelectButton.Enabled = false;
                }
                else
                {
                    if (pedal.Key != (tone.PedalList.ContainsKey(pedalSlot) ? tone.PedalList[pedalSlot].PedalKey : ""))
                    {
                        var pedalSetting = pedal.MakePedalSetting();
                        tone.PedalList[pedalSlot] = pedalSetting;
                        knobSelectButton.Enabled = pedalSetting.KnobValues.Any();
                    }
                    else {
                        knobSelectButton.Enabled = tone.PedalList[pedalSlot].KnobValues.Any();
                    }
                }
            };
        }

        private void toneNameBox_Leave(object sender, EventArgs e) {
            TextBox control = (TextBox)sender;
            control.Text = GetValidName(control.Text.Trim());
        }

        private string GetValidName(string value) {
            string name = String.Empty;
            if (!String.IsNullOrEmpty(value)) {
                Regex rgx = new Regex("[^a-zA-Z0-9\\- _]");
                name = rgx.Replace(value, "");
            }
            return name;
        }
    }
}

