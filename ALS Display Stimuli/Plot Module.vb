Module Plot_Module

    'Version 2.0 (Updated 2010-07-29)

    Structure Plot_Info
        Dim Width, Height As Integer
        Dim AutoScaleX, AutoScaleY, LogScaleX, LogScaleY, ShowAxisX, ShowAxisY, ConnectPoints As Boolean
        Dim XMin, XMax, YMin, YMax As Single
        Dim YData(,), XLabels() As Double 'Data arrays are (series, datum)
        Dim XYData(,) As PointF 'Data arrays are (series, datum)
        Dim XLabelStrings(), XTitle, YTitle As String
        Dim ColorBackground, ColorForeground As Color
        Dim BackgroundImage As Bitmap, BackgroundImagePlacement As RectangleF
    End Structure

    Public Function Plot(ByVal Info As Plot_Info) As Bitmap
        With Info
            'Verify validity of parameters
            Dim BoolParametersOK As Boolean = True
            If .Width <= 0 Then MsgBox("Width must be greater than zero", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If .Height <= 0 Then MsgBox("Height must be greater than zero", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If .YData Is Nothing And .XYData Is Nothing Then MsgBox("Data missing", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If (.XLabels Is Nothing) = False And (.XLabelStrings Is Nothing) = False Then MsgBox("Conflicting X labels present", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If ((.XLabels Is Nothing) = False Or (.XLabelStrings Is Nothing) = False) And .AutoScaleX = True Then _
                MsgBox("Autoscale X is incompatible with specifying X labels", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If ((.XLabels Is Nothing) = False Or (.XLabelStrings Is Nothing) = False) And .LogScaleX = True Then _
                MsgBox("Log scale X is incompatible with specifying X labels", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If ((.XLabels Is Nothing) = False Or (.XLabelStrings Is Nothing) = False) And (.XYData Is Nothing) = False Then _
                MsgBox("Providing X and Y data is incompatible with specifying X labels")
            If (.XLabels Is Nothing) = False And (.YData Is Nothing) = False Then
                If .XLabels.GetUpperBound(0) <> .YData.GetUpperBound(1) Then MsgBox("Number of X labels and data must match", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            End If
            If (.XLabelStrings Is Nothing) = False And (.YData Is Nothing) = False Then
                If .XLabelStrings.GetUpperBound(0) <> .YData.GetUpperBound(1) Then MsgBox("Number of X label strings and data must match", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            End If
            If .AutoScaleX = False And .XLabels Is Nothing And .XLabelStrings Is Nothing Then _
                If .XMin >= .XMax Then MsgBox("X Maximum must be greater than X Minimum", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If .AutoScaleY = False Then If .YMin >= .YMax Then MsgBox("Y Maximum must be greater than Y Minimum", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If .LogScaleX = True And (.YData Is Nothing) = False Then MsgBox("Log scale X is unavailable for Y only data", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            If (.BackgroundImage Is Nothing) = False Then
                If .Width <= 0 Then MsgBox("Background image width must be greater than zero.", MsgBoxStyle.Exclamation) : BoolParametersOK = False
                If .Height <= 0 Then MsgBox("Background image height must be greater than zero.", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            End If
            If BoolParametersOK = False Then
                Plot = New Bitmap(20, 20, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                Dim GrphPlotX As Graphics = Graphics.FromImage(Plot) : GrphPlotX.Clear(Color.White)
                GrphPlotX.DrawLine(Pens.Red, 0, 0, 20, 20) : GrphPlotX.DrawLine(Pens.Red, 0, 20, 20, 0)
                Exit Function
            End If

            'Scaling
            Dim IntSet, IntDatum As Integer
            If (.XLabels Is Nothing) = False Or (.XLabelStrings Is Nothing) = False Then .AutoScaleX = True
            If (.YData Is Nothing) = False Then
                If .AutoScaleX = True Then .XMin = 0 : .XMax = .YData.GetUpperBound(1)
                If .AutoScaleY = True Then
                    .YMin = .YData(0, 0) : .YMax = .YMin
                    For IntSet = 0 To .YData.GetUpperBound(0)
                        For IntDatum = 0 To .YData.GetUpperBound(1)
                            If .YMin > .YData(IntSet, IntDatum) Then .YMin = .YData(IntSet, IntDatum)
                            If .YMax < .YData(IntSet, IntDatum) Then .YMax = .YData(IntSet, IntDatum)
                        Next
                    Next
                    If .YMin = .YMax Then .YMin -= 1 : .YMax += 1
                End If
            Else '(.XYData Is Nothing) = False
                If .AutoScaleX = True Then
                    .XMin = .XYData(0, 0).X : .XMax = .XMin
                    For IntSet = 0 To .XYData.GetUpperBound(0)
                        For IntDatum = 0 To .XYData.GetUpperBound(1)
                            If .XMin > .XYData(IntSet, IntDatum).X Then .XMin = .XYData(IntSet, IntDatum).X
                            If .XMax < .XYData(IntSet, IntDatum).X Then .XMax = .XYData(IntSet, IntDatum).X
                        Next
                    Next
                    If .XMin = .XMax Then .XMin -= 1 : .XMax += 1
                End If
                If .AutoScaleY = True Then
                    .YMin = .XYData(0, 0).Y : .YMax = .YMin
                    For IntSet = 0 To .XYData.GetUpperBound(0)
                        For IntDatum = 0 To .XYData.GetUpperBound(1)
                            If .YMin > .XYData(IntSet, IntDatum).Y Then .YMin = .XYData(IntSet, IntDatum).Y
                            If .YMax < .XYData(IntSet, IntDatum).Y Then .YMax = .XYData(IntSet, IntDatum).Y
                        Next
                    Next
                    If .YMin = .YMax Then .YMin -= 1 : .YMax += 1
                End If
            End If
            Dim SngXMin, SngXMax, SngYMin, SngYMax As Single 'Data ranges
            If (.YData Is Nothing) = False Then
                SngXMin = 0 : SngXMax = .YData.GetUpperBound(1)
                SngYMin = .YData(0, 0) : SngYMax = SngYMin
                For IntSet = 0 To .YData.GetUpperBound(0)
                    For IntDatum = 0 To .YData.GetUpperBound(1)
                        If SngYMin > .YData(IntSet, IntDatum) Then SngYMin = .YData(IntSet, IntDatum)
                        If SngYMax < .YData(IntSet, IntDatum) Then SngYMax = .YData(IntSet, IntDatum)
                    Next
                Next
            Else '(.XYData Is Nothing) = False
                SngXMin = .XYData(0, 0).X : SngXMax = SngXMin
                SngYMin = .XYData(0, 0).Y : SngYMax = SngYMin
                For IntSet = 0 To .XYData.GetUpperBound(0)
                    For IntDatum = 0 To .XYData.GetUpperBound(1)
                        If SngXMin > .XYData(IntSet, IntDatum).X Then SngXMin = .XYData(IntSet, IntDatum).X
                        If SngXMax < .XYData(IntSet, IntDatum).X Then SngXMax = .XYData(IntSet, IntDatum).X
                        If SngYMin > .XYData(IntSet, IntDatum).Y Then SngYMin = .XYData(IntSet, IntDatum).Y
                        If SngYMax < .XYData(IntSet, IntDatum).Y Then SngYMax = .XYData(IntSet, IntDatum).Y
                    Next
                Next
            End If
            If .LogScaleX = True Then
                If .AutoScaleX = False And .XMin <= 0 Then MsgBox("Log scale X is incompatible with X labels of zero or less", MsgBoxStyle.Exclamation) : BoolParametersOK = False
                If SngXMin <= 0 Then MsgBox("Log scale X is incompatible with data containing X coordinates of zero or less", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            End If
            If .LogScaleY = True Then
                If .AutoScaleY = False And .YMin <= 0 Then MsgBox("Log scale Y is incompatible with Y labels of zero or less", MsgBoxStyle.Exclamation) : BoolParametersOK = False
                If SngYMin <= 0 Then MsgBox("Log scale Y is incompatible with data containing Y coordinates of zero or less", MsgBoxStyle.Exclamation) : BoolParametersOK = False
            End If
            If BoolParametersOK = False Then
                Plot = New Bitmap(20, 20, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                Dim GrphPlotX As Graphics = Graphics.FromImage(Plot) : GrphPlotX.Clear(Color.White)
                GrphPlotX.DrawLine(Pens.Red, 0, 0, 20, 20) : GrphPlotX.DrawLine(Pens.Red, 0, 20, 20, 0)
                Exit Function
            End If
            If .LogScaleX = True Then
                If .AutoScaleX = False Then
                    .XMin = Int(.XMin) : .XMax = Int(.XMax) + 1
                Else
                    .XMin = Int(Math.Log10(.XMin)) : .XMax = Int(Math.Log10(.XMax)) + 1
                End If
            End If
            If .LogScaleY = True Then
                If .AutoScaleY = False Then
                    .YMin = Int(.YMin) : .YMax = Int(.YMax) + 1
                Else
                    .YMin = Int(Math.Log10(.YMin)) : .YMax = Int(Math.Log10(.YMax)) + 1
                End If
            End If

            'Initialize
            If .ColorBackground = .ColorForeground Then .ColorBackground = Color.White : .ColorForeground = Color.Black
            Plot = New Bitmap(.Width, .Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            Dim GrphPlot As Graphics = Graphics.FromImage(Plot) : GrphPlot.Clear(.ColorBackground)

            'Define plot area
            If .Height - (8 + 2 * 16 + 3 + 1 + 8) < 100 Then .ShowAxisX = False 'Top buffer (8) + 2x String height (16) + Tick (3) + Line thickness (1) + Bottom buffer (8)
            If .Width - (8 + 78 + 16 + 3 + 1 + 8) < 100 Then .ShowAxisY = False 'Left buffer (8) + Max string width (78) + String height (16) + Tick (3) + Line thickness (1) + Right buffer (8)
            Dim RctPlotArea As Rectangle
            If .ShowAxisY = False Then
                RctPlotArea.X = 8 'Left buffer (8)
                RctPlotArea.Width = .Width - (8 + 8) 'Left buffer (8) + Right buffer (8)
            Else
                RctPlotArea.X = 8 + 78 + 16 + 1 'Left buffer (8) + Max string width (78) + String height (16) + Line thickness (1)
                RctPlotArea.Width = .Width - (8 + 78 + 16 + 3 + 1 + 8) 'Left buffer (8) + Max string width (78) + String height (16) + Tick (3) + Line thickness (1) + Right buffer (8)
            End If
            RctPlotArea.Y = 8 'Top buffer (8)
            If .ShowAxisX = False Then
                RctPlotArea.Height = .Height - (8 + 8) 'Top buffer (8) + Bottom buffer (8)
            Else
                RctPlotArea.Height = .Height - (8 + 2 * 16 + 3 + 1 + 8) 'Top buffer (8) + 2x String height (16) + Tick (3) + Line thickness (1) + Bottom buffer (8)
            End If

            'Draw background image (if present)
            If (.BackgroundImage Is Nothing) = False Then
                Dim SngXRatio As Single = (.XMax - .XMin) / RctPlotArea.Width '(units/pixel)
                Dim SngYRatio As Single = (.YMax - .YMin) / RctPlotArea.Height '(units/pixel)
                Dim RctBackgroundImage As Rectangle
                RctBackgroundImage.X = RctPlotArea.X + (.BackgroundImagePlacement.X - .XMin) / SngXRatio
                RctBackgroundImage.Y = RctPlotArea.Y + (.YMax - .BackgroundImagePlacement.Y) / SngYRatio
                RctBackgroundImage.Width = .BackgroundImagePlacement.Width / SngXRatio
                RctBackgroundImage.Height = .BackgroundImagePlacement.Height / SngYRatio
                GrphPlot.DrawImage(.BackgroundImage, RctBackgroundImage)
            End If

            'Data
            Dim IntSetMax, IntDatumMax, IntX, IntPoint, IntR(1), IntG(1), IntB(1) As Integer, PntPlot() As Point
            Dim PenPlot As New Pen(.ColorForeground) : Dim BrshPlot As New SolidBrush(.ColorForeground)
            If (.YData Is Nothing) = False Then IntSetMax = .YData.GetUpperBound(0) Else IntSetMax = .XYData.GetUpperBound(0)
            If (.YData Is Nothing) = False Then IntDatumMax = .YData.GetUpperBound(1) Else IntDatumMax = .XYData.GetUpperBound(1)
            For IntSet = 0 To IntSetMax
                If IntDatumMax < RctPlotArea.Width Or (.XYData Is Nothing) = False Then 'Fewer points than pixels or X specified
                    ReDim PntPlot(IntDatumMax)
                    For IntDatum = 0 To PntPlot.GetUpperBound(0)
                        If (.YData Is Nothing) = False Then
                            'If PntPlot.GetUpperBound(0) > 0 Then
                            PntPlot(IntDatum).X = RctPlotArea.X + ((IntDatum - .XMin) / (.XMax - .XMin)) * RctPlotArea.Width
                            'Else
                            '    PntPlot(IntDatum).X = RctPlotArea.X + RctPlotArea.Width / 2
                            'End If
                            If .LogScaleY = False Then
                                PntPlot(IntDatum).Y = RctPlotArea.Y + (RctPlotArea.Height - ((.YData(IntSet, IntDatum) - .YMin) / (.YMax - .YMin)) * RctPlotArea.Height)
                            Else
                                PntPlot(IntDatum).Y = RctPlotArea.Y + (RctPlotArea.Height - Math.Log10((.YData(IntSet, IntDatum) - .YMin) / (.YMax - .YMin)) * RctPlotArea.Height)
                            End If
                        Else '(.XYData Is Nothing) = False
                            If .LogScaleX = False Then
                                PntPlot(IntDatum).X = RctPlotArea.X + ((.XYData(IntSet, IntDatum).X - .XMin) / (.XMax - .XMin)) * RctPlotArea.Width
                            Else
                                PntPlot(IntDatum).X = RctPlotArea.X + Math.Log10((.XYData(IntSet, IntDatum).X - .XMin) / (.XMax - .XMin)) * RctPlotArea.Width
                            End If
                            If .LogScaleY = False Then
                                PntPlot(IntDatum).Y = RctPlotArea.Y + (RctPlotArea.Height - ((.XYData(IntSet, IntDatum).Y - .YMin) / (.YMax - .YMin)) * RctPlotArea.Height)
                            Else
                                PntPlot(IntDatum).Y = RctPlotArea.Y + (RctPlotArea.Height - Math.Log10((.XYData(IntSet, IntDatum).Y - .YMin) / (.YMax - .YMin)) * RctPlotArea.Height)
                            End If
                        End If
                    Next
                Else 'Fewer pixels than points and no X specified
                    ReDim PntPlot(RctPlotArea.Width) '***Might need a minus 1***
                    For IntX = 0 To PntPlot.GetUpperBound(0)
                        IntDatum = (IntX / PntPlot.GetUpperBound(0)) * .YData.GetUpperBound(1)
                        PntPlot(IntX).X = RctPlotArea.X + ((IntDatum - .XMin) / (.XMax - .XMin)) * RctPlotArea.Width
                        If .LogScaleY = False Then
                            PntPlot(IntX).Y = RctPlotArea.Y + (RctPlotArea.Height - ((.YData(IntSet, IntDatum) - .YMin) / (.YMax - .YMin)) * RctPlotArea.Height)
                        Else
                            PntPlot(IntX).Y = RctPlotArea.Y + (RctPlotArea.Height - Math.Log10((.YData(IntSet, IntDatum) - .YMin) / (.YMax - .YMin)) * RctPlotArea.Height)
                        End If
                    Next
                End If
                If IntSetMax > 0 Then
                    IntR(0) = .ColorBackground.R : IntG(0) = .ColorBackground.G : IntB(0) = .ColorBackground.B
                    IntR(1) = .ColorForeground.R : IntG(1) = .ColorForeground.G : IntB(1) = .ColorForeground.B
                    IntR(1) = Int(IntR(1) - ((IntR(1) - IntR(0)) / 2) * (IntSet / IntSetMax))
                    IntG(1) = Int(IntG(1) - ((IntG(1) - IntG(0)) / 2) * (IntSet / IntSetMax))
                    IntB(1) = Int(IntB(1) - ((IntB(1) - IntB(0)) / 2) * (IntSet / IntSetMax))
                Else
                    IntR(1) = .ColorForeground.R : IntG(1) = .ColorForeground.G : IntB(1) = .ColorForeground.B
                End If
                If .ConnectPoints = True And PntPlot.GetUpperBound(0) > 0 Then
                    PenPlot.Color = Color.FromArgb(IntR(1), IntG(1), IntB(1))
                    GrphPlot.DrawCurve(PenPlot, PntPlot)
                Else
                    BrshPlot.Color = Color.FromArgb(IntR(1), IntG(1), IntB(1))
                    For IntPoint = 0 To PntPlot.GetUpperBound(0)
                        GrphPlot.FillRectangle(BrshPlot, PntPlot(IntPoint).X - 2, PntPlot(IntPoint).Y - 2, 4, 4)
                    Next
                End If
            Next

            'Clear region outside plot area
            BrshPlot.Color = .ColorBackground
            GrphPlot.FillRectangle(BrshPlot, 0, 0, RctPlotArea.X - 1, .Height)
            GrphPlot.FillRectangle(BrshPlot, 0, 0, .Width, RctPlotArea.Y - 1)
            GrphPlot.FillRectangle(BrshPlot, 0, RctPlotArea.Y + RctPlotArea.Height + 1, .Width, .Height)
            GrphPlot.FillRectangle(BrshPlot, RctPlotArea.X + RctPlotArea.Width + 1, 0, .Width, .Height)

            'Axes
            Dim PenTick As New Pen(.ColorForeground)
            Dim PntTick(1), PntLabel As Point, IntTick, IntTickMax As Integer, BmpLabel As Bitmap, SngLabel As Single
            If .ShowAxisX = True Then
                PntTick(0).X = RctPlotArea.X : PntTick(0).Y = RctPlotArea.Y + RctPlotArea.Height + 1
                PntTick(1).X = RctPlotArea.X + (RctPlotArea.Width - 1) : PntTick(1).Y = RctPlotArea.Y + RctPlotArea.Height + 1
                GrphPlot.DrawLine(PenTick, PntTick(0), PntTick(1))
                If (.XLabels Is Nothing) = False Or (.XLabelStrings Is Nothing) = False Then 'Set number of labels
                    If (.XLabels Is Nothing) = False Then IntTickMax = .XLabels.GetUpperBound(0) Else IntTickMax = .XLabelStrings.GetUpperBound(0)
                    If IntTickMax = 0 Then 'Just the one (place in middle)
                        PntTick(0).X = RctPlotArea.X + RctPlotArea.Width / 2 : PntTick(0).Y = RctPlotArea.Y + RctPlotArea.Height + 1
                        PntTick(1).X = RctPlotArea.X + RctPlotArea.Width / 2 : PntTick(1).Y = RctPlotArea.Y + RctPlotArea.Height + 4
                        GrphPlot.DrawLine(PenTick, PntTick(0), PntTick(1))
                        If (.XLabels Is Nothing) = False Then
                            BmpLabel = String_Image(Format_Scientific(.XLabels(0), 3), .ColorBackground, .ColorForeground)
                        Else
                            BmpLabel = String_Image(.XLabelStrings(0), .ColorBackground, .ColorForeground)
                        End If
                        PntLabel.X = PntTick(0).X - BmpLabel.Width / 2 : PntLabel.Y = PntTick(1).Y + 1
                        GrphPlot.DrawImage(BmpLabel, PntLabel)
                        BmpLabel = String_Image(.XTitle, .ColorBackground, .ColorForeground)
                        PntLabel.X = PntTick(0).X - BmpLabel.Width / 2 : PntLabel.Y = PntTick(1).Y + 17
                        GrphPlot.DrawImage(BmpLabel, PntLabel)
                    Else 'Two ends plus possible middle value(s)
                        For IntTick = 0 To IntTickMax
                            PntTick(0).X = RctPlotArea.X + (IntTick / IntTickMax) * (RctPlotArea.Width - 1) : PntTick(0).Y = RctPlotArea.Y + RctPlotArea.Height + 1
                            PntTick(1).X = RctPlotArea.X + (IntTick / IntTickMax) * (RctPlotArea.Width - 1) : PntTick(1).Y = RctPlotArea.Y + RctPlotArea.Height + 4
                            GrphPlot.DrawLine(PenTick, PntTick(0), PntTick(1))
                            If (.XLabels Is Nothing) = False Then
                                BmpLabel = String_Image(Format_Scientific(.XLabels(IntTick), 3), .ColorBackground, .ColorForeground)
                            Else
                                BmpLabel = String_Image(.XLabelStrings(IntTick), .ColorBackground, .ColorForeground)
                            End If
                            If IntTick = 0 Then
                                PntLabel.X = PntTick(0).X
                            ElseIf IntTick = IntTickMax Then
                                PntLabel.X = PntTick(0).X - BmpLabel.Width
                            Else
                                PntLabel.X = PntTick(0).X - BmpLabel.Width / 2
                            End If
                            PntLabel.Y = PntTick(1).Y + 1
                            GrphPlot.DrawImage(BmpLabel, PntLabel)
                        Next
                    End If
                Else 'Fit number of labels (assume max width of 78 pixels)
                    IntTickMax = Int(RctPlotArea.Width / 78) - 1 'Shouldn't be able to get below zero
                    For IntTick = 0 To IntTickMax
                        PntTick(0).X = RctPlotArea.X + (IntTick / IntTickMax) * (RctPlotArea.Width - 1) : PntTick(0).Y = RctPlotArea.Y + RctPlotArea.Height + 1
                        PntTick(1).X = RctPlotArea.X + (IntTick / IntTickMax) * (RctPlotArea.Width - 1) : PntTick(1).Y = RctPlotArea.Y + RctPlotArea.Height + 4
                        GrphPlot.DrawLine(PenTick, PntTick(0), PntTick(1))
                        If .LogScaleX = False Then
                            SngLabel = .XMin + (IntTick / IntTickMax) * (.XMax - .XMin)
                        Else
                            SngLabel = 10 ^ (.XMin + (IntTick / IntTickMax) * (.XMax - .XMin))
                        End If
                        BmpLabel = String_Image(Format_Scientific(SngLabel, 3), .ColorBackground, .ColorForeground)
                        If IntTick = 0 Then
                            PntLabel.X = PntTick(0).X
                        ElseIf IntTick = IntTickMax Then
                            PntLabel.X = PntTick(0).X - BmpLabel.Width
                        Else
                            PntLabel.X = PntTick(0).X - BmpLabel.Width / 2
                        End If
                        PntLabel.Y = PntTick(1).Y + 1
                        GrphPlot.DrawImage(BmpLabel, PntLabel)
                    Next
                End If
                BmpLabel = String_Image(.XTitle, .ColorBackground, .ColorForeground)
                PntLabel.X = (RctPlotArea.X + RctPlotArea.Width / 2) - BmpLabel.Width / 2 : PntLabel.Y = PntTick(1).Y + 17
                GrphPlot.DrawImage(BmpLabel, PntLabel)
            End If
            If .ShowAxisY = True Then
                PntTick(0).X = RctPlotArea.X : PntTick(0).Y = RctPlotArea.Y
                PntTick(1).X = RctPlotArea.X : PntTick(1).Y = RctPlotArea.Y + RctPlotArea.Height + 1
                GrphPlot.DrawLine(PenTick, PntTick(0), PntTick(1))
                IntTickMax = Int(RctPlotArea.Height / 32) - 1
                For IntTick = 0 To IntTickMax
                    PntTick(0).X = RctPlotArea.X - 3 : PntTick(0).Y = RctPlotArea.Y + (IntTick / IntTickMax) * RctPlotArea.Height
                    PntTick(1).X = RctPlotArea.X : PntTick(1).Y = RctPlotArea.Y + (IntTick / IntTickMax) * RctPlotArea.Height
                    GrphPlot.DrawLine(PenTick, PntTick(0), PntTick(1))
                    If .LogScaleY = False Then
                        SngLabel = .YMax - ((IntTick / IntTickMax) * (.YMax - .YMin))
                    Else
                        SngLabel = 10 ^ (.YMax - ((IntTick / IntTickMax) * (.YMax - .YMin)))
                    End If
                    BmpLabel = String_Image(Format_Scientific(SngLabel, 3), .ColorBackground, .ColorForeground)
                    PntLabel.X = RctPlotArea.X - 3 - BmpLabel.Width
                    If IntTick = 0 Then
                        PntLabel.Y = PntTick(0).Y
                    ElseIf IntTick = IntTickMax Then
                        PntLabel.Y = PntTick(0).Y - BmpLabel.Height
                    Else
                        PntLabel.Y = PntTick(0).Y - BmpLabel.Height / 2
                    End If
                    GrphPlot.DrawImage(BmpLabel, PntLabel)
                Next
                BmpLabel = String_Image(.YTitle, .ColorBackground, .ColorForeground) : BmpLabel.RotateFlip(RotateFlipType.Rotate270FlipNone)
                PntLabel.X = 0 : PntLabel.Y = RctPlotArea.Y + RctPlotArea.Height / 2 - BmpLabel.Height / 2
                GrphPlot.DrawImage(BmpLabel, PntLabel)
            End If
        End With
    End Function

    Private Function String_Image(ByVal ImageString As String, ByVal BackColor As Color, ByVal ForeColor As Color) As Bitmap
        Dim FntString As New Font("Courier New", 10) 'Fixed with font
        'Letters are 7 pixels wide with 1 pixel spacing, but with an extra
        'pixel after every four characters starting after character #2
        'e.g. 7 -1- 7 -2- 7 -1- 7 -1- 7 -1- 7 -2- 7 -1- 7 -1- 7 -1- 7 -2- etc.
        'Formula (simplified) for width is then 8n + n/4 + 4
        '(+ 4 is for offset from left edge)
        'Need 4 pixels from top + 10 for font size = 14 height minimum (no need to pad top, maybe make it an even 16 for the bottom)
        '(Numbers in scientific format with 3 significnat digits will give a maximum width of 78)
        If ImageString = "" Then ImageString = " "
        Dim IntWidth As Integer = ImageString.Length * 8 + Int(ImageString.Length / 4) + 4
        Dim IntHeight As Integer = 16
        String_Image = New Bitmap(IntWidth, IntHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim GrphString As Graphics = Graphics.FromImage(String_Image)
        Dim BrshString As New SolidBrush(ForeColor)
        GrphString.Clear(BackColor) : GrphString.DrawString(ImageString, FntString, BrshString, 0, 0)
    End Function

    Private Function Format_Scientific(ByVal Value As Double, ByVal SigDigits As Integer) As String
        Dim Original As Double = Value
        Dim IntExponent As Integer
        Dim ChrSign As Char
        If Value >= 1 Or Value <= -1 Then
            ChrSign = "+"
            Do Until (Value > 0 And Value < 10) Or (Value < 0 And Value > -10)
                Value *= 0.1
                IntExponent += 1
            Loop
        ElseIf Value <> 0 Then '< 1
            ChrSign = "-"
            Do Until (Value > 0 And Value > 1) Or (Value < 0 And Value < -1)
                Value *= 10
                IntExponent += 1
            Loop
        End If
        Format_Scientific = Value
        Dim IntPad As Integer = 1 : If Value < 0 Then IntPad = 2
        If SigDigits > 1 Then
            If Format_Scientific.Length >= SigDigits + IntPad Then 'If not then it's already shorter
                Format_Scientific = Format_Scientific.Substring(0, SigDigits + IntPad)
            End If
        Else
            Format_Scientific = Format_Scientific.Substring(0, IntPad)
        End If
        If (ChrSign = "-" And IntExponent >= SigDigits - 1) Or (ChrSign = "+" And IntExponent >= SigDigits) Then
            Format_Scientific &= "E" & ChrSign & IntExponent
        Else 'Ignore scientfic notation for numbers near enough to 1
            If ChrSign = "+" Then
                Format_Scientific *= 10 ^ IntExponent
            Else
                Format_Scientific *= 10 ^ -IntExponent
            End If
        End If
    End Function

End Module