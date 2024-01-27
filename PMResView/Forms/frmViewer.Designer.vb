<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmViewer
  Inherits System.Windows.Forms.Form

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Me.components = New System.ComponentModel.Container()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewer))
    Me.mnuMain = New System.Windows.Forms.MainMenu(Me.components)
    Me.mnuFile = New System.Windows.Forms.MenuItem()
    Me.mnuFileOpenArchive = New System.Windows.Forms.MenuItem()
    Me.mnuFileCloseArchive = New System.Windows.Forms.MenuItem()
    Me.mnuFileSpace1 = New System.Windows.Forms.MenuItem()
    Me.mnuFileOpenFile = New System.Windows.Forms.MenuItem()
    Me.mnuFileExtractFile = New System.Windows.Forms.MenuItem()
    Me.mnuFilePropertiesFile = New System.Windows.Forms.MenuItem()
    Me.mnuFileSpace2 = New System.Windows.Forms.MenuItem()
    Me.mnuFileExit = New System.Windows.Forms.MenuItem()
    Me.mnuEdit = New System.Windows.Forms.MenuItem()
    Me.mnuEditAll = New System.Windows.Forms.MenuItem()
    Me.mnuEditNone = New System.Windows.Forms.MenuItem()
    Me.mnuEditInvert = New System.Windows.Forms.MenuItem()
    Me.mnuEditSelect = New System.Windows.Forms.MenuItem()
    Me.mnuEditDeselect = New System.Windows.Forms.MenuItem()
    Me.mnuEditSpace = New System.Windows.Forms.MenuItem()
    Me.mnuEditSelType = New System.Windows.Forms.MenuItem()
    Me.mnuEditDeselType = New System.Windows.Forms.MenuItem()
    Me.mnuView = New System.Windows.Forms.MenuItem()
    Me.mnuViewIcons = New System.Windows.Forms.MenuItem()
    Me.mnuViewIconsLarge = New System.Windows.Forms.MenuItem()
    Me.mnuViewIconsSmall = New System.Windows.Forms.MenuItem()
    Me.mnuViewIconsList = New System.Windows.Forms.MenuItem()
    Me.mnuViewIconsDetails = New System.Windows.Forms.MenuItem()
    Me.mnuViewIconsTile = New System.Windows.Forms.MenuItem()
    Me.mnuViewSort = New System.Windows.Forms.MenuItem()
    Me.mnuViewSortName = New System.Windows.Forms.MenuItem()
    Me.mnuViewSortType = New System.Windows.Forms.MenuItem()
    Me.mnuViewSortSize = New System.Windows.Forms.MenuItem()
    Me.mnuViewSortOrder = New System.Windows.Forms.MenuItem()
    Me.mnuViewSpace1 = New System.Windows.Forms.MenuItem()
    Me.mnuViewToolbar = New System.Windows.Forms.MenuItem()
    Me.mnuViewFlat = New System.Windows.Forms.MenuItem()
    Me.mnuViewTree = New System.Windows.Forms.MenuItem()
    Me.mnuViewSpace2 = New System.Windows.Forms.MenuItem()
    Me.mnuViewGoRoot = New System.Windows.Forms.MenuItem()
    Me.mnuViewGoParent = New System.Windows.Forms.MenuItem()
    Me.mnuHelp = New System.Windows.Forms.MenuItem()
    Me.mnuHelpAbout = New System.Windows.Forms.MenuItem()
    Me.pnlMain = New System.Windows.Forms.SplitContainer()
    Me.tvExplorer = New System.Windows.Forms.TreeView()
    Me.imlFolderTree = New System.Windows.Forms.ImageList(Me.components)
    Me.lvFiles = New PMResView.ListViewNoHScroll()
    Me.colName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
    Me.colType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
    Me.colSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
    Me.colCompression = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
    Me.imlFileList32 = New System.Windows.Forms.ImageList(Me.components)
    Me.imlFileList = New System.Windows.Forms.ImageList(Me.components)
    Me.stsInfo = New System.Windows.Forms.StatusStrip()
    Me.lblSelCount = New System.Windows.Forms.ToolStripStatusLabel()
    Me.lblSelUncompressed = New System.Windows.Forms.ToolStripStatusLabel()
    Me.lblSelSize = New System.Windows.Forms.ToolStripStatusLabel()
    Me.pbActivity = New System.Windows.Forms.ToolStripProgressBar()
    Me.mnuContextFile = New System.Windows.Forms.ContextMenu()
    Me.mnuContextFileOpen = New System.Windows.Forms.MenuItem()
    Me.mnuContextFileExtract = New System.Windows.Forms.MenuItem()
    Me.mnuContextFileSpace = New System.Windows.Forms.MenuItem()
    Me.mnuContextFileProperties = New System.Windows.Forms.MenuItem()
    Me.mnuContextView = New System.Windows.Forms.ContextMenu()
    Me.mnuContextViewIcons = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewIconsLarge = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewIconsSmall = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewIconsList = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewIconsDetails = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewIconsTile = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewSort = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewSortName = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewSortType = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewSortSize = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewSortOrder = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewSpace1 = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewFlat = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewTree = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewSpace2 = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewGoRoot = New System.Windows.Forms.MenuItem()
    Me.mnuContextViewGoParent = New System.Windows.Forms.MenuItem()
    Me.pnlUI = New System.Windows.Forms.ToolStripContainer()
    Me.tbNav = New System.Windows.Forms.ToolStrip()
    Me.cmdBack = New System.Windows.Forms.ToolStripButton()
    Me.cmdForward = New System.Windows.Forms.ToolStripButton()
    Me.cmdUp = New System.Windows.Forms.ToolStripButton()
    Me.sepSpace1 = New System.Windows.Forms.ToolStripSeparator()
    Me.lblAddress = New System.Windows.Forms.ToolStripLabel()
    Me.txtAddress = New System.Windows.Forms.ToolStripTextBox()
    Me.cmdGo = New System.Windows.Forms.ToolStripButton()
    Me.sepSpace2 = New System.Windows.Forms.ToolStripSeparator()
    Me.cmdExtract = New System.Windows.Forms.ToolStripButton()
    Me.cmdProperties = New System.Windows.Forms.ToolStripButton()
    Me.mnuContextHistory = New System.Windows.Forms.ContextMenu()
    Me.tmrSelectionUpdate = New System.Windows.Forms.Timer(Me.components)
    CType(Me.pnlMain, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.pnlMain.Panel1.SuspendLayout()
    Me.pnlMain.Panel2.SuspendLayout()
    Me.pnlMain.SuspendLayout()
    Me.stsInfo.SuspendLayout()
    Me.pnlUI.BottomToolStripPanel.SuspendLayout()
    Me.pnlUI.ContentPanel.SuspendLayout()
    Me.pnlUI.TopToolStripPanel.SuspendLayout()
    Me.pnlUI.SuspendLayout()
    Me.tbNav.SuspendLayout()
    Me.SuspendLayout()
    '
    'mnuMain
    '
    Me.mnuMain.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFile, Me.mnuEdit, Me.mnuView, Me.mnuHelp})
    '
    'mnuFile
    '
    Me.mnuFile.Index = 0
    Me.mnuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFileOpenArchive, Me.mnuFileCloseArchive, Me.mnuFileSpace1, Me.mnuFileOpenFile, Me.mnuFileExtractFile, Me.mnuFilePropertiesFile, Me.mnuFileSpace2, Me.mnuFileExit})
    Me.mnuFile.Text = "&File"
    '
    'mnuFileOpenArchive
    '
    Me.mnuFileOpenArchive.Index = 0
    Me.mnuFileOpenArchive.Shortcut = System.Windows.Forms.Shortcut.CtrlO
    Me.mnuFileOpenArchive.Text = "&Open Archive..."
    '
    'mnuFileCloseArchive
    '
    Me.mnuFileCloseArchive.Enabled = False
    Me.mnuFileCloseArchive.Index = 1
    Me.mnuFileCloseArchive.Shortcut = System.Windows.Forms.Shortcut.CtrlW
    Me.mnuFileCloseArchive.Text = "&Close Archive"
    '
    'mnuFileSpace1
    '
    Me.mnuFileSpace1.Index = 2
    Me.mnuFileSpace1.Text = "-"
    '
    'mnuFileOpenFile
    '
    Me.mnuFileOpenFile.Enabled = False
    Me.mnuFileOpenFile.Index = 3
    Me.mnuFileOpenFile.Text = "Open Selected &File(s)"
    '
    'mnuFileExtractFile
    '
    Me.mnuFileExtractFile.Enabled = False
    Me.mnuFileExtractFile.Index = 4
    Me.mnuFileExtractFile.Shortcut = System.Windows.Forms.Shortcut.CtrlE
    Me.mnuFileExtractFile.Text = "&Extract Selected File(s)..."
    '
    'mnuFilePropertiesFile
    '
    Me.mnuFilePropertiesFile.Enabled = False
    Me.mnuFilePropertiesFile.Index = 5
    Me.mnuFilePropertiesFile.Text = "Selected File &Properties..."
    '
    'mnuFileSpace2
    '
    Me.mnuFileSpace2.Index = 6
    Me.mnuFileSpace2.Text = "-"
    '
    'mnuFileExit
    '
    Me.mnuFileExit.Index = 7
    Me.mnuFileExit.Shortcut = System.Windows.Forms.Shortcut.AltF4
    Me.mnuFileExit.Text = "E&xit"
    '
    'mnuEdit
    '
    Me.mnuEdit.Index = 1
    Me.mnuEdit.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuEditAll, Me.mnuEditNone, Me.mnuEditInvert, Me.mnuEditSelect, Me.mnuEditDeselect, Me.mnuEditSpace, Me.mnuEditSelType, Me.mnuEditDeselType})
    Me.mnuEdit.Text = "&Edit"
    '
    'mnuEditAll
    '
    Me.mnuEditAll.Enabled = False
    Me.mnuEditAll.Index = 0
    Me.mnuEditAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA
    Me.mnuEditAll.Text = "Select &All"
    '
    'mnuEditNone
    '
    Me.mnuEditNone.Enabled = False
    Me.mnuEditNone.Index = 1
    Me.mnuEditNone.Shortcut = System.Windows.Forms.Shortcut.ShiftDel
    Me.mnuEditNone.Text = "&Deselect All"
    '
    'mnuEditInvert
    '
    Me.mnuEditInvert.Enabled = False
    Me.mnuEditInvert.Index = 2
    Me.mnuEditInvert.Text = "&Invert Selection"
    '
    'mnuEditSelect
    '
    Me.mnuEditSelect.Enabled = False
    Me.mnuEditSelect.Index = 3
    Me.mnuEditSelect.Text = "&Select..."
    '
    'mnuEditDeselect
    '
    Me.mnuEditDeselect.Enabled = False
    Me.mnuEditDeselect.Index = 4
    Me.mnuEditDeselect.Text = "&Deselect..."
    '
    'mnuEditSpace
    '
    Me.mnuEditSpace.Index = 5
    Me.mnuEditSpace.Text = "-"
    '
    'mnuEditSelType
    '
    Me.mnuEditSelType.Enabled = False
    Me.mnuEditSelType.Index = 6
    Me.mnuEditSelType.Text = "Select by &Type"
    '
    'mnuEditDeselType
    '
    Me.mnuEditDeselType.Enabled = False
    Me.mnuEditDeselType.Index = 7
    Me.mnuEditDeselType.Text = "Deselect by T&ype"
    '
    'mnuView
    '
    Me.mnuView.Index = 2
    Me.mnuView.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuViewIcons, Me.mnuViewSort, Me.mnuViewSpace1, Me.mnuViewToolbar, Me.mnuViewFlat, Me.mnuViewTree, Me.mnuViewSpace2, Me.mnuViewGoRoot, Me.mnuViewGoParent})
    Me.mnuView.Text = "&View"
    '
    'mnuViewIcons
    '
    Me.mnuViewIcons.Index = 0
    Me.mnuViewIcons.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuViewIconsLarge, Me.mnuViewIconsSmall, Me.mnuViewIconsList, Me.mnuViewIconsDetails, Me.mnuViewIconsTile})
    Me.mnuViewIcons.Text = "&Icons"
    '
    'mnuViewIconsLarge
    '
    Me.mnuViewIconsLarge.Index = 0
    Me.mnuViewIconsLarge.RadioCheck = True
    Me.mnuViewIconsLarge.Text = "&Large"
    '
    'mnuViewIconsSmall
    '
    Me.mnuViewIconsSmall.Index = 1
    Me.mnuViewIconsSmall.RadioCheck = True
    Me.mnuViewIconsSmall.Text = "&Small"
    '
    'mnuViewIconsList
    '
    Me.mnuViewIconsList.Index = 2
    Me.mnuViewIconsList.RadioCheck = True
    Me.mnuViewIconsList.Text = "&List"
    '
    'mnuViewIconsDetails
    '
    Me.mnuViewIconsDetails.Checked = True
    Me.mnuViewIconsDetails.Index = 3
    Me.mnuViewIconsDetails.RadioCheck = True
    Me.mnuViewIconsDetails.Text = "&Details"
    '
    'mnuViewIconsTile
    '
    Me.mnuViewIconsTile.Index = 4
    Me.mnuViewIconsTile.RadioCheck = True
    Me.mnuViewIconsTile.Text = "&Tiles"
    '
    'mnuViewSort
    '
    Me.mnuViewSort.Index = 1
    Me.mnuViewSort.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuViewSortName, Me.mnuViewSortType, Me.mnuViewSortSize, Me.mnuViewSortOrder})
    Me.mnuViewSort.Text = "&Sort"
    '
    'mnuViewSortName
    '
    Me.mnuViewSortName.Index = 0
    Me.mnuViewSortName.RadioCheck = True
    Me.mnuViewSortName.Text = "by &Name"
    '
    'mnuViewSortType
    '
    Me.mnuViewSortType.Index = 1
    Me.mnuViewSortType.RadioCheck = True
    Me.mnuViewSortType.Text = "by &Type"
    '
    'mnuViewSortSize
    '
    Me.mnuViewSortSize.Index = 2
    Me.mnuViewSortSize.RadioCheck = True
    Me.mnuViewSortSize.Text = "by &Size"
    '
    'mnuViewSortOrder
    '
    Me.mnuViewSortOrder.Checked = True
    Me.mnuViewSortOrder.Index = 3
    Me.mnuViewSortOrder.RadioCheck = True
    Me.mnuViewSortOrder.Text = "by &Order"
    '
    'mnuViewSpace1
    '
    Me.mnuViewSpace1.Index = 2
    Me.mnuViewSpace1.Text = "-"
    '
    'mnuViewToolbar
    '
    Me.mnuViewToolbar.Checked = True
    Me.mnuViewToolbar.Index = 3
    Me.mnuViewToolbar.Text = "T&oolbar"
    '
    'mnuViewFlat
    '
    Me.mnuViewFlat.Index = 4
    Me.mnuViewFlat.Text = "&Flat View"
    '
    'mnuViewTree
    '
    Me.mnuViewTree.Checked = True
    Me.mnuViewTree.Index = 5
    Me.mnuViewTree.Shortcut = System.Windows.Forms.Shortcut.CtrlE
    Me.mnuViewTree.Text = "&Tree View"
    '
    'mnuViewSpace2
    '
    Me.mnuViewSpace2.Index = 6
    Me.mnuViewSpace2.Text = "-"
    '
    'mnuViewGoRoot
    '
    Me.mnuViewGoRoot.Enabled = False
    Me.mnuViewGoRoot.Index = 7
    Me.mnuViewGoRoot.Shortcut = System.Windows.Forms.Shortcut.CtrlH
    Me.mnuViewGoRoot.Text = "Go to &Root"
    '
    'mnuViewGoParent
    '
    Me.mnuViewGoParent.Enabled = False
    Me.mnuViewGoParent.Index = 8
    Me.mnuViewGoParent.Shortcut = System.Windows.Forms.Shortcut.AltUpArrow
    Me.mnuViewGoParent.Text = "Go to &Parent"
    '
    'mnuHelp
    '
    Me.mnuHelp.Index = 3
    Me.mnuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuHelpAbout})
    Me.mnuHelp.Text = "&Help"
    '
    'mnuHelpAbout
    '
    Me.mnuHelpAbout.Index = 0
    Me.mnuHelpAbout.Text = "&About"
    '
    'pnlMain
    '
    Me.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill
    Me.pnlMain.Location = New System.Drawing.Point(0, 0)
    Me.pnlMain.Name = "pnlMain"
    '
    'pnlMain.Panel1
    '
    Me.pnlMain.Panel1.Controls.Add(Me.tvExplorer)
    Me.pnlMain.Panel1MinSize = 120
    '
    'pnlMain.Panel2
    '
    Me.pnlMain.Panel2.Controls.Add(Me.lvFiles)
    Me.pnlMain.Panel2MinSize = 200
    Me.pnlMain.Size = New System.Drawing.Size(704, 279)
    Me.pnlMain.SplitterDistance = 200
    Me.pnlMain.TabIndex = 0
    '
    'tvExplorer
    '
    Me.tvExplorer.Dock = System.Windows.Forms.DockStyle.Fill
    Me.tvExplorer.HideSelection = False
    Me.tvExplorer.ImageIndex = 0
    Me.tvExplorer.ImageList = Me.imlFolderTree
    Me.tvExplorer.Location = New System.Drawing.Point(0, 0)
    Me.tvExplorer.Name = "tvExplorer"
    Me.tvExplorer.SelectedImageIndex = 0
    Me.tvExplorer.ShowLines = False
    Me.tvExplorer.ShowNodeToolTips = True
    Me.tvExplorer.Size = New System.Drawing.Size(200, 279)
    Me.tvExplorer.TabIndex = 0
    '
    'imlFolderTree
    '
    Me.imlFolderTree.ImageStream = CType(resources.GetObject("imlFolderTree.ImageStream"), System.Windows.Forms.ImageListStreamer)
    Me.imlFolderTree.TransparentColor = System.Drawing.Color.Transparent
    Me.imlFolderTree.Images.SetKeyName(0, "archive")
    Me.imlFolderTree.Images.SetKeyName(1, "empty")
    Me.imlFolderTree.Images.SetKeyName(2, "files")
    Me.imlFolderTree.Images.SetKeyName(3, "folders")
    '
    'lvFiles
    '
    Me.lvFiles.AllowDrop = True
    Me.lvFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colName, Me.colType, Me.colSize, Me.colCompression})
    Me.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill
    Me.lvFiles.HideSelection = False
    Me.lvFiles.LabelWrap = False
    Me.lvFiles.LargeImageList = Me.imlFileList32
    Me.lvFiles.Location = New System.Drawing.Point(0, 0)
    Me.lvFiles.Name = "lvFiles"
    Me.lvFiles.ShowGroups = False
    Me.lvFiles.ShowItemToolTips = True
    Me.lvFiles.Size = New System.Drawing.Size(500, 279)
    Me.lvFiles.SmallImageList = Me.imlFileList
    Me.lvFiles.TabIndex = 0
    Me.lvFiles.UseCompatibleStateImageBehavior = False
    Me.lvFiles.View = System.Windows.Forms.View.Details
    '
    'colName
    '
    Me.colName.Text = "Name"
    Me.colName.Width = 218
    '
    'colType
    '
    Me.colType.Text = "Type"
    '
    'colSize
    '
    Me.colSize.Text = "Size"
    Me.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
    '
    'colCompression
    '
    Me.colCompression.Text = "Compression"
    Me.colCompression.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
    '
    'imlFileList32
    '
    Me.imlFileList32.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
    Me.imlFileList32.ImageSize = New System.Drawing.Size(32, 32)
    Me.imlFileList32.TransparentColor = System.Drawing.Color.Transparent
    '
    'imlFileList
    '
    Me.imlFileList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
    Me.imlFileList.ImageSize = New System.Drawing.Size(16, 16)
    Me.imlFileList.TransparentColor = System.Drawing.Color.Transparent
    '
    'stsInfo
    '
    Me.stsInfo.AllowMerge = False
    Me.stsInfo.Dock = System.Windows.Forms.DockStyle.None
    Me.stsInfo.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblSelCount, Me.lblSelUncompressed, Me.lblSelSize, Me.pbActivity})
    Me.stsInfo.Location = New System.Drawing.Point(0, 0)
    Me.stsInfo.Name = "stsInfo"
    Me.stsInfo.Size = New System.Drawing.Size(704, 22)
    Me.stsInfo.TabIndex = 1
    '
    'lblSelCount
    '
    Me.lblSelCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
    Me.lblSelCount.Margin = New System.Windows.Forms.Padding(0, 3, 32, 2)
    Me.lblSelCount.Name = "lblSelCount"
    Me.lblSelCount.Size = New System.Drawing.Size(505, 17)
    Me.lblSelCount.Spring = True
    Me.lblSelCount.Text = "No objects selected"
    Me.lblSelCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'lblSelUncompressed
    '
    Me.lblSelUncompressed.Margin = New System.Windows.Forms.Padding(0, 3, 32, 2)
    Me.lblSelUncompressed.Name = "lblSelUncompressed"
    Me.lblSelUncompressed.Size = New System.Drawing.Size(44, 17)
    Me.lblSelUncompressed.Text = "0 bytes"
    Me.lblSelUncompressed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'lblSelSize
    '
    Me.lblSelSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
    Me.lblSelSize.Margin = New System.Windows.Forms.Padding(0, 3, 32, 2)
    Me.lblSelSize.Name = "lblSelSize"
    Me.lblSelSize.Size = New System.Drawing.Size(44, 17)
    Me.lblSelSize.Text = "0 bytes"
    Me.lblSelSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'pbActivity
    '
    Me.pbActivity.Enabled = False
    Me.pbActivity.Margin = New System.Windows.Forms.Padding(1, 3, 33, 3)
    Me.pbActivity.Name = "pbActivity"
    Me.pbActivity.Size = New System.Drawing.Size(100, 16)
    Me.pbActivity.Style = System.Windows.Forms.ProgressBarStyle.Continuous
    Me.pbActivity.Visible = False
    '
    'mnuContextFile
    '
    Me.mnuContextFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuContextFileOpen, Me.mnuContextFileExtract, Me.mnuContextFileSpace, Me.mnuContextFileProperties})
    '
    'mnuContextFileOpen
    '
    Me.mnuContextFileOpen.Index = 0
    Me.mnuContextFileOpen.Text = "&Open"
    '
    'mnuContextFileExtract
    '
    Me.mnuContextFileExtract.Index = 1
    Me.mnuContextFileExtract.Text = "&Extract..."
    '
    'mnuContextFileSpace
    '
    Me.mnuContextFileSpace.Index = 2
    Me.mnuContextFileSpace.Text = "-"
    '
    'mnuContextFileProperties
    '
    Me.mnuContextFileProperties.Index = 3
    Me.mnuContextFileProperties.Text = "&Properties..."
    '
    'mnuContextView
    '
    Me.mnuContextView.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuContextViewIcons, Me.mnuContextViewSort, Me.mnuContextViewSpace1, Me.mnuContextViewFlat, Me.mnuContextViewTree, Me.mnuContextViewSpace2, Me.mnuContextViewGoRoot, Me.mnuContextViewGoParent})
    '
    'mnuContextViewIcons
    '
    Me.mnuContextViewIcons.Index = 0
    Me.mnuContextViewIcons.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuContextViewIconsLarge, Me.mnuContextViewIconsSmall, Me.mnuContextViewIconsList, Me.mnuContextViewIconsDetails, Me.mnuContextViewIconsTile})
    Me.mnuContextViewIcons.Text = "&Icons"
    '
    'mnuContextViewIconsLarge
    '
    Me.mnuContextViewIconsLarge.Index = 0
    Me.mnuContextViewIconsLarge.RadioCheck = True
    Me.mnuContextViewIconsLarge.Text = "&Large Icons"
    '
    'mnuContextViewIconsSmall
    '
    Me.mnuContextViewIconsSmall.Index = 1
    Me.mnuContextViewIconsSmall.RadioCheck = True
    Me.mnuContextViewIconsSmall.Text = "&Small Icons"
    '
    'mnuContextViewIconsList
    '
    Me.mnuContextViewIconsList.Index = 2
    Me.mnuContextViewIconsList.RadioCheck = True
    Me.mnuContextViewIconsList.Text = "&List"
    '
    'mnuContextViewIconsDetails
    '
    Me.mnuContextViewIconsDetails.Index = 3
    Me.mnuContextViewIconsDetails.RadioCheck = True
    Me.mnuContextViewIconsDetails.Text = "&Details"
    '
    'mnuContextViewIconsTile
    '
    Me.mnuContextViewIconsTile.Index = 4
    Me.mnuContextViewIconsTile.RadioCheck = True
    Me.mnuContextViewIconsTile.Text = "&Tiles"
    '
    'mnuContextViewSort
    '
    Me.mnuContextViewSort.Index = 1
    Me.mnuContextViewSort.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuContextViewSortName, Me.mnuContextViewSortType, Me.mnuContextViewSortSize, Me.mnuContextViewSortOrder})
    Me.mnuContextViewSort.Text = "&Sort"
    '
    'mnuContextViewSortName
    '
    Me.mnuContextViewSortName.Index = 0
    Me.mnuContextViewSortName.RadioCheck = True
    Me.mnuContextViewSortName.Text = "by &Name"
    '
    'mnuContextViewSortType
    '
    Me.mnuContextViewSortType.Index = 1
    Me.mnuContextViewSortType.RadioCheck = True
    Me.mnuContextViewSortType.Text = "by &Type"
    '
    'mnuContextViewSortSize
    '
    Me.mnuContextViewSortSize.Index = 2
    Me.mnuContextViewSortSize.RadioCheck = True
    Me.mnuContextViewSortSize.Text = "by &Size"
    '
    'mnuContextViewSortOrder
    '
    Me.mnuContextViewSortOrder.Checked = True
    Me.mnuContextViewSortOrder.Index = 3
    Me.mnuContextViewSortOrder.RadioCheck = True
    Me.mnuContextViewSortOrder.Text = "by &Order"
    '
    'mnuContextViewSpace1
    '
    Me.mnuContextViewSpace1.Index = 2
    Me.mnuContextViewSpace1.Text = "-"
    '
    'mnuContextViewFlat
    '
    Me.mnuContextViewFlat.Index = 3
    Me.mnuContextViewFlat.Text = "&Flat View"
    '
    'mnuContextViewTree
    '
    Me.mnuContextViewTree.Checked = True
    Me.mnuContextViewTree.Index = 4
    Me.mnuContextViewTree.Text = "&Tree View"
    '
    'mnuContextViewSpace2
    '
    Me.mnuContextViewSpace2.Index = 5
    Me.mnuContextViewSpace2.Text = "-"
    '
    'mnuContextViewGoRoot
    '
    Me.mnuContextViewGoRoot.Enabled = False
    Me.mnuContextViewGoRoot.Index = 6
    Me.mnuContextViewGoRoot.Text = "Go to &Root"
    '
    'mnuContextViewGoParent
    '
    Me.mnuContextViewGoParent.Enabled = False
    Me.mnuContextViewGoParent.Index = 7
    Me.mnuContextViewGoParent.Text = "Go to &Parent"
    '
    'pnlUI
    '
    '
    'pnlUI.BottomToolStripPanel
    '
    Me.pnlUI.BottomToolStripPanel.Controls.Add(Me.stsInfo)
    '
    'pnlUI.ContentPanel
    '
    Me.pnlUI.ContentPanel.Controls.Add(Me.pnlMain)
    Me.pnlUI.ContentPanel.Size = New System.Drawing.Size(704, 279)
    Me.pnlUI.Dock = System.Windows.Forms.DockStyle.Fill
    Me.pnlUI.LeftToolStripPanelVisible = False
    Me.pnlUI.Location = New System.Drawing.Point(0, 0)
    Me.pnlUI.Name = "pnlUI"
    Me.pnlUI.RightToolStripPanelVisible = False
    Me.pnlUI.Size = New System.Drawing.Size(704, 340)
    Me.pnlUI.TabIndex = 3
    '
    'pnlUI.TopToolStripPanel
    '
    Me.pnlUI.TopToolStripPanel.Controls.Add(Me.tbNav)
    Me.pnlUI.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
    '
    'tbNav
    '
    Me.tbNav.CanOverflow = False
    Me.tbNav.Dock = System.Windows.Forms.DockStyle.None
    Me.tbNav.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
    Me.tbNav.ImageScalingSize = New System.Drawing.Size(32, 32)
    Me.tbNav.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.cmdBack, Me.cmdForward, Me.cmdUp, Me.sepSpace1, Me.lblAddress, Me.txtAddress, Me.cmdGo, Me.sepSpace2, Me.cmdExtract, Me.cmdProperties})
    Me.tbNav.Location = New System.Drawing.Point(3, 0)
    Me.tbNav.Name = "tbNav"
    Me.tbNav.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
    Me.tbNav.Size = New System.Drawing.Size(435, 39)
    Me.tbNav.TabIndex = 0
    '
    'cmdBack
    '
    Me.cmdBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.cmdBack.Enabled = False
    Me.cmdBack.Image = CType(resources.GetObject("cmdBack.Image"), System.Drawing.Image)
    Me.cmdBack.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.cmdBack.Name = "cmdBack"
    Me.cmdBack.Size = New System.Drawing.Size(36, 36)
    Me.cmdBack.Text = "Back"
    '
    'cmdForward
    '
    Me.cmdForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.cmdForward.Enabled = False
    Me.cmdForward.Image = CType(resources.GetObject("cmdForward.Image"), System.Drawing.Image)
    Me.cmdForward.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.cmdForward.Name = "cmdForward"
    Me.cmdForward.Size = New System.Drawing.Size(36, 36)
    Me.cmdForward.Text = "Forward"
    '
    'cmdUp
    '
    Me.cmdUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.cmdUp.Enabled = False
    Me.cmdUp.Image = CType(resources.GetObject("cmdUp.Image"), System.Drawing.Image)
    Me.cmdUp.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.cmdUp.Name = "cmdUp"
    Me.cmdUp.Size = New System.Drawing.Size(36, 36)
    Me.cmdUp.Text = "Up"
    '
    'sepSpace1
    '
    Me.sepSpace1.Name = "sepSpace1"
    Me.sepSpace1.Size = New System.Drawing.Size(6, 39)
    '
    'lblAddress
    '
    Me.lblAddress.Name = "lblAddress"
    Me.lblAddress.Size = New System.Drawing.Size(52, 36)
    Me.lblAddress.Text = "Address:"
    '
    'txtAddress
    '
    Me.txtAddress.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
    Me.txtAddress.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
    Me.txtAddress.AutoSize = False
    Me.txtAddress.Enabled = False
    Me.txtAddress.Name = "txtAddress"
    Me.txtAddress.Size = New System.Drawing.Size(150, 39)
    '
    'cmdGo
    '
    Me.cmdGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.cmdGo.Enabled = False
    Me.cmdGo.Image = CType(resources.GetObject("cmdGo.Image"), System.Drawing.Image)
    Me.cmdGo.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.cmdGo.Name = "cmdGo"
    Me.cmdGo.Size = New System.Drawing.Size(36, 36)
    Me.cmdGo.Text = "Go"
    '
    'sepSpace2
    '
    Me.sepSpace2.Name = "sepSpace2"
    Me.sepSpace2.Size = New System.Drawing.Size(6, 39)
    '
    'cmdExtract
    '
    Me.cmdExtract.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.cmdExtract.Enabled = False
    Me.cmdExtract.Image = CType(resources.GetObject("cmdExtract.Image"), System.Drawing.Image)
    Me.cmdExtract.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.cmdExtract.Name = "cmdExtract"
    Me.cmdExtract.Size = New System.Drawing.Size(36, 36)
    Me.cmdExtract.Text = "Extract..."
    '
    'cmdProperties
    '
    Me.cmdProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
    Me.cmdProperties.Enabled = False
    Me.cmdProperties.Image = CType(resources.GetObject("cmdProperties.Image"), System.Drawing.Image)
    Me.cmdProperties.ImageTransparentColor = System.Drawing.Color.Magenta
    Me.cmdProperties.Name = "cmdProperties"
    Me.cmdProperties.Size = New System.Drawing.Size(36, 36)
    Me.cmdProperties.Text = "Properties..."
    '
    'tmrSelectionUpdate
    '
    Me.tmrSelectionUpdate.Interval = 200
    '
    'frmViewer
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(704, 340)
    Me.Controls.Add(Me.pnlUI)
    Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
    Me.Menu = Me.mnuMain
    Me.MinimumSize = New System.Drawing.Size(460, 300)
    Me.Name = "frmViewer"
    Me.Text = "PMRes Viewer"
    Me.pnlMain.Panel1.ResumeLayout(False)
    Me.pnlMain.Panel2.ResumeLayout(False)
    CType(Me.pnlMain, System.ComponentModel.ISupportInitialize).EndInit()
    Me.pnlMain.ResumeLayout(False)
    Me.stsInfo.ResumeLayout(False)
    Me.stsInfo.PerformLayout()
    Me.pnlUI.BottomToolStripPanel.ResumeLayout(False)
    Me.pnlUI.BottomToolStripPanel.PerformLayout()
    Me.pnlUI.ContentPanel.ResumeLayout(False)
    Me.pnlUI.TopToolStripPanel.ResumeLayout(False)
    Me.pnlUI.TopToolStripPanel.PerformLayout()
    Me.pnlUI.ResumeLayout(False)
    Me.pnlUI.PerformLayout()
    Me.tbNav.ResumeLayout(False)
    Me.tbNav.PerformLayout()
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents mnuMain As System.Windows.Forms.MainMenu
  Friend WithEvents mnuFile As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileOpenArchive As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileCloseArchive As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileSpace1 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileOpenFile As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileExtractFile As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFilePropertiesFile As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileSpace2 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileExit As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEdit As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEditAll As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEditNone As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEditInvert As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEditSelect As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEditDeselect As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEditSpace As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEditSelType As System.Windows.Forms.MenuItem
  Friend WithEvents mnuEditDeselType As System.Windows.Forms.MenuItem
  Friend WithEvents mnuView As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewSort As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewSortName As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewSortType As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewSortSize As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewSortOrder As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewSpace1 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewFlat As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewTree As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewSpace2 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewGoRoot As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewGoParent As System.Windows.Forms.MenuItem
  Friend WithEvents mnuHelp As System.Windows.Forms.MenuItem
  Friend WithEvents mnuHelpAbout As System.Windows.Forms.MenuItem
  Friend WithEvents pnlMain As System.Windows.Forms.SplitContainer
  Friend WithEvents tvExplorer As System.Windows.Forms.TreeView
  Friend WithEvents lvFiles As ListViewNoHScroll
  Friend WithEvents stsInfo As System.Windows.Forms.StatusStrip
  Friend WithEvents lblSelCount As System.Windows.Forms.ToolStripStatusLabel
  Friend WithEvents lblSelSize As System.Windows.Forms.ToolStripStatusLabel
  Friend WithEvents lblSelUncompressed As System.Windows.Forms.ToolStripStatusLabel
  Friend WithEvents pbActivity As System.Windows.Forms.ToolStripProgressBar
  Friend WithEvents imlFolderTree As System.Windows.Forms.ImageList
  Friend WithEvents colName As System.Windows.Forms.ColumnHeader
  Friend WithEvents colSize As System.Windows.Forms.ColumnHeader
  Friend WithEvents colCompression As System.Windows.Forms.ColumnHeader
  Friend WithEvents imlFileList As System.Windows.Forms.ImageList
  Friend WithEvents imlFileList32 As System.Windows.Forms.ImageList
  Friend WithEvents colType As System.Windows.Forms.ColumnHeader
  Friend WithEvents mnuContextFile As System.Windows.Forms.ContextMenu
  Friend WithEvents mnuContextFileOpen As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextFileExtract As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextFileSpace As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextFileProperties As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewIcons As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewIconsLarge As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewIconsSmall As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewIconsDetails As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewIconsList As System.Windows.Forms.MenuItem
  Friend WithEvents mnuViewIconsTile As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextView As System.Windows.Forms.ContextMenu
  Friend WithEvents mnuContextViewIcons As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewIconsLarge As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewIconsSmall As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewIconsList As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewIconsDetails As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewIconsTile As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewSort As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewSortName As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewSortType As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewSortSize As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewSortOrder As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewSpace1 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewFlat As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewTree As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewSpace2 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewGoRoot As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextViewGoParent As System.Windows.Forms.MenuItem
  Friend WithEvents pnlUI As System.Windows.Forms.ToolStripContainer
  Friend WithEvents tbNav As System.Windows.Forms.ToolStrip
  Friend WithEvents cmdBack As System.Windows.Forms.ToolStripButton
  Friend WithEvents cmdForward As System.Windows.Forms.ToolStripButton
  Friend WithEvents cmdUp As System.Windows.Forms.ToolStripButton
  Friend WithEvents sepSpace1 As System.Windows.Forms.ToolStripSeparator
  Friend WithEvents lblAddress As System.Windows.Forms.ToolStripLabel
  Friend WithEvents txtAddress As System.Windows.Forms.ToolStripTextBox
  Friend WithEvents cmdGo As System.Windows.Forms.ToolStripButton
  Friend WithEvents sepSpace2 As System.Windows.Forms.ToolStripSeparator
  Friend WithEvents cmdExtract As System.Windows.Forms.ToolStripButton
  Friend WithEvents cmdProperties As System.Windows.Forms.ToolStripButton
  Friend WithEvents mnuViewToolbar As System.Windows.Forms.MenuItem
  Friend WithEvents mnuContextHistory As System.Windows.Forms.ContextMenu
  Friend WithEvents tmrSelectionUpdate As System.Windows.Forms.Timer

End Class
