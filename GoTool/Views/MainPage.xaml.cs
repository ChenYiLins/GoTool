using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using GoTool.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace GoTool.Views;

public sealed partial class MainPage : Page
{

    public static string Path_App = GetLeft(System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.BaseDirectory.Length - 4);//获取程序运行路径

    //**************************
    string[] Item_Mscfg;//定义全局变量Item_Mscfg
    string Text_AllMscfg;
    //**************************

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        //Debug.WriteLine(Path_App);//测试代码
        //*************************************************************************************
        if (Directory.Exists(Path_App + @"Mscfg\") == true)
        {
            Item_Mscfg = EnumerateSubdirectories(Path_App + @"Mscfg\");//获取Mscfg目录下的子文件夹
            //*************************************************************************************
            //object Test_Items = "我是傻逼";//测试代码
            //ListView.Items.Add(Test_Items);//测试代码
            for (int i = 0; i < Item_Mscfg.Length; i++)
            {
                if (ReadIniData("Config", "Function", "", Path_App + @"Mscfg\" + Item_Mscfg[i] + @"\Config.ini") == "true")
                {
                    string Text_Mscfg = System.IO.File.ReadAllText(Path_App + @"Mscfg\" + Item_Mscfg[i] + @"\Mscfg.txt");//读入启动项命令行
                    Text_AllMscfg = Text_AllMscfg + Text_Mscfg + " ";
                }
            }//管理ListView中应该被选定的项目
            //Debug.WriteLine("Text:"+Text_AllMscfg);//测试代码
            if (Text_AllMscfg != null)
            {
                Text_AllMscfg = GetLeft(Text_AllMscfg, Text_AllMscfg.Length - 1);
            }
            //Debug.WriteLine(Text_AllMscfg);//测试代码
        }

    }
    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Path = "";
        Process[] Path_Process = Process.GetProcessesByName("Steam");
        foreach (Process Path_Name in Path_Process)
        {
            Path = Path_Name.MainModule.FileName.ToString();
        }
        //Debug.WriteLine("Steam:" + Path);//测试代码
        await Task.Delay(200);//延迟执行启动控制台
        ProcessStartInfo Info = new ProcessStartInfo();
        Info.FileName = Path;
        if (Text_AllMscfg != null)
        {
            Info.Arguments = "-applaunch 730 " + Text_AllMscfg;
        }
        else
        {
            Info.Arguments = "-applaunch 730 ";
        }
        Process Process_Steam = Process.Start(Info);
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
    #region 枚举子目录
    /// <summary>
    /// 取一个文件夹下级子目录,不带路径<para/>
    /// <returns>返回子目录数组</returns>
    /// </summary>;
    public static string[] EnumerateSubdirectories(string Path)//枚举子目录
    {
        string[] Path_Part = Directory.GetDirectories(Path);
        string[] Path_Result = Path_Part;
        for (int i = 0; i < Path_Part.Length; i++)//取出子目录名称
        {
            Path_Result[i] = GetRight(Path_Part[i], Path_Part[i].Length - Path_App.Length - @"Mscfg\".Length);
        }
        return Path_Result;
    }
    #endregion
    #region 取文本左边
    public static string GetLeft(string str, int n)//取文本左边
    {
        string Temp = str.Substring(0, n);
        return Temp;
    }
    #endregion
    #region 取文本右边
    public static string GetRight(string str, int n)//取文本右边
    {
        string Temp = str.Substring(str.Length - n);
        return Temp;
    }
    #endregion

}
