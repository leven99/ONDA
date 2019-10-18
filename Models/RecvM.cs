﻿using SocketDA.ViewModels;

namespace SocketDA.Models
{
    internal class RecvModel : MainWindowBase
    {
        #region 接收区 - Header
        /// <summary>
        /// 接收区Header中的接收计数
        /// </summary>
        private int _RecvDataCount;
        public int RecvDataCount
        {
            get
            {
                return _RecvDataCount;
            }
            set
            {
                if (_RecvDataCount != value)
                {
                    _RecvDataCount = value;
                    RaisePropertyChanged(nameof(RecvDataCount));
                }
            }
        }

        /// <summary>
        /// 接收区Header中的 [保存中/已停止] 字符串
        /// </summary>
        private string _RecvAutoSave;
        public string RecvAutoSave
        {
            get
            {
                return _RecvAutoSave;
            }
            set
            {
                if (_RecvAutoSave != value)
                {
                    _RecvAutoSave = value;
                    RaisePropertyChanged(nameof(RecvAutoSave));
                }
            }
        }

        /// <summary>
        /// 接收区Header中的 [允许/暂停] 字符串
        /// </summary>
        private string _RecvEnable;
        public string RecvEnable
        {
            get
            {
                return _RecvEnable;
            }
            set
            {
                if (_RecvEnable != value)
                {
                    _RecvEnable = value;
                    RaisePropertyChanged(nameof(RecvEnable));
                }
            }
        }
        #endregion

        private string _RecvData;
        public string RecvData
        {
            get
            {
                return _RecvData;
            }
            set
            {
                if (_RecvData != value)
                {
                    _RecvData = value;
                    RaisePropertyChanged(nameof(RecvData));
                }
            }
        }

        /// <summary>
        /// 辅助区 - 十六进制接收
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
            RecvDataCount = 0;
            RecvAutoSave = string.Format(cultureInfo, "已停止");
            RecvEnable = string.Format(cultureInfo, " 提示：双击文本框更改接收状态 ");

            RecvData = string.Empty;
            HexRecv = false;
        }
    }
}
