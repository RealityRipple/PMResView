﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAbout
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

  Friend WithEvents TableLayoutPanel As System.Windows.Forms.TableLayoutPanel
  Friend WithEvents LogoPictureBox As System.Windows.Forms.PictureBox
  Friend WithEvents lblProduct As PMResView.LinkLabel
  Friend WithEvents lblVersion As PMResView.LinkLabel
  Friend WithEvents lblCompany As PMResView.LinkLabel
  Friend WithEvents txtDescription As System.Windows.Forms.TextBox
  Friend WithEvents cmdOK As System.Windows.Forms.Button
  Friend WithEvents lblUpdate As System.Windows.Forms.Label

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAbout))
    Me.TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
    Me.LogoPictureBox = New System.Windows.Forms.PictureBox()
    Me.lblProduct = New PMResView.LinkLabel()
    Me.lblVersion = New PMResView.LinkLabel()
    Me.lblUpdate = New System.Windows.Forms.Label()
    Me.lblCompany = New PMResView.LinkLabel()
    Me.txtDescription = New System.Windows.Forms.TextBox()
    Me.cmdOK = New System.Windows.Forms.Button()
    Me.cmdDonate = New System.Windows.Forms.Button()
    Me.TableLayoutPanel.SuspendLayout()
    CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'TableLayoutPanel
    '
    Me.TableLayoutPanel.ColumnCount = 3
    Me.TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
    Me.TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
    Me.TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
    Me.TableLayoutPanel.Controls.Add(Me.LogoPictureBox, 0, 0)
    Me.TableLayoutPanel.Controls.Add(Me.lblProduct, 1, 0)
    Me.TableLayoutPanel.Controls.Add(Me.lblVersion, 1, 1)
    Me.TableLayoutPanel.Controls.Add(Me.lblUpdate, 1, 2)
    Me.TableLayoutPanel.Controls.Add(Me.lblCompany, 1, 3)
    Me.TableLayoutPanel.Controls.Add(Me.txtDescription, 1, 4)
    Me.TableLayoutPanel.Controls.Add(Me.cmdOK, 2, 5)
    Me.TableLayoutPanel.Controls.Add(Me.cmdDonate, 1, 5)
    Me.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
    Me.TableLayoutPanel.Location = New System.Drawing.Point(9, 9)
    Me.TableLayoutPanel.Name = "TableLayoutPanel"
    Me.TableLayoutPanel.RowCount = 6
    Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
    Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
    Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
    Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
    Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
    Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
    Me.TableLayoutPanel.Size = New System.Drawing.Size(396, 258)
    Me.TableLayoutPanel.TabIndex = 0
    '
    'LogoPictureBox
    '
    Me.LogoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill
    Me.LogoPictureBox.Image = Global.PMResView.My.Resources.Resources.lunarResources
    Me.LogoPictureBox.Location = New System.Drawing.Point(3, 3)
    Me.LogoPictureBox.Name = "LogoPictureBox"
    Me.TableLayoutPanel.SetRowSpan(Me.LogoPictureBox, 6)
    Me.LogoPictureBox.Size = New System.Drawing.Size(124, 252)
    Me.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
    Me.LogoPictureBox.TabIndex = 0
    Me.LogoPictureBox.TabStop = False
    '
    'lblProduct
    '
    Me.TableLayoutPanel.SetColumnSpan(Me.lblProduct, 2)
    Me.lblProduct.Cursor = System.Windows.Forms.Cursors.Hand
    Me.lblProduct.Dock = System.Windows.Forms.DockStyle.Fill
    Me.lblProduct.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
    Me.lblProduct.ForeColor = System.Drawing.Color.MediumBlue
    Me.lblProduct.Location = New System.Drawing.Point(136, 0)
    Me.lblProduct.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
    Me.lblProduct.MaximumSize = New System.Drawing.Size(0, 17)
    Me.lblProduct.Name = "lblProduct"
    Me.lblProduct.Size = New System.Drawing.Size(257, 17)
    Me.lblProduct.TabIndex = 0
    Me.lblProduct.Text = "Product Name"
    Me.lblProduct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'lblVersion
    '
    Me.TableLayoutPanel.SetColumnSpan(Me.lblVersion, 2)
    Me.lblVersion.Cursor = System.Windows.Forms.Cursors.Hand
    Me.lblVersion.Dock = System.Windows.Forms.DockStyle.Fill
    Me.lblVersion.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
    Me.lblVersion.ForeColor = System.Drawing.Color.MediumBlue
    Me.lblVersion.Location = New System.Drawing.Point(136, 25)
    Me.lblVersion.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
    Me.lblVersion.MaximumSize = New System.Drawing.Size(0, 17)
    Me.lblVersion.Name = "lblVersion"
    Me.lblVersion.Size = New System.Drawing.Size(257, 17)
    Me.lblVersion.TabIndex = 0
    Me.lblVersion.Text = "Version"
    Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'lblUpdate
    '
    Me.lblUpdate.AutoEllipsis = True
    Me.TableLayoutPanel.SetColumnSpan(Me.lblUpdate, 2)
    Me.lblUpdate.Dock = System.Windows.Forms.DockStyle.Fill
    Me.lblUpdate.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.lblUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
    Me.lblUpdate.Location = New System.Drawing.Point(136, 50)
    Me.lblUpdate.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
    Me.lblUpdate.MaximumSize = New System.Drawing.Size(0, 17)
    Me.lblUpdate.Name = "lblUpdate"
    Me.lblUpdate.Size = New System.Drawing.Size(257, 17)
    Me.lblUpdate.TabIndex = 0
    Me.lblUpdate.Text = "Initializing update check"
    Me.lblUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'lblCompany
    '
    Me.TableLayoutPanel.SetColumnSpan(Me.lblCompany, 2)
    Me.lblCompany.Cursor = System.Windows.Forms.Cursors.Hand
    Me.lblCompany.Dock = System.Windows.Forms.DockStyle.Fill
    Me.lblCompany.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
    Me.lblCompany.ForeColor = System.Drawing.Color.MediumBlue
    Me.lblCompany.Location = New System.Drawing.Point(136, 75)
    Me.lblCompany.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
    Me.lblCompany.MaximumSize = New System.Drawing.Size(0, 17)
    Me.lblCompany.Name = "lblCompany"
    Me.lblCompany.Size = New System.Drawing.Size(257, 17)
    Me.lblCompany.TabIndex = 0
    Me.lblCompany.Text = "Company Name"
    Me.lblCompany.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'txtDescription
    '
    Me.TableLayoutPanel.SetColumnSpan(Me.txtDescription, 2)
    Me.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill
    Me.txtDescription.Location = New System.Drawing.Point(136, 103)
    Me.txtDescription.Margin = New System.Windows.Forms.Padding(6, 3, 3, 3)
    Me.txtDescription.Multiline = True
    Me.txtDescription.Name = "txtDescription"
    Me.txtDescription.ReadOnly = True
    Me.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both
    Me.txtDescription.Size = New System.Drawing.Size(257, 123)
    Me.txtDescription.TabIndex = 0
    Me.txtDescription.TabStop = False
    Me.txtDescription.Text = resources.GetString("txtDescription.Text")
    '
    'cmdOK
    '
    Me.cmdOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.cmdOK.Location = New System.Drawing.Point(318, 232)
    Me.cmdOK.Name = "cmdOK"
    Me.cmdOK.Size = New System.Drawing.Size(75, 23)
    Me.cmdOK.TabIndex = 0
    Me.cmdOK.Text = "OK"
    '
    'cmdDonate
    '
    Me.cmdDonate.Anchor = System.Windows.Forms.AnchorStyles.None
    Me.cmdDonate.Location = New System.Drawing.Point(158, 232)
    Me.cmdDonate.Name = "cmdDonate"
    Me.cmdDonate.Size = New System.Drawing.Size(110, 23)
    Me.cmdDonate.TabIndex = 1
    Me.cmdDonate.Text = "Make a Donation"
    Me.cmdDonate.UseVisualStyleBackColor = True
    '
    'frmAbout
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.CancelButton = Me.cmdOK
    Me.ClientSize = New System.Drawing.Size(414, 276)
    Me.Controls.Add(Me.TableLayoutPanel)
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
    Me.MaximizeBox = False
    Me.MinimizeBox = False
    Me.Name = "frmAbout"
    Me.Padding = New System.Windows.Forms.Padding(9)
    Me.ShowInTaskbar = False
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "frmAbout"
    Me.TableLayoutPanel.ResumeLayout(False)
    Me.TableLayoutPanel.PerformLayout()
    CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents cmdDonate As System.Windows.Forms.Button

End Class
