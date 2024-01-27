Public Class ZIP
  Public Structure FileFlags
    Public Encrypted As Boolean
    Public Compression1 As Boolean
    Public Compression2 As Boolean
    Public DescriptorRequired As Boolean
    Public EnhancedDeflate As Boolean
    Public Patched As Boolean
    Public StrongEncrypted As Boolean
    Public Unused1 As Boolean
    Public Unused2 As Boolean
    Public Unused3 As Boolean
    Public Unused4 As Boolean
    Public Unused5 As Boolean
    Public EnhancedCompress As Boolean
    Public MaskedHeaders As Boolean
    Public Reserved1 As Boolean
    Public Reserved2 As Boolean
    Public Sub New(flags As UInt16)
      Encrypted = ((flags And &H1) = &H1)
      Compression1 = ((flags And &H2) = &H2)
      Compression2 = ((flags And &H4) = &H4)
      DescriptorRequired = ((flags And &H8) = &H8)
      EnhancedDeflate = ((flags And &H10) = &H10)
      Patched = ((flags And &H20) = &H20)
      StrongEncrypted = ((flags And &H40) = &H40)
      Unused1 = ((flags And &H80) = &H80)
      Unused2 = ((flags And &H100) = &H100)
      Unused3 = ((flags And &H200) = &H200)
      Unused4 = ((flags And &H400) = &H400)
      Unused5 = ((flags And &H800) = &H800)
      EnhancedCompress = ((flags And &H1000) = &H1000)
      MaskedHeaders = ((flags And &H2000) = &H2000)
      Reserved1 = ((flags And &H4000) = &H4000)
      Reserved2 = ((flags And &H8000) = &H8000)
    End Sub
  End Structure
  Public Class FileSystemEntry
    Public Index As Integer
    Public Name As String
    Public CompressedLength As UInt64
    Public UncompressedLength As UInt64
  End Class
  Public Class FileSystemFile
    Inherits FileSystemEntry
    Public FileType As String
    Public Compression As UInt32
    Public Offset As UInt32
    Public CRC As UInt32
    Public Flags As FileFlags
    Public Modified As Date
    Public Data As Byte()
  End Class
  Public Class FileSystemDirectory
    Inherits FileSystemEntry
    Public FileCount As UInt64
    Public DirectoryCount As UInt64
  End Class

  Private sFiles As List(Of ZIP.FileSystemEntry)

  Public Sub New()
    sFiles = New List(Of ZIP.FileSystemEntry)
  End Sub

  Public Delegate Sub ProgressInvoker(ByVal bPercent As Byte, ByVal sName As String)

