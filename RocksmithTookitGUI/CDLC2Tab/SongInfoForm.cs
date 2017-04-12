using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RocksmithToTabLib;
using RocksmithToolkitLib.Conversion;

namespace RocksmithToolkitGUI.CDLC2Tab
{
    public partial class SongInfoForm : Form
    {
        public SongInfoForm()
        {
            InitializeComponent();
        }

        public IList<SongInfoShort> SongListShort;

        private const string MESSAGEBOX_CAPTION = "SongInfo List Form";
        private bool toggleSelect = true;

        public void PopSongInfo(IList<SongInfo> songList)
        {
            lstSongList.Items.Clear();
            foreach (var song in songList)
                foreach (var arrangement in song.Arrangements)
                    lstSongList.Items.Add(song.Artist + " - " + song.Title + " - " + arrangement + " - " +
                                          song.Identifier);
        }

        public void PopSongOnly(IList<SongInfo> songList)
        {
            lstSongList.Items.Clear();
            foreach (var song in songList)
                lstSongList.Items.Add(song.Artist + " - " + song.Title + " - _ - " +
                                     song.Identifier);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            SongListShort = new List<SongInfoShort>();

            if (lstSongList.SelectedItems.Count > 0)
            {
                //  SongInfoShort currentInfo = null;
                foreach (var selectedItem in lstSongList.SelectedItems)
                {
                    string[] lstItemArray = selectedItem.ToString().Split(new string[] { " - " }, StringSplitOptions.None);
                    var arrangement = (lstItemArray[2] == "_") ? null : lstItemArray[2];
                    var identifier = lstItemArray[3];
                    var currentInfo = new SongInfoShort()
                        {
                            Arrangement = arrangement,
                            Identifier = identifier
                        };
                    SongListShort.Add(currentInfo);
                }
            }
            else
            {
                if (MessageBox.Show("Please select song(s) and arrangement(s) to convert." + Environment.NewLine +
                    "Click 'Retry' to continue or 'Cancel' to Abort", MESSAGEBOX_CAPTION,
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                    SongListShort.Add(new SongInfoShort { Identifier = "User Aborted", Arrangement = "User Aborted" });
            }
        }

        private void lstSongList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (toggleSelect)
                    for (int i = 0; i < lstSongList.Items.Count; i++)
                    {
                        lstSongList.SelectedIndex = i;
                        toggleSelect = false;
                    }
                else
                {
                    lstSongList.SelectedIndex = -1;
                    toggleSelect = true;
                }
            }
        }


    }
}
