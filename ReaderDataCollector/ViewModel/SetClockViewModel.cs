using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.Model;
using ReaderDataCollector.Reading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.ViewModel
{
    public class SetClockViewModel : ViewModelBase
    {
        private readonly Reader _reader;

        public SetClockViewModel(Reader reader)
        {
            _reader = reader;
        }

        [PreferredConstructor]
        public SetClockViewModel() { }


        private RelayCommand _setManualTimeCommand;
        public RelayCommand SetManualTimeCommand
        {
            get
            {
                return _setManualTimeCommand ?? (_setManualTimeCommand = new RelayCommand(() =>
                {
                    string action = string.Format("t1@{0}", 1);
                    TcpDateTime.Send(_reader.Host, action);
                }));
            }
        }

        private RelayCommand _setSystemTimeCommand;
        public RelayCommand SetSystemTimeCommand
        {
            get
            {
                return _setSystemTimeCommand ?? (_setSystemTimeCommand = new RelayCommand(() =>
                {
                    string action = string.Format("t2@{0}", DateTime.Now.ToString("HH:mm:ss"));
                    TcpDateTime.Send(_reader.Host, action);
                }));
            }
        }
    }
}
