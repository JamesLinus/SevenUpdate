!define PRODUCT_NAME "Seven Update SDK"
!define PRODUCT_VERSION "1.0.0.0"
!define PRODUCT_PUBLISHER "Seven Software"
!define PRODUCT_WEB_SITE "http://sevenupdate.sourceforge.net"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\Seven Update.SDK.exe"
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
!define MUI_ICON "D:\Documents\Visual Studio 2010\Projects\Seven Update\Seven Update.SDK\icon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"


; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE "D:\Documents\Software Development\Install Files\license.txt"
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\Seven Update.SDK.exe"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME}"
OutFile "Seven Update SDK Setup.exe"
InstallDir "$PROGRAMFILES64\Seven Software\Seven Update SDK"
ShowInstDetails show
ShowUnInstDetails show
RequestExecutionLevel admin

Function CloseSevenUpdate

Push $5

loop:
	push "Seven Update.SDK.exe"
  processwork::existsprocess
  pop $5
	IntCmp $5 0 done
	Goto prompt
prompt:
  MessageBox MB_RETRYCANCEL|MB_ICONSTOP 'Seven Update SDK must be closed before installation can begin.$\r$\nPress "Retry" to automatically close Seven Update SDK and continue or cancel the installation.'  IDCANCEL BailOut
  push "Seven Update.SDK.exe"
  processwork::KillProcess
  Sleep 1000
Goto loop

BailOut:
  Abort
	
done:
Pop $5

FunctionEnd

Function un.CloseSevenUpdate

Push $5

loop:
	push "Seven Update.SDK.exe"
  processwork::existsprocess
  pop $5
	IntCmp $5 0 done
	Goto prompt
prompt:
  MessageBox MB_RETRYCANCEL|MB_ICONSTOP 'Seven Update SDK must be closed before you can uninstall it.$\r$\nPress "Retry" to automatically close Seven Update SDK and continue or cancel the uninstallation.'  IDCANCEL BailOut
  push "Seven Update.SDK.exe"
  processwork::KillProcess
  Sleep 1000
Goto loop

BailOut:
  Abort

done:
Pop $5

FunctionEnd

Function .onInit
	${If} ${RunningX64}
		StrCpy $INSTDIR "$PROGRAMFILES64\Seven Software\Seven Update SDK"
	${Else}
		StrCpy $INSTDIR "$PROGRAMFILES\Seven Software\Seven Update SDK"
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
 
 Function RefreshShellIcons
  !define SHCNE_ASSOCCHANGED 0x08000000
  !define SHCNF_IDLIST 0
  System::Call 'shell32.dll::SHChangeNotify(i, i, i, i) v (${SHCNE_ASSOCCHANGED}, ${SHCNF_IDLIST}, 0, 0)'
FunctionEnd
 
Section "Main Section" SEC01
  SetOutPath $INSTDIR
	SetShellVarContext all
  SetOverwrite on
	SectionIn RO
	Call ConnectInternet
	!insertmacro CheckDotNET3Point5
	
	Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\Windows UI.dll"
  Delete "$INSTDIR\Seven Update.SDK.exe"
  Delete "$INSTDIR\Seven Update.SDK.exe.config"
  Delete "$INSTDIR\Seven Update.Library.dll"
	
	StrCpy $0 "$INSTDIR\Seven Update.SDK.exe"
	NSISdl::download http://ittakestime.org/su/apps/Seven%20Update%20SDK/Seven%20Update.SDK.exe $0
	Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
	MessageBox MB_OK "Download failed: $R0"
	Quit
	
	StrCpy $0 "$INSTDIR\Seven Update.SDK.exe.config"
  NSISdl::download http://ittakestime.org/su/apps/Seven%20Update%20SDK/Seven%20Update.SDK.exe.config $0
	Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
	MessageBox MB_OK "Download failed: $R0"
	Quit
	
	StrCpy $0 "$INSTDIR\Seven Update.Library.dll"
  NSISdl::download http://ittakestime.org/su/apps/Seven%20Update%20SDK/Seven%20Update.Library.dll $0
	Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
	MessageBox MB_OK "Download failed: $R0"
	Quit
	
	StrCpy $0 "$INSTDIR\Windows UI.dll"
  NSISdl::download http://ittakestime.org/su/apps/Seven%20Update%20SDK/Windows%20UI.dll $0
	Pop $R0 ;Get the return value
  StrCmp $R0 "success" +3
	MessageBox MB_OK "Download failed: $R0"
	Quit
	
	File "D:\Documents\Software Development\Install Files\Seven Shared\sui.ico"
	
	SetShellVarContext current
	CreateDirectory "$APPDATA\Seven Software\Seven Update SDK"
	SetShellVarContext all
	CreateDirectory "$SMPROGRAMS\Seven Software"
  CreateShortCut "$SMPROGRAMS\Seven Software\Seven Update SDK.lnk" "$INSTDIR\Seven Update.SDK.exe"
	
	WriteRegStr HKCR ".sui" "" "SevenUpdate.sui"
	WriteRegStr HKCR "SevenUpdate.sui" "" "Seven Update Information"
	WriteRegStr HKCR "SevenUpdate.sui\DefaultIcon" "" "$INSTDIR\sui.ico"
	WriteRegStr HKCR "SevenUpdate.sui\shell\open\command" "" '"$INSTDIR\Seven Update.SDK.exe" "%1"'
	
	Call RefreshShellIcons
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\Seven Update.SDK.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\Seven Update.SDK.exe"
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
	nsExec::Exec '"FTYPE" SevenUpdate.SUI="$INSTDIR\Seven Update.SDK.exe" %1 %*"'
	nsExec::Exec '"ASSOC" .sui=SevenUpdate.SUI"'
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\Windows UI.dll"
  Delete "$INSTDIR\Seven Update.SDK.exe"
  Delete "$INSTDIR\Seven Update.SDK.exe.config"
  Delete "$INSTDIR\Seven Update.Library.dll"
  Delete "$SMPROGRAMS\Seven Software\Seven Update SDK.lnk"

  RMDir "$SMPROGRAMS\Seven Software"
  RMDir /r "$INSTDIR"
	RMDir "$PROGRAMFILES64\Seven Software"
	RMDir "$PROGRAMFILES\Seven Software"
  
	SetShellVarContext current
	RMDir /r "$APPDATA\Seven Software\Seven Update SDK"
	RMDir "$APPDATA\Seven Software"
	
	SetShellVarContext all
	RMDir "$APPDATA\Seven Software"
  
	DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
	DeleteRegKey HKCR ".sui"
	DeleteRegKey HKCR "SevenUpdate.sui"
  SetAutoClose true
SectionEnd