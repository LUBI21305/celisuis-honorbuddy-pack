//#define PBDEBUG
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Styx.Helpers;
using TreeSharp;
using Styx.Logic.BehaviorTree;
using System.Xml;
using HighVoltz.Composites;
using Styx.Logic.Profiles;
using System.Threading;

namespace HighVoltz
{
    public partial class MainForm : Form
    {
        [Flags]
        enum CopyPasteOperactions { Cut = 0, IgnoreRoot = 1, Copy = 2 };
        CopyPasteOperactions copyAction = CopyPasteOperactions.Cut;

        TreeNode copySource = null;
        FileSystemWatcher profileWatcher;

        public static MainForm Instance { get; private set; }
        public static bool IsValid { get { return Instance != null && Instance.Visible && !Instance.Disposing && !Instance.IsDisposed; } }
        private Professionbuddy PB;
        private PropertyBag ProfilePropertyBag;

        private delegate void guiInvokeCB();
        private delegate void refreshActionTreeDelegate(IPBComposite pbComposite, Type type);
        // used to update GUI controls via other threads

        #region Initalize/update methods
        public MainForm()
        {
            Instance = this;
            PB = Professionbuddy.Instance;
            InitializeComponent();
            saveFileDialog.InitialDirectory = PB.ProfilePath;

            profileWatcher = new FileSystemWatcher(PB.ProfilePath);
            profileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            profileWatcher.Changed += profileWatcher_Changed;
            profileWatcher.Created += profileWatcher_Changed;
            profileWatcher.Deleted += profileWatcher_Changed;
            profileWatcher.Renamed += profileWatcher_Changed;
            profileWatcher.EnableRaisingEvents = true;

            // used by the dev to display the 'Secret button', a button that dumps some debug info of the Task list.
            if (Environment.UserName == "highvoltz")
            {
                toolStripSecretButton.Visible = true;
            }
        }


        private delegate void InitDelegate();
        public void Initialize()
        {
            RefreshProfileList();
            InitTradeSkillTab();
            InitActionTree();
            PopulateActionGridView();
            if (PB.HasDataStoreAddon && !toolStripAddCombo.Items.Contains("Banker"))
                toolStripAddCombo.Items.Add("Banker");
            toolStripAddCombo.SelectedIndex = 0;
#if DLL
            toolStripOpen.Image = HighVoltz.Properties.Resources.OpenPL;
            toolStripSave.Image = HighVoltz.Properties.Resources.SaveHL;
            toolStripCopy.Image = HighVoltz.Properties.Resources.copy;
            toolStripCut.Image = HighVoltz.Properties.Resources.cut;
            toolStripPaste.Image = HighVoltz.Properties.Resources.paste_32x32;
            toolStripDelete.Image = HighVoltz.Properties.Resources.delete;
            toolStripAddBtn.Image = HighVoltz.Properties.Resources._112_RightArrowLong_Orange_32x32_72;
            toolStripMaterials.Image = HighVoltz.Properties.Resources.Notepad_32x32;
            toolStripHelp.Image = HighVoltz.Properties.Resources._109_AllAnnotations_Help_32x32_72;
            toolStripSettings.Image = HighVoltz.Properties.Resources.settings_48);
#else
            string imagePath = Path.Combine(PB.PluginPath, "Icons//");
            toolStripOpen.Image = Image.FromFile(imagePath + "OpenPL.bmp");
            toolStripSave.Image = Image.FromFile(imagePath + "SaveHL.bmp");
            toolStripCopy.Image = Image.FromFile(imagePath + "copy.png");
            toolStripCut.Image = Image.FromFile(imagePath + "cut.png");
            toolStripPaste.Image = Image.FromFile(imagePath + "paste_32x32.png");
            toolStripDelete.Image = Image.FromFile(imagePath + "delete.png");
            toolStripAddBtn.Image = Image.FromFile(imagePath + "112_RightArrowLong_Orange_32x32_72.png");
            toolStripMaterials.Image = Image.FromFile(imagePath + "Notepad_32x32.png");
            toolStripHelp.Image = Image.FromFile(imagePath + "109_AllAnnotations_Help_32x32_72.png");
            toolStripSettings.Image = Image.FromFile(imagePath + "settings_48.png");
#endif

