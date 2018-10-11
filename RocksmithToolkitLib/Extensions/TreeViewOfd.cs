using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Linq;

namespace RocksmithToolkitLib.Extensions
{
    public partial class TreeViewOfd : Form
    {
        /// <summary>
        /// Custom TreeView OpenFileDialog by Cozy1
        /// <para>Output from FileNames property preserves</para>
        /// <para>the selection and custom ordering of items</para>
        /// </summary>
        public TreeViewOfd()
        {
            InitializeComponent();
        }

        private string title = "";
        /// <summary>
        /// Get/Set Custom TreeView OpenFileDialog Title
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                this.Text = title;
            }
        }

        private string initialDirectory = "";
        /// <summary>
        /// Get/Set Custom TreeView OpenFileDialog InitialDirectory
        /// </summary>
        public string InitialDirectory
        {
            get { return initialDirectory; }
            set
            {
                initialDirectory = value;
                treeViewBrowser.InitialDirectory = initialDirectory;
            }
        }

        private bool multiSelect = false;
        /// <summary>
        /// Get/Set Custom TreeView OpenFileDialog Multiselect boolean condition
        /// </summary>
        public bool Multiselect
        {
            get { return multiSelect; }
            set
            {
                multiSelect = value;
                treeViewBrowser.Multiselect = multiSelect;
            }
        }

        private string expandedStateSavePath = "";
        /// <summary>
        /// Get/Set Custom TreeView ExpandedState save file path
        /// </summary>
        public string ExpandedStateSavePath
        {
            get { return expandedStateSavePath; }
            set { expandedStateSavePath = value; }
        }

        private List<string> fileNames = new List<string>();
        /// <summary>
        /// Get/Set Custom TreeView Selected File Names/Paths
        /// <para>Output from FileNames property preserves</para>
        /// <para>the selection and custom ordering of items</para>
        /// </summary>
        public List<string> FileNames
        {
            get { return fileNames; }
            set { fileNames = value; }
        }

        private Dictionary<string, string> ConvertFilter(string filterString = "All Files (*.*)|*.*")
        {
            var regex = new Regex(@"(?<Name>[^|]*)\|(?<Extension>[^|]*)\|?");
            // known good filter for debugging
            //var matches = regex.Matches(@"Image Files (*.bmp, *.jpg)|*.bmp;*.jpg|All Files (*.*)|*.*");
            var matches = regex.Matches(filterString);
            var filterDict = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                filterDict.Add(match.Groups["Extension"].Value, match.Groups["Name"].Value);
                Debug.Print("Key Extension:'{1}' Value Name: '{0}'", match.Groups["Extension"].Value, match.Groups["Name"].Value);
            }

            return filterDict;
        }

        private string filter = "All Files (*.*)|*.*";
        /// <summary>
        /// Get/Set Custom TreeView OpenFileDialog Filter
        /// e.g. "Image Files (*.bmp, *.jpg)|*.bmp;*.jpg|All Files (*.*)|*.*" 
        /// </summary>
        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        private Dictionary<string, string> currentFilter = new Dictionary<string, string>();
        /// <summary>
        /// Get/Set Custom TreeView OpenFileDialog Filter as Dictionary 
        /// e.g. "All Files (*.*)|*.*" where Key = "*.*", Value = "All Files (*.*)"
        /// </summary>
        public Dictionary<string, string> CurrentFilter
        {
            get { return currentFilter; }
            set
            {
                currentFilter = value;
                treeViewBrowser.Filter = currentFilter;
            }
        }

        private void PopulateComboFilesOfType()
        {
            var filterDict = ConvertFilter(Filter);
            cmbFilesOfType.Items.Clear();
            cmbFilesOfType.DataSource = new BindingSource(filterDict, null);
            cmbFilesOfType.DisplayMember = "Value";
            cmbFilesOfType.ValueMember = "Key";
            cmbFilesOfType.SelectedIndex = 0;
        }

        private void butAdd_Click(object sender, System.EventArgs e)
        {
            try
            {
                // update imageList from treeViewBrowser.ImageList
                imageList.Images.Clear();
                foreach (Bitmap image in treeViewBrowser.ImageList.Images)
                    imageList.Images.Add(image);

                // add selected items from treeview to the listview on the right hand side
                foreach (TreeNode n in treeViewBrowser.SelectedNodes)
                {
                    // prevent duplicates
                    if (listView.Items.ContainsKey(n.FullPath))
                        continue;

                    // select files only
                    if (!Directory.Exists(n.FullPath) && treeViewBrowser.ListFiles)
                        listView.Items.Add(n.FullPath, n.Text, n.ImageIndex);
                    // select directories only
                    else if (Directory.Exists(n.FullPath) && (treeViewBrowser.ListDirectories && !treeViewBrowser.ListFiles))
                        listView.Items.Add(n.FullPath, n.Text, n.ImageIndex);
                    // select drives only
                    else if (!treeViewBrowser.ListDirectories && !treeViewBrowser.ListFiles)
                        listView.Items.Add(n.FullPath, n.Text, n.ImageIndex);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("<ERROR> Button Add: " + ex.Message);
            }
        }

        private void ListViewUpDown(Keys key)
        {
            // TODO: address movement of multi selection 
            try
            {
                var index = listView.SelectedItems[0].Index;
                var item = listView.Items[index];
                int newIndex = index;

                switch (key)
                {
                    case Keys.Up:
                        newIndex--;
                        break;
                    case Keys.Down:
                        newIndex++;
                        break;
                    default:
                        return;
                }

                listView.SelectedItems.Clear();
                listView.Items.RemoveAt(index);

                if (newIndex >= 0 && newIndex <= listView.Items.Count)
                {
                    listView.Items.Insert(newIndex, item);
                    listView.Items[newIndex].Selected = true;
                    listView.Items[newIndex].Focused = true;
                }
                else
                {
                    listView.Items.Insert(index, item);
                    listView.Items[index].Selected = true;
                    listView.Items[index].Focused = true;
                }

                listView.Focus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("<ERROR> ListViewUpDown: " + ex.Message);
            }
        }

        private void butDown_Click(object sender, System.EventArgs e)
        {
            ListViewUpDown(Keys.Down);
        }

        private void butUp_Click(object sender, System.EventArgs e)
        {
            ListViewUpDown(Keys.Up);
        }

        private void butRemove_Click(object sender, System.EventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItemCollection = listView.SelectedItems;

                // Warning : selectedItemCollection.Count dynamically updated by listView1.Items.Remove(...)
                while (selectedItemCollection.Count > 0)
                    listView.Items.Remove(selectedItemCollection[0]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("<ERROR> Button Remove: " + ex.Message);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var filePaths = new List<string>();
            foreach (ListViewItem item in listView.Items)
                filePaths.Add(item.Name);

            FileNames = filePaths;

            var initialDirectory = Path.GetDirectoryName(filePaths.FirstOrDefault());
            if (!String.IsNullOrEmpty(initialDirectory))
                InitialDirectory = initialDirectory;

            // (Optional) Save TreeView ExpandedState to a file
            if (!String.IsNullOrEmpty(ExpandedStateSavePath))
            {
                var expandedState = treeViewBrowser.GetAllExpandedList();
                var selectedNode = treeViewBrowser.SelectedNodes[0] as TreeNode;

                if (selectedNode != null && !String.IsNullOrEmpty(selectedNode.Tag.ToString()))
                {
                    var selectedPath = selectedNode.Tag.ToString();
                    expandedState.Add(selectedPath);
                }

                File.WriteAllLines(ExpandedStateSavePath, expandedState);
            }

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void TreeViewOfd_Load(object sender, EventArgs e)
        {
            PopulateComboFilesOfType();
            // call InitBrowserTreeview ONLY after the form is fully loaded 
            treeViewBrowser.InitTreeViewBrowser();
        }

        private void cmbFilesOfType_SelectedIndexChanged(object sender, EventArgs e)
        {
            KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>)cmbFilesOfType.SelectedItem;
            var key = selectedItem.Key;
            var value = selectedItem.Value;
            // convert KeyValuePair to Dictionary
            var currentItem = new Dictionary<string, string>();
            currentItem.Add(key, value);
            CurrentFilter = currentItem;

            // check if there is anything worth refiltering
            if (treeViewBrowser.Nodes.Count > 0)
                treeViewBrowser.UpdatedFilter();
        }

    }
}
