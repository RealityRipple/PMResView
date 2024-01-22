Public Class ListViewNoHScroll
  Inherits ListView

  Private Const SB_HORZ As Integer = 0
  Private Const SB_VERT As Integer = 1
  Private Const SB_BOTH As Integer = 3

  Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
    MyBase.WndProc(m)
    NativeMethods.ShowScrollBar(MyBase.Handle, SB_HORZ, False)
  End Sub

End Class