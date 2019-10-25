using SocketDA.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace SocketDA.Views
{
    public partial class MainWindow : Window
    {
        internal MainWindowViewModel mainWindowViewModel = null;

        public MainWindow()
        {
            InitializeComponent();

            Height = 635;
            Width = Height / 0.625;

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
            mainWindowViewModel.ExitWindow();

            Close();
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

        #region 视图
        /// <summary>
        /// 精简视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mainWindowViewModel.Reduced_Enable();
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
            mainWindowViewModel.Update();
        }

        /// <summary>
        /// Gitee 存储库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RPMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://gitee.com/leven9/SocketDA");
        }

        /// <summary>
        /// 报告问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssueMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://gitee.com/leven9/SocketDA/issues");
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

        #region TCP Server

        #region 打开/关闭网络
        private void TCPServerOnOffButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindowViewModel.TCPServerOpenCloseSocket();
        }
        #endregion

        #region 发送
        private void TCPServerSenfButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 发送文件
        private void TCPServerSendFileButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 路径选择
        private void TCPServerSaveReceButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清接收区
        private void TCPServerClearReceButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清发送区
        private void TCPServerClearSendButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清空计数
        private void TCPServerClearCountButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #endregion

        #region TCP Client

        #region 打开/关闭网络
        private void TCPClientOnOffButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindowViewModel.TCPClientOpenCloseSocket();
        }
        #endregion

        #region 发送
        private void TCPClientSenfButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 发送文件
        private void TCPClientSendFileButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 路径选择
        private void TCPClientSaveReceButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清接收区
        private void TCPClientClearReceButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清发送区
        private void TCPClientClearSendButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清空计数
        private void TCPClientClearCountButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #endregion

        #region UDP Server

        #region 打开/关闭网络
        private void UDPServerOnOffButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindowViewModel.UDPServerOpenCloseSocket();
        }
        #endregion

        #region 发送
        private void UDPServerSenfButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 发送文件
        private void UDPServerSendFileButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 路径选择
        private void UDPServerSaveReceButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清空接收
        private void UDPServerClearReceButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清空发送
        private void UDPServerClearSendButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清空计数
        private void UDPServerClearCountButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #endregion

        #region UDP Client

        #region 打开/关闭网络
        private void UDPClientOnOffButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindowViewModel.UDPServerOpenCloseSocket();
        }
        #endregion

        #region 发送
        private void UDPClientSenfButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 发送文件
        private void UDPClientSendFileButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 路径选择
        private void UDPClientSaveReceButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清空接收
        private void UDPClientClearReceButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清空发送
        private void UDPClientClearSendButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 清空计数
        private void UDPClientClearCountButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #endregion
    }
}
