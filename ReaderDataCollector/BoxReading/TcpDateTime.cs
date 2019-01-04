using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ReaderDataCollector.BoxReading
{
    public class TcpDateTime
    {
        static TcpClient client = new TcpClient();

        public static string GetBoxTime(string host, string action)
        {
            try
            {
                if (host.Equals(Consts.LOCALHOST))
                    host = Consts.LOCALHOST_IP;

                client = new TcpClient(host, Consts.TIME_PORT);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = Encoding.ASCII.GetBytes(action);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesReadLength = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                var time = Encoding.ASCII.GetString(bytesToRead, 0, bytesReadLength);
                client.Close();
                return time;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(GetBoxTime), ex.Message, ex.StackTrace));
                return string.Empty;
            }
        }

        public static string SetTime(string host, string action)
        {
            try
            {
                if (host.Equals(Consts.LOCALHOST))
                    host = Consts.LOCALHOST_IP;

                client = new TcpClient(host, Consts.TIME_PORT);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = Encoding.ASCII.GetBytes(action);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesReadLength = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                var time = Encoding.ASCII.GetString(bytesToRead, 0, bytesReadLength);
                client.Close();
                return time;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(SetTime), ex.Message, ex.StackTrace));
                return string.Empty;
            }
        }
    }
}
