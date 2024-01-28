Public Class frmViewer
  Private mArchivePath As String
  Private Property sArchivePath As String
    Get
      Return mArchivePath
    End Get
    Set(value As String)
      mArchivePath = value
      SelectionUpdate()
    End Set
  End Property
  Private sArchiveName As String
  Private mArchiveDir As String
  Private Property sArchiveDir As String
    Get
      Return mArchiveDir
    End Get
    Set(value As String)
      mArchiveDir = value
      txtAddress.Enabled = tvExplorer.Nodes.Count > 0 AndAlso Not String.IsNullOrEmpty(value)
      txtAddress.Text = value
      cmdGo.Enabled = txtAddress.Enabled And Not String.IsNullOrEmpty(txtAddress.Text)
      If String.IsNullOrEmpty(mArchiveDir) OrElse mArchiveDir = IO.Path.DirectorySeparatorChar Then
        mnuViewGoParent.Enabled = False
        mnuViewGoRoot.Enabled = False
        cmdUp.Enabled = False
      Else
        mnuViewGoParent.Enabled = True
        mnuViewGoRoot.Enabled = True
        cmdUp.Enabled = True
      End If
    End Set
  End Property
  Private zArchive() As ZIP.FileSystemEntry
  Private sDirHistory As New List(Of String)
  Private iDirHistIndex As Long = -1
  Private tBusy As Boolean
  Private regCulture As Boolean = False
  Private regCase As Boolean = False
  Private regHistory As New List(Of String)
  Private isLoaded As Boolean = False
  Private sExtractHistory As New List(Of String)

#Region "Menus"
#Region "File"
  Private Sub mnuFileOpenArchive_Click(sender As System.Object, e As System.EventArgs) Handles mnuFileOpenArchive.Click

    Using cdlOpen As New Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
      cdlOpen.DefaultExtension = "res"
      cdlOpen.DefaultFileName = "palemoon.res"

      cdlOpen.Filters.Add(New Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("Resource Archives", "*.res"))
      cdlOpen.Filters.Add(New Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("All Files", "*.*"))
      cdlOpen.Title = "Open palemoon.res Archive..."
      If IO.Directory.Exists(IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, "Pale Moon")) Then
        cdlOpen.InitialDirectory = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, "Pale Moon")
      Else
        cdlOpen.InitialDirectory = Nothing
      End If
      If Not cdlOpen.ShowDialog(Me.Handle) = Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok Then Return
      mnuFileCloseArchive.PerformClick()
      LoadFile(cdlOpen.FileName)
      lvFiles.Focus()
    End Using

  End Sub

  Private Sub mnuFileCloseArchive_Click(sender As System.Object, e As System.EventArgs) Handles mnuFileCloseArchive.Click
    sArchivePath = Nothing
    sArchiveName = Nothing
    sArchiveDir = Nothing
    tvExplorer.Nodes.Clear()
    lvFiles.Items.Clear()
    Erase zArchive
    iDirHistIndex = -1
    sDirHistory.Clear()
    cmdBack.Enabled = False
    cmdForward.Enabled = False
    Me.Text = "PMRes Viewer"
    lblSelCount.Text = "No objects selected"
    lblSelSize.Text = "0 bytes"
    lblSelUncompressed.Text = "0 bytes"
  End Sub

  Private Sub mnuFileOpenFile_Click(sender As System.Object, e As System.EventArgs) Handles mnuFileOpenFile.Click
    If lvFiles.SelectedItems.Count < 1 Then Return
    If lvFiles.SelectedItems.Count > 1 Then
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zItem As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        If zItem.GetType Is GetType(ZIP.FileSystemDirectory) Then Return
      Next
    End If
    lvFiles_Open()
  End Sub


  Private Sub mnuFileExtractFile_Click(sender As System.Object, e As System.EventArgs) Handles mnuFileExtractFile.Click
    If lvFiles.SelectedItems.Count < 1 Then Return
    lvFiles_Extract()
  End Sub

  Private Sub mnuFilePropertiesFile_Click(sender As System.Object, e As System.EventArgs) Handles mnuFilePropertiesFile.Click
    If Not lvFiles.SelectedItems.Count = 1 Then Return
    lvFiles_Properties()
  End Sub

  Private Sub mnuFileExit_Click(sender As System.Object, e As System.EventArgs) Handles mnuFileExit.Click
    Me.Close()
  End Sub
#End Region

#Region "Edit"
  Private Sub mnuEditAll_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditAll.Click
    If txtAddress.Focused Then
      txtAddress.SelectAll()
      Return
    End If
    If tvExplorer.Focused Then Return
    lvFiles.SuspendLayout()
    For I As Integer = 0 To lvFiles.Items.Count - 1
      lvFiles.Items(I).Selected = True
    Next
    lvFiles.ResumeLayout()
  End Sub

  Private Sub mnuEditNone_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditNone.Click
    lvFiles.SuspendLayout()
    For I As Integer = 0 To lvFiles.Items.Count - 1
      lvFiles.Items(I).Selected = False
    Next
    lvFiles.ResumeLayout()
  End Sub

  Private Sub mnuEditInvert_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditInvert.Click
    lvFiles.SuspendLayout()
    For I As Integer = 0 To lvFiles.Items.Count - 1
      lvFiles.Items(I).Selected = Not lvFiles.Items(I).Selected
    Next
    lvFiles.ResumeLayout()
  End Sub

  Private Sub mnuEditSelect_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditSelect.Click
    Using tReg As New frmRegEx(True)
      tReg.RegEx = regHistory.Last
      tReg.History = regHistory.ToArray
      tReg.IsCultureInvariant = regCulture
      tReg.CaseInsensitive = regCase
      If tReg.ShowDialog(Me) = Windows.Forms.DialogResult.Cancel Then Return
      regCulture = tReg.IsCultureInvariant
      regCase = tReg.CaseInsensitive
      If String.IsNullOrEmpty(tReg.RegEx) Then Return
      Try
        Dim rOpts As System.Text.RegularExpressions.RegexOptions = System.Text.RegularExpressions.RegexOptions.None
        If tReg.IsCultureInvariant Then rOpts = rOpts Or System.Text.RegularExpressions.RegexOptions.CultureInvariant
        If tReg.CaseInsensitive Then rOpts = rOpts Or System.Text.RegularExpressions.RegexOptions.IgnoreCase
        Dim r As New System.Text.RegularExpressions.Regex(tReg.RegEx, rOpts)
        If Not regHistory.Contains(tReg.RegEx) Then regHistory.Add(tReg.RegEx)
        For I As Integer = 0 To lvFiles.Items.Count - 1
          Dim zItem As ZIP.FileSystemEntry = lvFiles.Items(I).Tag
          If r.IsMatch(IO.Path.GetFileName(zItem.Name)) Then lvFiles.Items(I).Selected = True
        Next
        DoSelectionUpdate()
      Catch ex As Exception
        Beep()
      End Try
    End Using
  End Sub

  Private Sub mnuEditDeselect_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditDeselect.Click
    Using tReg As New frmRegEx(False)
      tReg.RegEx = regHistory.Last
      tReg.History = regHistory.ToArray
      tReg.IsCultureInvariant = regCulture
      tReg.CaseInsensitive = regCase
      If tReg.ShowDialog(Me) = Windows.Forms.DialogResult.Cancel Then Return
      regCulture = tReg.IsCultureInvariant
      regCase = tReg.CaseInsensitive
      If String.IsNullOrEmpty(tReg.RegEx) Then Return
      Try
        Dim rOpts As System.Text.RegularExpressions.RegexOptions = System.Text.RegularExpressions.RegexOptions.None
        If tReg.IsCultureInvariant Then rOpts = rOpts Or System.Text.RegularExpressions.RegexOptions.CultureInvariant
        If tReg.CaseInsensitive Then rOpts = rOpts Or System.Text.RegularExpressions.RegexOptions.IgnoreCase
        Dim r As New System.Text.RegularExpressions.Regex(tReg.RegEx, rOpts)
        If Not regHistory.Contains(tReg.RegEx) Then regHistory.Add(tReg.RegEx)
        For I As Integer = 0 To lvFiles.Items.Count - 1
          Dim zItem As ZIP.FileSystemEntry = lvFiles.Items(I).Tag
          If r.IsMatch(IO.Path.GetFileName(zItem.Name)) Then lvFiles.Items(I).Selected = False
        Next
        DoSelectionUpdate()
      Catch ex As Exception
        Beep()
      End Try
    End Using
  End Sub

  Private Sub mnuEditSelType_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditSelType.Click
    If lvFiles.FocusedItem Is Nothing Then Return
    Dim selType As String = lvFiles.FocusedItem.SubItems(1).Text
    For I As Integer = 0 To lvFiles.Items.Count - 1
      If lvFiles.Items(I).SubItems(1).Text = selType Then lvFiles.Items(I).Selected = True
    Next
    DoSelectionUpdate()
  End Sub

  Private Sub mnuEditDeselType_Click(sender As System.Object, e As System.EventArgs) Handles mnuEditDeselType.Click
    If lvFiles.FocusedItem Is Nothing Then Return
    Dim selType As String = lvFiles.FocusedItem.SubItems(1).Text
    For I As Integer = 0 To lvFiles.Items.Count - 1
      If lvFiles.Items(I).SubItems(1).Text = selType Then lvFiles.Items(I).Selected = False
    Next
    DoSelectionUpdate()
  End Sub
#End Region

