using ReaderDataCollector.BoxReading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Utils
{
    public class PathUtil
    {
        public static string GetReaderRecoveryFolder(string host)
        {
            string path = Path.Combine(Consts.RECOVERY_PATH, host);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Path.Combine(path);
        }

        public static string GetReaderRecoveryFilePath(string host, string fileName)
        {
            string path = Path.Combine(Consts.RECOVERY_PATH, host);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Path.Combine(path, fileName);
        }
    }
}
