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
                if (read.Id == Guid.Empty)
                {
                    read.Id = Guid.NewGuid();
                }

                _context.Reads.Add(read);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveReadRangeAsync(IEnumerable<Read> reads)
        {
            foreach (var read in reads)
            {
                if (read != null)
                {
                    if (read.Id == Guid.Empty)
                    {
                        read.Id = Guid.NewGuid();
                    }

                    _context.Reads.Add(read);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
