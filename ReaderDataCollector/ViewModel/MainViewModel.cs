using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Reading;
using ReaderDataCollector.Repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReaderDataCollector.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Context _context;
        private readonly IReadRepository _readRepository;
        private CancellationTokenSource _cancellationToken;
        private ReadingsListener readsListener;
        private List<Read> _entities = new List<Read>();

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

        private ObservableCollection<Read> _reads;
        public ObservableCollection<Read> Reads
        {
            get { return _reads; }
            set { _reads = value; RaisePropertyChanged("Reads"); }
        }

        private string _totalReadings;
        public string TotalReadings
        {
            get { return _totalReadings; }
            set { _totalReadings = value; RaisePropertyChanged("TotalReadings"); }
        }

        public MainViewModel()
        {
            Database.SetInitializer(new Initializer());
            _context = new Context();
            _readRepository = new ReadRepository(_context);
            _cancellationToken = new CancellationTokenSource();
            Reads = new ObservableCollection<Read>(_readRepository.Reads);
            Reads.CollectionChanged += ContentCollectionChanged;

            Host = "localhost";
            Port = "10000";
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TotalReadings = Reads.Count().ToString();
        }

        private RelayCommand _startReadingCommand;
        public RelayCommand StartReadingCommand
        {
            get
            {
                return _startReadingCommand ?? (_startReadingCommand = new RelayCommand(() =>
                  {
                      readsListener = new ReadingsListener(_host, int.Parse(_port), Reads, _readRepository, _cancellationToken);
                      readsListener.StartReading();
                  }));
            }
        }

        private RelayCommand _stopReading;
        public RelayCommand StopReadingCommand
        {
            get
            {
                return _stopReading ?? (_stopReading = new RelayCommand(() =>
                {
                    readsListener.StopWorking();
                }));
            }
        }


        private RelayCommand _syncReadingsCommand;
        public RelayCommand SyncReadingsCommand
        {
            get
            {
                return _syncReadingsCommand ?? (_syncReadingsCommand = new RelayCommand(() =>
                {
                    foreach (var read in Reads)
                    {
                        _readRepository.SaveRead(read);
                    }
                }));
            }
        }

        private RelayCommand _downloadRecovery;
        public RelayCommand DownloadRecovery
        {
            get
            {
                return _downloadRecovery ?? (_downloadRecovery = new RelayCommand(() =>
                {
                    Task.Run(() => FTPClient.Download(string.Format("{0}.txt", _reads?.First()?.Salt)));
                }));
            }
        }
    }
}