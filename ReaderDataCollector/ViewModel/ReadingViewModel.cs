using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.BoxReading;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
using ReaderDataCollector.Utils;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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

        private string duration;
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
        public ReadingViewModel(Reading reading)
        {
            Init(reading);
        }

        [PreferredConstructor]
        public ReadingViewModel()
        {            
            _context = new Context();
            _readRepository = new ReadRepository(_context);
            Reads = new ObservableCollection<Read>();
            Reads.CollectionChanged += ContentCollectionChanged;
        }
        #endregion

        #region methods
        private void Init(Reading reading)
        {
            try
            {
                Reads = reading.Reads;
                LastRead = Reads.LastOrDefault();

                Started = Consts.UNKNOWN;
                Duration = Consts.UNKNOWN;
                if (reading.StartedDateTime != null)
                {
                    startedDateTime = (DateTime)reading.StartedDateTime;
                    Started = startedDateTime.ToString("dd.MM.yy HH:mm:ss:fff", CultureInfo.InvariantCulture);
                }

                if (reading.Reads.Count == 0)
                    TimingPoint = Consts.UNKNOWN;

                if (reading.Reads.Count > 0)
                    TimingPoint = reading.TimingPoint;

                TotalReadings = reading.TotalReadings.ToString();
                IP = reading.Reader.Host;

                if (reading.IsConnected == true)
                {
                    Status = "Connected";
                }
                else if (reading.IsConnected == false)
                    Status = "Disconnected";
                else
                    Status = "Waiting for the BOX...";

                Task.Run(() =>
                {
                    while (true)
                    {
                        if (Started != Consts.UNKNOWN)
                        {
                            var tempDuration = DateTime.UtcNow.AddTicks(-startedDateTime.Ticks);
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                if (reading.IsConnected == true)
                                    Duration = tempDuration.ToString("HH:mm:ss:fff", CultureInfo.InvariantCulture);
                            }));
                        }

                        Thread.Sleep(100);
                    }
                });

                if (string.IsNullOrEmpty(reading.FileName))
                    RecoveryFile = Consts.UNKNOWN;
                else
                    RecoveryFile = reading.FileName;

                UniqueReads = GetUniqueReads();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(this.Init), ex.Message, ex.StackTrace));
            }
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                TotalReadings = Reads.Count().ToString();

                duration = (DateTime.UtcNow - startedDateTime).ToString();
                Duration = duration;
                UniqueReads = GetUniqueReads();
                LastRead = e.NewItems.Cast<Read>().FirstOrDefault();//SyncRoot;  
                dataGrid.ScrollIntoView(LastRead);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(ContentCollectionChanged), ex.Message, ex.StackTrace));
            }
        }

        private int GetUniqueReads()
        {
            try
            {
                return _reads.Select(x => x.EPC).Distinct().Count();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(GetUniqueReads), ex.Message, ex.StackTrace));
            }

            return 0;
        }
        #endregion

        #region commands

        private RelayCommand _openRecoveryFile;
        public RelayCommand OpenRecoveryFile
        {
            get
            {
                return _openRecoveryFile ?? (_openRecoveryFile = new RelayCommand(() =>
                {
                    if(!string.IsNullOrEmpty(_recoveryFile))
                    if (TcpFileReciever.GetFile(_recoveryFile, IP))
                        Process.Start(PathUtil.GetReaderRecoveryFilePath(IP, _recoveryFile));
                }));
            }
        }

        private RelayCommand _openFolderCommand;
        public RelayCommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? (_openFolderCommand = new RelayCommand(() =>
                {
                    if (!string.IsNullOrEmpty(_recoveryFile))
                        Process.Start(PathUtil.GetReaderRecoveryFolder(IP));
                }));
            }
        }
        

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