<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.numWL = New System.Windows.Forms.NumericUpDown()
        Me.numIntensity = New System.Windows.Forms.NumericUpDown()
        Me.numBW = New System.Windows.Forms.NumericUpDown()
        Me.grpGaussian = New System.Windows.Forms.GroupBox()
        Me.lblFWHM = New System.Windows.Forms.Label()
        Me.lblBW = New System.Windows.Forms.Label()
        Me.lblIntensity = New System.Windows.Forms.Label()
        Me.lblWL = New System.Windows.Forms.Label()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.lstSpectra = New System.Windows.Forms.ListBox()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.btnDisplay = New System.Windows.Forms.Button()
        Me.grpDisplay = New System.Windows.Forms.GroupBox()
        Me.btnOpenClose = New System.Windows.Forms.Button()
        Me.btnRemoveAll = New System.Windows.Forms.Button()
        Me.picSpectrum = New System.Windows.Forms.PictureBox()
        Me.grpMisc = New System.Windows.Forms.GroupBox()
        Me.chkCorrection = New System.Windows.Forms.CheckBox()
        Me.chkLLR = New System.Windows.Forms.CheckBox()
        Me.numColumns = New System.Windows.Forms.NumericUpDown()
        Me.lblColumns = New System.Windows.Forms.Label()
        Me.radGaussian = New System.Windows.Forms.RadioButton()
        Me.radFlat = New System.Windows.Forms.RadioButton()
        Me.chkOL490 = New System.Windows.Forms.CheckBox()
        Me.dlgOpenSpectrum = New System.Windows.Forms.OpenFileDialog()
        Me.radOpenSpectrum = New System.Windows.Forms.RadioButton()
        Me.chkScale = New System.Windows.Forms.CheckBox()
        Me.chkChangeGap = New System.Windows.Forms.CheckBox()
        Me.radOneOne = New System.Windows.Forms.RadioButton()
        CType(Me.numWL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numIntensity, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numBW, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpGaussian.SuspendLayout()
        Me.grpDisplay.SuspendLayout()
        CType(Me.picSpectrum, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpMisc.SuspendLayout()
        CType(Me.numColumns, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'numWL
        '
        Me.numWL.DecimalPlaces = 2
        Me.numWL.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numWL.Location = New System.Drawing.Point(152, 19)
        Me.numWL.Maximum = New Decimal(New Integer() {780, 0, 0, 0})
        Me.numWL.Minimum = New Decimal(New Integer() {380, 0, 0, 0})
        Me.numWL.Name = "numWL"
        Me.numWL.Size = New System.Drawing.Size(68, 20)
        Me.numWL.TabIndex = 0
        Me.numWL.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'numIntensity
        '
        Me.numIntensity.DecimalPlaces = 2
        Me.numIntensity.Location = New System.Drawing.Point(152, 45)
        Me.numIntensity.Name = "numIntensity"
        Me.numIntensity.Size = New System.Drawing.Size(68, 20)
        Me.numIntensity.TabIndex = 1
        Me.numIntensity.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'numBW
        '
        Me.numBW.DecimalPlaces = 2
        Me.numBW.Location = New System.Drawing.Point(152, 71)
        Me.numBW.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.numBW.Name = "numBW"
        Me.numBW.Size = New System.Drawing.Size(68, 20)
        Me.numBW.TabIndex = 2
        Me.numBW.Value = New Decimal(New Integer() {25, 0, 0, 0})
        '
        'grpGaussian
        '
        Me.grpGaussian.Controls.Add(Me.lblFWHM)
        Me.grpGaussian.Controls.Add(Me.lblBW)
        Me.grpGaussian.Controls.Add(Me.lblIntensity)
        Me.grpGaussian.Controls.Add(Me.lblWL)
        Me.grpGaussian.Controls.Add(Me.numWL)
        Me.grpGaussian.Controls.Add(Me.numBW)
        Me.grpGaussian.Controls.Add(Me.numIntensity)
        Me.grpGaussian.Location = New System.Drawing.Point(12, 12)
        Me.grpGaussian.Name = "grpGaussian"
        Me.grpGaussian.Size = New System.Drawing.Size(369, 100)
        Me.grpGaussian.TabIndex = 3
        Me.grpGaussian.TabStop = False
        Me.grpGaussian.Text = "Gaussians"
        '
        'lblFWHM
        '
        Me.lblFWHM.AutoSize = True
        Me.lblFWHM.Location = New System.Drawing.Point(236, 73)
        Me.lblFWHM.Name = "lblFWHM"
        Me.lblFWHM.Size = New System.Drawing.Size(88, 13)
        Me.lblFWHM.TabIndex = 7
        Me.lblFWHM.Text = "58.87 nm FWHM"
        '
        'lblBW
        '
        Me.lblBW.AutoSize = True
        Me.lblBW.Location = New System.Drawing.Point(6, 73)
        Me.lblBW.Name = "lblBW"
        Me.lblBW.Size = New System.Drawing.Size(129, 13)
        Me.lblBW.TabIndex = 6
        Me.lblBW.Text = "Peak Bandwidth (nm; SD)"
        '
        'lblIntensity
        '
        Me.lblIntensity.AutoSize = True
        Me.lblIntensity.Location = New System.Drawing.Point(6, 47)
        Me.lblIntensity.Name = "lblIntensity"
        Me.lblIntensity.Size = New System.Drawing.Size(91, 13)
        Me.lblIntensity.TabIndex = 5
        Me.lblIntensity.Text = "Peak Intensity (%)"
        '
        'lblWL
        '
        Me.lblWL.AutoSize = True
        Me.lblWL.Location = New System.Drawing.Point(6, 21)
        Me.lblWL.Name = "lblWL"
        Me.lblWL.Size = New System.Drawing.Size(116, 13)
        Me.lblWL.TabIndex = 4
        Me.lblWL.Text = "Peak Wavelength (nm)"
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(6, 19)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(85, 23)
        Me.btnAdd.TabIndex = 3
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'lstSpectra
        '
        Me.lstSpectra.FormattingEnabled = True
        Me.lstSpectra.Location = New System.Drawing.Point(6, 48)
        Me.lstSpectra.Name = "lstSpectra"
        Me.lstSpectra.Size = New System.Drawing.Size(260, 160)
        Me.lstSpectra.TabIndex = 7
        '
        'btnRemove
        '
        Me.btnRemove.Location = New System.Drawing.Point(94, 19)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(85, 23)
        Me.btnRemove.TabIndex = 8
        Me.btnRemove.Text = "Remove"
        Me.btnRemove.UseVisualStyleBackColor = True
        '
        'btnDisplay
        '
        Me.btnDisplay.Location = New System.Drawing.Point(6, 214)
        Me.btnDisplay.Name = "btnDisplay"
        Me.btnDisplay.Size = New System.Drawing.Size(75, 23)
        Me.btnDisplay.TabIndex = 9
        Me.btnDisplay.Text = "Display"
        Me.btnDisplay.UseVisualStyleBackColor = True
        '
        'grpDisplay
        '
        Me.grpDisplay.Controls.Add(Me.btnOpenClose)
        Me.grpDisplay.Controls.Add(Me.btnRemoveAll)
        Me.grpDisplay.Controls.Add(Me.btnAdd)
        Me.grpDisplay.Controls.Add(Me.btnDisplay)
        Me.grpDisplay.Controls.Add(Me.btnRemove)
        Me.grpDisplay.Controls.Add(Me.lstSpectra)
        Me.grpDisplay.Location = New System.Drawing.Point(12, 118)
        Me.grpDisplay.Name = "grpDisplay"
        Me.grpDisplay.Size = New System.Drawing.Size(284, 278)
        Me.grpDisplay.TabIndex = 10
        Me.grpDisplay.TabStop = False
        Me.grpDisplay.Text = "Display Spectra"
        '
        'btnOpenClose
        '
        Me.btnOpenClose.Location = New System.Drawing.Point(182, 214)
        Me.btnOpenClose.Name = "btnOpenClose"
        Me.btnOpenClose.Size = New System.Drawing.Size(86, 23)
        Me.btnOpenClose.TabIndex = 18
        Me.btnOpenClose.Text = "Close Shutter"
        Me.btnOpenClose.UseVisualStyleBackColor = True
        '
        'btnRemoveAll
        '
        Me.btnRemoveAll.Location = New System.Drawing.Point(183, 19)
        Me.btnRemoveAll.Name = "btnRemoveAll"
        Me.btnRemoveAll.Size = New System.Drawing.Size(85, 23)
        Me.btnRemoveAll.TabIndex = 10
        Me.btnRemoveAll.Text = "Remove All"
        Me.btnRemoveAll.UseVisualStyleBackColor = True
        '
        'picSpectrum
        '
        Me.picSpectrum.BackColor = System.Drawing.Color.Transparent
        Me.picSpectrum.Location = New System.Drawing.Point(220, 414)
        Me.picSpectrum.Name = "picSpectrum"
        Me.picSpectrum.Size = New System.Drawing.Size(600, 196)
        Me.picSpectrum.TabIndex = 12
        Me.picSpectrum.TabStop = False
        '
        'grpMisc
        '
        Me.grpMisc.Controls.Add(Me.chkCorrection)
        Me.grpMisc.Controls.Add(Me.chkLLR)
        Me.grpMisc.Controls.Add(Me.numColumns)
        Me.grpMisc.Controls.Add(Me.lblColumns)
        Me.grpMisc.Location = New System.Drawing.Point(612, 12)
        Me.grpMisc.Name = "grpMisc"
        Me.grpMisc.Size = New System.Drawing.Size(153, 100)
        Me.grpMisc.TabIndex = 13
        Me.grpMisc.TabStop = False
        Me.grpMisc.Text = "Miscellaneous"
        '
        'chkCorrection
        '
        Me.chkCorrection.AutoSize = True
        Me.chkCorrection.Enabled = False
        Me.chkCorrection.Location = New System.Drawing.Point(9, 58)
        Me.chkCorrection.Name = "chkCorrection"
        Me.chkCorrection.Size = New System.Drawing.Size(126, 17)
        Me.chkCorrection.TabIndex = 18
        Me.chkCorrection.Text = "Calibration Correction"
        Me.chkCorrection.UseVisualStyleBackColor = True
        '
        'chkLLR
        '
        Me.chkLLR.AutoSize = True
        Me.chkLLR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkLLR.Location = New System.Drawing.Point(5, 15)
        Me.chkLLR.Name = "chkLLR"
        Me.chkLLR.Size = New System.Drawing.Size(133, 17)
        Me.chkLLR.TabIndex = 17
        Me.chkLLR.Text = "Linear Light Reduction"
        Me.chkLLR.UseVisualStyleBackColor = True
        '
        'numColumns
        '
        Me.numColumns.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numColumns.Location = New System.Drawing.Point(70, 32)
        Me.numColumns.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.numColumns.Name = "numColumns"
        Me.numColumns.Size = New System.Drawing.Size(68, 20)
        Me.numColumns.TabIndex = 8
        Me.numColumns.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'lblColumns
        '
        Me.lblColumns.AutoSize = True
        Me.lblColumns.Location = New System.Drawing.Point(6, 35)
        Me.lblColumns.Name = "lblColumns"
        Me.lblColumns.Size = New System.Drawing.Size(47, 13)
        Me.lblColumns.TabIndex = 1
        Me.lblColumns.Text = "Columns"
        '
        'radGaussian
        '
        Me.radGaussian.AutoSize = True
        Me.radGaussian.Checked = True
        Me.radGaussian.Location = New System.Drawing.Point(387, 23)
        Me.radGaussian.Name = "radGaussian"
        Me.radGaussian.Size = New System.Drawing.Size(69, 17)
        Me.radGaussian.TabIndex = 14
        Me.radGaussian.TabStop = True
        Me.radGaussian.Text = "Gaussian"
        Me.radGaussian.UseVisualStyleBackColor = True
        '
        'radFlat
        '
        Me.radFlat.AutoSize = True
        Me.radFlat.Location = New System.Drawing.Point(387, 46)
        Me.radFlat.Name = "radFlat"
        Me.radFlat.Size = New System.Drawing.Size(70, 17)
        Me.radFlat.TabIndex = 15
        Me.radFlat.Text = "Flat Peak"
        Me.radFlat.UseVisualStyleBackColor = True
        '
        'chkOL490
        '
        Me.chkOL490.AutoSize = True
        Me.chkOL490.Checked = True
        Me.chkOL490.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkOL490.Location = New System.Drawing.Point(771, 24)
        Me.chkOL490.Name = "chkOL490"
        Me.chkOL490.Size = New System.Drawing.Size(97, 17)
        Me.chkOL490.TabIndex = 7
        Me.chkOL490.Text = "OL490 Present"
        Me.chkOL490.UseVisualStyleBackColor = True
        '
        'dlgOpenSpectrum
        '
        Me.dlgOpenSpectrum.DefaultExt = "csv"
        Me.dlgOpenSpectrum.InitialDirectory = "curdir()"
        '
        'radOpenSpectrum
        '
        Me.radOpenSpectrum.AutoSize = True
        Me.radOpenSpectrum.Location = New System.Drawing.Point(387, 69)
        Me.radOpenSpectrum.Name = "radOpenSpectrum"
        Me.radOpenSpectrum.Size = New System.Drawing.Size(99, 17)
        Me.radOpenSpectrum.TabIndex = 16
        Me.radOpenSpectrum.Text = "Open Spectrum"
        Me.radOpenSpectrum.UseVisualStyleBackColor = True
        '
        'chkScale
        '
        Me.chkScale.AutoSize = True
        Me.chkScale.Location = New System.Drawing.Point(302, 137)
        Me.chkScale.Name = "chkScale"
        Me.chkScale.Size = New System.Drawing.Size(61, 17)
        Me.chkScale.TabIndex = 17
        Me.chkScale.Text = "Scaling"
        Me.chkScale.UseVisualStyleBackColor = True
        '
        'chkChangeGap
        '
        Me.chkChangeGap.AutoSize = True
        Me.chkChangeGap.Location = New System.Drawing.Point(501, 23)
        Me.chkChangeGap.Name = "chkChangeGap"
        Me.chkChangeGap.Size = New System.Drawing.Size(56, 17)
        Me.chkChangeGap.TabIndex = 18
        Me.chkChangeGap.Text = "4 => 5"
        Me.chkChangeGap.UseVisualStyleBackColor = True
        '
        'radOneOne
        '
        Me.radOneOne.AutoSize = True
        Me.radOneOne.Location = New System.Drawing.Point(387, 92)
        Me.radOneOne.Name = "radOneOne"
        Me.radOneOne.Size = New System.Drawing.Size(69, 17)
        Me.radOneOne.TabIndex = 19
        Me.radOneOne.Text = "Open 1:1"
        Me.radOneOne.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(869, 647)
        Me.Controls.Add(Me.radOneOne)
        Me.Controls.Add(Me.chkChangeGap)
        Me.Controls.Add(Me.chkScale)
        Me.Controls.Add(Me.radOpenSpectrum)
        Me.Controls.Add(Me.chkOL490)
        Me.Controls.Add(Me.radFlat)
        Me.Controls.Add(Me.radGaussian)
        Me.Controls.Add(Me.grpMisc)
        Me.Controls.Add(Me.picSpectrum)
        Me.Controls.Add(Me.grpDisplay)
        Me.Controls.Add(Me.grpGaussian)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.numWL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numIntensity, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numBW, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpGaussian.ResumeLayout(False)
        Me.grpGaussian.PerformLayout()
        Me.grpDisplay.ResumeLayout(False)
        CType(Me.picSpectrum, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpMisc.ResumeLayout(False)
        Me.grpMisc.PerformLayout()
        CType(Me.numColumns, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents numWL As System.Windows.Forms.NumericUpDown
    Friend WithEvents numIntensity As System.Windows.Forms.NumericUpDown
    Friend WithEvents numBW As System.Windows.Forms.NumericUpDown
    Friend WithEvents grpGaussian As System.Windows.Forms.GroupBox
    Friend WithEvents lblBW As System.Windows.Forms.Label
    Friend WithEvents lblIntensity As System.Windows.Forms.Label
    Friend WithEvents lblWL As System.Windows.Forms.Label
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents lstSpectra As System.Windows.Forms.ListBox
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents btnDisplay As System.Windows.Forms.Button
    Friend WithEvents grpDisplay As System.Windows.Forms.GroupBox
    Friend WithEvents picSpectrum As System.Windows.Forms.PictureBox
    Friend WithEvents grpMisc As System.Windows.Forms.GroupBox
    Friend WithEvents radGaussian As System.Windows.Forms.RadioButton
    Friend WithEvents radFlat As System.Windows.Forms.RadioButton
    Friend WithEvents chkOL490 As System.Windows.Forms.CheckBox
    Friend WithEvents dlgOpenSpectrum As System.Windows.Forms.OpenFileDialog
    Friend WithEvents radOpenSpectrum As System.Windows.Forms.RadioButton
    Friend WithEvents btnRemoveAll As System.Windows.Forms.Button
    Friend WithEvents chkLLR As System.Windows.Forms.CheckBox
    Friend WithEvents numColumns As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblColumns As System.Windows.Forms.Label
    Friend WithEvents chkScale As System.Windows.Forms.CheckBox
    Friend WithEvents chkCorrection As System.Windows.Forms.CheckBox
    Friend WithEvents btnOpenClose As System.Windows.Forms.Button
    Friend WithEvents chkChangeGap As System.Windows.Forms.CheckBox
    Friend WithEvents lblFWHM As System.Windows.Forms.Label
    Friend WithEvents radOneOne As RadioButton
End Class