            if (PB.ProfileSettings.Settings.Count == 0)
                toolStripSettings.Enabled = false;
            if (PB.TradeSkillList.Count > 0)
                TradeSkillTabControl.Visible = true;
            UpdateControls();
        }

        #region ProfileTab
        public void RefreshProfileList()
        {
            if (!IsValid)
                return;
            if (ProfileTab.InvokeRequired)
                ProfileTab.BeginInvoke(new guiInvokeCB(RefreshProfileListCallback));
            else
                RefreshProfileListCallback();
        }

        void RefreshProfileListCallback()
        {
            ProfileListView.SuspendLayout();
            ProfileListView.Clear();
            string[] profiles = Directory.GetFiles(PB.ProfilePath, "*.xml", SearchOption.TopDirectoryOnly).
                Select(s => Path.GetFileName(s)).
                Union(Directory.GetFiles(PB.ProfilePath, "*.package", SearchOption.TopDirectoryOnly)).
                Select(s => Path.GetFileName(s)).
                ToArray();

            // remove all profile names from ListView that are not in the 'profile' array
            for (int i = 0; i < ProfileListView.Items.Count; i++)
            {
                if (!profiles.Contains(ProfileListView.Items[i].Name))
                    ProfileListView.Items.RemoveAt(i);
            }
            // Add all profiles that are not in ListView
            foreach (string profile in profiles)
            {
                if (!ProfileListView.Items.ContainsKey(profile))
                    ProfileListView.Items.Add(profile, profile, null);
            }
            ProfileListView.ResumeLayout();
        }
        #endregion

        #region TradeSkillTab
        public void InitTradeSkillTab()
        {
            if (!IsValid)
                return;
            if (TradeSkillTabControl.InvokeRequired)
                TradeSkillTabControl.BeginInvoke(new guiInvokeCB(InitTradeSkillTabCallback));
            else
                InitTradeSkillTabCallback();
        }

        void InitTradeSkillTabCallback()
        {
            TradeSkillTabControl.SuspendLayout();
            TradeSkillTabControl.TabPages.Clear();
            for (int i = 0; i < PB.TradeSkillList.Count; i++)
            {
                TradeSkillTabControl.TabPages.Add(new TradeSkillListView(i));
            }
            TradeSkillTabControl.ResumeLayout();
        }
        #endregion

        #region RefreshActionTree
        public void RefreshActionTree(Type type)
        {
            RefreshActionTree(null, type);
        }

        public void RefreshActionTree(IPBComposite pbComp)
        {
            RefreshActionTree(pbComp, null);
        }

        public void RefreshActionTree()
        {
            RefreshActionTree(null, null);
        }

        /// <summary>
        /// Refreshes all actions of specified type in ActionTree or all if type is null
        /// </summary>
        /// <param name="type"></param>
        public void RefreshActionTree(IPBComposite pbComp, Type type)
        {
            // Don't update ActionTree while PB is running to improve performance.
            if (PB.IsRunning || !IsValid)
                return;
            if (ActionTree.InvokeRequired)
                ActionTree.BeginInvoke(new refreshActionTreeDelegate(RefreshActionTreeCallback), pbComp, type);
            else
                RefreshActionTreeCallback(pbComp, type);
        }
        void RefreshActionTreeCallback(IPBComposite pbComp, Type type)
        {
            ActionTree.SuspendLayout();
            foreach (TreeNode node in ActionTree.Nodes)
            {
                UdateTreeNode(node, pbComp, type, true);
            }
            ActionTree.ResumeLayout();
        }
        #endregion

