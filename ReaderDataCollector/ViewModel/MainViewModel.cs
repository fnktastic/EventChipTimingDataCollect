using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Reading;
using ReaderDataCollector.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

        private bool _isReadingInProgress;
        public bool IsReadingInProgress
        {
            get { return _isReadingInProgress; }
            set { _isReadingInProgress = value; RaisePropertyChanged("IsReadingInProgress"); }
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
            IsReadingInProgress = false;
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TotalReadings = Reads.Count().ToString();
        }

        private void GetReadingsFromFile(string filePath)
        {
            var readingsFromFile = new List<Read>();
            StreamReader fs = new StreamReader(filePath);
            string s = "";
            while (s != null)
            {
                s = fs.ReadLine();
                if (!string.IsNullOrEmpty(s))
                {
                    var read = ReadingsListener.MappRead(s);
                    if (read != null)
                        readingsFromFile.Add(read);
                }
            }

            if (readingsFromFile.Count == _reads.Count)
                return;

            if (readingsFromFile.Count > 0)
            {
                var lostReadings = new List<Read>();
                readingsFromFile.ForEach((x) =>
                {
                    if ((_reads.FirstOrDefault(y => y.UniqueReadingID == x.UniqueReadingID)) == null)
                        lostReadings.Add(x);
                });

                if (lostReadings.Count > 0)
                    AddToReads(lostReadings);
            }
        }

        private void AddToReads(IEnumerable<Read> reads)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                foreach (var read in reads)
                    _reads.Add(read);

                _reads.OrderByDescending(x => x.Time);
            }));
        }

        private RelayCommand _startReadingCommand;
        public RelayCommand StartReadingCommand
        {
            get
            {
                return _startReadingCommand ?? (_startReadingCommand = new RelayCommand(() =>
                  {
                      readsListener = new ReadingsListener(_host, int.Parse(_port), Reads, _readRepository, _cancellationToken);
                      var worker = readsListener.StartReading();
                      IsReadingInProgress = true;
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
                    readsListener.StopReading();
                    IsReadingInProgress = false;
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
                    string fileName = string.Format("{0}.txt", _reads?.First()?.TimingPoint);
                    Task.Run(() => FTPClient.Download(fileName, _host))
                    .ContinueWith((i) =>
                    {
                        GetReadingsFromFile(fileName);
                    });
                }));
            }
        }
    }
}