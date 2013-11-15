using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.ToolkitTone;
using Pedal = RocksmithToolkitLib.DLCPackage.Tone.Pedal;
using System.Text.RegularExpressions;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ToneKnobForm : Form
    {
        Regex nameParser = new Regex(@"\$\[\d+\] (.+)");

        public ToneKnobForm()
        {
            InitializeComponent();
        }

        public void Init(dynamic pedal, IList<ToolkitKnob> knobs)
        {
            var sortedKnobs = knobs.OrderBy(k => k.Index).ToList();
            tableLayoutPanel.RowCount = knobs.Count;
            for (var i = 0; i < sortedKnobs.Count; i++)
            {
                var knob = sortedKnobs[i];
                var label = new Label();
                tableLayoutPanel.Controls.Add(label, 0, i);
                var name = knob.Name;
                var niceName = nameParser.Match(name);
                if(niceName != null && niceName.Groups.Count > 1) {
                    name = niceName.Groups[1].Value;
                }
                label.Text = name;
                label.Anchor = AnchorStyles.Left;

                var numericControl = new NumericUpDownFixed();
                tableLayoutPanel.Controls.Add(numericControl, 1, i);
                numericControl.Minimum = knob.MinValue;
                numericControl.Maximum = knob.MaxValue;
                numericControl.Increment = knob.ValueStep;
                numericControl.DecimalPlaces = 1;
                numericControl.Value = pedal.KnobValues[knob.Key];
                numericControl.ValueChanged += (obj, args) =>
                    pedal.KnobValues[knob.Key] = numericControl.Value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    public class NumericUpDownFixed : NumericUpDown
    {
        protected override void OnValidating(CancelEventArgs e)
        {
            // Prevent bug where typing a value bypasses min/max validation
            var fixValidation = Value;
            base.OnValidating(e);
        }
    }
}
