using GalaSoft.MvvmLight;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.ViewModel
{
    public class StoredReadingsViewModel : ViewModelBase
    {
        #region fields
        private readonly Context _context;
        private readonly IReaderRepository _readerRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IReadRepository _readRepository;
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
        #endregion
        #region constructor
        public StoredReadingsViewModel()
        {
            _context = new Context();
            _readerRepository = new ReaderRepository(_context);
            _readingRepository = new ReadingRepository(_context);
            _readRepository = new ReadRepository(_context);
            Readers = new ObservableCollection<Reader>(_readerRepository.Readers);
        }
        #endregion
        #region methods

        #endregion
        #region commands

        #endregion

    }
}