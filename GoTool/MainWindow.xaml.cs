using GoTool.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.UI;
using Windows.Graphics;

namespace GoTool;
public sealed partial class MainWindow : WindowEx
{
    //**************************
    //定义SetWindowLong函数
    [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);
    //**************************
    //定义GetWindowLong函数
    [DllImport("user32.dll", EntryPoint = "GetWindowLongW")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    //**************************
    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hwnd, int msg,int wParam, int lParam);
    //**************************
    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow([In] IntPtr hmonitor);

    //**************************
    //定义全局变量
    public static class GlobalVar
    {
        public static IntPtr hWnd;

    }
    //**************************
    //定义常量
    public const int GWL_STYLE = -16;
    public const long MAXIMIZEBOX = 0x00010000L;
    //**************************

    public MainWindow()
    {
        InitializeComponent();
        //*************************************************************************************
        // Use 'this' rather than 'window' as variable if this is about the current window.获取窗口句柄
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        GlobalVar.hWnd = hWnd;
        Debug.WriteLine("***************");//测试代码
        Debug.WriteLine("hWnd:" + Convert.ToString(hWnd));//测试代码
        Debug.WriteLine("***************");//测试代码
        //*************************************************************************************
        //AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;//允许自定义标题栏
        var dpiForWindow = GetDpiForWindow(WinRT.Interop.WindowNative.GetWindowHandle(this));//获取系统DPI
        var windowRatio = (double)dpiForWindow / 96.0;
        //Debug.WriteLine(windowRatio);//测试代码
        RectInt32 MoveSpace;//定义RectInt32
        MoveSpace.X = (int)(45 * windowRatio);//返回框宽45
        MoveSpace.Y = 0;
        MoveSpace.Width = (int)(1080 * windowRatio);//标题栏宽度1080
        MoveSpace.Height = (int)(48 * windowRatio);//标题栏高度48
        AppWindow.TitleBar.SetDragRectangles(new RectInt32[] { MoveSpace});//设置拖动区域
        AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 0, 0);//设置默认背景色
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 0, 0, 0);//设置失焦背景色
        AppWindow.TitleBar.IconShowOptions = Microsoft.UI.Windowing.IconShowOptions.HideIconAndSystemMenu;//隐藏图标
        long style = GetWindowLong(hWnd, GWL_STYLE); //获取窗口风格
        style &= ~(MAXIMIZEBOX); //禁用最大化按钮
        SetWindowLong(hWnd, GWL_STYLE, style); //设置窗口风格
        //*************************************************************************************
    }
}