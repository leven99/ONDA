using SocketDA.Models;

namespace SocketDA.ViewModels
{
    class MainWindowViewModel : MainWindowBase
    {
        public TimerModel TimerModel { get; set; }
        public HelpModel HelpModel { get; set; }

        #region 状态栏- 信息描述
        private string _DepictInfo;
        public string DepictInfo
        {
            get { return _DepictInfo; }
            set
            {
                if (_DepictInfo != value)
                {
                    _DepictInfo = value;
                    RaisePropertyChanged(nameof(DepictInfo));
                }
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            HelpModel = new HelpModel();
            HelpModel.HelpDataContext();

            TimerModel = new TimerModel();
            TimerModel.TimerDataContext();

            DepictInfo = string.Format(cultureInfo, "Socket调试助手");
        }
    }
}
