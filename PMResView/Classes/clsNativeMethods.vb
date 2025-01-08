Imports System.Runtime.InteropServices
Public NotInheritable Class NativeMethods
  <DllImport("shell32", SetLastError:=True, CharSet:=CharSet.Auto)>
  Public Shared Function ShellExecute(ByVal hwnd As Long, ByVal lpszOp As String, ByVal lpszFile As String, ByVal lpszParams As String, ByVal LpszDir As String, ByVal FsShowCmd As Long) As Long
  End Function
  <DllImport("shell32", SetLastError:=True, CharSet:=CharSet.Auto)>
  Public Shared Function SHGetFileInfo(ByVal pszPath As String, ByVal dwFileAttributes As UInteger, ByRef psfi As SHFILEINFO, ByVal cbFileInfo As Integer, ByVal uFlags As UInteger) As IntPtr
  End Function
  <DllImport("user32", SetLastError:=True, CharSet:=CharSet.Auto)>
  Public Shared Function DestroyIcon(ByVal hWnd As IntPtr) As Boolean
  End Function
  <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
  Public Structure SHFILEINFO
    Public hIcon As IntPtr
    Public iIcon As Integer
    Public dwAttributes As UInteger
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)>
    Public szDisplayName As String
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)>
    Public szTypeName As String
  End Structure
  <DllImport("user32", SetLastError:=True, CharSet:=CharSet.Auto)>
  Public Shared Function GetSystemMetrics(ByVal nIndex As MetricsList) As Integer
  End Function
  Public Enum EShellGetFileInfoConstants
    SHGFI_ICON = &H100
    SHGFI_DISPLAYNAME = &H200
    SHGFI_TYPENAME = &H400
    SHGFI_ATTRIBUTES = &H800
    SHGFI_ICONLOCATION = &H1000
    SHGFI_EXETYPE = &H2000
    SHGFI_SYSICONINDEX = &H4000
    SHGFI_LINKOVERLAY = &H8000
    SHGFI_SELECTED = &H10000
    SHGFI_ATTR_SPECIFIED = &H20000
    SHGFI_LARGEICON = &H0
    SHGFI_SMALLICON = &H1
    SHGFI_OPENICON = &H2
    SHGFI_SHELLICONSIZE = &H4
    SHGFI_PIDL = &H8
    SHGFI_USEFILEATTRIBUTES = &H10
  End Enum
  Public Enum MetricsList As Integer
    SM_CXSCREEN = 0
    SM_CYSCREEN = 1
    SM_CXVSCROLL = 2
    SM_CYHSCROLL = 3
    SM_CYCAPTION = 4
    SM_CXBORDER = 5
    SM_CYBORDER = 6
    SM_CXDLGFRAME = 7
    SM_CYDLGFRAME = 8
    SM_CYVTHUMB = 9
    SM_CXHTHUMB = 10
    SM_CXICON = 11
    SM_CYICON = 12
    SM_CXCURSOR = 13
    SM_CYCURSOR = 14
    SM_CYMENU = 15
    SM_CXFULLSCREEN = 16
    SM_CYFULLSCREEN = 17
    SM_CYKANJIWINDOW = 18
    SM_MOUSEPRESENT = 19
    SM_CYVSCROLL = 20
    SM_CXHSCROLL = 21
    SM_DEBUG = 22
    SM_SWAPBUTTON = 23
    SM_CXMIN = 28
    SM_CYMIN = 29
    SM_CXSIZE = 30
    SM_CYSIZE = 31
    SM_CXFRAME = 32
    SM_CYFRAME = 33
    SM_CXMINTRACK = 34
    SM_CYMINTRACK = 35
    SM_CXDOUBLECLICK = 36
    SM_CYDOUBLECLICK = 37
    SM_CXICONSPACING = 38
    SM_CYICONSPACING = 39
    SM_MENUDROPALIGNMENT = 40
    SM_PENWINDOWS = 41
    SM_DBCSENABLED = 42
    SM_CMOUSEBUTTONS = 43
    SM_SECURE = 44
    SM_CXEDGE = 45
    SM_CYEDGE = 46
    SM_CXMINSPACING = 47
    SM_CYMINSPACING = 48
    SM_CXSMICON = 49
    SM_CYSMICON = 50
    SM_CYSMCAPTION = 51
    SM_CXSMSIZE = 52
    SM_CYSMSIZE = 53
    SM_CXMENUSIZE = 54
    SM_CYMENUSIZE = 55
    SM_ARRANGE = 56
    SM_CXMINIMZED = 57
    SM_CYMINIMIZED = 58
    SM_CXMAXTRACK = 59
    SM_CYMAXTRACK = 60
    SM_CXMAXIMIZED = 61
    SM_CYMAXIMIZED = 62
    SM_NETWORK = 63
    SM_CLEANBOOT = 67
    SM_CXDRAG = 68
    SM_CYDRAG = 69
    SM_SHOWSOUNDS = 70
    SM_CXMENUCHECK = 71
    SM_CYMENUCHECK = 72
    SM_SLOWMACHINE = 73
    SM_MIDEASTENABLED = 74
    SM_MOUSEWHEELPRESENT = 75
    SM_XVIRTUALSCREEN = 76
    SM_YVIRTUALSCREEN = 77
    SM_CXVIRTUALSCREEN = 78
    SM_CYVIRTUALSCREEN = 79
    SM_CMONITORS = 80
    SM_DISPLAYFORMAT = 81
    SM_IMMENABLED = 82
    SM_CXFOCUSBORDER = 83
    SM_CYFOCUSBORDER = 84
    SM_TABLETPC = 86
    SM_MEDIACENTER = 87
    SM_STARTER = 88
    SM_SERVER2 = 89
    SM_MOUSEHORIZONTALWHEELPRESENT = 91
    SM_CXPADDEDBORDER = 92
    SM_DIGITIZER = 94
    SM_MAXIMUMTOUCHES = 95
    SM_REMOTESESSION = &H1000
    SM_SHUTTINGDOWN = &H2000
    SM_REMOTECONTROL = &H2001
    SM_CONVERTIBLESLATEMODE = &H2003
    SM_SYSTEMDOCKED = &H2004
  End Enum
  <DllImport("user32", SetLastError:=True, CharSet:=CharSet.Auto)>
  Public Shared Function LoadCursor(ByVal hInstance As IntPtr, ByVal lpCursorName As IntPtr) As IntPtr
  End Function
  <DllImport("user32", SetLastError:=True, CharSet:=CharSet.Auto)>
  Public Shared Function SetCursor(ByVal hCursor As IntPtr) As IntPtr
  End Function
  <DllImport("user32", SetLastError:=True, CharSet:=CharSet.Auto)>
  Public Shared Function ShowScrollBar(ByVal hwnd As IntPtr, ByVal wBar As Integer, ByVal bShow As Boolean) As Integer
  End Function
  <DllImport("kernel32", SetLastError:=True, CharSet:=CharSet.Unicode)>
  Public Shared Function WritePrivateProfileStringW(ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
  End Function
  <DllImport("kernel32", SetLastError:=True, CharSet:=CharSet.Unicode)>
  Public Shared Function GetPrivateProfileStringW(ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Int32, ByVal lpFileName As String) As Integer
  End Function
End Class
