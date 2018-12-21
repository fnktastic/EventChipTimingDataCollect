﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ReaderDataCollector.Reading
{
    public class TcpFileReciever
    {
        const int PORT = 5000;
        static TcpClient client = new TcpClient();

        public static void GetFile(string fileName, string host)
        {
            try
            {
                if (host.Equals("localhost"))
                    host = "127.0.0.1";

                client = new TcpClient(host, PORT);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(fileName);

                Console.WriteLine("Sending : " + fileName);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);

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
