using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Services.Read
{
    public interface IReadService
    {

    }

    public class ReadService : IReadService
    {
        private readonly Context _context;
        private readonly IReadRepository _readRepository;

        public ReadService(Context context)
        {
            _context = context;
            _readRepository = new ReadRepository(_context);
        }
    }
}
