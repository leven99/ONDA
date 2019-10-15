using SocketDA.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace SocketDA.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel mainWindowViewModel = null;

        public MainWindow()
        {
            InitializeComponent();

            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;
        }

        #region Menu Mouse Support
        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMove_Click(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        #endregion

        #region 菜单栏

        #region 文件
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 工具
        /// <summary>
        /// 计算器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalcMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("calc.exe");
        }
        #endregion

        #region 帮助
        /// <summary>
        /// 检查更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerUpMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Gitee 存储库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 报告问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssueMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }
        #endregion
    }
}
