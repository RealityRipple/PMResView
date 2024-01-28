Public Class clsRIFF
  Implements IDisposable
  Private Structure ChunkHeader
    Public ChunkID As String
    Public ChunkSize As UInt32
    Public Format As String
  End Structure
  Private Structure Chunk
    Public Header As ChunkHeader
    Public Data As Byte()
  End Structure
  Private bValid As Boolean
  Private bDTS As Boolean
  Private bWAV As Boolean

  Public Structure WAVEFORMAT
    Public wFormatTag As WAVFormatTag
    Public nChannels As UInt16
    Public nSamplesPerSec As UInt32
    Public nAvgBytesPerSec As UInt32
    Public nBlockAlign As UInt16
  End Structure

  Public Structure WAVEFORMATEX
    Public Format As WAVEFORMAT
    Public wBitsPerSample As UInt16
    Public cbSize As UInt16
  End Structure

  Public Structure WAVEFORMATEXTENSIBLE_SAMPLES
    Public wValidBitsPerSample As UInt16
    Public wSamplesPerBlock As UInt16
    Public wReserved As UInt16
  End Structure

  Public Structure WAVEFORMATEXTENSIBLE
    Public Format As WAVEFORMATEX
    Public Samples As WAVEFORMATEXTENSIBLE_SAMPLES
    Public dwChannelMask As UInt32
    Public SubFormat As Guid
  End Structure

  Public Enum WAVFormatTag As UInt16
    WAVE_FORMAT_UNKNOWN = &H0
    WAVE_FORMAT_PCM = &H1
    WAVE_FORMAT_ADPCM = &H2
    WAVE_FORMAT_IEEE_FLOAT = &H3
    WAVE_FORMAT_VSELP = &H4
    WAVE_FORMAT_IBM_CVSD = &H5
    WAVE_FORMAT_ALAW = &H6
    WAVE_FORMAT_MULAW = &H7
    WAVE_FORMAT_DTS = &H8
    WAVE_FORMAT_DRM = &H9
    WAVE_FORMAT_WMAVOICE9 = &HA
    WAVE_FORMAT_OKI_ADPCM = &H10
    WAVE_FORMAT_DVI_ADPCM = &H11
    WAVE_FORMAT_MEDIASPACE_ADPCM = &H12
    WAVE_FORMAT_SIERRA_ADPCM = &H13
    WAVE_FORMAT_G723_ADPCM = &H14
    WAVE_FORMAT_DIGISTD = &H15
    WAVE_FORMAT_DIGIFIX = &H16
    WAVE_FORMAT_DIALOGIC_OKI_ADPCM = &H17
    WAVE_FORMAT_MEDIAVISION_ADPCM = &H18
    WAVE_FORMAT_CU_CODEC = &H19
    WAVE_FORMAT_YAMAHA_ADPCM = &H20
    WAVE_FORMAT_SONARC = &H21
    WAVE_FORMAT_DSPGROUP_TRUESPEECH = &H22
    WAVE_FORMAT_ECHOSC1 = &H23
    WAVE_FORMAT_AUDIOFILE_AF36 = &H24
    WAVE_FORMAT_APTX = &H25
    WAVE_FORMAT_AUDIOFILE_AF10 = &H26
    WAVE_FORMAT_PROSODY_1612 = &H27
    WAVE_FORMAT_LRC = &H28
    WAVE_FORMAT_DOLBY_AC2 = &H30
    WAVE_FORMAT_GSM610 = &H31
    WAVE_FORMAT_MSNAUDIO = &H32
    WAVE_FORMAT_ANTEX_ADPCME = &H33
    WAVE_FORMAT_CONTROL_RES_VQLPC = &H34
    WAVE_FORMAT_DIGIREAL = &H35
    WAVE_FORMAT_DIGIADPCM = &H36
    WAVE_FORMAT_CONTROL_RES_CR10 = &H37
    WAVE_FORMAT_NMS_VBXADPCM = &H38
    WAVE_FORMAT_CS_IMAADPCM = &H39
    WAVE_FORMAT_ECHOSC3 = &H3A
    WAVE_FORMAT_ROCKWELL_ADPCM = &H3B
    WAVE_FORMAT_ROCKWELL_DIGITALK = &H3C
    WAVE_FORMAT_XEBEC = &H3D
    WAVE_FORMAT_G721_ADPCM = &H40
    WAVE_FORMAT_G728_CELP = &H41
    WAVE_FORMAT_MSG723 = &H42
    WAVE_FORMAT_MPEG = &H50
    WAVE_FORMAT_RT24 = &H52
    WAVE_FORMAT_PAC = &H53
    WAVE_FORMAT_MPEGLAYER3 = &H55
    WAVE_FORMAT_LUCENT_G723 = &H59
    WAVE_FORMAT_CIRRUS = &H60
    WAVE_FORMAT_ESPCM = &H61
    WAVE_FORMAT_VOXWARE = &H62
    WAVEFORMAT_CANOPUS_ATRAC = &H63
    WAVE_FORMAT_G726_ADPCM = &H64
    WAVE_FORMAT_G722_ADPCM = &H65
    WAVE_FORMAT_DSAT = &H66
    WAVE_FORMAT_DSAT_DISPLAY = &H67
    WAVE_FORMAT_VOXWARE_BYTE_ALIGNED = &H69
    WAVE_FORMAT_VOXWARE_AC8 = &H70
    WAVE_FORMAT_VOXWARE_AC10 = &H71
    WAVE_FORMAT_VOXWARE_AC16 = &H72
    WAVE_FORMAT_VOXWARE_AC20 = &H73
    WAVE_FORMAT_VOXWARE_RT24 = &H74
    WAVE_FORMAT_VOXWARE_RT29 = &H75
    WAVE_FORMAT_VOXWARE_RT29HW = &H76
    WAVE_FORMAT_VOXWARE_VR12 = &H77
    WAVE_FORMAT_VOXWARE_VR18 = &H78
    WAVE_FORMAT_VOXWARE_TQ40 = &H79
    WAVE_FORMAT_SOFTSOUND = &H80
    WAVE_FORMAT_VOXWARE_TQ60 = &H81
    WAVE_FORMAT_MSRT24 = &H82
    WAVE_FORMAT_G729A = &H83
    WAVE_FORMAT_MVI_MV12 = &H84
    WAVE_FORMAT_DF_G726 = &H85
    WAVE_FORMAT_DF_GSM610 = &H86
    WAVE_FORMAT_ISIAUDIO = &H88
    WAVE_FORMAT_ONLIVE = &H89
    WAVE_FORMAT_SBC24 = &H91
    WAVE_FORMAT_DOLBY_AC3_SPDIF = &H92
    WAVE_FORMAT_ZYXEL_ADPCM = &H97
    WAVE_FORMAT_PHILIPS_LPCBB = &H98
    WAVE_FORMAT_PACKED = &H99
    WAVE_FORMAT_RAW_AAC1 = &HFF
    WAVE_FORMAT_RHETOREX_ADPCM = &H100
    WAVE_FORMAT_IRAT = &H101
    WAVE_FORMAT_VIVO_G723 = &H111
    WAVE_FORMAT_VIVO_SIREN = &H112
    WAVE_FORMAT_DIGITAL_G723 = &H123
    WAVE_FORMAT_WMAUDIO2 = &H161
    WAVE_FORMAT_WMAUDIO3 = &H162
    WAVE_FORMAT_WMAUDIO_LOSSLESS = &H163
    WAVE_FORMAT_WMASPDIF = &H164
    WAVE_FORMAT_CREATIVE_ADPCM = &H200
    WAVE_FORMAT_CREATIVE_FASTSPEECH8 = &H202
    WAVE_FORMAT_CREATIVE_FASTSPEECH10 = &H203
    WAVE_FORMAT_QUARTERDECK = &H220
    WAVE_FORMAT_RAW_SPORT = &H240
    WAVE_FORMAT_ESST_AC3 = &H241
    WAVE_FORMAT_FM_TOWNS_SND = &H300
    WAVE_FORMAT_BTV_DIGITAL = &H400
    WAVE_FORMAT_VME_VMPCM = &H680
    WAVE_FORMAT_OLIGSM = &H1000
    WAVE_FORMAT_OLIADPCM = &H1001
    WAVE_FORMAT_OLICELP = &H1002
    WAVE_FORMAT_OLISBC = &H1003
    WAVE_FORMAT_OLIOPR = &H1004
    WAVE_FORMAT_LH_CODEC = &H1100
    WAVE_FORMAT_NORRIS = &H1400
    WAVE_FORMAT_ISIAUDIO2 = &H1401
    WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS = &H1500
    WAVE_FORMAT_MPEG_ADTS_AAC = &H1600
    WAVE_FORMAT_MPEG_LOAS = &H1602
    WAVE_FORMAT_MPEG_HEAAC = &H1610
    WAVE_FORMAT_DVM = &H2000
    WAVE_FORMAT_DTS2 = &H2001
    WAVE_FORMAT_EXTENSIBLE = &HFFFE
    WAVE_FORMAT_DEVELOPMENT = &HFFFF
  End Enum

  Public Class StringValueAttribute
    Inherits Attribute
    Public Property Value As String
    Public Sub New(ByVal val As String)
      Value = val
    End Sub
    Public Overrides Function ToString() As String
      Return Value
    End Function
  End Class


  Public Enum ChannelStruct As UInt32
    FrontLeft = &H1
    FrontRight = &H2
    FrontCenter = &H4
    LFE = &H8
    RearLeft = &H10
    RearRight = &H20
    FrontCenterLeft = &H40
    FrontCenterRight = &H80
    RearCenter = &H100
    SideLeft = &H200
    SideRight = &H400
    TopCenter = &H800
    TopFrontLeft = &H1000
    TopFrontCenter = &H2000
    TopFrontRight = &H4000
    TopRearLeft = &H8000
    TopRearCenter = &H10000
    TopRearRight = &H20000
  End Enum

  Public Structure DTSInfo
    Public uSYNC As UInt32
    Public bFTYPE As Boolean
    Public iSHORT As UInt16
    Public bCPF As Boolean
    Public iNBLKS As UInt16
    Public iFSIZE As UInt16
    Public iAMODE As UInt16
    Public iSFREQ As UInt16
    Public iRATE As UInt16
    Public bFixedBit As Boolean
    Public bDYNF As Boolean
    Public bTIMEF As Boolean
    Public bAUXF As Boolean
    Public bHDCD As Boolean
    Public iEXT_AUDIO_ID As UInt16
    Public bEXT_AUDIO As Boolean
    Public bASPF As Boolean
    Public iLFF As UInt16
    Public bHFLAG As Boolean
    Public iHCRC As UInt16
    Public bFILTS As Boolean
    Public iVERNUM As UInt16
    Public iCHIST As UInt16
    Public iPCMR As UInt16
    Public bSUMF As Boolean
    Public bSUMS As Boolean
    Public iDIALNORM As UInt16
    Public iDNG As Integer
  End Structure

  Private wfEX As WAVEFORMATEXTENSIBLE
  Private dtsEX As DTSInfo

  Public ReadOnly Property WAVData As WAVEFORMATEXTENSIBLE
    Get
      Return wfEX
    End Get
  End Property

  Public ReadOnly Property DTSData As DTSInfo
    Get
      Return dtsEX
    End Get
  End Property

  Public Sub New(bData() As Byte)
    bValid = False
    Using ioFile As New IO.BinaryReader(New IO.MemoryStream(bData))
      Dim mChunk As New Chunk
      mChunk.Header.ChunkID = ioFile.ReadChars(4)
      If Not mChunk.Header.ChunkID = "RIFF" Then Return
      mChunk.Header.ChunkSize = ioFile.ReadUInt32
      mChunk.Header.Format = ioFile.ReadChars(4)
      Select Case mChunk.Header.Format
        Case "WAVE"
          'WAVEFORMAT
          Do While ioFile.BaseStream.Position < mChunk.Header.ChunkSize
            Dim wavChunk As New Chunk
            wavChunk.Header.ChunkID = ioFile.ReadChars(4)
            wavChunk.Header.ChunkSize = ioFile.ReadUInt32
            wavChunk.Data = ioFile.ReadBytes(wavChunk.Header.ChunkSize)
            Using ioData As New IO.BinaryReader(New IO.MemoryStream(wavChunk.Data))
              Select Case wavChunk.Header.ChunkID
                Case "fmt "
                  wfEX = New WAVEFORMATEXTENSIBLE
                  Select Case wavChunk.Header.ChunkSize
                    Case 16 'WAVEFORMAT
                      wfEX.Format.Format.wFormatTag = ioData.ReadUInt16
                      wfEX.Format.Format.nChannels = ioData.ReadUInt16
                      wfEX.Format.Format.nSamplesPerSec = ioData.ReadUInt32
                      wfEX.Format.Format.nAvgBytesPerSec = ioData.ReadUInt32
                      wfEX.Format.Format.nBlockAlign = ioData.ReadUInt16
                      bWAV = True
                      bValid = True
                    Case 18, 20 ' WAVEFORMATEX
                      wfEX.Format.Format.wFormatTag = ioData.ReadUInt16
                      wfEX.Format.Format.nChannels = ioData.ReadUInt16
                      wfEX.Format.Format.nSamplesPerSec = ioData.ReadUInt32
                      wfEX.Format.Format.nAvgBytesPerSec = ioData.ReadUInt32
                      wfEX.Format.Format.nBlockAlign = ioData.ReadUInt16
                      wfEX.Format.wBitsPerSample = ioData.ReadUInt16
                      wfEX.Format.cbSize = ioData.ReadUInt16
                      bWAV = True
                      bValid = True
                    Case 40 'WAVEFORMATEXTENSIBLE
                      wfEX.Format.Format.wFormatTag = ioData.ReadUInt16
                      wfEX.Format.Format.nChannels = ioData.ReadUInt16
                      wfEX.Format.Format.nSamplesPerSec = ioData.ReadUInt32
                      wfEX.Format.Format.nAvgBytesPerSec = ioData.ReadUInt32
                      wfEX.Format.Format.nBlockAlign = ioData.ReadUInt16
                      wfEX.Format.wBitsPerSample = ioData.ReadUInt16
                      wfEX.Format.cbSize = ioData.ReadUInt16
                      wfEX.Samples.wValidBitsPerSample = ioData.ReadUInt16
                      wfEX.dwChannelMask = ioData.ReadUInt32
                      wfEX.SubFormat = New Guid(ioData.ReadBytes(16))
                      bWAV = True
                      bValid = True
                    Case Else
                      Debug.Print("Unkown Size " & wavChunk.Header.ChunkSize)
                      Return
                  End Select
                Case "data"
                  Dim firstFour As Byte() = ioData.ReadBytes(4)
                  Do While firstFour(0) = 0
                    ioData.BaseStream.Position -= 3
                    firstFour = ioData.ReadBytes(4)
                  Loop
                  Select Case BitConverter.ToString(firstFour)
                    Case "7F-FE-80-01" 'Raw Big Endian
                    Case "FE-7F-01-80" 'Raw Little Endian
                    Case "1F-FF-E8-00" '14-bit Big Endian
                    Case "FF-1F-00-E8" '14-bit Little Endian
                      ioData.BaseStream.Position -= 4
                      Dim bDTSa As Byte() = BytesTo14BitL(ioData.ReadBytes(24))
                      If bDTSa(0) = &H7F And bDTSa(1) = &HFE And bDTSa(2) = &H80 And bDTSa(3) = &H1 Then
                        bDTS = True
                        sizeLeft = 8
                        currentByte = bDTSa(0)
                        dtsEX.uSYNC = ReadBits(bDTSa, 32)
                        dtsEX.bFTYPE = ReadBits(bDTSa, 1) = 1
                        dtsEX.iSHORT = ReadBits(bDTSa, 5)
                        dtsEX.bCPF = ReadBits(bDTSa, 1) = 1
                        dtsEX.iNBLKS = ReadBits(bDTSa, 7)
                        dtsEX.iFSIZE = ReadBits(bDTSa, 14)
                        dtsEX.iAMODE = ReadBits(bDTSa, 6)
                        dtsEX.iSFREQ = ReadBits(bDTSa, 4)
                        dtsEX.iRATE = ReadBits(bDTSa, 5)
                        dtsEX.bFixedBit = ReadBits(bDTSa, 1) = 1
                        dtsEX.bDYNF = ReadBits(bDTSa, 1) = 1
                        dtsEX.bTIMEF = ReadBits(bDTSa, 1) = 1
                        dtsEX.bAUXF = ReadBits(bDTSa, 1) = 1
                        dtsEX.bHDCD = ReadBits(bDTSa, 1) = 1
                        dtsEX.iEXT_AUDIO_ID = ReadBits(bDTSa, 3)
                        dtsEX.bEXT_AUDIO = ReadBits(bDTSa, 1) = 1
                        dtsEX.bASPF = ReadBits(bDTSa, 1) = 1
                        dtsEX.iLFF = ReadBits(bDTSa, 2)
                        dtsEX.bHFLAG = ReadBits(bDTSa, 1) = 1
                        If dtsEX.bCPF Then dtsEX.iHCRC = ReadBits(bDTSa, 16)
                        dtsEX.bFILTS = ReadBits(bDTSa, 1)
                        dtsEX.iVERNUM = ReadBits(bDTSa, 4)
                        dtsEX.iCHIST = ReadBits(bDTSa, 2)
                        dtsEX.iPCMR = ReadBits(bDTSa, 3)
                        dtsEX.bSUMF = ReadBits(bDTSa, 1) = 1
                        dtsEX.bSUMS = ReadBits(bDTSa, 1) = 1
                        dtsEX.iDIALNORM = ReadBits(bDTSa, 4)
                        Select Case dtsEX.iVERNUM
                          Case 6
                            dtsEX.iDNG = -1 * (16 + DTSData.iDIALNORM)
                          Case 7
                            dtsEX.iDNG = -1 * DTSData.iDIALNORM
                          Case Else
                            dtsEX.iDNG = 0
                            dtsEX.iDIALNORM = 0
                        End Select
                      Else
                        bDTS = False
                        Return
                      End If
                    Case Else
                      ioData.BaseStream.Position -= 4
                      Dim bDTSa As Byte() = BytesTo14BitL(ioData.ReadBytes(16))
                      Debug.Print("Unknown Data ID: " & Hex(bDTSa(0)) & Hex(bDTSa(1)) & Hex(bDTSa(2)) & Hex(bDTSa(3)))
                  End Select
                  Exit Do
                Case Else
                  Debug.Print("Unknown Chunk ID: " & wavChunk.Header.ChunkID)
              End Select
            End Using
          Loop
        Case Else
          Debug.Print("Unknown RIFF Format: " & mChunk.Header.Format)
          Return
      End Select
    End Using
  End Sub

  Public ReadOnly Property IsValid As Boolean
    Get
      Return bValid
    End Get
  End Property

  Public ReadOnly Property IsDTS As Boolean
    Get
      Return bDTS
    End Get
  End Property

  Public ReadOnly Property IsWAV As Boolean
    Get
      Return bWAV
    End Get
  End Property

  Private Function BytesTo14BitL(inBytes As Byte()) As Byte()
    Dim bitPairs As Byte() = Nothing
    Dim j As Integer = 0
    For I As Integer = 0 To inBytes.Count - 1 Step 2
      Dim b1 As Byte = inBytes(I)
      Dim b0 As Byte = inBytes(I + 1)
      ReDim Preserve bitPairs(j + 6)
      bitPairs(j) = (b0 And &H30) >> 4
      bitPairs(j + 1) = (b0 And &HC) >> 2
      bitPairs(j + 2) = (b0 And &H3)
      bitPairs(j + 3) = (b1 And &HC0) >> 6
      bitPairs(j + 4) = (b1 And &H30) >> 4
      bitPairs(j + 5) = (b1 And &HC) >> 2
      bitPairs(j + 6) = (b1 And &H3)
      j += 7
    Next
    Dim bytes(bitPairs.Count / 4 - 1) As Byte
    j = 0
    For I As Integer = 0 To bitPairs.Count - 1 Step 4
      bytes(j) = (bitPairs(I) << 6) + (bitPairs(I + 1) << 4) + (bitPairs(I + 2) << 2) + (bitPairs(I + 3))
      j += 1
    Next
    Return bytes
  End Function

  Private sizeLeft As Integer
  Private currentByte As Byte
  Private idx As Integer
  Private Function ReadBits(bData As Byte(), size As Integer) As UInt32
    Dim ret As UInt32 = 0
    If (size <= sizeLeft) Then
      sizeLeft -= size
      ret = (currentByte >> sizeLeft) And (Math.Pow(2, size) - 1)
    Else
      Dim oSize As Integer = sizeLeft
      ret = ReadBits(bData, sizeLeft) << size - oSize
      ret = ret Or ReadBits(bData, size - oSize)
    End If
    If sizeLeft = 0 Then
      idx += 1
      currentByte = bData(idx)
      sizeLeft = 8
    End If
    Return ret
  End Function

#Region "IDisposable Support"
  Private disposedValue As Boolean
  Protected Overridable Sub Dispose(disposing As Boolean)
    If Not Me.disposedValue Then
      If disposing Then
      End If
    End If
    Me.disposedValue = True
  End Sub

  Public Sub Dispose() Implements IDisposable.Dispose
    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    Dispose(True)
    GC.SuppressFinalize(Me)
  End Sub
#End Region

End Class
