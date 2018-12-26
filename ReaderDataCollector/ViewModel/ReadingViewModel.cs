using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ReaderDataCollector.ViewModel
{
    public class ReadingViewModel : ViewModelBase
    {
        #region fields
        private const string UNKNOWN = "<unknown>";
        private readonly Context _context;
        private readonly IReadRepository _readRepository;
        private DataGrid dataGrid;
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

        private DateTime startedDateTime;
        private string _started;
        public string Started
        {
            get { return _started; }
            set { _started = value; RaisePropertyChanged("Started"); }
        }

        private DateTime duration;
        private string _duration;
        public string Duration
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
            Init(reader);
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
        private void Init(Reader reader)
        {
            Reads = reader.Reads;
            LastRead = Reads.LastOrDefault();

            Started = UNKNOWN;
            Duration = UNKNOWN;
            if (reader.StartedDateTime != null)
            {
                startedDateTime = (DateTime)reader.StartedDateTime;
                Started = startedDateTime.ToString("dd.MM.yy HH:mm:ss:fff", CultureInfo.InvariantCulture);
            }

            if (reader.Reads.Count == 0)
                TimingPoint = UNKNOWN;

            if (reader.Reads.Count > 0)
                TimingPoint = reader.TimingPoint;

            TotalReadings = reader.TotalReadings.ToString();
            IP = reader.Host;

            if (reader.IsConnected == true)
            {
                Status = "Connected";
            }
            else if (reader.IsConnected == false)
                Status = "Disconnected";
            else
                Status = "Waiting for the BOX...";

            Task.Run(() =>
            {
                while (true)
                {
                    if (Started != UNKNOWN)
                    {
                        var tempDuration = DateTime.UtcNow.AddTicks(-startedDateTime.Ticks);
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            if (reader.IsConnected == true)
                                Duration = tempDuration.ToString("HH:mm:ss:fff", CultureInfo.InvariantCulture);
                        }));
                    }

                    Thread.Sleep(100);
                }
            });

            if (string.IsNullOrEmpty(reader.FileName))
                RecoveryFile = UNKNOWN;
            else
                RecoveryFile = reader.FileName;

            UniqueReads = GetUniqueReads();
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                TotalReadings = Reads.Count().ToString();
                duration = DateTime.UtcNow.AddTicks(-startedDateTime.Ticks);
                Duration = duration.ToString("HH:mm:ss:fff", CultureInfo.InvariantCulture);
                UniqueReads = GetUniqueReads();
                LastRead = e.NewItems.Cast<Read>().FirstOrDefault();//SyncRoot;  
                dataGrid.ScrollIntoView(LastRead);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        private int GetUniqueReads()
        {
            return _reads.Select(x => x.EPC).Distinct().Count();
        }
        #endregion

        #region commands
        private RelayCommand<DataGrid> _dataGridLoadedCommand;
        public RelayCommand<DataGrid> DataGridLoadedCommand
        {
            get
            {
                return _dataGridLoadedCommand ?? (_dataGridLoadedCommand = new RelayCommand<DataGrid>((datagrid) =>
                {
                    dataGrid = datagrid;
                    if (dataGrid == null || dataGrid.ItemsSource == null) return;

                    var sourceCollection = dataGrid.ItemsSource as ObservableCollection<Read>;
                    if (sourceCollection == null) return;

                    sourceCollection.CollectionChanged +=
                        new NotifyCollectionChangedEventHandler(ContentCollectionChanged);

                }));
            }
        }
        #endregion
    }
}