#Region "Read Functions"
  Public Shared Function ReadFileSystem(bData As Byte(), Optional ProgressViewer As ProgressInvoker = Nothing) As ZIP.FileSystemEntry()
    Dim pCount As UInt64 = 0
    Dim lStart As Long = 0
    For I As Long = 0 To bData.LongLength - 5
      Dim iDWORD As UInt32 = BitConverter.ToUInt32(bData, I)
      Select Case iDWORD
        'Case &H4034B50
        '  'file
        '  Exit For
        'Case &H2014B50
        '  'cdr entry
        '  'Application.DoEvents()
        '  'pCount = BitConverter.ToUInt16(bData, I + 8)
        '  pCount += 1
        '  'Exit For
        Case &H6054B50
          pCount = BitConverter.ToUInt16(bData, I + 8)
          lStart = I + 4
          'eocdr
          Exit For
      End Select
    Next
    If pCount < 1 Then Return New ZIP.FileSystemEntry(-1) {}
    If pCount > 1 Then
      If ProgressViewer IsNot Nothing Then ProgressViewer.Invoke(0, Nothing)
      Application.DoEvents()
    End If
    Dim sFiles As New List(Of ZIP.FileSystemEntry)
    Dim pIDX As Integer = 0
    For I As Long = lStart To bData.LongLength - 5
      Dim iDWORD As UInt32 = BitConverter.ToUInt32(bData, I)
      Select Case iDWORD
        Case &H4034B50
          pIDX += 1
          Dim zFile As ZIP.FileSystemFile = ParseFileSystemInfo(bData, I)
          If zFile Is Nothing Then
            If pCount > 1 Then
              ProgressViewer.Invoke(0, "Error")
            End If
            Return New ZIP.FileSystemEntry(-1) {}
          End If
          If pCount > 1 Then ProgressViewer.Invoke(Math.Floor((pIDX / pCount) * 100), zFile.Name)
          Application.DoEvents()
          zFile.Index = pIDX
          sFiles.Add(zFile)
        Case &H2014B50
          'cdr entry
          Exit For
        Case &H6054B50
          'eocdr
          Exit For
      End Select
    Next
    Dim dIDX As Integer = 0
    Dim sDirStructure As New Dictionary(Of String, ZIP.FileSystemDirectory)
    For I As Long = 0 To sFiles.LongCount - 1
      Dim sDirs() As String = Split("ROOT" & IO.Path.GetDirectoryName(sFiles(I).Name), IO.Path.DirectorySeparatorChar)
      For J As Long = 0 To sDirs.LongLength - 1
        If String.IsNullOrEmpty(sDirs(J)) Then Continue For
        Dim sDir As String = ""
        For K As Long = 0 To J
          sDir = IO.Path.Combine(sDir, sDirs(K))
        Next
        Dim sName As String = sDir
        If sName = "ROOT" Then
          sName = IO.Path.DirectorySeparatorChar
        Else
          If Not sName.Substring(0, 5) = "ROOT" & IO.Path.DirectorySeparatorChar Then Stop
          sName = sName.Substring(4)
        End If
        If Not sDirStructure.ContainsKey(sDir) Then sDirStructure.Add(sDir, New ZIP.FileSystemDirectory() With {.Name = sName})
        sDirStructure(sDir).FileCount += 1
        sDirStructure(sDir).CompressedLength += sFiles(I).CompressedLength
        sDirStructure(sDir).UncompressedLength += sFiles(I).UncompressedLength
      Next
    Next
    For I As Long = 0 To sDirStructure.Keys.LongCount - 1
      Dim dDir As ZIP.FileSystemDirectory = sDirStructure(sDirStructure.Keys(I))
      dDir.Index = I
      dDir.DirectoryCount = countSubdirsOf(dDir.Name, sDirStructure.Keys.ToArray)
      sFiles.Add(dDir)
    Next
    If pCount > 1 Then
      ProgressViewer.Invoke(100, Nothing)
    End If
    Return sFiles.ToArray
  End Function

  Private Shared Function countSubdirsOf(sPath As String, sDirs As String()) As Long
    If sPath = IO.Path.DirectorySeparatorChar Then
      Dim rUnique As New List(Of String)
      For I As Long = 0 To sDirs.LongLength - 1
        Dim sTest As String() = Split(sDirs(I), IO.Path.DirectorySeparatorChar)
        Dim sCheck As String = ""
        For J As Long = 0 To sTest.LongLength - 1
          sCheck &= sTest(J) & IO.Path.DirectorySeparatorChar
          If Not rUnique.Contains(sCheck) Then rUnique.Add(sCheck)
        Next
      Next
      If rUnique.Contains("ROOT" & IO.Path.DirectorySeparatorChar) Then rUnique.Remove("ROOT" & IO.Path.DirectorySeparatorChar)
      Return rUnique.LongCount
    End If
    Dim ret As Long = 0
    For I As Long = 0 To sDirs.LongLength - 1
      Dim sTest As String() = Split(sDirs(I), IO.Path.DirectorySeparatorChar)
      Dim sCheck As String = ""
      For J As Long = 0 To sTest.LongLength - 2
        sCheck &= sTest(J) & IO.Path.DirectorySeparatorChar
        If "ROOT" & sPath & IO.Path.DirectorySeparatorChar = sCheck Then
          ret += 1
          Exit For
        End If
      Next
    Next
    Return ret
  End Function

  Private Shared Function ParseFileSystemInfo(bData As Byte(), ByRef iStart As Long) As ZIP.FileSystemFile
    Dim zFile As New ZIP.FileSystemFile
    zFile.Name = Nothing
    zFile.CRC = 0
    zFile.Offset = 0
    zFile.Compression = 0
    zFile.CompressedLength = 0
    zFile.UncompressedLength = 0
    zFile.Flags = New FileFlags(0)
    zFile.FileType = "File"
    Dim iPos As UInt64 = iStart
    If bData.LongLength <= iPos + 4 Then Return Nothing
    Dim iDWORD As UInt32 = BitConverter.ToUInt32(bData, iPos) : iPos += 4
    If Not iDWORD = &H4034B50 Then Return Nothing
    zFile.Offset = iStart
    If bData.LongLength <= iPos + 2 Then Return Nothing
    Dim iVer As UInt16 = BitConverter.ToUInt16(bData, iPos) : iPos += 2
    If (iVer And &HFF) > &H14 Then
      '0x0A and 0x14
      Return Nothing
    End If
    If bData.LongLength <= iPos + 2 Then Return Nothing
    Dim iGenFlags As UInt16 = BitConverter.ToUInt16(bData, iPos) : iPos += 2
    If (iGenFlags And &HFFFD) > 0 Then
      'Compression1 on or off only
      Return Nothing
    End If
    zFile.Flags = New FileFlags(iGenFlags)
    If bData.LongLength <= iPos + 2 Then Return Nothing
    Dim iCompress As UInt16 = BitConverter.ToUInt16(bData, iPos) : iPos += 2
    If Not (iCompress = 0 Or iCompress = &H81) Then
      'No Compression or 0x81
      Return Nothing
    End If
    zFile.Compression = iCompress
    If bData.LongLength <= iPos + 2 Then Return Nothing
    Dim iModTime As UInt16 = BitConverter.ToUInt16(bData, iPos) : iPos += 2
    If Not iModTime = 0 Then
      'Time Value not expected
      Return Nothing
    End If
    If bData.LongLength <= iPos + 2 Then Return Nothing
    Dim iModDate As UInt16 = BitConverter.ToUInt16(bData, iPos) : iPos += 2
    If Not iModDate = 15393 Then
      'Date Value not expected
      Return Nothing
    End If
    zFile.Modified = MSDOSToDateTime(iModDate, iModTime)
    If bData.LongLength <= iPos + 4 Then Return Nothing
    Dim iCRC As UInt32 = BitConverter.ToUInt32(bData, iPos) : iPos += 4
    zFile.CRC = iCRC
    If bData.LongLength <= iPos + 4 Then Return Nothing
    Dim iCompressed As UInt32 = BitConverter.ToUInt32(bData, iPos) : iPos += 4
    zFile.CompressedLength = iCompressed
    If bData.LongLength <= iPos + 4 Then Return Nothing
    Dim iUncompressed As UInt32 = BitConverter.ToUInt32(bData, iPos) : iPos += 4
    zFile.UncompressedLength = iUncompressed
    If bData.LongLength <= iPos + 2 Then Return Nothing
    Dim iFileName As UInt16 = BitConverter.ToUInt16(bData, iPos) : iPos += 2
    If iFileName < 1 Then Return Nothing
    If bData.LongLength <= iPos + 2 Then Return Nothing
    Dim iExtraField As UInt16 = BitConverter.ToUInt16(bData, iPos) : iPos += 2
    If iExtraField > 0 Then
      'Extras not expected
      Return Nothing
    End If
    If bData.LongLength <= iPos + iFileName Then Return Nothing
    Dim sFileName As String = System.Text.Encoding.UTF8.GetString(bData, iPos, iFileName) : iPos += iFileName
    zFile.Name = IO.Path.DirectorySeparatorChar & sFileName.Replace("/", IO.Path.DirectorySeparatorChar)
    If Not String.IsNullOrEmpty(zFile.Name) Then zFile.FileType = GetRegType(IO.Path.GetExtension(zFile.Name))

    If bData.LongLength <= iPos + iExtraField Then Return Nothing
    iPos += iExtraField

    If iCompress = 0 Then
      Dim bFile(iCompressed - 1) As Byte
      Array.ConstrainedCopy(bData, iPos, bFile, 0, iCompressed) : iPos += iCompressed
      If Not bFile.LongLength = iUncompressed Then
        'Length Mismatch
        Return Nothing
      End If
      zFile.Data = bFile
    Else
      Dim bUnbrot() As Byte = BrotliSharpLib.Brotli.DecompressBuffer(bData, iPos, iCompressed) : iPos += iCompressed
      If Not bUnbrot.LongLength = iUncompressed Then
        'Length Mismatch
        Return Nothing
      End If
      zFile.Data = bUnbrot
    End If
    Dim crcCheck As UInteger = CRC32.ComputeChecksum(zFile.Data)
    If Not crcCheck = iCRC Then
      'CRC failure
      Return Nothing
    End If
    iStart = iPos - 1

    Return zFile
  End Function

