using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Repository
{
    public class SettingRepository : ISettingRepository
    {
        private readonly Context _context;

        public SettingRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Setting> Settings => _context
            .Settings
            .ToList();

        public async Task SaveSetting(Setting setting)
        {
            if (setting != null)
            {
                var dbEntry = _context.Settings.FirstOrDefault(x => x.Name == setting.Name);
                if (dbEntry != null)
                    dbEntry.Value = setting.Value;
                if (dbEntry == null)
                    _context.Settings.Add(setting);
            }

            await _context.SaveChangesAsync();
        }
    }
}
