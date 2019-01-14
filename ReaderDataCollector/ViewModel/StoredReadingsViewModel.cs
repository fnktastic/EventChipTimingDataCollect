using GalaSoft.MvvmLight;
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
using System.Windows;

namespace ReaderDataCollector.ViewModel
{
    public class StoredReadingsViewModel : ViewModelBase
    {
        #region fields
        private readonly Context _context;
        private IReaderRepository _readerRepository;
        private IReadingRepository _readingRepository;
        private IReadRepository _readRepository;
        #endregion
        #region properies
        private Reading _selectedReading;
        public Reading SelectedReading
        {
            get { return _selectedReading; }
            set
            {
                _selectedReading = value;
                Reads = new ObservableCollection<Read>(_readRepository.Reads.Where(x => x.ReadingID == _selectedReading.ID));
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
        public StoredReadingsViewModel()
        {
            IsLoadingInProgress = true;
            _context = new Context();
            _readerRepository = new ReaderRepository(_context);
            _readingRepository = new ReadingRepository(_context);
            _readRepository = new ReadRepository(_context);
            UpdateStatements();
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
                    Readers = new ObservableCollection<Reader>(_readerRepository.Readers);
                    if (Readings == null)
                        Readings = new ObservableCollection<Reading>();
                    if (_selectedReader != null)
                        Readings = new ObservableCollection<Reading>(_readingRepository.Readings.Where(x => x.ReaderID == _selectedReader.ID));
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