#Region "View"
  Private Sub mnuViewIconsLarge_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewIconsLarge.Click, mnuContextViewIconsLarge.Click
    mnuViewIconsLarge.Checked = True
    mnuViewIconsSmall.Checked = False
    mnuViewIconsList.Checked = False
    mnuViewIconsDetails.Checked = False
    mnuViewIconsTile.Checked = False
    lvFiles.View = View.LargeIcon
    Settings.IconMethod = View.LargeIcon
  End Sub

  Private Sub mnuViewIconsSmall_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewIconsSmall.Click, mnuContextViewIconsSmall.Click
    mnuViewIconsLarge.Checked = False
    mnuViewIconsSmall.Checked = True
    mnuViewIconsList.Checked = False
    mnuViewIconsDetails.Checked = False
    mnuViewIconsTile.Checked = False
    lvFiles.View = View.SmallIcon
    Settings.IconMethod = View.SmallIcon
  End Sub

  Private Sub mnuViewIconsList_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewIconsList.Click, mnuContextViewIconsList.Click
    mnuViewIconsLarge.Checked = False
    mnuViewIconsSmall.Checked = False
    mnuViewIconsList.Checked = True
    mnuViewIconsDetails.Checked = False
    mnuViewIconsTile.Checked = False
    lvFiles.View = View.List
    Settings.IconMethod = View.List
  End Sub

  Private Sub mnuViewIconsDetails_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewIconsDetails.Click, mnuContextViewIconsDetails.Click
    mnuViewIconsLarge.Checked = False
    mnuViewIconsSmall.Checked = False
    mnuViewIconsList.Checked = False
    mnuViewIconsDetails.Checked = True
    mnuViewIconsTile.Checked = False
    lvFiles.View = View.Details
    Settings.IconMethod = View.Details
    ResizeColumns()
  End Sub

  Private Sub mnuViewIconsTile_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewIconsTile.Click, mnuContextViewIconsTile.Click
    mnuViewIconsLarge.Checked = False
    mnuViewIconsSmall.Checked = False
    mnuViewIconsList.Checked = False
    mnuViewIconsDetails.Checked = False
    mnuViewIconsTile.Checked = True
    lvFiles.View = View.Tile
    Settings.IconMethod = View.Tile
  End Sub


  Private Sub mnuViewSortName_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewSortName.Click, mnuContextViewSortName.Click
    If mnuViewSortName.Checked Then
      If lvFiles.Sorting = SortOrder.Ascending Then
        lvFiles.Sorting = SortOrder.Descending
      Else
        lvFiles.Sorting = SortOrder.Ascending
      End If
    Else
      lvFiles.Sorting = SortOrder.Descending
    End If
    mnuViewSortName.Checked = True
    mnuViewSortType.Checked = False
    mnuViewSortSize.Checked = False
    mnuViewSortOrder.Checked = False
    lvFiles.ListViewItemSorter = New ListViewItemComparer("Name", lvFiles.Sorting)
    lvFiles.Sort()
    If lvFiles.Sorting = SortOrder.Ascending Then
      lvFiles.Columns(0).Text = "Name ↑"
    Else
      lvFiles.Columns(0).Text = "Name ↓"
    End If
    lvFiles.Columns(1).Text = "Type"
    lvFiles.Columns(2).Text = "Size"
    Settings.SortMethod = "Name"
  End Sub

  Private Sub mnuViewSortType_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewSortType.Click, mnuContextViewSortType.Click
    If mnuViewSortType.Checked Then
      If lvFiles.Sorting = SortOrder.Ascending Then
        lvFiles.Sorting = SortOrder.Descending
      Else
        lvFiles.Sorting = SortOrder.Ascending
      End If
    Else
      lvFiles.Sorting = SortOrder.Descending
    End If
    mnuViewSortName.Checked = False
    mnuViewSortType.Checked = True
    mnuViewSortSize.Checked = False
    mnuViewSortOrder.Checked = False
    lvFiles.ListViewItemSorter = New ListViewItemComparer("Type", lvFiles.Sorting)
    lvFiles.Sort()
    lvFiles.Columns(0).Text = "Name"
    If lvFiles.Sorting = SortOrder.Ascending Then
      lvFiles.Columns(1).Text = "Type ↑"
    Else
      lvFiles.Columns(1).Text = "Type ↓"
    End If
    lvFiles.Columns(2).Text = "Size"
    Settings.SortMethod = "Type"
  End Sub

  Private Sub mnuViewSortSize_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewSortSize.Click, mnuContextViewSortSize.Click
    If mnuViewSortSize.Checked Then
      If lvFiles.Sorting = SortOrder.Ascending Then
        lvFiles.Sorting = SortOrder.Descending
      Else
        lvFiles.Sorting = SortOrder.Ascending
      End If
    Else
      lvFiles.Sorting = SortOrder.Descending
    End If
    mnuViewSortName.Checked = False
    mnuViewSortType.Checked = False
    mnuViewSortSize.Checked = True
    mnuViewSortOrder.Checked = False
    lvFiles.ListViewItemSorter = New ListViewItemComparer("Size", lvFiles.Sorting)
    lvFiles.Sort()
    lvFiles.Columns(0).Text = "Name"
    lvFiles.Columns(1).Text = "Type"
    If lvFiles.Sorting = SortOrder.Ascending Then
      lvFiles.Columns(2).Text = "Size ↑"
    Else
      lvFiles.Columns(2).Text = "Size ↓"
    End If
    Settings.SortMethod = "Size"
  End Sub

  Private Sub mnuViewSortOrder_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewSortOrder.Click, mnuContextViewSortOrder.Click
    If mnuViewSortOrder.Checked Then
      If lvFiles.Sorting = SortOrder.Ascending Then
        lvFiles.Sorting = SortOrder.Descending
      Else
        lvFiles.Sorting = SortOrder.Ascending
      End If
    Else
      lvFiles.Sorting = SortOrder.Descending
    End If
    mnuViewSortName.Checked = False
    mnuViewSortType.Checked = False
    mnuViewSortSize.Checked = False
    mnuViewSortOrder.Checked = True
    lvFiles.ListViewItemSorter = New ListViewItemComparer("Order", lvFiles.Sorting)
    lvFiles.Sort()
    lvFiles.Columns(0).Text = "Name"
    lvFiles.Columns(1).Text = "Type"
    lvFiles.Columns(2).Text = "Size"
    Settings.SortMethod = "Order"
  End Sub


  Private Sub mnuToolbar_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewToolbar.Click
    mnuViewToolbar.Checked = Not mnuViewToolbar.Checked
    Settings.ShowToolbar = mnuViewToolbar.Checked
    tbNav.Visible = mnuViewToolbar.Checked
  End Sub

  Private Sub mnuViewFlat_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewFlat.Click, mnuContextViewFlat.Click
    mnuViewFlat.Checked = Not mnuViewFlat.Checked
    Settings.FlatView = mnuViewFlat.Checked
    ToggleFlat()
  End Sub

  Private Sub mnuViewTree_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewTree.Click, mnuContextViewTree.Click
    mnuViewTree.Checked = Not mnuViewTree.Checked
    Settings.TreeView = mnuViewTree.Checked
    ToggleTree(mnuViewTree.Checked)
  End Sub


  Private Sub mnuViewGoRoot_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewGoRoot.Click, mnuContextViewGoRoot.Click
    sArchiveDir = IO.Path.DirectorySeparatorChar
    'RenderArchive()
    RenderDir(sArchiveDir)
  End Sub

  Private Sub mnuViewGoParent_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewGoParent.Click, mnuContextViewGoParent.Click
    sArchiveDir = IO.Path.GetDirectoryName(sArchiveDir)
    RenderDir(sArchiveDir)
  End Sub
#End Region

  Private Sub mnuHelpAbout_Click(sender As System.Object, e As System.EventArgs) Handles mnuHelpAbout.Click
    frmAbout.Show(Me)
  End Sub

#Region "Context Menus"
  Private Sub mnuContextFileOpen_Click(sender As System.Object, e As System.EventArgs) Handles mnuContextFileOpen.Click
    If mnuContextFile.SourceControl.Name = lvFiles.Name Then
      lvFiles_Open()
    ElseIf mnuContextFile.SourceControl.Name = tvExplorer.Name Then
      RenderDir(mnuContextFile.Tag)
    End If
  End Sub

  Private Sub mnuContextFileExtract_Click(sender As System.Object, e As System.EventArgs) Handles mnuContextFileExtract.Click
    If mnuContextFile.SourceControl.Name = lvFiles.Name Then
      lvFiles_Extract()
    ElseIf mnuContextFile.SourceControl.Name = tvExplorer.Name Then
      tvExplorer_Extract()
    End If
  End Sub


  Private Sub mnuContextFileExpand_Click(sender As System.Object, e As System.EventArgs) Handles mnuContextFileExpand.Click
    Dim aNodes As TreeNode() = tvExplorer.Nodes.Find(mnuContextFile.Tag, True)
    If aNodes Is Nothing OrElse aNodes.LongLength = 0 Then Return
    aNodes(0).Expand()
  End Sub

  Private Sub mnuContextFileExpandAll_Click(sender As System.Object, e As System.EventArgs) Handles mnuContextFileExpandAll.Click
    Dim aNodes As TreeNode() = tvExplorer.Nodes.Find(mnuContextFile.Tag, True)
    If aNodes Is Nothing OrElse aNodes.LongLength = 0 Then Return
    aNodes(0).ExpandAll()
  End Sub

  Private Sub mnuContextFileCollapse_Click(sender As System.Object, e As System.EventArgs) Handles mnuContextFileCollapse.Click
    Dim aNodes As TreeNode() = tvExplorer.Nodes.Find(mnuContextFile.Tag, True)
    If aNodes Is Nothing OrElse aNodes.LongLength = 0 Then Return
    aNodes(0).Collapse(True)
  End Sub

  Private Sub mnuContextFileCollapseAll_Click(sender As System.Object, e As System.EventArgs) Handles mnuContextFileCollapseAll.Click
    Dim aNodes As TreeNode() = tvExplorer.Nodes.Find(mnuContextFile.Tag, True)
    If aNodes Is Nothing OrElse aNodes.LongLength = 0 Then Return
    aNodes(0).Collapse(False)
  End Sub


  Private Sub mnuContextFileGo_Click(sender As System.Object, e As System.EventArgs) Handles mnuContextFileGo.Click
    If Not mnuContextFile.SourceControl.Name = lvFiles.Name Then Return
    If Not Settings.FlatView Then Return
    RenderDir(IO.Path.GetDirectoryName(CType(lvFiles.SelectedItems(0).Tag, ZIP.FileSystemEntry).Name))
  End Sub

  Private Sub mnuContextFileProperties_Click(sender As System.Object, e As System.EventArgs) Handles mnuContextFileProperties.Click
    If mnuContextFile.SourceControl.Name = lvFiles.Name Then
      lvFiles_Properties()
    ElseIf mnuContextFile.SourceControl.Name = tvExplorer.Name Then
      tvExplorer_Properties()
    End If
  End Sub

  Private Sub mnuContextView_Popup(sender As Object, e As System.EventArgs) Handles mnuContextView.Popup
    mnuContextViewIconsLarge.Checked = mnuViewIconsLarge.Checked
    mnuContextViewIconsSmall.Checked = mnuViewIconsSmall.Checked
    mnuContextViewIconsList.Checked = mnuViewIconsList.Checked
    mnuContextViewIconsDetails.Checked = mnuViewIconsDetails.Checked
    mnuContextViewIconsTile.Checked = mnuViewIconsTile.Checked
    mnuContextViewSortName.Checked = mnuViewSortName.Checked
    mnuContextViewSortType.Checked = mnuViewSortType.Checked
    mnuContextViewSortSize.Checked = mnuViewSortSize.Checked
    mnuContextViewSortOrder.Checked = mnuViewSortOrder.Checked
    mnuContextViewTree.Checked = Settings.TreeView
    mnuContextViewFlat.Checked = Settings.FlatView
    mnuContextViewGoRoot.Enabled = mnuViewGoRoot.Enabled
    mnuContextViewGoParent.Enabled = mnuViewGoParent.Enabled
  End Sub

  Private Sub mnuContextHistoryItem_Click(sender As Object, e As EventArgs)
    Dim idx As Integer = CType(sender, MenuItem).Tag
    iDirHistIndex = idx
    RenderDir(sDirHistory(iDirHistIndex), True)
  End Sub
