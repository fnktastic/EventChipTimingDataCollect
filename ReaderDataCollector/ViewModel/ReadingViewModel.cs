using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
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
    public class ReadingViewModel : ViewModelBase
    {
        #region fields
        private readonly Context _context;
        private readonly IReadRepository _readRepository;
        private CancellationTokenSource _cancellationToken;
        private ReadingsListener readsListener;
        private List<Read> _entities = new List<Read>();
        #endregion

        #region properties
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
        #endregion

        #region constructor
        public ReadingViewModel(ObservableCollection<Read> reads)
        {
            Reads = reads;
            Reads.CollectionChanged += ContentCollectionChanged;
        }

        [PreferredConstructor]
        public ReadingViewModel()
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
        #endregion

        #region methods
        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TotalReadings = Reads.Count().ToString();
        }
        #endregion

        #region commands
        private Task tsk1, tsk2, tsk3;
        private CancellationTokenSource cncTok1, cncTok2, cncTok3;

        private void startreading(string tsk, CancellationTokenSource cncTok)
        {
            var _readsListener = new ReadingsListener(_host, int.Parse(_port), Reads, cncTok);
            var worker = _readsListener.StartReading(tsk);
        }

        private RelayCommand _startReadingCommand;
        public RelayCommand StartReadingCommand
        {
            get
            {
                return _startReadingCommand ?? (_startReadingCommand = new RelayCommand(() =>
                  {
                      tsk1 = new Task(() =>
                      {
                          cncTok1 = new CancellationTokenSource();
                          startreading("tsk1", cncTok1);
                      });
                      tsk1.Start();

                      Thread.Sleep(1500);

                      tsk2 = new Task(() =>
                      {
                          cncTok2 = new CancellationTokenSource();
                          startreading("tsk2", cncTok2);
                      });

                      tsk2.Start();

                      Thread.Sleep(3000);

                      tsk3 = new Task(() =>
                      {
                          cncTok3 = new CancellationTokenSource();
                          startreading("tsk3", cncTok3);
                      });

                      tsk3.Start();
                      /*readsListener = new ReadingsListener(_host, int.Parse(_port), Reads, _readRepository, _cancellationToken);
                      var worker = readsListener.StartReading();*/
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
                    /*readsListener.StopReading();*/
                    IsReadingInProgress = false;
                    cncTok1.Cancel();
                    cncTok3.Cancel();
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
        #endregion
    }
}