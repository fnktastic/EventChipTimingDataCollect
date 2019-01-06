using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Repository
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly Context _context;

        public ReaderRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Reader> Readers => _context
            .Readers
            .ToList();

        public async void SaveReader(Reader reader)
        {
            _context.Readers.Add(reader);
            await _context.SaveChangesAsync();
        }
    }
}
