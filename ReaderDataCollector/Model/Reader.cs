using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Threading;

namespace ReaderDataCollector.Model
{
    public class Reader : ViewModelBase
    {
        #region fields

        #endregion

        #region properties
        public string FileName { get; private set; } = string.Empty;

        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("ID"); }
        }

        private string _host;
        public string Host
        {
            get { return _host; }
            set { _host = value; RaisePropertyChanged("Host"); }
        }

        private string _port;
        public string Port
        {
            get { return _port; }
            set { _port = value; RaisePropertyChanged("Port"); }
        }

        private int _totalReadings;
        public int TotalReadings
        {
            get { return _totalReadings; }
            set { _totalReadings = value; RaisePropertyChanged("TotalReadings"); }
        }

        private bool? _isConnected;
        public bool? IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; RaisePropertyChanged("IsConnected"); }
        }

        private ObservableCollection<Read> _reads;
        public ObservableCollection<Read> Reads
        {
            get { return _reads; }
            set { _reads = value; RaisePropertyChanged("Reads"); }
        }

        private string _lastRead;
        public string LastRead
        {
            get { return _lastRead; }
            set { _lastRead = value; RaisePropertyChanged("LastRead"); }
        }

        private string _lastReadTooltip;
        public string LastReadToolTip
        {
            get { return _lastReadTooltip; }
            set { _lastReadTooltip = value; RaisePropertyChanged("LastReadToolTip"); }
        }

        private string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set { _toolTip = value; RaisePropertyChanged("ToolTip"); }
        }

        private string _timingPoint;
        public string TimingPoint
        {
            get { return _timingPoint; }
            set { _timingPoint = value; RaisePropertyChanged("TimingPoint"); }
        }

        private DateTime? _startedDateTime;
        public DateTime? StartedDateTime
        {
            get { return _startedDateTime; }
            set { _startedDateTime = value; RaisePropertyChanged("StartedDateTime"); }
        }

        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Task Task { get; set; }
        #endregion

        #region constructor
        public Reader()
        {
            Reads = new ObservableCollection<Read>();
            Reads.CollectionChanged += ContentCollectionChanged;
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return string.Format("Reader {0}\nHost: {1}\nPort: {2}\nReads: {3}\nIs Connected: {4}\nLast Read: {5}", _id, _host, _port, _totalReadings, _isConnected, string.IsNullOrWhiteSpace(_lastRead) ? "<none>" : _lastRead);
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var lastRead = _reads.LastOrDefault();
            if (lastRead != null)
            {
                LastRead = lastRead.Time.ToString("HH:mm:ss:fff", CultureInfo.InvariantCulture);
                LastReadToolTip = lastRead.ToString();
                if (string.IsNullOrEmpty(FileName)) FileName = lastRead.TimingPoint;
                if (string.IsNullOrEmpty(TimingPoint)) TimingPoint = lastRead.TimingPoint.Split('_')?[0];
            }
            TotalReadings = _reads.Count();
            ToolTip = this.ToString();
        }
        #endregion
    }
}
