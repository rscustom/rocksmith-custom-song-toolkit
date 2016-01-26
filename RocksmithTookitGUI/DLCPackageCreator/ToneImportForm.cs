﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
﻿﻿﻿﻿﻿using RocksmithToolkitLib.DLCPackage.Manifest2014.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
﻿﻿﻿﻿﻿﻿using System.Linq;

namespace RocksmithToolkitGUI.DLCPackageCreator
{
    public partial class ToneImportForm : Form
    {
        public ToneImportForm()
        {
            InitializeComponent();
        }

        ~ToneImportForm()//GC unmanaged memory only, TODO: switch to Dispose() pattern
        {
            Tone2014 = null;
            Tone = null;
        }

        public List<Tone2014> Tone2014;
        public List<Tone> Tone;

        bool isRS2014 {
            get
            {
                return Tone2014 != null && Tone == null;
            }
        }

        private const string MESSAGEBOX_CAPTION = "Tone Import Form";
        private bool toggleSelect = true;
        public void PopList()
        {
            lstToneList.Items.Clear();
            if( isRS2014 )
                foreach(var tone in Tone2014)
                    lstToneList.Items.Add(tone.Name+ " (" +tone.Volume+")");
            else
                foreach(var tone in Tone)
                    lstToneList.Items.Add(tone.Name+ " (" +tone.Volume+")");
        }

        #region Events
        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (lstToneList.SelectedItems.Count > 0)
            {
                //Get Selected tones
                var toneNames = new List<string>();
                foreach (var selectedItem in lstToneList.SelectedItems) {
                    var lstItemArray = selectedItem.ToString().Split(new [] { " " }, StringSplitOptions.None);
                    toneNames.Add(lstItemArray[0]);
                }
                //Replace tones with selected.
                if (isRS2014) {
                    var selected = new List<Tone2014>();
                    foreach( var tName in toneNames )
                        selected.Add(Tone2014.Find(t => t.Name == tName));
                    Tone2014 = selected;
                } else {
                    var selected = new List<Tone>();
                    foreach( var tName in toneNames )
                        selected.Add(Tone.Find(t => t.Name == tName));
                    Tone = selected;
                }
            }
            else
            {
                if (MessageBox.Show("Please select tone(s) to import." + Environment.NewLine +
                    "Click 'Retry' to continue or 'Cancel' to Abort", MESSAGEBOX_CAPTION,
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                    this.DialogResult = DialogResult.Cancel;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lstSongList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (toggleSelect)
                    for (int i = 0; i < lstToneList.Items.Count; i++)
                    {
                        lstToneList.SelectedIndex = i;
                        toggleSelect = false;
                    }
                else
                {
                    lstToneList.SelectedIndex = -1;
                    toggleSelect = true;
                }
            }
        }
        #endregion
    }
}
