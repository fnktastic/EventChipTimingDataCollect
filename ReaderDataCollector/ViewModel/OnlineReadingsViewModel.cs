using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.ServiceModel;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Repository;
using ReaderDataCollector.AtwService;
using ReaderDataCollector.BoxReading;
using ReaderDataCollector.Utils;
using Reading = ReaderDataCollector.Data.Model.Reading;
using Read = ReaderDataCollector.Data.Model.Read;
using Reader = ReaderDataCollector.Data.Model.Reader;

using ReaderDataCollector.View;
using AutoMapper;

namespace ReaderDataCollector.ViewModel
{
    public class OnlineReadingsViewModel : ViewModelBase
    {
        #region fields
        WSHttpBinding binding = null;
        EndpointAddress endpoint = null;
        ChannelFactory<IService> channelFactory = null;
        IService service = null;
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
                    Reads = new ObservableCollection<Read>(reads.Where(x => x.Id == _selectedReading.Id));
                RaisePropertyChanged("SelectedReading");
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

            InitService();
        }
        #endregion
        #region methods
        private void InitService()
        {
            binding = new WSHttpBinding(SecurityMode.None)
            {
                MaxReceivedMessageSize = 104857600
            };

            endpoint = new EndpointAddress(Consts.HttpUrl());          
        }

        private async Task UpdateStatementsAsync()
        {
            await GetDataFromServer();
        }

        private async Task GetDataFromServer()
        {
            await Task.Run(async () =>
             {
                 try
                 {
                     try
                     {
                         using (channelFactory = new ChannelFactory<IService>(binding, endpoint))
                         {
                             channelFactory.Credentials.UserName.UserName = string.Empty;
                             channelFactory.Credentials.UserName.Password = string.Empty;

                             service = channelFactory.CreateChannel();

                             while (true)
                             {
                                 IsLoadingInProgress = true;
                                 var allReaders = service.GetAllReaders();
                                 var asyncReadings = await service.GetAllReadingsAsync();
                                 Readings = new ObservableCollection<Reading>(asyncReadings
                                     .Select(x => Mapper.Map<Reading>(x))
                                     .OrderByDescending(x => x.StartedDateTime));

                                 await Task.Delay(TimeSpan.FromSeconds(SettingUtil.UpdatePeriod));
                                 IsLoadingInProgress = false;
                             }
                         }                          
                     }
                     catch (Exception ex)
                     {
                         (service as ICommunicationObject)?.Abort();
                         Debug.WriteLine(string.Format("{0}", ex.Message));
                         IsLoadingInProgress = false;
                     }                   
                 }
                 catch (Exception ex)
                 {
                     (service as ICommunicationObject)?.Abort();
                     Debug.WriteLine(string.Format("{0}", ex.Message));
                     IsLoadingInProgress = false;
                 }

                 IsLoadingInProgress = false;
             });
        }
        #endregion
        #region commands
        private RelayCommand _refreshDataCommand;
        public RelayCommand RefreshDataCommand
        {
            get
            {
                return _refreshDataCommand ?? (_refreshDataCommand = new RelayCommand(async () =>
                {
                    await UpdateStatementsAsync();
                }));
            }
        }

        private RelayCommand<Reading> _readingDetailsCommand;
        public RelayCommand<Reading> ReadingDetailsCommand
        {
            get
            {
                return _readingDetailsCommand ?? (_readingDetailsCommand = new RelayCommand<Reading>((reading) =>
                {                    
                    var onlineReadingDetailsViewModel = new OnlineReadingDetailsViewModel(null);
                    var window = new Window
                    {
                        Title = string.Format("Reading Details"),
                        Width = 450,
                        Height = 650,
                        Content = new OnlineReadingDetailsControl(),
                        ResizeMode = ResizeMode.NoResize,
                        DataContext = onlineReadingDetailsViewModel
                    };

                    window.ShowDialog();
                }));
            }
        }

        #endregion
    }
}
