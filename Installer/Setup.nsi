; ============================================================
; DevKit2 Installer - NSIS
; - Sign installer + uninstaller at build time (!finalize / !uninstfinalize)
; - Reinstall/upgrade allowed (overwrite files)
; - Uninstall preserves UserData (does NOT delete UserData)
; NSIS 3.11+
; ============================================================

!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "x64.nsh"
!include "FileFunc.nsh"

Var InstallSizeKB

; ---------------------------
; App constants
; ---------------------------
!define APP_NAME        "DevKit2"
!define APP_VERSION     "1.0.0"
!define APP_PUBLISHER   "Minh Research"
!define APP_URL         "https://github.com/minhnguyenerp/devkit2/"
!define APP_EXE         "devkit2.exe"

!define DOTNET_EXE      "windowsdesktop-runtime-10.0.3-win-x64.exe"

!define INSTALL_DIR     "$LOCALAPPDATA\Programs\Minh Research\${APP_NAME}"

!define SRC_DIR         "..\bin\Release\net10.0-windows"

; ---------------------------
; General
; ---------------------------
Name "${APP_NAME}"
OutFile "Output\DevKit2-Setup.exe"
InstallDir "${INSTALL_DIR}"
InstallDirRegKey HKLM "Software\Minh Research\${APP_NAME}" "InstallDir"
RequestExecutionLevel admin
Unicode True
SetOverwrite on

!define MUI_ABORTWARNING

; ---------------------------
; UI pages
; ---------------------------
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY

Var CreateDesktopIcon
!define MUI_PAGE_CUSTOMFUNCTION_PRE ComponentsPre
!insertmacro MUI_PAGE_COMPONENTS

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_TEXT "Launch ${APP_NAME}"
!define MUI_FINISHPAGE_RUN_FUNCTION LaunchApp
!insertmacro MUI_PAGE_FINISH

; ---- Uninstall pages (MUI2)
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

; ---------------------------
; Components
; ---------------------------
Function ComponentsPre
  ; default checked
  StrCpy $CreateDesktopIcon 1
FunctionEnd

Section "Create Desktop Icon" SecDesktopIcon
  StrCpy $CreateDesktopIcon 1
SectionEnd

; ---------------------------
; Main install
; ---------------------------
Section "Main Application" SecMain
  SectionIn RO

  ; x64 only
  ${IfNot} ${RunningX64}
    MessageBox MB_ICONSTOP "This installer requires 64-bit Windows."
    Abort
  ${EndIf}

  SetRegView 64

  ; prerequisites
  Call EnsureDotNetDesktopRuntime

  ; Copy EVERYTHING from build output (including UserData) -> overwrite on upgrade
  SetOutPath "$INSTDIR"
  File /r "${SRC_DIR}\*.*"

  ; Sau khi copy file xong
  ${GetSize} "$INSTDIR" "/S=0K" $InstallSizeKB $0 $1

  ; Save install dir
  WriteRegStr HKLM "Software\Minh Research\${APP_NAME}" "InstallDir" "$INSTDIR"

  ; Uninstall entry
  WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "DisplayName" "${APP_NAME}"
  WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "DisplayVersion" "${APP_VERSION}"
  WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "Publisher" "${APP_PUBLISHER}"
  WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "URLInfoAbout" "${APP_URL}"
  WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "InstallLocation" "$INSTDIR"
  WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "DisplayIcon" "$INSTDIR\${APP_EXE}"
  WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "NoRepair" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "EstimatedSize" $InstallSizeKB

  WriteUninstaller "$INSTDIR\uninstall.exe"

  ; Shortcuts
  CreateDirectory "$SMPROGRAMS\${APP_NAME}"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"

  ${If} $CreateDesktopIcon == 1
    CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"
  ${EndIf}

SectionEnd

; ---------------------------
; Uninstall (preserve UserData)
; ---------------------------
Section "Uninstall"
  Delete "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk"
  RMDir  "$SMPROGRAMS\${APP_NAME}"
  Delete "$DESKTOP\${APP_NAME}.lnk"

  ; Delete everything in $INSTDIR EXCEPT "UserData"
  Call un.Uninstall_DeleteExceptUserData

  ; Remove uninstaller itself
  Delete "$INSTDIR\uninstall.exe"

  ; Remove registry
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"
  DeleteRegKey HKLM "Software\Minh Research\${APP_NAME}"
SectionEnd

; ============================================================
; Functions
; ============================================================

Function LaunchApp
  Exec '"$INSTDIR\${APP_EXE}"'
FunctionEnd

; ---- .NET Desktop Runtime 10.0.3 presence check
Function DotNetIsMissing
  Push $0
  StrCpy $0 0

  ${If} ${FileExists} "$PROGRAMFILES64\dotnet\shared\Microsoft.WindowsDesktop.App\10.0.3\*.*"
    StrCpy $0 0
  ${ElseIf} ${FileExists} "$PROGRAMFILES\dotnet\shared\Microsoft.WindowsDesktop.App\10.0.3\*.*"
    StrCpy $0 0
  ${Else}
    StrCpy $0 1
  ${EndIf}

  Exch $0
FunctionEnd

Function EnsureDotNetDesktopRuntime
  Push $0
  Call DotNetIsMissing
  Pop $0

  ${If} $0 == 1
    DetailPrint "Installing .NET Desktop Runtime 10.0.3 (silent)..."

    SetOutPath "$TEMP"
    File "/oname=$TEMP\${DOTNET_EXE}" "${DOTNET_EXE}"

    ExecWait '"$TEMP\${DOTNET_EXE}" /install /quiet /norestart' $0
    ${If} $0 != 0
      MessageBox MB_ICONSTOP ".NET Runtime installer exited with code $0. Please fix and run installer again."
      Abort
    ${EndIf}
  ${EndIf}

  Pop $0
FunctionEnd

; ---- Uninstaller-only function: delete everything except $INSTDIR\UserData
Function un.Uninstall_DeleteExceptUserData
  Push $0
  Push $1
  Push $2
  Push $3

  StrCpy $0 "$INSTDIR"

  FindFirst $1 $2 "$0\*.*"
  ${DoWhile} $2 != ""
    ${If} $2 != "."
    ${AndIf} $2 != ".."
      ${If} $2 == "UserData"
        ; keep UserData
      ${Else}
        StrCpy $3 "$0\$2"
        ${If} ${FileExists} "$3\*.*"
          RMDir /r "$3"
        ${Else}
          Delete "$3"
        ${EndIf}
      ${EndIf}
    ${EndIf}
    FindNext $1 $2
  ${Loop}
  FindClose $1

  Pop $3
  Pop $2
  Pop $1
  Pop $0
FunctionEnd
