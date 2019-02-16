using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Model
{
    public class Reader
    {
        public int Id { get; set; }

        public string Host { get; set; }

        public string Port { get; set; }

        public virtual ICollection<Reading> Readings { get; set; }
    }
}
