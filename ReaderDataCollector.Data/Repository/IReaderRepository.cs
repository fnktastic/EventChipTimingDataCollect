using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReaderDataCollector.Data.Model;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Repository
{
    public interface IReaderRepository
    {
        IEnumerable<Reader> Readers { get; }

        Task SaveReader(Reader reader);
    }
}
