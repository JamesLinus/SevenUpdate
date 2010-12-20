!define PRODUCT_NAME "Seven Update SDK"
!define PRODUCT_VERSION "1.2.0.3"
!define PRODUCT_PUBLISHER "Seven Software"
!define PRODUCT_WEB_SITE "http://sevenupdate.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\SevenUpdate.Sdk.exe"
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
!define MUI_ICON "..\Source\SevenUpdate.Sdk\icon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"


; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE ".\license.txt"
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\SevenUpdate.Sdk.exe"
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
	push "SevenUpdate.Sdk.exe"
  processwork::existsprocess
  pop $5
	IntCmp $5 0 done
	Goto prompt
prompt:
  MessageBox MB_RETRYCANCEL|MB_ICONSTOP '$(^Name) must be closed before installation can begin.$\r$\nPress "Retry" to automatically close $(^Name) and continue or cancel the installation.'  IDCANCEL BailOut
  push "SevenUpdate.Sdk.exe"
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
	push "SevenUpdate.Sdk.exe"
  processwork::existsprocess
  pop $5
	IntCmp $5 0 done
	Goto prompt
prompt:
  MessageBox MB_RETRYCANCEL|MB_ICONSTOP '$(^Name) must be closed before you can uninstall it.$\r$\nPress "Retry" to automatically close $(^Name) and continue or cancel the uninstallation.'  IDCANCEL BailOut
  push "SevenUpdate.Sdk.exe"
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
  ${If} ${RunningX64}
	SetRegView 64
  ${EndIf}
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
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdateSDK/SevenUpdate.Sdk.exe" "$INSTDIR\SevenUpdate.Sdk.exe"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdateSDK/SevenUpdate.Sdk.exe.config" "$INSTDIR\SevenUpdate.Sdk.exe.config"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdateSDK/SevenUpdate.Base.dll" "$INSTDIR\SevenUpdate.Base.dll"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdateSDK/System.Windows.dll" "$INSTDIR\System.Windows.dll"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdateSDK/protobuf-net.dll" "$INSTDIR\protobuf-net.dll"
  !insertmacro DownloadFile "http://sevenupdate.com/apps/SevenUpdateSDK/WPFLocalizeExtension.dll" "$INSTDIR\WPFLocalizeExtension.dll"
  
  DetailPrint "Creating shortcuts..."
  SetShellVarContext current
  CreateDirectory "$APPDATA\Seven Software\Seven Update SDK"
  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\Seven Software"
  CreateShortCut "$SMPROGRAMS\Seven Software\$(^Name).lnk" "$INSTDIR\SevenUpdate.Sdk.exe"
  DetailPrint "Registering filetypes..."
  WriteRegStr HKCR ".sui" "" "SevenUpdate.sui"
  WriteRegStr HKCR "SevenUpdate.sui" "" "Seven Update Information"
  WriteRegStr HKCR "SevenUpdate.sui\DefaultIcon" "" "$INSTDIR\SevenUpdate.Base.dll,0"
  Call RefreshShellIcons
  
  DetailPrint "Optimizing .Net framework..."
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" install "$INSTDIR\SevenUpdate.Sdk.exe" /queue:1 /nologo /silent'
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" update /queue:2 /nologo /silent'
SectionEnd

Section -Post
  ${If} ${RunningX64}
	SetRegView 64
  ${EndIf}
  
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\SevenUpdate.Sdk.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\SevenUpdate.Sdk.exe"
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
  
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" uninstall "$INSTDIR\SevenUpdate.exe" /nologo /silent'
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" uninstall "$INSTDIR\SevenUpdate.Admin.exe" /nologo /silent'
  nsExec::Exec '"C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe" uninstall "$INSTDIR\SevenUpdate.Helper.exe" /nologo /silent'
  
  nsExec::Exec '"FTYPE" SevenUpdate.SUI="$INSTDIR\SevenUpdate.Sdk.exe" %1 %*"'
  nsExec::Exec '"ASSOC" .sui=SevenUpdate.SUI"'
  
  Delete "$SMPROGRAMS\Seven Software\$(^Name).lnk"
  
  RMDir "$SMPROGRAMS\Seven Software"
  RMDir /r /REBOOTOK $INSTDIR
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