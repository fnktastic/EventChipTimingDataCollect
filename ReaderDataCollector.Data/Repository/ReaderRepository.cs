using System;
using System.Collections.Generic;
using System.Linq;
using ReaderDataCollector.Data.Model;
using ReaderDataCollector.Data.DataAccess;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Repository
{
    public interface IReaderRepository
    {
        IEnumerable<Reader> Readers { get; }

        Task SaveReader(Reader reader);
    }

    public class ReaderRepository : IReaderRepository
    {
        private readonly Context _context;

        public ReaderRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Reader> Readers => _context
            .Readers
            .AsNoTracking()
            .ToList();

        public async Task SaveReader(Reader reader)
        {
            if(reader != null)
            {
                if(reader.Id == 0)
                {
                    _context.Readers.Add(reader);
                }
                else
                {
                    var dbEntry = _context.Readers.FirstOrDefault(x => x.Id == reader.Id);

                    if (dbEntry != null)
                    {
                        dbEntry.Host = reader.Host;
                        dbEntry.Port = reader.Port;
                    }

                    if(dbEntry == null)
                        _context.Readers.Add(reader);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
