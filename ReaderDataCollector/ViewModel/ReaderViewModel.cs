using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.Model;
using ReaderDataCollector.Reading;
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
        private ObservableCollection<Reader> _readers;
        public ObservableCollection<Reader> Readers
        {
            get { return _readers; }
            set { _readers = value; RaisePropertyChanged("Readers"); }
        }
        #endregion

        #region constructor
        public ReaderViewModel()
        {
            Readers = new ObservableCollection<Reader>()
            {
                new Reader() { ID = 1, Host="localhost", Port = "10000", IsConnected = false },
                new Reader() { ID = 2, Host="192.168.1.102", Port = "10000", IsConnected = false }
            };
        }
        #endregion

        #region commands
        private RelayCommand<Reader> _openReadsWindowCommand;
        public RelayCommand<Reader> OpenReadsWindowCommand
        {
            get
            {
                return _openReadsWindowCommand ?? (_openReadsWindowCommand = new RelayCommand<Reader>((reader) =>
                {
                    var readingViewModel = new ReadingViewModel(reader);
                    var window = new Window
                    {
                        Title = string.Format("Reads | Reader {0} - Event Chip Timing", reader.ID),
                        Content = new ReadingsControl(),
                        DataContext = readingViewModel
                    };

                    window.ShowDialog();
                }));
            }
        }

        private RelayCommand<Reader> _openRewindWindowCommand;
        public RelayCommand<Reader> OpenRewindWindowCommand
        {
            get
            {
                return _openRewindWindowCommand ?? (_openRewindWindowCommand = new RelayCommand<Reader>((reader) =>
                {
                    var rewindViewModel = new RewindViewModel
                    {
                        Host = reader.Host,
                        FileName = reader.FileName,
                        RecievedReads = reader.Reads
                    };

                    var window = new Window
                    {
                        Title = string.Format("Rewind | Reader {0} - Event Chip Timing", reader.ID),
                        Height = 550,
                        Width = 820,
                        Content = new RewindControl(),
                        DataContext = rewindViewModel
                    };

                    window.ShowDialog();
                }));
            }
        }

        private RelayCommand<Reader> _startReaderCommand;
        public RelayCommand<Reader> StartReaderCommand
        {
            get
            {
                return _startReaderCommand ?? (_startReaderCommand = new RelayCommand<Reader>((reader) =>
                {
                    if (reader.IsConnected != true)
                    {
                        reader.CancellationTokenSource = new CancellationTokenSource();
                        reader.IsConnected = null;
                        reader.Task = new Task(() =>
                        {
                            var _readsListener = new ReadingsListener(reader.Host, int.Parse(reader.Port), reader.Reads, reader.CancellationTokenSource, reader);
                            _readsListener.StartReading(string.Format("Reader {0} has been read.", reader.ID));
                        });

                        if (reader.StartedDateTime == null)
                            reader.StartedDateTime = DateTime.UtcNow;

                        reader.Task.Start();
                    }
                    if (reader.IsConnected == true)
                    {
                        reader.CancellationTokenSource.Cancel();
                        reader.IsConnected = false;
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
                    int maxIndex = _readers.Max(x => x.ID);
                    _readers.Add(new Reader() { IsConnected = false, ID = maxIndex + 1, StartedDateTime = null });
                }));
            }
        }

        private RelayCommand<Reader> _pingCommand;
        public RelayCommand<Reader> PingCommand
        {
            get
            {
                return _pingCommand ?? (_pingCommand = new RelayCommand<Reader>((Reader reader) =>
                {
                    var pingViewModel = new PingViewModel(reader);
                    var window = new Window
                    {
                        Title = string.Format("Ping | Reader {0} - Event Chip Timing", reader.ID),
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

        private RelayCommand<Reader> _setClockCommand;
        public RelayCommand<Reader> SetClockCommand
        {
            get
            {
                return _setClockCommand ?? (_setClockCommand = new RelayCommand<Reader>((Reader reader) =>
                {
                    var pingViewModel = new SetClockViewModel(reader);
                    var window = new Window
                    {
                        Title = string.Format("Set Clock | Reader {0} - Event Chip Timing", reader.ID),
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