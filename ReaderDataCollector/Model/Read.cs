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
    }
}
