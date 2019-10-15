using SocketDA.ViewModels;

namespace SocketDA.Models
{
    public class RecvModel : MainWindowBase
    {
        /// <summary>
        /// ������ - ʮ�����ƽ���
        /// </summary>
        private bool _HexRecv;
        public bool HexRecv
        {
            get
            {
                return _HexRecv;
            }
            set
            {
                if (_HexRecv != value)
                {
                    _HexRecv = value;
                    RaisePropertyChanged(nameof(HexRecv));
                }
            }
        }

        public void RecvDataContext()
        {
            HexRecv = false;
        }
    }
}
