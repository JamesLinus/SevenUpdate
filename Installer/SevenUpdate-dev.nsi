!define PRODUCT_NAME "Seven Update - Dev Channel"
!define PRODUCT_VERSION "1.2.1.2"
!define PRODUCT_PUBLISHER "Seven Software"
!define PRODUCT_WEB_SITE "http://sevenupdate.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\SevenUpdate.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"
!include "x64.nsh"
!include "LogicLib.nsh"
!include "WinVer.nsh"
!include "DotNetVer.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "su-dev.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE ".\license.txt"
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\SevenUpdate.exe"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME}"
OutFile "Seven Update Setup - Dev Channel.exe"
InstallDir "$PROGRAMFILES64\Seven Software\Seven Update"
ShowInstDetails show
ShowUnInstDetails show
RequestExecutionLevel admin

Function CloseSevenUpdate

Push $5

loop:
	push "SevenUpdate.exe"
  processwork::existsprocess
  pop $5
	IntCmp $5 0 CheckAdmin
	Goto prompt
prompt:
  MessageBox MB_RETRYCANCEL|MB_ICONSTOP '$(^Name) must be closed before installation can begin.$\r$\nPress "Retry" to automatically close $(^Name) and continue or cancel the installation.'  IDCANCEL BailOut
  push "SevenUpdate.exe"
  processwork::KillProcess
	push "SevenUpdate.Admin.exe"
  processwork::KillProcess
  	push "SevenUpdate.Helper.exe"
  processwork::KillProcess
  Sleep 1000
Goto loop

BailOut:
  Abort

CheckAdmin:
push "SevenUpdate.Admin.exe"
processwork::existsprocess
pop $5
IntCmp $5 0 done
Goto prompt
done:
Pop $5

FunctionEnd

Function un.CloseSevenUpdate

Push $5

loop:
	push "SevenUpdate.exe"
  processwork::existsprocess
  pop $5
	IntCmp $5 0 CheckAdmin
	Goto prompt
prompt:
  MessageBox MB_RETRYCANCEL|MB_ICONSTOP '$(^Name) must be closed before you can uninstall it.$\r$\nPress "Retry" to automatically close $(^Name) and continue or cancel the uninstallation.'  IDCANCEL BailOut
  push "SevenUpdate.exe"
  processwork::KillProcess
	push "SevenUpdate.Admin.exe"
  processwork::KillProcess
  Sleep 1000
Goto loop

BailOut:
  Abort

CheckAdmin:
push "SevenUpdate.Admin.exe"
processwork::existsprocess
pop $5
IntCmp $5 0 CheckHelper
Goto prompt
CheckHelper:
push "SevenUpdate.Helper.exe"
processwork::existsprocess
pop $5
IntCmp $5 0 done
Goto prompt
done:
Pop $5

FunctionEnd

Function .onInit
	${If} ${RunningX64}
		StrCpy $INSTDIR "$PROGRAMFILES64\Seven Software\Seven Update"
	${Else}
		StrCpy $INSTDIR "$PROGRAMFILES\Seven Software\Seven Update"
	${EndIf}
	
FunctionEnd
	 
 Function ConnectInternet

  Push $R0
    
    ClearErrors
    Dialer::AttemptConnect
    IfErrors noie3
    
    Pop $R0
    StrCmp $R0 "online" connected
      MessageBox MB_OK|MB_ICONSTOP "Cannot connect to the internet."
      Quit ;Remove to make error not fatal
    
    noie3:
  
    ; IE3 not installed
    MessageBox MB_OK|MB_ICONINFORMATION "Please connect to the internet now."
    
    connected:
  
  Pop $R0
  
FunctionEnd

Function RefreshShellIcons
  !define SHCNE_ASSOCCHANGED 0x08000000
  !define SHCNF_IDLIST 0
  System::Call 'shell32.dll::SHChangeNotify(i, i, i, i) v (${SHCNE_ASSOCCHANGED}, ${SHCNF_IDLIST}, 0, 0)'
FunctionEnd
 
