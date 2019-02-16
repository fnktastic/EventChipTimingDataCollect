using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Repository
{
    public interface IReadRepository
    {
        IEnumerable<Read> Reads { get; }
        Task SaveRead(Read read);
    }
}
