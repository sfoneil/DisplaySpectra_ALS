Public Class Form1
    Private Declare Function GetAsyncKeyState Lib "user32.dll" (ByVal keyCode As Integer) As Short      'Forget. Something with button press, obsolete?
    Dim OL490 As New OL490_SDK_Dll.OL490SdkLibrary          'Add from .dll, name defined as "OL490"
    'Starts with a structure, ultimately dblSpectrum as 2x401
    Structure Spectrum_Info
        Dim WL As Single
        Dim pct As Single
    End Structure

    'Global variables, region of poor programming design
    Dim Spectrum(400) As Data_Point
    Dim Calibrate As Calibration_Set = Open_Correction(CurDir() & "\Correction.cal")
    'Correction is generated with Kyle's program that reads in PR-650 measurements at 100,75,50,25% intensity full white
    Dim ConeData As Cone_Data = Open_Cone_Data(CurDir() & "\Cone Fundamentals.txt")
    'Cone data may be necessary. Text file should be accessible. Shouldn't need to change it. Not sure where Kyle got the numbers, don't
    'quite match up with data on www.cvrl.org but similar idea.
    Dim MySpectrum As Spectrum_Data
    Dim DblValue As DblValues
    Dim dblSpectrum(1, 400) As Double       '2 columns, 401 rows
    Dim PlotSpectrum, PlotCIE As Plot_Info  'Load in structure from Plot Module.vb
    Dim strLastInput As String              'For Input Spectrum. Saves filename

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        End
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Randomize()
        Initialize_Plots() : Update_Plots()
        lstSpectra.Items.Clear()
        lstSpectra.Items.Add("ALL")
        'Set default file types and directory for OpenSpectrum
        dlgOpenSpectrum.Filter = "Comma-separated files (*.csv)|*.csv|Text files (*.txt)|*.txt|Excel Files (*.xl*;*.xlsx;*.xls)|*.xl*;*.xlsx;*.xls|All files (*.*)|*.*"
        dlgOpenSpectrum.InitialDirectory = CurDir()
        'If GetAsyncKeyState(Keys.Q) Then
        '    btnDisplay.PerformClick()
        'End If
        'chkOL490.Checked = False    '\@
        'Try
        '    'some sort of initialization
        'Catch ex As Exception
        '    MsgBox("AAA")
        '    chkOL490.Checked = False            'Uncheck if can't connect
        'End Try
        If chkOL490.Checked = True Then OL490Connect()
        'Can comment this out to avoid closing multiple error messages upon start. Default is ON for now because it is more annoying to 
        'constantly click CONNECT on each start.
    End Sub

    Private Sub OL490Connect()
        With OL490
            Try
                'To connect and intialize, a lot of this stuff needs to be run ROUGHLY in this order.
                'Some might be obsolete/unnecessary, but it still works!
                ErrorCheck(.ConnectToOL490(0))
                ErrorCheck(.SetCurrentOL490(0))
                .ResetSequence()
                If .GetNumberOfStoredCalibrations() = -1 Then MsgBox("Error - Not connected") 'No calibration file
                ErrorCheck(.LoadAndUseStoredCalibration(1))     'Second calibration, for 350 micro
                ErrorCheck(.SetGrayScaleValue(0))               'Monochrome
                'ErrorCheck(.SetTriggerMode(0))
                ErrorCheck(.EnableLinearLightReduction(100))    'Linearizes light and reduces intensity. 100 = ?
                ErrorCheck(.SendLivePeak(650, 4, 50))           'to prevent errors. Bug?
                chkOL490.Checked = True
            Catch ex As Exception
                'StatusConnected.Text = "OL490 not detected"
                chkOL490.Checked = False
                MsgBox("OL490 not detected. Connection will not occur")
                Exit Sub
            End Try
        End With
        ShowSpectrum()
    End Sub

