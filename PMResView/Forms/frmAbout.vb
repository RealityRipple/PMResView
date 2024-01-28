Public NotInheritable Class frmAbout

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
  End Sub

  Private Sub lblProduct_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblProduct.LinkClicked
    If e.Button = Windows.Forms.MouseButtons.Left Then OpenURL("realityripple.com/Software/Applications/PMResView", Me)
  End Sub

  Private Sub lblVersion_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblVersion.LinkClicked
    If e.Button = Windows.Forms.MouseButtons.Left Then OpenURL("realityripple.com/Software/Applications/PMResView/changes.php", Me)
  End Sub

  Private Sub lblCompany_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblCompany.LinkClicked
    If e.Button = Windows.Forms.MouseButtons.Left Then OpenURL("realityripple.com", Me)
  End Sub

  Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
    Me.Close()
  End Sub

  Private Sub cmdDonate_Click(sender As System.Object, e As System.EventArgs) Handles cmdDonate.Click
    OpenURL("realityripple.com/donate.php?itm=PMResView", Me)
  End Sub
End Class
