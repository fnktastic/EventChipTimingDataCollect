using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ReaderDataCollector.Reading
{
    public class TcpFileReciever
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static TcpClient client = new TcpClient();

        public static void GetFile(string fileName, string host)
        {
            try
            {
                if (host.Equals("localhost"))
                    host = "127.0.0.1";

                //---create a TCPClient object at the IP and port no.---
                client = new TcpClient(SERVER_IP, PORT_NO);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(fileName);

                //---send the text---
                Console.WriteLine("Sending : " + fileName);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                //---read back the text---
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);

                int sizeOfFile = int.Parse(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                byte[] bytesOfFile = new byte[sizeOfFile];
                nwStream.Read(bytesOfFile, 0, sizeOfFile);

                Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesOfFile, 0, sizeOfFile));
                Write(fileName, bytesOfFile);
                Console.ReadLine();
                client.Close();
            }
             catch (Exception ex)
            {
                Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        public static void Write(string fileName, byte[] bytes)
        {
            File.WriteAllBytes(fileName, bytes);
        }
    }
}
