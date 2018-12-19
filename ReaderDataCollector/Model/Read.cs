using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Model
{
    public class Read
    {
        public int ID { get; set; }
        public string EPC { get; set; }
        public string Time { get; set; }
        public string PeakRssiInDbm { get; set; }
        public string AntennaNumber { get; set; }
        public string ReaderNumber { get; set; }
        public string IpAddress { get; set; }
        public string UniqueReadingID { get; set; }
        public string TimingPoint { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}", ID, EPC, Time, PeakRssiInDbm, AntennaNumber, ReaderNumber, IpAddress, UniqueReadingID, TimingPoint);
        }
    }
}
