using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reading = ReaderDataCollector.Data.Model.Reading;
using Read = ReaderDataCollector.Data.Model.Read;
using Reader = ReaderDataCollector.Data.Model.Reader;
using Race = ReaderDataCollector.Data.Model.Race;
using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Services;
using GalaSoft.MvvmLight.Command;

namespace ReaderDataCollector.ViewModel
{
    public class LocalReadingsViewModel : ViewModelBase
    {
        #region fields
        private readonly Context _context;
        private readonly IRaceService _raceService;
        private readonly IReaderService _readerService;
        private readonly IReadingService _readingService;
        private readonly IReadService _readService;
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
        public LocalReadingsViewModel()
        {
            _context = new Context();
            _raceService = new RaceService(_context);
            _readerService = new ReaderService(_context);
            _readingService = new ReadingService(_context);
            _readService = new ReadService(_context);

            Init();
        }
        #endregion

        #region methods
        private async void Init()
        {
            Readings = new ObservableCollection<Reading>(await _readingService.GetAllAsync());
        }
        #endregion

        #region commands
        private RelayCommand<Reading> _readingDetailsCommand;
        public RelayCommand<Reading> ReadingDetailsCommand
        {
            get
            {
                return _readingDetailsCommand ?? (_readingDetailsCommand = new RelayCommand<Reading>(async (reading) =>
                {
                    await Task.Delay(1000);
                }));
            }
        }

        private RelayCommand<Reading> _readingRemoveCommand;
        public RelayCommand<Reading> ReadingRemoveCommand
        {
            get
            {
                return _readingRemoveCommand ?? (_readingRemoveCommand = new RelayCommand<Reading>(async (reading) =>
                {
                    await Task.Delay(1000);
                }));
            }
        }

        private RelayCommand<Reading> _exportToExcelCommand;
        public RelayCommand<Reading> ExportToExcelCommand
        {
            get
            {
                return _exportToExcelCommand ?? (_exportToExcelCommand = new RelayCommand<Reading>(async (reading) =>
                {
                    await Task.Delay(1000);
                }));
            }
        }
        
        #endregion
    }
}
