using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Repository
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly Context _context;
        private readonly IReaderRepository _readerRepository;
        private readonly IReadRepository _readRepository;

        public ReadingRepository(Context context)
        {
            _context = context;
            _readerRepository = new ReaderRepository(_context);
            _readRepository = new ReadRepository(_context);
        }

        public IEnumerable<Reading> Readings => _context
            .Readings
            .ToList();

        public async void SaveReading(Reading reading)
        {
            if (reading != null)
            {
                if (reading.ID == 0)
                {
                    var reader = _readerRepository
                        .Readers
                        .FirstOrDefault(r => r.Host == reading.Reader.Host && r.Port == reading.Reader.Port);

                    if (reader != null)
                    {
                        reading.Reader = null;
                        reading.ReaderID = reader.ID;
                    }

                    _context.Readings.Add(reading);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
