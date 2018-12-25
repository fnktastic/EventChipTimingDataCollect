using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Model
{
    public class Read : ViewModelBase
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("ID"); }
        }

        private string _epc;
        public string EPC
        {
            get { return _epc; }
            set { _epc = value; RaisePropertyChanged("EPC"); }
        }

        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; RaisePropertyChanged("Time"); }
        }

        private string _peakRssiInDbm;
        public string PeakRssiInDbm
        {
            get { return _peakRssiInDbm; }
            set { _peakRssiInDbm = value; RaisePropertyChanged("PeakRssiInDbm"); }
        }

        private string _antennaNumber;
        public string AntennaNumber
        {
            get { return _antennaNumber; }
            set { _antennaNumber = value; RaisePropertyChanged("AntennaNumber"); }
        }

        private string _readerNumber;
        public string ReaderNumber
        {
            get { return _readerNumber; }
            set { _readerNumber = value; RaisePropertyChanged("ReaderNumber"); }
        }

        private string _ipAddress;
        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; RaisePropertyChanged("IpAddress"); }
        }

        private string _uniqueReadingID;
        public string UniqueReadingID
        {
            get { return _uniqueReadingID; }
            set { _uniqueReadingID = value; RaisePropertyChanged("UniqueReadingID"); }
        }

        private string _timingPoint;
        public string TimingPoint
        {
            get { return _timingPoint; }
            set { _timingPoint = value; RaisePropertyChanged("TimingPoint"); }
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}", ID, EPC, Time, PeakRssiInDbm, AntennaNumber, ReaderNumber, IpAddress, UniqueReadingID, TimingPoint);
        }
    }
}
