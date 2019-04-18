using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Model
{
    public class Reading
    {
        public Guid Id { get; set; }

        public string ReaderNumber { get; set; }

        public string IPAddress { get; set; }

        public string TimingPoint { get; set; }

        public int TotalReads { get; set; }

        public string FileName { get; set; }

        public DateTime? StartedDateTime { get; set; }

        public DateTime? EndedDateTime { get; set; }

        public int ReaderId { get; set; }

        public Reader Reader { get; set; }

        public ICollection<Read> Reads { get; set; }
    }
}
