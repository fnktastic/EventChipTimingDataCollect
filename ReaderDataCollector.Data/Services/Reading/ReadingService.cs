using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Services
{
    public interface IReadingService
    {

    }

    public class ReadingService : IReadingService
    {
        private readonly Context _context;
        private readonly IReadingRepository _readingRepository;

        public ReadingService(Context context)
        {
            _context = context;
            _readingRepository = new ReadingRepository(_context);
        }
    }
}
