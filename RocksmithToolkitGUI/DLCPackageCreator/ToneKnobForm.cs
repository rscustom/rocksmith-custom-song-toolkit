using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.ToolkitTone;
using Pedal = RocksmithToolkitLib.DLCPackage.Manifest.Tone.Pedal;
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
                if (knob.Name != null)
                {
                    var name = knob.Name;
                    var niceName = nameParser.Match(name);
                    if(niceName.Groups.Count > 1) {
                        name = niceName.Groups[1].Value;
                    }
                    label.Text = name;
                }
                label.Anchor = AnchorStyles.Left;

                var numericControl = new NumericUpDownFixed();
                tableLayoutPanel.Controls.Add(numericControl, 1, i);
                numericControl.DecimalPlaces = 2;
                numericControl.Minimum = (decimal)knob.MinValue;
                numericControl.Maximum = (decimal)knob.MaxValue;
                numericControl.Increment = (decimal)knob.ValueStep;                
                numericControl.Value = Math.Min((decimal)pedal.KnobValues[knob.Key], numericControl.Maximum);
                numericControl.ValueChanged += (obj, args) =>
                    pedal.KnobValues[knob.Key] = (float)Math.Min(numericControl.Value, numericControl.Maximum);
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
