Public Class dlgUpdate
  Private Const ChangeLogURL As String = "//realityripple.com/Software/Applications/PMResView/changes.php"
  Private Const BCM_SETSHIELD As Integer = &H160C
  <Runtime.InteropServices.DllImport("user32", CharSet:=Runtime.InteropServices.CharSet.Auto, setlasterror:=True)>
  Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal msg As UInt32, ByVal wParam As UInt32, ByVal lParam As UInt32) As UInt32
  End Function
  Public Sub New()
    Me.New("1.0.0.0")
  End Sub
  Public Sub New(ByVal ProgramVersion As String)
    InitializeComponent()
    If String.IsNullOrEmpty(ProgramVersion) Then ProgramVersion = "1.0.0.0"
    Do While ProgramVersion.EndsWith(".0")
      ProgramVersion = ProgramVersion.Substring(0, ProgramVersion.Length - 2)
    Loop
    lblTitle.Text = lblTitle.Text.Replace("%P", Application.ProductName).Replace("%1", ProgramVersion)
    lblInfo.Text = lblInfo.Text.Replace("%P", Application.ProductName).Replace("%1", ProgramVersion)
  End Sub
  Private Sub dlgUpdate_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
    SendMessage(cmdUpdate.Handle, BCM_SETSHIELD, 0, 1)
  End Sub
  Private Sub lblChangeLog_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblChangeLog.LinkClicked
    If e.Button = Windows.Forms.MouseButtons.Left Then OpenURL(clsUpdate.ProtoURL(ChangeLogURL), Me)
  End Sub
  Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
    Me.DialogResult = Windows.Forms.DialogResult.OK
  End Sub
  Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
    Me.DialogResult = Windows.Forms.DialogResult.Cancel
  End Sub
End Class
