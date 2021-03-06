﻿using ReaderDataCollector.Utils;
using System;
using System.Diagnostics;
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
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                int sizeOfFile = int.Parse(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                byte[] bytesOfFile = new byte[sizeOfFile];

                if (sizeOfFile != bytesOfFile.Length)
                    return false;

                nwStream.Read(bytesOfFile, 0, sizeOfFile);
                Write(fileName, bytesOfFile, host);
                client.Close();

                return true;
            }
             catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(GetFile), ex.Message, ex.StackTrace));
                return false;
            }
        }

        private static void Write(string fileName, byte[] bytes, string host)
        {
            try
            {
                File.WriteAllBytes(PathUtil.GetReaderRecoveryFilePath(host, fileName), bytes);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(Write), ex.Message, ex.StackTrace));
            }
        }
    }
}
