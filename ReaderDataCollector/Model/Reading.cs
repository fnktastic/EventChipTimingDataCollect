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
using System.ComponentModel.DataAnnotations.Schema;

namespace ReaderDataCollector.Model
{
    public class Reading : ViewModelBase
    {
        #region properties
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("ID"); }
        }

        private int _totalReadings;
        public int TotalReadings
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

        public string FileName { get; set; } = string.Empty;

        private DateTime? _startedDateTime;
        public DateTime? StartedDateTime
        {
            get { return _startedDateTime; }
            set { _startedDateTime = value; RaisePropertyChanged("StartedDateTime"); }
        } 

        private DateTime? _endedDateTime;
        public DateTime? EndedDateTime
        {
            get { return _endedDateTime; }
            set { _endedDateTime = value; RaisePropertyChanged("EndedDateTime"); }
        }

        private string _raceName;
        public string RaceName
        {
            get { return _raceName; }
            set { _raceName = value; RaisePropertyChanged("RaceName"); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; RaisePropertyChanged("UserName"); }
        }


        public int ReaderID { get; set; }
        public virtual Reader Reader { get; set; }

        private ObservableCollection<Read> _reads;
        public ObservableCollection<Read> Reads
        {
            get { return _reads; }
            set { _reads = value; RaisePropertyChanged("Reads"); }
        } 

        private string _lastRead;
        [NotMapped]
        public string LastRead
        {
            get { return _lastRead; }
            set { _lastRead = value; RaisePropertyChanged("LastRead"); }
        }

        private string _lastReadTooltip;
        [NotMapped]
        public string LastReadToolTip
        {
            get { return _lastReadTooltip; }
            set { _lastReadTooltip = value; RaisePropertyChanged("LastReadToolTip"); }
        }

        private string _toolTip;
        [NotMapped]
        public string ToolTip
        {
            get { return _toolTip; }
            set { _toolTip = value; RaisePropertyChanged("ToolTip"); }
        }

        private bool? _isConnected;
        [NotMapped]
        public bool? IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; RaisePropertyChanged("IsConnected"); }
        }

        private bool _isLoadingInProgress;
        [NotMapped]
        public bool IsLoadingInProgress
        {
            get { return _isLoadingInProgress; }
            set { _isLoadingInProgress = value; RaisePropertyChanged("IsLoadingInProgress"); }
        }

        private bool? _isFinished;
        [NotMapped]
        public bool? IsFinished
        {
            get { return _isFinished; }
            set { _isFinished = value; RaisePropertyChanged("IsFinished"); }
        }

        private int _number;
        [NotMapped]
        public int Number
        {
            get { return _number; }
            set { _number = value; RaisePropertyChanged("Number"); }
        }

        [NotMapped]
        public CancellationTokenSource CancellationTokenSource { get; set; }

        [NotMapped]
        public Task Task { get; set; }
        #endregion

        #region constructor
        public Reading()
        {
            Reads = new ObservableCollection<Read>();
            Reads.CollectionChanged += ContentCollectionChanged;
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return string.Empty;
            //return string.Format("Reader {0}\nHost: {1}\nPort: {2}\nReads: {3}\nIs Connected: {4}\nLast Read: {5}", _id, Reader.Host, Reader.Port, _totalReadings, _isConnected, string.IsNullOrWhiteSpace(_lastRead) ? "<none>" : _lastRead);
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var lastRead = _reads.LastOrDefault();
            if (lastRead != null)
            {
                LastRead = lastRead.Time.ToString("HH:mm:ss:fff", CultureInfo.InvariantCulture);
                LastReadToolTip = lastRead.ToString();
                if (string.IsNullOrEmpty(TimingPoint)) TimingPoint = lastRead.TimingPoint;
            }
            TotalReadings = _reads.Count();
            ToolTip = this.ToString();
        }
        #endregion
    }
}
