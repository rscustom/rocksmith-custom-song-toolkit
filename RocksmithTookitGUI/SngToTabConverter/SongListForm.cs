using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithToolkitLib.Song2014ToTab;
using RocksmithToolkitLib.Xml;

namespace RocksmithToolkitGUI.SngToTabConverter
{
    public partial class SongListForm : Form
    {
        public SongListForm()
        {
            InitializeComponent();
        }

        public IList<SongInfoShort> SongListShort;

        private const string MESSAGEBOX_CAPTION = "SongInfo List Form";

        public void PopSongListBox(IList<SongInfo> songList)
        {
            lstSongList.Items.Clear();
            foreach (var song in songList)
                foreach (var arrangement in song.Arrangements)
                    lstSongList.Items.Add(song.Artist + " - " + song.Title + " - " + arrangement + " - " + song.Identifier);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (lstSongList.SelectedItems.Count > 0)
            {
                SongListShort = new List<SongInfoShort>();
                SongInfoShort currentInfo = null;

                foreach(var selectedItem in lstSongList.SelectedItems)
                {                  
                    string[] lstItemArray = selectedItem.ToString().Split(new string[] {" - "},                                                                                    StringSplitOptions.None);
                    var arrangement = lstItemArray[2];
                    var identifier = lstItemArray[3];
                    currentInfo = new SongInfoShort()
                        {
                            Identifier = identifier,
                            Arrangement = arrangement
                        };
                    SongListShort.Add(currentInfo);
                }

             //  Close();
             //  Refresh();
             }
            else
            {
                MessageBox.Show("Please select song(s) and arrangement(s) to convert.", MESSAGEBOX_CAPTION,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }

}
