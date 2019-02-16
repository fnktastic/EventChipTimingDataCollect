using ReaderDataCollector.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using ReaderDataCollector.DataAccess;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Utils
{
    public static class SettingUtil
    {
        private static readonly Context _context;
        private static readonly ISettingRepository _settingRepository;
        public static readonly int UpdatePeriod; 

        static SettingUtil()
        {
            _context = new Context();
            _settingRepository = new SettingRepository(_context);

            UpdatePeriod = int.Parse(GetUpdatePeriod());
        }

        private static string GetUpdatePeriod()
        {
            return _settingRepository.Settings.FirstOrDefault(x => x.Name == "Update Period").Value;
        }
    }
}
