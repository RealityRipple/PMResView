Public Class Settings
  Private Shared mSavePath As String = Nothing
  Private Shared Function SavePath() As String
    If Not mSavePath = Nothing Then Return mSavePath
    Dim sPath As String = My.Application.Info.DirectoryPath
    Try
      CreateAllSubdirs(sPath)
      My.Computer.FileSystem.WriteAllText(IO.Path.Combine(sPath, "test.del"), "delete me", False)
      My.Computer.FileSystem.DeleteFile(IO.Path.Combine(sPath, "test.del"))
      mSavePath = IO.Path.Combine(sPath, "config.ini")
      Return mSavePath
    Catch ex As Exception
    End Try
    sPath = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.CompanyName, My.Application.Info.ProductName)
    Try
      CreateAllSubdirs(sPath)
      My.Computer.FileSystem.WriteAllText(IO.Path.Combine(sPath, "test.del"), "delete me", False)
      My.Computer.FileSystem.DeleteFile(IO.Path.Combine(sPath, "test.del"))
      mSavePath = IO.Path.Combine(sPath, "config.ini")
      Return mSavePath
    Catch ex As Exception
    End Try
    mSavePath = ""
    Return mSavePath
  End Function
  Private Shared Sub SaveSetting(ByVal sGroup As String, ByVal sSetting As String, ByVal sValue As String)
    Dim p As String = SavePath()
    If String.IsNullOrEmpty(p) Then Return
    NativeMethods.WritePrivateProfileString(sGroup, sSetting, sValue, p)
  End Sub
  Private Shared Function ReadSetting(ByVal sGroup As String, ByVal sSetting As String, ByVal sDefault As String) As String
    Dim p As String = SavePath()
    If String.IsNullOrEmpty(p) Then Return sDefault
    Dim sb As New System.Text.StringBuilder(127)
    Dim iRet As Long = NativeMethods.GetPrivateProfileString(sGroup, sSetting, "UNSET", sb, sb.Capacity, p)
    If iRet = 0 Then Return sDefault
    If sb.ToString = "UNSET" Then Return sDefault
    Return sb.ToString
  End Function
  Public Shared Property IconMethod As View
    Get
      Select Case ReadSetting("Settings", "IconMethod", "LargeIcon")
        Case "SmallIcon" : Return View.SmallIcon
        Case "List" : Return View.List
        Case "Details" : Return View.Details
        Case "Tiles" : Return View.Tile
      End Select
      Return View.Details
    End Get
    Set(ByVal value As View)
      Select Case value
        Case View.LargeIcon : SaveSetting("Settings", "IconMethod", "LargeIcon")
        Case View.SmallIcon : SaveSetting("Settings", "IconMethod", "SmallIcon")
        Case View.List : SaveSetting("Settings", "IconMethod", "List")
        Case View.Tile : SaveSetting("Settings", "IconMethod", "Tiles")
        Case Else : SaveSetting("Settings", "IconMethod", "Details")
      End Select
    End Set
  End Property
  Public Shared Property SortMethod As String
    Get
      Select Case ReadSetting("Settings", "SortMethod", "Order")
        Case "Name" : Return "Name"
        Case "Type" : Return "Type"
        Case "Size" : Return "Size"
      End Select
      Return "Order"
    End Get
    Set(ByVal value As String)
      Select Case value
        Case "Name" : SaveSetting("Settings", "SortMethod", "Name")
        Case "Type" : SaveSetting("Settings", "SortMethod", "Type")
        Case "Size" : SaveSetting("Settings", "SortMethod", "Size")
        Case Else : SaveSetting("Settings", "SortMethod", "Order")
      End Select
    End Set
  End Property
  Public Shared Property ShowToolbar As Boolean
    Get
      Return Not ReadSetting("Settings", "Toolbar", "Y") = "N"
    End Get
    Set(ByVal value As Boolean)
      If value Then
        SaveSetting("Settings", "Toolbar", "Y")
      Else
        SaveSetting("Settings", "Toolbar", "N")
      End If
    End Set
  End Property
  Public Shared Property FlatView As Boolean
    Get
      Return Not ReadSetting("Settings", "FlatView", "N") = "N"
    End Get
    Set(ByVal value As Boolean)
      If value Then
        SaveSetting("Settings", "FlatView", "Y")
      Else
        SaveSetting("Settings", "FlatView", "N")
      End If
    End Set
  End Property
  Public Shared Property TreeView As Boolean
    Get
      Return Not ReadSetting("Settings", "TreeView", "Y") = "N"
    End Get
    Set(ByVal value As Boolean)
      If value Then
        SaveSetting("Settings", "TreeView", "Y")
      Else
        SaveSetting("Settings", "TreeView", "N")
      End If
    End Set
  End Property
  Private Shared DefaultTextEditor As String() = {
    IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, "Notepad++", "notepad++.exe"),
    IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "notepad.exe")
  }
  Public Shared Property TextEditor As String
    Get
      Static sDefault As String
      If String.IsNullOrEmpty(sDefault) Then
        For I As Long = 0 To DefaultTextEditor.LongLength - 1
          If Not IO.File.Exists(DefaultTextEditor(I)) Then Continue For
          sDefault = DefaultTextEditor(I)
          Exit For
        Next
      End If
      Return ReadSetting("Editor", "Text", sDefault)
    End Get
    Set(ByVal value As String)
      If Not IO.File.Exists(value) Then Return
      SaveSetting("Editor", "Text", value)
    End Set
  End Property
  Private Shared DefaultImageEditor As String() = {
    IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, "GIMP 2", "bin", "gimp-2.10.exe"),
    IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mspaint.exe")
  }
  Public Shared Property ImageEditor As String
    Get
      Static sDefault As String
      If String.IsNullOrEmpty(sDefault) Then
        For I As Long = 0 To DefaultImageEditor.LongLength - 1
          If Not IO.File.Exists(DefaultImageEditor(I)) Then Continue For
          sDefault = DefaultImageEditor(I)
          Exit For
        Next
      End If
      Return ReadSetting("Editor", "Image", sDefault)
    End Get
    Set(ByVal value As String)
      If Not IO.File.Exists(value) Then Return
      SaveSetting("Editor", "Image", value)
    End Set
  End Property
  Private Shared DefaultBinaryEditor As String() = {
    IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "XVI32", "XVI32.exe"),
    IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "notepad.exe")
  }
  Public Shared Property BinaryEditor As String
    Get
      Static sDefault As String
      If String.IsNullOrEmpty(sDefault) Then
        For I As Long = 0 To DefaultBinaryEditor.LongLength - 1
          If Not IO.File.Exists(DefaultBinaryEditor(I)) Then Continue For
          sDefault = DefaultBinaryEditor(I)
          Exit For
        Next
      End If
      Return ReadSetting("Editor", "Binary", sDefault)
    End Get
    Set(ByVal value As String)
      If Not IO.File.Exists(value) Then Return
      SaveSetting("Editor", "Binary", value)
    End Set
  End Property
  Public Shared Property WindowSize As Size
    Get
      Dim szRet As New Size
      szRet.Width = Int(ReadSetting("Size", "Width", "720"))
      szRet.Height = Int(ReadSetting("Size", "Height", "400"))
      If szRet.Width > Screen.PrimaryScreen.WorkingArea.Width * 0.8 Then szRet.Width = Screen.PrimaryScreen.WorkingArea.Width * 0.8
      If szRet.Height > Screen.PrimaryScreen.WorkingArea.Height * 0.8 Then szRet.Height = Screen.PrimaryScreen.WorkingArea.Height * 0.8
      If szRet.Width < frmViewer.MinimumSize.Width Then szRet.Width = frmViewer.MinimumSize.Width
      If szRet.Height < frmViewer.MinimumSize.Height Then szRet.Height = frmViewer.MinimumSize.Height
      Return szRet
    End Get
    Set(ByVal value As Size)
      SaveSetting("Size", "Width", value.Width)
      SaveSetting("Size", "Height", value.Height)
    End Set
  End Property
  Public Shared Property TreeWidth As Integer
    Get
      Return Int(ReadSetting("Size", "Tree", "200"))
    End Get
    Set(ByVal value As Integer)
      SaveSetting("Size", "Tree", value)
    End Set
  End Property
End Class