!macro DownloadFile SOURCE DEST 
  DetailPrint "Downloading: ${SOURCE}"
  inetc::get /TIMEOUT=30000 "${SOURCE}" "${DEST}" /END
  Pop $0 ;Get the return value
  StrCmp $0 "OK" +3
  StrCmp $0 "cancelled" +6
  inetc::get /TIMEOUT=30000 /NOPROXY "${SOURCE}" "${DEST}" /END
  Pop $0
  DetailPrint "Result: $0"
  StrCmp $0 "OK" +3
  MessageBox MB_OK "Download failed: $0"
  Quit
  
!macroend

!include "WordFunc.nsh"
!insertmacro VersionCompare

!macro DownloadDotNet DotNetReqVer
!define DOTNET_URL "http://download.microsoft.com/download/5/6/2/562A10F9-C9F4-4313-A044-9C94E0A8FAC8/dotNetFx40_Client_x86_x64.exe"
  DetailPrint "Checking your .NET Framework version..."
  ${If} ${DOTNETVER_4_0} HasDotNetClientProfile 1
		DetailPrint "Microsoft .NET Framework 4.0 (Client Profile) Installed."
  ${Else}
		!insertmacro DownloadFile ${DOTNET_URL} "$TEMP\dotnetfx40.exe"
		DetailPrint "Pausing installation while downloaded .NET Framework installer runs."
		ExecWait '$TEMP\dotnetfx40.exe /q /norestart /c:"install /q"'
		DetailPrint "Completed .NET Framework install/update. Removing .NET Framework installer."
		Delete "$TEMP\dotnetfx40.exe"
		DetailPrint ".NET Framework installer removed."
  ${EndIf}
!macroend

Section "Main Section" SEC01
  MessageBox MB_YESNO|MB_ICONSTOP "You are about to install the Development Channel of Seven Update. It may contain bugs, crash, or do worse. This is intended for bug testers and developers who want to check out any new features and test any changes. By installing this dev channel, you agree to not hold Seven Software, Robert Baker, or any of it's developers or partners responsible for data corruption, data loss, or other failures resulting in installing this software. Are you sure you would like to install the dev channel?" IDYES +2
  Quit

  SetOutPath $INSTDIR
  SetShellVarContext all
  SetOverwrite on
  SectionIn RO
  Call ConnectInternet
  !insertmacro DownloadDotNet "4.0"
  Call CloseSevenUpdate
  
  DetailPrint "Removing old installation files"
  RMDir /r $INSTDIR
  
  DetailPrint "Downloading $(^Name)..."
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SevenUpdate.exe" "$INSTDIR\SevenUpdate.exe"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SevenUpdate.exe.config" "$INSTDIR\SevenUpdate.exe.config"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SevenUpdate.Admin.exe" "$INSTDIR\SevenUpdate.Admin.exe"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SevenUpdate.Helper.exe" "$INSTDIR\SevenUpdate.Helper.exe"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SevenUpdate.Base.dll" "$INSTDIR\SevenUpdate.Base.dll"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SevenUpdate.Service.dll" "$INSTDIR\SevenUpdate.Service.dll"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/System.Windows.dll" "$INSTDIR\System.Windows.dll"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SharpBits.Base.dll" "$INSTDIR\SharpBits.Base.dll"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/protobuf-net.dll" "$INSTDIR\protobuf-net.dll"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/WPFLocalizeExtension.dll" "$INSTDIR\WPFLocalizeExtension.dll"
	  
  ${If} ${AtMostWinXP}
	  WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" 'Seven Update Automatic Checking' '$INSTDIR\SevenUpdate.Helper.exe'
	  
  ${Else}
  	DetailPrint "Installing $(^Name) service..."
  
    !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SevenUpdate.xml" "$TEMP\SevenUpdate.xml"
    !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdate-dev/SevenUpdate.Admin.xml" "$TEMP\SevenUpdate.Admin.xml"
	  
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "Seven Update" /F"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "Seven Update.Admin" /F"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "SevenUpdate" /F"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "SevenUpdate.Admin" /F"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /create /XML "$TEMP\SevenUpdate.xml" /TN "SevenUpdate"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /create /XML "$TEMP\SevenUpdate.Admin.xml" /TN "SevenUpdate.Admin"'
  ${EndIf}
  
  Delete "$APPDATA\Seven Software\Seven Update\updates.sui"
  
  DetailPrint "Creating shortcuts..."
  SetShellVarContext current
  CreateDirectory "$APPDATA\Seven Software\Seven Update"
  SetShellVarContext all
  CreateDirectory "$APPDATA\Seven Software\Seven Update"
  CreateDirectory "$SMPROGRAMS\Seven Software"
  CreateShortCut "$SMPROGRAMS\Seven Software\Seven Update.lnk" "$INSTDIR\SevenUpdate.exe"
  
  ${If} ${RunningX64}
	SetRegView 64
  ${EndIf}
  
  DetailPrint "Enabling the Dev Channel"
  WriteRegStr HKLM "SOFTWARE\Seven Software\Seven Update" "channel" "dev"
  
  DetailPrint "Registering file types..."
  WriteRegStr HKCR "sevenupdate" "" "URL:Seven Update Protocol"
  WriteRegStr HKCR "sevenupdate" "URL Protocol" ""
  WriteRegStr HKCR "sevenupdate\DefaultIcon" "" "SevenUpdate.exe,0"
  WriteRegStr HKCR "sevenupdate\shell\open\command" "" '"$INSTDIR\SevenUpdate.exe" "%1"'
  
  WriteRegStr HKCR ".sua" "" "SevenUpdate.sua"
  WriteRegStr HKCR "SevenUpdate.sua" "" "Seven Update Application Information"
  WriteRegStr HKCR "SevenUpdate.sua\DefaultIcon" "" "$INSTDIR\SevenUpdate.Base.dll,1"
  WriteRegStr HKCR "SevenUpdate.sua\shell\open\command" "" '"$INSTDIR\SevenUpdate.exe" "%1"'
  Call RefreshShellIcons
  
  DetailPrint "Optimizing .Net framework..."
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" install "$INSTDIR\SevenUpdate.exe" /queue:1 /nologo /silent'
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" install "$INSTDIR\SevenUpdate.Admin.exe" /queue:1 /nologo /silent'
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" install "$INSTDIR\SevenUpdate.Helper.exe" /queue:1 /nologo /silent'
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" update /queue:2 /nologo /silent'

