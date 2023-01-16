using GoTool.ViewModels;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Principal;

using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Controls;

namespace GoTool.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public static string Path_App = System.AppDomain.CurrentDomain.BaseDirectory;//获取程序运行路径

    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
        //*************************************************************************************
        TextBox_Path.Text= ReadIniData("Config", "Path", "", Path_App + @"Config.ini");//读入CSGO路径
        //*************************************************************************************
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
            Button Button_Sender = (Button)sender;
            switch (Button_Sender.Name)
            {
                case "Button_Chose":
                //*************************************************************************************
                //Debug.WriteLine("我是傻逼");//测试代码
                FolderPicker Folder_Picker = new FolderPicker();
                WinRT.Interop.InitializeWithWindow.Initialize(Folder_Picker, MainWindow.GlobalVar.hWnd);
                var File =await Folder_Picker.PickSingleFolderAsync();
                //Debug.WriteLine(File.Path);//测试代码
                if (File != null)
                {
                    TextBox_Path.Text = File.Path;
                    WriteIniData("Config","Path",File.Path,Path_App+@"Config.ini");//写入CSGO路径
                }//如果获取路径不为空
                //*************************************************************************************
                break;
            }

    }

    private void TextBox_Path_TextChanged(object sender, TextChangedEventArgs e)
    {
        WriteIniData("Config", "Path", TextBox_Path.Text, Path_App + @"Config.ini");//写入CSGO路径
    }

    #region API函数声明
    [DllImport("kernel32")]//返回0表示失败，非0为成功
    private static extern long WritePrivateProfileString(string section, string key,
      string val, string filePath);
    [DllImport("kernel32")]//返回取得字符串缓冲区的长度
    private static extern long GetPrivateProfileString(string section, string key,
      string def, StringBuilder retVal, int size, string filePath);
    #endregion
    #region 读Ini文件
    /// <summary>
    /// 读取ini文件内容的方法
    /// </summary>
    /// <param name="Section">ini文件的节名</param>
    /// <param name="Key">ini文件对应节下的键名</param>
    /// <param name="NoText">ini文件对应节对应键下无内容时返回的值</param>
    /// <param name="iniFilePath">该ini文件的路径</param>
    /// <returns></returns>
    public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
    {
        if (File.Exists(iniFilePath))
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
            return temp.ToString();
        }
        else
        {
            return String.Empty;
        }
    }
    #endregion
    #region 写Ini文件
    /// <summary>
    /// 将内容写入指定的ini文件中
    /// </summary>
    /// <param name="Section">ini文件中的节名</param>
    /// <param name="Key">ini文件中的键</param>
    /// <param name="Value">要写入该键所对应的值</param>
    /// <param name="iniFilePath">ini文件路径</param>
    /// <returns></returns>
    public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
    {
        if (File.Exists(iniFilePath))
        {
            long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
            if (OpStation == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    #endregion
}
public class MFTScanner
{
    private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    private const uint GENERIC_READ = 0x80000000;
    private const int FILE_SHARE_READ = 0x1;
    private const int FILE_SHARE_WRITE = 0x2;
    private const int OPEN_EXISTING = 3;
    private const int FILE_READ_ATTRIBUTES = 0x80;
    private const int FILE_NAME_IINFORMATION = 9;
    private const int FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;
    private const int FILE_OPEN_FOR_BACKUP_INTENT = 0x4000;
    private const int FILE_OPEN_BY_FILE_ID = 0x2000;
    private const int FILE_OPEN = 0x1;
    private const int OBJ_CASE_INSENSITIVE = 0x40;
    private const int FSCTL_ENUM_USN_DATA = 0x900b3;

    [StructLayout(LayoutKind.Sequential)]
    private struct MFT_ENUM_DATA
    {
        public long StartFileReferenceNumber;
        public long LowUsn;
        public long HighUsn;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct USN_RECORD
    {
        public int RecordLength;
        public short MajorVersion;
        public short MinorVersion;
        public long FileReferenceNumber;
        public long ParentFileReferenceNumber;
        public long Usn;
        public long TimeStamp;
        public int Reason;
        public int SourceInfo;
        public int SecurityId;
        public FileAttributes FileAttributes;
        public short FileNameLength;
        public short FileNameOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IO_STATUS_BLOCK
    {
        public int Status;
        public int Information;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct UNICODE_STRING
    {
        public short Length;
        public short MaximumLength;
        public IntPtr Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct OBJECT_ATTRIBUTES
    {
        public int Length;
        public IntPtr RootDirectory;
        public IntPtr ObjectName;
        public int Attributes;
        public int SecurityDescriptor;
        public int SecurityQualityOfService;
    }

    //// MFT_ENUM_DATA
    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool DeviceIoControl(IntPtr hDevice, int dwIoControlCode, ref MFT_ENUM_DATA lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
    private static extern Int32 CloseHandle(IntPtr lpObject);

    [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int NtCreateFile(ref IntPtr FileHandle, int DesiredAccess, ref OBJECT_ATTRIBUTES ObjectAttributes, ref IO_STATUS_BLOCK IoStatusBlock, int AllocationSize, int FileAttribs, int SharedAccess, int CreationDisposition, int CreateOptions, int EaBuffer,
    int EaLength);

    [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int NtQueryInformationFile(IntPtr FileHandle, ref IO_STATUS_BLOCK IoStatusBlock, IntPtr FileInformation, int Length, int FileInformationClass);

    private IntPtr m_hCJ;
    private IntPtr m_Buffer;
    private int m_BufferSize;

    private string m_DriveLetter;

    private class FSNode
    {
        public long FRN;
        public long ParentFRN;
        public string FileName;

        public bool IsFile;

        public FSNode(long lFRN, long lParentFSN, string sFileName, bool bIsFile)
        {
            FRN = lFRN;
            ParentFRN = lParentFSN;
            FileName = sFileName;
            IsFile = bIsFile;
        }
    }

    private IntPtr OpenVolume(string szDriveLetter)
    {
        IntPtr hCJ = default(IntPtr);
        //// volume handle

        m_DriveLetter = szDriveLetter;
        hCJ = CreateFile(@"\\.\" + szDriveLetter, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

        return hCJ;
    }

    private void Cleanup()
    {
        if (m_hCJ != IntPtr.Zero)
        {
            // Close the volume handle.
            CloseHandle(m_hCJ);
            m_hCJ = INVALID_HANDLE_VALUE;
        }

        if (m_Buffer != IntPtr.Zero)
        {
            // Free the allocated memory
            Marshal.FreeHGlobal(m_Buffer);
            m_Buffer = IntPtr.Zero;
        }
    }

    public IEnumerable<String> EnumerateFiles(string szDriveLetter)
    {
        try
        {
            var usnRecord = default(USN_RECORD);
            var mft = default(MFT_ENUM_DATA);
            var dwRetBytes = 0;
            var cb = 0;
            var dicFRNLookup = new Dictionary<long, FSNode>();
            var bIsFile = false;

            // This shouldn't be called more than once.
            if (m_Buffer.ToInt32() != 0)
            {
                throw new Exception("invalid buffer");
            }

            // Assign buffer size
            m_BufferSize = 65536;
            //64KB

            // Allocate a buffer to use for reading records.
            m_Buffer = Marshal.AllocHGlobal(m_BufferSize);

            // correct path
            szDriveLetter = szDriveLetter.TrimEnd('\\');

            // Open the volume handle
            m_hCJ = OpenVolume(szDriveLetter);

            // Check if the volume handle is valid.
            if (m_hCJ == INVALID_HANDLE_VALUE)
            {
                string errorMsg = "Couldn't open handle to the volume.";
                if (!IsAdministrator())
                    errorMsg += "Current user is not administrator";
            }

            mft.StartFileReferenceNumber = 0;
            mft.LowUsn = 0;
            mft.HighUsn = long.MaxValue;

            do
            {
                if (DeviceIoControl(m_hCJ, FSCTL_ENUM_USN_DATA, ref mft, Marshal.SizeOf(mft), m_Buffer, m_BufferSize, ref dwRetBytes, IntPtr.Zero))
                {
                    cb = dwRetBytes;
                    // Pointer to the first record
                    IntPtr pUsnRecord = new IntPtr(m_Buffer.ToInt64() + 8);

                    while ((dwRetBytes > 8))
                    {
                        // Copy pointer to USN_RECORD structure.
                        usnRecord = (USN_RECORD)Marshal.PtrToStructure(pUsnRecord, usnRecord.GetType());

                        // The filename within the USN_RECORD.
                        string FileName = Marshal.PtrToStringUni(new IntPtr(pUsnRecord.ToInt64() + usnRecord.FileNameOffset), usnRecord.FileNameLength / 2);

                        bIsFile = !usnRecord.FileAttributes.HasFlag(FileAttributes.Directory);
                        dicFRNLookup.Add(usnRecord.FileReferenceNumber, new FSNode(usnRecord.FileReferenceNumber, usnRecord.ParentFileReferenceNumber, FileName, bIsFile));

                        // Pointer to the next record in the buffer.
                        pUsnRecord = new IntPtr(pUsnRecord.ToInt64() + usnRecord.RecordLength);

                        dwRetBytes -= usnRecord.RecordLength;
                    }

                    // The first 8 bytes is always the start of the next USN.
                    mft.StartFileReferenceNumber = Marshal.ReadInt64(m_Buffer, 0);
                }
                else
                {
                    break; // TODO: might not be correct. Was : Exit Do
                }
            } while (!(cb <= 8));

            // Resolve all paths for Files
            foreach (FSNode oFSNode in dicFRNLookup.Values.Where(o => o.IsFile))
            {
                string sFullPath = oFSNode.FileName;
                FSNode oParentFSNode = oFSNode;

                while (dicFRNLookup.TryGetValue(oParentFSNode.ParentFRN, out oParentFSNode))
                {
                    sFullPath = string.Concat(oParentFSNode.FileName, @"\", sFullPath);
                }
                sFullPath = string.Concat(szDriveLetter, @"\", sFullPath);

                yield return sFullPath;
            }
        }
        finally
        {
            //// cleanup
            Cleanup();
        }
    }

    public static bool IsAdministrator()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
public static class DriveInfoExtension
{
    public static IEnumerable<String> EnumerateFiles(this DriveInfo drive)
    {
        return (new MFTScanner()).EnumerateFiles(drive.Name);
    }
}