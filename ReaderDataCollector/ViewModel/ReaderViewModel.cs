using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.BoxReading;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
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
        private readonly Context _context;
        private readonly IReadRepository _readRepository;
        private readonly IReaderRepository _readerRepository;
        private readonly IReadingRepository _readingRepository;
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
            _context = new Context();
            _readerRepository = new ReaderRepository(_context);
            _readingRepository = new ReadingRepository(_context);
            _readRepository = new ReadRepository(_context);

            // mock data
            Readings = new ObservableCollection<Reading>()
            {
                new Reading() { Number = 1, Reader = new Reader() { Host="127.0.0.1", Port = "10000" }, IsConnected = false },
                new Reading() { Number = 2, Reader = new Reader() { Host="192.168.1.102", Port = "10000" }, IsConnected = false }
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
                        Title = string.Format("Reads | Reader {0} - Event Chip Timing", reading.Number),
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
                        Title = string.Format("Rewind | Reader {0} - Event Chip Timing", reading.Number),
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
                    int maxIndex = _readings.Max(x => x.Number);
                    _readings.Add(new Reading() { IsConnected = false, Number = maxIndex + 1, Reader = new Reader(), StartedDateTime = null });
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
                        Title = string.Format("Ping | Reader {0} - Event Chip Timing", reading.Number),
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
                        Title = string.Format("Set Clock | Reader {0} - Event Chip Timing", reading.Number),
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

        private RelayCommand<Reading> _finishAndSaveCommand;
        public RelayCommand<Reading> FinishAndSaveCommand
        {
            get
            {
                return _finishAndSaveCommand ?? (_finishAndSaveCommand = new RelayCommand<Reading>((reading) =>
                {
                    // TODO HERE
                    reading.EndedDateTime = DateTime.UtcNow;
                    _readingRepository.SaveReading(reading);
                }));
            }
        }        
        #endregion
    }
}