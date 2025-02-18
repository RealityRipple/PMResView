﻿Public Class frmRegEx
  Private Const RegExURL As String = "learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference"
  Public Property RegEx As String
    Get
      Return txtMatch.Text
    End Get
    Set(ByVal value As String)
      txtMatch.Text = value
    End Set
  End Property
  Public Property IsCultureInvariant As Boolean
    Get
      Return chkCultureInvariant.Checked
    End Get
    Set(ByVal value As Boolean)
      chkCultureInvariant.Checked = value
    End Set
  End Property
  Public Property CaseInsensitive As Boolean
    Get
      Return chkIgnoreCase.Checked
    End Get
    Set(ByVal value As Boolean)
      chkIgnoreCase.Checked = value
    End Set
  End Property
  Public WriteOnly Property History As String()
    Set(ByVal value As String())
      txtMatch.Items.AddRange(value)
    End Set
  End Property
  Public Sub New(ByVal Match As Boolean)
    InitializeComponent()
    If Match Then
      Me.Text = "Advanced Selection"
      lblInfo.Text = "Select files using a regluar expression:"
      lblMatch.Text = "Match:"
    Else
      Me.Text = "Advanced Deselection"
      lblInfo.Text = "Unselect files using a regluar expression:"
      lblMatch.Text = "Unmatch:"
    End If
  End Sub
  Private Sub lblHelp_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblHelp.LinkClicked
    If e.Button = Windows.Forms.MouseButtons.Left Then OpenURL(regexurl, Me)
  End Sub
  Private Sub txtMatch_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtMatch.KeyUp
    TestRegex()
  End Sub
  Private Sub txtMatch_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtMatch.SelectedValueChanged
    TestRegex()
  End Sub
  Private Sub txtMatch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtMatch.TextChanged
    TestRegex()
  End Sub
  Private Sub TestRegex()
    If String.IsNullOrEmpty(Me.RegEx) Then
      pctStatus.Image = My.Resources.bad
      cmdOK.Enabled = False
      Return
    End If
    Try
      Dim rOpts As System.Text.RegularExpressions.RegexOptions = System.Text.RegularExpressions.RegexOptions.None
      If chkCultureInvariant.Checked Then rOpts = rOpts Or System.Text.RegularExpressions.RegexOptions.CultureInvariant
      If chkIgnoreCase.Checked Then rOpts = rOpts Or System.Text.RegularExpressions.RegexOptions.IgnoreCase
      Dim r As New System.Text.RegularExpressions.Regex(Me.RegEx, rOpts)
      pctStatus.Image = My.Resources.ok
      cmdOK.Enabled = True
    Catch ex As Exception
      pctStatus.Image = My.Resources.bad
      cmdOK.Enabled = False
    End Try
  End Sub
  Private Sub lblHelp_LinkClicked_1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
  End Sub
End Class