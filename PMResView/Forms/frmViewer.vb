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
    tbNav.Visible = mnuViewToolbar.Checked
    Settings.ShowToolbar = mnuViewToolbar.Checked
  End Sub

  Private Sub mnuViewFlat_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewFlat.Click, mnuContextViewFlat.Click
    mnuViewFlat.Checked = Not mnuViewFlat.Checked
    ToggleFlat()
    Settings.FlatView = mnuViewFlat.Checked
  End Sub

  Private Sub mnuViewTree_Click(sender As System.Object, e As System.EventArgs) Handles mnuViewTree.Click, mnuContextViewTree.Click
    mnuViewTree.Checked = Not mnuViewTree.Checked
    ToggleTree(mnuViewTree.Checked)
    Settings.TreeView = mnuViewTree.Checked
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
    mnuContextViewTree.Checked = mnuViewTree.Checked
    mnuContextViewFlat.Checked = mnuViewFlat.Checked
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

  Private Sub frmViewer_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
    regHistory.Add("^.*$")
    Me.Size = Settings.WindowSize
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
  End Sub

  Private Sub frmViewer_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
    Dim newWidth As Integer = pnlUI.TopToolStripPanel.ClientRectangle.Width - (cmdBack.Width + cmdForward.Width + cmdUp.Width + sepSpace1.Width + lblAddress.Width + cmdGo.Width + sepSpace2.Width + cmdExtract.Width + cmdProperties.Width + 12)
    If newWidth < 150 Then newWidth = 150
    If newWidth > 400 Then newWidth = 400
    If Not txtAddress.Width = newWidth Then txtAddress.Width = newWidth
  End Sub

  Private Sub frmViewer_ResizeEnd(sender As Object, e As System.EventArgs) Handles Me.ResizeEnd
    If Me.WindowState = FormWindowState.Maximized Then Return
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

  Private Sub tvExplorer_AfterSelect(sender As System.Object, e As System.Windows.Forms.TreeViewEventArgs) Handles tvExplorer.AfterSelect
    If tBusy Then Return
    RenderDir(e.Node.Name)
  End Sub

  Private Sub tvExplorer_NodeMouseClick(sender As Object, e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvExplorer.NodeMouseClick
    If Not e.Button = Windows.Forms.MouseButtons.Right Then Return
    mnuContextFileOpen.Enabled = True
    mnuContextFileProperties.Enabled = True
    mnuContextFile.Tag = e.Node.Name
    mnuContextFile.Show(tvExplorer, e.Location)
  End Sub

  'Private Sub tvExplorer_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles tvExplorer.MouseUp
  '  If Not e.Button = Windows.Forms.MouseButtons.Right Then Return
  '  If tvExplorer.SelectedNode Is Nothing Then Return
  '  mnuContextOpen.Visible = True
  '  mnuContextFile.Show(tvExplorer, e.Location)
  'End Sub

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
    tvExplorer.Nodes.Add(IO.Path.DirectorySeparatorChar, sArchiveName, "archive", "archive")
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

      Else
        Dim zParent As TreeNode() = tvExplorer.Nodes.Find(IO.Path.GetDirectoryName(zDir.Name), True)
        If Not zParent.LongLength = 1 Then Stop
        zParent(0).Nodes.Add(zDir.Name, IO.Path.GetFileName(zDir.Name), zImg, zImg)
      End If
    Next
    txtAddress.AutoCompleteCustomSource.Clear()
    txtAddress.AutoCompleteCustomSource.AddRange(sDirs.ToArray)
    If tvExplorer.Nodes.Count > 0 Then tvExplorer.Nodes(0).Expand()
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
    If mnuViewFlat.Checked Then
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
          sType = AddIcon(zArchive(I).Name)
        ElseIf zArchive(I).GetType Is GetType(ZIP.FileSystemDirectory) Then
          Dim ZDir As ZIP.FileSystemDirectory = zArchive(I)
          If ZDir.DirectoryCount > 0 Then
            sType = "FOLDERS"
          ElseIf ZDir.FileCount > 0 Then
            sType = "FILES"
          Else
            sType = "EMPTY"
          End If
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
          sType = AddIcon(zArchive(I).Name)
        ElseIf zArchive(I).GetType Is GetType(ZIP.FileSystemDirectory) Then
          Dim ZDir As ZIP.FileSystemDirectory = zArchive(I)
          If ZDir.DirectoryCount > 0 Then
            sType = "FOLDERS"
          ElseIf ZDir.FileCount > 0 Then
            sType = "FILES"
          Else
            sType = "EMPTY"
          End If
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
    Me.Text = "PMRes Viewer - [" & sArchiveName & "]"
    lblSelUncompressed.Text = "0%"
    lblSelSize.Visible = False
    lblSelCount.Text = "Reading Archive Structure..."
    pbActivity.Visible = True
    Dim bArchive() As Byte = IO.File.ReadAllBytes(path)
    zArchive = ZIP.ReadFileSystem(bArchive, AddressOf ShowProgress)
    If zArchive.Length = 0 Then Return
    lblSelUncompressed.Text = "0 bytes"
    lblSelSize.Visible = True
    lblSelSize.Text = "0 bytes"
    lblSelCount.Text = "No objects selected"
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
        Dim sTemp As String = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, IO.Path.GetFileName(zFile.Name))
        My.Computer.FileSystem.WriteAllBytes(sTemp, CType(zFile, ZIP.FileSystemFile).Data, False)
        NativeMethods.ShellExecute(0, vbNullString, sTemp, vbNullString, vbNullString, vbNormalFocus)
      End If
    Else
      Dim sNames As New List(Of String)
      For I As Integer = 0 To lvFiles.SelectedItems.Count - 1
        Dim zFile As ZIP.FileSystemEntry = lvFiles.SelectedItems(I).Tag
        If zFile.GetType Is GetType(ZIP.FileSystemDirectory) Then Continue For
        Dim sName As String = IO.Path.GetFileName(zFile.Name)
        Dim iCopy As Integer = 0
        While sNames.Contains(sName)
          iCopy += 1
          sName = iCopy & "-" & IO.Path.GetFileName(zFile.Name)
        End While
        sNames.Add(sName)
        Dim sTemp As String = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, sName)
        My.Computer.FileSystem.WriteAllBytes(sTemp, CType(zFile, ZIP.FileSystemFile).Data, False)
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
    If mnuViewFlat.Checked Then
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
        cdlSave.Filters.Add(New Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter(IO.Path.GetExtension(zFile.Name).Substring(1).ToUpper & " Files", IO.Path.GetExtension(zFile.Name)))
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
      sProps &= vbNewLine & "Real Size: " & ByteSize(zDir.UncompressedLength) & " (" & zDir.UncompressedLength & " bytes)"
      sProps &= vbNewLine & "Compressed: " & ByteSize(zDir.CompressedLength) & " (" & Math.Floor((zDir.CompressedLength / zDir.UncompressedLength) * 100) & "%)"
      sProps &= vbNewLine & "Contains " & zDir.FileCount & " file(s) and " & zDir.DirectoryCount & " folder(s)"
      Dim fr As Decimal = zDir.CompressedLength / zDir.UncompressedLength
      If fr > 1 Then fr = -1
      SuperMsgBox(Me, "Directory Properties", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information, IO.Path.GetFileName(zDir.Name), sProps, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Close, , , fr)
    ElseIf zFSE.GetType Is GetType(ZIP.FileSystemFile) Then
      'file properties
      Dim zFile As ZIP.FileSystemFile = zFSE
      Dim sProps As String = "Full Path: " & zFile.Name
      sProps &= vbNewLine & "Real Size: " & ByteSize(zFile.UncompressedLength) & " (" & zFile.UncompressedLength & " bytes)"
      If zFile.Compression = 0 Then
        sProps &= vbNewLine & "Uncompressed"
      Else
        sProps &= vbNewLine & "Compressed: " & ByteSize(zFile.CompressedLength) & " (" & Math.Floor((zFile.CompressedLength / zFile.UncompressedLength) * 100) & "%)"
      End If
      'sProps &= vbNewLine & "Date: " & zFile.Modified.ToString("g")
      sProps &= vbNewLine & "CRC32: 0x" & Hex(zFile.CRC)

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
    Dim sProps As String = "Full Path: " & zDir.Name
    sProps &= vbNewLine & "Real Size: " & ByteSize(zDir.UncompressedLength) & " (" & zDir.UncompressedLength & " bytes)"
    sProps &= vbNewLine & "Compressed: " & ByteSize(zDir.CompressedLength) & " (" & Math.Floor((zDir.CompressedLength / zDir.UncompressedLength) * 100) & "%)"
    sProps &= vbNewLine & "Contains " & zDir.FileCount & " file(s) and " & zDir.DirectoryCount & " folder(s)"
    Dim fr As Decimal = zDir.CompressedLength / zDir.UncompressedLength
    If fr > 1 Then fr = -1
    SuperMsgBox(Me, "Directory Properties", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information, IO.Path.GetFileName(zDir.Name), sProps, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Close, , , fr)
  End Sub
#End Region
End Class
