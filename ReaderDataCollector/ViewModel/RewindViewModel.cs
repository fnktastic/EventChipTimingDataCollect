using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.BoxReading;
using ReaderDataCollector.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector.ViewModel
{
    public class RewindViewModel : ViewModelBase
    {
        #region fields
        private List<Read> _readingsFromFile;
        #endregion

        #region properties
        public string FileName { private get; set; }
        public ObservableCollection<Read> RecievedReads { get; set; }

        private ObservableCollection<Read> _reads;
        public ObservableCollection<Read> Reads
        {
            get { return _reads; }
            set { _reads = value; RaisePropertyChanged("Reads"); }
        }

        private int _showMode;
        public int ShowMode
        {
            get { return _showMode; }
            set
            {
                _showMode = value;
                Show();
                RaisePropertyChanged("ShowMode");
            }
        }

        private string _retrievedReadsCount;
        public string RetrievedReadsCount
        {
            get { return _retrievedReadsCount; }
            set { _retrievedReadsCount = value; RaisePropertyChanged("RetrievedReadsCount"); }
        }

        private bool _isDataExist;
        public bool IsDataExist
        {
            get { return _isDataExist; }
            set { _isDataExist = value; RaisePropertyChanged("IsDataExist"); }
        }

        private string _host;
        public string Host
        {
            get { return _host; }
            set { _host = value; RaisePropertyChanged("Host"); }
        }
        #endregion

        #region constructor
        public RewindViewModel()
        {
            Reads = new ObservableCollection<Read>();
            _readingsFromFile = new List<Read>();
            IsDataExist = false;
            ShowMode = 0;
        }
        #endregion

        #region methods
        private void Show()
        {
            if (_showMode == 1)
                ShowLostOnly();
            if (_showMode == 3)
                ShowInDateRange();
            if (_showMode == 3)
                ShowAll();
        }

        private void GetReadingsFromFile(string filePath)
        {
            _readingsFromFile.Clear();
            StreamReader fs = new StreamReader(filePath);
            string s = "";
            while (s != null)
            {
                s = fs.ReadLine();
                if (!string.IsNullOrEmpty(s))
                {
                    var read = ReadingsListener.MappRead(s);
                    if (read != null)
                        _readingsFromFile.Add(read);
                }
            }
        }
        private void ShowAll()
        {
            Reads = new ObservableCollection<Read>(_readingsFromFile);
            RetrievedReadsCount = string.Format("All Reads - {0}", _reads.Count());
        }

        private void ShowInDateRange()
        {
            Reads = new ObservableCollection<Read>(_readingsFromFile);
            RetrievedReadsCount = string.Format("In Range Reads - {0}", _reads.Count());
        }

        private void ShowLostOnly()
        {
            if (_readingsFromFile.Count > 0)
            {
                _reads.Clear();
                var lostReadings = new List<Read>();
                _readingsFromFile.ForEach((x) =>
                {
                    if ((RecievedReads.FirstOrDefault(y => y.UniqueReadingID == x.UniqueReadingID)) == null)
                        lostReadings.Add(x);
                });

                if (lostReadings.Count > 0)
                    AddToReads(lostReadings);
            }

            RetrievedReadsCount = string.Format("Lost Reads - {0}", _reads.Count());
        }

        private void ClearUIReads()
        {
            _reads.Clear();
        }

        private void AddToReads(IEnumerable<Read> reads)
        {
            foreach (var read in reads)
                _reads.Add(read);

            _reads.OrderByDescending(x => x.Time);
        }
        #endregion

        #region commands
        private RelayCommand _downloadRecovery;
        public RelayCommand DownloadRecovery
        {
            get
            {
                return _downloadRecovery ?? (_downloadRecovery = new RelayCommand(() =>
                {
                    if (string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(Host))
                    {
                        MessageBox.Show("The Reader is not running. \nRun the Reader first to get some Reads.", "Warning");
                        return;
                    }
                    IsDataExist = false;
                    Task.Run(() => TcpFileReciever.GetFile(FileName, Host))
                    .ContinueWith((i) =>
                    {
                        if (i.Result == true)
                        {
                            GetReadingsFromFile(FileName);
                            if (_readingsFromFile.Count > 0)
                            {
                                IsDataExist = true;
                                ShowMode = 3;
                                MessageBox.Show(string.Format("Recieved Reads - {0}", _readingsFromFile.Count), "Complete!");
                                Show();
                            }
                        }
                        if (i.Result == false)
                            MessageBox.Show(string.Format("Something went wrong. Please try again this operation."), "Error!");
                    });
                }));
            }
        }

        private RelayCommand _showCommand;
        public RelayCommand ShowCommand
        {
            get
            {
                return _showCommand ?? (_showCommand = new RelayCommand(() =>
                {
                    Show();
                }));
            }
        }

        private RelayCommand _clearCommand;
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new RelayCommand(() =>
                {
                    RetrievedReadsCount = string.Empty;
                    ClearUIReads();
                }));
            }
        }

        private RelayCommand _acceptDataCommand;
        public RelayCommand AcceptDataCommand
        {
            get
            {
                return _acceptDataCommand ?? (_acceptDataCommand = new RelayCommand(() =>
                {
                    if (_reads.Count > 0)
                    {
                        foreach (var read in _reads)
                        {
                            RecievedReads.Add(read);
                        }
                        MessageBox.Show(string.Format("{0} Reads has been accepted!", _reads.Count), "Complete!");
                        RetrievedReadsCount = string.Empty;
                        _reads.Clear();
                    }
                }));
            }
        }
        #endregion
    }
}
