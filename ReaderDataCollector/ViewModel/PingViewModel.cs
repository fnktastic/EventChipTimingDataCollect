using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.BoxReading;
using ReaderDataCollector.Model;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

                    if (PingHost(reading.Reader.Host) == true)
                        info += string.Format(" OK");
                    else if (PingHostViaTcp(reading.Reader.Host, int.Parse(reading.Reader.Port)) == true)
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

        public static bool PingHostViaTcp(string hostUri, int portNumber)
        {
            try
            {
                using (var client = new TcpClient(hostUri, portNumber))
                    return true;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("Error pinging host:'" + hostUri + ":" + portNumber.ToString() + "'");
                return false;
            }
        }

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