#Region "Stimuli"
    'Size is default (1,400), total length 802
    '(0,x) = intensity at wavelength...
    '(1,x) = corresponding wavelength, 380 to 780, steps of 1
    'Gaussian and Flat run when displayed, input spectrum is generated earlier, when chosen
    Private Function BuildGaussian(ByVal wavelength As Single, ByVal intensity As Single, ByVal bw As Single) As Double(,)
        'Build Gaussian spectrum given 3 inputs
        Dim tempspectra() As Spectrum_Info
        Dim dblTempSpectrum(1, 400) As Double
        ReDim BuildGaussian(1, 400)
        For i = 0 To 400
            ReDim Preserve tempspectra(i)
            tempspectra(i).WL = i + 380
            tempspectra(i).pct = intensity * Math.Exp(-(i + 380 - wavelength) ^ 2 / (2 * bw ^ 2))
            'If ChkCorrection.Checked = True Then Spectrum(i) = Adjust_Point(Calibrate, Spectrum(i)) 'Calibration routine
            dblTempSpectrum(0, i) = tempspectra(i).pct
            dblTempSpectrum(1, i) = tempspectra(i).WL
        Next
        BuildGaussian = dblTempSpectrum
    End Function

    Private Function BuildFlat(ByVal wavelength As Single, ByVal intensity As Single, ByVal bw As Single) As Double(,)
        'Build flat peak similar to Gaussian; bw is bandwidth range of peak
        Dim tempspectra() As Spectrum_Info
        Dim dblTempSpectrum(1, 400) As Double
        ReDim BuildFlat(1, 400)

        'For now: Assumed that width is an ODD number
        Dim PlusMinus As Single = (bw - 1) / 2
        For i = 0 To 400
            ReDim Preserve tempspectra(i)
            tempspectra(i).WL = i + 380
            If (tempspectra(i).WL < wavelength - PlusMinus) Or (tempspectra(i).WL > wavelength + PlusMinus) Then
                tempspectra(i).pct = 0
            Else
                tempspectra(i).pct = intensity
            End If

            'If ChkCorrection.Checked = True Then Spectrum(i) = Adjust_Point(Calibrate, Spectrum(i)) 'Calibration routine
            dblTempSpectrum(0, i) = tempspectra(i).pct
            dblTempSpectrum(1, i) = tempspectra(i).WL
        Next
        BuildFlat = dblTempSpectrum
    End Function

    Private Function BuildOneOne(ByVal strFileName As String) As Double(,)
        'Dim flattenedSpectrum(201) As String
        Dim flattenedSpectrum(801) As String
        Dim dblTempSpectrum(1, 400) As Double
        Dim c As Integer
        Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(strFileName)
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters(",")
            Dim currentRow As String()
            While Not MyReader.EndOfData
                Try
                    currentRow = MyReader.ReadFields()
                    Dim currentField As String
                    For Each currentField In currentRow
                        flattenedSpectrum(c) = CStr(currentField)
                        c += 1
                    Next
                Catch ex As _
                        Microsoft.VisualBasic.FileIO.MalformedLineException
                    MsgBox("Line " & ex.Message &
                    "is not valid and will be skipped.")
                End Try
            End While
        End Using
        For i = 0 To 400
            dblTempSpectrum(1, i) = flattenedSpectrum(i * 2)
        Next
        For i = 0 To 400
            dblTempSpectrum(0, i) = flattenedSpectrum(i * 2 + 1)
        Next

        BuildOneOne = dblTempSpectrum
    End Function

    Private Function BuildInputSpectrum(ByVal strFileName As String) As Double(,)
        'Reads in a spectrum from a file. Format is:
        'CSV (comma separated) file with COLUMN A = wavelengths and COLUMN B = intensity at that wavelength.
        'By default the wavelength is in steps of 4 (380,384,388) per PR650 output.
        'Checking chkChangeGap makes it in steps of 5 (380,385,390)...
        Dim flattenedSpectrum(201) As String
        Dim dblTempSpectrum(1, 400) As Double
        ReDim BuildInputSpectrum(1, 400)
        Dim c As Integer
        'Read in file into array
        Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(strFileName)
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters(",")
            Dim currentRow As String()
            While Not MyReader.EndOfData
                Try
                    currentRow = MyReader.ReadFields()
                    Dim currentField As String
                    For Each currentField In currentRow
                        flattenedSpectrum(c) = CStr(currentField)
                        c += 1
                    Next
                Catch ex As  _
                        Microsoft.VisualBasic.FileIO.MalformedLineException
                    MsgBox("Line " & ex.Message & _
                    "is not valid and will be skipped.")
                End Try
            End While
        End Using
        'Convert to 2d and add intermediate wavelengths due to PR-65x input being in steps of 4
        If chkChangeGap.Checked = True Then
            ReDim flattenedSpectrum(149)
            For i = 0 To 396 Step 5
                'Add wavelength column
                Dim j = i / 2       '0=0;4=2;8=4
                dblTempSpectrum(1, i) = flattenedSpectrum(j)
                dblTempSpectrum(1, i + 1) = flattenedSpectrum(j) + 1
                dblTempSpectrum(1, i + 2) = flattenedSpectrum(j) + 2
                dblTempSpectrum(1, i + 3) = flattenedSpectrum(j) + 3
                dblTempSpectrum(1, i + 4) = flattenedSpectrum(j) + 4

                'Add intensity column
                j = i / 2 + 1
                dblTempSpectrum(0, i) = flattenedSpectrum(j)
                dblTempSpectrum(0, i + 1) = flattenedSpectrum(j)
                dblTempSpectrum(0, i + 2) = flattenedSpectrum(j)
                dblTempSpectrum(0, i + 3) = flattenedSpectrum(j)
                dblTempSpectrum(0, i + 4) = flattenedSpectrum(j)
            Next
            dblTempSpectrum(1, 400) = 780       'Need to add last one manually because /5
            dblTempSpectrum(0, 400) = flattenedSpectrum(flattenedSpectrum.GetUpperBound(0))
            BuildInputSpectrum = dblTempSpectrum
        ElseIf chkChangeGap.Checked = False Then
            For i = 0 To 397 Step 4
                'Add wavelength column
                Dim j = i / 2       '0=0;4=2;8=4
                dblTempSpectrum(1, i) = flattenedSpectrum(j)
                dblTempSpectrum(1, i + 1) = flattenedSpectrum(j) + 1
                dblTempSpectrum(1, i + 2) = flattenedSpectrum(j) + 2
                dblTempSpectrum(1, i + 3) = flattenedSpectrum(j) + 3

                'Add intensity column
                j = i / 2 + 1
                dblTempSpectrum(0, i) = flattenedSpectrum(j)
                dblTempSpectrum(0, i + 1) = flattenedSpectrum(j)
                dblTempSpectrum(0, i + 2) = flattenedSpectrum(j)
                dblTempSpectrum(0, i + 3) = flattenedSpectrum(j)
            Next
            dblTempSpectrum(1, 400) = 780       'Need to add last one manually because /4
            dblTempSpectrum(0, 400) = flattenedSpectrum(flattenedSpectrum.GetUpperBound(0))
            BuildInputSpectrum = dblTempSpectrum
        End If
    End Function
