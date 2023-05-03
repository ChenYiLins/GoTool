using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using GoTool.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace GoTool.Views;

public sealed partial class CfgPage : Page
{
    public static string Path_App = GetLeft(System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.BaseDirectory.Length - 4);//获取程序运行路径

    public CfgViewModel ViewModel
    {
        get;
    }

    //**************************
    string[] Item_Cfg;//定义全局变量Item_Cfg
    string Path_CSGO = ReadIniData("Config", "Path", "", Path_App + @"bin\Config.ini");//读入CSGO路径
    //**************************
    //定义Dll文件
    [DllImport("kernel32.dll")]
    public static extern int WinExec(string exeName, int operType);
    //**************************

    public CfgPage()//CfgPage主函数
    {
        ViewModel = App.GetService<CfgViewModel>();
        InitializeComponent();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//注册编码
        //Debug.WriteLine(Path_App);//测试代码
        //Debug.WriteLine(MainWindow.GlobalVar.hWnd);//测试代码
        //*************************************************************************************
        if (Directory.Exists(Path_App + @"Script\") == true)
        {
            //Debug.WriteLine("我是傻逼");//测试代码
            Item_Cfg = EnumerateSubdirectories(Path_App + @"Script\");//获取Script目录下的子文件夹
            //*************************************************************************************
            //object Test_Items = "我是傻逼";//测试代码
            //ListView.Items.Add(Test_Items);//测试代码
            for (int i = 0; i < Item_Cfg.Length; i++)
            {
                ListView.Items.Add(Item_Cfg[i]);
            }//向ListView添加项目
            for (int i = 0; i < Item_Cfg.Length; i++)
            {
                if (ReadIniData("Config", "Function", "", Path_App + @"Script\" + Item_Cfg[i] + @"\Config.ini") == "true")
                {
                    ListView.SelectRange(new Microsoft.UI.Xaml.Data.ItemIndexRange(i, 1));
                }
            }//管理ListView中应该被选定的项目
            //*************************************************************************************
        }//如果能正确找到Script文件夹
        //Debug.WriteLine("***************");//测试代码
        //Debug.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);//测试代码
        //Debug.WriteLine("***************");//测试代码
        //*************************************************************************************
        //Debug.WriteLine("***************");//测试代码
        //Debug.WriteLine(Path_App + @"Script\");//测试代码
        //获取Script目录下的子目录
        //foreach(string Test_Items_Cfg in Item_Cfg) Debug.WriteLine(Test_Items_Cfg);//测试代码
        //Debug.WriteLine("***************");//测试代码
        //*************************************************************************************
    }

    private async void ListView_ItemClick(object sender, ItemClickEventArgs e)//ListView项被点击
    {
        //Debug.WriteLine("我是傻逼");//测试代码
        //Debug.WriteLine(e.ClickedItem);//测试代码
        //Debug.WriteLine(ListView.Items.IndexOf("一键跳投"));//测试代码
        if (ListView.SelectionMode == ListViewSelectionMode.Multiple)
        {

            //*************************************************************************************
            if (File.Exists(Path_CSGO + @"\csgo.exe") == true)//是否能找到CSGO文件
            {
                string Name_Cfg = ReadIniData("Config", "Cmd", "", Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Config.ini");
                if (ReadIniData("Config", "Function", "", Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Config.ini") == "true")
                {
                    WriteIniData("Config", "Function", "false", Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Config.ini");
                    File.Delete(Path_CSGO + @"\csgo\cfg\" + Name_Cfg);

                }//如果配置文件启用
                else
                {
                    WriteIniData("Config", "Function", "true", Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Config.ini");
                    if (File.Exists(Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\" + Name_Cfg) == true)
                    {
                        File.Copy(Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\" + Name_Cfg, Path_CSGO + @"\csgo\cfg\" + Name_Cfg, true);//复制脚本
                    }
                }//否则配置文件未启用
            }
            else
            {
                await Dialog_CSGOError.ShowAsync();
                ListView.DeselectRange(new Microsoft.UI.Xaml.Data.ItemIndexRange(ListView.Items.IndexOf(e.ClickedItem), 1));
            }

            //*************************************************************************************
        }//如果当前为编辑模式
        else
        {
            //Debug.WriteLine("我是傻逼");//测试代码
            //Debug.WriteLine(Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Console.exe");//测试代码
            if (File.Exists(Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Console.exe") == true)
            {
                //Debug.WriteLine(Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Console.exe " + Convert.ToString(MainWindow.GlobalVar.hWnd));//测试代码
                
                await Task.Delay(200);//延迟执行启动控制台

                //*******************************************************************************
                //File.WriteAllText(Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Run.bat","start "+ Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Console.exe",Encoding.GetEncoding("GBK"));
                //WinExec(Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Run.bat",0);
                //*******************************************************************************

                Process Process = Process.Start(Path_App + @"Script\" + Convert.ToString(e.ClickedItem) + @"\Console.exe");
                _ = Process.WaitForExitAsync();
            }//如果能找到脚本控制台
            else
            {
                await Dialog_CmdError.ShowAsync();
            }
        }//否则为设置模式
    }

    private void MenuFlyoutItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)//ListView右键菜单事件
    {
        if (sender is MenuFlyoutItem SelectedItem)//获取被点击的菜单项信息
        {
            if (SelectedItem.Tag.ToString() == "Edit")
            {
                //Debug.WriteLine("我是傻逼1号");//测试代码
                ListView.SelectionMode = ListViewSelectionMode.Multiple;//切换ListView为多选模式
                for (int i = 0; i < Item_Cfg.Length; i++)
                {
                    if (ReadIniData("Config", "Function", "", Path_App + @"\Script\" + Item_Cfg[i] + @"\Config.ini") == "true")
                    {
                        ListView.SelectRange(new Microsoft.UI.Xaml.Data.ItemIndexRange(i, 1));
                    }
                }//管理ListView中应该被选定的项目
            }//进入编辑模式
            else
            {
                //Debug.WriteLine("我是傻逼2号");//测试代码
                ListView.SelectionMode = ListViewSelectionMode.Single;//切换ListView为单选模式
            }//进入设置模式
        }
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
            Path_Result[i] = GetRight(Path_Part[i], Path_Part[i].Length - Path_App.Length - @"Script\".Length);
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
