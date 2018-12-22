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
using System.Globalization;
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

        private string _ip;
        public string IP
        {
            get { return _ip; }
            set { _ip = value; RaisePropertyChanged("IP"); }
        }

        private int _uniqueReads;
        public int UniqueReads
        {
            get { return _uniqueReads; }
            set { _uniqueReads = value; RaisePropertyChanged("UniqueReads"); }
        }

        private DateTime _started;
        public DateTime Started
        {
            get { return _started; }
            set { _started = value; RaisePropertyChanged("Started"); }
        }

        private DateTime _duration;
        public DateTime Duration
        {
            get { return _duration; }
            set { _duration = value; RaisePropertyChanged("Duration"); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }

        private string _recoveryFile;
        public string RecoveryFile
        {
            get { return _recoveryFile; }
            set { _recoveryFile = value; RaisePropertyChanged("RecoveryFile"); }
        }

        private Read _lastRead;
        public Read LastRead
        {
            get { return _lastRead; }
            set { _lastRead = value; RaisePropertyChanged("LastRead"); }
        }
        #endregion

        #region constructor
        public ReadingViewModel(Reader reader)
        {
            Reads = reader.Reads;
            LastRead = Reads.LastOrDefault();
            if (reader.Reads.Count == 0)
                TimingPoint = "<unknown>";
            if (reader.Reads.Count > 0)
                TimingPoint = reader.TimingPoint;
            TotalReadings = reader.TotalReadings.ToString();
            IP = LastRead.IpAddress;
            if (reader.IsConnected == true)
                Status = "Connected";
            else if (reader.IsConnected == true)
                Status = "Disconnected";
            else
                Status = "Waiting for the BOX...";
            if (reader.StartedDateTime != null)
            {
                Started = (DateTime)reader.StartedDateTime;
                Duration = DateTime.Now.AddTicks(-_started.Ticks);
            }
            RecoveryFile = reader.FileName;
            UniqueReads = GetUniqueReads();


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
            Duration = DateTime.Now.AddTicks(-_started.Ticks);
            UniqueReads = GetUniqueReads();
        }

        private int GetUniqueReads()
        {
            return _reads.Select(x => x.EPC).Distinct().Count();
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