#End Region

#Region "Basic Functions"
  Private Shared Function TimeToMSDOS(dTime As Date) As UInt16
    Dim iHour As UInt16 = dTime.Hour
    Dim iMinute As UInt16 = dTime.Minute
    Dim iSecond As UInt16 = Math.Ceiling(dTime.Second / 2)
    Dim iTime As UInt16 = 0
    iTime = iTime Or (iHour And &H1F) << 11
    iTime = iTime Or (iMinute And &H3F) << 5
    iTime = iTime Or (iSecond And &H1F)
    Return iTime
  End Function

  Private Shared Function DateToMSDOS(dDate As Date) As UInt16
    If dDate.Year < 1980 Then Return 0
    If dDate.Year > 2107 Then Return 0
    Dim iYear As UInt16 = dDate.Year - 1980
    Dim iMonth As UInt16 = dDate.Month
    Dim iDay As UInt16 = dDate.Day
    Dim iDate As UInt16 = 0
    iDate = iDate Or (iYear And &H7F) << 9
    iDate = iDate Or (iMonth And &HF) << 5
    iDate = iDate Or (iDay And &H1F)
    Return iDate
  End Function

  Private Shared Function MSDOSToDateTime(iDate As UInt16, iTime As UInt16) As Date
    Dim iYear As UInt16 = (iDate And &HFE00) >> 9
    Dim iMonth As UInt16 = (iDate And &H1E0) >> 5
    Dim iDay As UInt16 = (iDate And &H1F)
    Dim iHour As UInt16 = (iTime And &HF800) >> 11
    Dim iMinute As UInt16 = (iTime And &H7E0) >> 5
    Dim iSecond As UInt16 = (iTime And &H1F)
    Return New Date(iYear + 1980, iMonth, iDay, iHour, iMinute, iSecond)
  End Function

  Private Class CRC32
    Shared table As UInteger()

    Shared Sub New()
      Dim poly As UInteger = &HEDB88320UI
      table = New UInteger(255) {}
      Dim temp As UInteger = 0
      For I As UInteger = 0 To table.Length - 1
        temp = I
        For J As Integer = 8 To 1 Step -1
          If (temp And 1) = 1 Then
            temp = CUInt((temp >> 1) Xor poly)
          Else
            temp >>= 1
          End If
        Next
        table(I) = temp
      Next
    End Sub

    Public Shared Function ComputeChecksum(bytes As Byte()) As UInteger
      Dim crc As UInteger = &HFFFFFFFFUI
      For i As Integer = 0 To bytes.Length - 1
        Dim index As Byte = CByte(((crc) And &HFF) Xor bytes(i))
        crc = CUInt((crc >> 8) Xor table(index))
      Next
      Return Not crc
    End Function
  End Class

  Private Shared KnownTypes As New Dictionary(Of String, String)
  Private Shared Function GetRegType(sExt As String) As String
    If String.IsNullOrEmpty(sExt) Then Return "File"
    sExt = sExt.ToLower
    If Not sExt.Substring(0, 1) = "." Then sExt = "." & sExt
    If KnownTypes.ContainsKey(sExt) Then Return KnownTypes(sExt)
    Dim rRegID As Microsoft.Win32.RegistryKey = My.Computer.Registry.ClassesRoot.OpenSubKey(sExt)
    If rRegID Is Nothing Then
      KnownTypes(sExt) = sExt.Substring(1).ToUpper & " File"
      Return sExt.Substring(1).ToUpper & " File"
    End If
    Dim sRegID As String = rRegID.GetValue("", "UNSET")
    If sRegID = "UNSET" Then
      KnownTypes(sExt) = sExt.Substring(1).ToUpper & " File"
      Return sExt.Substring(1).ToUpper & " File"
    End If
    Dim rRegVal As Microsoft.Win32.RegistryKey = My.Computer.Registry.ClassesRoot.OpenSubKey(sRegID)
    If rRegVal Is Nothing Then
      KnownTypes(sExt) = sExt.Substring(1).ToUpper & " File"
      Return sExt.Substring(1).ToUpper & " File"
    End If
    Dim sRegVal As String = rRegVal.GetValue("", "UNSET")
    If sRegVal = "UNSET" Then
      KnownTypes(sExt) = sExt.Substring(1).ToUpper & " File"
      Return sExt.Substring(1).ToUpper & " File"
    End If
    KnownTypes(sExt) = sRegVal
    Return sRegVal
  End Function

  Public Shared Function GetRegTypeForDirectory() As String
    If KnownTypes.ContainsKey("Directory") Then Return KnownTypes("Directory")
    Dim rRegVal As Microsoft.Win32.RegistryKey = My.Computer.Registry.ClassesRoot.OpenSubKey("Directory")
    If rRegVal Is Nothing Then
      KnownTypes("Directory") = "File Folder"
      Return "File Folder"
    End If
    Dim sRegVal As String = rRegVal.GetValue("", "UNSET")
    If sRegVal = "UNSET" Then
      KnownTypes("Directory") = "File Folder"
      Return "File Folder"
    End If
    KnownTypes("Directory") = sRegVal
    Return sRegVal
  End Function
#End Region
End Class
