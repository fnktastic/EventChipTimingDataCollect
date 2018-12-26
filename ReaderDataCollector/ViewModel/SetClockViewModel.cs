using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.Model;
using ReaderDataCollector.Reading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector.ViewModel
{
    public class SetClockViewModel : ViewModelBase
    {
        private readonly Reader _reader;
        private DateTime _time;
        private Task _timeWorker;
        private CancellationTokenSource _cancellationTokenSource;

        private DateTime _manualTime;
        public DateTime ManualTime
        {
            get { return _manualTime; }
            set { _manualTime = value; RaisePropertyChanged("ManualTime"); }
        }

        private string _currentBoxTime;
        public string CurrentBoxTime
        {
            get { return _currentBoxTime; }
            set { _currentBoxTime = value; RaisePropertyChanged("CurrentBoxTime"); }
        }

        public SetClockViewModel(Reader reader)
        {
            _reader = reader;
            ManualTime = DateTime.Now;
            GetBoxTime();
        }

        [PreferredConstructor]
        public SetClockViewModel() { }

        private DateTime ParseTime(string timeString)
        {
            string format = "HH:mm:ss";
            CultureInfo provider = CultureInfo.InvariantCulture;

            try
            {
                DateTime result = DateTime.ParseExact(timeString, format, provider);
                return result;
            }
            catch (FormatException)
            {
                Console.WriteLine("{0} is not in the correct format.", timeString);
                return DateTime.Now;
            }
        }

        private void GetBoxTime()
        {
            string action = string.Format("t1@{0}", 1);
            string timeString = TcpDateTime.GetBoxTime(_reader.Host, action);
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                _time = ParseTime(timeString);
                _timeWorker = new Task(() =>
                {
                    do
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            CurrentBoxTime = _time.ToString("HH:mm:ss");
                        }));
                        _time = _time.AddSeconds(1);
                        Thread.Sleep(1000);
                    } while (!_cancellationTokenSource.Token.IsCancellationRequested);
                });

                _timeWorker.Start();
            }
            catch (FormatException)
            {
                Console.WriteLine("{0} is not in the correct format.", timeString);
            }
        }

        private RelayCommand _setManualTimeCommand;
        public RelayCommand SetManualTimeCommand
        {
            get
            {
                return _setManualTimeCommand ?? (_setManualTimeCommand = new RelayCommand(() =>
                {
                    string action = string.Format("t2@{0}", _manualTime.ToString("HH:mm:ss"));
                    TcpDateTime.SetTime(_reader.Host, action);
                    _cancellationTokenSource.Cancel();
                    _timeWorker.Wait();
                    GetBoxTime();
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
                    TcpDateTime.SetTime(_reader.Host, action);
                    _cancellationTokenSource.Cancel();
                    _timeWorker.Wait();
                    GetBoxTime();
                }));
            }
        }
    }
}
