Module modFunctions

  Public Function SuperMsgBox(Parent As Form, Title As String, Icon As Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon, Header As String, Message As String, Buttons As Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons, Optional Footer As String = Nothing, Optional FooterIcon As Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.None, Optional Fraction As Decimal = -1) As Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult
    Dim sTitle As String = My.Application.Info.Title & " - " & Title
    If Microsoft.WindowsAPICodePack.Dialogs.TaskDialog.IsPlatformSupported Then
      Dim tD As New Microsoft.WindowsAPICodePack.Dialogs.TaskDialog
      tD.OwnerWindowHandle = Parent.Handle
      tD.Caption = sTitle
      tD.InstructionText = Header
      tD.Text = Message
      tD.Icon = Icon
      If Not String.IsNullOrEmpty(Footer) Then
        tD.FooterIcon = FooterIcon
        tD.FooterText = Footer
      End If
      If Fraction >= 0 Then tD.ProgressBar = New Microsoft.WindowsAPICodePack.Dialogs.TaskDialogProgressBar(0, 100, Math.Floor(Fraction * 100))
      tD.StandardButtons = Buttons
      tD.StartupLocation = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStartupLocation.CenterOwner
      AddHandler tD.Opened, AddressOf taskDialog_Opened
      Return tD.Show()
    Else
      Dim sMsg As String = Header & vbNewLine & vbNewLine & Message
      If Not String.IsNullOrEmpty(Footer) Then sMsg &= vbNewLine & vbNewLine & Footer
      Dim mStyle As MsgBoxStyle = MsgBoxStyle.OkOnly
      If (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes And
        (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No Then
        If (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel Then
          mStyle = mStyle Or MsgBoxStyle.YesNoCancel
        Else
          mStyle = mStyle Or MsgBoxStyle.YesNo
        End If
      ElseIf (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok And
        (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel Then
        mStyle = mStyle Or MsgBoxStyle.OkCancel
      ElseIf (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Retry) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Retry And
        (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel Then
        mStyle = mStyle Or MsgBoxStyle.RetryCancel
      End If
      If Icon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Error Then
        mStyle = mStyle Or MsgBoxStyle.Critical
      ElseIf Icon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Warning Then
        mStyle = mStyle Or MsgBoxStyle.Exclamation
      ElseIf Icon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information Then
        mStyle = mStyle Or MsgBoxStyle.Information
      End If
      Dim mRet As MsgBoxResult = MsgBox(sMsg, mStyle, sTitle)
      Select Case mRet
        Case MsgBoxResult.Yes : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes
        Case MsgBoxResult.No : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No
        Case MsgBoxResult.Retry : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Retry
        Case MsgBoxResult.Ok : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Ok
        Case MsgBoxResult.Cancel : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Cancel
        Case MsgBoxResult.Abort : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Close
        Case MsgBoxResult.Ignore : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.CustomButtonClicked
      End Select
      Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.None
    End If
  End Function

  Private Sub taskDialog_Opened(sender As Object, e As EventArgs)
    Dim taskDialog As Microsoft.WindowsAPICodePack.Dialogs.TaskDialog = sender
    If Not taskDialog.Icon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.None Then taskDialog.Icon = taskDialog.Icon
    If Not taskDialog.FooterIcon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.None Then taskDialog.FooterIcon = taskDialog.FooterIcon
    If Not String.IsNullOrEmpty(taskDialog.InstructionText) Then taskDialog.InstructionText = taskDialog.InstructionText
  End Sub

  Public Sub CreateAllSubdirs(sPath As String)
    Dim sSegments() As String = Split(sPath, IO.Path.DirectorySeparatorChar)
    Dim sDir As String = ""
    For I As Integer = 0 To sSegments.Length - 1
      sDir &= sSegments(I) & IO.Path.DirectorySeparatorChar
      If Not IO.Directory.Exists(sDir) Then IO.Directory.CreateDirectory(sDir)
    Next
  End Sub

  Public Function ByteSize(InBytes As UInt64) As String
    If InBytes >= 1000 Then
      If InBytes / 1024 >= 1000 Then
        If InBytes / 1024 / 1024 >= 1000 Then
          If InBytes / 1024 / 1024 / 1024 >= 1000 Then
            If InBytes / 1024 / 1024 / 1024 / 1024 >= 1000 Then
              Return Format((InBytes) / 1024 / 1024 / 1024 / 1024 / 1024, "0.##") & " PB"
            Else
              Return Format((InBytes) / 1024 / 1024 / 1024 / 1024, "0.##") & " TB"
            End If
          Else
            Return Format((InBytes) / 1024 / 1024 / 1024, "0.##") & " GB"
          End If
        Else
          Return Format((InBytes) / 1024 / 1024, "0.##") & " MB"
        End If
      Else
        Return Format((InBytes) / 1024, "0.#") & " KB"
      End If
    Else
      Return InBytes & " B"
    End If
  End Function

  Public Function GetDefaultShell() As String
    Dim explorer As String = "explorer.exe"
    Try
      explorer = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE", False).OpenSubKey("Microsoft", False).OpenSubKey("Windows NT", False).OpenSubKey("CurrentVersion", False).OpenSubKey("Winlogon", False).GetValue("Shell", "explorer.exe")
    Catch ex As Exception
    End Try
    Return explorer
  End Function

  Public Function GetFileIcon(ByVal sPath As String) As Bitmap
    Dim cW As Integer = NativeMethods.GetSystemMetrics(NativeMethods.MetricsList.SM_CXSMICON)
    Dim cH As Integer = NativeMethods.GetSystemMetrics(NativeMethods.MetricsList.SM_CYSMICON)
    Dim shInfo As New NativeMethods.SHFILEINFO
    Dim hImgSmall As IntPtr
    hImgSmall = NativeMethods.SHGetFileInfo(sPath, 0, shInfo, Runtime.InteropServices.Marshal.SizeOf(shInfo), NativeMethods.EShellGetFileInfoConstants.SHGFI_ICON Or NativeMethods.EShellGetFileInfoConstants.SHGFI_SMALLICON)
    Try
      If shInfo.hIcon = 0 Then
        If IO.File.Exists(sPath) Then
          Using bX As New Bitmap(cW, cH)
            Using g As Graphics = Graphics.FromImage(bX)
              g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
              g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
              g.DrawIcon(Icon.ExtractAssociatedIcon(sPath), New Rectangle(0, 0, cW, cH))
            End Using
            Return bX.Clone
          End Using
        End If
        Return Nothing
      End If
      Dim icoRet As Icon = Icon.FromHandle(shInfo.hIcon).Clone
      NativeMethods.DestroyIcon(shInfo.hIcon)
      Using bX As New Bitmap(cW, cH)
        Using g As Graphics = Graphics.FromImage(bX)
          g.DrawIconUnstretched(icoRet, New Rectangle(0, 0, cW, cH))
        End Using
        Return bX.Clone
      End Using
    Catch ex As Exception
      If IO.File.Exists(sPath) Then
        Using bX As New Bitmap(cW, cH)
          Using g As Graphics = Graphics.FromImage(bX)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.DrawIcon(Icon.ExtractAssociatedIcon(sPath), New Rectangle(0, 0, cW, cH))
          End Using
          Return bX.Clone
        End Using
      End If
      Return Nothing
    End Try
  End Function

  Public Function GetFileIcon32(ByVal sPath As String) As Bitmap
    Dim cW As Integer = NativeMethods.GetSystemMetrics(NativeMethods.MetricsList.SM_CXICON)
    Dim cH As Integer = NativeMethods.GetSystemMetrics(NativeMethods.MetricsList.SM_CYICON)
    Dim shInfo As New NativeMethods.SHFILEINFO
    Dim hImgSmall As IntPtr
    hImgSmall = NativeMethods.SHGetFileInfo(sPath, 0, shInfo, Runtime.InteropServices.Marshal.SizeOf(shInfo), NativeMethods.EShellGetFileInfoConstants.SHGFI_ICON Or NativeMethods.EShellGetFileInfoConstants.SHGFI_LARGEICON)
    Try
      If shInfo.hIcon = 0 Then
        If IO.File.Exists(sPath) Then
          Using bX As New Bitmap(cW, cH)
            Using g As Graphics = Graphics.FromImage(bX)
              g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
              g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
              g.DrawIcon(Icon.ExtractAssociatedIcon(sPath), New Rectangle(0, 0, cW, cH))
            End Using
            Return bX.Clone
          End Using
        End If
        Return Nothing
      End If
      Dim icoRet As Icon = Icon.FromHandle(shInfo.hIcon).Clone
      NativeMethods.DestroyIcon(shInfo.hIcon)
      Using bX As New Bitmap(cW, cH)
        Using g As Graphics = Graphics.FromImage(bX)
          g.DrawIconUnstretched(icoRet, New Rectangle(0, 0, cW, cH))
        End Using
        Return bX.Clone
      End Using
    Catch ex As Exception
      If IO.File.Exists(sPath) Then
        Using bX As New Bitmap(cW, cH)
          Using g As Graphics = Graphics.FromImage(bX)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.DrawIcon(Icon.ExtractAssociatedIcon(sPath), New Rectangle(0, 0, cW, cH))
          End Using
          Return bX.Clone
        End Using
      End If
      Return Nothing
    End Try
  End Function
End Module
