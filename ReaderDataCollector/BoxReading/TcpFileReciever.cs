using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.BoxReading
{
    public class TcpFileReciever
    {
        static TcpClient client = new TcpClient();

        public static bool GetFile(string fileName, string host)
        {
            try
            {
                if (host.Equals(Consts.LOCALHOST))
                    host = Consts.LOCALHOST_IP;

                client = new TcpClient(host, Consts.FILE_PORT);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(fileName);

                Console.WriteLine("Sending : " + fileName);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);

                int sizeOfFile = int.Parse(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                byte[] bytesOfFile = new byte[sizeOfFile];

                if (sizeOfFile != bytesOfFile.Length)
                    return false;

                nwStream.Read(bytesOfFile, 0, sizeOfFile);

                Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesOfFile, 0, sizeOfFile));
                Write(fileName, bytesOfFile);
                client.Close();

                return true;
            }
             catch (Exception ex)
            {
                Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
                return false;
            }
        }

        public static void Write(string fileName, byte[] bytes)
        {
            File.WriteAllBytes(fileName, bytes);
        }
    }
}
