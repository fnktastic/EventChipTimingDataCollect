using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Services.Race
{
    public interface IRaceService
    {

    }

    public class RaceService : IRaceService
    {
        private readonly Context _context;
        private readonly IReaderRepository _readerRepository;
        private readonly IReadingRepository _readingRepository;
        private readonly IReadRepository _readRepository;

        public RaceService(Context context)
        {
            _context = context;
            _readerRepository = new ReaderRepository(_context);
            _readingRepository = new ReadingRepository(_context);
            _readRepository = new ReadRepository(_context);
        }
    }
}
