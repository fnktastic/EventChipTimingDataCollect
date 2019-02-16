using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Model
{
    public class Race
    {
        public Guid Id { get; set; }

        public Reading Reading { get; set; }

        public Reader Reader { get; set; }

        public List<Read> Reads { get; set; }
    }
}