#End Region

#Region "lstSpectra Functions"
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        'Take information from number boxes and add to list
        Dim strPrefix As String = ""
        If radGaussian.Checked = True Then
            strPrefix = "G"
        ElseIf radFlat.Checked = True Then
            strPrefix = "F"
        ElseIf radOpenSpectrum.Checked = True Then
            strPrefix = "I"
        ElseIf radOneOne.Checked = True Then
            strPrefix = "O"
        End If
        If strPrefix = "I" Or strPrefix = "O" Then
            'Add items to list in Input Spectrum format
            lstSpectra.Items.Add(strPrefix & vbTab & dlgOpenSpectrum.FileName.Substring(dlgOpenSpectrum.FileName.LastIndexOf("\") + 1))
        Else
            'Add items to list in format: Prefix + tab + wavelength + tab + intensity + bandwidth
            lstSpectra.Items.Add(strPrefix & vbTab & Format(numWL.Value, "F") & vbTab & Format(numIntensity.Value, "F") & vbTab & Format(numBW.Value, "F"))
        End If
    End Sub

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        'Remove selected spectrum from list
        Dim intRemIndex As Integer
        intRemIndex = lstSpectra.SelectedIndex
        If intRemIndex > 0 Then lstSpectra.Items.RemoveAt(intRemIndex) 'Do not remove "ALL"
    End Sub

    Private Sub btnRemoveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveAll.Click
        'Clear list of all items
        lstSpectra.Items.Clear()
        lstSpectra.Items.Add("ALL")
        ReDim dblSpectrum(1, 400)       'Clear current stored spectrum
    End Sub

    Private Sub ShowSpectrum()
        'Calls below DrawSpectrum, decides which part to run
        Dim strType, strList As String
        Dim sngWL, sngInt, sngBW As Single

        Select Case lstSpectra.SelectedIndex
            Case 0          'Is "ALL" spectra
                DrawSpectrum()
            Case 1 To lstSpectra.Items.Count - 1       'Anything else
                strList = lstSpectra.SelectedItem.ToString
                strType = strList.Substring(0, strList.IndexOf(vbTab))
                strList = strList.Substring(strList.IndexOf(vbTab) + 1)
                If strType <> "I" And strType <> "O" Then
                    'Read in WL, intensity, BW from list based on tab stops
                    sngWL = strList.Substring(0, strList.IndexOf(vbTab))
                    sngInt = strList.Substring(strList.IndexOf(vbTab) + 1, strList.LastIndexOf(vbTab) - strList.IndexOf(vbTab) - 1)
                    sngBW = strList.Substring(strList.LastIndexOf(vbTab) + 1)
                End If
                DrawSpectrum(sngWL, sngInt, sngBW)
            Case -1    'Item not selected
                Beep()
        End Select
    End Sub

    Private Sub DrawSpectrum(Optional ByVal wavelength As Single = 500, Optional ByVal intensity As Single = 100, Optional ByVal bw As Single = 25)
        'Draw spectrum based on inputs. Generally loops through equation to build.
        'Checks for saturation, scales if box is checked, plots on graph, displays to OL490
        Dim dblTemp(1, 400) As Double
        Dim strType, strList As String
        Dim sngWL, sngInt, sngBW As Single

        For i = 0 To 400
            dblSpectrum(0, i) = 0
        Next

        Select Case lstSpectra.SelectedIndex
            Case 0
                For i = 1 To lstSpectra.Items.Count - 1
                    lstSpectra.SelectedIndex = i
                    strList = lstSpectra.Items.Item(lstSpectra.SelectedIndex)
                    strType = strList.Substring(0, strList.IndexOf(vbTab))
                    strList = strList.Substring(strList.IndexOf(vbTab) + 1)
                    If strType <> "I" And strType <> "O" Then
                        sngWL = strList.Substring(0, strList.IndexOf(vbTab))
                        sngInt = strList.Substring(strList.IndexOf(vbTab) + 1, strList.LastIndexOf(vbTab) - strList.IndexOf(vbTab) - 1)
                        sngBW = strList.Substring(strList.LastIndexOf(vbTab) + 1)
                    End If
                    With lstSpectra.SelectedItem.ToString
                        If .Substring(0, .IndexOf(vbTab)) = "G" Then
                            dblTemp = BuildGaussian(sngWL, sngInt, sngBW)
                        ElseIf .Substring(0, .IndexOf(vbTab)) = "F" Then
                            dblTemp = BuildFlat(sngWL, sngInt, sngBW)
                        ElseIf .Substring(0, .IndexOf(vbTab)) = "I" Then
                            dblTemp = BuildInputSpectrum(strLastInput)
                        ElseIf .Substring(0, .IndexOf(vbTab)) = "O" Then
                            dblTemp = BuildOneOne(strLastInput)
                        End If
                    End With

                    If chkScale.Checked = True Then
                        For j = 0 To dblTemp.GetUpperBound(1)       'get max
                            dblSpectrum(0, j) += dblTemp(0, j)
                        Next
                    Else
                        For j = 0 To dblTemp.GetUpperBound(1)
                            dblSpectrum(0, j) += dblTemp(0, j)
                            If dblSpectrum(0, j) > 100 Then dblSpectrum(0, j) = 100
                            If dblSpectrum(0, j) < 0 Then dblSpectrum(0, j) = 100
                        Next
                    End If

                    'dblTemp = Nothing
                    lstSpectra.SelectedIndex = 0
                Next
                For i = 0 To 400
                    dblSpectrum(1, i) = i + 380
                Next
                If chkScale.Checked = True Then
                    Dim sngMax As Single = 0
                    For j = 0 To dblTemp.GetUpperBound(1)       'get max
                        'dblSpectrum(0, j) += dblTemp(0, j)
                        If dblSpectrum(0, j) > sngMax Then sngMax = dblSpectrum(0, j)
                    Next
                    'MsgBox(sngMax)
                    For j = 0 To dblTemp.GetUpperBound(1)       'Rescale
                        dblSpectrum(0, j) = dblSpectrum(0, j) / sngMax * 100
                    Next
                    Beep()
                Else
                    'For j = 0 To dblTemp.GetUpperBound(1)
                    '    dblSpectrum(0, j) += dblTemp(0, j)
                    '    If dblSpectrum(0, j) > 100 Then dblSpectrum(0, j) = 100
                    '    If dblSpectrum(0, j) < 0 Then dblSpectrum(0, j) = 100
                    'Next
                End If

                With PlotSpectrum
                    For IntIntensity = 0 To .XYData.GetUpperBound(0)
                        For IntWavelength = 0 To .XYData.GetUpperBound(1)
                            '.XYData(IntIntensity, IntWavelength).X = 380 + IntWavelength '4 * IntWavelength
                            .XYData(IntIntensity, IntWavelength).Y = dblSpectrum(0, IntWavelength) '4 * IntWavelength
                        Next
                    Next
                End With
                Update_Plots()          'Needs to be above OL490 commands?
                If chkOL490.Checked = True Then
                    DisplaySpectrumOL490()
                End If

            Case 1 To lstSpectra.Items.Count - 1
                With lstSpectra.SelectedItem.ToString
                    If .Substring(0, .IndexOf(vbTab)) = "G" Then
                        dblSpectrum = BuildGaussian(wavelength, intensity, bw)
                    ElseIf .Substring(0, .IndexOf(vbTab)) = "F" Then
                        dblSpectrum = BuildFlat(wavelength, intensity, bw)
                    ElseIf .Substring(0, .IndexOf(vbTab)) = "I" Then
                        dblSpectrum = BuildInputSpectrum(strLastInput)
                    ElseIf .Substring(0, .IndexOf(vbTab)) = "O" Then
                        dblSpectrum = BuildOneOne(strLastInput)
                    End If
                End With
                For i = 0 To 400
                    dblSpectrum(1, i) = i + 380
                Next
                With PlotSpectrum
                    For IntIntensity = 0 To .XYData.GetUpperBound(0)
                        For IntWavelength = 0 To .XYData.GetUpperBound(1)
                            '.XYData(IntIntensity, IntWavelength).X = 380 + IntWavelength '4 * IntWavelength
                            .XYData(IntIntensity, IntWavelength).Y = dblSpectrum(0, IntWavelength) '4 * IntWavelength
                        Next
                    Next
                End With
                Update_Plots()          'Needs to be above OL490 commands?
                If chkOL490.Checked = True Then
                    DisplaySpectrumOL490()
                End If
            Case -1
                Beep()
        End Select
    End Sub

    Private Sub lstSpectra_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstSpectra.Click
        'Choose spectra that were added to list. Specific string format interprets the type:
        'G = Gaussian spectrum. Specify 1) wavelength at peak, 2) intensity at peak, 3) width of Gaussian
        'F = Flat spectrum
        'I = Input spectrum from .csv file
        'Input into OL490 takes SD instead, so FWHM = ~2.355*SD or 1FWHM = 2*sqrt(2*ln(2)) or 2 * Math.Sqrt(2 * Math.Log(2))
        Dim sngWL, sngInt, sngBW As Single
        Dim strList, strType As String
        strList = ""
        strList = lstSpectra.SelectedItem.ToString          'TODO: Fix so clicking nothing doesn't crash
        If strList <> "ALL" Then
            strType = strList.Substring(0, strList.IndexOf(vbTab))
            strList = strList.Substring(strList.IndexOf(vbTab) + 1)
            If (strType <> "I") And (strType <> "O") Then

                sngWL = strList.Substring(0, strList.IndexOf(vbTab))
                sngInt = strList.Substring(strList.IndexOf(vbTab) + 1, strList.LastIndexOf(vbTab) - strList.IndexOf(vbTab) - 1)
                sngBW = strList.Substring(strList.LastIndexOf(vbTab) + 1)

                Select Case strType
                    Case "G"
                        radGaussian.Checked = True
                    Case "F"
                        radFlat.Checked = True
                    Case "I"
                        radOpenSpectrum.Checked = True
                    Case "O"
                        radOneOne.Checked = True
                End Select
                numWL.Value = sngWL
                numIntensity.Value = sngInt
                numBW.Value = sngBW
            End If
        End If
    End Sub

    Private Sub lstSpectra_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstSpectra.SelectedIndexChanged
        'Does nothing currently. Selected index determines what stimulus to display
        'Index = 0: show all indices 1 to length - 1
        'Index = 1 to length - 1: show that index
        'Index = -1: error state
    End Sub
#End Region

#Region "PLOTS"
    'Plot spectra into form. Does not rely on OL490 operation, and can be run independently
    Private Sub Initialize_Plots()
        With PlotSpectrum
            .Width = picSpectrum.Width : .Height = picSpectrum.Height
            .YTitle = "Spectrum"
        End With
        Initialize_Spectrum_Plot(PlotSpectrum)
        'With PlotCIE
        '    .Width = PicCIE.Width : .Height = PicCIE.Height
        '    .ShowAxisX = True : .ShowAxisY = True
        '    .XTitle = "x" : .YTitle = "y"
        '    .XMin = 0 : .XMax = 0.85
        '    .YMin = 0 : .YMax = 0.85
        '    ReDim .XYData(0, 3 * IntIntensitiesPerGun * IntReadingsPerDatum - 1)
        '    .ColorBackground = Me.BackColor : .ColorForeground = Me.ForeColor
        '    .BackgroundImagePlacement.X = 0 : .BackgroundImagePlacement.Y = 1
        '    .BackgroundImagePlacement.Width = 1
        '    .BackgroundImagePlacement.Height = 1
        'End With
    End Sub

    Private Sub Draw_CIE_Background()
        'With PlotCIE
        '    .BackgroundImage = New Bitmap(500, 500, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        '    Dim GrphCIE As Graphics = Graphics.FromImage(.BackgroundImage)
        '    Dim PntFCIE As PointF, PntCIE() As Point, IntDatum As Integer
        '    FileOpen(1, StrExeDir & "\Spectrum Locus.txt", OpenMode.Input)
        '    While EOF(1) = False
        '        ReDim Preserve PntCIE(IntDatum)
        '        Input(1, PntFCIE.X) : Input(1, PntFCIE.Y)
        '        PntCIE(IntDatum).X = PntFCIE.X * .BackgroundImage.Width
        '        PntCIE(IntDatum).Y = .BackgroundImage.Height - PntFCIE.Y * .BackgroundImage.Height
        '        IntDatum += 1
        '    End While
        '    FileClose(1)
        '    Dim ColBackground As Color = .ColorBackground
        '    Dim IntR As Integer = ColBackground.R
        '    Dim IntG As Integer = ColBackground.G
        '    Dim IntB As Integer = ColBackground.B
        '    Dim IntRGB As Integer = (IntR + IntG + IntB) / 3
        '    If IntRGB < 128 Then
        '        ColBackground = Color.FromArgb(ColBackground.R + 50, ColBackground.G + 50, ColBackground.B + 50)
        '    Else
        '        ColBackground = Color.FromArgb(ColBackground.R - 50, ColBackground.G - 50, ColBackground.B - 50)
        '    End If
        '    GrphCIE.Clear(ColBackground)
        '    Dim BrshFill As New SolidBrush(.ColorBackground)
        '    GrphCIE.FillClosedCurve(BrshFill, PntCIE)
        'End With
    End Sub

    Private Sub Initialize_Spectrum_Plot(ByRef PlotInfo As Plot_Info)
        With PlotInfo
            .ShowAxisX = True : .ShowAxisY = True
            .XTitle = "Wavelength (nm)"
            '.YTitle &= " Rad. (w/sr/m2)"
            .YTitle = " Intensity (%)"
            .XMin = 380 : .XMax = 780
            .YMin = 0 : .YMax = 110
            '.AutoScaleY = True
            ReDim .XYData(0, 400)
            Dim IntIntensity, IntWavelength As Integer
            For IntIntensity = 0 To .XYData.GetUpperBound(0)
                For IntWavelength = 0 To .XYData.GetUpperBound(1)
                    .XYData(IntIntensity, IntWavelength).X = 380 + IntWavelength '4 * IntWavelength
                Next
            Next
            .ConnectPoints = True
            .ColorBackground = Me.BackColor : .ColorForeground = Me.ForeColor
            .BackgroundImage = Image.FromFile(CurDir() & "\Spectrum.bmp")
            .BackgroundImagePlacement.X = .XMin : .BackgroundImagePlacement.Y = .YMax
            .BackgroundImagePlacement.Width = .XMax - .XMin
            .BackgroundImagePlacement.Height = .YMax - .YMin
        End With
    End Sub

    Private Sub Update_Plots()
        'Draw_CIE_Background()
        picSpectrum.Image = Plot(PlotSpectrum) : picSpectrum.Refresh()
        Beep()
        'PicCIE.Image = Plot(PlotCIE) : PicCIE.Refresh()
    End Sub
#End Region

    Public Sub ErrorCheck(ByVal eError As OL490_SDK_Dll.eErrorCodes)
        'Checks for error in OL490 access
        'Use to call other OL490 functions and to reduce errors

        'If eError = OL490_SDK_Dll.eErrorCodes.Success Or eError = OL490_SDK_Dll.eErrorCodes.NoAction Then
        '    Beep()
        'Else : MsgBox(eError & " - " & eError.ToString)
        'End If
        If eError <> OL490_SDK_Dll.eErrorCodes.Success And eError <> OL490_SDK_Dll.eErrorCodes.NoAction Then MsgBox(eError & " - " & eError.ToString)
    End Sub


    Private Sub btnDisplay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDisplay.Click
        'btnDisplay --> ShowSpectrum --> DrawSpectrum
        ShowSpectrum()
        'Clear out stored spectrum when done
        For i = 0 To 400
            dblSpectrum(0, i) = 0
            dblSpectrum(1, i) = 0
        Next
    End Sub

    Private Sub DisplaySpectrumOL490()
        'These commands in this order at least works. Changes in SDK might've made parts less necessary
        If chkOL490.Checked = True Then
            Try
                With OL490
                    'Makes sure OL490 is in runnable state, displays
                    ErrorCheck(.Stop)                       'Stop any previous running
                    .ResetSequence()                        'Clear buffer
                    System.Threading.Thread.Sleep(200)      'Prevents stimulus desynchronization by pausing 1/5 second
                    ErrorCheck(.OpenShutter())              'Open shutter to show light
                    ErrorCheck(.AddScanDataSpectrumToSequence(dblSpectrum, 401))    'Store user-generated dblSpectrum to buffer
                    ErrorCheck(.SetFramesPerSecond(15))
                    ErrorCheck(.SetRamLooping(500))
                    ErrorCheck(.SendToRam())                                        'Store in memory
                    ErrorCheck(.Start)                                              'Start runnning
                End With
            Catch ex As Exception
                'MsgBox("Stimulus cannot be displayed with OL490 disconnected")
            End Try
        End If
    End Sub

#Region "Radio Buttons"
    Sub radGaussian_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radGaussian.CheckedChanged
        'This (and next 2 subs): changes allowable options based on which radio button is selected.
        'Enables Gaussian options, disables others
        If radGaussian.Checked = True Then
            numWL.Enabled = True
            numIntensity.Enabled = True
            numBW.Enabled = True
            grpGaussian.Text = "Gaussians"
            lblBW.Text = "Peak Bandwidth (nm; SD)"
        End If
    End Sub

    Private Sub radFlat_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radFlat.CheckedChanged
        'Enables Flat options, disables others
        If radFlat.Checked = True Then
            numWL.Enabled = True
            numIntensity.Enabled = True
            numBW.Enabled = True
            grpGaussian.Text = "Flat"
            lblBW.Text = "Width (nm)"
        End If
    End Sub

    Private Sub radOpenSpectrum_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radOpenSpectrum.Click ', radOpenSpectrum.CheckedChanged
        'When radOpenSpectrum is checked (changed), open file dialog
        If radOpenSpectrum.Checked = True Then
            numWL.Enabled = False
            numIntensity.Enabled = False
            numBW.Enabled = False
            grpGaussian.Text = "Input Spectrum"
            If dlgOpenSpectrum.ShowDialog = Windows.Forms.DialogResult.OK Then
                strLastInput = dlgOpenSpectrum.FileName
                BuildInputSpectrum(strLastInput)
            End If
        End If
    End Sub

    Private Sub radOneOne_CheckedChanged(sender As Object, e As EventArgs) Handles radOneOne.CheckedChanged
        If radOneOne.Checked = True Then
            numWL.Enabled = False
            numIntensity.Enabled = False
            numBW.Enabled = False
            grpGaussian.Text = "Input Spectrum"
            If dlgOpenSpectrum.ShowDialog = Windows.Forms.DialogResult.OK Then
                strLastInput = dlgOpenSpectrum.FileName
                BuildOneOne(strLastInput)
            End If
        End If
    End Sub
#End Region

    Private Sub chkScale_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkScale.CheckedChanged
        'This makes max intensity = 100 to prevent oversaturation, especially when multiple spectra overlap.
        'Currently nothing happens until display is changed, so this part does nothing.
    End Sub

    Private Sub btnOpenClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenClose.Click
        'Toggle shutter state
        If btnOpenClose.Text = "Close Shutter" Then
            OL490.CloseShutter()
            btnOpenClose.Text = "Open Shutter"
        ElseIf btnOpenClose.Text = "Open Shutter" Then
            OL490.OpenShutter()
            btnOpenClose.Text = "Close Shutter"
        Else
            MsgBox("btnOpenClose in unuseable state")
        End If
    End Sub

    Private Sub numBW_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles numBW.ValueChanged
        'Calculate FWHM (full width at half maximum) value corresponding to inputted standard deviation.
        '1 SD = 68% of center
        '1 FWHM = width across peak wavelength, spanning from 2 symmetrical points where intensity = 50% of max
        'Half WHM = from center to either side
        Dim FWHM = Math.Round(numBW.Value * 2 * Math.Sqrt(2 * Math.Log(2)), 2)
        lblFWHM.Text = String.Format("{0} nm FWMH", FWHM)       'FWHM replaces {0}
    End Sub



    Private Sub chkLLR_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkLLR.CheckedChanged
        'Change Linear Light Reduction value when check state is changed. Must display again to reset lighting level.
        ErrorCheck(OL490.EnableLinearLightReduction(numColumns.Value))
    End Sub
End Class

