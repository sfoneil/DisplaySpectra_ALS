Module OL490_Calibration_Correction_v1

    Structure Calibration_Point
        Dim IntWavelength As Integer
        Dim SngIntensity() As Single 'Will be 0-3 for 25, 50, 75 and 100
    End Structure
    Structure Calibration_Set
        Dim CalibrationPoint() As Calibration_Point
    End Structure

    Structure Data_Point
        Dim SngWavelength, SngIntensity As Single
    End Structure

    Public Function Open_Correction(ByVal FileName As String) As Calibration_Set
        ReDim Open_Correction.CalibrationPoint(0) : ReDim Open_Correction.CalibrationPoint(0).SngIntensity(3) 'Avoids Null reference error
        Dim IntDatum, IntIndex As Integer
        FileOpen(1, FileName, OpenMode.Input)
        While EOF(1) = False
            ReDim Preserve Open_Correction.CalibrationPoint(IntDatum)
            With Open_Correction.CalibrationPoint(IntDatum)
                ReDim .SngIntensity(3)
                Input(1, .IntWavelength)
                For IntIndex = 3 To 0 Step -1
                    Input(1, .SngIntensity(IntIndex))
                Next
            End With
            IntDatum += 1
        End While
        FileClose(1)
    End Function

    Public Function Adjust_Point(ByVal CalibrationSet As Calibration_Set, ByVal DataPoint As Data_Point) As Data_Point
        Adjust_Point.SngWavelength = DataPoint.SngWavelength 'Direct copy
        Dim SngWavBounds(1), SngIntBounds(1) As Single 'Hard coded now but treating as variable JIC
        SngWavBounds(0) = 380 : SngWavBounds(1) = 780
        SngIntBounds(0) = 25 : SngIntBounds(1) = 100
        Dim StrWavLocation, StrIntLocation As String, IntWav, IntInt, IntCount, IntLine As Integer
        Select Case DataPoint.SngWavelength
            Case Is <= SngWavBounds(0) : StrWavLocation = "Ultraviolet" : IntWav = 0
            Case Is >= SngWavBounds(1) : StrWavLocation = "Infrared" : IntWav = CalibrationSet.CalibrationPoint.Length - 1
            Case Else
                StrWavLocation = "Visible" : IntCount += 1
                Do
                    IntWav += 1
                Loop Until DataPoint.SngWavelength <= CalibrationSet.CalibrationPoint(IntWav).IntWavelength
        End Select
        Select Case DataPoint.SngIntensity
            Case Is <= SngIntBounds(0) : StrIntLocation = "Dim" : IntInt = 0 : IntLine = 25
            Case Is >= SngIntBounds(1) : StrIntLocation = "Too bright" : IntInt = 3 : IntLine = 100
            Case Else
                StrIntLocation = "Normal" : IntCount += 1
                IntInt = Int(DataPoint.SngIntensity * 0.04) - 1 '0, 1, 2, or 3
                IntLine = IntInt * 25 + 25
        End Select
        Dim SngInfluence(,) As Single 'One member for each point (1, 2, or 4)
        With CalibrationSet
            Select Case IntCount
                Case 0 'Corner (use 1 reference point)
                    ReDim SngInfluence(0, 0)
                    SngInfluence(0, 0) = 1
                    Adjust_Point.SngIntensity = DataPoint.SngIntensity - SngInfluence(0, 0) * (IntLine - .CalibrationPoint(IntWav).SngIntensity(IntInt))
                Case 1 'Edge (use 2 reference points)
                    ReDim SngInfluence(1, 0)
                    If StrWavLocation = "Visible" Then
                        SngInfluence(0, 0) = 1 - Math.Abs((DataPoint.SngWavelength - .CalibrationPoint(IntWav - 1).IntWavelength) / (.CalibrationPoint(IntWav).IntWavelength - .CalibrationPoint(IntWav - 1).IntWavelength))
                        SngInfluence(1, 0) = 1 - Math.Abs((DataPoint.SngWavelength - .CalibrationPoint(IntWav).IntWavelength) / (.CalibrationPoint(IntWav).IntWavelength - .CalibrationPoint(IntWav - 1).IntWavelength))
                        Adjust_Point.SngIntensity = DataPoint.SngIntensity - SngInfluence(0, 0) * (IntLine - .CalibrationPoint(IntWav - 1).SngIntensity(IntInt)) - _
                                                                             SngInfluence(1, 0) * (IntLine - .CalibrationPoint(IntWav).SngIntensity(IntInt))
                    Else 'StrIntLocation = "Normal"
                        SngInfluence(0, 0) = 1 - Math.Abs((DataPoint.SngIntensity - IntLine) / 25)
                        SngInfluence(1, 0) = 1 - Math.Abs((DataPoint.SngIntensity - (IntLine + 25)) / 25)
                        Adjust_Point.SngIntensity = DataPoint.SngIntensity - SngInfluence(0, 0) * (IntLine - .CalibrationPoint(IntWav).SngIntensity(IntInt)) - _
                                                                             SngInfluence(1, 0) * ((IntLine + 25) - .CalibrationPoint(IntWav).SngIntensity(IntInt + 1))
                    End If
                Case 2 'Middle (use 4 reference points)
                    ReDim SngInfluence(3, 1)
                    SngInfluence(0, 0) = 1 - Math.Abs((DataPoint.SngWavelength - .CalibrationPoint(IntWav - 1).IntWavelength) / (.CalibrationPoint(IntWav).IntWavelength - .CalibrationPoint(IntWav - 1).IntWavelength))
                    SngInfluence(0, 1) = 1 - Math.Abs((DataPoint.SngIntensity - IntLine) / 25)
                    SngInfluence(1, 0) = 1 - Math.Abs((DataPoint.SngWavelength - .CalibrationPoint(IntWav - 1).IntWavelength) / (.CalibrationPoint(IntWav).IntWavelength - .CalibrationPoint(IntWav - 1).IntWavelength))
                    SngInfluence(1, 1) = 1 - Math.Abs((DataPoint.SngIntensity - (IntLine + 25)) / 25)
                    SngInfluence(2, 0) = 1 - Math.Abs((DataPoint.SngWavelength - .CalibrationPoint(IntWav).IntWavelength) / (.CalibrationPoint(IntWav).IntWavelength - .CalibrationPoint(IntWav - 1).IntWavelength))
                    SngInfluence(2, 1) = 1 - Math.Abs((DataPoint.SngIntensity - IntLine) / 25)
                    SngInfluence(3, 0) = 1 - Math.Abs((DataPoint.SngWavelength - .CalibrationPoint(IntWav).IntWavelength) / (.CalibrationPoint(IntWav).IntWavelength - .CalibrationPoint(IntWav - 1).IntWavelength))
                    SngInfluence(3, 1) = 1 - Math.Abs((DataPoint.SngIntensity - (IntLine + 25)) / 25)
                    Adjust_Point.SngIntensity = DataPoint.SngIntensity - SngInfluence(0, 0) * SngInfluence(0, 1) * (IntLine - .CalibrationPoint(IntWav - 1).SngIntensity(IntInt)) - _
                                                                         SngInfluence(1, 0) * SngInfluence(1, 1) * ((IntLine + 25) - .CalibrationPoint(IntWav - 1).SngIntensity(IntInt + 1)) - _
                                                                         SngInfluence(2, 0) * SngInfluence(2, 1) * (IntLine - .CalibrationPoint(IntWav).SngIntensity(IntInt)) - _
                                                                         SngInfluence(3, 0) * SngInfluence(3, 1) * ((IntLine + 25) - .CalibrationPoint(IntWav).SngIntensity(IntInt + 1))
            End Select
        End With
        If Adjust_Point.SngIntensity < 0 Then Adjust_Point.SngIntensity = 0
        'If Adjust_Point.SngIntensity > 100 Then MsgBox("Saturation at " & Adjust_Point.SngWavelength & " nm")
        If Adjust_Point.SngIntensity > 100 Then Adjust_Point.SngIntensity = 100
    End Function

End Module
