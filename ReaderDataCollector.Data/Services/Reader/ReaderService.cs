using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Services
{
    public interface IReaderService
    {

    }

    public class ReaderService : IReaderService
    {
        private readonly Context _context;
        private readonly IReaderRepository _readerRepository;

        public ReaderService(Context context)
        {
            _context = context;
            _readerRepository = new ReaderRepository(_context);
        }
    }
}
