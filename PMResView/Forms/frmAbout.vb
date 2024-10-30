Public NotInheritable Class frmAbout
  Private WithEvents cUpdate As clsUpdate
  Private tUpdate As Threading.Timer
  Private tThrobber As Threading.Timer
  Private sUpdate As String = IO.Path.Combine(IO.Path.GetTempPath, "PRV_Setup.exe")
  Private ellipsis As String
  Private Const HomeURL As String = "realityripple.com"
  Private Const ProductURL As String = "realityripple.com/Software/Applications/PMResView"
  Private Const DonateURL As String = "realityripple.com/donate.php?itm=PMResView"
  Private Sub frmAbout_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    Dim ApplicationTitle As String
    If My.Application.Info.Title <> "" Then
      ApplicationTitle = My.Application.Info.Title
    Else
      ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
    End If
    Me.Text = String.Format("About {0}", ApplicationTitle)
    lblProduct.Text = My.Application.Info.ProductName
    lblVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString(2))
    lblCompany.Text = My.Application.Info.CompanyName
    txtDescription.Text = My.Application.Info.Description
    SetUpdateValue("Initializing update check", UpdateStatus.Throbber)
    tUpdate = New Threading.Timer(New Threading.TimerCallback(AddressOf CheckForUpdates), Nothing, 1000, 5000)
  End Sub
  Private Sub lblProduct_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblProduct.LinkClicked
    If e.Button = Windows.Forms.MouseButtons.Left Then OpenURL(ProductURL, Me)
  End Sub
  Private Sub lblVersion_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblVersion.LinkClicked
    If e.Button = Windows.Forms.MouseButtons.Left Then OpenURL(ChangeLogURL, Me)
  End Sub
  Private Sub lblCompany_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblCompany.LinkClicked
    If e.Button = Windows.Forms.MouseButtons.Left Then OpenURL(HomeURL, Me)
  End Sub
  Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
    Me.Close()
  End Sub
  Private Sub cmdDonate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDonate.Click
    OpenURL(DonateURL, Me)
  End Sub
  Private Enum UpdateStatus
    Throbber
    Red
    Green
    None
  End Enum
  Private Sub SetUpdateValue(ByVal Message As String, ByVal Status As UpdateStatus)
    If Status = UpdateStatus.Throbber Then
      If tThrobber Is Nothing Then
        throbberFrame = 0
        tThrobber = New Threading.Timer(New Threading.TimerCallback(AddressOf tmrThrobber_Tick), Nothing, 0, 33)
      End If
      If Not lblUpdate.Text = "      " & Message Then lblUpdate.Text = "      " & Message
    ElseIf Status = UpdateStatus.Red Then
      throbberFrame = 255
      If tThrobber IsNot Nothing Then
        tThrobber.Dispose()
        tThrobber = Nothing
      End If
      lblUpdate.Image = My.Resources.bad.Clone
      If Not lblUpdate.Text = "      " & Message Then lblUpdate.Text = "      " & Message
    ElseIf Status = UpdateStatus.Green Then
      throbberFrame = 255
      If tThrobber IsNot Nothing Then
        tThrobber.Dispose()
        tThrobber = Nothing
      End If
      lblUpdate.Image = My.Resources.ok.Clone
      If Not lblUpdate.Text = "      " & Message Then lblUpdate.Text = "      " & Message
    Else
      throbberFrame = 255
      If tThrobber IsNot Nothing Then
        tThrobber.Dispose()
        tThrobber = Nothing
      End If
      lblUpdate.Image = Nothing
      If Not lblUpdate.Text = Message Then lblUpdate.Text = Message
    End If
  End Sub
