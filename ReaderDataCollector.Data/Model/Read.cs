using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Model
{
    public class Read
    {
        public Guid Id { get; set; }

        public string EPC { get; set; }

        public DateTime Time { get; set; }

        public string Signal { get; set; }

        public string AntennaNumber { get; set; }

        public Guid ReadingId { get; set; }

        public virtual Reading Reading { get; set; }
    }
}
