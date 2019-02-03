using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.BoxReading
{
    public class Consts
    {
        public const int PING_PORT = 5001;
        public const int TIME_PORT = 5001;
        public const int FILE_PORT = 5000;
        public const string LOCALHOST = "localhost";
        public const string LOCALHOST_IP = "127.0.0.1";
        public const string TIME_FORMAT = "HH:mm:ss";
        public const string TIME_MILLISECONDS_FORMAT = "HH:mm:ss:fff";
        public const string UNKNOWN = "<unknown>";
        public const string STOP = "!STOP!";
        public static string RECOVERY_PATH = Path.Combine(Path.Combine(Environment.CurrentDirectory, "recovery"));

        private const string HTTP_SERVICE_PATH = "ReadingService.svc"; //ReadingService.svc
        public static string HttpUrl(int port = 81, string host = "77.68.12.158")
        {
            return string.Format("http://{0}/{2}", host, port, HTTP_SERVICE_PATH);
        }
    }
}
