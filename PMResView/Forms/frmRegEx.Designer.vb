<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRegEx
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
    Me.pnlMain = New System.Windows.Forms.TableLayoutPanel()
    Me.pnlButtons = New System.Windows.Forms.TableLayoutPanel()
    Me.cmdOK = New System.Windows.Forms.Button()
    Me.cmdCancel = New System.Windows.Forms.Button()
    Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
    Me.lblInfo = New System.Windows.Forms.Label()
    Me.lblMatch = New System.Windows.Forms.Label()
    Me.pctStatus = New System.Windows.Forms.PictureBox()
    Me.pnlOptions = New System.Windows.Forms.TableLayoutPanel()
    Me.chkIgnoreCase = New System.Windows.Forms.CheckBox()
    Me.chkCultureInvariant = New System.Windows.Forms.CheckBox()
    Me.txtMatch = New System.Windows.Forms.ComboBox()
    Me.lblHelp = New PMResView.LinkLabel()
    Me.pnlMain.SuspendLayout()
    Me.pnlButtons.SuspendLayout()
    Me.TableLayoutPanel1.SuspendLayout()
    CType(Me.pctStatus, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.pnlOptions.SuspendLayout()
    Me.SuspendLayout()
    '
    'pnlMain
    '
    Me.pnlMain.ColumnCount = 2
    Me.pnlMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
    Me.pnlMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
    Me.pnlMain.Controls.Add(Me.pnlButtons, 1, 1)
    Me.pnlMain.Controls.Add(Me.TableLayoutPanel1, 0, 0)
    Me.pnlMain.Controls.Add(Me.lblHelp, 0, 1)
    Me.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill
    Me.pnlMain.Location = New System.Drawing.Point(0, 0)
    Me.pnlMain.Name = "pnlMain"
    Me.pnlMain.RowCount = 2
    Me.pnlMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
    Me.pnlMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
    Me.pnlMain.Size = New System.Drawing.Size(357, 150)
    Me.pnlMain.TabIndex = 0
    '
    'pnlButtons
    '
    Me.pnlButtons.AutoSize = True
    Me.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
    Me.pnlButtons.ColumnCount = 2
    Me.pnlButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.pnlButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.pnlButtons.Controls.Add(Me.cmdOK, 0, 0)
    Me.pnlButtons.Controls.Add(Me.cmdCancel, 1, 0)
    Me.pnlButtons.Location = New System.Drawing.Point(192, 118)
    Me.pnlButtons.Name = "pnlButtons"
    Me.pnlButtons.RowCount = 1
    Me.pnlButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.pnlButtons.Size = New System.Drawing.Size(162, 29)
    Me.pnlButtons.TabIndex = 0
    '
    'cmdOK
    '
    Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.System
    Me.cmdOK.Location = New System.Drawing.Point(3, 3)
    Me.cmdOK.Name = "cmdOK"
    Me.cmdOK.Size = New System.Drawing.Size(75, 23)
    Me.cmdOK.TabIndex = 0
    Me.cmdOK.Text = "OK"
    Me.cmdOK.UseVisualStyleBackColor = True
    '
    'cmdCancel
    '
    Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
    Me.cmdCancel.Location = New System.Drawing.Point(84, 3)
    Me.cmdCancel.Name = "cmdCancel"
    Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
    Me.cmdCancel.TabIndex = 1
    Me.cmdCancel.Text = "Cancel"
    Me.cmdCancel.UseVisualStyleBackColor = True
    '
    'TableLayoutPanel1
    '
    Me.TableLayoutPanel1.ColumnCount = 3
    Me.pnlMain.SetColumnSpan(Me.TableLayoutPanel1, 2)
    Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
    Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
    Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
    Me.TableLayoutPanel1.Controls.Add(Me.lblInfo, 0, 0)
    Me.TableLayoutPanel1.Controls.Add(Me.lblMatch, 0, 1)
    Me.TableLayoutPanel1.Controls.Add(Me.pctStatus, 2, 1)
    Me.TableLayoutPanel1.Controls.Add(Me.pnlOptions, 1, 2)
    Me.TableLayoutPanel1.Controls.Add(Me.txtMatch, 1, 1)
    Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
    Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
    Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
    Me.TableLayoutPanel1.RowCount = 3
    Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
    Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
    Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
    Me.TableLayoutPanel1.Size = New System.Drawing.Size(351, 109)
    Me.TableLayoutPanel1.TabIndex = 2
    '
    'lblInfo
    '
    Me.lblInfo.Anchor = System.Windows.Forms.AnchorStyles.None
    Me.lblInfo.AutoSize = True
    Me.TableLayoutPanel1.SetColumnSpan(Me.lblInfo, 3)
    Me.lblInfo.Location = New System.Drawing.Point(82, 19)
    Me.lblInfo.Name = "lblInfo"
    Me.lblInfo.Size = New System.Drawing.Size(186, 13)
    Me.lblInfo.TabIndex = 0
    Me.lblInfo.Text = "Select files using a regluar expression:"
    '
    'lblMatch
    '
    Me.lblMatch.Anchor = System.Windows.Forms.AnchorStyles.Left
    Me.lblMatch.AutoSize = True
    Me.lblMatch.Location = New System.Drawing.Point(3, 59)
    Me.lblMatch.Name = "lblMatch"
    Me.lblMatch.Size = New System.Drawing.Size(40, 13)
    Me.lblMatch.TabIndex = 1
    Me.lblMatch.Text = "Match:"
    '
    'pctStatus
    '
    Me.pctStatus.Anchor = System.Windows.Forms.AnchorStyles.Right
    Me.pctStatus.Location = New System.Drawing.Point(332, 57)
    Me.pctStatus.Name = "pctStatus"
    Me.pctStatus.Size = New System.Drawing.Size(16, 16)
    Me.pctStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
    Me.pctStatus.TabIndex = 3
    Me.pctStatus.TabStop = False
    '
    'pnlOptions
    '
    Me.pnlOptions.AutoSize = True
    Me.pnlOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
    Me.pnlOptions.ColumnCount = 2
    Me.pnlOptions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.pnlOptions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.pnlOptions.Controls.Add(Me.chkIgnoreCase, 0, 0)
    Me.pnlOptions.Controls.Add(Me.chkCultureInvariant, 1, 0)
    Me.pnlOptions.Dock = System.Windows.Forms.DockStyle.Bottom
    Me.pnlOptions.Location = New System.Drawing.Point(49, 82)
    Me.pnlOptions.Name = "pnlOptions"
    Me.pnlOptions.RowCount = 1
    Me.pnlOptions.RowStyles.Add(New System.Windows.Forms.RowStyle())
    Me.pnlOptions.Size = New System.Drawing.Size(277, 24)
    Me.pnlOptions.TabIndex = 4
    '
    'chkIgnoreCase
    '
    Me.chkIgnoreCase.AutoSize = True
    Me.chkIgnoreCase.FlatStyle = System.Windows.Forms.FlatStyle.System
    Me.chkIgnoreCase.Location = New System.Drawing.Point(3, 3)
    Me.chkIgnoreCase.Name = "chkIgnoreCase"
    Me.chkIgnoreCase.Size = New System.Drawing.Size(89, 18)
    Me.chkIgnoreCase.TabIndex = 0
    Me.chkIgnoreCase.Text = "Ignore Case"
    Me.chkIgnoreCase.UseVisualStyleBackColor = True
    '
    'chkCultureInvariant
    '
    Me.chkCultureInvariant.AutoSize = True
    Me.chkCultureInvariant.FlatStyle = System.Windows.Forms.FlatStyle.System
    Me.chkCultureInvariant.Location = New System.Drawing.Point(141, 3)
    Me.chkCultureInvariant.Name = "chkCultureInvariant"
    Me.chkCultureInvariant.Size = New System.Drawing.Size(109, 18)
    Me.chkCultureInvariant.TabIndex = 1
    Me.chkCultureInvariant.Text = "Culture Invariant"
    Me.chkCultureInvariant.UseVisualStyleBackColor = True
    '
    'txtMatch
    '
    Me.txtMatch.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.txtMatch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
    Me.txtMatch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
    Me.txtMatch.FlatStyle = System.Windows.Forms.FlatStyle.System
    Me.txtMatch.FormattingEnabled = True
    Me.txtMatch.Location = New System.Drawing.Point(49, 55)
    Me.txtMatch.Name = "txtMatch"
    Me.txtMatch.Size = New System.Drawing.Size(277, 21)
    Me.txtMatch.TabIndex = 5
    '
    'lblHelp
    '
    Me.lblHelp.Anchor = System.Windows.Forms.AnchorStyles.Left
    Me.lblHelp.AutoSize = True
    Me.lblHelp.Cursor = System.Windows.Forms.Cursors.Hand
    Me.lblHelp.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
    Me.lblHelp.ForeColor = System.Drawing.Color.MediumBlue
    Me.lblHelp.Location = New System.Drawing.Point(3, 126)
    Me.lblHelp.Name = "lblHelp"
    Me.lblHelp.Size = New System.Drawing.Size(150, 13)
    Me.lblHelp.TabIndex = 3
    Me.lblHelp.Text = "Help with Regular Expressions"
    '
    'frmRegEx
    '
    Me.AcceptButton = Me.cmdOK
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.CancelButton = Me.cmdCancel
    Me.ClientSize = New System.Drawing.Size(357, 150)
    Me.Controls.Add(Me.pnlMain)
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
    Me.MaximizeBox = False
    Me.MinimizeBox = False
    Me.Name = "frmRegEx"
    Me.ShowIcon = False
    Me.ShowInTaskbar = False
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "Advanced Selection"
    Me.pnlMain.ResumeLayout(False)
    Me.pnlMain.PerformLayout()
    Me.pnlButtons.ResumeLayout(False)
    Me.TableLayoutPanel1.ResumeLayout(False)
    Me.TableLayoutPanel1.PerformLayout()
    CType(Me.pctStatus, System.ComponentModel.ISupportInitialize).EndInit()
    Me.pnlOptions.ResumeLayout(False)
    Me.pnlOptions.PerformLayout()
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents pnlMain As System.Windows.Forms.TableLayoutPanel
  Friend WithEvents pnlButtons As System.Windows.Forms.TableLayoutPanel
  Friend WithEvents cmdOK As System.Windows.Forms.Button
  Friend WithEvents cmdCancel As System.Windows.Forms.Button
  Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
  Friend WithEvents lblInfo As System.Windows.Forms.Label
  Friend WithEvents lblMatch As System.Windows.Forms.Label
  Friend WithEvents pctStatus As System.Windows.Forms.PictureBox
  Friend WithEvents pnlOptions As System.Windows.Forms.TableLayoutPanel
  Friend WithEvents chkIgnoreCase As System.Windows.Forms.CheckBox
  Friend WithEvents chkCultureInvariant As System.Windows.Forms.CheckBox
  Friend WithEvents lblHelp As PMResView.LinkLabel
  Friend WithEvents txtMatch As System.Windows.Forms.ComboBox
End Class
