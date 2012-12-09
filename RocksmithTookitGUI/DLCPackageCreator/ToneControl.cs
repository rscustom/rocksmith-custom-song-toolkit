using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib;
using RocksmithToolkitLib.DLCPackage.Tone;
using RocksmithToolkitLib.Tone;
using Pedal = RocksmithToolkitLib.Tone.Pedal;

namespace RocksmithTookitGUI.DLCPackageCreator
{
    public partial class ToneControl : UserControl
    {
        public Tone Tone { get; private set; }

        public ToneControl()
        {
            InitializeComponent();
            var allPedals = GameData.GetPedalData();

            Tone = new Tone();
            Tone.PedalList.Add("Amp", allPedals.First(p => p.Key == "Amp_Fusion").MakePedalSetting());
            Tone.PedalList.Add("Cabinet", allPedals.First(p => p.Key == "Cab_2X12_Fusion_57_Cone").MakePedalSetting());

            InitializeTextBoxes();
            InitializeComboBoxes(allPedals);
            RefreshControls();
        }

        public void RefreshControls()
        {
            toneNameBox.Text = Tone.Name ?? "";
            volumeBox.Value = Tone.Volume;

            UpdateComboSelection(ampBox, "Amp");
            UpdateComboSelection(cabinetBox, "Cabinet");

            UpdateComboSelection(prePedal1Box, "PrePedal1");
            UpdateComboSelection(prePedal2Box, "PrePedal2");
            UpdateComboSelection(prePedal3Box, "PrePedal3");

            UpdateComboSelection(loopPedal1Box, "LoopPedal1");
            UpdateComboSelection(loopPedal2Box, "LoopPedal2");
            UpdateComboSelection(loopPedal3Box, "LoopPedal3");

            UpdateComboSelection(postPedal1Box, "PostPedal1");
            UpdateComboSelection(postPedal2Box, "PostPedal2");
            UpdateComboSelection(postPedal3Box, "PostPedal3");
        }

        private void UpdateComboSelection(ComboBox box, string pedalSlot)
        {
            box.SelectedItem = Tone.PedalList.ContainsKey(pedalSlot)
                ? box.Items.OfType<Pedal>().First(p => p.Key == Tone.PedalList[pedalSlot].PedalKey)
                : null;
        }

        private void InitializeTextBoxes()
        {
            toneNameBox.TextChanged += (sender, e) =>
                Tone.Name = toneNameBox.Text;
            volumeBox.ValueChanged += (sender, e) =>
                Tone.Volume = volumeBox.Value;
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
                var pedal = Tone.PedalList[pedalSlot];
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
                var pedal = box.SelectedItem as Pedal;
                if (pedal == null)
                {
                    Tone.PedalList.Remove(pedalSlot);
                    knobSelectButton.Enabled = false;
                }
                else
                {
                    var pedalSetting = pedal.MakePedalSetting();
                    Tone.PedalList[pedalSlot] = pedalSetting;
                    knobSelectButton.Enabled = pedalSetting.KnobValues.Any();
                }
            };
            UpdateComboSelection(box, pedalSlot);
        }
    }
}

