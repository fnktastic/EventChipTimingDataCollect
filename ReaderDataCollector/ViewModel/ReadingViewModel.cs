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
        #endregion

        #region properties
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

        private string _timingPoint;
        public string TimingPoint
        {
            get { return _timingPoint; }
            set { _timingPoint = value; RaisePropertyChanged("TimingPoint"); }
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
            Reads = new ObservableCollection<Read>();
            Reads.CollectionChanged += ContentCollectionChanged;
        }
        #endregion

        #region methods
        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TotalReadings = Reads.Count().ToString();
        }
        #endregion

        #region commands
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