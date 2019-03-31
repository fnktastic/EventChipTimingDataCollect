using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reading = ReaderDataCollector.Data.Model.Reading;
using Read = ReaderDataCollector.Data.Model.Read;
using Reader = ReaderDataCollector.Data.Model.Reader;
using Race = ReaderDataCollector.Data.Model.Race;
using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Repository;
using GalaSoft.MvvmLight.Command;

namespace ReaderDataCollector.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        #region fields
        private readonly Context _context;
        private IReaderRepository _readerRepository;
        private IReadingRepository _readingRepository;
        private IReadRepository _readRepository;
        #endregion

        #region properties
        private int _totlaReadings;
        public int TotlaReadings
        {
            get { return _totlaReadings; }
            set { _totlaReadings = value; RaisePropertyChanged("TotlaReadings"); }
        }

        private int _totlaReads;
        public int TotlaReads
        {
            get { return _totlaReads; }
            set { _totlaReads = value; RaisePropertyChanged("TotlaReads"); }
        }
        private bool _isLoadingInProgress;
        public bool IsLoadingInProgress
        {
            get { return _isLoadingInProgress; }
            set { _isLoadingInProgress = value; RaisePropertyChanged("IsLoadingInProgress"); }
        }
        #endregion

        #region construct
        public StatisticsViewModel()
        {
            _context = new Context();
            _readerRepository = new ReaderRepository(_context);
            _readingRepository = new ReadingRepository(_context);
            _readRepository = new ReadRepository(_context);

            RefreshDataCommand.Execute(null);
            IsLoadingInProgress = false;
        }
        #endregion

        #region methods
        private RelayCommand _refreshDataCommand;
        public RelayCommand RefreshDataCommand
        {
            get
            {
                return _refreshDataCommand ?? (_refreshDataCommand = new RelayCommand(async () =>
                {
                    IsLoadingInProgress = true;

                    TotlaReadings = (await _readingRepository.ReadingsAsync()).Count();
                    TotlaReads = _readRepository.Reads.Count();

                    IsLoadingInProgress = false;
                }));
            }
        }
        #endregion

        #region commands
        #endregion
    }
}