        #region InitActionTree
        public void InitActionTree()
        {
            if (!IsValid)
                return;
            if (ActionTree.InvokeRequired)
                ActionTree.BeginInvoke(new guiInvokeCB(InitActionTreeCallback));
            else
                InitActionTreeCallback();
        }
        public void InitActionTreeCallback()
        {
            ActionTree.SuspendLayout();
            int selectedIndex = -1;
            if (ActionTree.SelectedNode != null)
                selectedIndex = ActionTree.Nodes.IndexOf(ActionTree.SelectedNode);
            ActionTree.Nodes.Clear();
            foreach (IPBComposite composite in PB.CurrentProfile.Branch.Children)
            {
                TreeNode node = new TreeNode(composite.Title);
                node.ForeColor = composite.Color;
                node.Tag = composite;
                if (composite is GroupComposite)
                {
                    ActionTreeAddChildren((GroupComposite)composite, node);
                }
                ActionTree.Nodes.Add(node);
            }
            //ActionTree.ExpandAll();
            if (selectedIndex != -1)
            {
                if (selectedIndex < ActionTree.Nodes.Count)
                    ActionTree.SelectedNode = ActionTree.Nodes[selectedIndex];
                else
                    ActionTree.SelectedNode = ActionTree.Nodes[ActionTree.Nodes.Count - 1];
            }
            ActionTree.ResumeLayout();
        }
        #endregion

        #region RefreshTradeSkillTabs
        public void RefreshTradeSkillTabs()
        {
            if (!IsValid)
                return;
            if (TradeSkillTabControl.InvokeRequired)
                TradeSkillTabControl.BeginInvoke(new guiInvokeCB(RefreshTradeSkillTabsCallback));
            else
                RefreshTradeSkillTabsCallback();
        }

        private void RefreshTradeSkillTabsCallback()
        {
            foreach (TradeSkillListView tv in TradeSkillTabControl.TabPages)
            {
                tv.TradeDataView.SuspendLayout();
                foreach (DataGridViewRow row in tv.TradeDataView.Rows)
                {
                    TradeSkillRecipeCell cell = (TradeSkillRecipeCell)row.Cells[0].Value;
                    row.Cells[1].Value = Util.CalculateRecipeRepeat(cell.Recipe);
                    row.Cells[2].Value = cell.Recipe.Difficulty;
                }
                tv.TradeDataView.ResumeLayout();
            }
        }
        #endregion

        #region UpdateControls
        // locks/unlocks controls depending on if PB is running on not.
        public void UpdateControls()
        {
            if (!IsValid)
                return;
            if (this.InvokeRequired)
                this.BeginInvoke(new guiInvokeCB(UpdateControlsCallback));
            else
                UpdateControlsCallback();
        }

        void UpdateControlsCallback()
        {
            if (PB.IsRunning)
            {
                DisableControls();
                this.Text = string.Format("Profession Buddy - Running {0}",
                    !string.IsNullOrEmpty(PB.MySettings.LastProfile) ? "(" + Path.GetFileName(PB.MySettings.LastProfile) + ")" : "");
                toolStripStart.BackColor = Color.Green;
                toolStripStart.Text = "Running";
            }
            else
            {
                EnableControls();
                this.Text = string.Format("Profession Buddy - Stopped {0}",
                    !string.IsNullOrEmpty(PB.MySettings.LastProfile) ? "(" + Path.GetFileName(PB.MySettings.LastProfile) + ")" : "");
                toolStripStart.BackColor = Color.Red;
                toolStripStart.Text = "Stopped";
            }
        }
        #endregion

        bool IsChildNode(TreeNode parent, TreeNode child)
        {
            if ((child == null && parent != null) || child.Parent == null)
                return false;
            if (child.Parent == parent)
                return true;
            else
                return IsChildNode(parent, child.Parent);
        }

        void ToggleStart()
        {
            try
            {
                if (PB.IsRunning)
                {
                    PB.MySettings.IsRunning = PB.IsRunning = false;
                    PB.MySettings.Save();
                }
                else
                {
                    // reset all actions 
                    foreach (IPBComposite comp in PB.CurrentProfile.Branch.Children)
                    {
                        comp.Reset();
                    }
                    if (PB.CodeWasModified)
                    {
                        PB.GenorateDynamicCode();
                    }
                    PB.ProfileSettings.LoadDefaultValues();
                    PB.MySettings.IsRunning = PB.IsRunning = true;
                    PB.MySettings.Save();
                    Professionbuddy.PreLoadHbProfile();
                    Professionbuddy.PreChangeBot();
                    if (!TreeRoot.IsRunning)
                        TreeRoot.Start();
                }
                UpdateControls();
            }
            catch (Exception ex) { Professionbuddy.Err(ex.ToString()); }
        }

