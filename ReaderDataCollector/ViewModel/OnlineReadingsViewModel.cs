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
using Race = ReaderDataCollector.Data.Model.Race;

using ReaderDataCollector.View;
using AutoMapper;
using System.Threading;
using ReaderDataCollector.Data.Services;

namespace ReaderDataCollector.ViewModel
{
    public class OnlineReadingsViewModel : ViewModelBase
    {
        #region fields
        CancellationTokenSource cts = new CancellationTokenSource();
        WSHttpBinding binding = null;
        EndpointAddress endpoint = null;

        private readonly Context _context;
        private readonly IRaceService _raceService;

        private List<Read> reads = new List<Read>();
        private List<Reading> readings = new List<Reading>();
        private List<LastSeenLog> aliveLastSeenLogs = new List<LastSeenLog>();
        private List<LastSeenLog> pastLastSeenLogs = new List<LastSeenLog>();
        #endregion
        #region properties
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

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

        private bool _isShowLiveReadings;
        public bool IsShowLiveReadings
        {
            get { return _isShowLiveReadings; }
            set
            {
                _isShowLiveReadings = value;
                RaisePropertyChanged("IsShowLiveReadings");
            }
        }

        #endregion
        #region constructor
        public OnlineReadingsViewModel()
        {
            IsLoadingInProgress = false;
            _context = new Context();
            _raceService = new RaceService(_context);
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

        private async Task<List<LastSeenLog>> GetActualReadingsToShow(IService service)
        {
            if (_isShowLiveReadings == true)
            {
                aliveLastSeenLogs = (await service.GetAllAliveLastSyncLogsAsync()).ToList();
                return aliveLastSeenLogs;
            }

            if (_isShowLiveReadings == false)
            {
                pastLastSeenLogs = (await service.GetAllPastLastSyncLogsAsync()).ToList();
                return pastLastSeenLogs;
            }

            return null;
        }

        private async Task UpdateStatementsAsync()
        {
            await GetDataFromServer();
        }

        private async Task GetDataFromServer()
        {
            await Task.Run(async () =>
            {
                IService service = null;

                try
                 {                   
                     using (var channelFactory = new ChannelFactory<IService>(binding, endpoint))
                     {
                         channelFactory.Credentials.UserName.UserName = string.Empty;
                         channelFactory.Credentials.UserName.Password = string.Empty;

                         service = channelFactory.CreateChannel();

                         while (cts.IsCancellationRequested == false)
                         {
                            cts.Token.ThrowIfCancellationRequested();

                             IsLoadingInProgress = true;

                             var readingsToShow = await GetActualReadingsToShow(service);

                             var readings = await service.GetReadingsByIdsAsync(readingsToShow.Select(x => x.ReadingId).ToArray());

                             Readings = new ObservableCollection<Reading>(readings
                                 .Select(x => Mapper.Map<Reading>(x))
                                 .OrderByDescending(x => x.StartedDateTime));

                             IsLoadingInProgress = false;
                             await Task.Delay(TimeSpan.FromSeconds(SettingUtil.UpdatePeriod));
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     (service as ICommunicationObject)?.Abort();
                     Debug.WriteLine(string.Format("{0}", ex.Message));
                     IsLoadingInProgress = false;
                 }
             }, cts.Token);
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
                    using (var channelFactory = new ChannelFactory<IService>(binding, endpoint))
                    {
                        channelFactory.Credentials.UserName.UserName = string.Empty;
                        channelFactory.Credentials.UserName.Password = string.Empty;

                        IService service = channelFactory.CreateChannel();

                        var reads = service
                                    .GetAllReadsByReadingId(reading.Id)
                                    .Select(x => { return Mapper.Map<Read>(x); });

                        var onlineReadingDetailsViewModel = new OnlineReadingDetailsViewModel(reads);

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
                    }
                }));
            }
        }

        private RelayCommand<Reading> _saveReadingCommand;
        public RelayCommand<Reading> SaveReadingCommand
        {
            get
            {
                return _saveReadingCommand ?? (_saveReadingCommand = new RelayCommand<Reading>(async (reading) =>
                {
                    await Task.Run(async () =>
                    {
                        IsBusy = true;
                        using (var channelFactory = new ChannelFactory<IService>(binding, endpoint))
                        {
                            channelFactory.Credentials.UserName.UserName = string.Empty;
                            channelFactory.Credentials.UserName.Password = string.Empty;

                            IService service = channelFactory.CreateChannel();

                            var race = await service.GetRaceByReadingIdAsync(reading.Id);

                            await _raceService.SaveRaceAsync(Mapper.Map<Race>(race));
                        }
                        IsBusy = false;
                    }).ConfigureAwait(false);
                }));
            }
        }
        #endregion
    }
}
