Public Class clsFTYP
  Private Structure BOX
    Public Size As Int32
    Public Name As String
    Public Sub New(ByVal bData As Byte(), ByVal idx As Long)
      If bData.LongLength < idx + 8 Then Return
      Dim lLoc As Long = idx
      Size = ToInt32(bData, lLoc) : lLoc += 4
      If bData.LongLength < idx + Size Then Return
      Name = System.Text.Encoding.GetEncoding(LATIN_1).GetString(bData, lLoc, 4) : lLoc += 4
    End Sub
  End Structure
  Private Structure BOXftyp
    Public Size As Int32
    Public Name As String
    Public MajorBrand As String
    Public MinorVersion As UInt32
    Public CompatibleBrands As String()
    Public Sub New(ByVal bData As Byte(), ByVal idx As Long)
      If bData.LongLength < idx + 8 Then Return
      Dim lLoc As Long = idx
      Size = ToInt32(bData, lLoc) : lLoc += 4
      If bData.LongLength < idx + Size Then Return
      Name = System.Text.Encoding.GetEncoding(LATIN_1).GetString(bData, lLoc, 4) : lLoc += 4
      If Not Name = "ftyp" Then Return
      MajorBrand = System.Text.Encoding.GetEncoding(LATIN_1).GetString(bData, lLoc, 4) : lLoc += 4
      MinorVersion = ToUInt32(bData, lLoc) : lLoc += 4
      Dim brandList As New List(Of String)
      Do While lLoc < Size
        brandList.Add(System.Text.Encoding.GetEncoding(LATIN_1).GetString(bData, lLoc, 4)) : lLoc += 4
      Loop
      CompatibleBrands = brandList.ToArray
    End Sub
  End Structure
  Private Structure BOXmoov
    Public Size As Int32
    Public Name As String
    Public mvhd As BOXmvhd
    Public trak As BOXtrak
    Public Sub New(ByVal bData As Byte(), ByVal idx As Long)
      If bData.LongLength < idx + 8 Then Return
      Dim lLoc As Long = idx
      Size = ToInt32(bData, lLoc) : lLoc += 4
      If bData.LongLength < idx + Size Then Return
      Name = System.Text.Encoding.GetEncoding(LATIN_1).GetString(bData, lLoc, 4) : lLoc += 4
      If Not Name = "moov" Then Return
      Do While lLoc < idx + Size
        Dim mBox As New BOX(bData, lLoc)
        Select Case mBox.Name
          Case "mvhd"
            mvhd = New BOXmvhd(bData, lLoc)
          Case "trak"
            trak = New BOXtrak(bData, lLoc)
        End Select
        lLoc += mBox.Size
      Loop
    End Sub
  End Structure
  Private Structure BOXmvhd
    Public Size As Int32
    Public Name As String
    Public Version As Byte
    Public Flags As UInt32
    Public Created As UInt32
    Public Modified As UInt32
    Public Timescale As UInt32
    Public Duration As UInt32
    Public Rate As Decimal '32
    Public Volume As Decimal '16
    Public MatrixStruct As Byte() '36
    Public PreviewTime As UInt32
    Public PreviewDuration As UInt32
    Public PosterTime As UInt32
    Public SelectionTime As UInt32
    Public SelectionDuration As UInt32
    Public CurrentTime As UInt32
    Public NextTrackID As UInt32
    Public Sub New(ByVal bData As Byte(), ByVal idx As Long)
      If bData.LongLength < idx + 8 Then Return
      Dim lLoc As Long = idx
      Size = ToInt32(bData, lLoc) : lLoc += 4
      If bData.LongLength < idx + Size Then Return
      Name = System.Text.Encoding.GetEncoding(LATIN_1).GetString(bData, lLoc, 4) : lLoc += 4
      If Not Name = "mvhd" Then Return
      Version = bData(lLoc) : lLoc += 1
      Flags = ToUInt24(bData, lLoc) : lLoc += 3
      Created = ToUInt32(bData, lLoc) : lLoc += 4
      Modified = ToUInt32(bData, lLoc) : lLoc += 4
      Timescale = ToUInt32(bData, lLoc) : lLoc += 4
      Duration = ToUInt32(bData, lLoc) : lLoc += 4
      Rate = ToFixed32(bData, lLoc) : lLoc += 4
      Volume = ToFixed16(bData, lLoc) : lLoc += 2
      ReDim MatrixStruct(35)
      Array.ConstrainedCopy(bData, lLoc, MatrixStruct, 0, 36) : lLoc += 36
      PreviewTime = ToUInt32(bData, lLoc) : lLoc += 4
      PreviewDuration = ToUInt32(bData, lLoc) : lLoc += 4
      PosterTime = ToUInt32(bData, lLoc) : lLoc += 4
      SelectionTime = ToUInt32(bData, lLoc) : lLoc += 4
      SelectionDuration = ToUInt32(bData, lLoc) : lLoc += 4
      CurrentTime = ToUInt32(bData, lLoc) : lLoc += 4
      NextTrackID = ToUInt32(bData, lLoc) : lLoc += 4
    End Sub
  End Structure
  Private Structure BOXtrak
    Public Size As Int32
    Public Name As String
    Public tkhd As BOXtkhd
    Public Sub New(ByVal bData As Byte(), ByVal idx As Long)
      If bData.LongLength < idx + 8 Then Return
      Dim lLoc As Long = idx
      Size = ToInt32(bData, lLoc) : lLoc += 4
      If bData.LongLength < idx + Size Then Return
      Name = System.Text.Encoding.GetEncoding(LATIN_1).GetString(bData, lLoc, 4) : lLoc += 4
      If Not Name = "trak" Then Return
      Do While lLoc < idx + Size
        Dim mBox As New BOX(bData, lLoc)
        Select Case mBox.Name
          Case "tkhd"
            tkhd = New BOXtkhd(bData, lLoc)
        End Select
        lLoc += mBox.Size
      Loop
    End Sub
  End Structure
  Private Structure BOXtkhd
    Public Size As Int32
    Public Name As String
    Public Version As Byte
    Public Flags As UInt32
    Public Created As UInt32
    Public Modified As UInt32
    Public TrackID As UInt32
    Public Reserved As UInt32
    Public Duration As UInt32
    Public Reserved2 As UInt64
    Public Layer As UInt16
    Public AlternateGroup As UInt16
    Public Volume As Decimal '16
    Public Reserved3 As UInt16
    Public MatrixStruct As Byte() '36
    Public TrackWidth As Decimal '32
    Public TrackHeight As Decimal '32
    Public Sub New(ByVal bData As Byte(), ByVal idx As Long)
      If bData.LongLength < idx + 8 Then Return
      Dim lLoc As Long = idx
      Size = ToInt32(bData, lLoc) : lLoc += 4
      If bData.LongLength < idx + Size Then Return
      Name = System.Text.Encoding.GetEncoding(LATIN_1).GetString(bData, lLoc, 4) : lLoc += 4
      If Not Name = "tkhd" Then Return
      Version = bData(lLoc) : lLoc += 1
      Flags = ToUInt24(bData, lLoc) : lLoc += 3
      Created = ToUInt32(bData, lLoc) : lLoc += 4
      Modified = ToUInt32(bData, lLoc) : lLoc += 4
      TrackID = ToUInt32(bData, lLoc) : lLoc += 4
      Reserved = ToUInt32(bData, lLoc) : lLoc += 4
      Duration = ToUInt32(bData, lLoc) : lLoc += 4
      Reserved2 = ToUInt64(bData, lLoc) : lLoc += 8
      Layer = ToUInt16(bData, lLoc) : lLoc += 2
      AlternateGroup = ToUInt16(bData, lLoc) : lLoc += 2
      Volume = ToFixed16(bData, lLoc) : lLoc += 2
      Reserved3 = ToUInt16(bData, lLoc) : lLoc += 2
      ReDim MatrixStruct(35)
      Array.ConstrainedCopy(bData, lLoc, MatrixStruct, 0, 36) : lLoc += 36
      TrackWidth = ToFixed32(bData, lLoc) : lLoc += 4
      TrackHeight = ToFixed32(bData, lLoc) : lLoc += 4
    End Sub
  End Structure
  Private bValid As Boolean
  Private mBrands As List(Of String)
  Private lDuration As UInt32
  Private szResolution As SizeF
  Public Sub New(ByVal bData As Byte())
    bValid = False
    Dim lIDX As Long = 0
    Do While lIDX < bData.LongLength
      Dim mBox As New BOX(bData, lIDX)
      Select Case mBox.Name
        Case "ftyp"
          Dim mFtyp As New BOXftyp(bData, lIDX)
          mBrands = New List(Of String)
          For I As Long = 0 To mFtyp.CompatibleBrands.LongLength - 1
            If mFtyp.CompatibleBrands(I) = "isom" Then Continue For
            If mFtyp.CompatibleBrands(I) = "iso2" Then Continue For
            mBrands.Add(mFtyp.CompatibleBrands(I))
            bValid = True
          Next
        Case "moov"
          Dim mMoov As New BOXmoov(bData, lIDX)
          If mMoov.mvhd.Duration > 0 Then
            lDuration = mMoov.mvhd.Duration
            bValid = True
          End If
          If Not (mMoov.trak.tkhd.TrackWidth = 0 Or mMoov.trak.tkhd.TrackHeight = 0) Then
            szResolution = New SizeF(mMoov.trak.tkhd.TrackWidth, mMoov.trak.tkhd.TrackHeight)
            bValid = True
          End If
      End Select
      lIDX += mBox.Size
    Loop
  End Sub
  Public ReadOnly Property IsValid As Boolean
    Get
      Return bValid
    End Get
  End Property
  Public ReadOnly Property Brands As String()
    Get
      Return mBrands.ToArray
    End Get
  End Property
  Public ReadOnly Property Duration As UInt32
    Get
      Return lDuration
    End Get
  End Property
  Public ReadOnly Property Resolution As SizeF
    Get
      Return szResolution
    End Get
  End Property
  Private Shared Function ToUInt16(ByVal value As Byte(), ByVal startIndex As Long) As UInt16
    Dim b1 As Byte = value(startIndex)
    Dim b2 As Byte = value(startIndex + 1)
    Return (CUShort(b1) << 8) Or CUShort(b2)
  End Function
  Private Shared Function ToUInt24(ByVal value As Byte(), ByVal startIndex As Long) As UInt32
    Dim b1 As Byte = value(startIndex)
    Dim b2 As Byte = value(startIndex + 1)
    Dim b3 As Byte = value(startIndex + 2)
    Return (CUInt(b1) << 16) Or (CUInt(b2) << 8) Or CUInt(b3)
  End Function
  Private Shared Function ToUInt32(ByVal value As Byte(), ByVal startIndex As Long) As UInt32
    Dim b1 As Byte = value(startIndex)
    Dim b2 As Byte = value(startIndex + 1)
    Dim b3 As Byte = value(startIndex + 2)
    Dim b4 As Byte = value(startIndex + 3)
    Return (CUInt(b1) << 24) Or (CUInt(b2) << 16) Or (CUInt(b3) << 8) Or CUInt(b4)
  End Function
  Private Shared Function ToInt32(ByVal value As Byte(), ByVal startIndex As Long) As Int32
    Dim b1 As Byte = value(startIndex)
    Dim b2 As Byte = value(startIndex + 1)
    Dim b3 As Byte = value(startIndex + 2)
    Dim b4 As Byte = value(startIndex + 3)
    If (b1 And &H80) = &H80 Then
      b1 = b1 And &H7F
      Return -1 * CInt((CUInt(b1) << 24) Or (CUInt(b2) << 16) Or (CUInt(b3) << 8) Or CUInt(b4))
    End If
    Return (CUInt(b1) << 24) Or (CUInt(b2) << 16) Or (CUInt(b3) << 8) Or CUInt(b4)
  End Function
  Private Shared Function ToUInt64(ByVal value As Byte(), ByVal startIndex As Long) As UInt64
    Dim b1 As Byte = value(startIndex)
    Dim b2 As Byte = value(startIndex + 1)
    Dim b3 As Byte = value(startIndex + 2)
    Dim b4 As Byte = value(startIndex + 3)
    Dim b5 As Byte = value(startIndex + 4)
    Dim b6 As Byte = value(startIndex + 5)
    Dim b7 As Byte = value(startIndex + 6)
    Dim b8 As Byte = value(startIndex + 7)
    Return (CULng(b1) << 56) Or (CULng(b2) << 48) Or (CULng(b3) << 40) Or CULng(b4 << 32) Or (CULng(b5) << 24) Or (CULng(b6) << 16) Or (CULng(b7) << 8) Or CULng(b8)
  End Function
  Private Shared Function ToFixed32(ByVal value As Byte(), ByVal startIndex As Long) As Decimal
    Dim v1 As Byte = value(startIndex)
    Dim v2 As Byte = value(startIndex + 1)
    Dim f1 As Byte = value(startIndex + 2)
    Dim f2 As Byte = value(startIndex + 3)
    Dim f As Int16 = (CShort(f1) << 8) Or f2
    Dim v As Int16 = (CShort(v1) << 8) Or v2
    Return v + (CDec(f) / 32767)
  End Function
  Private Shared Function ToFixed16(ByVal value As Byte(), ByVal startIndex As Long) As Decimal
    Dim v As Byte = value(startIndex)
    Dim f As Byte = value(startIndex + 1)
    Return v + (CDec(f) / 255)
  End Function
End Class
