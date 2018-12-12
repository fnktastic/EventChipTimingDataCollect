using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Reading;
using ReaderDataCollector.Repository;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading;

namespace ReaderDataCollector.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Context _context;
        private readonly IReadRepository _readRepository;
        private CancellationTokenSource _cancellationToken;
        private ReadingsListener readsListener;

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

        public MainViewModel()
        {
            Database.SetInitializer<Context>(new Initializer());
            _context = new Context();
            _readRepository = new ReadRepository(_context);
            _cancellationToken = new CancellationTokenSource();
            Reads = new ObservableCollection<Read>(_readRepository.Reads);

            Host = "localhost";
            Port = "10000";
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
    }
}