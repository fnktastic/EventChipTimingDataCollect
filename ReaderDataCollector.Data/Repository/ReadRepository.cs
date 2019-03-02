using System;
using System.Collections.Generic;
using System.Linq;
using ReaderDataCollector.Data.Model;
using ReaderDataCollector.Data.DataAccess;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Repository
{
    public interface IReadRepository
    {
        IEnumerable<Read> Reads { get; }

        Task SaveReadAsync(Read readType);

        Task SaveReadRangeAsync(IEnumerable<Read> reads);

        Task DeleteByReadingId(Guid readingId);
    }

    public class ReadRepository : IReadRepository
    {
        private readonly Context _context;

        public ReadRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Read> Reads => _context
            .Reads
            .AsNoTracking()
            .ToList();

        public async Task SaveReadAsync(Read read)
        {
            if (read != null)
            {
                _context.Reads.Add(read);

                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveReadRangeAsync(IEnumerable<Read> reads)
        {
            if (reads != null)
            {
                foreach (var read in reads)
                {
                    _context.Reads.Add(read);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByReadingId(Guid readingId)
        {
            var reads = _context.Reads.Where(x => x.ReadingId == readingId);

            _context.Reads.RemoveRange(reads);

            await _context.SaveChangesAsync();
        }
    }
}
