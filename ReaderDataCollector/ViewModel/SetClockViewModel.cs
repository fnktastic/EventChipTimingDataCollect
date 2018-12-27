using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.BoxReading;
using ReaderDataCollector.Model;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector.ViewModel
{
    public class SetClockViewModel : ViewModelBase
    {
        private readonly Reading _reading;
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

        public SetClockViewModel(Reading reading)
        {
            _reading = reading;
            ManualTime = DateTime.Now;
            GetBoxTime();
        }

        [PreferredConstructor]
        public SetClockViewModel() { }

        private DateTime ParseTime(string timeString)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            try
            {
                DateTime result = DateTime.ParseExact(timeString, Consts.TIME_FORMAT, provider);
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
            string timeString = TcpDateTime.GetBoxTime(_reading.Reader.Host, action);
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
                            CurrentBoxTime = _time.ToString(Consts.TIME_FORMAT);
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
                    string action = string.Format("t2@{0}", _manualTime.ToString(Consts.TIME_FORMAT));
                    TcpDateTime.SetTime(_reading.Reader.Host, action);
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
                    string action = string.Format("t2@{0}", DateTime.Now.ToString(Consts.TIME_FORMAT));
                    TcpDateTime.SetTime(_reading.Reader.Host, action);
                    _cancellationTokenSource.Cancel();
                    _timeWorker.Wait();
                    GetBoxTime();
                }));
            }
        }
    }
}
