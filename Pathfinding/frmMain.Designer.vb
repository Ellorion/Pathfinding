<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.pbImage = New System.Windows.Forms.PictureBox()
        Me.grpPosition = New System.Windows.Forms.GroupBox()
        Me.btnWall = New System.Windows.Forms.Button()
        Me.btnStopPos = New System.Windows.Forms.Button()
        Me.btnStartPos = New System.Windows.Forms.Button()
        Me.btnStartPathfinding = New System.Windows.Forms.Button()
        Me.grpPathfinding = New System.Windows.Forms.GroupBox()
        Me.ckbAvoidRotation = New System.Windows.Forms.CheckBox()
        Me.ckbDriveEnabled = New System.Windows.Forms.CheckBox()
        Me.cboPFStategies = New System.Windows.Forms.ComboBox()
        Me.ckbAnimation = New System.Windows.Forms.CheckBox()
        Me.grpOptions = New System.Windows.Forms.GroupBox()
        Me.ckbDebugMode = New System.Windows.Forms.CheckBox()
        Me.btnClearMap = New System.Windows.Forms.Button()
        Me.nudGridSize = New System.Windows.Forms.NumericUpDown()
        Me.lblGridSize = New System.Windows.Forms.Label()
        Me.nudRowCount = New System.Windows.Forms.NumericUpDown()
        Me.lblRows = New System.Windows.Forms.Label()
        Me.nudColumnCount = New System.Windows.Forms.NumericUpDown()
        Me.lblColumns = New System.Windows.Forms.Label()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.tsStatus = New System.Windows.Forms.ToolStripStatusLabel()
        CType(Me.pbImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpPosition.SuspendLayout()
        Me.grpPathfinding.SuspendLayout()
        Me.grpOptions.SuspendLayout()
        CType(Me.nudGridSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudRowCount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudColumnCount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'pbImage
        '
        Me.pbImage.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbImage.Location = New System.Drawing.Point(12, 92)
        Me.pbImage.Name = "pbImage"
        Me.pbImage.Size = New System.Drawing.Size(544, 322)
        Me.pbImage.TabIndex = 0
        Me.pbImage.TabStop = False
        '
        'grpPosition
        '
        Me.grpPosition.Controls.Add(Me.btnWall)
        Me.grpPosition.Controls.Add(Me.btnStopPos)
        Me.grpPosition.Controls.Add(Me.btnStartPos)
        Me.grpPosition.Location = New System.Drawing.Point(12, 12)
        Me.grpPosition.Name = "grpPosition"
        Me.grpPosition.Size = New System.Drawing.Size(252, 71)
        Me.grpPosition.TabIndex = 0
        Me.grpPosition.TabStop = False
        Me.grpPosition.Text = "Positions"
        '
        'btnWall
        '
        Me.btnWall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnWall.Location = New System.Drawing.Point(168, 19)
        Me.btnWall.Name = "btnWall"
        Me.btnWall.Size = New System.Drawing.Size(75, 46)
        Me.btnWall.TabIndex = 2
        Me.btnWall.Text = "&Wall"
        Me.btnWall.UseVisualStyleBackColor = True
        '
        'btnStopPos
        '
        Me.btnStopPos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnStopPos.Location = New System.Drawing.Point(87, 19)
        Me.btnStopPos.Name = "btnStopPos"
        Me.btnStopPos.Size = New System.Drawing.Size(75, 46)
        Me.btnStopPos.TabIndex = 1
        Me.btnStopPos.Text = "&End"
        Me.btnStopPos.UseVisualStyleBackColor = True
        '
        'btnStartPos
        '
        Me.btnStartPos.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStartPos.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnStartPos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnStartPos.Location = New System.Drawing.Point(6, 19)
        Me.btnStartPos.Name = "btnStartPos"
        Me.btnStartPos.Size = New System.Drawing.Size(75, 46)
        Me.btnStartPos.TabIndex = 0
        Me.btnStartPos.Text = "&Begin"
        Me.btnStartPos.UseVisualStyleBackColor = True
        '
        'btnStartPathfinding
        '
        Me.btnStartPathfinding.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStartPathfinding.Location = New System.Drawing.Point(205, 19)
        Me.btnStartPathfinding.Name = "btnStartPathfinding"
        Me.btnStartPathfinding.Size = New System.Drawing.Size(75, 23)
        Me.btnStartPathfinding.TabIndex = 1
        Me.btnStartPathfinding.Text = "&Start"
        Me.btnStartPathfinding.UseVisualStyleBackColor = True
        '
        'grpPathfinding
        '
        Me.grpPathfinding.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpPathfinding.Controls.Add(Me.ckbAvoidRotation)
        Me.grpPathfinding.Controls.Add(Me.ckbDriveEnabled)
        Me.grpPathfinding.Controls.Add(Me.cboPFStategies)
        Me.grpPathfinding.Controls.Add(Me.btnStartPathfinding)
        Me.grpPathfinding.Controls.Add(Me.ckbAnimation)
        Me.grpPathfinding.Location = New System.Drawing.Point(270, 12)
        Me.grpPathfinding.Name = "grpPathfinding"
        Me.grpPathfinding.Size = New System.Drawing.Size(286, 71)
        Me.grpPathfinding.TabIndex = 1
        Me.grpPathfinding.TabStop = False
        Me.grpPathfinding.Text = "Pathfinding"
        '
        'ckbAvoidRotation
        '
        Me.ckbAvoidRotation.AutoSize = True
        Me.ckbAvoidRotation.Location = New System.Drawing.Point(178, 48)
        Me.ckbAvoidRotation.Name = "ckbAvoidRotation"
        Me.ckbAvoidRotation.Size = New System.Drawing.Size(101, 17)
        Me.ckbAvoidRotation.TabIndex = 6
        Me.ckbAvoidRotation.Text = "A&void Rotations"
        Me.ckbAvoidRotation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ckbAvoidRotation.UseVisualStyleBackColor = True
        '
        'ckbDriveEnabled
        '
        Me.ckbDriveEnabled.AutoSize = True
        Me.ckbDriveEnabled.Location = New System.Drawing.Point(6, 48)
        Me.ckbDriveEnabled.Name = "ckbDriveEnabled"
        Me.ckbDriveEnabled.Size = New System.Drawing.Size(88, 17)
        Me.ckbDriveEnabled.TabIndex = 5
        Me.ckbDriveEnabled.Text = "Path -> D&rive"
        Me.ckbDriveEnabled.UseVisualStyleBackColor = True
        '
        'cboPFStategies
        '
        Me.cboPFStategies.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboPFStategies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboPFStategies.FormattingEnabled = True
        Me.cboPFStategies.Location = New System.Drawing.Point(6, 21)
        Me.cboPFStategies.Name = "cboPFStategies"
        Me.cboPFStategies.Size = New System.Drawing.Size(193, 21)
        Me.cboPFStategies.TabIndex = 0
        '
        'ckbAnimation
        '
        Me.ckbAnimation.AutoSize = True
        Me.ckbAnimation.Location = New System.Drawing.Point(100, 48)
        Me.ckbAnimation.Name = "ckbAnimation"
        Me.ckbAnimation.Size = New System.Drawing.Size(72, 17)
        Me.ckbAnimation.TabIndex = 0
        Me.ckbAnimation.Text = "&Animation"
        Me.ckbAnimation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ckbAnimation.UseVisualStyleBackColor = True
        '
        'grpOptions
        '
        Me.grpOptions.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.grpOptions.Controls.Add(Me.ckbDebugMode)
        Me.grpOptions.Controls.Add(Me.btnClearMap)
        Me.grpOptions.Controls.Add(Me.nudGridSize)
        Me.grpOptions.Controls.Add(Me.lblGridSize)
        Me.grpOptions.Controls.Add(Me.nudRowCount)
        Me.grpOptions.Controls.Add(Me.lblRows)
        Me.grpOptions.Controls.Add(Me.nudColumnCount)
        Me.grpOptions.Controls.Add(Me.lblColumns)
        Me.grpOptions.Location = New System.Drawing.Point(12, 420)
        Me.grpOptions.Name = "grpOptions"
        Me.grpOptions.Size = New System.Drawing.Size(544, 45)
        Me.grpOptions.TabIndex = 2
        Me.grpOptions.TabStop = False
        Me.grpOptions.Text = "Options"
        '
        'ckbDebugMode
        '
        Me.ckbDebugMode.AutoSize = True
        Me.ckbDebugMode.Location = New System.Drawing.Point(37, 19)
        Me.ckbDebugMode.Name = "ckbDebugMode"
        Me.ckbDebugMode.Size = New System.Drawing.Size(58, 17)
        Me.ckbDebugMode.TabIndex = 1
        Me.ckbDebugMode.Text = "&Debug"
        Me.ckbDebugMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ckbDebugMode.UseVisualStyleBackColor = True
        '
        'btnClearMap
        '
        Me.btnClearMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearMap.Location = New System.Drawing.Point(444, 13)
        Me.btnClearMap.Name = "btnClearMap"
        Me.btnClearMap.Size = New System.Drawing.Size(94, 23)
        Me.btnClearMap.TabIndex = 5
        Me.btnClearMap.Text = "&Clear map"
        Me.btnClearMap.UseVisualStyleBackColor = True
        '
        'nudGridSize
        '
        Me.nudGridSize.Location = New System.Drawing.Point(368, 16)
        Me.nudGridSize.Maximum = New Decimal(New Integer() {50, 0, 0, 0})
        Me.nudGridSize.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.nudGridSize.Name = "nudGridSize"
        Me.nudGridSize.Size = New System.Drawing.Size(43, 20)
        Me.nudGridSize.TabIndex = 4
        Me.nudGridSize.Value = New Decimal(New Integer() {32, 0, 0, 0})
        '
        'lblGridSize
        '
        Me.lblGridSize.AutoSize = True
        Me.lblGridSize.Location = New System.Drawing.Point(316, 20)
        Me.lblGridSize.Name = "lblGridSize"
        Me.lblGridSize.Size = New System.Drawing.Size(49, 13)
        Me.lblGridSize.TabIndex = 8
        Me.lblGridSize.Text = "GridSize:"
        '
        'nudRowCount
        '
        Me.nudRowCount.Location = New System.Drawing.Point(266, 16)
        Me.nudRowCount.Maximum = New Decimal(New Integer() {85, 0, 0, 0})
        Me.nudRowCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudRowCount.Name = "nudRowCount"
        Me.nudRowCount.Size = New System.Drawing.Size(44, 20)
        Me.nudRowCount.TabIndex = 3
        Me.nudRowCount.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'lblRows
        '
        Me.lblRows.AutoSize = True
        Me.lblRows.Location = New System.Drawing.Point(223, 20)
        Me.lblRows.Name = "lblRows"
        Me.lblRows.Size = New System.Drawing.Size(37, 13)
        Me.lblRows.TabIndex = 7
        Me.lblRows.Text = "Rows:"
        '
        'nudColumnCount
        '
        Me.nudColumnCount.Location = New System.Drawing.Point(173, 16)
        Me.nudColumnCount.Maximum = New Decimal(New Integer() {180, 0, 0, 0})
        Me.nudColumnCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudColumnCount.Name = "nudColumnCount"
        Me.nudColumnCount.Size = New System.Drawing.Size(44, 20)
        Me.nudColumnCount.TabIndex = 2
        Me.nudColumnCount.Value = New Decimal(New Integer() {17, 0, 0, 0})
        '
        'lblColumns
        '
        Me.lblColumns.AutoSize = True
        Me.lblColumns.Location = New System.Drawing.Point(119, 20)
        Me.lblColumns.Name = "lblColumns"
        Me.lblColumns.Size = New System.Drawing.Size(50, 13)
        Me.lblColumns.TabIndex = 6
        Me.lblColumns.Text = "Columns:"
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsStatus})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 474)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(568, 22)
        Me.StatusStrip.TabIndex = 3
        Me.StatusStrip.Text = "StatusStrip1"
        '
        'tsStatus
        '
        Me.tsStatus.Name = "tsStatus"
        Me.tsStatus.Size = New System.Drawing.Size(61, 17)
        Me.tsStatus.Text = "Status: idle"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(568, 496)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.grpOptions)
        Me.Controls.Add(Me.grpPathfinding)
        Me.Controls.Add(Me.grpPosition)
        Me.Controls.Add(Me.pbImage)
        Me.DoubleBuffered = True
        Me.KeyPreview = True
        Me.MinimumSize = New System.Drawing.Size(576, 523)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Pathfinding"
        CType(Me.pbImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpPosition.ResumeLayout(False)
        Me.grpPathfinding.ResumeLayout(False)
        Me.grpPathfinding.PerformLayout()
        Me.grpOptions.ResumeLayout(False)
        Me.grpOptions.PerformLayout()
        CType(Me.nudGridSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudRowCount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudColumnCount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pbImage As System.Windows.Forms.PictureBox
    Friend WithEvents grpPosition As System.Windows.Forms.GroupBox
    Friend WithEvents btnStopPos As System.Windows.Forms.Button
    Friend WithEvents btnStartPos As System.Windows.Forms.Button
    Friend WithEvents btnWall As System.Windows.Forms.Button
    Friend WithEvents btnStartPathfinding As System.Windows.Forms.Button
    Friend WithEvents grpPathfinding As System.Windows.Forms.GroupBox
    Friend WithEvents cboPFStategies As System.Windows.Forms.ComboBox
    Friend WithEvents grpOptions As System.Windows.Forms.GroupBox
    Friend WithEvents ckbAnimation As System.Windows.Forms.CheckBox
    Friend WithEvents nudRowCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblRows As System.Windows.Forms.Label
    Friend WithEvents nudColumnCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblColumns As System.Windows.Forms.Label
    Friend WithEvents nudGridSize As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblGridSize As System.Windows.Forms.Label
    Friend WithEvents btnClearMap As System.Windows.Forms.Button
    Friend WithEvents ckbDebugMode As System.Windows.Forms.CheckBox
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents tsStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ckbDriveEnabled As System.Windows.Forms.CheckBox
    Friend WithEvents ckbAvoidRotation As System.Windows.Forms.CheckBox

End Class
