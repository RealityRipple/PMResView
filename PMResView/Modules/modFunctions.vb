Module modFunctions
  Public Const LATIN_1 As Integer = 28591
  Public Const UTF_8 As Integer = 65001
  Public Const UTF_16_LE As Integer = 1200
  Public Const UTF_32_LE As Integer = 12000
  Public Function SuperMsgBox(Parent As Form, Title As String, Icon As Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon, Header As String, Message As String, Buttons As Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons, Optional Footer As String = Nothing, Optional FooterIcon As Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.None, Optional Fraction As Decimal = -1) As Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult
    Dim sTitle As String = My.Application.Info.Title & " - " & Title
    If Microsoft.WindowsAPICodePack.Dialogs.TaskDialog.IsPlatformSupported Then
      Dim tD As New Microsoft.WindowsAPICodePack.Dialogs.TaskDialog
      If Parent IsNot Nothing AndAlso Parent.IsHandleCreated Then tD.OwnerWindowHandle = Parent.Handle
      tD.Caption = sTitle
      tD.InstructionText = Header
      tD.Text = Message
      tD.Icon = Icon
      If Not String.IsNullOrEmpty(Footer) Then
        tD.FooterIcon = FooterIcon
        tD.FooterText = Footer
      End If
      If Fraction >= 0 Then tD.ProgressBar = New Microsoft.WindowsAPICodePack.Dialogs.TaskDialogProgressBar(0, 100, Math.Floor(Fraction * 100))
      tD.StandardButtons = Buttons
      tD.StartupLocation = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStartupLocation.CenterOwner
      AddHandler tD.Opened, AddressOf taskDialog_Opened
      Return tD.Show()
    Else
      Dim sMsg As String = Header & vbNewLine & vbNewLine & Message
      If Not String.IsNullOrEmpty(Footer) Then sMsg &= vbNewLine & vbNewLine & Footer
      Dim mStyle As MsgBoxStyle = MsgBoxStyle.OkOnly
      If (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes And
        (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No Then
        If (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel Then
          mStyle = mStyle Or MsgBoxStyle.YesNoCancel
        Else
          mStyle = mStyle Or MsgBoxStyle.YesNo
        End If
      ElseIf (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok And
        (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel Then
        mStyle = mStyle Or MsgBoxStyle.OkCancel
      ElseIf (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Retry) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Retry And
        (Buttons And Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel) = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Cancel Then
        mStyle = mStyle Or MsgBoxStyle.RetryCancel
      End If
      If Icon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Error Then
        mStyle = mStyle Or MsgBoxStyle.Critical
      ElseIf Icon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Warning Then
        mStyle = mStyle Or MsgBoxStyle.Exclamation
      ElseIf Icon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information Then
        mStyle = mStyle Or MsgBoxStyle.Information
      End If
      Dim mRet As MsgBoxResult = MsgBox(sMsg, mStyle, sTitle)
      Select Case mRet
        Case MsgBoxResult.Yes : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes
        Case MsgBoxResult.No : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No
        Case MsgBoxResult.Retry : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Retry
        Case MsgBoxResult.Ok : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Ok
        Case MsgBoxResult.Cancel : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Cancel
        Case MsgBoxResult.Abort : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Close
        Case MsgBoxResult.Ignore : Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.CustomButtonClicked
      End Select
      Return Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.None
    End If
  End Function
  Private Sub taskDialog_Opened(sender As Object, e As EventArgs)
    Dim taskDialog As Microsoft.WindowsAPICodePack.Dialogs.TaskDialog = sender
    If Not taskDialog.Icon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.None Then taskDialog.Icon = taskDialog.Icon
    If Not taskDialog.FooterIcon = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.None Then taskDialog.FooterIcon = taskDialog.FooterIcon
    If Not String.IsNullOrEmpty(taskDialog.InstructionText) Then taskDialog.InstructionText = taskDialog.InstructionText
  End Sub
  Public Sub CreateAllSubdirs(sPath As String)
    Dim sSegments() As String = Split(sPath, IO.Path.DirectorySeparatorChar)
    Dim sDir As String = ""
    For I As Integer = 0 To sSegments.Length - 1
      sDir &= sSegments(I) & IO.Path.DirectorySeparatorChar
      If Not IO.Directory.Exists(sDir) Then IO.Directory.CreateDirectory(sDir)
    Next
  End Sub
  Public Function ByteSize(InBytes As UInt64) As String
    If InBytes >= 1000 Then
      If InBytes / 1024 >= 1000 Then
        If InBytes / 1024 / 1024 >= 1000 Then
          If InBytes / 1024 / 1024 / 1024 >= 1000 Then
            If InBytes / 1024 / 1024 / 1024 / 1024 >= 1000 Then
              Return Format((InBytes) / 1024 / 1024 / 1024 / 1024 / 1024, "0.##") & " PB"
            Else
              Return Format((InBytes) / 1024 / 1024 / 1024 / 1024, "0.##") & " TB"
            End If
          Else
            Return Format((InBytes) / 1024 / 1024 / 1024, "0.##") & " GB"
          End If
        Else
          Return Format((InBytes) / 1024 / 1024, "0.##") & " MB"
        End If
      Else
        Return Format((InBytes) / 1024, "0.#") & " KB"
      End If
    Else
      Return InBytes & " B"
    End If
  End Function
  Public Function GetDefaultShell() As String
    Dim explorer As String = "explorer.exe"
    Try
      explorer = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE", False).OpenSubKey("Microsoft", False).OpenSubKey("Windows NT", False).OpenSubKey("CurrentVersion", False).OpenSubKey("Winlogon", False).GetValue("Shell", "explorer.exe")
    Catch ex As Exception
    End Try
    Return explorer
  End Function
  Public Function GetFileIcon(ByVal sPath As String) As Bitmap
    Dim cW As Integer = NativeMethods.GetSystemMetrics(NativeMethods.MetricsList.SM_CXSMICON)
    Dim cH As Integer = NativeMethods.GetSystemMetrics(NativeMethods.MetricsList.SM_CYSMICON)
    Dim shInfo As New NativeMethods.SHFILEINFO
    Dim hImgSmall As IntPtr
    hImgSmall = NativeMethods.SHGetFileInfo(sPath, 0, shInfo, Runtime.InteropServices.Marshal.SizeOf(shInfo), NativeMethods.EShellGetFileInfoConstants.SHGFI_ICON Or NativeMethods.EShellGetFileInfoConstants.SHGFI_SMALLICON)
    Try
      If shInfo.hIcon = 0 Then
        If IO.File.Exists(sPath) Then
          Using bX As New Bitmap(cW, cH)
            Using g As Graphics = Graphics.FromImage(bX)
              g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
              g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
              g.DrawIcon(Icon.ExtractAssociatedIcon(sPath), New Rectangle(0, 0, cW, cH))
            End Using
            Return bX.Clone
          End Using
        End If
        Return Nothing
      End If
      Dim icoRet As Icon = Icon.FromHandle(shInfo.hIcon).Clone
      NativeMethods.DestroyIcon(shInfo.hIcon)
      Using bX As New Bitmap(cW, cH)
        Using g As Graphics = Graphics.FromImage(bX)
          g.DrawIconUnstretched(icoRet, New Rectangle(0, 0, cW, cH))
        End Using
        Return bX.Clone
      End Using
    Catch ex As Exception
      If IO.File.Exists(sPath) Then
        Using bX As New Bitmap(cW, cH)
          Using g As Graphics = Graphics.FromImage(bX)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.DrawIcon(Icon.ExtractAssociatedIcon(sPath), New Rectangle(0, 0, cW, cH))
          End Using
          Return bX.Clone
        End Using
      End If
      Return Nothing
    End Try
  End Function
  Public Function GetFileIcon32(ByVal sPath As String) As Bitmap
    Dim cW As Integer = NativeMethods.GetSystemMetrics(NativeMethods.MetricsList.SM_CXICON)
    Dim cH As Integer = NativeMethods.GetSystemMetrics(NativeMethods.MetricsList.SM_CYICON)
    Dim shInfo As New NativeMethods.SHFILEINFO
    Dim hImgSmall As IntPtr
    hImgSmall = NativeMethods.SHGetFileInfo(sPath, 0, shInfo, Runtime.InteropServices.Marshal.SizeOf(shInfo), NativeMethods.EShellGetFileInfoConstants.SHGFI_ICON Or NativeMethods.EShellGetFileInfoConstants.SHGFI_LARGEICON)
    Try
      If shInfo.hIcon = 0 Then
        If IO.File.Exists(sPath) Then
          Using bX As New Bitmap(cW, cH)
            Using g As Graphics = Graphics.FromImage(bX)
              g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
              g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
              g.DrawIcon(Icon.ExtractAssociatedIcon(sPath), New Rectangle(0, 0, cW, cH))
            End Using
            Return bX.Clone
          End Using
        End If
        Return Nothing
      End If
      Dim icoRet As Icon = Icon.FromHandle(shInfo.hIcon).Clone
      NativeMethods.DestroyIcon(shInfo.hIcon)
      Using bX As New Bitmap(cW, cH)
        Using g As Graphics = Graphics.FromImage(bX)
          g.DrawIconUnstretched(icoRet, New Rectangle(0, 0, cW, cH))
        End Using
        Return bX.Clone
      End Using
    Catch ex As Exception
      If IO.File.Exists(sPath) Then
        Using bX As New Bitmap(cW, cH)
          Using g As Graphics = Graphics.FromImage(bX)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.DrawIcon(Icon.ExtractAssociatedIcon(sPath), New Rectangle(0, 0, cW, cH))
          End Using
          Return bX.Clone
        End Using
      End If
      Return Nothing
    End Try
  End Function
  Public Function GetRIFFData(cRiff As clsRIFF) As String
    If cRiff.IsDTS Then
      Dim sChan As String
      Select Case cRiff.DTSData.iAMODE
        Case 0 : sChan = "Mono"
        Case 1 : sChan = "Dual Mono (A + B)"
        Case 2 : sChan = "Left and Right Stereo"
        Case 3 : sChan = "(Left + Right) - (Left - Right) Stereo"
        Case 4 : sChan = "Totals Stereo"
        Case 5 : sChan = "Center, Left, Right"
        Case 6 : sChan = "Left, Right, Surround"
        Case 7 : sChan = "Center, Left, Right, Surround"
        Case 8 : sChan = "Left, Right, Surround Left, Surround Right"
        Case 9 : sChan = "Center, Left, Right, Surround Left, Surround Right"
        Case 10 : sChan = "Center Left, Center Right, Left, Right, Surround Left, Surround Right"
        Case 11 : sChan = "Center, Left, Right, Left Rear, Right Rear, Overhead"
        Case 12 : sChan = "Center Front, Center Rear, Left Front, Right Front, Left Rear, Right Rear"
        Case 13 : sChan = "Center Left, Center, Center Right, Left, Right, Surround Left, Surround Right"
        Case 14 : sChan = "Center Left, Center Right, Left, Right, Surround Left 1, Surround Left 2, Surround Right 1, Surround Right 2"
        Case 15 : sChan = "Center Left, Center, Center Right, Left, Right, Surround Left, Surround, Surround Right"
        Case Else : sChan = cRiff.DTSData.iAMODE
      End Select
      If cRiff.DTSData.iLFF > 0 Then sChan &= " & Low Frequency Emitter (" & IIf(cRiff.DTSData.iLFF = 2, "64", "128") & " Interpolation Factor)"
      Dim sRate As String = "Unknown"
      Select Case cRiff.DTSData.iRATE
        Case 0 : sRate = "32 kbps"
        Case 1 : sRate = "56 kbps"
        Case 2 : sRate = "64 kbps"
        Case 3 : sRate = "96 kbps"
        Case 4 : sRate = "112 kbps"
        Case 5 : sRate = "128 kbps"
        Case 6 : sRate = "192 kbps"
        Case 7 : sRate = "224 kbps"
        Case 8 : sRate = "256 kbps"
        Case 9 : sRate = "320 kbps"
        Case 10 : sRate = "384 kbps"
        Case 11 : sRate = "448 kbps"
        Case 12 : sRate = "512 kbps"
        Case 13 : sRate = "576 kbps"
        Case 14 : sRate = "640 kbps"
        Case 15 : sRate = "768 kbps"
        Case 16 : sRate = "960 kbps"
        Case 17 : sRate = "1024 kbps"
        Case 18 : sRate = "1152 kbps"
        Case 19 : sRate = "1280 kbps"
        Case 20 : sRate = "1344 kbps"
        Case 21 : sRate = "1408 kbps"
        Case 22 : sRate = "1411.2 kbps"
        Case 23 : sRate = "1472 kbps"
        Case 24 : sRate = "1536 kbps"
        Case 25 : sRate = "Open"
      End Select
      Return "  Encoding: DTS" & vbNewLine &
             "  Channels: " & sChan & vbNewLine &
             "  Bitrate: " & sRate
    End If
    If cRiff.IsWAV Then
      Dim sEnc As String = WAVAudioCodecs(cRiff.WAVData.Format.Format.wFormatTag)
      Dim sChan As String
      Select Case cRiff.WAVData.Format.Format.nChannels
        Case 1 : sChan = "Mono"
        Case 2 : sChan = "Stereo"
        Case 3 : sChan = "2.1 Stereo"
        Case 4 : sChan = "Quadraphonic"
        Case 5 : sChan = "Surround"
        Case 6 : sChan = "5.1 Surround"
        Case 7 : sChan = "6.1 Surround"
        Case 8 : sChan = "7.1 Surround"
        Case Else : sChan = cRiff.WAVData.Format.Format.nChannels
      End Select
      If cRiff.WAVData.Format.cbSize > 0 Or cRiff.WAVData.Format.wBitsPerSample > 0 Then
        If cRiff.WAVData.Format.cbSize >= 22 Then
          If cRiff.WAVData.dwChannelMask > 0 Then
            sChan = String.Empty
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.FrontLeft) = clsRIFF.ChannelStruct.FrontLeft Then sChan &= "Front Left, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.FrontCenterLeft) = clsRIFF.ChannelStruct.FrontCenterLeft Then sChan &= "Front Center Left, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.FrontCenter) = clsRIFF.ChannelStruct.FrontCenter Then sChan &= "Front Center, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.FrontCenterRight) = clsRIFF.ChannelStruct.FrontCenterRight Then sChan &= "Front Center Right, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.FrontRight) = clsRIFF.ChannelStruct.FrontRight Then sChan &= "Front Right, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.SideLeft) = clsRIFF.ChannelStruct.SideLeft Then sChan &= "Side Left, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.SideRight) = clsRIFF.ChannelStruct.SideRight Then sChan &= "Side Right, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.RearLeft) = clsRIFF.ChannelStruct.RearLeft Then sChan &= "Rear Left, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.RearCenter) = clsRIFF.ChannelStruct.RearCenter Then sChan &= "Rear Center, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.RearRight) = clsRIFF.ChannelStruct.RearRight Then sChan &= "Rear Right, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.TopCenter) = clsRIFF.ChannelStruct.TopCenter Then sChan &= "Top Center, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.TopFrontLeft) = clsRIFF.ChannelStruct.TopFrontLeft Then sChan &= "Top Front Left, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.TopFrontCenter) = clsRIFF.ChannelStruct.TopFrontCenter Then sChan &= "Top Front Center, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.TopFrontRight) = clsRIFF.ChannelStruct.TopFrontRight Then sChan &= "Top Front Right, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.TopRearLeft) = clsRIFF.ChannelStruct.TopRearLeft Then sChan &= "Top Rear Left, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.TopRearCenter) = clsRIFF.ChannelStruct.TopRearCenter Then sChan &= "Top Rear Center, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.TopRearRight) = clsRIFF.ChannelStruct.TopRearRight Then sChan &= "Top Rear Right, "
            If (cRiff.WAVData.dwChannelMask And clsRIFF.ChannelStruct.LFE) = clsRIFF.ChannelStruct.LFE Then sChan &= "Low Frequency Emitter, "
            If sChan.Length > 2 Then sChan = sChan.Substring(0, sChan.Length - 2)
          End If
          Select Case cRiff.WAVData.SubFormat.ToString.ToLower
            Case "6dba3190-67bd-11cf-a0f7-0020afd156e4" : sEnc = "Analog"
            Case "00000001-0000-0010-8000-00aa00389b71" : sEnc = "PCM"
            Case "00000003-0000-0010-8000-00aa00389b71" : sEnc = "Float (IEEE)"
            Case "00000009-0000-0010-8000-00aa00389b71" : sEnc = "DRM"
            Case "00000006-0000-0010-8000-00aa00389b71" : sEnc = "A-Law"
            Case "00000007-0000-0010-8000-00aa00389b71" : sEnc = "µ-Law"
            Case "00000002-0000-0010-8000-00aa00389b71" : sEnc = "ADPCM"
            Case "00000050-0000-0010-8000-00aa00389b71" : sEnc = "MPEG"
            Case "4995daee-9ee6-11d0-a40e-00a0c9223196" : sEnc = "RIFF"
            Case "e436eb8b-524f-11ce-9f53-0020af0ba770" : sEnc = "RIFF WAVE"
            Case "1d262760-e957-11cf-a5d6-28db04c10000" : sEnc = "MIDI"
            Case "2ca15fa0-6cfe-11cf-a5d6-28dB04c10000" : sEnc = "MIDI Bus"
            Case "4995daf0-9ee6-11d0-a40e-00a0c9223196" : sEnc = "RIFF MIDI"
            Case Else : sEnc = "Unknown {" & cRiff.WAVData.SubFormat.ToString & "}"
          End Select
        End If
      End If
      Return "  Encoding: " & sEnc & vbNewLine &
             "  Channels: " & sChan
    End If
    Return "  Encoding: Unknown"
  End Function
  Private Function WAVAudioCodecs(formatTag As clsRIFF.WAVFormatTag) As String
    Select Case formatTag
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_UNKNOWN : Return "Unknown Wave Format"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_PCM : Return "Microsoft PCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ADPCM : Return "Microsoft ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_IEEE_FLOAT : Return "Float (IEEE)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VSELP : Return "Compaq Computer's VSELP codec for Windows CE 2.0 devices"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_IBM_CVSD : Return "IBM CVSD"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ALAW : Return "A-Law"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MULAW : Return "µ-Law"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DTS : Return "Digital Theater Systems (DTS)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DRM : Return "DRM Encryped"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_WMAVOICE9 : Return "Windows Media Audio 9 Voice"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_OKI_ADPCM : Return "OKI ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DVI_ADPCM : Return "Intel's DVI ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MEDIASPACE_ADPCM : Return "Videologic's MediaSpace ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_SIERRA_ADPCM : Return "Sierra ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_G723_ADPCM : Return "G.723 ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DIGISTD : Return "DSP Solution's DIGISTD"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DIGIFIX : Return "DSP Solution's DIGIFIX"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DIALOGIC_OKI_ADPCM : Return "Dialogic OKI ADPCM for OKI ADPCM chips or firmware"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MEDIAVISION_ADPCM : Return "MediaVision ADPCM for Jazz 16 chip set"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_CU_CODEC : Return "HP CU"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_YAMAHA_ADPCM : Return "Yamaha ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_SONARC : Return "Speech Compression's Sonarc"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DSPGROUP_TRUESPEECH : Return "DSP Group's True Speech"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ECHOSC1 : Return "Echo Speech's EchoSC1"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_AUDIOFILE_AF36 : Return "Audiofile AF36"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_APTX : Return "APTX"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_AUDIOFILE_AF10 : Return "AudioFile AF10"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_PROSODY_1612 : Return "Prosody 1612 CTI Speech Card"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_LRC : Return "LRC"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DOLBY_AC2 : Return "Dolby AC2"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_GSM610 : Return "GSM610"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MSNAUDIO : Return "Microsoft MSN Audio Codec"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ANTEX_ADPCME : Return "Antex ADPCME"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_CONTROL_RES_VQLPC : Return "Control Res VQLPC"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DIGIREAL : Return "Digireal"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DIGIADPCM : Return "DigiADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_CONTROL_RES_CR10 : Return "Control Res CR10"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_NMS_VBXADPCM : Return "NMS VBXADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_CS_IMAADPCM : Return "Crystal Semiconductor IMA ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ECHOSC3 : Return "EchoSC3 Proprietary Compression"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ROCKWELL_ADPCM : Return "Rockwell ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ROCKWELL_DIGITALK : Return "Rockwell Digit LK DIGITALK"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_XEBEC : Return "Xebec Proprietary Compression"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_G721_ADPCM : Return "Antex Electronics G.721"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_G728_CELP : Return "G.728 CELP"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MSG723 : Return "MSG723"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MPEG : Return "MPEG"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_RT24 : Return "Voxware MetaVoice MSRT24"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_PAC : Return "PAC"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MPEGLAYER3 : Return "ISO/MPEG Layer3"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_LUCENT_G723 : Return "Lucent G.723"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_CIRRUS : Return "Cirrus"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ESPCM : Return "ESPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE : Return "Voxware (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVEFORMAT_CANOPUS_ATRAC : Return "Canopus Atrac"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_G726_ADPCM : Return "G.726 ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_G722_ADPCM : Return "G.722 ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DSAT : Return "DSAT"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DSAT_DISPLAY : Return "DSAT Display"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_BYTE_ALIGNED : Return "Voxware Byte Aligned (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_AC8 : Return "Voxware AC8 (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_AC10 : Return "Voxware AC10 (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_AC16 : Return "Voxware AC16 (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_AC20 : Return "Voxware AC20 (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_RT24 : Return "Voxware MetaVoice RT24"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_RT29 : Return "Voxware MetaSound RT29"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_RT29HW : Return "Voxware MetaSound Hardware RT29HW (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_VR12 : Return "Voxware VR12 (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_VR18 : Return "Voxware VR18 (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_TQ40 : Return "Voxware TQ40 (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_SOFTSOUND : Return "Softsound"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VOXWARE_TQ60 : Return "Voxware TQ60 (Obsolete)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MSRT24 : Return "Voxware MetaVoice MSRT24"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_G729A : Return "AT&T G.729A"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MVI_MV12 : Return "MVI MV12"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DF_G726 : Return "DataFusion G.726"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DF_GSM610 : Return "DataFusion GSM610"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ISIAUDIO : Return "Iterated Systems, Inc. Audio"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ONLIVE : Return "OnLive!"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_SBC24 : Return "Siemens Business Communications Systems SBC24"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DOLBY_AC3_SPDIF : Return "Dolby AC3 SPDIF"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ZYXEL_ADPCM : Return "ZyXEL ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_PHILIPS_LPCBB : Return "Philips LPCBB"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_PACKED : Return "Packed"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_RAW_AAC1 : Return "Advanced Audio Coding"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_RHETOREX_ADPCM : Return "Rhetorex ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_IRAT : Return "BeCubed Software's IRAT"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VIVO_G723 : Return "Vivo G.723"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VIVO_SIREN : Return "Vivo Siren"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DIGITAL_G723 : Return "Digital G.723"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_WMAUDIO2 : Return "WMA 8/9"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_WMAUDIO3 : Return "WMA 9 Professional"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_WMAUDIO_LOSSLESS : Return "WMA 9 Lossless"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_WMASPDIF : Return "WMA over S/PDIF"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_CREATIVE_ADPCM : Return "Creative ADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_CREATIVE_FASTSPEECH8 : Return "Creative FastSpeech8"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_CREATIVE_FASTSPEECH10 : Return "Creative FastSpeech10"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_QUARTERDECK : Return "Quarterdeck"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_RAW_SPORT : Return "AC-3 over S/PDIF"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ESST_AC3 : Return "AC-3 over S/PDIF"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_FM_TOWNS_SND : Return "Fujitsu FM Towns Snd"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_BTV_DIGITAL : Return "Brooktree digital audio"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_VME_VMPCM : Return "AT&T VME VMPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_OLIGSM : Return "OLIGSM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_OLIADPCM : Return "OLIADPCM"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_OLICELP : Return "OLICELP"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_OLISBC : Return "OLISBC"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_OLIOPR : Return "OLIOPR"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_LH_CODEC : Return "Lernout & Hauspie"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_NORRIS : Return "Norris"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_ISIAUDIO2 : Return "AT&T ISIAudio"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS : Return "Soundspace Music Compression"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MPEG_ADTS_AAC : Return "DTS Advanced Audio Coding"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MPEG_LOAS : Return "MPEG-4 Audio with Synchronization and Multiplex Layers"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_MPEG_HEAAC : Return "MPEG Advanced Audio Coding"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DVM : Return "Dolby Digital AC-3"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_DTS2 : Return "Digital Theater Systems (DTS)"
      Case clsRIFF.WAVFormatTag.WAVE_FORMAT_EXTENSIBLE : Return "WAVE_FORMAT_EXTENSIBLE" : Debug.Print("Format Tag: WAVE_FORMAT_EXTENSIBLE")
      Case Else : Return "Unknown: " & CUInt(formatTag)
    End Select
  End Function
  Public Function MSToTime(uMS As UInt32) As String
    Dim sDur As String = uMS & "ms"
    Dim lH As UInt32 = 0
    Dim lM As UInt32 = 0
    Dim lS As UInt32 = 0
    Dim lMS As UInt32 = uMS
    While lMS > 1000
      While lMS > 1000 * 60
        While lMS > 1000 * 60 * 60
          lH += 1
          lMS -= 1000 * 60 * 60
        End While
        lM += 1
        lMS -= 1000 * 60
      End While
      lS += 1
      lMS -= 1000
    End While
    If lH > 0 Then
      sDur = lH & ":" & Format(lM, "00") & ":" & Format(lS, "00") & "." & Format(lMS, "000")
    ElseIf lM > 0 Then
      sDur = lM & ":" & Format(lS, "00") & "." & Format(lMS, "000")
    ElseIf lS > 0 Then
      sDur = lS & "." & Format(lMS, "000")
    End If
    Return sDur
  End Function
  Public Sub OpenURL(sURL As String, parent As Form)
    Dim sOpen As String = sURL
    If Not sOpen.Contains(Uri.SchemeDelimiter) Then sOpen = "http://" & sURL
    Try
      Process.Start(sOpen)
    Catch ex As Exception
      Try
        Clipboard.SetText(sOpen)
      Catch ex2 As Exception
        SuperMsgBox(parent, "Error Opening Browser", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Error, "Failed to open your Browser.", My.Application.Info.ProductName & " could not navigate to """ & sURL & """!", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok, ex.Message)
        Return
      End Try
      SuperMsgBox(parent, "URL Copied to Clipboard", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information, "Failed to open your Browser.", My.Application.Info.ProductName & " could not navigate to """ & sURL & """, so the URL was copied to your clipboard!", Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok)
    End Try
  End Sub
End Module
