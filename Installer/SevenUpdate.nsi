!define PRODUCT_NAME "Seven Update"
!define PRODUCT_VERSION "1.0.0.0"
!define PRODUCT_PUBLISHER "Seven Software"
!define PRODUCT_WEB_SITE "http://sevenupdate.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\SevenUpdate.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"
!include "x64.nsh"
!include "DotNET.nsh"
!include "LogicLib.nsh"
!include "WinVer.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "D:\Documents\Visual Studio 2010\Projects\SevenUpdate\Source\SevenUpdate\icon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"


; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE "D:\Documents\Software Development\Install Files\license.txt"
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
OutFile "Seven Update Setup.exe"
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
  MessageBox MB_RETRYCANCEL|MB_ICONSTOP 'Seven Update must be closed before installation can begin.$\r$\nPress "Retry" to automatically close Seven Update and continue or cancel the installation.'  IDCANCEL BailOut
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
  MessageBox MB_RETRYCANCEL|MB_ICONSTOP 'Seven Update must be closed before you can uninstall it.$\r$\nPress "Retry" to automatically close Seven Update and continue or cancel the uninstallation.'  IDCANCEL BailOut
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
	
	Call CloseSevenUpdate
	
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
 
Section "Main Section" SEC01
  SetOutPath $INSTDIR
  SetShellVarContext all
  SetOverwrite on
  SectionIn RO
  Call ConnectInternet
  !insertmacro CheckDotNET 3.5
	
  RMDir /r $INSTDIR
	
  StrCpy $0 "$INSTDIR\SevenUpdate.exe"
  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SevenUpdate.exe" $0
  Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
  MessageBox MB_OK "Download failed: $R0"
  Quit
  
  StrCpy $0 "$INSTDIR\SharpBits.Base.dll"
  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SharpBits.Base.dll" $0
  Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
  MessageBox MB_OK "Download failed: $R0"
  Quit
  
  StrCpy $0 "$INSTDIR\SevenUpdate.exe.config"
  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SevenUpdate.exe.config" $0
  Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
  MessageBox MB_OK "Download failed: $R0"
  Quit
  
  StrCpy $0 "$INSTDIR\SevenUpdate.Admin.exe.config"
  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SevenUpdate.Admin.exe.config" $0
  Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
  MessageBox MB_OK "Download failed: $R0"
  Quit
  
  StrCpy $0 "$INSTDIR\SevenUpdate.Admin.exe"
  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SevenUpdate.Admin.exe" $0
  Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
  MessageBox MB_OK "Download failed: $R0"
  Quit
  
  StrCpy $0 "$INSTDIR\SevenUpdate.Base.dll"
  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SevenUpdate.Base.dll" $0
  Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
  MessageBox MB_OK "Download failed: $R0"
  Quit
  
  StrCpy $0 "$INSTDIR\Interop.IWshRuntimeLibrary.dll"
  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/Interop.IWshRuntimeLibrary.dll" $0
  Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
  MessageBox MB_OK "Download failed: $R0"
  Quit
  
  StrCpy $0 "$INSTDIR\SevenUpdate.Helper.exe"
  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SevenUpdate.Helper.exe" $0
  Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
  MessageBox MB_OK "Download failed: $R0"
  Quit
	  
  ${If} ${AtMostWinXP}
	  WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" 'Seven Update Automatic Checking' '$INSTDIR\SevenUpdate.Helper.exe'
	  
  ${Else}
  
	  StrCpy $0 "$TEMP\SevenUpdate.xml"
	  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SevenUpdate.xml" $0
	  Pop $R0 ;Get the return value
	  StrCmp $R0 "success" +3
	  MessageBox MB_OK "Download failed: $R0"
	  Quit
  
	  StrCpy $0 "$TEMP\SevenUpdate.Admin.xml"
	  NSISdl::download "http://sevenupdate.com/apps/SevenUpdate/SevenUpdate.Admin.xml" $0
	  Pop $R0 ;Get the return value
	  StrCmp $R0 "success" +3
	  MessageBox MB_OK "Download failed: $R0"
	  Quit
	  
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "Seven Update" /F"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "Seven Update.Admin" /F"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "SevenUpdate" /F"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /delete /TN "SevenUpdate.Admin" /F"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /create /XML "$TEMP\SevenUpdate.xml" /TN "SevenUpdate"'
	  nsExec::Exec '"$SYSDIR\schtasks.exe" /create /XML "$TEMP\SevenUpdate.Admin.xml" /TN "SevenUpdate.Admin"'
  ${EndIf}
	
  File "D:\Documents\Software Development\Install Files\Seven Update\sua.ico"
  
  SetShellVarContext current
  CreateDirectory "$APPDATA\Seven Software\Seven Update"
  SetShellVarContext all
  CreateDirectory "$APPDATA\Seven Software\Seven Update"
  CreateDirectory "$SMPROGRAMS\Seven Software"
  
CreateShortCut "$SMPROGRAMS\Seven Software\Seven Update.lnk" "$INSTDIR\SevenUpdate.exe"
  
  WriteRegStr HKCR "sevenupdate" "" "URL:Seven Update Protocol"
  WriteRegStr HKCR "sevenupdate" "URL Protocol" ""
  WriteRegStr HKCR "sevenupdate\DefaultIcon" "" "SevenUpdate.exe,0"
  WriteRegStr HKCR "sevenupdate\shell\open\command" "" '"$INSTDIR\SevenUpdate.exe" "%1"'
  
  
  WriteRegStr HKCR ".sua" "" "SevenUpdate.sua"
  WriteRegStr HKCR "SevenUpdate.sua" "" "Seven Update Application Information"
  WriteRegStr HKCR "SevenUpdate.sua\DefaultIcon" "" "$INSTDIR\sua.ico"
  WriteRegStr HKCR "SevenUpdate.sua\shell\open\command" "" '"$INSTDIR\SevenUpdate.exe" "%1"'
	
SectionEnd

Section -Post
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