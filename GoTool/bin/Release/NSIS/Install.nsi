; �ýű�ʹ�� HM VNISEdit �ű��༭���򵼲���

; ��װ�����ʼ���峣��
!define PRODUCT_NAME "GoTool"
!define PRODUCT_VERSION "3.1"
!define PRODUCT_PUBLISHER "ChenYiLin"
!define PRODUCT_WEB_SITE "https://github.com/ChenYiLins/GoTool"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

SetCompressor lzma

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "Setup.exe"
LoadLanguageFile "${NSISDIR}\Contrib\Language files\SimpChinese.nlf"
InstallDir "$PROGRAMFILES\GoTool"
Icon "D:\Project\Source\CSharp\GoTool\GoTool\Assets\WindowIcon.ico"
UninstallIcon "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"
DirText "��װ���򽫰�װ $(^Name) �������ļ��С�$\r$\n$\r$\nҪ��װ�ڲ�ͬ�ļ��У��������������ѡ�������ļ��С�"
ShowInstDetails show
ShowUnInstDetails show
BrandingText " "

Section "������" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File /r "D:\Project\Source\CSharp\GoTool\GoTool\bin\Release\GoTool\*.*"
SectionEnd

Section -AdditionalIcons
  CreateDirectory "$SMPROGRAMS\GoTool"
  CreateShortCut "$SMPROGRAMS\GoTool\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

/******************************
 *  �����ǰ�װ�����ж�ز���  *
 ******************************/

Section Uninstall
  Delete "$INSTDIR\uninst.exe"

  Delete "$SMPROGRAMS\GoTool\Uninstall.lnk"

  RMDir "$SMPROGRAMS\GoTool"

  RMDir /r "$INSTDIR\VideoRes"
  RMDir /r "$INSTDIR\Script"
  RMDir /r "$INSTDIR\Mscfg"
  RMDir /r "$INSTDIR\bin"

  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  SetAutoClose true
SectionEnd

#-- ���� NSIS �ű��༭�������� Function ���α�������� Section ����֮���д���Ա��ⰲװ�������δ��Ԥ֪�����⡣--#

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "��ȷʵҪ��ȫ�Ƴ� $(^Name) ���������е������" IDYES +2
  Abort
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) �ѳɹ��ش����ļ�����Ƴ���"
FunctionEnd
