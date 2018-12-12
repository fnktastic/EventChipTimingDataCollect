using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Repository
{
    public class ReadRepository : IReadRepository
    {
        private readonly Context _context;

        public ReadRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Read> Reads => _context.Reads;

        public async void SaveRead(Read read)
        {
            _context.Reads.Add(read);
            await _context.SaveChangesAsync();
        }
    }
}
