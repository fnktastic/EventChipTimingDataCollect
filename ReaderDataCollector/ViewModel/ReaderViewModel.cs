using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.Model;
using ReaderDataCollector.Reading;
using ReaderDataCollector.View;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector.ViewModel
{
    public class Reader : ViewModelBase
    {
        #region fields

        #endregion

        #region properties
        public string FileName { get; private set; } = string.Empty;

        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("ID"); }
        }

        private string _host;
        public string Host
        {
            get { return _host; }
            set { _host = value; RaisePropertyChanged("Host"); }
        }

        private string _port;
        public string Port
        {
            get { return _port; }
            set { _port = value; RaisePropertyChanged("Port"); }
        }

        private int _totalReadings;
        public int TotalReadings
        {
            get { return _totalReadings; }
            set { _totalReadings = value; RaisePropertyChanged("TotalReadings"); }
        }

        private bool? _isConnected;
        public bool? IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; RaisePropertyChanged("IsConnected"); }
        }

        private ObservableCollection<Read> _reads;
        public ObservableCollection<Read> Reads
        {
            get { return _reads; }
            set { _reads = value; RaisePropertyChanged("Reads"); }
        }

        private string _lastRead;
        public string LastRead
        {
            get { return _lastRead; }
            set { _lastRead = value; RaisePropertyChanged("LastRead"); }
        }

        private string _lastReadTooltip;
        public string LastReadToolTip
        {
            get { return _lastReadTooltip; }
            set { _lastReadTooltip = value; RaisePropertyChanged("LastReadToolTip"); }
        }

        private string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set { _toolTip = value; RaisePropertyChanged("ToolTip"); }
        }

        private string _timingPoint;
        public string TimingPoint
        {
            get { return _timingPoint; }
            set { _timingPoint = value; RaisePropertyChanged("TimingPoint"); }
        }

        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Task Task { get; set; }
        #endregion

        #region constructor
        public Reader()
        {
            Reads = new ObservableCollection<Read>();
            Reads.CollectionChanged += ContentCollectionChanged;
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return string.Format("Reader {0}\nHost: {1}\nPort: {2}\nReads: {3}\nIs Connected: {4}\nLast Read: {5}", _id, _host, _port, _totalReadings, _isConnected, string.IsNullOrWhiteSpace(_lastRead) ? "<none>" : _lastRead);
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var lastRead = _reads.LastOrDefault();
            if (lastRead != null)
            {
                LastRead = lastRead.Time.Split(' ')[1];
                LastReadToolTip = lastRead.ToString();
                if (string.IsNullOrEmpty(FileName)) FileName = lastRead.TimingPoint;
                if (string.IsNullOrEmpty(TimingPoint)) TimingPoint = lastRead.TimingPoint.Split('_')?[0];
            }
            TotalReadings = _reads.Count();
            ToolTip = this.ToString();
        }
        #endregion
    }

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
                new Reader() { ID = 2, Host="localhost", Port = "10000", IsConnected = false }
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
                    var readingViewModel = new ReadingViewModel(reader.Reads);
                    if(reader.Reads.Count == 0)
                        readingViewModel.TimingPoint = "<unknown>";
                    if (reader.Reads.Count > 0)
                        readingViewModel.TimingPoint = reader.TimingPoint;
                    readingViewModel.TotalReadings = reader.TotalReadings.ToString();
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
                    _readers.Add(new Reader() { IsConnected = false, ID = maxIndex + 1 });
                }));
            }
        }
        #endregion
    }
}