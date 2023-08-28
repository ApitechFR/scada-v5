﻿using Scada.Scheme.Editor.AppCode;
using System.Windows.Forms;

namespace Scada.Scheme.Editor
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Standard", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Pointer", "(none)");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Static Text", "(none)");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Dynamic Text", "(none)");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Static Picture", "(none)");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Dynamic Picture", "(none)");
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnFileNew = new System.Windows.Forms.ToolStripDropDownButton();
            this.newSymbolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFileOpen = new System.Windows.Forms.ToolStripButton();
            this.btnFileSave = new System.Windows.Forms.ToolStripSplitButton();
            this.miFileSaveAs2 = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveSymbol = new System.Windows.Forms.ToolStripSplitButton();
            this.miSaveSymbolAs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFileOpenBrowser = new System.Windows.Forms.ToolStripButton();
            this.sep1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnEditCut = new System.Windows.Forms.ToolStripButton();
            this.btnEditCopy = new System.Windows.Forms.ToolStripButton();
            this.btnEditPaste = new System.Windows.Forms.ToolStripButton();
            this.sep2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnEditUndo = new System.Windows.Forms.ToolStripButton();
            this.btnEditRedo = new System.Windows.Forms.ToolStripButton();
            this.sep3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnEditPointer = new System.Windows.Forms.ToolStripButton();
            this.btnEditDelete = new System.Windows.Forms.ToolStripButton();
            this.btnGroup = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.pageComponents = new System.Windows.Forms.TabPage();
            this.lvCompTypes = new System.Windows.Forms.ListView();
            this.colCompName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilCompTypes = new System.Windows.Forms.ImageList(this.components);
            this.pageProperties = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.cbSchComp = new System.Windows.Forms.ComboBox();
            this.ComponentsTree = new System.Windows.Forms.TabPage();
            this.treeView1 = new Scada.Scheme.Editor.TreeViewMultipleSelection();
            this.ofdScheme = new System.Windows.Forms.OpenFileDialog();
            this.sfdScheme = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpenBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSep = new System.Windows.Forms.ToolStripSeparator();
            this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditPasteSpecial = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.miEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.miEditPointer = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.miTools = new System.Windows.Forms.ToolStripMenuItem();
            this.miToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.pageComponents.SuspendLayout();
            this.pageProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.ComponentsTree.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFileNew,
            this.btnFileOpen,
            this.btnFileSave,
            this.miSaveSymbol,
            this.btnFileOpenBrowser,
            this.sep1,
            this.btnEditCut,
            this.btnEditCopy,
            this.btnEditPaste,
            this.sep2,
            this.btnEditUndo,
            this.btnEditRedo,
            this.sep3,
            this.btnEditPointer,
            this.btnEditDelete,
            this.btnGroup,
            this.toolStripButton2});
            this.toolStrip.Location = new System.Drawing.Point(0, 28);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(411, 27);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.MouseEnter += new System.EventHandler(this.FrmMain_MouseEnter);
            // 
            // btnFileNew
            // 
            this.btnFileNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFileNew.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSymbolToolStripMenuItem});
            this.btnFileNew.Image = ((System.Drawing.Image)(resources.GetObject("btnFileNew.Image")));
            this.btnFileNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFileNew.Name = "btnFileNew";
            this.btnFileNew.Size = new System.Drawing.Size(34, 24);
            this.btnFileNew.ToolTipText = "New scheme (Ctrl+N)";
            this.btnFileNew.Click += new System.EventHandler(this.miFileNew_Click);
            // 
            // newSymbolToolStripMenuItem
            // 
            this.newSymbolToolStripMenuItem.Image = global::Scada.Scheme.Editor.Properties.Resources.component;
            this.newSymbolToolStripMenuItem.Name = "newSymbolToolStripMenuItem";
            this.newSymbolToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.newSymbolToolStripMenuItem.Text = "New symbol";
            this.newSymbolToolStripMenuItem.Click += new System.EventHandler(this.newSymbolToolStripMenuItem_Click);
            // 
            // btnFileOpen
            // 
            this.btnFileOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnFileOpen.Image")));
            this.btnFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFileOpen.Name = "btnFileOpen";
            this.btnFileOpen.Size = new System.Drawing.Size(29, 24);
            this.btnFileOpen.ToolTipText = "Open scheme (Ctrl+O)";
            this.btnFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
            // 
            // btnFileSave
            // 
            this.btnFileSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFileSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileSaveAs2});
            this.btnFileSave.Image = ((System.Drawing.Image)(resources.GetObject("btnFileSave.Image")));
            this.btnFileSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFileSave.Name = "btnFileSave";
            this.btnFileSave.Size = new System.Drawing.Size(39, 24);
            this.btnFileSave.ToolTipText = "Save scheme (Ctrl+S)";
            this.btnFileSave.ButtonClick += new System.EventHandler(this.miFileSave_Click);
            // 
            // miFileSaveAs2
            // 
            this.miFileSaveAs2.Image = ((System.Drawing.Image)(resources.GetObject("miFileSaveAs2.Image")));
            this.miFileSaveAs2.Name = "miFileSaveAs2";
            this.miFileSaveAs2.Size = new System.Drawing.Size(152, 26);
            this.miFileSaveAs2.Text = "Save As...";
            this.miFileSaveAs2.Click += new System.EventHandler(this.miFileSaveAs_Click);
            // 
            // miSaveSymbol
            // 
            this.miSaveSymbol.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miSaveSymbol.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSaveSymbolAs});
            this.miSaveSymbol.Image = global::Scada.Scheme.Editor.Properties.Resources.component;
            this.miSaveSymbol.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miSaveSymbol.Name = "miSaveSymbol";
            this.miSaveSymbol.Size = new System.Drawing.Size(39, 24);
            this.miSaveSymbol.Text = "Save Symbol";
            this.miSaveSymbol.ButtonClick += new System.EventHandler(this.miSaveSymbol_ButtonClick);
            // 
            // miSaveSymbolAs
            // 
            this.miSaveSymbolAs.Image = global::Scada.Scheme.Editor.Properties.Resources.component;
            this.miSaveSymbolAs.Name = "miSaveSymbolAs";
            this.miSaveSymbolAs.Size = new System.Drawing.Size(224, 26);
            this.miSaveSymbolAs.Text = "Save Symbol As...";
            this.miSaveSymbolAs.Click += new System.EventHandler(this.miSaveSymbolAs_Click);
            // 
            // btnFileOpenBrowser
            // 
            this.btnFileOpenBrowser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFileOpenBrowser.Image = ((System.Drawing.Image)(resources.GetObject("btnFileOpenBrowser.Image")));
            this.btnFileOpenBrowser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFileOpenBrowser.Name = "btnFileOpenBrowser";
            this.btnFileOpenBrowser.Size = new System.Drawing.Size(29, 24);
            this.btnFileOpenBrowser.ToolTipText = "Open new browser tab";
            this.btnFileOpenBrowser.Click += new System.EventHandler(this.miFileOpenBrowser_Click);
            // 
            // sep1
            // 
            this.sep1.Name = "sep1";
            this.sep1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnEditCut
            // 
            this.btnEditCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditCut.Image = ((System.Drawing.Image)(resources.GetObject("btnEditCut.Image")));
            this.btnEditCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditCut.Name = "btnEditCut";
            this.btnEditCut.Size = new System.Drawing.Size(29, 24);
            this.btnEditCut.ToolTipText = "Cut scheme components (Ctrl+X)";
            this.btnEditCut.Click += new System.EventHandler(this.miEditCut_Click);
            // 
            // btnEditCopy
            // 
            this.btnEditCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnEditCopy.Image")));
            this.btnEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditCopy.Name = "btnEditCopy";
            this.btnEditCopy.Size = new System.Drawing.Size(29, 24);
            this.btnEditCopy.ToolTipText = "Copy scheme components (Ctrl+C)";
            this.btnEditCopy.Click += new System.EventHandler(this.miEditCopy_Click);
            // 
            // btnEditPaste
            // 
            this.btnEditPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditPaste.Image = ((System.Drawing.Image)(resources.GetObject("btnEditPaste.Image")));
            this.btnEditPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditPaste.Name = "btnEditPaste";
            this.btnEditPaste.Size = new System.Drawing.Size(29, 24);
            this.btnEditPaste.ToolTipText = "Paste scheme components (Ctrl+V)";
            this.btnEditPaste.Click += new System.EventHandler(this.miEditPaste_Click);
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnEditUndo
            // 
            this.btnEditUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnEditUndo.Image")));
            this.btnEditUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditUndo.Name = "btnEditUndo";
            this.btnEditUndo.Size = new System.Drawing.Size(29, 24);
            this.btnEditUndo.ToolTipText = "Undo (Ctrl+Z)";
            this.btnEditUndo.Click += new System.EventHandler(this.miEditUndo_Click);
            // 
            // btnEditRedo
            // 
            this.btnEditRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditRedo.Image = ((System.Drawing.Image)(resources.GetObject("btnEditRedo.Image")));
            this.btnEditRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditRedo.Name = "btnEditRedo";
            this.btnEditRedo.Size = new System.Drawing.Size(29, 24);
            this.btnEditRedo.ToolTipText = "Redo (Ctrl+Y)";
            this.btnEditRedo.Click += new System.EventHandler(this.miEditRedo_Click);
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Size = new System.Drawing.Size(6, 27);
            // 
            // btnEditPointer
            // 
            this.btnEditPointer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditPointer.Image = ((System.Drawing.Image)(resources.GetObject("btnEditPointer.Image")));
            this.btnEditPointer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditPointer.Name = "btnEditPointer";
            this.btnEditPointer.Size = new System.Drawing.Size(29, 24);
            this.btnEditPointer.ToolTipText = "Cancel adding component (Esc)";
            this.btnEditPointer.Click += new System.EventHandler(this.miEditPointer_Click);
            // 
            // btnEditDelete
            // 
            this.btnEditDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEditDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnEditDelete.Image")));
            this.btnEditDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditDelete.Name = "btnEditDelete";
            this.btnEditDelete.Size = new System.Drawing.Size(29, 24);
            this.btnEditDelete.ToolTipText = "Delete selected components (Del)";
            this.btnEditDelete.Click += new System.EventHandler(this.miEditDelete_Click);
            // 
            // btnGroup
            // 
            this.btnGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGroup.Image = global::Scada.Scheme.Editor.Properties.Resources.shape_square;
            this.btnGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Size = new System.Drawing.Size(29, 24);
            this.btnGroup.ToolTipText = "Group/Ungroup";
            this.btnGroup.Click += new System.EventHandler(this.miGroup_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Enabled = false;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.ToolTipText = "Alias Manager";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 554);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(411, 26);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.MouseEnter += new System.EventHandler(this.FrmMain_MouseEnter);
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(66, 20);
            this.lblStatus.Text = "lblStatus";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.pageComponents);
            this.tabControl.Controls.Add(this.pageProperties);
            this.tabControl.Controls.Add(this.ComponentsTree);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 55);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(411, 499);
            this.tabControl.TabIndex = 1;
            this.tabControl.MouseEnter += new System.EventHandler(this.FrmMain_MouseEnter);
            // 
            // pageComponents
            // 
            this.pageComponents.Controls.Add(this.lvCompTypes);
            this.pageComponents.Location = new System.Drawing.Point(4, 25);
            this.pageComponents.Margin = new System.Windows.Forms.Padding(4);
            this.pageComponents.Name = "pageComponents";
            this.pageComponents.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pageComponents.Size = new System.Drawing.Size(403, 470);
            this.pageComponents.TabIndex = 2;
            this.pageComponents.Text = "Components";
            this.pageComponents.UseVisualStyleBackColor = true;
            // 
            // lvCompTypes
            // 
            this.lvCompTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCompName});
            this.lvCompTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvCompTypes.FullRowSelect = true;
            listViewGroup1.Header = "Standard";
            listViewGroup1.Name = "lvgStandard";
            this.lvCompTypes.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1});
            this.lvCompTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvCompTypes.HideSelection = false;
            listViewItem1.Group = listViewGroup1;
            listViewItem1.IndentCount = 1;
            listViewItem2.Group = listViewGroup1;
            listViewItem2.IndentCount = 1;
            listViewItem2.Tag = "Scada.Scheme.Model.StaticText";
            listViewItem3.Group = listViewGroup1;
            listViewItem3.IndentCount = 1;
            listViewItem3.Tag = "Scada.Scheme.Model.DynamicText";
            listViewItem4.Group = listViewGroup1;
            listViewItem4.IndentCount = 1;
            listViewItem4.Tag = "Scada.Scheme.Model.StaticPicture";
            listViewItem5.Group = listViewGroup1;
            listViewItem5.IndentCount = 1;
            listViewItem5.Tag = "Scada.Scheme.Model.DynamicPicture";
            this.lvCompTypes.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.lvCompTypes.LabelWrap = false;
            this.lvCompTypes.Location = new System.Drawing.Point(3, 2);
            this.lvCompTypes.Margin = new System.Windows.Forms.Padding(4);
            this.lvCompTypes.MultiSelect = false;
            this.lvCompTypes.Name = "lvCompTypes";
            this.lvCompTypes.Size = new System.Drawing.Size(397, 466);
            this.lvCompTypes.SmallImageList = this.ilCompTypes;
            this.lvCompTypes.TabIndex = 0;
            this.lvCompTypes.UseCompatibleStateImageBehavior = false;
            this.lvCompTypes.View = System.Windows.Forms.View.Details;
            this.lvCompTypes.SelectedIndexChanged += new System.EventHandler(this.lvCompTypes_SelectedIndexChanged);
            // 
            // colCompName
            // 
            this.colCompName.Width = 250;
            // 
            // ilCompTypes
            // 
            this.ilCompTypes.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilCompTypes.ImageSize = new System.Drawing.Size(16, 16);
            this.ilCompTypes.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pageProperties
            // 
            this.pageProperties.Controls.Add(this.splitContainer1);
            this.pageProperties.Controls.Add(this.propertyGrid);
            this.pageProperties.Controls.Add(this.cbSchComp);
            this.pageProperties.Location = new System.Drawing.Point(4, 25);
            this.pageProperties.Margin = new System.Windows.Forms.Padding(4);
            this.pageProperties.Name = "pageProperties";
            this.pageProperties.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pageProperties.Size = new System.Drawing.Size(403, 470);
            this.pageProperties.TabIndex = 0;
            this.pageProperties.Text = "Properties";
            this.pageProperties.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 26);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(397, 442);
            this.splitContainer1.SplitterDistance = 404;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Size = new System.Drawing.Size(397, 404);
            this.splitContainer2.SplitterDistance = 29;
            this.splitContainer2.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(397, 34);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Enabled = false;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(29, 31);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Link to a symbol property";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid.Location = new System.Drawing.Point(3, 26);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(397, 442);
            this.propertyGrid.TabIndex = 1;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            this.propertyGrid.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGrid_SelectedGridItemChanged);
            // 
            // cbSchComp
            // 
            this.cbSchComp.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbSchComp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSchComp.FormattingEnabled = true;
            this.cbSchComp.Location = new System.Drawing.Point(3, 2);
            this.cbSchComp.Margin = new System.Windows.Forms.Padding(4);
            this.cbSchComp.Name = "cbSchComp";
            this.cbSchComp.Size = new System.Drawing.Size(397, 24);
            this.cbSchComp.TabIndex = 0;
            this.cbSchComp.SelectedIndexChanged += new System.EventHandler(this.cbSchComp_SelectedIndexChanged);
            // 
            // ComponentsTree
            // 
            this.ComponentsTree.Controls.Add(this.treeView1);
            this.ComponentsTree.Location = new System.Drawing.Point(4, 25);
            this.ComponentsTree.Margin = new System.Windows.Forms.Padding(4);
            this.ComponentsTree.Name = "ComponentsTree";
            this.ComponentsTree.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ComponentsTree.Size = new System.Drawing.Size(403, 470);
            this.ComponentsTree.TabIndex = 3;
            this.ComponentsTree.Text = "View";
            this.ComponentsTree.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.White;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("treeView1.SelectedNodes")));
            this.treeView1.Size = new System.Drawing.Size(433, 508);
            this.treeView1.Sorted = true;
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_onNodeSelection);
            // 
            // ofdScheme
            // 
            this.ofdScheme.DefaultExt = "*.sch";
            this.ofdScheme.Filter = "Schemes (*.sch)|*.sch|All Files (*.*)|*.*";
            // 
            // sfdScheme
            // 
            this.sfdScheme.DefaultExt = "*.sch";
            this.sfdScheme.Filter = "Schemes (*.sch)|*.sch|All Files (*.*)|*.*";
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miEdit,
            this.miTools,
            this.miHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(411, 28);
            this.menuStrip.TabIndex = 3;
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileNew,
            this.miFileOpen,
            this.miFileSave,
            this.miFileSaveAs,
            this.miFileOpenBrowser,
            this.miFileSep,
            this.miFileExit});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(46, 24);
            this.miFile.Text = "&File";
            // 
            // miFileNew
            // 
            this.miFileNew.Image = ((System.Drawing.Image)(resources.GetObject("miFileNew.Image")));
            this.miFileNew.Name = "miFileNew";
            this.miFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.miFileNew.Size = new System.Drawing.Size(190, 26);
            this.miFileNew.Text = "New";
            this.miFileNew.Click += new System.EventHandler(this.miFileNew_Click);
            // 
            // miFileOpen
            // 
            this.miFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("miFileOpen.Image")));
            this.miFileOpen.Name = "miFileOpen";
            this.miFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.miFileOpen.Size = new System.Drawing.Size(190, 26);
            this.miFileOpen.Text = "Open...";
            this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
            // 
            // miFileSave
            // 
            this.miFileSave.Image = ((System.Drawing.Image)(resources.GetObject("miFileSave.Image")));
            this.miFileSave.Name = "miFileSave";
            this.miFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miFileSave.Size = new System.Drawing.Size(190, 26);
            this.miFileSave.Text = "Save";
            this.miFileSave.Click += new System.EventHandler(this.miFileSave_Click);
            // 
            // miFileSaveAs
            // 
            this.miFileSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("miFileSaveAs.Image")));
            this.miFileSaveAs.Name = "miFileSaveAs";
            this.miFileSaveAs.Size = new System.Drawing.Size(190, 26);
            this.miFileSaveAs.Text = "Save As...";
            this.miFileSaveAs.Click += new System.EventHandler(this.miFileSaveAs_Click);
            // 
            // miFileOpenBrowser
            // 
            this.miFileOpenBrowser.Image = ((System.Drawing.Image)(resources.GetObject("miFileOpenBrowser.Image")));
            this.miFileOpenBrowser.Name = "miFileOpenBrowser";
            this.miFileOpenBrowser.Size = new System.Drawing.Size(190, 26);
            this.miFileOpenBrowser.Text = "Open Browser";
            this.miFileOpenBrowser.Click += new System.EventHandler(this.miFileOpenBrowser_Click);
            // 
            // miFileSep
            // 
            this.miFileSep.Name = "miFileSep";
            this.miFileSep.Size = new System.Drawing.Size(187, 6);
            // 
            // miFileExit
            // 
            this.miFileExit.Image = ((System.Drawing.Image)(resources.GetObject("miFileExit.Image")));
            this.miFileExit.Name = "miFileExit";
            this.miFileExit.Size = new System.Drawing.Size(190, 26);
            this.miFileExit.Text = "Exit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // miEdit
            // 
            this.miEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miEditCut,
            this.miEditCopy,
            this.miEditPaste,
            this.miEditPasteSpecial,
            this.miEditSep1,
            this.miEditUndo,
            this.miEditRedo,
            this.miEditSep2,
            this.miEditPointer,
            this.miEditDelete});
            this.miEdit.Name = "miEdit";
            this.miEdit.Size = new System.Drawing.Size(49, 24);
            this.miEdit.Text = "&Edit";
            // 
            // miEditCut
            // 
            this.miEditCut.Image = ((System.Drawing.Image)(resources.GetObject("miEditCut.Image")));
            this.miEditCut.Name = "miEditCut";
            this.miEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.miEditCut.Size = new System.Drawing.Size(187, 26);
            this.miEditCut.Text = "Cut";
            this.miEditCut.Click += new System.EventHandler(this.miEditCut_Click);
            // 
            // miEditCopy
            // 
            this.miEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("miEditCopy.Image")));
            this.miEditCopy.Name = "miEditCopy";
            this.miEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.miEditCopy.Size = new System.Drawing.Size(187, 26);
            this.miEditCopy.Text = "Copy";
            this.miEditCopy.Click += new System.EventHandler(this.miEditCopy_Click);
            // 
            // miEditPaste
            // 
            this.miEditPaste.Image = ((System.Drawing.Image)(resources.GetObject("miEditPaste.Image")));
            this.miEditPaste.Name = "miEditPaste";
            this.miEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.miEditPaste.Size = new System.Drawing.Size(187, 26);
            this.miEditPaste.Text = "Paste";
            this.miEditPaste.Click += new System.EventHandler(this.miEditPaste_Click);
            // 
            // miEditPasteSpecial
            // 
            this.miEditPasteSpecial.Name = "miEditPasteSpecial";
            this.miEditPasteSpecial.Size = new System.Drawing.Size(187, 26);
            this.miEditPasteSpecial.Text = "Paste Special...";
            this.miEditPasteSpecial.Click += new System.EventHandler(this.miEditPasteSpecial_Click);
            // 
            // miEditSep1
            // 
            this.miEditSep1.Name = "miEditSep1";
            this.miEditSep1.Size = new System.Drawing.Size(184, 6);
            // 
            // miEditUndo
            // 
            this.miEditUndo.Image = ((System.Drawing.Image)(resources.GetObject("miEditUndo.Image")));
            this.miEditUndo.Name = "miEditUndo";
            this.miEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.miEditUndo.Size = new System.Drawing.Size(187, 26);
            this.miEditUndo.Text = "Undo";
            this.miEditUndo.Click += new System.EventHandler(this.miEditUndo_Click);
            // 
            // miEditRedo
            // 
            this.miEditRedo.Image = ((System.Drawing.Image)(resources.GetObject("miEditRedo.Image")));
            this.miEditRedo.Name = "miEditRedo";
            this.miEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.miEditRedo.Size = new System.Drawing.Size(187, 26);
            this.miEditRedo.Text = "Redo";
            this.miEditRedo.Click += new System.EventHandler(this.miEditRedo_Click);
            // 
            // miEditSep2
            // 
            this.miEditSep2.Name = "miEditSep2";
            this.miEditSep2.Size = new System.Drawing.Size(184, 6);
            // 
            // miEditPointer
            // 
            this.miEditPointer.Image = ((System.Drawing.Image)(resources.GetObject("miEditPointer.Image")));
            this.miEditPointer.Name = "miEditPointer";
            this.miEditPointer.Size = new System.Drawing.Size(187, 26);
            this.miEditPointer.Text = "Pointer";
            this.miEditPointer.Click += new System.EventHandler(this.miEditPointer_Click);
            // 
            // miEditDelete
            // 
            this.miEditDelete.Image = ((System.Drawing.Image)(resources.GetObject("miEditDelete.Image")));
            this.miEditDelete.Name = "miEditDelete";
            this.miEditDelete.Size = new System.Drawing.Size(187, 26);
            this.miEditDelete.Text = "Delete";
            this.miEditDelete.Click += new System.EventHandler(this.miEditDelete_Click);
            // 
            // miTools
            // 
            this.miTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miToolsOptions});
            this.miTools.Name = "miTools";
            this.miTools.Size = new System.Drawing.Size(58, 24);
            this.miTools.Text = "&Tools";
            // 
            // miToolsOptions
            // 
            this.miToolsOptions.Image = ((System.Drawing.Image)(resources.GetObject("miToolsOptions.Image")));
            this.miToolsOptions.Name = "miToolsOptions";
            this.miToolsOptions.Size = new System.Drawing.Size(153, 26);
            this.miToolsOptions.Text = "Options...";
            this.miToolsOptions.Click += new System.EventHandler(this.miToolsOptions_Click);
            // 
            // miHelp
            // 
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHelpAbout});
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(55, 24);
            this.miHelp.Text = "&Help";
            // 
            // miHelpAbout
            // 
            this.miHelpAbout.Image = ((System.Drawing.Image)(resources.GetObject("miHelpAbout.Image")));
            this.miHelpAbout.Name = "miHelpAbout";
            this.miHelpAbout.Size = new System.Drawing.Size(133, 26);
            this.miHelpAbout.Text = "About";
            this.miHelpAbout.Click += new System.EventHandler(this.miHelpAbout_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 580);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(261, 355);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Scheme Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
            this.MouseEnter += new System.EventHandler(this.FrmMain_MouseEnter);
            this.Move += new System.EventHandler(this.FrmMain_Move);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.pageComponents.ResumeLayout(false);
            this.pageProperties.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ComponentsTree.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnFileOpen;
        private System.Windows.Forms.ToolStripButton btnEditCut;
        private System.Windows.Forms.ToolStripButton btnEditCopy;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage pageProperties;
        private System.Windows.Forms.ComboBox cbSchComp;
        private System.Windows.Forms.TabPage pageComponents;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripSeparator sep1;
        private System.Windows.Forms.ToolStripSplitButton btnFileSave;
        private System.Windows.Forms.ToolStripMenuItem miFileSaveAs2;
        private System.Windows.Forms.ToolStripButton btnEditPaste;
        private System.Windows.Forms.ToolStripSeparator sep2;
        private System.Windows.Forms.ToolStripButton btnEditPointer;
        private System.Windows.Forms.ToolStripButton btnEditDelete;
        private System.Windows.Forms.ToolStripSeparator sep3;
        private System.Windows.Forms.ToolStripButton btnFileOpenBrowser;
        private System.Windows.Forms.ToolStripButton btnEditUndo;
        private System.Windows.Forms.ToolStripButton btnEditRedo;
        private System.Windows.Forms.OpenFileDialog ofdScheme;
        private System.Windows.Forms.SaveFileDialog sfdScheme;
        private System.Windows.Forms.ListView lvCompTypes;
        private System.Windows.Forms.ImageList ilCompTypes;
        private System.Windows.Forms.ColumnHeader colCompName;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem miFile;
        private System.Windows.Forms.ToolStripMenuItem miFileNew;
        private System.Windows.Forms.ToolStripMenuItem miFileOpen;
        private System.Windows.Forms.ToolStripMenuItem miFileSave;
        private System.Windows.Forms.ToolStripMenuItem miFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator miFileSep;
        private System.Windows.Forms.ToolStripMenuItem miFileExit;
        private System.Windows.Forms.ToolStripMenuItem miEdit;
        private System.Windows.Forms.ToolStripMenuItem miTools;
        private System.Windows.Forms.ToolStripMenuItem miHelp;
        private System.Windows.Forms.ToolStripMenuItem miEditCut;
        private System.Windows.Forms.ToolStripMenuItem miEditCopy;
        private System.Windows.Forms.ToolStripMenuItem miEditPaste;
        private System.Windows.Forms.ToolStripSeparator miEditSep1;
        private System.Windows.Forms.ToolStripMenuItem miEditUndo;
        private System.Windows.Forms.ToolStripMenuItem miEditRedo;
        private System.Windows.Forms.ToolStripSeparator miEditSep2;
        private System.Windows.Forms.ToolStripMenuItem miEditPointer;
        private System.Windows.Forms.ToolStripMenuItem miEditDelete;
        private System.Windows.Forms.ToolStripMenuItem miToolsOptions;
        private System.Windows.Forms.ToolStripMenuItem miHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem miFileOpenBrowser;
        private System.Windows.Forms.ToolStripMenuItem miEditPasteSpecial;
        private System.Windows.Forms.TabPage ComponentsTree;
        private ToolStripButton btnGroup;
        private TreeViewMultipleSelection treeView1;
        private ToolStrip toolStrip1;
        private PropertyGrid propertyGrid;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer1;
        private ToolStripButton toolStripButton1;
        private ToolStripSplitButton miSaveSymbol;
        private ToolStripMenuItem miSaveSymbolAs;
        private ToolStripDropDownButton btnFileNew;
        private ToolStripMenuItem newSymbolToolStripMenuItem;
        private ToolStripButton toolStripButton2;
    }
}

