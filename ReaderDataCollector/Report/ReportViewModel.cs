using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reading = ReaderDataCollector.Data.Model.Reading;
using Read = ReaderDataCollector.Data.Model.Read;

namespace ReaderDataCollector.Report
{
    public class ReportViewModel
    {
        public Reading Reading { get; set; }

        public List<Read> Reads { get; set; }
    }
}
