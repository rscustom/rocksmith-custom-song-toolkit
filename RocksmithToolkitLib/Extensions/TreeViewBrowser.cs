using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;

namespace RocksmithToolkitLib.Extensions
{
    /// <summary>
    /// Custom TreeView Browser by Cozy1
    /// </summary>
    public class TreeViewBrowser : TreeView
    {
        #region TreeViewBrowser Code
        // some code from: https://github.com/ItsEddie/system-explorer-treeview

        // set variables here for debugging
        private bool enableEventHandlers = true;
        private bool enableOverrides = true;
        private bool enableAddEmptyNode = true;
        private bool enableNodesClear = true;


        #region Win32 Icon Extraction
        IntPtr hImgSmall; //the handle to the system image list
        IntPtr hImgLarge; //the handle to the system image list
        string fName; // 'the file name to get icon from
        SHFILEINFO shinfo = new SHFILEINFO();

        [StructLayout(LayoutKind.Sequential)]

        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath,
                uint dwFileAttributes,
                ref SHFILEINFO psfi,
                uint cbSizeFileInfo,
                uint uFlags);
        }

        #endregion

        // Icon mapper to map file icons to an image list
        private Dictionary<string, int> iconMapper = new Dictionary<string, int>();

        private List<string> defaultFolders = Environment.GetLogicalDrives().ToList();
        /// <summary>
        /// The default folders in the TreeView
        /// </summary>
        public List<string> DefaultFolders
        {
            get { return defaultFolders; }
            set { defaultFolders = value; }
        }

        private string initialDirectory = "";
        /// <summary>
        /// Gets/Sets the TreeView startup state InitialDirectory
        /// </summary>
        public string InitialDirectory
        {
            get { return initialDirectory; }
            set { initialDirectory = value; }
        }

        private Dictionary<string, string> filter = new Dictionary<string, string>();
        /// <summary>
        /// Get/Set a key/value dictionary Filter extension/name
        /// that is converted from a Windows style OFD Filter
        /// </summary>
        public Dictionary<string, string> Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        private bool multiselect = false;
        /// <summary>
        /// Gets/Sets the TreeView Multiselect boolean condition
        /// </summary>
        public bool Multiselect
        {
            get { return multiselect; }
            set { multiselect = value; }
        }

        private bool listDirectories = true;
        /// <summary>
        /// Get/Set boolean whether or not to list directories in TreeView
        /// </summary>
        public bool ListDirectories
        {
            get { return listDirectories; }
            set { listDirectories = value; }
        }

        private bool listFiles = true;
        /// <summary>
        /// Get/Set boolean whether or not to list files in TreeView
        /// </summary>
        public bool ListFiles
        {
            get { return listFiles; }
            set { listFiles = value; }
        }

        /// <summary>
        /// The DesignMode property does not correctly tell you if
        /// you are in design mode.  IsDesignerHosted is a corrected
        /// version of that property.
        /// (see https://connect.microsoft.com/VisualStudio/feedback/details/553305
        /// and http://stackoverflow.com/a/2693338/238419 )
        /// </summary>
        public bool IsDesignerHosted
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;

                Control ctrl = this;
                while (ctrl != null)
                {
                    if ((ctrl.Site != null) && ctrl.Site.DesignMode)
                        return true;
                    ctrl = ctrl.Parent;
                }
                return false;
            }
        }

        public TreeViewBrowser()
        {
            // DO NOTHING UNTIL FULLY LOADED
        }

        /// <summary>
        /// Initialize a new browser TreeView
        /// </summary>
        public void InitTreeViewBrowser()
        {
            if (IsDesignerHosted)
                return;

            if (DefaultFolders == null || DefaultFolders.Count == 0)
                return;

            ImageList = new ImageList();
            ImageList.ImageSize = new Size(16, 16);
            ImageList.ColorDepth = ColorDepth.Depth32Bit;

            hImgSmall = Win32.SHGetFileInfo(Environment.GetFolderPath(Environment.SpecialFolder.System), 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            ImageList.Images.Add(Icon.FromHandle(shinfo.hIcon));

            // activate EH's
            ToggleEventHandlers(true);
            m_coll = new ArrayList(); // start with fresh ArrayList
            InitDefaultFolders();
            // de-activate EH's
            ToggleEventHandlers(false);
            RestoreTreeView(new List<string>() { InitialDirectory });
            // re- activate EH's
            ToggleEventHandlers(true); // this trashes the InitialDirectory Restore
        }

        private void ToggleEventHandlers(bool enable)
        {
            if (!enableEventHandlers)
                return;

            if (enable)
            {
                BeforeExpand += TreeViewBrowser_BeforeExpand;
                BeforeCollapse += TreeViewBrowser_BeforeCollapse;
                NodeMouseDoubleClick += TreeViewBrowser_NodeMouseDoubleClick;
            }
            else
            {
                BeforeExpand -= TreeViewBrowser_BeforeExpand;
                BeforeCollapse -= TreeViewBrowser_BeforeCollapse;
                NodeMouseDoubleClick -= TreeViewBrowser_NodeMouseDoubleClick;
            }
        }

        private void InitDefaultFolders()
        {
            if (enableNodesClear)
                Nodes.Clear(); // removes all tree nodes from the collection

            foreach (var defaultFolder in DefaultFolders)
            {
                if (!iconMapper.ContainsKey(defaultFolder))
                {
                    hImgSmall = Win32.SHGetFileInfo(Environment.GetLogicalDrives()[0], 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
                    ImageList.Images.Add(Icon.FromHandle(shinfo.hIcon));
                    iconMapper[defaultFolder] = ImageList.Images.Count - 1;
                }

                Nodes.Add(new TreeNode(defaultFolder) { Tag = defaultFolder, ImageIndex = iconMapper[defaultFolder], SelectedImageIndex = iconMapper[defaultFolder] });
            }

            // adds a new empty nodes
            if (enableAddEmptyNode)
            {
                foreach (TreeNode Node in Nodes)
                    Node.Nodes.Add("");
            }
        }

        private void TreeViewBrowser_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // opens the double clicked directory or file in windows explorer
            if (File.Exists(e.Node.Tag.ToString()) || Directory.Exists(e.Node.Tag.ToString()))
                Process.Start(e.Node.Tag.ToString());
        }

        private void TreeViewBrowser_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (enableNodesClear)
                e.Node.Nodes.Clear();

            if (enableAddEmptyNode)
                e.Node.Nodes.Add(""); // adds a new empty node ... why???
        }

        // NOTE: Before activating this EventHandler ... 
        // Populate DefaultFolders and Restore InitialDirectory
        private void TreeViewBrowser_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            TreeNode node = e.Node;

            if (treeView == null || node == null)
            {
                e.Cancel = true;
                return;
            }

            AddNodes(treeView, node);
        }

        private void AddNodes(TreeView treeView, TreeNode node)
        {
            // perform TreeView data integrity checks
            try
            {
                var nodes = treeView.Nodes;
                var tag = node.Tag.ToString();
                var text = node.Text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("<ERROR> AddNodes " + ex.StackTrace);
                throw new IOException("Missing critical TreeView information ... Check It!");
            }

            if (enableNodesClear)
                node.Nodes.Clear();

            if (ListDirectories)
            {
                try
                {
                    var directories = Directory.GetDirectories(node.Tag.ToString());

                    foreach (var dir in directories)
                    {
                        node.Nodes.Add(new TreeNode(Path.GetFileNameWithoutExtension(dir))
                        {
                            Tag = dir,
                            ImageIndex = 0,
                            SelectedImageIndex = 0
                        });

                        // adds a new empty node ... why???
                        if (enableAddEmptyNode)
                            node.Nodes[node.Nodes.Count - 1].Nodes.Add("");
                    }
                }
                catch
                {
                    node.Nodes.Add("Directory listing access denied at: " + node.Tag.ToString());
                }
            }

            if (ListFiles)
            {
                try
                {
                    var files = Directory.GetFiles(node.Tag.ToString()).ToList();

                    // filter the file extensions
                    if (Filter != null && Filter.Count > 0 && !Filter.ContainsKey("*.*"))
                    {
                        List<string> fileExt = new List<string>();
                        foreach (var key in Filter.Keys)
                        {
                            var cleanKey = key.Replace("*", "");
                            fileExt.Add(cleanKey);
                        }

                        files = files.Where(fi => fileExt.Any(fi.ToLower().EndsWith)).ToList();
                    }

                    foreach (var file in files)
                    {
                        var extension = Path.GetExtension(file).ToLower();

                        if (!iconMapper.ContainsKey(extension))
                        {
                            hImgSmall = Win32.SHGetFileInfo(file, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
                            var image = Icon.FromHandle(shinfo.hIcon);

                            ImageList.Images.Add(image.ToBitmap());
                            iconMapper[extension] = ImageList.Images.Count - 1;
                        }

                        node.Nodes.Add(new TreeNode(Path.GetFileName(file))
                        {
                            Tag = file,
                            ImageIndex = iconMapper[extension],
                            SelectedImageIndex = iconMapper[extension]
                        });
                    }
                }
                catch
                {
                    node.Nodes.Add("File listing access denied at: " + node.Tag.ToString());
                }
            }
        }

        /// <summary>
        /// Clear the TreeView
        /// </summary>
        public void ClearTreeView()
        {
            if (enableNodesClear)
                Nodes.Clear();
        }

        /// <summary>
        /// Dispose of the TreeView
        /// </summary>
        public void DisposeTreeView()
        {
            if (enableNodesClear)
                Nodes.Clear();

            for (var i = 0; i < ImageList.Images.Count; i++)
                ImageList.Images[i].Dispose();

            ImageList.Dispose();
        }
        #endregion

        #region TreeViewMultiselect Code
        // some code from: https://www.codeproject.com/Articles/2756/C-TreeView-with-multiple-selection

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (enableOverrides)
                return;

            // TODO: Add custom paint code here

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private ArrayList m_coll;
        public ArrayList SelectedNodes
        {
            get
            {
                return m_coll;
            }
            set
            {
                if (m_coll == null)
                    m_coll = new ArrayList();

                RemovePaintFromNodes();
                m_coll.Clear();
                m_coll = value;
                PaintSelectedNodes();
            }
        }

        // (overriden method, and base class called to ensure events are triggered)
        protected TreeNode m_firstNode, m_lastNode;
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            if (!enableOverrides)
                return;

            base.OnBeforeSelect(e);
            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            // selecting twice the node while pressing CTRL ?
            if (bControl && m_coll.Contains(e.Node))
            {
                // unselect it (let framework know we don't want selection this time)
                e.Cancel = true;

                // update nodes
                RemovePaintFromNodes();
                m_coll.Remove(e.Node);
                PaintSelectedNodes();
                return;
            }

            m_lastNode = e.Node;
            if (!bShift)
                m_firstNode = e.Node; // store begin of shift sequence
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (!enableOverrides)
                return;

            base.OnAfterSelect(e);
            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl)
            {
                if (!m_coll.Contains(e.Node)) // new node ?
                    m_coll.Add(e.Node);
                else  // not new, remove it from the collection
                {
                    RemovePaintFromNodes();
                    m_coll.Remove(e.Node);
                }

                PaintSelectedNodes();
            }
            else
            {
                // SHIFT is pressed
                if (bShift && multiselect)
                {
                    Queue myQueue = new Queue();

                    TreeNode uppernode = m_firstNode;
                    TreeNode bottomnode = e.Node;
                    // case 1 : begin and end nodes are parent
                    bool bParent = IsParent(m_firstNode, e.Node); // is m_firstNode parent (direct or not) of e.Node
                    if (!bParent)
                    {
                        bParent = IsParent(bottomnode, uppernode);
                        if (bParent) // swap nodes
                        {
                            TreeNode t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }
                    if (bParent)
                    {
                        TreeNode n = bottomnode;
                        while (n != uppernode.Parent)
                        {
                            if (!m_coll.Contains(n)) // new node ?
                                myQueue.Enqueue(n);

                            n = n.Parent;
                        }
                    }
                    // case 2 : nor the begin nor the end node are descendant one another
                    else
                    {
                        if ((uppernode.Parent == null && bottomnode.Parent == null) || (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode))) // are they siblings ?
                        {
                            int nIndexUpper = uppernode.Index;
                            int nIndexBottom = bottomnode.Index;
                            if (nIndexBottom < nIndexUpper) // reversed?
                            {
                                TreeNode t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                nIndexUpper = uppernode.Index;
                                nIndexBottom = bottomnode.Index;
                            }

                            TreeNode n = uppernode;
                            while (nIndexUpper <= nIndexBottom)
                            {
                                if (!m_coll.Contains(n)) // new node ?
                                    myQueue.Enqueue(n);

                                n = n.NextNode;

                                nIndexUpper++;
                            } // end while

                        }
                        else
                        {
                            if (!m_coll.Contains(uppernode)) myQueue.Enqueue(uppernode);
                            if (!m_coll.Contains(bottomnode)) myQueue.Enqueue(bottomnode);
                        }
                    }

                    m_coll.AddRange(myQueue);

                    PaintSelectedNodes();
                    m_firstNode = e.Node; // let us chain several SHIFTs if we like it
                } // end if m_bShift
                else
                {
                    // in the case of a simple click, just add this item
                    if (m_coll != null && m_coll.Count > 0)
                    {
                        RemovePaintFromNodes();
                        m_coll.Clear();
                    }

                    m_coll.Add(e.Node);
                }
            }
        }

        // Helpers
        protected bool IsParent(TreeNode parentNode, TreeNode childNode)
        {
            if (parentNode == childNode)
                return true;

            TreeNode n = childNode;
            bool bFound = false;

            while (!bFound && n != null)
            {
                n = n.Parent;
                bFound = (n == parentNode);
            }

            return bFound;
        }

        protected void PaintSelectedNodes()
        {
            if (!enableOverrides)
                return;

            foreach (TreeNode n in m_coll)
            {
                n.BackColor = SystemColors.Highlight;
                n.ForeColor = SystemColors.HighlightText;
            }
        }

        protected void RemovePaintFromNodes()
        {
            if (!enableOverrides)
                return;

            try
            {
                TreeNode n0 = (TreeNode)m_coll[0];
                Color back = n0.TreeView.BackColor;
                Color fore = n0.TreeView.ForeColor;

                foreach (TreeNode n in m_coll)
                {
                    n.BackColor = back;
                    n.ForeColor = fore;
                }
            }
            catch (Exception ex) // DO NOTHING
            {
                Debug.WriteLine("<ERROR> removePaintFromNodes: " + ex.Message);
            }
        }
        #endregion

        #region Save/RestoreTreeView/UpdateFilter
        private void UpdateExpandedList(ref List<string> expandedList, TreeNode node)
        {
            if (node.IsExpanded)
                expandedList.Add(node.Tag.ToString());

            foreach (TreeNode n in node.Nodes)
            {
                if (n.IsExpanded)
                    UpdateExpandedList(ref expandedList, n);
            }
        }

        /// <summary>
        /// Usage Tip:
        /// var expandedState = GetAllExpandedList();
        /// File.WriteAllLines(@"C:\Temp\ExpandedState.txt", expandedState);
        /// </summary>
        public List<string> GetAllExpandedList()
        {
            var expandedList = new List<string>();

            foreach (TreeNode node in Nodes)
            {
                UpdateExpandedList(ref expandedList, node);
            }

            return expandedList;
        }

        private void ExpandNodes(TreeNode node, string nodeFullPath)
        {
            if (node.Tag.ToString() == nodeFullPath)
                node.Expand();

            foreach (TreeNode n in node.Nodes)
            {
                if (n.Nodes.Count > 0)
                    ExpandNodes(n, nodeFullPath);
            }
        }

        /// <summary>
        /// Usage Tip:
        /// var expandedArray = File.ReadAllLines(@"C:\Temp\ExpandedState.txt");
        /// var expandedState = new List<string>(expandedArray);
        /// RestoreTreeViewState(expandedState);
        /// </summary>
        public void RestoreTreeViewState(List<string> expandedState)
        {
            foreach (TreeNode node in Nodes)
            {
                foreach (var state in expandedState)
                {
                    ExpandNodes(node, state);
                }
            }
        }

        public void RestoreTreeView(List<string> paths, string separator = @"\")
        {
            if (paths == null || paths.Count == 0)
                return;

            foreach (var path in paths.Where(x => !string.IsNullOrEmpty(x.Trim())))
            {
                BeginUpdate(); // prevent TreeView flickering and scrolling

                var pathParts = path.Split(new[] { separator }, StringSplitOptions.None);
                var rootPath = Path.GetPathRoot(path);
                var rootNode = Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == rootPath);
                if (rootNode != null)
                {
                    Debug.WriteLine("Added RootNode: " + rootNode.Text);
                    AddNodes(this, rootNode);
                }
                else
                    throw new DirectoryNotFoundException("Did not find the rootNode text: " + rootPath);

                var childNode = rootNode;

                for (int i = 1; i < pathParts.Count(); i++)
                {
                    var subNode = childNode.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == pathParts[i]);
                    if (subNode != null)
                    {
                        Debug.WriteLine("Added SubNode: " + subNode.Text);
                        AddNodes(this, subNode);
                        childNode = subNode;
                    }
                    else
                        throw new DirectoryNotFoundException("Did not find node text: " + pathParts[i]);

                }

                var restoreNode = FindNodeByTagRecursive(rootNode, childNode.Tag);
                if (restoreNode == null)
                    throw new DirectoryNotFoundException("Could not restoreNode for: " + path);

                EndUpdate(); // enable TreeView redrawing and scrolling
                Debug.WriteLine("Set InitialDirectory to: " + restoreNode.FullPath);
                SelectedNode = restoreNode; // select node
                PaintSelectedNodes(); // paint selected node
                restoreNode.Expand(); // shows node and contents
                restoreNode.EnsureVisible(); // scroll selected node into view
            }
        }

        static TreeNode FindNodeByTagRecursive(TreeNode parentNode, object tag)
        {
            if (parentNode.Tag == tag)
                return parentNode;

            foreach (TreeNode node in parentNode.Nodes)
            {
                TreeNode n = FindNodeByTagRecursive(node, tag);
                if (n != null)
                    return n;
            }

            return null;
        }

        public void UpdatedFilter()
        {
            var selectedNode = SelectedNode;
            var rootPath = Path.GetPathRoot(selectedNode.Tag.ToString());
            var rootNode = Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == rootPath);
            var restoreNode = FindNodeByTagRecursive(rootNode, selectedNode.Tag);

            if (restoreNode != null)
                AddNodes(this, restoreNode); // updates the filtered files
        }

        #endregion

    }
}
