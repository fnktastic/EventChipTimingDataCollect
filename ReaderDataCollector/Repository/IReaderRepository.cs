using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Repository
{
    public interface IReaderRepository
    {
        IEnumerable<Reader> Readers { get; }
        Task SaveReader(Reader reader);
    }
}