#Region "Updates"
  Private Sub CheckForUpdates(ByVal state As Object)
    If InvokeRequired Then
      Invoke(New Threading.TimerCallback(AddressOf CheckForUpdates), state)
      Return
    End If
    If tUpdate IsNot Nothing Then
      tUpdate.Dispose()
      tUpdate = Nothing
    Else
      Return
    End If
    ellipsis = ""
    SetUpdateValue("Checking for updates", UpdateStatus.Throbber)
    cUpdate = New clsUpdate
    cUpdate.CheckVersion()
  End Sub
  Private Sub cUpdate_CheckingVersion(ByVal sender As Object, ByVal e As System.EventArgs) Handles cUpdate.CheckingVersion
    If InvokeRequired Then
      Invoke(New EventHandler(AddressOf cUpdate_CheckingVersion), sender, e)
      Return
    End If
    ellipsis &= "."
    If ellipsis = "...." Then ellipsis = ""
    SetUpdateValue("Checking for updates" & ellipsis, UpdateStatus.Throbber)
  End Sub
  Private Sub cUpdate_CheckProgressChanged(ByVal sender As Object, ByVal e As clsUpdate.ProgressEventArgs) Handles cUpdate.CheckProgressChanged
    If InvokeRequired Then
      Invoke(New EventHandler(Of clsUpdate.ProgressEventArgs)(AddressOf cUpdate_CheckProgressChanged), sender, e)
      Return
    End If
    ellipsis &= "."
    If ellipsis = "...." Then ellipsis = ""
    SetUpdateValue("Checking for updates" & ellipsis, UpdateStatus.Throbber)
  End Sub
  Private Sub cUpdate_CheckResult(ByVal sender As Object, ByVal e As clsUpdate.CheckEventArgs) Handles cUpdate.CheckResult
    If InvokeRequired Then
      Invoke(New EventHandler(Of clsUpdate.CheckEventArgs)(AddressOf cUpdate_CheckResult), sender, e)
      Return
    End If
    If e.Cancelled Then
      SetUpdateValue("Update check cancelled", UpdateStatus.Red)
    ElseIf e.Error IsNot Nothing Then
      SetUpdateValue("Update check failed", UpdateStatus.Red)
    Else
      If e.Result = clsUpdate.CheckEventArgs.ResultType.NewUpdate Then
        If IO.File.Exists(IO.Path.Combine(My.Application.Info.DirectoryPath, "unins000.exe")) Then
          SetUpdateValue("New version available: " & e.Version, UpdateStatus.Green)
          Using dUpdate As New dlgUpdate(e.Version)
            If dUpdate.ShowDialog(Me) = Windows.Forms.DialogResult.Cancel Then
              SetUpdateValue("Update download cancelled", UpdateStatus.Red)
              Return
            End If
          End Using
          Try
            If IO.File.Exists(sUpdate) Then IO.File.Delete(sUpdate)
            ellipsis = ""
            cUpdate.DownloadUpdate(sUpdate)
          Catch ex As Exception
            SetUpdateValue("Update download file in use", UpdateStatus.Red)
          End Try
        Else
          SetUpdateValue("New version available: " & e.Version, UpdateStatus.Green)
        End If
      Else
        SetUpdateValue("No new updates", UpdateStatus.Green)
      End If
    End If
  End Sub
  Private Sub cUpdate_DownloadingUpdate(ByVal sender As Object, ByVal e As System.EventArgs) Handles cUpdate.DownloadingUpdate
    If InvokeRequired Then
      Invoke(New EventHandler(AddressOf cUpdate_DownloadingUpdate), sender, e)
      Return
    End If
    SetUpdateValue("Downloading new version...", UpdateStatus.Throbber)
  End Sub
  Private Sub cUpdate_UpdateProgressChanged(ByVal sender As Object, ByVal e As clsUpdate.ProgressEventArgs) Handles cUpdate.UpdateProgressChanged
    If InvokeRequired Then
      Invoke(New EventHandler(Of clsUpdate.ProgressEventArgs)(AddressOf cUpdate_UpdateProgressChanged), sender, e)
      Return
    End If
    SetUpdateValue("Downloading new version: " & e.ProgressPercentage & "%", UpdateStatus.Throbber)
  End Sub
  Private Sub cUpdate_DownloadResult(ByVal sender As Object, ByVal e As clsUpdate.DownloadEventArgs) Handles cUpdate.DownloadResult
    If InvokeRequired Then
      Invoke(New EventHandler(Of clsUpdate.DownloadEventArgs)(AddressOf cUpdate_DownloadResult), sender, e)
      Return
    End If
    If e.Cancelled Then
      SetUpdateValue("Update download cancelled", UpdateStatus.Red)
    ElseIf e.Error IsNot Nothing Then
      SetUpdateValue("Update download failed", UpdateStatus.Red)
    Else
      SetUpdateValue("Update download complete", UpdateStatus.Green)
      Dim v As Authenticode.Validity = Authenticode.IsSelfSigned(sUpdate)
      If Not (v = Authenticode.Validity.SignedAndValid Or v = Authenticode.Validity.SignedButUntrusted) Then
        Dim sErr As String = "0x" & v.ToString("x")
        If Not CStr(v) = v.ToString Then sErr = v.ToString & " (0x" & v.ToString("x") & ")"
        SetUpdateValue("Update was not correctly signed: " & sErr, UpdateStatus.Red)
        IO.File.Delete(sUpdate)
        Return
      End If
      Try
        Dim oProc As New Process
        oProc.StartInfo.FileName = sUpdate
        oProc.StartInfo.Arguments = "/silent"
        oProc.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        oProc.StartInfo.UseShellExecute = False
        oProc.Start()
        Application.Exit()
      Catch ex As Exception
        SetUpdateValue("Update failed to install", UpdateStatus.Red)
      End Try
    End If
  End Sub
#End Region
#Region "Throbber"
  Private throbberFrame As Byte = 0
  Private throbberHeight As Integer = -1
  Private Sub tmrThrobber_Tick(ByVal state As Object)
    If tThrobber Is Nothing Then Return
    If throbberFrame = 255 Then Return
    If throbberHeight = -1 Then throbberHeight = My.Resources.throbber.Height
    Dim r As New Rectangle(0, throbberFrame * 16, 16, 16)
    throbberFrame += 1
    If throbberFrame * 16 >= throbberHeight Then throbberFrame = 0
    Using pFrame As New Bitmap(16, 16, Imaging.PixelFormat.Format32bppArgb)
      Using g As Graphics = Graphics.FromImage(pFrame)
        g.DrawImage(My.Resources.throbber, New Rectangle(0, 0, 16, 16), r, GraphicsUnit.Pixel)
      End Using
      If throbberFrame = 255 Then Return
      lblUpdate.Image = pFrame.Clone
    End Using
  End Sub
#End Region
End Class
