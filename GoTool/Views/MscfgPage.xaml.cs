using GoTool.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ICommand = System.Windows.Input.ICommand;

namespace GoTool.Views;

public sealed partial class MscfgPage : Page
{

    public static string Path_App = GetLeft(System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.BaseDirectory.Length - 4);//获取程序运行路径

    public MscfgViewModel ViewModel
    {
        get;
    }

    //**************************
    string[] Item_Mscfg;//定义全局变量Item_Mscfg
    //**************************

    public MscfgPage()
    {
        ViewModel = App.GetService<MscfgViewModel>();
        InitializeComponent();
        //*************************************************************************************
        if (Directory.Exists(Path_App + @"Mscfg\") == true)
        {
            //Debug.WriteLine("我是傻逼");//测试代码
            Item_Mscfg = EnumerateSubdirectories(Path_App + @"Mscfg\");//获取Mscfg目录下的子文件夹
            //*************************************************************************************
            //object Test_Items = "我是傻逼";//测试代码
            //ListView.Items.Add(Test_Items);//测试代码
            for (int i = 0; i < Item_Mscfg.Length; i++)
            {
                ListView.Items.Add(Item_Mscfg[i]);
            }//向ListView添加项目
            for (int i = 0; i < Item_Mscfg.Length; i++)
            {
                if (ReadIniData("Config", "Function", "", Path_App + @"Mscfg\" + Item_Mscfg[i] + @"\Config.ini") == "true")
                {
                    ListView.SelectRange(new Microsoft.UI.Xaml.Data.ItemIndexRange(i, 1));
                    string Text_Mscfg = System.IO.File.ReadAllText(Path_App + @"Mscfg\" + Item_Mscfg[i]+@"\Mscfg.txt");//读入启动项命令行
                    TextBox.Text = TextBox.Text + Text_Mscfg + " ";
                }
            }//管理ListView中应该被选定的项目
            if (TextBox.Text != "")
            {
                TextBox.Text = GetLeft(TextBox.Text, TextBox.Text.Length - 1);
            }
            //*************************************************************************************
        }//如果能正确找到Mscfg文件夹
    }

    private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (ListView.SelectionMode == ListViewSelectionMode.Multiple)
        {

            //*************************************************************************************
            if (ReadIniData("Config", "Function", "", Path_App + @"Mscfg\" + Convert.ToString(e.ClickedItem) + @"\Config.ini") == "true")
            {
                WriteIniData("Config", "Function", "false", Path_App + @"Mscfg\" + Convert.ToString(e.ClickedItem) + @"\Config.ini");
                string Text_Mscfg = System.IO.File.ReadAllText(Path_App + @"Mscfg\" + Convert.ToString(e.ClickedItem) + @"\Mscfg.txt");//读入启动项命令行
                TextBox.Text = TextBox.Text.Replace(" " + Text_Mscfg, "");
                TextBox.Text = TextBox.Text.Replace(Text_Mscfg, "");

            }//如果配置文件启用
            else
            {
                WriteIniData("Config", "Function", "true", Path_App + @"Mscfg\" + Convert.ToString(e.ClickedItem) + @"\Config.ini");
                string Text_Mscfg = System.IO.File.ReadAllText(Path_App + @"Mscfg\" + Convert.ToString(e.ClickedItem) + @"\Mscfg.txt");//读入启动项命令行
                if (TextBox.Text == "")
                {
                    TextBox.Text = Text_Mscfg;
                }
                else
                {
                    TextBox.Text = TextBox.Text + " " + Text_Mscfg;
                }

            }//否则配置文件未启用

            //*************************************************************************************
        }//如果当前为编辑模式
        else
        {
            if (File.Exists(Path_App + @"Mscfg\" + Convert.ToString(e.ClickedItem) + @"\Console.exe") == true)
            {
                await Task.Delay(200);//延迟执行启动控制台
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.FileName = Path_App + @"Mscfg\" + Convert.ToString(e.ClickedItem) + @"\Console.exe";
                Info.Arguments = Convert.ToString(MainWindow.GlobalVar.hWnd);
                Process Process = Process.Start(Info);
                Process.WaitForExit();
            }//如果能找到启动项控制台
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
                ListView.SelectionMode = ListViewSelectionMode.Multiple;//切换ListView为多选模式
                for (int i = 0; i < Item_Mscfg.Length; i++)
                {
                    if (ReadIniData("Config", "Function", "", Path_App + @"\Mscfg\" + Item_Mscfg[i] + @"\Config.ini") == "true")
                    {
                        ListView.SelectRange(new Microsoft.UI.Xaml.Data.ItemIndexRange(i, 1));
                    }
                }//管理ListView中应该被选定的项目
            }//进入编辑模式
            else
            {
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
