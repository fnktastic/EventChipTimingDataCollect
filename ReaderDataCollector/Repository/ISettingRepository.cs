using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Repository
{
    public interface ISettingRepository
    {
        IEnumerable<Setting> Settings { get; }
        Task SaveSetting(Setting setting);
    }
}
