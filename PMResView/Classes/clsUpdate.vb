﻿Public Class clsUpdate
  Implements IDisposable
  Private Enum SecurityProtocolTypeEx As Integer
    None = 0
    Ssl3 = &H30
    Tls10 = &HC0
    Tls11 = &H300
    Tls12 = &HC00
  End Enum
  Private Const VersionURL As String = "update.realityripple.com/PMResView/update.ver?sha=512"
  Class ProgressEventArgs
    Inherits EventArgs
    Public BytesReceived As Long
    Public ProgressPercentage As Integer
    Public TotalBytesToReceive As Long
    Friend Sub New(ByVal bReceived As Long, ByVal bToReceive As Long, ByVal iPercentage As Integer)
      BytesReceived = bReceived
      TotalBytesToReceive = bToReceive
      ProgressPercentage = iPercentage
    End Sub
  End Class
  Class CheckEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs
    Enum ResultType
      NoUpdate
      NewUpdate
    End Enum
    Public Result As ResultType
    Public Version As String
    Friend Sub New(ByVal rtResult As ResultType, ByVal sVersion As String, ByVal ex As Exception, ByVal bCancelled As Boolean, ByVal state As Object)
      MyBase.New(ex, bCancelled, state)
      Version = sVersion
      Result = rtResult
    End Sub
    Friend Sub New(ByVal rtResult As ResultType, ByVal sVersion As String, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
      MyBase.New(e.Error, e.Cancelled, e.UserState)
      Version = sVersion
      Result = rtResult
    End Sub
  End Class
  Class DownloadEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs
    Public Version As String
    Friend Sub New(ByVal sVersion As String, ByVal [error] As Exception, ByVal [cancelled] As Boolean, ByVal [userState] As Object)
      MyBase.New([error], [cancelled], [userState])
      Version = sVersion
    End Sub
    Friend Sub New(ByVal sVersion As String, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
      MyBase.New(e.Error, e.Cancelled, e.UserState)
      Version = sVersion
    End Sub
  End Class
  Public Event CheckingVersion(ByVal sender As Object, ByVal e As EventArgs)
  Public Event CheckProgressChanged(ByVal sender As Object, ByVal e As ProgressEventArgs)
  Public Event CheckResult(ByVal sender As Object, ByVal e As CheckEventArgs)
  Public Event DownloadingUpdate(ByVal sender As Object, ByVal e As EventArgs)
  Public Event UpdateProgressChanged(ByVal sender As Object, ByVal e As ProgressEventArgs)
  Public Event DownloadResult(ByVal sender As Object, ByVal e As DownloadEventArgs)
  Private WithEvents wsVer As New Net.WebClient
  Private DownloadURL As String
  Private DownloadHash As String
  Private DownloadLoc As String
  Private VerNumber As String
#Region "IDisposable Support"
  Private disposedValue As Boolean
  Protected Overridable Sub Dispose(ByVal disposing As Boolean)
    If Not Me.disposedValue Then
      If disposing Then
        If wsVer IsNot Nothing Then
          If wsVer.IsBusy Then
            wsVer.CancelAsync()
          End If
          wsVer.Dispose()
          wsVer = Nothing
        End If
      End If
    End If
    Me.disposedValue = True
  End Sub
  Public Sub Dispose() Implements IDisposable.Dispose
    Dispose(True)
    GC.SuppressFinalize(Me)
  End Sub
#End Region
  Public Sub CheckVersion()
    ModernProtcol()
    DownloadURL = Nothing
    DownloadHash = Nothing
    VerNumber = Nothing
    DownloadLoc = Nothing
    wsVer.Headers.Add("User-Agent", "none")
    wsVer.CachePolicy = New Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore)
    RaiseEvent CheckingVersion(Me, New EventArgs)
    wsVer.Headers.Add("X-Thumb", Authenticode.RRSignThumb)
    wsVer.Headers.Add("X-Serial", Authenticode.RRSignSerial)
    wsVer.DownloadStringAsync(New Uri(ProtoURL(VersionURL)), "INFO")
  End Sub
  Public Shared Function QuickCheckVersion() As CheckEventArgs.ResultType
    ModernProtcol()
    Dim sVerStr As String
    Using wsCheck As New Net.WebClient
      wsCheck.Headers.Add("User-Agent", "none")
      wsCheck.CachePolicy = New Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore)
      wsCheck.Headers.Add("X-Thumb", Authenticode.RRSignThumb)
      wsCheck.Headers.Add("X-Serial", Authenticode.RRSignSerial)
      sVerStr = wsCheck.DownloadString(New Uri(ProtoURL(VersionURL)))
      Dim sHash As String = Nothing
      For Each sKey As String In wsCheck.ResponseHeaders
        If sKey.ToLower = "x-update-signature" Then
          sHash = wsCheck.ResponseHeaders(sKey)
          Exit For
        End If
      Next
      If Not VerifySignature(sVerStr, sHash) Then Return CheckEventArgs.ResultType.NoUpdate
    End Using
    If String.IsNullOrEmpty(sVerStr) Then Return CheckEventArgs.ResultType.NoUpdate
    If Not sVerStr.Contains("|"c) Then Return CheckEventArgs.ResultType.NoUpdate
    Dim sVU As String() = Split(sVerStr, "|", 3)
    If CompareVersions(sVU(0)) Then Return CheckEventArgs.ResultType.NewUpdate
    Return CheckEventArgs.ResultType.NoUpdate
  End Function
  Public Sub DownloadUpdate(ByVal toLocation As String)
    If Not String.IsNullOrEmpty(DownloadURL) Then
      DownloadLoc = toLocation
      wsVer.CachePolicy = New Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore)
      RaiseEvent DownloadingUpdate(Me, New EventArgs)
      wsVer.DownloadFileAsync(New Uri(DownloadURL), toLocation, "FILE")
    Else
      RaiseEvent DownloadResult(Me, New DownloadEventArgs(Nothing, New Exception("Version Check was not run."), True, Nothing))
    End If
  End Sub
  Private Sub wsVer_DownloadProgressChanged(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs) Handles wsVer.DownloadProgressChanged
    If e.UserState = "INFO" Then
      RaiseEvent CheckProgressChanged(sender, New ProgressEventArgs(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage))
    ElseIf e.UserState = "FILE" Then
      RaiseEvent UpdateProgressChanged(sender, New ProgressEventArgs(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage))
    End If
  End Sub
  Private Sub wsVer_DownloadStringCompleted(ByVal sender As Object, ByVal e As System.Net.DownloadStringCompletedEventArgs) Handles wsVer.DownloadStringCompleted
    Dim rRet As CheckEventArgs.ResultType = CheckEventArgs.ResultType.NoUpdate
    DownloadURL = Nothing
    DownloadHash = Nothing
    VerNumber = Nothing
    If e.Error Is Nothing Then
      Try
        Dim sVerStr As String = e.Result
        If String.IsNullOrEmpty(sVerStr) Then
          RaiseEvent CheckResult(sender, New CheckEventArgs(CheckEventArgs.ResultType.NoUpdate, VerNumber, New Exception("No Server Response"), e.Cancelled, e.UserState))
          Return
        End If
        Dim sHash As String = Nothing
        For Each sKey As String In wsVer.ResponseHeaders
          If sKey.ToLower = "x-update-signature" Then
            sHash = wsVer.ResponseHeaders(sKey)
            Exit For
          End If
        Next
        If Not VerifySignature(sVerStr, sHash) Then
          RaiseEvent CheckResult(sender, New CheckEventArgs(CheckEventArgs.ResultType.NoUpdate, VerNumber, New Exception("Invalid Server Response"), e.Cancelled, e.UserState))
          Return
        End If
        Dim sVU As String() = Split(sVerStr, "|", 3)
        If CompareVersions(sVU(0)) Then
          rRet = CheckEventArgs.ResultType.NewUpdate
          VerNumber = sVU(0)
          DownloadURL = ProtoURL(sVU(1))
          DownloadHash = sVU(2).ToUpper
        End If
      Catch ex As Exception
        RaiseEvent CheckResult(sender, New CheckEventArgs(CheckEventArgs.ResultType.NoUpdate, VerNumber, New Exception("Version Parsing Error", ex), e.Cancelled, e.UserState))
        Return
      End Try
    End If
    RaiseEvent CheckResult(sender, New CheckEventArgs(rRet, VerNumber, e))
  End Sub
  Private Sub wsVer_DownloadFileCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs) Handles wsVer.DownloadFileCompleted
    If Not IO.File.Exists(DownloadLoc) Then
      RaiseEvent DownloadResult(sender, New DownloadEventArgs(VerNumber, New Exception("Download Failure"), e.Cancelled, e.UserState))
      Return
    End If
    Dim bData As Byte() = IO.File.ReadAllBytes(DownloadLoc)
    Dim sha512 As New Security.Cryptography.SHA512CryptoServiceProvider
    Dim bHash As Byte() = sha512.ComputeHash(bData)
    Dim sHash As String = BitConverter.ToString(bHash).Replace("-", "").ToUpper
    If sHash = DownloadHash Then
      RaiseEvent DownloadResult(sender, New DownloadEventArgs(VerNumber, e))
    Else
      RaiseEvent DownloadResult(sender, New DownloadEventArgs(VerNumber, New Exception("Download Failure"), e.Cancelled, e.UserState))
    End If
  End Sub
  Private Shared Function VerifySignature(ByVal Message As String, ByVal Signature As String) As Boolean
    If String.IsNullOrEmpty(Signature) Then Return False
    Dim bMsg As Byte() = System.Text.Encoding.GetEncoding("latin1").GetBytes(Message)
    Dim bSig As Byte() = Nothing
    Try
      bSig = System.Convert.FromBase64String(Signature)
    Catch ex As Exception
      Return False
    End Try
    Dim rsa As New Security.Cryptography.RSACryptoServiceProvider
    rsa.FromXmlString(My.Resources.pubkey)
    Return rsa.VerifyData(bMsg, Security.Cryptography.CryptoConfig.MapNameToOID("SHA512"), bSig)
  End Function
  Private Shared Function CompareVersions(ByVal sRemote As String) As Boolean
    Dim sLocal As String = Application.ProductVersion
    Dim LocalVer(3) As String
    If sLocal.Contains(".") Then
      For I As Integer = 0 To 3
        If sLocal.Split(".").Count > I Then
          Dim sTmp As String = sLocal.Split(".")(I).Trim
          If IsNumeric(sTmp) And sTmp.Length < 4 Then sTmp &= StrDup(4 - sTmp.Length, "0"c)
          LocalVer(I) = sTmp
        Else
          LocalVer(I) = "0000"
        End If
      Next
    ElseIf sLocal.Contains(",") Then
      For I As Integer = 0 To 3
        If sLocal.Split(",").Count > I Then
          Dim sTmp As String = sLocal.Split(",")(I).Trim
          If IsNumeric(sTmp) And sTmp.Length < 4 Then sTmp &= StrDup(4 - sTmp.Length, "0"c)
          LocalVer(I) = sTmp
        Else
          LocalVer(I) = "0000"
        End If
      Next
    End If
    Dim RemoteVer(3) As String
    If sRemote.Contains(".") Then
      For I As Integer = 0 To 3
        If sRemote.Split(".").Count > I Then
          Dim sTmp As String = sRemote.Split(".")(I).Trim
          If IsNumeric(sTmp) And sTmp.Length < 4 Then sTmp &= StrDup(4 - sTmp.Length, "0"c)
          RemoteVer(I) = sTmp
        Else
          RemoteVer(I) = "0000"
        End If
      Next
    ElseIf sRemote.Contains(",") Then
      For I As Integer = 0 To 3
        If sRemote.Split(",").Count > I Then
          Dim sTmp As String = sRemote.Split(",")(I).Trim
          If IsNumeric(sTmp) And sTmp.Length < 4 Then sTmp &= StrDup(4 - sTmp.Length, "0"c)
          RemoteVer(I) = sTmp
        Else
          RemoteVer(I) = "0000"
        End If
      Next
    End If
    Dim bUpdate As Boolean = False
    If Val(LocalVer(0)) > Val(RemoteVer(0)) Then
    ElseIf Val(LocalVer(0)) = Val(RemoteVer(0)) Then
      If Val(LocalVer(1)) > Val(RemoteVer(1)) Then
      ElseIf Val(LocalVer(1)) = Val(RemoteVer(1)) Then
        If Val(LocalVer(2)) > Val(RemoteVer(2)) Then
        ElseIf Val(LocalVer(2)) = Val(RemoteVer(2)) Then
          If Val(LocalVer(3)) >= Val(RemoteVer(3)) Then
          Else
            bUpdate = True
          End If
        Else
          bUpdate = True
        End If
      Else
        bUpdate = True
      End If
    Else
      bUpdate = True
    End If
    Return bUpdate
  End Function
  Public Shared Sub ModernProtcol()
    Dim useProtocol As SecurityProtocolTypeEx = SecurityProtocolTypeEx.None
    For Each protocolTest In [Enum].GetValues(GetType(SecurityProtocolTypeEx))
      Try
        Net.ServicePointManager.SecurityProtocol = protocolTest
        useProtocol = useProtocol Or protocolTest
      Catch ex As Exception
      End Try
    Next
    Try
      Net.ServicePointManager.SecurityProtocol = useProtocol
    Catch ex As Exception
    End Try
  End Sub
End Class
