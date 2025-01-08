Public Class cSettings
  Private Shared useReg As TriState = TriState.UseDefault
  Private Class cDefault
    Public Structure DefaultValue
      Public [Return] As Object
      Public Registry As KeyValuePair(Of String, KeyValuePair(Of String, Object))
      Public File As KeyValuePair(Of String, KeyValuePair(Of String, String))
      Public Sub New(ByVal ret As Object, ByVal regGroup As String, ByVal regKey As String, ByVal regVal As Object, ByVal iniGroup As String, ByVal iniKey As String, ByVal iniVal As String)
        [Return] = ret
        Registry = New KeyValuePair(Of String, KeyValuePair(Of String, Object))(regGroup, New KeyValuePair(Of String, Object)(regKey, regVal))
        File = New KeyValuePair(Of String, KeyValuePair(Of String, String))(iniGroup, New KeyValuePair(Of String, String)(iniKey, iniVal))
      End Sub
    End Structure
    Private Shared DefaultTextEditor As String() = {
      IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, "Notepad++", "notepad++.exe"),
      IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "notepad.exe")
    }
    Private Shared DefaultImageEditor As String() = {
      IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, "GIMP 2", "bin", "gimp-2.10.exe"),
      IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mspaint.exe")
    }
    Private Shared DefaultBinaryEditor As String() = {
      IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "XVI32", "XVI32.exe"),
      IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "notepad.exe")
    }
    Public Shared ReadOnly Property IconMethod As DefaultValue
      Get
        Return New DefaultValue(View.Details, Nothing, "IconMethod", "Details", "Settings", "IconMethod", "Details")
      End Get
    End Property
    Public Shared ReadOnly Property SortMethod As DefaultValue
      Get
        Return New DefaultValue("Order", Nothing, "SortMethod", "Order", "Settings", "SortMethod", "Order")
      End Get
    End Property
    Public Shared ReadOnly Property ShowToolbar As DefaultValue
      Get
        Return New DefaultValue(True, Nothing, "Toolbar", 1, "Settings", "Toolbar", "Y")
      End Get
    End Property
    Public Shared ReadOnly Property FlatView As DefaultValue
      Get
        Return New DefaultValue(False, Nothing, "FlatView", 0, "Settings", "FlatView", "N")
      End Get
    End Property
    Public Shared ReadOnly Property TreeView As DefaultValue
      Get
        Return New DefaultValue(True, Nothing, "TreeView", 1, "Settings", "TreeView", "Y")
      End Get
    End Property
    Public Shared ReadOnly Property TextEditor As DefaultValue
      Get
        Static sDefault As String
        If String.IsNullOrEmpty(sDefault) Then
          For I As Long = 0 To DefaultTextEditor.LongLength - 1
            If Not IO.File.Exists(DefaultTextEditor(I)) Then Continue For
            sDefault = DefaultTextEditor(I)
            Exit For
          Next
        End If
        Return New DefaultValue(sDefault, "Editor", "Text", sDefault, "Editor", "Text", sDefault)
      End Get
    End Property
    Public Shared ReadOnly Property ImageEditor As DefaultValue
      Get
        Static sDefault As String
        If String.IsNullOrEmpty(sDefault) Then
          For I As Long = 0 To DefaultImageEditor.LongLength - 1
            If Not IO.File.Exists(DefaultImageEditor(I)) Then Continue For
            sDefault = DefaultImageEditor(I)
            Exit For
          Next
        End If
        Return New DefaultValue(sDefault, "Editor", "Image", sDefault, "Editor", "Image", sDefault)
      End Get
    End Property
    Public Shared ReadOnly Property BinaryEditor As DefaultValue
      Get
        Static sDefault As String
        If String.IsNullOrEmpty(sDefault) Then
          For I As Long = 0 To DefaultBinaryEditor.LongLength - 1
            If Not IO.File.Exists(DefaultBinaryEditor(I)) Then Continue For
            sDefault = DefaultBinaryEditor(I)
            Exit For
          Next
        End If
        Return New DefaultValue(sDefault, "Editor", "Binary", sDefault, "Editor", "Binary", sDefault)
      End Get
    End Property
    Public Shared ReadOnly Property WindowSizeWidth As DefaultValue
      Get
        Return New DefaultValue(720, "Size", "Width", 720, "Size", "Width", "720")
      End Get
    End Property
    Public Shared ReadOnly Property WindowSizeHeight As DefaultValue
      Get
        Return New DefaultValue(400, "Size", "Height", 400, "Size", "Height", "400")
      End Get
    End Property
    Public Shared ReadOnly Property TreeWidth As DefaultValue
      Get
        Return New DefaultValue(200, "Size", "Tree", 200, "Size", "Tree", "200")
      End Get
    End Property
  End Class
  Private Class cRegistry
    Private Const sProfiles As String = "Profiles"
    Private Shared Function saveToRegistry(Optional ByVal writable As Boolean = False) As Microsoft.Win32.RegistryKey
      Try
        If Not My.Computer.Registry.CurrentUser.GetSubKeyNames.Contains("Software", StringComparer.OrdinalIgnoreCase) Then My.Computer.Registry.CurrentUser.CreateSubKey("Software")
        If Not My.Computer.Registry.CurrentUser.OpenSubKey("Software").GetSubKeyNames.Contains(Application.CompanyName) Then My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).CreateSubKey(Application.CompanyName)
        If Not My.Computer.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(Application.CompanyName).GetSubKeyNames.Contains(Application.ProductName) Then My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).OpenSubKey(Application.CompanyName, True).CreateSubKey(Application.ProductName)
        Return My.Computer.Registry.CurrentUser.OpenSubKey("Software", writable).OpenSubKey(Application.CompanyName, writable).OpenSubKey(Application.ProductName, writable)
      Catch ex As Exception
        Return Nothing
      End Try
    End Function
    Public Shared Property IconMethod As View
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.IconMethod.Return
          If Not String.IsNullOrEmpty(cDefault.IconMethod.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.IconMethod.Registry.Key) Then Return cDefault.IconMethod.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.IconMethod.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.IconMethod.Registry.Value.Key) Then Return cDefault.IconMethod.Return
          If Not myRegKey.GetValueKind(cDefault.IconMethod.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.String Then Return cDefault.IconMethod.Return
          Select Case myRegKey.GetValue(cDefault.IconMethod.Registry.Value.Key, cDefault.IconMethod.Registry.Value.Value)
            Case "LargeIcon" : Return View.LargeIcon
            Case "SmallIcon" : Return View.SmallIcon
            Case "List" : Return View.List
            Case "Details" : Return View.Details
            Case "Tiles" : Return View.Tile
          End Select
          Return cDefault.IconMethod.Return
        Catch ex As Exception
          Return cDefault.IconMethod.Return
        End Try
      End Get
      Set(ByVal value As View)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.IconMethod.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.IconMethod.Registry.Key) Then myRegKey.CreateSubKey(cDefault.IconMethod.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.IconMethod.Registry.Key, True)
          End If
          Dim sValue As String = cDefault.IconMethod.Registry.Value.Value
          Select Case value
            Case View.LargeIcon : sValue = "LargeIcon"
            Case View.SmallIcon : sValue = "SmallIcon"
            Case View.List : sValue = "List"
            Case View.Tile : sValue = "Tiles"
            Case View.Details : sValue = "Details"
          End Select
          myRegKey.SetValue(cDefault.IconMethod.Registry.Value.Key, sValue, Microsoft.Win32.RegistryValueKind.String)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property SortMethod As String
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.SortMethod.Return
          If Not String.IsNullOrEmpty(cDefault.SortMethod.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.SortMethod.Registry.Key) Then Return cDefault.SortMethod.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.SortMethod.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.SortMethod.Registry.Value.Key) Then Return cDefault.SortMethod.Return
          If Not myRegKey.GetValueKind(cDefault.SortMethod.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.String Then Return cDefault.SortMethod.Return
          Select Case CStr(myRegKey.GetValue(cDefault.SortMethod.Registry.Value.Key, cDefault.SortMethod.Registry.Value.Value)).ToLower
            Case "order" : Return "Order"
            Case "name" : Return "Name"
            Case "type" : Return "Type"
            Case "size" : Return "Size"
          End Select
          Return cDefault.SortMethod.Return
        Catch ex As Exception
          Return cDefault.SortMethod.Return
        End Try
      End Get
      Set(ByVal value As String)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.SortMethod.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.SortMethod.Registry.Key) Then myRegKey.CreateSubKey(cDefault.SortMethod.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.SortMethod.Registry.Key, True)
          End If
          Dim sValue As String = cDefault.SortMethod.Registry.Value.Value
          Select Case value
            Case "Order" : sValue = "Order"
            Case "Name" : sValue = "Name"
            Case "Type" : sValue = "Type"
            Case "Size" : sValue = "Size"
          End Select
          myRegKey.SetValue(cDefault.SortMethod.Registry.Value.Key, sValue, Microsoft.Win32.RegistryValueKind.String)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property ShowToolbar As Boolean
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.ShowToolbar.Return
          If Not String.IsNullOrEmpty(cDefault.ShowToolbar.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.ShowToolbar.Registry.Key) Then Return cDefault.ShowToolbar.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.ShowToolbar.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.ShowToolbar.Registry.Value.Key) Then Return cDefault.ShowToolbar.Return
          If Not myRegKey.GetValueKind(cDefault.ShowToolbar.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.DWord Then Return cDefault.ShowToolbar.Return
          Return myRegKey.GetValue(cDefault.ShowToolbar.Registry.Value.Key, cDefault.ShowToolbar.Registry.Value.Value) = 1
        Catch ex As Exception
          Return cDefault.ShowToolbar.Return
        End Try
      End Get
      Set(ByVal value As Boolean)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.ShowToolbar.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.ShowToolbar.Registry.Key) Then myRegKey.CreateSubKey(cDefault.ShowToolbar.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.ShowToolbar.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.ShowToolbar.Registry.Value.Key, IIf(value, 1, 0), Microsoft.Win32.RegistryValueKind.DWord)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property FlatView As Boolean
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.FlatView.Return
          If Not String.IsNullOrEmpty(cDefault.FlatView.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.FlatView.Registry.Key) Then Return cDefault.FlatView.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.FlatView.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.FlatView.Registry.Value.Key) Then Return cDefault.FlatView.Return
          If Not myRegKey.GetValueKind(cDefault.FlatView.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.DWord Then Return cDefault.FlatView.Return
          Return myRegKey.GetValue(cDefault.ShowToolbar.Registry.Value.Key, cDefault.FlatView.Registry.Value.Value) = 1
        Catch ex As Exception
          Return cDefault.FlatView.Return
        End Try
      End Get
      Set(ByVal value As Boolean)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.FlatView.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.FlatView.Registry.Key) Then myRegKey.CreateSubKey(cDefault.FlatView.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.FlatView.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.FlatView.Registry.Value.Key, IIf(value, 1, 0), Microsoft.Win32.RegistryValueKind.DWord)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property TreeView As Boolean
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.TreeView.Return
          If Not String.IsNullOrEmpty(cDefault.TreeView.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.TreeView.Registry.Key) Then Return cDefault.TreeView.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.TreeView.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.TreeView.Registry.Value.Key) Then Return cDefault.TreeView.Return
          If Not myRegKey.GetValueKind(cDefault.TreeView.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.DWord Then Return cDefault.TreeView.Return
          Return myRegKey.GetValue(cDefault.TreeView.Registry.Value.Key, cDefault.TreeView.Registry.Value.Value) = 1
        Catch ex As Exception
          Return cDefault.TreeView.Return
        End Try
      End Get
      Set(ByVal value As Boolean)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.TreeView.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.TreeView.Registry.Key) Then myRegKey.CreateSubKey(cDefault.TreeView.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.TreeView.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.TreeView.Registry.Value.Key, IIf(value, 1, 0), Microsoft.Win32.RegistryValueKind.DWord)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property TextEditor As String
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.TextEditor.Return
          If Not String.IsNullOrEmpty(cDefault.TextEditor.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.TextEditor.Registry.Key) Then Return cDefault.TextEditor.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.TextEditor.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.TextEditor.Registry.Value.Key) Then Return cDefault.TextEditor.Return
          If Not myRegKey.GetValueKind(cDefault.TextEditor.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.String Then Return cDefault.TextEditor.Return
          Return myRegKey.GetValue(cDefault.TextEditor.Registry.Value.Key, cDefault.TextEditor.Registry.Value.Value)
        Catch ex As Exception
          Return cDefault.TextEditor.Return
        End Try
      End Get
      Set(ByVal value As String)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.TextEditor.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.TextEditor.Registry.Key) Then myRegKey.CreateSubKey(cDefault.TextEditor.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.TextEditor.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.TextEditor.Registry.Value.Key, value, Microsoft.Win32.RegistryValueKind.String)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property ImageEditor As String
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.ImageEditor.Return
          If Not String.IsNullOrEmpty(cDefault.ImageEditor.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.ImageEditor.Registry.Key) Then Return cDefault.ImageEditor.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.ImageEditor.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.ImageEditor.Registry.Value.Key) Then Return cDefault.ImageEditor.Return
          If Not myRegKey.GetValueKind(cDefault.ImageEditor.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.String Then Return cDefault.ImageEditor.Return
          Return myRegKey.GetValue(cDefault.ImageEditor.Registry.Value.Key, cDefault.ImageEditor.Registry.Value.Value)
        Catch ex As Exception
          Return cDefault.ImageEditor.Return
        End Try
      End Get
      Set(ByVal value As String)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.ImageEditor.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.ImageEditor.Registry.Key) Then myRegKey.CreateSubKey(cDefault.ImageEditor.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.ImageEditor.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.ImageEditor.Registry.Value.Key, value, Microsoft.Win32.RegistryValueKind.String)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property BinaryEditor As String
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.BinaryEditor.Return
          If Not String.IsNullOrEmpty(cDefault.BinaryEditor.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.BinaryEditor.Registry.Key) Then Return cDefault.BinaryEditor.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.BinaryEditor.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.BinaryEditor.Registry.Value.Key) Then Return cDefault.BinaryEditor.Return
          If Not myRegKey.GetValueKind(cDefault.BinaryEditor.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.String Then Return cDefault.BinaryEditor.Return
          Return myRegKey.GetValue(cDefault.BinaryEditor.Registry.Value.Key, cDefault.BinaryEditor.Registry.Value.Value)
        Catch ex As Exception
          Return cDefault.BinaryEditor.Return
        End Try
      End Get
      Set(ByVal value As String)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.BinaryEditor.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.BinaryEditor.Registry.Key) Then myRegKey.CreateSubKey(cDefault.BinaryEditor.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.BinaryEditor.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.BinaryEditor.Registry.Value.Key, value, Microsoft.Win32.RegistryValueKind.String)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property WindowSizeWidth As UInt32
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.WindowSizeWidth.Return
          If Not String.IsNullOrEmpty(cDefault.WindowSizeWidth.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.WindowSizeWidth.Registry.Key) Then Return cDefault.WindowSizeWidth.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.WindowSizeWidth.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.WindowSizeWidth.Registry.Value.Key) Then Return cDefault.WindowSizeWidth.Return
          If Not myRegKey.GetValueKind(cDefault.WindowSizeWidth.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.DWord Then Return cDefault.WindowSizeWidth.Return
          Return myRegKey.GetValue(cDefault.WindowSizeWidth.Registry.Value.Key, cDefault.WindowSizeWidth.Registry.Value.Value)
        Catch ex As Exception
          Return cDefault.WindowSizeWidth.Return
        End Try
      End Get
      Set(ByVal value As UInt32)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.WindowSizeWidth.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.WindowSizeWidth.Registry.Key) Then myRegKey.CreateSubKey(cDefault.WindowSizeWidth.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.WindowSizeWidth.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.WindowSizeWidth.Registry.Value.Key, value, Microsoft.Win32.RegistryValueKind.DWord)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property WindowSizeHeight As UInt32
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.WindowSizeHeight.Return
          If Not String.IsNullOrEmpty(cDefault.WindowSizeHeight.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.WindowSizeHeight.Registry.Key) Then Return cDefault.WindowSizeHeight.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.WindowSizeHeight.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.WindowSizeHeight.Registry.Value.Key) Then Return cDefault.WindowSizeHeight.Return
          If Not myRegKey.GetValueKind(cDefault.WindowSizeHeight.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.DWord Then Return cDefault.WindowSizeHeight.Return
          Return myRegKey.GetValue(cDefault.WindowSizeHeight.Registry.Value.Key, cDefault.WindowSizeHeight.Registry.Value.Value)
        Catch ex As Exception
          Return cDefault.WindowSizeHeight.Return
        End Try
      End Get
      Set(ByVal value As UInt32)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.WindowSizeHeight.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.WindowSizeHeight.Registry.Key) Then myRegKey.CreateSubKey(cDefault.WindowSizeHeight.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.WindowSizeHeight.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.WindowSizeHeight.Registry.Value.Key, value, Microsoft.Win32.RegistryValueKind.DWord)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Property TreeWidth As UInt32
      Get
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(False)
          If myRegKey Is Nothing Then Return cDefault.TreeWidth.Return
          If Not String.IsNullOrEmpty(cDefault.TreeWidth.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.TreeWidth.Registry.Key) Then Return cDefault.TreeWidth.Return
            myRegKey = myRegKey.OpenSubKey(cDefault.TreeWidth.Registry.Key)
          End If
          If Not myRegKey.GetValueNames.Contains(cDefault.TreeWidth.Registry.Value.Key) Then Return cDefault.TreeWidth.Return
          If Not myRegKey.GetValueKind(cDefault.TreeWidth.Registry.Value.Key) = Microsoft.Win32.RegistryValueKind.DWord Then Return cDefault.TreeWidth.Return
          Return myRegKey.GetValue(cDefault.TreeWidth.Registry.Value.Key, cDefault.TreeWidth.Registry.Value.Value)
        Catch ex As Exception
          Return cDefault.TreeWidth.Return
        End Try
      End Get
      Set(ByVal value As UInt32)
        Try
          Dim myRegKey As Microsoft.Win32.RegistryKey = saveToRegistry(True)
          If myRegKey Is Nothing Then Return
          If Not String.IsNullOrEmpty(cDefault.TreeWidth.Registry.Key) Then
            If Not myRegKey.GetSubKeyNames.Contains(cDefault.TreeWidth.Registry.Key) Then myRegKey.CreateSubKey(cDefault.TreeWidth.Registry.Key)
            myRegKey = myRegKey.OpenSubKey(cDefault.TreeWidth.Registry.Key, True)
          End If
          myRegKey.SetValue(cDefault.TreeWidth.Registry.Value.Key, value, Microsoft.Win32.RegistryValueKind.DWord)
        Catch ex As Exception
        End Try
      End Set
    End Property
    Public Shared Sub RemoveAll()
      Try
        If Not My.Computer.Registry.CurrentUser.GetSubKeyNames.Contains("Software", StringComparer.OrdinalIgnoreCase) Then Return
        If Not My.Computer.Registry.CurrentUser.OpenSubKey("Software").GetSubKeyNames.Contains(Application.CompanyName) Then Return
        If Not My.Computer.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(Application.CompanyName).GetSubKeyNames.Contains(Application.ProductName) Then Return
        My.Computer.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(Application.CompanyName, True).DeleteSubKeyTree(Application.ProductName, False)
        If My.Computer.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(Application.CompanyName).SubKeyCount = 0 Then My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).DeleteSubKeyTree(Application.CompanyName, False)
      Catch ex As Exception
      End Try
    End Sub
  End Class
  Private Class cFile
    Private Shared storedPath As String = Nothing
    Private Shared showedError As Boolean = False
    Private Shared saveLock As New Object
    Private Shared profLock As New Object
    Private Shared tSave As Threading.Timer = Nothing
    Private Shared tRead As Threading.Timer = Nothing
    Private Shared eLATIN As System.Text.Encoding = System.Text.Encoding.GetEncoding(28591)
    Private Const saveWait As Integer = 500
    Private Const readInterval As Integer = 120000
    Private Const Unix2000 As Int64 = &H386D4380L
    Private Shared ReadOnly Property uNix As Int64
      Get
        Return DateDiff(DateInterval.Second, New Date(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), Date.UtcNow)
      End Get
    End Property
    Private Shared m_PossiblePaths As String() = Nothing
    Private Shared ReadOnly Property PossiblePaths As String()
      Get
        If m_PossiblePaths IsNot Nothing Then Return m_PossiblePaths
        Dim lPaths As New List(Of String)
        If IsInstalled Then
          If Not String.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) Then lPaths.Add(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.CompanyName, Application.ProductName))
          If Not String.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) Then lPaths.Add(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.CompanyName, Application.ProductName))
          If Not String.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) Then lPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
          If Not String.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) Then lPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
        ElseIf IsLocal Then
          lPaths.Add(My.Application.Info.DirectoryPath)
          If Not String.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) Then lPaths.Add(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.CompanyName, Application.ProductName))
          If Not String.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) Then lPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
          If Not String.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) Then lPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
        Else
          lPaths.Add(My.Application.Info.DirectoryPath)
          lPaths.Add(IO.Path.GetPathRoot(My.Application.Info.DirectoryPath))
        End If
        m_PossiblePaths = lPaths.ToArray
        Return m_PossiblePaths
      End Get
    End Property
    Private Shared Function getLatestINI() As String
      Dim lLatest As Long = 0
      Dim dLatest As String = "[Settings]" & vbNewLine
      For I As Integer = 0 To PossiblePaths.Length - 1
        Dim sTest As String = IO.Path.Combine(PossiblePaths(I), CleanProductName & ".ini")
        If Not IO.File.Exists(sTest) Then Continue For
        Dim uTest As Long = GetINITimestamp(sTest)
        If uTest < Unix2000 Then Continue For
        If uTest < lLatest Then Continue For
        Dim dTest As String = Nothing
        Try
          dTest = IO.File.ReadAllText(sTest, eLATIN)
        Catch ex As Exception
          Continue For
        End Try
        If String.IsNullOrEmpty(dTest) Then Continue For
        dLatest = dTest
        lLatest = uTest
      Next
      Return dLatest
    End Function
    Private Shared Function testWritableINI(ByVal sPath As String) As Boolean
      Dim oPath As String() = sPath.Split(IO.Path.DirectorySeparatorChar)
      Dim testPath As String = ""
      For P As Integer = 0 To oPath.Length - 1
        If String.IsNullOrEmpty(testPath) Then
          testPath = oPath(P) & IO.Path.DirectorySeparatorChar
        Else
          testPath = IO.Path.Combine(testPath, oPath(P))
        End If
        If Not IO.Directory.Exists(testPath) Then IO.Directory.CreateDirectory(testPath)
      Next
      testPath = IO.Path.Combine(testPath, CleanProductName & ".ini")
      Const sEmpty As String = "[Settings]" & vbNewLine & "Dummy=1" & vbNewLine
      If Not IO.File.Exists(testPath) Then
        Try
          IO.File.WriteAllText(testPath, sEmpty, eLATIN)
          Dim bSuccess As Boolean = IO.File.ReadAllText(testPath, eLATIN) = sEmpty
          IO.File.Delete(testPath)
          Return bSuccess
        Catch ex As Exception
          Return False
        End Try
      End If
      Dim sOld As String = IO.File.ReadAllText(testPath, eLATIN)
      Dim pOld As String = IO.Path.ChangeExtension(testPath, "bak")
      Try
        IO.File.Move(testPath, pOld)
        IO.File.WriteAllText(testPath, sEmpty, eLATIN)
        Dim bSuccessA As Boolean = IO.File.ReadAllText(testPath, eLATIN) = sEmpty
        IO.File.Delete(testPath)
        IO.File.Move(pOld, testPath)
        Dim bSuccessB As Boolean = IO.File.ReadAllText(testPath, eLATIN) = sOld
        Return bSuccessA And bSuccessB
      Catch ex As Exception
        Return False
      End Try
    End Function
    Private Shared Function saveToINI() As String
      If Not String.IsNullOrEmpty(storedPath) Then Return storedPath
      storedPath = "NO"
      Try
        Dim dLatest As String = getLatestINI()
        For I As Integer = 0 To PossiblePaths.Length - 1
          If Not testWritableINI(PossiblePaths(I)) Then Continue For
          Try
            Dim savePath As String = IO.Path.Combine(PossiblePaths(I), CleanProductName & ".ini")
            IO.File.WriteAllText(savePath, dLatest, eLATIN)
            If Not IO.File.ReadAllText(savePath, eLATIN) = dLatest Then Continue For
            storedPath = savePath
            Exit For
          Catch ex As Exception
            Continue For
          End Try
        Next
      Catch ex As Exception
      Finally
        CleanOldINIs()
      End Try
      Return storedPath
    End Function
    Private Shared Sub CleanOldINIs()
      Try
        If String.IsNullOrEmpty(storedPath) Then Return
        If storedPath = "NO" Then Return
        For I As Integer = 0 To PossiblePaths.Count - 1
          Dim sTest As String = IO.Path.Combine(PossiblePaths(I), CleanProductName & ".ini")
          If sTest = storedPath Then Continue For
          If Not IO.File.Exists(sTest) Then Continue For
          FileSafeDelete(sTest)
        Next
      Catch ex As Exception
      End Try
    End Sub
    Public Shared ReadOnly Property CanSave As Boolean
      Get
        Return Not saveToINI() = "NO"
      End Get
    End Property
    Private Shared Function INIRead(ByVal INIPath As String, ByVal SectionName As String, ByVal KeyName As String, ByVal DefaultValue As String) As String
      Try
        Dim sData As String = Space(1024)
        Dim n As Integer = NativeMethods.GetPrivateProfileStringW(SectionName, KeyName, DefaultValue, sData, sData.Length, INIPath)
        If n > 0 Then Return sData.Substring(0, n)
      Catch ex As Exception
      End Try
      Return DefaultValue
    End Function
    Private Shared Function GetValidLng(ByVal s As String) As Int64
      Dim dS As String = s.Where(Function(c As Char) As Boolean
                                   Return Char.IsDigit(c)
                                 End Function).ToArray()
      Try
        Dim uS As UInt64 = CULng(dS)
        If uS > Int64.MaxValue Then Return 0
        Return CLng(uS)
      Catch ex As Exception
        Return 0
      End Try
    End Function
    Private Shared Function IsINIOK(ByVal INIPath As String) As Boolean
      Return Not GetINITimestamp(INIPath) = 0
    End Function
    Private Shared Function GetINITimestamp(ByVal INIPath As String) As Int64
      Try
        If Not IO.File.Exists(INIPath) Then Return 0
        Dim sNum As String = INIRead(INIPath, "META", "save", "0")
        Dim uNum As Int64 = GetValidLng(sNum)
        If uNum < Unix2000 Then Return 0
        If sNum = CStr(uNum) Then Return uNum
      Catch ex As Exception
      End Try
      Return 0
    End Function
    Private Shared Function FileSafeDelete(ByVal sFile As String) As Boolean
      Try
        Dim fFile As New IO.FileInfo(sFile)
        If Not fFile.Exists Then Return True
        If fFile.IsReadOnly Then fFile.IsReadOnly = False
        fFile.Delete()
        Return True
      Catch ex As Exception
      End Try
      Return False
    End Function
    Private Shared Function FileSafeDelete(ByVal fFile As IO.FileInfo) As Boolean
      Try
        If Not fFile.Exists Then Return True
        If fFile.IsReadOnly Then fFile.IsReadOnly = False
        fFile.Delete()
        Return True
      Catch ex As Exception
      End Try
      Return False
    End Function
    Private Shared Function FileSafeMove(ByVal sFrom As String, ByVal sTo As String, Optional ByVal noFrom As Boolean = True) As Boolean
      Try
        Dim fFrom As New IO.FileInfo(sFrom)
        If Not fFrom.Exists Then Return noFrom
        FileSafeDelete(sTo)
        fFrom.MoveTo(sTo)
        Return True
      Catch ex As Exception
      End Try
      Return False
    End Function
    Private Shared Function FileSafeCopy(ByVal sFrom As String, ByVal sTo As String, Optional ByVal noFrom As Boolean = True) As Boolean
      Try
        Dim fFrom As New IO.FileInfo(sFrom)
        If Not fFrom.Exists Then Return noFrom
        FileSafeDelete(sTo)
        Dim fTo As IO.FileInfo = fFrom.CopyTo(sTo, True)
        If Not fTo.Exists Then Return False
        If fTo.IsReadOnly Then fTo.IsReadOnly = False
        Return True
      Catch ex As Exception
      End Try
      Return False
    End Function
    Private Shared Function FileSafeRead(ByVal fFile As IO.FileInfo, Optional ByVal encoding As System.Text.Encoding = Nothing) As String
      If encoding Is Nothing Then encoding = eLATIN
      Try
        Using r As IO.FileStream = fFile.Open(IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
          Try
            r.Lock(0, r.Length)
            Dim bData As Byte()
            ReDim bData(r.Length - 1)
            r.Read(bData, 0, r.Length)
            r.Unlock(0, r.Length)
            Return encoding.GetString(bData)
          Catch ex As Exception
          Finally
            r.Close()
          End Try
        End Using
      Catch ex As Exception
      End Try
      Return Nothing
    End Function
    Private Shared Function FileSafeWrite(ByVal fFile As IO.FileInfo, ByVal sData As String, Optional ByVal encoding As System.Text.Encoding = Nothing) As Boolean
      If encoding Is Nothing Then encoding = eLATIN
      Try
        If fFile.Exists AndAlso fFile.IsReadOnly Then fFile.IsReadOnly = False
      Catch ex As Exception
        Return False
      End Try
      Try
        Using w As IO.FileStream = fFile.Open(IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
          Try
            Dim bData As Byte() = encoding.GetBytes(sData)
            w.Lock(0, bData.Length)
            w.Write(bData, 0, bData.Length)
            w.Flush()
            w.Unlock(0, bData.Length)
            Return True
          Catch ex As Exception
            Return False
          Finally
            w.Close()
          End Try
        End Using
      Catch ex As Exception
      End Try
      Return False
    End Function
    Private Shared Function IsRunningProcess(ByVal pID As Int64) As Boolean
      If pID < Int32.MinValue Then Return False
      If pID > Int32.MaxValue Then Return False
      Try
        If Not System.Diagnostics.Process.GetProcessById(pID).HasExited Then Return True
      Catch ex As Exception
      End Try
      Return False
    End Function
    Private Shared Function SafeRead(ByVal INIPath As String, ByVal SectionName As String, ByVal KeyName As String, ByVal DefaultValue As String) As String
      Dim sNew As String = IO.Path.Combine(IO.Path.GetDirectoryName(INIPath), "~" & IO.Path.GetFileName(INIPath))
      Dim sOld As String = IO.Path.Combine(IO.Path.GetDirectoryName(INIPath), IO.Path.GetFileName(INIPath) & "~")
      If IsINIOK(INIPath) Then
        FileSafeDelete(sNew)
        FileSafeDelete(sOld)
        Return INIRead(INIPath, SectionName, KeyName, DefaultValue)
      End If
      If IsINIOK(sNew) Then
        FileSafeDelete(INIPath)
        FileSafeDelete(sOld)
        Return INIRead(sNew, SectionName, KeyName, DefaultValue)
      End If
      If IsINIOK(sOld) Then
        FileSafeDelete(INIPath)
        FileSafeDelete(sNew)
        Return INIRead(sOld, SectionName, KeyName, DefaultValue)
      End If
      Return DefaultValue
    End Function
    Private Shared Function ReadSections(ByVal INIPath As String) As String()
      Dim sTmp As String = SafeRead(INIPath, Nothing, Nothing, "").TrimEnd(vbNullChar).Replace(vbNullChar & vbNullChar, vbNullChar)
      If String.IsNullOrEmpty(sTmp) Then Return (New List(Of String)).ToArray
      Return sTmp.Split(vbNullChar)
    End Function
    Private Shared Function ReadKeys(ByVal INIPath As String, ByVal SectionName As String) As String()
      Dim sTmp As String = SafeRead(INIPath, SectionName, Nothing, "").TrimEnd(vbNullChar).Replace(vbNullChar & vbNullChar, vbNullChar)
      If String.IsNullOrEmpty(sTmp) Then Return (New List(Of String)).ToArray
      Return sTmp.Split(vbNullChar)
    End Function
    Private Shared Function FLock(ByVal Path As String) As Boolean
      Dim sLock As String = IO.Path.Combine(IO.Path.GetDirectoryName(Path), IO.Path.ChangeExtension(IO.Path.GetFileName(Path), "lock"))
      Dim fLocker As New IO.FileInfo(sLock)
      If fLocker.Exists AndAlso IsRunningProcess(GetValidLng(FileSafeRead(fLocker))) Then Return False
      Return FileSafeWrite(fLocker, System.Diagnostics.Process.GetCurrentProcess.Id)
    End Function
    Private Shared Sub FUnlock(ByVal Path As String)
      Dim sLock As String = IO.Path.Combine(IO.Path.GetDirectoryName(Path), IO.Path.ChangeExtension(IO.Path.GetFileName(Path), "lock"))
      Dim fLocker As New IO.FileInfo(sLock)
      If fLocker.Exists AndAlso Not GetValidLng(FileSafeRead(fLocker)) = System.Diagnostics.Process.GetCurrentProcess.Id Then Return
      FileSafeDelete(fLocker)
    End Sub
    Private Shared Sub SafeWrite(ByVal INIPath As String, ByVal Entries As Dictionary(Of String, Dictionary(Of String, String)))
      If Not FLock(INIPath) Then
        If showedError Then Return
        MsgBox("Unable to Save Configuration" & vbNewLine & vbNewLine & My.Application.Info.ProductName & "'s config file is locked. It may be in use by another task.", MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal, Application.ProductName)
        showedError = True
        Return
      End If
      Dim sNew As String = IO.Path.Combine(IO.Path.GetDirectoryName(INIPath), "~" & IO.Path.GetFileName(INIPath))
      Dim sOld As String = IO.Path.Combine(IO.Path.GetDirectoryName(INIPath), IO.Path.GetFileName(INIPath) & "~")
      Try
        FileSafeDelete(sNew)
        FileSafeDelete(sOld)
        Dim uNum As Int64 = GetINITimestamp(INIPath)
        If uNum < Unix2000 Then uNum = Unix2000
        uNum += 1
        If uNix > uNum Then uNum = uNix
        Dim iFail As Integer = 0
        For Each SectionName As String In Entries.Keys
          For Each KeyName As String In Entries(SectionName).Keys
            Dim Value As String = Entries(SectionName)(KeyName)
            If NativeMethods.WritePrivateProfileStringW(SectionName, KeyName, Value, sNew) = 0 Then iFail = System.Runtime.InteropServices.Marshal.GetLastWin32Error
            If iFail > 0 Then Exit For
          Next
          If iFail > 0 Then Exit For
        Next
        If iFail = 0 AndAlso NativeMethods.WritePrivateProfileStringW("META", "save", CStr(uNum), sNew) = 0 Then iFail = System.Runtime.InteropServices.Marshal.GetLastWin32Error
        If Not iFail = 0 Then
          If showedError Then Return
          MsgBox("Unable to Save Configuration" & vbNewLine & vbNewLine & "There was a problem while trying to write to " & My.Application.Info.ProductName & "'s new config file: " & Conversion.ErrorToString(iFail), MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal, Application.ProductName)
          showedError = True
          Return
        End If
        If Not FileSafeMove(INIPath, sOld) Then
          If showedError Then Return
          MsgBox("Unable to Save Configuration" & vbNewLine & vbNewLine & "There was a problem while trying to move " & My.Application.Info.ProductName & "'s old config file to a abackup location.", MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal, Application.ProductName)
          showedError = True
          Return
        End If
        If Not FileSafeMove(sNew, INIPath, False) Then
          If showedError Then Return
          MsgBox("Unable to Save Configuration" & vbNewLine & vbNewLine & "There was a problem while trying to finalize " & My.Application.Info.ProductName & "'s new config file.", MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal, Application.ProductName)
          showedError = True
          Return
        End If
        Dim readOK = True
        For Each SectionName As String In Entries.Keys
          For Each KeyName As String In Entries(SectionName).Keys
            Dim Value As String = Entries(SectionName)(KeyName)
            If String.IsNullOrEmpty(KeyName) Then
              If ReadSections(INIPath).Contains(SectionName) Then readOK = False
            ElseIf String.IsNullOrEmpty(Value) Then
              If ReadKeys(INIPath, SectionName).Contains(KeyName) Then readOK = False
            Else
              If Not GetValidLng(INIRead(INIPath, "META", "save", "0")) = uNum Then readOK = False
            End If
          Next
        Next
        If Not readOK Then
          FileSafeDelete(INIPath)
          If showedError Then Return
          MsgBox("Unable to Save Configuration" & vbNewLine & vbNewLine & "There was a fidelity test failure: " & My.Application.Info.ProductName & "'s config was not saved.", MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal, Application.ProductName)
          showedError = True
          Return
        End If
        FileSafeDelete(sOld)
        showedError = False
      Catch ex As Exception
        If showedError Then Return
        MsgBox("Unable to Save Configuration" & vbNewLine & vbNewLine & "There was a problem while saving " & My.Application.Info.ProductName & "'s config: " & ex.Message, MsgBoxStyle.Critical Or MsgBoxStyle.SystemModal, Application.ProductName)
        showedError = True
      Finally
        FUnlock(INIPath)
      End Try
    End Sub
    Private Shared Sub ReadSettings(ByVal state As Object)
      If canUseReg() Then Return
      SyncLock saveLock
        m_IconMethod = ReadIconMethod()
        m_SortMethod = ReadSortMethod()
        m_ShowToolbar = ReadShowToolbar()
        m_FlatView = ReadFlatView()
        m_TreeView = ReadTreeView()
        m_TextEditor = ReadTextEditor()
        m_ImageEditor = ReadImageEditor()
        m_BinaryEditor = ReadBinaryEditor()
        m_WindowSizeWidth = ReadWindowSizeWidth()
        m_WindowSizeHeight = ReadWindowSizeHeight()
        m_TreeWidth = ReadTreeWidth()
      End SyncLock
    End Sub
    Private Shared Sub WriteSettings()
      If canUseReg() Then Return
      SyncLock saveLock
        If tRead IsNot Nothing Then
          tRead.Dispose()
          tRead = Nothing
        End If
        If tSave IsNot Nothing Then
          tSave.Dispose()
          tSave = Nothing
        End If
        tSave = New Threading.Timer(AddressOf TrueWriteSettings, Nothing, saveWait, System.Threading.Timeout.Infinite)
      End SyncLock
    End Sub
    Private Shared Sub TrueWriteSettings(ByVal state As Object)
      If canUseReg() Then Return
      SyncLock saveLock
        If tSave Is Nothing Then Return
        tSave.Dispose()
        tSave = Nothing
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return
        Dim wList As New Dictionary(Of String, Dictionary(Of String, String))
        If Not wList.Keys.Contains(cDefault.IconMethod.File.Key) Then wList.Add(cDefault.IconMethod.File.Key, New Dictionary(Of String, String))
        Dim sIconMethod As String = cDefault.IconMethod.File.Value.Value
        Select Case m_IconMethod
          Case View.LargeIcon : sIconMethod = "LargeIcon"
          Case View.SmallIcon : sIconMethod = "SmallIcon"
          Case View.List : sIconMethod = "List"
          Case View.Tile : sIconMethod = "Tiles"
          Case View.Details : sIconMethod = "Details"
        End Select
        wList.Item(cDefault.IconMethod.File.Key).Add(cDefault.IconMethod.File.Value.Key, sIconMethod)
        If Not wList.Keys.Contains(cDefault.SortMethod.File.Key) Then wList.Add(cDefault.SortMethod.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.SortMethod.File.Key).Add(cDefault.SortMethod.File.Value.Key, m_SortMethod)
        If Not wList.Keys.Contains(cDefault.ShowToolbar.File.Key) Then wList.Add(cDefault.ShowToolbar.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.ShowToolbar.File.Key).Add(cDefault.ShowToolbar.File.Value.Key, IIf(m_ShowToolbar, "Y", "N"))
        If Not wList.Keys.Contains(cDefault.FlatView.File.Key) Then wList.Add(cDefault.FlatView.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.FlatView.File.Key).Add(cDefault.FlatView.File.Value.Key, IIf(m_FlatView, "Y", "N"))
        If Not wList.Keys.Contains(cDefault.TreeView.File.Key) Then wList.Add(cDefault.TreeView.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.TreeView.File.Key).Add(cDefault.TreeView.File.Value.Key, IIf(m_TreeView, "Y", "N"))
        If Not wList.Keys.Contains(cDefault.TextEditor.File.Key) Then wList.Add(cDefault.TextEditor.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.TextEditor.File.Key).Add(cDefault.TextEditor.File.Value.Key, m_TextEditor)
        If Not wList.Keys.Contains(cDefault.ImageEditor.File.Key) Then wList.Add(cDefault.ImageEditor.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.ImageEditor.File.Key).Add(cDefault.ImageEditor.File.Value.Key, m_ImageEditor)
        If Not wList.Keys.Contains(cDefault.BinaryEditor.File.Key) Then wList.Add(cDefault.BinaryEditor.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.BinaryEditor.File.Key).Add(cDefault.BinaryEditor.File.Value.Key, m_BinaryEditor)
        If Not wList.Keys.Contains(cDefault.WindowSizeWidth.File.Key) Then wList.Add(cDefault.WindowSizeWidth.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.WindowSizeWidth.File.Key).Add(cDefault.WindowSizeWidth.File.Value.Key, m_WindowSizeWidth)
        If Not wList.Keys.Contains(cDefault.WindowSizeHeight.File.Key) Then wList.Add(cDefault.WindowSizeHeight.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.WindowSizeHeight.File.Key).Add(cDefault.WindowSizeHeight.File.Value.Key, m_WindowSizeHeight)
        If Not wList.Keys.Contains(cDefault.TreeWidth.File.Key) Then wList.Add(cDefault.TreeWidth.File.Key, New Dictionary(Of String, String))
        wList.Item(cDefault.TreeWidth.File.Key).Add(cDefault.TreeWidth.File.Value.Key, m_TreeWidth)
        SafeWrite(cPath, wList)
        tRead = New Threading.Timer(AddressOf ReadSettings, Nothing, readInterval, readInterval)
      End SyncLock
    End Sub
    Shared Sub New()
      m_IconMethod = cDefault.IconMethod.Return
      m_SortMethod = cDefault.SortMethod.Return
      m_ShowToolbar = cDefault.ShowToolbar.Return
      m_FlatView = cDefault.FlatView.Return
      m_TreeView = cDefault.TreeView.Return
      m_TextEditor = cDefault.TextEditor.Return
      m_ImageEditor = cDefault.ImageEditor.Return
      m_BinaryEditor = cDefault.BinaryEditor.Return
      m_WindowSizeWidth = cDefault.WindowSizeWidth.Return
      m_WindowSizeHeight = cDefault.WindowSizeHeight.Return
      m_TreeWidth = cDefault.TreeWidth.Return
      ReadSettings(Nothing)
      tRead = New Threading.Timer(AddressOf ReadSettings, Nothing, readInterval, readInterval)
    End Sub
    Private Shared m_IconMethod As View
    Private Shared Function ReadIconMethod() As View
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.IconMethod.Return
        Dim r As String = SafeRead(cPath, cDefault.IconMethod.File.Key, cDefault.IconMethod.File.Value.Key, cDefault.IconMethod.File.Value.Value)
        If r.Length < 1 Then Return cDefault.IconMethod.Return
        Select Case r.ToLower
          Case "largeicon" : Return View.LargeIcon
          Case "smallicon" : Return View.SmallIcon
          Case "list" : Return View.List
          Case "tiles" : Return View.Tile
          Case "details" : Return View.Details
        End Select
        Return cDefault.IconMethod.Return
      Catch ex As Exception
        Return cDefault.IconMethod.Return
      End Try
    End Function
    Public Shared Property IconMethod As View
      Get
        Return m_IconMethod
      End Get
      Set(ByVal value As View)
        m_IconMethod = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_SortMethod As String
    Private Shared Function ReadSortMethod() As String
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.SortMethod.Return
        Dim r As String = SafeRead(cPath, cDefault.SortMethod.File.Key, cDefault.SortMethod.File.Value.Key, cDefault.SortMethod.File.Value.Value)
        If r.Length < 1 Then Return cDefault.SortMethod.Return
        Select Case r.ToLower
          Case "order" : Return "Order"
          Case "name" : Return "Name"
          Case "type" : Return "Type"
          Case "size" : Return "Size"
        End Select
        Return cDefault.SortMethod.Return
      Catch ex As Exception
        Return cDefault.SortMethod.Return
      End Try
    End Function
    Public Shared Property SortMethod As String
      Get
        Return m_SortMethod
      End Get
      Set(ByVal value As String)
        m_SortMethod = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_ShowToolbar As Boolean
    Private Shared Function ReadShowToolbar() As Boolean
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.ShowToolbar.Return
        Dim r As String = SafeRead(cPath, cDefault.ShowToolbar.File.Key, cDefault.ShowToolbar.File.Value.Key, cDefault.ShowToolbar.File.Value.Value)
        If r.Length < 1 Then Return cDefault.ShowToolbar.Return
        r = r.Substring(0, 1).ToUpper
        Return r = "Y" OrElse r = "T" OrElse r = "1"
      Catch ex As Exception
        Return cDefault.ShowToolbar.Return
      End Try
    End Function
    Public Shared Property ShowToolbar As Boolean
      Get
        Return m_ShowToolbar
      End Get
      Set(ByVal value As Boolean)
        m_ShowToolbar = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_FlatView As Boolean
    Private Shared Function ReadFlatView() As Boolean
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.FlatView.Return
        Dim r As String = SafeRead(cPath, cDefault.FlatView.File.Key, cDefault.FlatView.File.Value.Key, cDefault.FlatView.File.Value.Value)
        If r.Length < 1 Then Return cDefault.FlatView.Return
        r = r.Substring(0, 1).ToUpper
        Return r = "Y" OrElse r = "T" OrElse r = "1"
      Catch ex As Exception
        Return cDefault.FlatView.Return
      End Try
    End Function
    Public Shared Property FlatView As Boolean
      Get
        Return m_FlatView
      End Get
      Set(ByVal value As Boolean)
        m_FlatView = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_TreeView As Boolean
    Private Shared Function ReadTreeView() As Boolean
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.TreeView.Return
        Dim r As String = SafeRead(cPath, cDefault.TreeView.File.Key, cDefault.TreeView.File.Value.Key, cDefault.TreeView.File.Value.Value)
        If r.Length < 1 Then Return cDefault.TreeView.Return
        r = r.Substring(0, 1).ToUpper
        Return r = "Y" OrElse r = "T" OrElse r = "1"
      Catch ex As Exception
        Return cDefault.TreeView.Return
      End Try
    End Function
    Public Shared Property TreeView As Boolean
      Get
        Return m_TreeView
      End Get
      Set(ByVal value As Boolean)
        m_TreeView = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_TextEditor As String
    Private Shared Function ReadTextEditor() As String
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.TextEditor.Return
        Dim r As String = SafeRead(cPath, cDefault.TextEditor.File.Key, cDefault.TextEditor.File.Value.Key, cDefault.TextEditor.File.Value.Value)
        If r.Length < 1 Then Return cDefault.TextEditor.Return
        Return r
      Catch ex As Exception
        Return cDefault.TextEditor.Return
      End Try
    End Function
    Public Shared Property TextEditor As String
      Get
        Return m_TextEditor
      End Get
      Set(ByVal value As String)
        m_TextEditor = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_ImageEditor As String
    Private Shared Function ReadImageEditor() As String
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.ImageEditor.Return
        Dim r As String = SafeRead(cPath, cDefault.ImageEditor.File.Key, cDefault.ImageEditor.File.Value.Key, cDefault.ImageEditor.File.Value.Value)
        If r.Length < 1 Then Return cDefault.ImageEditor.Return
        Return r
      Catch ex As Exception
        Return cDefault.ImageEditor.Return
      End Try
    End Function
    Public Shared Property ImageEditor As String
      Get
        Return m_ImageEditor
      End Get
      Set(ByVal value As String)
        m_ImageEditor = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_BinaryEditor As String
    Private Shared Function ReadBinaryEditor() As String
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.BinaryEditor.Return
        Dim r As String = SafeRead(cPath, cDefault.BinaryEditor.File.Key, cDefault.BinaryEditor.File.Value.Key, cDefault.BinaryEditor.File.Value.Value)
        If r.Length < 1 Then Return cDefault.BinaryEditor.Return
        Return r
      Catch ex As Exception
        Return cDefault.BinaryEditor.Return
      End Try
    End Function
    Public Shared Property BinaryEditor As String
      Get
        Return m_BinaryEditor
      End Get
      Set(ByVal value As String)
        m_BinaryEditor = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_WindowSizeWidth As UInt32
    Private Shared Function ReadWindowSizeWidth() As UInt32
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.WindowSizeWidth.Return
        Dim r As String = SafeRead(cPath, cDefault.WindowSizeWidth.File.Key, cDefault.WindowSizeWidth.File.Value.Key, cDefault.WindowSizeWidth.File.Value.Value)
        If r.Length < 1 Then Return cDefault.WindowSizeWidth.Return
        Return CUInt(r)
      Catch ex As Exception
        Return cDefault.WindowSizeWidth.Return
      End Try
    End Function
    Public Shared Property WindowSizeWidth As UInt32
      Get
        Return m_WindowSizeWidth
      End Get
      Set(ByVal value As UInt32)
        m_WindowSizeWidth = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_WindowSizeHeight As UInt32
    Private Shared Function ReadWindowSizeHeight() As UInt32
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.WindowSizeHeight.Return
        Dim r As String = SafeRead(cPath, cDefault.WindowSizeHeight.File.Key, cDefault.WindowSizeHeight.File.Value.Key, cDefault.WindowSizeHeight.File.Value.Value)
        If r.Length < 1 Then Return cDefault.WindowSizeHeight.Return
        Return CUInt(r)
      Catch ex As Exception
        Return cDefault.WindowSizeHeight.Return
      End Try
    End Function
    Public Shared Property WindowSizeHeight As UInt32
      Get
        Return m_WindowSizeHeight
      End Get
      Set(ByVal value As UInt32)
        m_WindowSizeHeight = value
        WriteSettings()
      End Set
    End Property
    Private Shared m_TreeWidth As UInt32
    Private Shared Function ReadTreeWidth() As UInt32
      Try
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return cDefault.TreeWidth.Return
        Dim r As String = SafeRead(cPath, cDefault.TreeWidth.File.Key, cDefault.TreeWidth.File.Value.Key, cDefault.TreeWidth.File.Value.Value)
        If r.Length < 1 Then Return cDefault.TreeWidth.Return
        Return CUInt(r)
      Catch ex As Exception
        Return cDefault.TreeWidth.Return
      End Try
    End Function
    Public Shared Property TreeWidth As UInt32
      Get
        Return m_TreeWidth
      End Get
      Set(ByVal value As UInt32)
        m_TreeWidth = value
        WriteSettings()
      End Set
    End Property
    Public Shared Sub LegacyUpgrade()
      Const UNSET As String = "UNSET"
      Dim sAppPath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, "config.ini")
      Dim sDataPath As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.CompanyName, My.Application.Info.ProductName, "config.ini")
      Dim sFoundPath As String = Nothing
      If IO.File.Exists(sAppPath) Then
        sFoundPath = sAppPath
      ElseIf IO.File.Exists(sDataPath) Then
        sFoundPath = sDataPath
      End If
      If String.IsNullOrEmpty(sFoundPath) Then Return
      Try
        Dim sIconMethod As String = INIRead(sFoundPath, "Settings", "IconMethod", UNSET)
        If Not sIconMethod = UNSET Then
          Select Case sIconMethod
            Case "LargeIcon" : cSettings.IconMethod = View.LargeIcon
            Case "SmallIcon" : cSettings.IconMethod = View.SmallIcon
            Case "List" : cSettings.IconMethod = View.List
            Case "Tiles" : cSettings.IconMethod = View.Tile
            Case "Details" : cSettings.IconMethod = View.Details
          End Select
        End If
        Dim sSortMethod As String = INIRead(sFoundPath, "Settings", "SortMethod", UNSET)
        If Not sSortMethod = UNSET Then
          Select Case sSortMethod
            Case "Order" : cSettings.SortMethod = "Order"
            Case "Name" : cSettings.SortMethod = "Name"
            Case "Type" : cSettings.SortMethod = "Type"
            Case "Size" : cSettings.SortMethod = "Size"
          End Select
        End If
        Dim sShowToolbar As String = INIRead(sFoundPath, "Settings", "Toolbar", UNSET)
        If Not sShowToolbar = UNSET Then cSettings.ShowToolbar = Not sShowToolbar = "N"
        Dim sFlatView As String = INIRead(sFoundPath, "Settings", "FlatView", UNSET)
        If Not sFlatView = UNSET Then cSettings.FlatView = Not sFlatView = "N"
        Dim sTreeView As String = INIRead(sFoundPath, "Settings", "TreeView", UNSET)
        If Not sTreeView = UNSET Then cSettings.TreeView = Not sTreeView = "N"
        Dim sEditorText As String = INIRead(sFoundPath, "Editor", "Text", UNSET)
        If Not sEditorText = UNSET Then cSettings.TextEditor = sEditorText
        Dim sEditorImage As String = INIRead(sFoundPath, "Editor", "Image", UNSET)
        If Not sEditorImage = UNSET Then cSettings.ImageEditor = sEditorImage
        Dim sEditorBinary As String = INIRead(sFoundPath, "Editor", "Binary", UNSET)
        If Not sEditorBinary = UNSET Then cSettings.BinaryEditor = sEditorBinary
        Dim sWindowSizeWidth As String = INIRead(sFoundPath, "Size", "Width", UNSET)
        Dim sWindowSizeHeight As String = INIRead(sFoundPath, "Size", "Height", UNSET)
        If Not sWindowSizeHeight = UNSET Or Not sWindowSizeWidth = UNSET Then
          If sWindowSizeHeight = UNSET Then sWindowSizeHeight = "720"
          If sWindowSizeWidth = UNSET Then sWindowSizeWidth = "400"
          cSettings.WindowSize = New Size(Int(sWindowSizeWidth), Int(sWindowSizeHeight))
        End If
        Dim sTreeWidth As String = INIRead(sFoundPath, "Size", "Tree", UNSET)
        If Not sTreeWidth = UNSET Then cSettings.TreeWidth = Int(sTreeWidth)
      Catch ex As Exception
      Finally
        Try
          IO.File.Delete(sFoundPath)
        Catch ex As Exception
        End Try
      End Try
    End Sub
    Public Shared Sub RemoveAll()
      Try
        m_IconMethod = cDefault.IconMethod.Return
        m_SortMethod = cDefault.SortMethod.Return
        m_ShowToolbar = cDefault.ShowToolbar.Return
        m_FlatView = cDefault.FlatView.Return
        m_TreeView = cDefault.TreeView.Return
        m_TextEditor = cDefault.TextEditor.Return
        m_ImageEditor = cDefault.ImageEditor.Return
        m_BinaryEditor = cDefault.BinaryEditor.Return
        m_WindowSizeWidth = cDefault.WindowSizeWidth.Return
        m_WindowSizeHeight = cDefault.WindowSizeHeight.Return
        m_TreeWidth = cDefault.TreeWidth.Return
        Dim cPath As String = saveToINI()
        If cPath = "NO" Then Return
        FileSafeDelete(cPath)
        While cPath.Length > 3
          cPath = IO.Path.GetDirectoryName(cPath)
          If IO.Directory.GetFileSystemEntries(cPath).Length > 0 Then Return
          IO.Directory.Delete(cPath, False)
        End While
      Catch ex As Exception
      End Try
    End Sub
  End Class
  Private Shared Function canUseReg() As Boolean
    If useReg = TriState.True Then Return True
    If useReg = TriState.False Then Return False
    Try
      If IsInstalledIsh Then
        If My.Computer.Registry.CurrentUser.OpenSubKey("Software").GetSubKeyNames.Contains(Application.CompanyName & "-writeTest") Then My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).DeleteSubKeyTree(Application.CompanyName & "-writeTest", False)
        My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).CreateSubKey(Application.CompanyName & "-writeTest")
        My.Computer.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(Application.CompanyName & "-writeTest", True).SetValue("", Application.ProductName, Microsoft.Win32.RegistryValueKind.String)
        If My.Computer.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(Application.CompanyName & "-writeTest").GetValue("", "") = Application.ProductName Then useReg = TriState.True
        My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).DeleteSubKeyTree(Application.CompanyName & "-writeTest", True)
        If useReg = TriState.True Then
          cFile.LegacyUpgrade()
          Return True
        End If
      End If
    Catch ex As Exception
    Finally
      My.Computer.Registry.CurrentUser.OpenSubKey("Software", True).DeleteSubKeyTree(Application.CompanyName & "-writeTest", False)
    End Try
    useReg = TriState.False
    cFile.LegacyUpgrade()
    Return False
  End Function
  Private Shared m_CleanProd As String = Nothing
  Private Shared ReadOnly Property CleanProductName As String
    Get
      If String.IsNullOrEmpty(m_CleanProd) Then m_CleanProd = IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath).
        Where(Function(c As Char) As Boolean
                Return Char.IsLetterOrDigit(c)
              End Function).ToArray()
      Return m_CleanProd
    End Get
  End Property
  Public Shared Property IconMethod As View
    Get
      If canUseReg() Then Return cRegistry.IconMethod
      Return cFile.IconMethod
    End Get
    Set(ByVal value As View)
      If canUseReg() Then
        cRegistry.IconMethod = value
      Else
        cFile.IconMethod = value
      End If
    End Set
  End Property
  Public Shared Property SortMethod As String
    Get
      If canUseReg() Then Return cRegistry.SortMethod
      Return cFile.SortMethod
    End Get
    Set(ByVal value As String)
      If canUseReg() Then
        cRegistry.SortMethod = value
      Else
        cFile.SortMethod = value
      End If
    End Set
  End Property
  Public Shared Property ShowToolbar As Boolean
    Get
      If canUseReg() Then Return cRegistry.ShowToolbar
      Return cFile.ShowToolbar
    End Get
    Set(ByVal value As Boolean)
      If canUseReg() Then
        cRegistry.ShowToolbar = value
      Else
        cFile.ShowToolbar = value
      End If
    End Set
  End Property
  Public Shared Property FlatView As Boolean
    Get
      If canUseReg() Then Return cRegistry.FlatView
      Return cFile.FlatView
    End Get
    Set(ByVal value As Boolean)
      If canUseReg() Then
        cRegistry.FlatView = value
      Else
        cFile.FlatView = value
      End If
    End Set
  End Property
  Public Shared Property TreeView As Boolean
    Get
      If canUseReg() Then Return cRegistry.TreeView
      Return cFile.TreeView
    End Get
    Set(ByVal value As Boolean)
      If canUseReg() Then
        cRegistry.TreeView = value
      Else
        cFile.TreeView = value
      End If
    End Set
  End Property
  Public Shared Property TextEditor As String
    Get
      If canUseReg() Then Return cRegistry.TextEditor
      Return cFile.TextEditor
    End Get
    Set(ByVal value As String)
      If Not IO.File.Exists(value) Then Return
      If canUseReg() Then
        cRegistry.TextEditor = value
      Else
        cFile.TextEditor = value
      End If
    End Set
  End Property
  Public Shared Property ImageEditor As String
    Get
      If canUseReg() Then Return cRegistry.ImageEditor
      Return cFile.ImageEditor
    End Get
    Set(ByVal value As String)
      If Not IO.File.Exists(value) Then Return
      If canUseReg() Then
        cRegistry.ImageEditor = value
      Else
        cFile.ImageEditor = value
      End If
    End Set
  End Property
  Public Shared Property BinaryEditor As String
    Get
      If canUseReg() Then Return cRegistry.BinaryEditor
      Return cFile.BinaryEditor
    End Get
    Set(ByVal value As String)
      If Not IO.File.Exists(value) Then Return
      If canUseReg() Then
        cRegistry.BinaryEditor = value
      Else
        cFile.BinaryEditor = value
      End If
    End Set
  End Property
  Public Shared Property WindowSize As Size
    Get
      Dim szRet As Size
      If canUseReg() Then
        szRet = New Size(cRegistry.WindowSizeWidth, cRegistry.WindowSizeHeight)
      Else
        szRet = New Size(cFile.WindowSizeWidth, cFile.WindowSizeHeight)
      End If
      If szRet.Width > Screen.PrimaryScreen.WorkingArea.Width * 0.8 Then szRet.Width = Screen.PrimaryScreen.WorkingArea.Width * 0.8
      If szRet.Height > Screen.PrimaryScreen.WorkingArea.Height * 0.8 Then szRet.Height = Screen.PrimaryScreen.WorkingArea.Height * 0.8
      If szRet.Width < frmViewer.MinimumSize.Width Then szRet.Width = frmViewer.MinimumSize.Width
      If szRet.Height < frmViewer.MinimumSize.Height Then szRet.Height = frmViewer.MinimumSize.Height
      Return szRet
    End Get
    Set(ByVal value As Size)
      If value.Width > Screen.PrimaryScreen.WorkingArea.Width * 0.8 Then value.Width = Screen.PrimaryScreen.WorkingArea.Width * 0.8
      If value.Height > Screen.PrimaryScreen.WorkingArea.Height * 0.8 Then value.Height = Screen.PrimaryScreen.WorkingArea.Height * 0.8
      If value.Width < frmViewer.MinimumSize.Width Then value.Width = frmViewer.MinimumSize.Width
      If value.Height < frmViewer.MinimumSize.Height Then value.Height = frmViewer.MinimumSize.Height
      If canUseReg() Then
        cRegistry.WindowSizeWidth = value.Width
        cRegistry.WindowSizeHeight = value.Height
      Else
        cFile.WindowSizeWidth = value.Width
        cFile.WindowSizeHeight = value.Height
      End If
    End Set
  End Property
  Public Shared Property TreeWidth As Integer
    Get
      If canUseReg() Then Return cRegistry.TreeWidth
      Return cFile.TreeWidth
    End Get
    Set(ByVal value As Integer)
      If value < 0 Then value = 0
      If canUseReg() Then
        cRegistry.TreeWidth = value
      Else
        cFile.TreeWidth = value
      End If
    End Set
  End Property
  Public Shared Sub RemoveAll()
    If canUseReg() Then
      cRegistry.RemoveAll()
    Else
      cFile.RemoveAll()
    End If
  End Sub
  Public Shared ReadOnly Property IsInstalled As Boolean
    Get
      Dim pfLegacy As String = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
      If pfLegacy.Length > 0 AndAlso pfLegacy.Length <= My.Application.Info.DirectoryPath.Length AndAlso My.Application.Info.DirectoryPath.Substring(0, pfLegacy.Length) = pfLegacy Then Return True
      Dim pfNative As String = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
      If pfNative.Length > 0 AndAlso pfNative.Length <= My.Application.Info.DirectoryPath.Length AndAlso My.Application.Info.DirectoryPath.Substring(0, pfNative.Length) = pfNative Then Return True
      Return False
    End Get
  End Property
  Public Shared ReadOnly Property IsLocal As Boolean
    Get
      Dim pfLocalAppData As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
      If pfLocalAppData.Length = 0 Then Return False
      pfLocalAppData = IO.Path.Combine(pfLocalAppData, "Programs")
      If pfLocalAppData.Length <= My.Application.Info.DirectoryPath.Length AndAlso My.Application.Info.DirectoryPath.Substring(0, pfLocalAppData.Length) = pfLocalAppData Then Return True
      Return False
    End Get
  End Property
  Public Shared ReadOnly Property IsInstalledIsh As Boolean
    Get
      If IsInstalled Then Return True
      If IsLocal Then Return True
      Return False
    End Get
  End Property
End Class