        void AddToActionTree(object action, TreeNode dest)
        {
            bool ignoreRoot = (copyAction & CopyPasteOperactions.IgnoreRoot) == CopyPasteOperactions.IgnoreRoot ? true : false;
            bool cloneActions = (copyAction & CopyPasteOperactions.Copy) == CopyPasteOperactions.Copy ? true : false;
            TreeNode newNode = null;
            if (action is TreeNode)
            {
                if (cloneActions)
                {
                    newNode = RecursiveCloning(((TreeNode)action));
                }
                else
                    newNode = (TreeNode)((TreeNode)action).Clone();
            }
            else if (action.GetType().GetInterface("IPBComposite") != null)
            {
                IPBComposite composite = (IPBComposite)action;
                newNode = new TreeNode(composite.Title);
                newNode.ForeColor = composite.Color;
                newNode.Tag = composite;
            }
            else
                return;
            ActionTree.SuspendLayout();
            if (dest != null)
            {
                int treeIndex = action is TreeNode && ((TreeNode)action).Parent == dest.Parent &&
                    ((TreeNode)action).Index <= dest.Index && !cloneActions ?
                        dest.Index + 1 : dest.Index;
                GroupComposite gc = null;
                // If, While and SubRoutines are Decorators...
                if (!ignoreRoot && dest.Tag is GroupComposite)
                    gc = (GroupComposite)dest.Tag;
                else
                    gc = (GroupComposite)((Composite)dest.Tag).Parent;

                if ((dest.Tag is If || dest.Tag is SubRoutine) && !ignoreRoot)
                {
                    dest.Nodes.Add(newNode);
                    gc.AddChild((Composite)newNode.Tag);
                    if (!dest.IsExpanded)
                        dest.Expand();
                }
                else
                {
                    if (dest.Index >= gc.Children.Count)
                        gc.AddChild((Composite)newNode.Tag);
                    else
                        gc.InsertChild(dest.Index, (Composite)newNode.Tag);
                    if (dest.Parent == null)
                    {
                        if (treeIndex >= ActionTree.Nodes.Count)
                            ActionTree.Nodes.Add(newNode);
                        else
                            ActionTree.Nodes.Insert(treeIndex, newNode);
                    }
                    else
                    {
                        if (treeIndex >= dest.Parent.Nodes.Count)
                            dest.Parent.Nodes.Add(newNode);
                        else
                            dest.Parent.Nodes.Insert(treeIndex, newNode);
                    }
                }
            }
            else
            {
                ActionTree.Nodes.Add(newNode);
                PB.CurrentProfile.Branch.AddChild((Composite)newNode.Tag);
            }
            ActionTree.ResumeLayout();
        }

