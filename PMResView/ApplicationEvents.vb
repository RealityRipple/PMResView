Namespace My
  Partial Friend Class MyApplication
    Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
      Dim v As Authenticode.Validity = Authenticode.IsSelfSigned(Reflection.Assembly.GetExecutingAssembly().Location)
      If Not (v = Authenticode.Validity.SignedAndValid Or v = Authenticode.Validity.SignedButUntrusted) Then
        Dim sErr As String = "0x" & v.ToString("x")
        If Not CStr(v) = v.ToString Then sErr = v.ToString & " (0x" & v.ToString("x") & ")"
        If SuperMsgBox(Nothing, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Error, "Application integrity not guaranteed.", "The Executable """ & IO.Path.GetFileName(Reflection.Assembly.GetExecutingAssembly().Location) & """ is not signed and may be corrupted or modified." & vbNewLine & "Would you like to continue loading " & My.Application.Info.ProductName & " anyway?", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes Or Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No, , "Error Code: " & sErr, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Warning) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No Then
          e.Cancel = True
          Return
        End If
      End If
      If e.CommandLine IsNot Nothing AndAlso e.CommandLine.Count > 0 Then
        If e.CommandLine.Contains("/uninstall") OrElse e.CommandLine.Contains("-uninstall") Then
          cSettings.RemoveAll()
          e.Cancel = True
          Return
        End If
      End If
    End Sub
  End Class
End Namespace
