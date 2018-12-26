using System;
using System.Net.Sockets;
using System.Text;

namespace ReaderDataCollector.Reading
{
    public class TcpDateTime
    {
        const int PORT = 5001;
        static TcpClient client = new TcpClient();

        public static string BoxTime { get; private set; }

        public static string GetBoxTime(string host, string action)
        {
            try
            {
                if (host.Equals("localhost"))
                    host = "127.0.0.1";

                client = new TcpClient(host, PORT);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(action);

                Console.WriteLine("Sending : " + action);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesReadLength = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);

                var time = Encoding.ASCII.GetString(bytesToRead, 0, bytesReadLength);

                Console.WriteLine("Received : " + BoxTime);
                client.Close();
                return time;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
                return string.Empty;
            }
        }

        public static string SetTime(string host, string action)
        {
            try
            {
                if (host.Equals("localhost"))
                    host = "127.0.0.1";

                client = new TcpClient(host, PORT);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(action);

                Console.WriteLine("Sending : " + action);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesReadLength = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);

                var time = Encoding.ASCII.GetString(bytesToRead, 0, bytesReadLength);

                Console.WriteLine("Received : " + BoxTime);
                client.Close();
                return time;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
                return string.Empty;
            }
        }
    }
}
