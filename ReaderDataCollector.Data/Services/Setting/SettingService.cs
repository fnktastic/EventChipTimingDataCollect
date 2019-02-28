using ReaderDataCollector.Data.DataAccess;
using ReaderDataCollector.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Data.Services
{
    public interface ISettingService
    {

    }

    public class SettingService : ISettingService
    {
        private readonly Context _context;
        private readonly ISettingRepository _settingRepository;

        public SettingService(Context context)
        {
            _context = context;
            _settingRepository = new SettingRepository(_context);
        }
    }
}
