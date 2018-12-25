using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
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

        public PingViewModel(Reader reader)
        {
            Host = string.Format("Pinging {0}", reader.Host);
            Task.Run(() =>
            {
                while (true)
                {
                    string info = string.Empty;
                    info += string.Format("Pinging {0}...", reader.Host);
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        PingInfo += info;
                    }));

                    info = string.Empty;

                    if (PingHost(reader.Host) == true)
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

        public static bool PingHost(string host)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(host);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
    }
}
