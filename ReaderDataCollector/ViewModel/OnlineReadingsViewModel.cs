using GalaSoft.MvvmLight;
using System.ServiceModel;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReaderDataCollector.AtwService;
using ReaderDataCollector.BoxReading;
using Reading = ReaderDataCollector.Model.Reading;
using Read = ReaderDataCollector.Model.Read;
using Reader = ReaderDataCollector.Model.Reader;

namespace ReaderDataCollector.ViewModel
{
    public class OnlineReadingsViewModel : ViewModelBase
    {
        #region fields
        private readonly Context _context;
        private IReaderRepository _readerRepository;
        private IReadingRepository _readingRepository;
        private IReadRepository _readRepository;
        private List<Read> reads = new List<Read>();
        private List<Reading> readings = new List<Reading>();
        #endregion
        #region properies
        private Reading _selectedReading;
        public Reading SelectedReading
        {
            get { return _selectedReading; }
            set
            {
                _selectedReading = value;
                if (_selectedReading != null)
                    Reads = new ObservableCollection<Read>(reads.Where(x => x.ReadingID == _selectedReading.ID));
                RaisePropertyChanged("SelectedReading");
            }
        }

        private Reader _selectedReader;
        public Reader SelectedReader
        {
            get { return _selectedReader; }
            set
            {
                _selectedReader = value;
                Readings = new ObservableCollection<Reading>(_readingRepository.Readings.Where(x => x.ReaderID == _selectedReader.ID));
                RaisePropertyChanged("SelectedReader");
            }
        }

        private ObservableCollection<Reader> _readers;
        public ObservableCollection<Reader> Readers
        {
            get { return _readers; }
            set { _readers = value; RaisePropertyChanged("Readers"); }
        }

        private ObservableCollection<Reading> _readings;
        public ObservableCollection<Reading> Readings
        {
            get { return _readings; }
            set { _readings = value; RaisePropertyChanged("Readings"); }
        }

        private ObservableCollection<Read> _reads;
        public ObservableCollection<Read> Reads
        {
            get { return _reads; }
            set { _reads = value; RaisePropertyChanged("Reads"); }
        }

        private bool _isLoadingInProgress;
        public bool IsLoadingInProgress
        {
            get { return _isLoadingInProgress; }
            set { _isLoadingInProgress = value; RaisePropertyChanged("IsLoadingInProgress"); }
        }
        #endregion
        #region constructor
        public OnlineReadingsViewModel()
        {
            IsLoadingInProgress = false;
            _context = new Context();
            _readerRepository = new ReaderRepository(_context);
            _readingRepository = new ReadingRepository(_context);
            _readRepository = new ReadRepository(_context);
        }
        #endregion
        #region methods
        private void UpdateStatements()
        {
            IsLoadingInProgress = true;
            Task.Run(() =>
            {
                try
                {
                    var binding = new WSHttpBinding(SecurityMode.None);
                    var endpoint = new EndpointAddress(Consts.HttpUrl());

                    using (var channelFactory = new ChannelFactory<IReadingService>(binding, endpoint))
                    {
                        channelFactory.Credentials.UserName.UserName = "test";
                        channelFactory.Credentials.UserName.Password = "test123";

                        IReadingService service = null;
                        try
                        {
                            service = channelFactory.CreateChannel();
                            var races = service.GetAllRaces();
                            var readings = service.GetAllReadings();
                        }
                        catch (Exception ex)
                        {
                            (service as ICommunicationObject)?.Abort();
                            Debug.WriteLine(string.Format("{0}", ex.Message));
                        }
                    }
                    IsLoadingInProgress = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
        #endregion
        #region commands
        private RelayCommand _refreshDataCommand;
        public RelayCommand RefreshDataCommand
        {
            get
            {
                return _refreshDataCommand ?? (_refreshDataCommand = new RelayCommand(() =>
                {
                    UpdateStatements();
                }));
            }
        }

        #endregion
    }
}
