using SocketDA.ViewModels;
using System.Windows;

namespace SocketDA.Views
{
    /// <summary>
    /// WPFUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class WPFUpdate : Window
    {
        private readonly WPFUpdateViewModel wPFUpdateViewModel = null;

        public WPFUpdate()
        {
            InitializeComponent();

            wPFUpdateViewModel = new WPFUpdateViewModel();
            DataContext = wPFUpdateViewModel;
        }
    }
}
