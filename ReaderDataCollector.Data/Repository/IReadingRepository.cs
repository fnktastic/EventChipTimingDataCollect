using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReaderDataCollector.Data.Model;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Repository
{
    public interface IReadingRepository
    {
        Task<IEnumerable<Reading>> ReadingsAsync();

        Task SaveReading(Reading reading);

        Reading GetById(Guid Id);
    }
}
