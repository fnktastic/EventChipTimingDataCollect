using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Repository
{
    public class ReadingRepository :IReadingRepository
    {
        private readonly Context _context;

        public ReadingRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Reading> Readings => _context.Readings;

        public async void SaveReading(Reading reading)
        {
            _context.Readings.Add(reading);
            await _context.SaveChangesAsync();
        }
    }
}