#End Region
#End Region

#Region "UI"
  Private Sub frmViewer_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    If sExtractHistory.LongCount < 1 Then Return
    For I As Long = 0 To sExtractHistory.LongCount - 1
      Dim sTemp As String = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, sExtractHistory(I))
      If Not IO.File.Exists(sTemp) Then Continue For
      Try
        IO.File.Delete(sTemp)
      Catch ex As Exception
      End Try
    Next
  End Sub

  Private Sub frmViewer_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
    regHistory.Add("^.*$")
    Me.Size = Settings.WindowSize
    pnlMain.SplitterDistance = Settings.TreeWidth
    Select Case Settings.IconMethod
      Case View.LargeIcon : mnuViewIconsLarge.PerformClick()
      Case View.SmallIcon : mnuViewIconsSmall.PerformClick()
      Case View.List : mnuViewIconsList.PerformClick()
      Case View.Details : mnuViewIconsDetails.PerformClick()
      Case View.Tile : mnuViewIconsTile.PerformClick()
    End Select
    Select Case Settings.SortMethod
      Case "Name" : mnuViewSortName.PerformClick()
      Case "Type" : mnuViewSortType.PerformClick()
      Case "Size" : mnuViewSortSize.PerformClick()
      Case "Order" : mnuViewSortOrder.PerformClick()
    End Select
    mnuViewToolbar.Checked = Settings.ShowToolbar
    tbNav.Visible = mnuViewToolbar.Checked
    mnuViewFlat.Checked = Settings.FlatView
    ToggleFlat()
    mnuViewTree.Checked = Settings.TreeView
    ToggleTree(mnuViewTree.Checked)
    isLoaded = True
    If My.Application.CommandLineArgs.LongCount = 1 Then
      Dim sLoad As String = My.Application.CommandLineArgs(0)
      If IO.File.Exists(sLoad) Then LoadFile(sLoad)
    End If
  End Sub

  Private Sub frmViewer_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
    Dim newWidth As Integer = pnlUI.TopToolStripPanel.ClientRectangle.Width - 12
    For I As Integer = 0 To tbNav.Items.Count - 1
      If tbNav.Items(I).Name = txtAddress.Name Then Continue For
      newWidth -= tbNav.Items(I).Width
    Next
    If newWidth < 150 Then newWidth = 150
    If newWidth > 400 Then newWidth = 400
    If Not txtAddress.Width = newWidth Then txtAddress.Width = newWidth
  End Sub

  Private Sub frmViewer_ResizeEnd(sender As Object, e As System.EventArgs) Handles Me.ResizeEnd
    If Me.WindowState = FormWindowState.Maximized Then Return
    If Not isLoaded Then Return
    Settings.WindowSize = Me.Size
  End Sub

#Region "Toolbar"
  Private Sub cmdBack_Click(sender As System.Object, e As System.EventArgs) Handles cmdBack.Click
    If iDirHistIndex < 1 Then Return
    iDirHistIndex -= 1
    RenderDir(sDirHistory(iDirHistIndex), True)
  End Sub

  Private Sub cmdBack_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles cmdBack.MouseUp
    If Not e.Button = Windows.Forms.MouseButtons.Right Then Return
    If Not cmdBack.Enabled Then Return
    If iDirHistIndex < 1 Then Return
    mnuContextHistory.MenuItems.Clear()
    Dim iCt As Integer = 0
    For I As Long = iDirHistIndex - 1 To 0 Step -1
      iCt += 1
      mnuContextHistory.MenuItems.Add(New MenuItem(sDirHistory(I), AddressOf mnuContextHistoryItem_Click) With {.Tag = I})
      If iCt >= 5 Then Exit For
    Next
    mnuContextHistory.Show(tbNav, New Point(tbNav.Left, tbNav.Top + tbNav.Height))
  End Sub

  Private Sub cmdForward_Click(sender As System.Object, e As System.EventArgs) Handles cmdForward.Click
    If iDirHistIndex > sDirHistory.LongCount - 1 Then Return
    iDirHistIndex += 1
    RenderDir(sDirHistory(iDirHistIndex), True)
  End Sub

  Private Sub cmdForward_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles cmdForward.MouseUp
    If Not e.Button = Windows.Forms.MouseButtons.Right Then Return
    If Not cmdForward.Enabled Then Return
    If iDirHistIndex > sDirHistory.LongCount - 1 Then Return
    mnuContextHistory.MenuItems.Clear()
    Dim iCt As Integer = 0
    For I As Long = iDirHistIndex + 1 To sDirHistory.LongCount - 1
      iCt += 1
      mnuContextHistory.MenuItems.Add(New MenuItem(sDirHistory(I), AddressOf mnuContextHistoryItem_Click) With {.Tag = I})
      If iCt >= 5 Then Exit For
    Next
    mnuContextHistory.Show(tbNav, New Point(tbNav.Left, tbNav.Top + tbNav.Height))
  End Sub

  Private Sub cmdUp_Click(sender As System.Object, e As System.EventArgs) Handles cmdUp.Click
    sArchiveDir = IO.Path.GetDirectoryName(sArchiveDir)
    RenderDir(sArchiveDir)
  End Sub

  Private Sub txtAddress_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtAddress.KeyUp
    If e.KeyCode = Keys.Enter Then
      e.Handled = True
      e.SuppressKeyPress = True
      cmdGo.PerformClick()
      Return
    End If
  End Sub

  Private Sub txtAddress_TextChanged(sender As Object, e As System.EventArgs) Handles txtAddress.TextChanged
    cmdGo.Enabled = txtAddress.Enabled And Not String.IsNullOrEmpty(txtAddress.Text)
  End Sub

  Private Sub cmdGo_Click(sender As System.Object, e As System.EventArgs) Handles cmdGo.Click
    txtAddress.Text = txtAddress.Text.Replace("/", IO.Path.DirectorySeparatorChar)
    While txtAddress.Text.Length > 1 AndAlso txtAddress.Text.EndsWith(IO.Path.DirectorySeparatorChar)
      txtAddress.Text = txtAddress.Text.Substring(0, txtAddress.Text.Length - 1)
    End While
    RenderDir(txtAddress.Text)
  End Sub

  Private Sub cmdExtract_Click(sender As System.Object, e As System.EventArgs) Handles cmdExtract.Click
    lvFiles_Extract()
  End Sub

  Private Sub cmdProperties_Click(sender As System.Object, e As System.EventArgs) Handles cmdProperties.Click
    lvFiles_Properties()
  End Sub