        TreeNode RecursiveCloning(TreeNode node)
        {
            IPBComposite newComp = (IPBComposite)(((IPBComposite)node.Tag).Clone());
            TreeNode newNode = new TreeNode(newComp.Title);
            newNode.ForeColor = newComp.Color;
            newNode.Tag = newComp;
            if (node.Nodes != null)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    GroupComposite gc = null;
                    // If, While and SubRoutine are Decorators.
                    if (newComp is GroupComposite)
                    {
                        gc = (GroupComposite)newComp;

                        TreeNode newChildNode = RecursiveCloning(child);
                        gc.AddChild((Composite)newChildNode.Tag);
                        newNode.Nodes.Add(newChildNode);
                    }
                }
            }
            return newNode;
        }

        void DisableControls()
        {
            ActionTree.Enabled = false;
            toolStripAddBtn.Enabled = false;
            toolStripOpen.Enabled = false;
            toolStripDelete.Enabled = false;
            toolStripCopy.Enabled = false;
            toolStripCut.Enabled = false;
            toolStripPaste.Enabled = false;
            ActionGrid.Enabled = false;
            LoadProfileButton.Enabled = false;
        }

        void EnableControls()
        {
            ActionTree.Enabled = true;
            toolStripAddBtn.Enabled = true;
            toolStripOpen.Enabled = true;
            toolStripDelete.Enabled = true;
            toolStripCopy.Enabled = true;
            toolStripCut.Enabled = true;
            toolStripPaste.Enabled = true;
            ActionGrid.Enabled = true;
            LoadProfileButton.Enabled = true;
        }

        void UdateTreeNode(TreeNode node, IPBComposite pbComp, Type type, bool recursive)
        {
            IPBComposite comp = (IPBComposite)node.Tag;
            if ((pbComp == null && type == null) ||
                (pbComp != null && pbComp == node.Tag) ||
                (type != null && type.IsAssignableFrom(node.Tag.GetType()))
                )
            {
                node.ForeColor = comp.Color;
                node.Text = comp.Title;
            }
            if (node.Nodes != null && recursive)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    UdateTreeNode(child, pbComp, type, true);
                }
            }
        }

        void ActionTreeAddChildren(GroupComposite ds, TreeNode node)
        {
            foreach (IPBComposite child in ds.Children)
            {
                TreeNode childNode = new TreeNode(child.Title);
                childNode.ForeColor = child.Color;
                childNode.Tag = child;
                // If, While and SubRoutine are Decorators.
                if (child is GroupComposite)
                {
                    ActionTreeAddChildren((GroupComposite)child, childNode);
                }
                node.Nodes.Add(childNode);
            }
        }

        void PopulateActionGridView()
        {
            ActionGridView.Rows.Clear();
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type type in asm.GetTypes())
            {
                if (type.GetInterface("IPBComposite") != null && !type.IsAbstract)
                {
                    IPBComposite pa = (IPBComposite)Activator.CreateInstance(type);
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = pa.Name;
                    row.Cells.Add(cell);
                    row.Tag = pa;
                    row.Height = 16;
                    ActionGridView.Rows.Add(row);
                    row.DefaultCellStyle.ForeColor = pa.Color;
                    //row.DefaultCellStyle.SelectionBackColor = pa.Color;
                }
            }
        }
        #endregion

        #region Callbacks
        void ActionTree_DragDrop(object sender, DragEventArgs e)
        {
            copyAction = CopyPasteOperactions.Cut;

            if ((e.KeyState & 4) > 0) // shift key
                copyAction |= CopyPasteOperactions.IgnoreRoot;
            if ((e.KeyState & 8) > 0) // ctrl key
                copyAction |= CopyPasteOperactions.Copy;

            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode dest = ((TreeView)sender).GetNodeAt(pt);
                TreeNode newNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                PasteAction(newNode, dest);
            }
            else if (e.Data.GetDataPresent("System.Windows.Forms.DataGridViewRow", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode dest = ((TreeView)sender).GetNodeAt(pt);
                DataGridViewRow row = (DataGridViewRow)e.Data.GetData("System.Windows.Forms.DataGridViewRow");
                if (row.Tag.GetType().GetInterface("IPBComposite") != null)
                {
                    IPBComposite pa = (IPBComposite)Activator.CreateInstance(row.Tag.GetType());
                    AddToActionTree(pa, dest);
                }
            }
        }

        void PasteAction(TreeNode source, TreeNode dest)
        {
            if (dest != source && (!IsChildNode(source, dest) || dest == null))
            {
                GroupComposite gc = (GroupComposite)((Composite)source.Tag).Parent;
                if ((copyAction & CopyPasteOperactions.Copy) != CopyPasteOperactions.Copy)
                    gc.Children.Remove((Composite)source.Tag);
                AddToActionTree(source, dest);
                if ((copyAction & CopyPasteOperactions.Copy) != CopyPasteOperactions.Copy) // ctrl key
                    source.Remove();
                copySource = null;// free any ref..
            }
        }

        void ActionTree_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void ActionTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        void ActionTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                ActionTree.SelectedNode = null;
                e.Handled = true;
            }
            else if (e.KeyData == Keys.Delete)
            {
                if (ActionTree.SelectedNode != null)
                    RemoveSelectedNodes();
            }
        }
        private void ActionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!IsValid)
                return;
            IPBComposite comp = (IPBComposite)e.Node.Tag;
            if (comp != null && comp.Properties != null)
            {
                MainForm.Instance.ActionGrid.SelectedObject = comp.Properties;
            }
        }

        public void OnTradeSkillsLoadedEventHandler(object sender, EventArgs e)
        {
            // must create GUI elements on its parent thread
            if (this.IsHandleCreated)
                this.BeginInvoke(new InitDelegate(Initialize));
            else
            {
                this.HandleCreated += MainForm_HandleCreated;
            }
            PB.OnTradeSkillsLoaded -= OnTradeSkillsLoadedEventHandler;
        }

        void MainForm_HandleCreated(object sender, EventArgs e)
        {
            this.BeginInvoke(new InitDelegate(Initialize));
            this.HandleCreated -= MainForm_HandleCreated;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!PB.IsTradeSkillsLoaded)
            {
                PB.OnTradeSkillsLoaded -= OnTradeSkillsLoadedEventHandler;
                PB.OnTradeSkillsLoaded += OnTradeSkillsLoadedEventHandler;
            }
            else
                Initialize();
            if (PB.CodeWasModified)
                PB.GenorateDynamicCode();
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            this.SuspendLayout();
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            this.ResumeLayout();
        }


        private void StartButton_Click(object sender, EventArgs e)
        {
            ToggleStart();
        }


        private void ActionGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (ActionGrid.SelectedObject is CastSpellAction && ((CastSpellAction)ActionGrid.SelectedObject).IsRecipe)
            {
                CastSpellAction ca = (CastSpellAction)ActionGrid.SelectedObject;
                PB.UpdateMaterials();
                RefreshTradeSkillTabs();
                RefreshActionTree(typeof(CastSpellAction));
            }
            else
            {
                ActionTree.SuspendLayout();
                UdateTreeNode(ActionTree.SelectedNode, null, null, false);
                ActionTree.ResumeLayout();
            }

            if (PB.CodeWasModified)
                PB.GenorateDynamicCode();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedNodes();
        }

        void RemoveSelectedNodes()
        {
            if (ActionTree.SelectedNode != null)
            {
                Composite comp = (Composite)ActionTree.SelectedNode.Tag;
                ((GroupComposite)comp.Parent).Children.Remove(comp);
                if (comp is CastSpellAction && ((CastSpellAction)comp).IsRecipe)
                {
                    PB.UpdateMaterials();
                    RefreshTradeSkillTabs();
                }
                if (ActionTree.SelectedNode.Parent != null)
                    ActionTree.SelectedNode.Parent.Nodes.RemoveAt(ActionTree.SelectedNode.Index);
                else
                    ActionTree.Nodes.RemoveAt(ActionTree.SelectedNode.Index);
            }
        }

        private void ActionGridView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ActionGridView.DoDragDrop(this.ActionGridView.CurrentRow, DragDropEffects.All);
            }
        }

        private void ActionGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (ActionGridView.SelectedRows != null && ActionGridView.SelectedRows.Count > 0)
                HelpTextBox.Text = ((IPBComposite)ActionGridView.SelectedRows[0].Tag).Help;
        }

        private void toolStripOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Professionbuddy.LoadProfile(openFileDialog.FileName);
                // check for a LoadProfileAction and load the profile to stop all the crying from the lazy noobs 
                if (PB.ProfileSettings.Settings.Count > 0)
                    toolStripSettings.Enabled = true;
            }
        }

        private void toolStripSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.FilterIndex = 1;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool zip = Path.GetExtension(saveFileDialog.FileName).Equals(".package", StringComparison.InvariantCultureIgnoreCase);
                // if we are saving to a zip check if CurrentProfile.XmlPath is not blank/null and use it if not. 
                // otherwise use the selected zipname with xml ext.
                string xmlfile = zip ? (string.IsNullOrEmpty(PB.CurrentProfile.XmlPath) ?
                    Path.ChangeExtension(saveFileDialog.FileName, ".xml") : PB.CurrentProfile.XmlPath)
                    : saveFileDialog.FileName;
                Professionbuddy.Log("Packaging profile to {0}", saveFileDialog.FileName);
                PB.CurrentProfile.SaveXml(xmlfile);
                if (zip)
                    PB.CurrentProfile.CreatePackage(saveFileDialog.FileName, xmlfile);
                PB.MySettings.LastProfile = saveFileDialog.FileName;
                PB.MySettings.Save();
                UpdateControls();
            }
        }

        private void toolStripAddBtn_Click(object sender, EventArgs e)
        {
            List<IPBComposite> compositeList = new List<IPBComposite>();
            // if the tradeskill tab is selected
            if (MainTabControl.SelectedTab == TradeSkillTab)
            {
                TradeSkillListView tv = TradeSkillTabControl.SelectedTab as TradeSkillListView;
                if (tv.TradeDataView.SelectedRows == null)
                    return;

                DataGridViewSelectedRowCollection rowCollection = tv.TradeDataView.SelectedRows;
                foreach (DataGridViewRow row in rowCollection)
                {
                    TradeSkillRecipeCell cell = (TradeSkillRecipeCell)row.Cells[0].Value;
                    Recipe recipe = PB.TradeSkillList[tv.TradeIndex].Recipes[cell.RecipeID];
                    int repeat;
                    int.TryParse(toolStripAddNum.Text, out repeat);
                    CastSpellAction.RepeatCalculationType repeatType = CastSpellAction.RepeatCalculationType.Specific;
                    switch (toolStripAddCombo.SelectedIndex)
                    {
                        case 1:
                            repeatType = CastSpellAction.RepeatCalculationType.Craftable;
                            break;
                        case 2:
                            repeatType = CastSpellAction.RepeatCalculationType.Banker;
                            break;
                    }
                    CastSpellAction ca = new CastSpellAction(recipe, repeat, repeatType);
                    compositeList.Add(ca);
                }
            }
            else if (MainTabControl.SelectedTab == ActionsTab)
            {
                if (ActionGridView.SelectedRows != null)
                {
                    foreach (DataGridViewRow row in ActionGridView.SelectedRows)
                    {
                        IPBComposite pa = (IPBComposite)Activator.CreateInstance(row.Tag.GetType());
                        compositeList.Add(pa);
                    }
                }
            }
            copyAction = CopyPasteOperactions.Copy;
            foreach (IPBComposite composite in compositeList)
            {
                if (ActionTree.SelectedNode == null)
                    AddToActionTree(composite, null);
                else
                    AddToActionTree(composite, ActionTree.SelectedNode);
            }
            // now update the CanRepeatCount. 
            PB.UpdateMaterials();
            RefreshTradeSkillTabs();
        }

        private void toolStripDelete_Click(object sender, EventArgs e)
        {
            RemoveSelectedNodes();
        }

        private void toolStripStart_Click(object sender, EventArgs e)
        {
            ToggleStart();
        }

        private void Materials_Click(object sender, EventArgs e)
        {
            new MaterialListForm().ShowDialog();
        }

        private void toolStripHelp_Click(object sender, EventArgs e)
        {
            Form helpWindow = new Form();
            helpWindow.Height = 600;
            helpWindow.Width = 600;
            helpWindow.Text = "ProfessionBuddy Guide";
            RichTextBox helpView = new RichTextBox();
            helpView.Dock = DockStyle.Fill;
            helpView.ReadOnly = true;
#if DLL
            helpView.Rtf = HighVoltz.Properties.Resources.Guide;
#else
            helpView.LoadFile(Path.Combine(PB.PluginPath, "Guide.rtf"));
#endif
            helpWindow.Controls.Add(helpView);
            helpWindow.Show();
        }

        private void toolStripCopy_Click(object sender, EventArgs e)
        {
            copySource = ActionTree.SelectedNode;
            if (copySource != null)
                copyAction = CopyPasteOperactions.Copy;
        }

        private void toolStripPaste_Click(object sender, EventArgs e)
        {
            if (copySource != null && ActionTree.SelectedNode != null)
                PasteAction(copySource, ActionTree.SelectedNode);
        }

        private void toolStripCut_Click(object sender, EventArgs e)
        {
            copySource = ActionTree.SelectedNode;
            if (copySource != null)
                copyAction = CopyPasteOperactions.Cut;
        }

        private void toolStripSecretButton_Click(object sender, EventArgs e)
        {
            PrioritySelector ps = TreeRoot.Current.Root as PrioritySelector;
            int n = 0;

            Logging.Write("** BotBase **");
            foreach (var p in ps.Children)
            {
                // add alternating amount of spaces to the end of log entries to prevent spam filter from blocking it
                n = (n + 1) % 2;
                Logging.Write("[{0}] {1}", p.GetType(), new string(' ', n));
            }

            //Logging.Write("** Profile Settings **");
            //foreach (var kv in PB.ProfileSettings.Settings)
            //    Logging.Write("{0} {1}", kv.Key, kv.Value);

            Logging.Write("** ActionSelector **");
            printComposite(PB.CurrentProfile.Branch, 0);

            //Logging.Write("** Material List **");
            //foreach (var kv in PB.MaterialList)
            //    Logging.Write("Ingredient ID: {0} Amount required:{1}", kv.Key, kv.Value);

            //Logging.Write("** DataStore **");
            //foreach (var kv in PB.DataStore)
            //    Logging.Write("item ID: {0} Amount in bag/bank/ah/alts:{1}", kv.Key, kv.Value);

            //if (PB.CsharpStringBuilder != null)
            //    Logging.Write(PB.CsharpStringBuilder.ToString());
        }

        void printComposite(Composite comp, int cnt)
        {
            string name;
            if (comp is IPBComposite)
                name = ((IPBComposite)comp).Title;
            else
                name = comp.GetType().ToString();
            if (typeof(IPBComposite).IsAssignableFrom(comp.GetType()))
                Logging.Write("{0}{1} IsDone:{2} LastStatus:{3}", new string(' ', cnt * 4), ((IPBComposite)comp).Title, ((IPBComposite)comp).IsDone, comp.LastStatus);
            if (comp is GroupComposite)
            {
                foreach (Composite child in ((GroupComposite)comp).Children)
                {
                    printComposite(child, cnt + 1);
                }
            }
        }

        private void toolStripSettings_Click(object sender, EventArgs e)
        {
            Form settingWindow = new Form();
            settingWindow.Height = 300;
            settingWindow.Width = 300;
            settingWindow.Text = "Profile Settings";
            PropertyGrid pg = new PropertyGrid();
            pg.Dock = DockStyle.Fill;
            settingWindow.Controls.Add(pg);

            ProfilePropertyBag = new PropertyBag();
            foreach (var kv in PB.ProfileSettings.Settings)
            {
                string sum = PB.ProfileSettings.Summaries.ContainsKey(kv.Key) ?
                    PB.ProfileSettings.Summaries[kv.Key] : "";
                ProfilePropertyBag[kv.Key] = new MetaProp(kv.Key, kv.Value.GetType(),
                    new DescriptionAttribute(sum));
                ProfilePropertyBag[kv.Key].Value = kv.Value;
                ProfilePropertyBag[kv.Key].PropertyChanged += new EventHandler(MainForm_PropertyChanged);
            }
            pg.SelectedObject = ProfilePropertyBag;
            toolStripSettings.Enabled = false;
            settingWindow.Show();
            settingWindow.Disposed += new EventHandler(settingWindow_Disposed);
        }

        void settingWindow_Disposed(object sender, EventArgs e)
        {
            // cleanup 
            foreach (var kv in PB.ProfileSettings.Settings)
            {
                ProfilePropertyBag[kv.Key].PropertyChanged -= MainForm_PropertyChanged;
            }
            ((Form)sender).Disposed -= settingWindow_Disposed;
            toolStripSettings.Enabled = true;
        }

        void MainForm_PropertyChanged(object sender, EventArgs e)
        {
            PB.ProfileSettings[((MetaProp)sender).Name] = ((MetaProp)sender).Value;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            PB.LoadTradeSkills();
            MainForm.Instance.InitTradeSkillTab();
        }

        void profileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            RefreshProfileList();
        }

        private void LoadProfileButton_Click(object sender, EventArgs e)
        {
            if (ProfileListView.SelectedItems != null && ProfileListView.SelectedItems.Count > 0)
            {
                Professionbuddy.LoadProfile(Path.Combine(PB.ProfilePath, ProfileListView.SelectedItems[0].Name));
                // check for a LoadProfileAction and load the profile to stop all the crying from the lazy noobs 
                if (PB.ProfileSettings.Settings.Count > 0)
                    toolStripSettings.Enabled = true;
            }
        }

    }
        #endregion

}