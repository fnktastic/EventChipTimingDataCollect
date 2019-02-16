using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReaderDataCollector.Data.Model;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Repository
{
    public interface IReadRepository
    {
        IEnumerable<Read> Reads { get; }

        Task SaveReadAsync(Read readType);
    }
}