#End Region

  Private Sub pnlMain_SplitterMoved(sender As System.Object, e As System.Windows.Forms.SplitterEventArgs) Handles pnlMain.SplitterMoved
    If Not isLoaded Then Return
    Settings.TreeWidth = pnlMain.SplitterDistance
  End Sub

  Private Sub tvExplorer_AfterSelect(sender As System.Object, e As System.Windows.Forms.TreeViewEventArgs) Handles tvExplorer.AfterSelect
    If tBusy Then Return
    RenderDir(e.Node.Name)
  End Sub

  Private Sub tvExplorer_NodeMouseClick(sender As Object, e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvExplorer.NodeMouseClick
    If Not e.Button = Windows.Forms.MouseButtons.Right Then Return
    mnuContextFileOpen.Enabled = True
    mnuContextFileExpand.Visible = True
    mnuContextFileExpand.Enabled = Not e.Node.IsExpanded
    mnuContextFileExpandAll.Visible = True
    mnuContextFileExpandAll.Enabled = Not AllExpanded(e.Node)
    mnuContextFileCollapse.Visible = True
    mnuContextFileCollapse.Enabled = e.Node.IsExpanded
    mnuContextFileCollapseAll.Visible = True
    mnuContextFileCollapseAll.Enabled = e.Node.IsExpanded
    mnuContextFileSpace2.Visible = True
    mnuContextFileGo.Visible = False
    mnuContextFileProperties.Enabled = True
    mnuContextFile.Tag = e.Node.Name
    mnuContextFile.Show(tvExplorer, e.Location)
  End Sub

  Private Function AllExpanded(node As TreeNode) As Boolean
    If node.Nodes.Count < 1 Then Return True
    If Not node.IsExpanded Then Return False
    For I As Integer = 0 To node.Nodes.Count - 1
      If Not AllExpanded(node.Nodes(I)) Then Return False
    Next
    Return True
  End Function

  Private Sub lvFiles_ColumnClick(sender As Object, e As System.Windows.Forms.ColumnClickEventArgs) Handles lvFiles.ColumnClick
    Select Case e.Column
      Case 0 : mnuViewSortName.PerformClick()
      Case 1 : mnuViewSortType.PerformClick()
      Case 2, 3 : mnuViewSortSize.PerformClick()
    End Select
  End Sub

  Private Sub lvFiles_ColumnWidthChanging(sender As Object, e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles lvFiles.ColumnWidthChanging
    e.Cancel = True
  End Sub

  Private Sub lvFiles_DoubleClick(sender As Object, e As System.EventArgs) Handles lvFiles.DoubleClick
    If lvFiles.SelectedItems.Count < 1 Then Return
    Dim canOpen As Boolean = False
    If lvFiles.SelectedItems.Count = 1 Then
      canOpen = True
    Else
      canOpen = True
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zItem As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        If zItem.GetType Is GetType(ZIP.FileSystemDirectory) Then
          canOpen = False
          Exit For
        End If
      Next
    End If
    If canOpen Then lvFiles_Open()
  End Sub

  Private Sub lvFiles_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles lvFiles.DragDrop
    If Not e.Data.GetDataPresent(DataFormats.FileDrop, False) Then Return
    Dim eData As String() = e.Data.GetData(DataFormats.FileDrop, False)
    If eData Is Nothing OrElse Not eData.LongLength = 1 Then Return
    If Not IO.File.Exists(eData(0)) Then Return
    LoadFile(eData(0))
  End Sub

  Private Sub lvFiles_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles lvFiles.DragEnter
    If Not e.Data.GetDataPresent(DataFormats.FileDrop, False) Then Return
    e.Effect = DragDropEffects.Copy
  End Sub

  Private Sub lvFiles_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles lvFiles.MouseUp
    If Not e.Button = Windows.Forms.MouseButtons.Right Then Return
    If lvFiles.SelectedItems.Count < 1 Then
      mnuContextView.Show(lvFiles, e.Location)
      Return
    End If
    Dim canOpen As Boolean = False
    If lvFiles.SelectedItems.Count = 1 Then
      canOpen = True
    Else
      canOpen = True
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zItem As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        If zItem.GetType Is GetType(ZIP.FileSystemDirectory) Then
          canOpen = False
          Exit For
        End If
      Next
    End If
    If canOpen Then
      mnuContextFileOpen.DefaultItem = True
      mnuContextFileExtract.DefaultItem = False
    Else
      mnuContextFileOpen.DefaultItem = False
      mnuContextFileExtract.DefaultItem = True
    End If
    mnuContextFileOpen.Enabled = canOpen
    mnuContextFileExpand.Visible = False
    mnuContextFileExpandAll.Visible = False
    mnuContextFileCollapse.Visible = False
    mnuContextFileCollapseAll.Visible = False
    mnuContextFileSpace2.Visible = False
    mnuContextFileGo.Visible = lvFiles.SelectedItems.Count = 1 AndAlso Settings.FlatView
    mnuContextFileProperties.Enabled = lvFiles.SelectedItems.Count = 1
    mnuContextFile.Tag = Nothing
    mnuContextFile.Show(lvFiles, e.Location)
  End Sub

  Private Sub lvFiles_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lvFiles.SelectedIndexChanged
    DoSelectionUpdate()
  End Sub

  Private Sub lvFiles_SizeChanged(sender As Object, e As System.EventArgs) Handles lvFiles.SizeChanged
    ResizeColumns()
  End Sub

  Private Sub ResizeColumns()
    lvFiles.SuspendLayout()
    If lvFiles.Items.Count = 0 Then
      lvFiles.Columns(1).Width = 50
      lvFiles.Columns(2).Width = 50
      lvFiles.Columns(3).Width = 75
    Else
      lvFiles.Columns(1).AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
      lvFiles.Columns(2).AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
      lvFiles.Columns(3).AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
    End If
    Dim firstWidth As Integer = lvFiles.ClientSize.Width - (lvFiles.Columns(1).Width + lvFiles.Columns(2).Width + lvFiles.Columns(3).Width)
    If Not lvFiles.Columns(0).Width = firstWidth Then lvFiles.Columns(0).Width = firstWidth
    lvFiles.ResumeLayout()
  End Sub
#End Region

#Region "Layout"
  Private Sub ToggleFlat()
    'redo display of the current path
    RenderDir(sArchiveDir)
  End Sub

  Private Sub ToggleTree(v As Boolean)
    If v Then
      tvExplorer.Visible = True
      pnlMain.Panel1Collapsed = False
    Else
      tvExplorer.Visible = False
      pnlMain.Panel1Collapsed = True
    End If
  End Sub

  Private Sub RenderArchive()
    tvExplorer.Nodes.Clear()
    tvExplorer.SuspendLayout()
    Dim tnArchive As TreeNode = tvExplorer.Nodes.Add(IO.Path.DirectorySeparatorChar, sArchiveName, "archive", "archive")
    tnArchive.ToolTipText = "Size: 0 bytes" & vbNewLine & "Contents: Unknown"
    Dim sDirs As New List(Of String)
    For I As Long = 0 To zArchive.LongLength - 1
      If Not zArchive(I).GetType Is GetType(ZIP.FileSystemDirectory) Then Continue For
      Dim zDir As ZIP.FileSystemDirectory = zArchive(I)
      sDirs.Add(zDir.Name)
      Dim zImg As String = "empty"
      If zDir.DirectoryCount > 0 Then
        zImg = "folders"
      ElseIf zDir.FileCount > 0 Then
        zImg = "files"
      End If
      If zDir.Name = IO.Path.DirectorySeparatorChar Then
        tnArchive.ToolTipText = "Type: Archive" & vbNewLine &
                                "Size: " & ByteSize(zDir.UncompressedLength) & " (" & Math.Floor((zDir.CompressedLength / zDir.UncompressedLength) * 100) & "%)" & vbNewLine &
                                "Contents: " & zDir.FileCount & " file" & IIf(zDir.FileCount = 1, "", "s") & ", " & zDir.DirectoryCount & " folder" & IIf(zDir.DirectoryCount = 1, "", "s")
      Else
        Dim zParent As TreeNode() = tvExplorer.Nodes.Find(IO.Path.GetDirectoryName(zDir.Name), True)
        If Not zParent.LongLength = 1 Then Stop
        Dim zChild As TreeNode = zParent(0).Nodes.Add(zDir.Name, IO.Path.GetFileName(zDir.Name), zImg, zImg)
        zChild.ToolTipText = "Type: " & ZIP.GetRegTypeForDirectory() & vbNewLine &
                             "Size: " & ByteSize(zDir.UncompressedLength) & " (" & Math.Floor((zDir.CompressedLength / zDir.UncompressedLength) * 100) & "%)" & vbNewLine &
                             "Contents: " & zDir.FileCount & " file" & IIf(zDir.FileCount = 1, "", "s") & ", " & zDir.DirectoryCount & " folder" & IIf(zDir.DirectoryCount = 1, "", "s")
      End If
    Next
    txtAddress.AutoCompleteCustomSource.Clear()
    txtAddress.AutoCompleteCustomSource.AddRange(sDirs.ToArray)
    tnArchive.Expand()
    tvExplorer.ResumeLayout()
    RenderDir(IO.Path.DirectorySeparatorChar)
  End Sub

  Private Function AddIcon(sFile As String) As String
    Dim sType As String = IO.Path.GetExtension(sFile)
    If String.IsNullOrEmpty(sType) OrElse Not sType.Substring(0, 1) = "." Then
      Return "BLANK"
    Else
      sType = sType.Substring(1).ToLower
      If Not imlFileList.Images.ContainsKey(sType) Then
        Try
          Dim sTemp As String = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, "forico." & sType)
          My.Computer.FileSystem.WriteAllText(sTemp, "DUMMY FILE", False)
          imlFileList.Images.Add(sType, GetFileIcon(sTemp))
          imlFileList32.Images.Add(sType, GetFileIcon32(sTemp))
          My.Computer.FileSystem.DeleteFile(sTemp)
        Catch ex As Exception

        End Try
      End If
      Return sType
    End If
  End Function

  Private Sub RenderDir(sDir As String, Optional usedHistory As Boolean = False)
    If zArchive Is Nothing Then Return
    Dim fNodes As TreeNode() = tvExplorer.Nodes.Find(sDir, True)
    If fNodes.Length = 0 Then
      Beep()
      Return
    End If
    tBusy = True
    If fNodes.LongLength = 1 Then
      If tvExplorer.SelectedNode Is Nothing OrElse Not tvExplorer.SelectedNode.Name = fNodes(0).Name Then
        fNodes(0).Expand()
        tvExplorer.SelectedNode = fNodes(0)
        tBusy = False
        'Return
      End If
    End If
    tBusy = False
    lvFiles.SuspendLayout()
    lvFiles.Items.Clear()
    lblSelCount.Text = "No objects selected"
    imlFileList.Images.Clear()
    imlFileList.Images.Add("BLANK", My.Resources.blank16)
    imlFileList.Images.Add("EMPTY", My.Resources.empty16)
    imlFileList.Images.Add("FILES", My.Resources.files16)
    imlFileList.Images.Add("FOLDERS", My.Resources.folders16)
    imlFileList32.Images.Clear()
    imlFileList32.Images.Add("BLANK", My.Resources.blank32)
    imlFileList32.Images.Add("EMPTY", My.Resources.empty32)
    imlFileList32.Images.Add("FILES", My.Resources.files32)
    imlFileList32.Images.Add("FOLDERS", My.Resources.folders32)
    If String.IsNullOrEmpty(sDir) Then
      lvFiles.ResumeLayout()
      Return
    End If
    If Not sDir.Substring(0, 1) = IO.Path.DirectorySeparatorChar Then
      lvFiles.ResumeLayout()
      Return
    End If
    Dim addToHistory = True
    If usedHistory Then
      addToHistory = False
    ElseIf sDirHistory.LongCount > 0 Then
      If sDirHistory(iDirHistIndex) = sDir Then
        addToHistory = False
      ElseIf Not iDirHistIndex = sDirHistory.LongCount - 1 Then
        sDirHistory.RemoveRange(iDirHistIndex + 1, sDirHistory.LongCount - (iDirHistIndex + 1))
      End If
    End If
    If addToHistory Then
      sDirHistory.Add(sDir)
      iDirHistIndex = sDirHistory.LongCount - 1
    End If
    If sDirHistory.LongCount = 0 Then
      cmdBack.Enabled = False
      cmdForward.Enabled = False
    Else
      cmdBack.Enabled = Not iDirHistIndex < 1
      cmdForward.Enabled = Not iDirHistIndex = sDirHistory.LongCount - 1
    End If
    Dim sPath As String = sDir
    Dim addList As New List(Of ListViewItem)
    If Settings.FlatView Then
      Dim subDirPos As Integer = 1
      If Not sDir = IO.Path.DirectorySeparatorChar Then subDirPos = sDir.Length + 1
      For I As Long = 0 To zArchive.LongLength - 1
        If zArchive(I).Name = IO.Path.DirectorySeparatorChar Then Continue For
        If Not String.IsNullOrEmpty(sPath) AndAlso Not sPath = IO.Path.DirectorySeparatorChar Then
          Dim archiveDir As String = IO.Path.GetDirectoryName(zArchive(I).Name)
          If archiveDir.Length < sPath.Length Then Continue For
          Dim isInPath As Boolean = False
          Do
            If archiveDir = sPath Then
              isInPath = True
              Exit Do
            End If
            archiveDir = IO.Path.GetDirectoryName(archiveDir)
          Loop Until String.IsNullOrEmpty(archiveDir)
          If Not isInPath Then Continue For
        End If
        Dim lvAdd As New ListViewItem(zArchive(I).Name.Substring(subDirPos))
        lvAdd.Tag = zArchive(I)
        Dim sType As String = "BLANK"
        If zArchive(I).GetType Is GetType(ZIP.FileSystemFile) Then
          Dim ZFile As ZIP.FileSystemFile = zArchive(I)
          sType = AddIcon(ZFile.Name)
          lvAdd.ToolTipText = "Type: " & ZFile.FileType & vbNewLine &
                              "Size: " & ByteSize(ZFile.UncompressedLength) & " (" & Math.Floor((ZFile.CompressedLength / ZFile.UncompressedLength) * 100) & "%)"
        ElseIf zArchive(I).GetType Is GetType(ZIP.FileSystemDirectory) Then
          Dim ZDir As ZIP.FileSystemDirectory = zArchive(I)
          If ZDir.DirectoryCount > 0 Then
            sType = "FOLDERS"
          ElseIf ZDir.FileCount > 0 Then
            sType = "FILES"
          Else
            sType = "EMPTY"
          End If
          lvAdd.ToolTipText = "Type: " & ZIP.GetRegTypeForDirectory() & vbNewLine &
                              "Size: " & ByteSize(ZDir.UncompressedLength) & " (" & Math.Floor((ZDir.CompressedLength / ZDir.UncompressedLength) * 100) & "%)" & vbNewLine &
                              "Contents: " & ZDir.FileCount & " file" & IIf(ZDir.FileCount = 1, "", "s") & ", " & ZDir.DirectoryCount & " folder" & IIf(ZDir.DirectoryCount = 1, "", "s")
        End If
        lvAdd.ImageKey = sType
        If sType = "BLANK" Then
          lvAdd.SubItems.Add("File")
        ElseIf sType = "FOLDERS" Or sType = "FILES" Or sType = "EMPTY" Then
          lvAdd.SubItems.Add("Folder")
        Else
          lvAdd.SubItems.Add(sType.ToUpper & " File")
        End If
        lvAdd.SubItems.Add(ByteSize(zArchive(I).UncompressedLength))
        If zArchive(I).GetType Is GetType(ZIP.FileSystemFile) AndAlso CType(zArchive(I), ZIP.FileSystemFile).Compression = 0 Then
          lvAdd.SubItems.Add("N/A")
        Else
          lvAdd.SubItems.Add(ByteSize(zArchive(I).CompressedLength) & " (" & Math.Floor((zArchive(I).CompressedLength / zArchive(I).UncompressedLength) * 100) & "%)")
        End If
        addList.Add(lvAdd)
      Next
    Else
      For I As Long = 0 To zArchive.LongLength - 1
        Dim archiveDir As String = IO.Path.GetDirectoryName(zArchive(I).Name)
        If Not archiveDir = sPath Then Continue For
        Dim lvAdd As New ListViewItem(IO.Path.GetFileName(zArchive(I).Name))
        lvAdd.Tag = zArchive(I)
        Dim sType As String = "BLANK"
        If zArchive(I).GetType Is GetType(ZIP.FileSystemFile) Then
          Dim ZFile As ZIP.FileSystemFile = zArchive(I)
          sType = AddIcon(ZFile.Name)
          lvAdd.ToolTipText = "Type: " & ZFile.FileType & vbNewLine &
                              "Size: " & ByteSize(ZFile.UncompressedLength) & " (" & Math.Floor((ZFile.CompressedLength / ZFile.UncompressedLength) * 100) & "%)"
        ElseIf zArchive(I).GetType Is GetType(ZIP.FileSystemDirectory) Then
          Dim ZDir As ZIP.FileSystemDirectory = zArchive(I)
          If ZDir.DirectoryCount > 0 Then
            sType = "FOLDERS"
          ElseIf ZDir.FileCount > 0 Then
            sType = "FILES"
          Else
            sType = "EMPTY"
          End If
          lvAdd.ToolTipText = "Type: " & ZIP.GetRegTypeForDirectory() & vbNewLine &
                              "Size: " & ByteSize(ZDir.UncompressedLength) & " (" & Math.Floor((ZDir.CompressedLength / ZDir.UncompressedLength) * 100) & "%)" & vbNewLine &
                              "Contents: " & ZDir.FileCount & " file" & IIf(ZDir.FileCount = 1, "", "s") & ", " & ZDir.DirectoryCount & " folder" & IIf(ZDir.DirectoryCount = 1, "", "s")
        End If
        lvAdd.ImageKey = sType
        If sType = "BLANK" Then
          lvAdd.SubItems.Add("File")
        ElseIf sType = "FOLDERS" Or sType = "FILES" Or sType = "EMPTY" Then
          lvAdd.SubItems.Add("Folder")
        Else
          lvAdd.SubItems.Add(sType.ToUpper & " File")
        End If
        lvAdd.SubItems.Add(ByteSize(zArchive(I).UncompressedLength))
        If zArchive(I).GetType Is GetType(ZIP.FileSystemFile) AndAlso CType(zArchive(I), ZIP.FileSystemFile).Compression = 0 Then
          lvAdd.SubItems.Add("N/A")
        Else
          lvAdd.SubItems.Add(ByteSize(zArchive(I).CompressedLength) & " (" & Math.Floor((zArchive(I).CompressedLength / zArchive(I).UncompressedLength) * 100) & "%)")
        End If
        addList.Add(lvAdd)
      Next
    End If
    lvFiles.Items.AddRange(addList.ToArray)

    Dim useSort As String = "Order"
    If mnuViewSortName.Checked Then
      useSort = "Name"
    ElseIf mnuViewSortType.Checked Then
      useSort = "Type"
    ElseIf mnuViewSortSize.Checked Then
      useSort = "Size"
    End If
    lvFiles.ListViewItemSorter = New ListViewItemComparer(useSort, lvFiles.Sorting)
    lvFiles.ResumeLayout()
    sArchiveDir = sDir
    DoSelectionUpdate()
    ResizeColumns()
  End Sub

  Class ListViewItemComparer
    Implements IComparer
    Private useSort As String
    Private useOrder As SortOrder

    Public Sub New(s As String, o As SortOrder)
      useSort = s
      useOrder = o
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
      Dim zX As ZIP.FileSystemEntry = CType(x, ListViewItem).Tag
      Dim zY As ZIP.FileSystemEntry = CType(y, ListViewItem).Tag
      If zX.GetType Is GetType(ZIP.FileSystemDirectory) And Not zY.GetType Is GetType(ZIP.FileSystemDirectory) Then Return -1
      If Not zX.GetType Is GetType(ZIP.FileSystemDirectory) And zY.GetType Is GetType(ZIP.FileSystemDirectory) Then Return 1
      If useSort = "Order" Then
        If zX.Index < zY.Index Then
          If useOrder = SortOrder.Ascending Then Return 1
          Return -1
        ElseIf zX.Index > zY.Index Then
          If useOrder = SortOrder.Ascending Then Return -1
          Return 1
        Else
          Return 0
        End If
      ElseIf useSort = "Name" Then
        If useOrder = SortOrder.Ascending Then
          Return [String].Compare(zY.Name, zX.Name)
        Else
          Return [String].Compare(zX.Name, zY.Name)
        End If
      ElseIf useSort = "Size" Then
        If zX.UncompressedLength < zY.UncompressedLength Then
          If useOrder = SortOrder.Ascending Then Return 1
          Return -1
        ElseIf zX.UncompressedLength > zY.UncompressedLength Then
          If useOrder = SortOrder.Ascending Then Return -1
          Return 1
        Else
          Return 0
        End If
      ElseIf useSort = "Type" Then
        If useOrder = SortOrder.Ascending Then
          Return [String].Compare(IO.Path.GetExtension(zY.Name), IO.Path.GetExtension(zX.Name))
        Else
          Return [String].Compare(IO.Path.GetExtension(zX.Name), IO.Path.GetExtension(zY.Name))
        End If
      End If
      Return 0
    End Function
  End Class

  Private Sub DoSelectionUpdate()
    tmrSelectionUpdate.Stop()
    tmrSelectionUpdate.Start()
  End Sub

  Private Sub tmrSelectionUpdate_Tick(sender As System.Object, e As System.EventArgs) Handles tmrSelectionUpdate.Tick
    tmrSelectionUpdate.Stop()
    SelectionUpdate()
  End Sub

  Private Sub SelectionUpdate()
    If String.IsNullOrEmpty(mArchivePath) OrElse zArchive Is Nothing OrElse zArchive.LongLength < 1 Then
      mnuFileCloseArchive.Enabled = False
      mnuFileOpenFile.Enabled = False
      mnuFileExtractFile.Enabled = False
      mnuFilePropertiesFile.Enabled = False

      mnuEditAll.Enabled = False
      mnuEditNone.Enabled = False
      mnuEditInvert.Enabled = False
      mnuEditSelect.Enabled = False
      mnuEditDeselect.Enabled = False
      mnuEditSelType.Enabled = False
      mnuEditDeselType.Enabled = False

      cmdExtract.Enabled = False
      cmdProperties.Enabled = False
      Return
    End If

    Dim canOpen As Boolean = True
    Dim sFiles As New List(Of String)
    If lvFiles.SelectedItems.Count = 0 Then
      lblSelCount.Text = "No objects selected"
      canOpen = False
    ElseIf lvFiles.SelectedItems.Count = 1 Then
      lblSelCount.Text = "1 object selected"
      Dim zItem As ZIP.FileSystemEntry = lvFiles.SelectedItems(0).Tag
      sFiles.Add(zItem.Name)
    Else
      lblSelCount.Text = lvFiles.SelectedItems.Count & " objects selected"
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zItem As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        sFiles.Add(zItem.Name)
        If canOpen AndAlso zItem.GetType Is GetType(ZIP.FileSystemDirectory) Then canOpen = False
      Next
    End If
    Dim szU As ULong = 0
    Dim szC As ULong = 0
    If sFiles.LongCount > 0 Then
      For I As Long = 0 To zArchive.LongLength - 1
        If Not sFiles.Contains(zArchive(I).Name) Then Continue For
        szU += zArchive(I).UncompressedLength
        szC += zArchive(I).CompressedLength
      Next
    Else
      For I As Long = 0 To zArchive.LongLength - 1
        If Not zArchive(I).Name = sArchiveDir Then Continue For
        szU += zArchive(I).UncompressedLength
        szC += zArchive(I).CompressedLength
      Next
    End If
    lblSelSize.Text = ByteSize(szC)
    lblSelUncompressed.Text = ByteSize(szU)

    mnuFileCloseArchive.Enabled = True
    mnuFileOpenFile.Enabled = canOpen
    mnuFileExtractFile.Enabled = lvFiles.SelectedItems.Count > 0
    mnuFilePropertiesFile.Enabled = lvFiles.SelectedItems.Count = 1
    cmdExtract.Enabled = lvFiles.SelectedItems.Count > 0
    cmdProperties.Enabled = lvFiles.SelectedItems.Count = 1

    mnuEditAll.Enabled = lvFiles.SelectedItems.Count < lvFiles.Items.Count
    mnuEditNone.Enabled = lvFiles.SelectedItems.Count > 0
    mnuEditInvert.Enabled = True
    mnuEditSelect.Enabled = lvFiles.SelectedItems.Count < lvFiles.Items.Count
    mnuEditDeselect.Enabled = lvFiles.SelectedItems.Count > 0
    mnuEditSelType.Enabled = lvFiles.SelectedItems.Count < lvFiles.Items.Count
    mnuEditDeselType.Enabled = lvFiles.SelectedItems.Count > 0
  End Sub
#End Region

#Region "Parsing"
  Private Sub LoadFile(path As String)
    Erase zArchive
    iDirHistIndex = 0
    sDirHistory.Clear()
    cmdBack.Enabled = False
    cmdForward.Enabled = False
    tvExplorer.Nodes.Clear()
    lvFiles.Items.Clear()
    sArchiveDir = IO.Path.DirectorySeparatorChar
    sArchivePath = path
    sArchiveName = IO.Path.GetFileName(path)
    Me.Text = "PMRes Viewer - [" & path & "]"
    lblSelUncompressed.Text = "0%"
    lblSelSize.Visible = False
    lblSelCount.Text = "Reading Archive Structure..."
    pbActivity.Visible = True
    Dim bArchive() As Byte = IO.File.ReadAllBytes(path)
    zArchive = ZIP.ReadFileSystem(bArchive, AddressOf ShowProgress)
    lblSelUncompressed.Text = "0 bytes"
    lblSelSize.Visible = True
    lblSelSize.Text = "0 bytes"
    lblSelCount.Text = "No objects selected"
    If zArchive.Length = 0 Then
      pbActivity.Visible = False
      SuperMsgBox(Me, "Error", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Error, "Unable to load.", "No files could be extracted from the archive.", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok, path)
      Return
    End If
    RenderArchive()
    pbActivity.Visible = False
  End Sub

  Private Sub ShowProgress(ByVal bPercent As Byte, ByVal sName As String)
    pbActivity.Value = bPercent
    If sName = "Error" Then
      lblSelCount.Text = "Error Reading Archive!"
      lblSelUncompressed.Text = "0 bytes"
    Else
      lblSelCount.Text = "Reading " & sName & "..."
      lblSelUncompressed.Text = bPercent & "%"
    End If
  End Sub
#End Region

#Region "Extraction"
  Private Sub lvFiles_Open()
    If lvFiles.SelectedItems.Count = 0 Then Return
    If lvFiles.SelectedItems.Count = 1 Then
      Dim zFile As ZIP.FileSystemEntry = lvFiles.SelectedItems(0).Tag
      If zFile.GetType Is GetType(ZIP.FileSystemDirectory) Then
        RenderDir(zFile.Name)
      Else
        Dim sName As String = IO.Path.GetFileName(zFile.Name)
        Dim iCopy As Integer = 0
        Dim sTemp As String = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, sName)
        While IO.File.Exists(sTemp)
          iCopy += 1
          sName = iCopy & "-" & IO.Path.GetFileName(zFile.Name)
          sTemp = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, sName)
        End While
        My.Computer.FileSystem.WriteAllBytes(sTemp, CType(zFile, ZIP.FileSystemFile).Data, False)
        If Not sExtractHistory.Contains(sName) Then sExtractHistory.Add(sName)
        NativeMethods.ShellExecute(0, vbNullString, sTemp, vbNullString, vbNullString, vbNormalFocus)
      End If
    Else
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zFile As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        If zFile.GetType Is GetType(ZIP.FileSystemDirectory) Then Continue For
        Dim sName As String = IO.Path.GetFileName(zFile.Name)
        Dim iCopy As Integer = 0
        Dim sTemp As String = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, sName)
        While IO.File.Exists(sTemp)
          iCopy += 1
          sName = iCopy & "-" & IO.Path.GetFileName(zFile.Name)
          sTemp = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, sName)
        End While
        My.Computer.FileSystem.WriteAllBytes(sTemp, CType(zFile, ZIP.FileSystemFile).Data, False)
        If Not sExtractHistory.Contains(sName) Then sExtractHistory.Add(sName)
        NativeMethods.ShellExecute(0, vbNullString, sTemp, vbNullString, vbNullString, vbNormalFocus)
      Next
    End If
  End Sub

  Private Sub lvFiles_Extract()
    If lvFiles.SelectedItems.Count = 0 Then Return
    If lvFiles.SelectedItems.Count = 1 Then
      Dim zFile As ZIP.FileSystemEntry = lvFiles.SelectedItems(0).Tag
      If zFile.GetType Is GetType(ZIP.FileSystemDirectory) Then
        'extract folder
        Dim subDirPos As Integer = 1
        If Not sArchiveDir = IO.Path.DirectorySeparatorChar Then subDirPos = sArchiveDir.Length + 1
        ExtractFolder(zFile, subDirPos)
      ElseIf zFile.GetType Is GetType(ZIP.FileSystemFile) Then
        'extract file
        ExtractFile(zFile)
      End If
    Else
      'extract bulk
      Dim zFiles As New List(Of ZIP.FileSystemEntry)
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zFile As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        zFiles.Add(zFile)
      Next
      Extract(zFiles.ToArray)
    End If
  End Sub

  Private Sub tvExplorer_Extract()
    Dim sFind As String = mnuContextFile.Tag
    Dim zFile As ZIP.FileSystemDirectory = Nothing
    For I As Long = 0 To zArchive.LongLength - 1
      If Not zArchive(I).GetType Is GetType(ZIP.FileSystemDirectory) Then Continue For
      If Not zArchive(I).Name = sFind Then Continue For
      zFile = zArchive(I)
      Exit For
    Next
    If zFile Is Nothing Then Return
    Dim subDirPos As Integer = 1
    If Not mnuContextFile.Tag = IO.Path.DirectorySeparatorChar Then
      subDirPos = sFind.LastIndexOf(IO.Path.DirectorySeparatorChar) + 1
    End If
    ExtractFolder(zFile, subDirPos)
  End Sub

  Private Sub Extract(zFiles As ZIP.FileSystemEntry())
    Dim toPath As String = Nothing
    Dim andSubdirs As TriState = TriState.UseDefault
    For I As Long = 0 To zFiles.LongLength - 1
      If zFiles(I).GetType Is GetType(ZIP.FileSystemDirectory) Then
        If CType(zFiles(I), ZIP.FileSystemDirectory).DirectoryCount > 0 Then
          andSubdirs = TriState.True
          Exit For
        End If
      End If
    Next
    Using cdlSave As New Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
      cdlSave.IsFolderPicker = True
      cdlSave.Title = "Extract Selection to Location..."
      cdlSave.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
      If Not andSubdirs = TriState.UseDefault Then cdlSave.Controls.Add(New Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogCheckBox("chkSubdirs", "Extract subfolders", Not andSubdirs = TriState.False))
      If Not cdlSave.ShowDialog(Me.Handle) = Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok Then
        Return
      End If
      toPath = cdlSave.FileName
      If String.IsNullOrEmpty(toPath) Then
        Return
      End If
      If Not andSubdirs = TriState.UseDefault Then
        If CType(cdlSave.Controls("chkSubdirs"), Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogCheckBox).IsChecked Then
          andSubdirs = TriState.True
        Else
          andSubdirs = TriState.False
        End If
      End If
    End Using
    tvExplorer.Enabled = False
    lvFiles.Enabled = False
    lblSelSize.Visible = False
    lblSelUncompressed.Visible = False
    pbActivity.Visible = True
    pbActivity.Style = ProgressBarStyle.Marquee
    lblSelCount.Text = "Extracting..."
    'extraction process is messier on flat view
    If Settings.FlatView Then
      Dim subDirPos As Integer = 1
      If Not sArchiveDir = IO.Path.DirectorySeparatorChar Then subDirPos = sArchiveDir.Length + 1
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zFile As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        lblSelCount.Text = "Extracting " & zFile.Name & "..."
        If zFile.GetType Is GetType(ZIP.FileSystemDirectory) Then
          ExtractFolder(zFile, subDirPos, toPath, andSubdirs)
        ElseIf zFile.GetType Is GetType(ZIP.FileSystemFile) Then
          ExtractFile(zFile, IO.Path.Combine(toPath, zFile.Name.Substring(subDirPos)))
        End If
        If I Mod 20 = 19 Then Application.DoEvents()
      Next
    Else
      Dim subDirPos As Integer = 1
      If Not sArchiveDir = IO.Path.DirectorySeparatorChar Then subDirPos = sArchiveDir.Length + 1
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zFile As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        lblSelCount.Text = "Extracting " & zFile.Name & "..."
        If zFile.GetType Is GetType(ZIP.FileSystemDirectory) Then
          ExtractFolder(zFile, subDirPos, toPath, andSubdirs)
        ElseIf zFile.GetType Is GetType(ZIP.FileSystemFile) Then
          ExtractFile(zFile, IO.Path.Combine(toPath, IO.Path.GetFileName(zFile.Name)))
        End If
        If I Mod 20 = 19 Then Application.DoEvents()
      Next
    End If
    lblSelSize.Visible = True
    lblSelUncompressed.Visible = True
    pbActivity.Style = ProgressBarStyle.Continuous
    pbActivity.Visible = False
    lblSelCount.Text = "No objects selected"
    tvExplorer.Enabled = True
    lvFiles.Enabled = True
    If SuperMsgBox(Me,
      "Extraction Complete",
      Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information,
      "The extraction process is complete.",
      "Would you like to view the extracted files?",
      Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes Or Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No Then Return
    Process.Start(GetDefaultShell(), """" & toPath & """")
    DoSelectionUpdate()
  End Sub

  Private Sub ExtractFolder(zFile As ZIP.FileSystemDirectory, Optional subDirPos As Integer = 1, Optional toPath As String = Nothing, Optional andSubdirs As TriState = TriState.UseDefault)
    Dim showPrompts As Boolean = String.IsNullOrEmpty(toPath)
    If showPrompts Then
      Using cdlSave As New Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
        cdlSave.IsFolderPicker = True
        cdlSave.Title = "Extract Directory to Location..."
        cdlSave.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        If zFile.DirectoryCount > 0 Then cdlSave.Controls.Add(New Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogCheckBox("chkSubdirs", "Extract subfolders", True))
        If Not cdlSave.ShowDialog(Me.Handle) = Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok Then Return
        toPath = cdlSave.FileName
        If String.IsNullOrEmpty(toPath) Then Return
        If zFile.DirectoryCount > 0 Then
          andSubdirs = TriState.True
          If Not CType(cdlSave.Controls("chkSubdirs"), Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogCheckBox).IsChecked Then andSubdirs = TriState.False
        Else
          andSubdirs = TriState.False
        End If
      End Using
    ElseIf zFile.DirectoryCount = 0 Then
      andSubdirs = TriState.False
    End If
    Dim sExtract As New List(Of ZIP.FileSystemFile)
    If andSubdirs = TriState.False Then
      'no subdirs
      For I As Long = 0 To zArchive.LongLength - 1
        If Not zArchive(I).GetType Is GetType(ZIP.FileSystemFile) Then Continue For
        Dim zEntry As ZIP.FileSystemFile = zArchive(I)
        If IO.Path.GetDirectoryName(zEntry.Name) = zFile.Name Then
          sExtract.Add(zEntry)
        End If
      Next
    Else
      'subdirs
      For I As Long = 0 To zArchive.LongLength - 1
        If Not zArchive(I).GetType Is GetType(ZIP.FileSystemFile) Then Continue For
        Dim zEntry As ZIP.FileSystemFile = zArchive(I)
        Dim sEDir As String = IO.Path.GetDirectoryName(zEntry.Name)
        If sEDir.Length < zFile.Name.Length Then Continue For
        If sEDir.Substring(0, zFile.Name.Length) = zFile.Name Then
          sExtract.Add(zEntry)
        End If
      Next
    End If
    If sExtract.LongCount < 1 Then
      For I As Long = 0 To zArchive.LongLength - 1
        If Not zArchive(I).GetType Is GetType(ZIP.FileSystemDirectory) Then Continue For
        Dim zEntry As ZIP.FileSystemDirectory = zArchive(I)
        If zEntry.Name = zFile.Name Then
          sExtract.Add(New ZIP.FileSystemFile() With {.Name = zEntry.Name & IO.Path.DirectorySeparatorChar & "NULL", .Data = Nothing})
        End If
      Next
    End If
    If zFile.Name = IO.Path.DirectorySeparatorChar Then toPath = IO.Path.Combine(toPath, sArchiveName)
    If showPrompts Then
      tvExplorer.Enabled = False
      lvFiles.Enabled = False
      lblSelSize.Visible = False
      lblSelUncompressed.Visible = False
      pbActivity.Visible = True
      pbActivity.Style = ProgressBarStyle.Marquee
      lblSelCount.Text = "Extracting..."
    End If
    For I As Long = 0 To sExtract.LongCount - 1
      lblSelCount.Text = "Extracting " & sExtract(I).Name & "..."
      Dim sFile As String = IO.Path.Combine(toPath, sExtract(I).Name.Substring(subDirPos))
      CreateAllSubdirs(IO.Path.GetDirectoryName(sFile))
      If IO.Path.GetFileName(sFile) = "NULL" And sExtract(I).Data Is Nothing Then Continue For
      My.Computer.FileSystem.WriteAllBytes(sFile, sExtract(I).Data, False)
      IO.File.SetCreationTime(sFile, sExtract(I).Modified)
      IO.File.SetLastWriteTime(sFile, sExtract(I).Modified)
      If I Mod 20 = 19 Then Application.DoEvents()
    Next
    If Not showPrompts Then Return

    lblSelSize.Visible = True
    lblSelUncompressed.Visible = True
    pbActivity.Style = ProgressBarStyle.Continuous
    pbActivity.Visible = False
    lblSelCount.Text = "No objects selected"
    tvExplorer.Enabled = True
    lvFiles.Enabled = True

    If SuperMsgBox(Me,
      "Extraction Complete",
      Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information,
      "The extraction process is complete.",
      "Would you like to view the extracted directory?",
      Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes Or Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No Then Return

    Process.Start(GetDefaultShell(), "/select,""" & IO.Path.Combine(toPath, IO.Path.GetFileName(zFile.Name)) & """")
    DoSelectionUpdate()
  End Sub

  Private Sub ExtractFile(zFile As ZIP.FileSystemFile, Optional toPath As String = Nothing)
    Dim showPrompts As Boolean = String.IsNullOrEmpty(toPath)
    If showPrompts Then
      Using cdlSave As New Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog
        cdlSave.Title = "Extract File to Location..."
        cdlSave.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        cdlSave.DefaultFileName = IO.Path.GetFileName(zFile.Name)
        If Not String.IsNullOrEmpty(IO.Path.GetExtension(zFile.Name)) Then
          cdlSave.Filters.Add(New Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter(zFile.FileType & "s", IO.Path.GetExtension(zFile.Name)))
        End If
        cdlSave.Filters.Add(New Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("All Files", "*.*"))
        If Not cdlSave.ShowDialog(Me.Handle) = Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok Then Return
        toPath = cdlSave.FileName
        If String.IsNullOrEmpty(toPath) Then Return
      End Using
    End If
    CreateAllSubdirs(IO.Path.GetDirectoryName(toPath))
    My.Computer.FileSystem.WriteAllBytes(toPath, zFile.Data, False)
    IO.File.SetCreationTime(toPath, zFile.Modified)
    IO.File.SetLastWriteTime(toPath, zFile.Modified)
    If Not showPrompts Then Return
    If SuperMsgBox(Me,
      "Extraction Complete",
      Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information,
      "The extraction process is complete.",
      "Would you like to view the extracted file?",
      Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes Or Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No Then Return

    Process.Start(GetDefaultShell(), "/select,""" & toPath & """")
    DoSelectionUpdate()
  End Sub
#End Region

#Region "Properties"
  Private Sub lvFiles_Properties()
    If Not lvFiles.SelectedItems.Count = 1 Then Return
    Dim zFSE As ZIP.FileSystemEntry = lvFiles.SelectedItems(0).Tag
    If zFSE.GetType Is GetType(ZIP.FileSystemDirectory) Then
      'folder properties
      Dim zDir As ZIP.FileSystemDirectory = zFSE
      Dim sProps As String = "Full Path: " & zDir.Name
      sProps &= vbNewLine & vbNewLine & "Real Size: " & ByteSize(zDir.UncompressedLength) & " (" & zDir.UncompressedLength & " bytes)"
      If Not zDir.CompressedLength = zDir.UncompressedLength Then
        sProps &= vbNewLine & "Compressed: " & ByteSize(zDir.CompressedLength) & " (" & Math.Floor((zDir.CompressedLength / zDir.UncompressedLength) * 100) & "%)"
      End If
      sProps &= vbNewLine & vbNewLine & "Contains " & zDir.FileCount & " file" & IIf(zDir.FileCount = 1, "", "s") & " and " & zDir.DirectoryCount & " folder" & IIf(zDir.DirectoryCount = 1, "", "s")
      Dim fr As Decimal = zDir.CompressedLength / zDir.UncompressedLength
      If fr > 1 Then fr = -1
      SuperMsgBox(Me, "Folder Properties", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information, IO.Path.GetFileName(zDir.Name), sProps, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Close, , , fr)
    ElseIf zFSE.GetType Is GetType(ZIP.FileSystemFile) Then
      'file properties
      Dim zFile As ZIP.FileSystemFile = zFSE
      Dim sProps As String = "Full Path: " & zFile.Name
      sProps &= vbNewLine & vbNewLine & "File Type: " & zFile.FileType
      sProps &= vbNewLine & GetFileSnippet(zFile)
      sProps &= vbNewLine & vbNewLine & "Real Size: " & ByteSize(zFile.UncompressedLength) & " (" & zFile.UncompressedLength & " bytes)"
      If zFile.Compression = 0 Then
        sProps &= vbNewLine & "Uncompressed"
      Else
        sProps &= vbNewLine & "Compressed: " & ByteSize(zFile.CompressedLength) & " (" & Math.Floor((zFile.CompressedLength / zFile.UncompressedLength) * 100) & "%)"
      End If
      'sProps &= vbNewLine & "Date: " & zFile.Modified.ToString("g")
      sProps &= vbNewLine & vbNewLine & "CRC32: 0x" & Hex(zFile.CRC)

      Dim fr As Decimal = zFile.CompressedLength / zFile.UncompressedLength
      If fr > 1 Then fr = -1
      SuperMsgBox(Me, "File Properties", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information, IO.Path.GetFileName(zFile.Name), sProps, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Close, , , fr)
    End If
  End Sub

  Private Sub tvExplorer_Properties()
    Dim zFSE As ZIP.FileSystemDirectory = Nothing
    For I As Long = 0 To zArchive.LongLength - 1
      If Not zArchive(I).GetType Is GetType(ZIP.FileSystemDirectory) Then Continue For
      If Not zArchive(I).Name = mnuContextFile.Tag Then Continue For
      zFSE = zArchive(I)
      Exit For
    Next
    If zFSE Is Nothing Then Return
    Dim zDir As ZIP.FileSystemDirectory = zFSE
    Dim sTitle As String = IO.Path.GetFileName(zDir.Name)
    If zDir.Name = IO.Path.DirectorySeparatorChar Then sTitle = sArchiveName
    Dim sProps As String
    If zDir.Name = IO.Path.DirectorySeparatorChar Then
      sProps = "Full Path: " & sArchivePath
    Else
      sProps = "Full Path: " & zDir.Name
    End If
    sProps &= vbNewLine & vbNewLine & "Real Size: " & ByteSize(zDir.UncompressedLength) & " (" & zDir.UncompressedLength & " bytes)"
    If Not zDir.CompressedLength = zDir.UncompressedLength Then
      sProps &= vbNewLine & "Compressed: " & ByteSize(zDir.CompressedLength) & " (" & Math.Floor((zDir.CompressedLength / zDir.UncompressedLength) * 100) & "%)"
    End If
    sProps &= vbNewLine & vbNewLine & "Contains " & zDir.FileCount & " file" & IIf(zDir.FileCount = 1, "", "s") & " and " & zDir.DirectoryCount & " folder" & IIf(zDir.DirectoryCount = 1, "", "s")
    Dim fr As Decimal = zDir.CompressedLength / zDir.UncompressedLength
    If fr > 1 Then fr = -1
    SuperMsgBox(Me, "Directory Properties", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information, sTitle, sProps, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Close, , , fr)
  End Sub

  Private Function GetFileSnippet(ZFile As ZIP.FileSystemFile) As String
    Dim catTypeLib As XPCOMTypeLib = GetFileCatTypeLib(ZFile)
    If catTypeLib.Major > 0 Then
      Return "  XPCOM TypeLib: Version " & catTypeLib.Major & "." & catTypeLib.Minor & vbNewLine &
                     "  Interfaces: " & catTypeLib.InterfaceCount
    End If

    Using catRIFF As clsRIFF = GetFileCatRIFF(ZFile)
      If catRIFF IsNot Nothing Then Return GetRIFFData(catRIFF)
    End Using

    Dim catFTYP As clsFTYP = GetFileCatFTYP(ZFile)
    If catFTYP IsNot Nothing Then
      Return "  Encoding: " & Join(catFTYP.Brands, ", ") & vbNewLine &
             "  Resolution: " & catFTYP.Resolution.Width & "x" & catFTYP.Resolution.Height & " pixels" & vbNewLine &
             "  Duration: " & MSToTime(catFTYP.Duration)
    End If

    Dim catImage As ImageData = GetFileCatImage(ZFile)
    If Not String.IsNullOrEmpty(catImage.Format) Then
      Return "  Image Format: " & catImage.Format & vbNewLine &
             "  Dimensions: " & catImage.Dimensions.Width & "x" & catImage.Dimensions.Height & " pixels" & vbNewLine &
             "  Color Quality: " & catImage.ColorQuality
    End If

    Dim catText As String = GetFileCatText(ZFile)
    If Not String.IsNullOrEmpty(catText) Then
      Dim lLines As Long = Array.FindAll(Of Byte)(ZFile.Data, New Predicate(Of Byte)(Function(t As Byte)
                                                                                       Return t = 10
                                                                                     End Function)).LongCount + 1
      Return "  Encoding: " & catText & vbNewLine &
             "  Contents: " & lLines & " lines"
    End If

    Return "  Contents: Unknown Binary Data"
  End Function

  Private Enum FileCat
    Text
    Image
    Audio
    Video
    TypeLib
    Binary
  End Enum

  Private Function DetermineFileCategory(ZFile As ZIP.FileSystemFile) As FileCat
    If GetFileCatTypeLib(ZFile).Major > 0 Then Return FileCat.TypeLib
    If GetFileCatRIFF(ZFile) IsNot Nothing Then Return FileCat.Audio
    If GetFileCatFTYP(ZFile) IsNot Nothing Then Return FileCat.Video
    If Not String.IsNullOrEmpty(GetFileCatImage(ZFile).Format) Then Return FileCat.Image
    If Not String.IsNullOrEmpty(GetFileCatText(ZFile)) Then Return FileCat.Text
    Return FileCat.Binary
  End Function

  Private Function GetFileCatText(ZFile As ZIP.FileSystemFile) As String
    Dim bom0 As Byte = ZFile.Data(0)
    Dim bom1 As Byte = ZFile.Data(1)
    Dim bom2 As Byte = ZFile.Data(2)
    Dim bom3 As Byte = ZFile.Data(3)
    If bom0 = &HEF And bom1 = &HBB And bom2 = &HBF Then Return "UTF-8"
    If bom0 = &H0 And bom1 = &H0 And bom2 = &HFE And bom3 = &HFF Then Return "UTF-32 (Big-Endian)"
    If bom0 = &HFF And bom1 = &HFE And bom2 = &H0 And bom3 = &H0 Then Return "UTF-32"
    If bom0 = &HFE And bom1 = &HFF Then Return "UTF-16 (Big-Endian)"
    If bom0 = &HFF And bom1 = &HFE Then Return "UTF-16"
    Dim lOver126 As Long = Array.FindAll(Of Byte)(ZFile.Data, New Predicate(Of Byte)(Function(t As Byte)
                                                                                       Return t > 126 Or (t < 32 And Not (t = 13 Or t = 10 Or t = 9))
                                                                                     End Function)).LongCount
    If lOver126 = 0 Then Return "US-ASCII"
    Dim tEnc As New List(Of System.Text.Encoding)
    tEnc.Add(System.Text.Encoding.GetEncoding(UTF_32_LE))
    tEnc.Add(System.Text.Encoding.GetEncoding(UTF_8))
    tEnc.Add(System.Text.Encoding.GetEncoding(UTF_16_LE))
    For Each testEnc As System.Text.Encoding In tEnc
      Try
        Dim sDecsTo As String = testEnc.GetString(ZFile.Data)
        Dim bEncsTo As Byte() = testEnc.GetBytes(sDecsTo)
        If Not bEncsTo.LongLength = ZFile.Data.LongLength Then Continue For
        Dim noVariants As Boolean = True
        For I As Long = 0 To ZFile.Data.LongLength - 1
          If bEncsTo(I) = ZFile.Data(I) Then Continue For
          noVariants = False
          Exit For
        Next
        If noVariants Then Return testEnc.WebName.ToUpper
      Catch ex As Exception
      End Try
    Next
    Return Nothing
  End Function

  Private Structure ImageData
    Public Format As String
    Public Dimensions As Size
    Public ColorQuality As String

    Public Sub New(picture As Image)
      Format = GetImageTypeName(picture.RawFormat)
      Dimensions = New Size(picture.Width, picture.Height)
      ColorQuality = GetImageTypeColors(picture.PixelFormat)
    End Sub

    Private Function GetImageTypeName(fmt As Imaging.ImageFormat) As String
      Select Case fmt.Guid
        Case Imaging.ImageFormat.Bmp.Guid : Return "Bitmap"
        Case Imaging.ImageFormat.Emf.Guid : Return "Enhanced Metafile"
        Case Imaging.ImageFormat.Exif.Guid : Return "Exchangeable Image File (Exif)"
        Case Imaging.ImageFormat.Gif.Guid : Return "Graphics Interchange Format"
        Case Imaging.ImageFormat.Icon.Guid : Return "Windows Icon"
        Case Imaging.ImageFormat.Jpeg.Guid : Return "Joint Picture Experts Group"
        Case Imaging.ImageFormat.Png.Guid : Return "Portable Network Graphics"
        Case Imaging.ImageFormat.Tiff.Guid : Return "Tagged Image File Format (TIFF)"
        Case Imaging.ImageFormat.Wmf.Guid : Return "Windows Metafile"
        Case Imaging.ImageFormat.MemoryBmp.Guid : Return "Bitmap (Memory)"
      End Select
      Return "Unknown"
    End Function

    Private Function GetImageTypeColors(fmt As Imaging.PixelFormat) As String
      Select Case fmt
        Case Imaging.PixelFormat.Format1bppIndexed : Return "Monochrome"
        Case Imaging.PixelFormat.Format4bppIndexed : Return "Indexed (4bit)"
        Case Imaging.PixelFormat.Format8bppIndexed : Return "Indexed (8bit)"
        Case Imaging.PixelFormat.Format16bppGrayScale : Return "Grayscale"
        Case Imaging.PixelFormat.Format16bppRgb555 : Return "16bit RGB"
        Case Imaging.PixelFormat.Format16bppRgb565 : Return "16bit (Extra Green)"
        Case Imaging.PixelFormat.Format16bppArgb1555 : Return "16bit TRGB"
        Case Imaging.PixelFormat.Format24bppRgb : Return "24bit RGB"
        Case Imaging.PixelFormat.Format32bppRgb : Return "32bit RGB"
        Case Imaging.PixelFormat.Format32bppArgb : Return "32bit ARGB"
        Case Imaging.PixelFormat.Format32bppPArgb : Return "32bit Premultiplied ARGB"
        Case Imaging.PixelFormat.Format48bppRgb : Return "48bit RGB"
        Case Imaging.PixelFormat.Format64bppArgb : Return "64bit ARGB"
        Case Imaging.PixelFormat.Format64bppPArgb : Return "64bit Premultiplied ARGB"
      End Select
      Stop
      Return "Unknown"
    End Function
  End Structure

  Private Function GetFileCatImage(ZFile As ZIP.FileSystemFile) As ImageData
    Try
      Using zStream As New IO.MemoryStream(ZFile.Data)
        Using iImage As Image = Image.FromStream(zStream, True, True)
          Return New ImageData(iImage)
        End Using
      End Using
    Catch ex As Exception
    End Try
    Return Nothing
  End Function

  Private Function GetFileCatRIFF(ZFile As ZIP.FileSystemFile) As clsRIFF
    Try
      Dim cRiff As New clsRIFF(ZFile.Data)
      If cRiff.IsValid Then Return cRiff
    Catch ex As Exception
    End Try
    Return Nothing
  End Function

  Private Function GetFileCatFTYP(ZFile As ZIP.FileSystemFile) As clsFTYP
    Try
      Dim cFtyp As New clsFTYP(ZFile.Data)
      If cFtyp.IsValid Then Return cFtyp
    Catch ex As Exception
    End Try
    Return Nothing
  End Function

  Private Structure XPCOMTypeLib
    Public Major As Byte
    Public Minor As Byte
    Public InterfaceCount As UInt16
  End Structure

  Private Function GetFileCatTypeLib(ZFile As ZIP.FileSystemFile) As XPCOMTypeLib
    If ZFile.Data.LongLength < 16 Then Return Nothing
    Dim zStr As String = System.Text.Encoding.GetEncoding(LATIN_1).GetString(ZFile.Data, 0, 16)
    If Not zStr = "XPCOM" & vbLf & "TypeLib" & vbCr & vbLf & Chr(&H1A) Then Return Nothing
    Dim lIdx As Long = 16
    If ZFile.Data.LongLength <= lIdx + 1 Then Return Nothing
    Dim bMajor As Byte = ZFile.Data(lIdx) : lIdx += 1
    If ZFile.Data.LongLength <= lIdx + 1 Then Return Nothing
    Dim bMinor As Byte = ZFile.Data(lIdx) : lIdx += 1
    If ZFile.Data.LongLength <= lIdx + 2 Then Return Nothing
    Dim iInterfaces As UInt16 = BitConverter.ToUInt16(ZFile.Data, lIdx) : lIdx += 2
    Dim ret As XPCOMTypeLib
    ret.Major = bMajor
    ret.Minor = bMinor
    ret.InterfaceCount = iInterfaces
    Return ret
  End Function
#End Region
End Class