SectionEnd

Section -Post
  ${If} ${RunningX64}
	SetRegView 64
  ${EndIf}
  
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\SevenUpdate.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\SevenUpdate.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
	Call un.CloseSevenUpdate
FunctionEnd

Section Uninstall
  SetShellVarContext all
  
  ${If} ${RunningX64}
	SetRegView 64
  ${EndIf}
  
  DeleteRegValue HKLM "SOFTWARE\Seven Software\Seven Update" "channel"
  
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" uninstall "$INSTDIR\SevenUpdate.exe" /nologo /silent'
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" uninstall "$INSTDIR\SevenUpdate.Admin.exe" /nologo /silent'
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" uninstall "$INSTDIR\SevenUpdate.Helper.exe" /nologo /silent'
  
  Delete "$SMPROGRAMS\Seven Software\Seven Update.lnk"

  RMDir "$SMPROGRAMS\Seven Software"
  RMDir /r /REBOOTOK $INSTDIR

  SetShellVarContext current
  RMDir /r "$APPDATA\Seven Software\Seven Update"
  RMDir "$APPDATA\Seven Software"
  
  SetShellVarContext all
  RMDir /r "$APPDATA\Seven Software\Seven Update"
  RMDir "$APPDATA\Seven Software"
  
  ${If} ${AtMostWinXP}
	DeleteRegValue HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" "Seven Update Automatic Checking"
  ${Else}
	nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "Seven Update" /F'
	nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "Seven Update.Admin" /F'
  ${EndIf}

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  DeleteRegKey HKCR ".sua"
  DeleteRegKey HKCR "SevenUpdate.sua"
  SetAutoClose true
SectionEnd