using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.BoxReading;
using ReaderDataCollector.Model;
using ReaderDataCollector.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector.ViewModel
{
    public class PingViewModel : ViewModelBase
    {
        private string _host;
        public string Host
        {
            get { return _host; }
            set { _host = value; RaisePropertyChanged("Host"); }
        }

        private string _pingInfo;
        public string PingInfo
        {
            get { return _pingInfo; }
            set { _pingInfo = value; RaisePropertyChanged("PingInfo"); }
        }

        public PingViewModel(Reading reading)
        {
            Host = string.Format("Pinging {0}", reading.Reader.Host);
            Task.Run(() =>
            {
                while (true)
                {
                    string info = string.Empty;
                    info += string.Format("{0}: Pinging {1}...", DateTime.Now.ToString(Consts.TIME_FORMAT), reading.Reader.Host);
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        PingInfo += info;
                    }));

                    info = string.Empty;

                    if (PingUtil.PingHost(reading.Reader.Host) == true)
                        info += string.Format(" OK");
                    else if (PingUtil.PingHostViaTcp(reading.Reader.Host, Consts.PING_PORT) == true)
                        info += string.Format(" OK");
                    else
                        info += string.Format(" Request Timeout.");

                    info += Environment.NewLine;
                    Thread.Sleep(50);

                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        PingInfo += info;
                    }));

                    Thread.Sleep(1500);
                }
            });
        }

        [PreferredConstructor]
        public PingViewModel() { }
    }
}
