using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.BoxReading;
using ReaderDataCollector.Model;
using ReaderDataCollector.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector.ViewModel
{
    public class ReaderViewModel : ViewModelBase
    {
        #region fields
        #endregion

        #region properties
        private ObservableCollection<Reading> _readings;
        public ObservableCollection<Reading> Readings
        {
            get { return _readings; }
            set { _readings = value; RaisePropertyChanged("Readings"); }
        }
        #endregion

        #region constructor
        public ReaderViewModel()
        {
            Readings = new ObservableCollection<Reading>()
            {
                new Reading() { ID = 1, Reader = new Reader() { Host="127.0.0.1", Port = "10000" }, IsConnected = false },
                new Reading() { ID = 2, Reader = new Reader() { Host="192.168.1.102", Port = "10000" }, IsConnected = false }
            };
        }
        #endregion

        #region commands
        private RelayCommand<Reading> _openReadsWindowCommand;
        public RelayCommand<Reading> OpenReadsWindowCommand
        {
            get
            {
                return _openReadsWindowCommand ?? (_openReadsWindowCommand = new RelayCommand<Reading>((reading) =>
                {
                    var readingViewModel = new ReadingViewModel(reading);
                    var window = new Window
                    {
                        Title = string.Format("Reads | Reader {0} - Event Chip Timing", reading.ID),
                        Content = new ReadingsControl(),
                        DataContext = readingViewModel
                    };

                    window.ShowDialog();
                }));
            }
        }

        private RelayCommand<Reading> _openRewindWindowCommand;
        public RelayCommand<Reading> OpenRewindWindowCommand
        {
            get
            {
                return _openRewindWindowCommand ?? (_openRewindWindowCommand = new RelayCommand<Reading>((reading) =>
                {
                    var rewindViewModel = new RewindViewModel
                    {
                        Host = reading.Reader.Host,
                        FileName = reading.FileName,
                        RecievedReads = reading.Reads
                    };

                    var window = new Window
                    {
                        Title = string.Format("Rewind | Reader {0} - Event Chip Timing", reading.ID),
                        Height = 550,
                        Width = 820,
                        Content = new RewindControl(),
                        DataContext = rewindViewModel
                    };

                    window.ShowDialog();
                }));
            }
        }

        private RelayCommand<Reading> _startReaderCommand;
        public RelayCommand<Reading> StartReaderCommand
        {
            get
            {
                return _startReaderCommand ?? (_startReaderCommand = new RelayCommand<Reading>((reading) =>
                {
                    if (reading.IsConnected == false)
                    {
                        reading.CancellationTokenSource = new CancellationTokenSource();
                        reading.IsConnected = null;
                        reading.Task = new Task(() =>
                        {
                            var _readsListener = new ReadingsListener(reading.Reader.Host, int.Parse(reading.Reader.Port), reading.Reads, reading.CancellationTokenSource, reading);
                            _readsListener.StartReading();
                        });

                        if (reading.StartedDateTime == null)
                            reading.StartedDateTime = DateTime.UtcNow;

                        reading.Task.Start();
                    }
                    else
                    {
                        reading.CancellationTokenSource.Cancel();
                        reading.IsConnected = false;
                    }
                }));
            }
        }

        private RelayCommand _addNewReaderCommand;
        public RelayCommand AddNewReaderCommand
        {
            get
            {
                return _addNewReaderCommand ?? (_addNewReaderCommand = new RelayCommand(() =>
                {
                    int maxIndex = _readings.Max(x => x.ID);
                    _readings.Add(new Reading() { IsConnected = false, ID = maxIndex + 1, Reader = new Reader(), StartedDateTime = null });
                }));
            }
        }

        private RelayCommand<Reading> _pingCommand;
        public RelayCommand<Reading> PingCommand
        {
            get
            {
                return _pingCommand ?? (_pingCommand = new RelayCommand<Reading>((reading) =>
                {
                    var pingViewModel = new PingViewModel(reading);
                    var window = new Window
                    {
                        Title = string.Format("Ping | Reader {0} - Event Chip Timing", reading.ID),
                        Content = new PingControl(),
                        Height = 360,
                        Width = 340,
                        ResizeMode = ResizeMode.NoResize,
                        DataContext = pingViewModel
                    };

                    window.Show();
                }));
            }
        }

        private RelayCommand<Reading> _setClockCommand;
        public RelayCommand<Reading> SetClockCommand
        {
            get
            {
                return _setClockCommand ?? (_setClockCommand = new RelayCommand<Reading>((reading) =>
                {
                    var pingViewModel = new SetClockViewModel(reading);
                    var window = new Window
                    {
                        Title = string.Format("Set Clock | Reader {0} - Event Chip Timing", reading.ID),
                        Width = 350,
                        Height = 350,
                        Content = new SetClockControl(),
                        ResizeMode = ResizeMode.NoResize,
                        DataContext = pingViewModel
                    };

                    window.ShowDialog();
                }));
            }
        }
        #endregion
    }
}