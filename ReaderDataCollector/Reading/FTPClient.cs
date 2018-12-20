using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tftp.Net;

namespace ReaderDataCollector.Reading
{
    public class FTPClient
    {
        private static AutoResetEvent TransferFinishedEvent = new AutoResetEvent(false);
        private static MemoryStream ms = null;

        public static void Download(string fileName, string host)
        {
            try
            {
                var client = new TftpClient(host);

                var transfer = client.Download(fileName);

                transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
                transfer.OnFinished += new TftpEventHandler(transfer_OnFinshed);
                transfer.OnError += new TftpErrorHandler(transfer_OnError);

                ms = new MemoryStream();
                transfer.Start(ms);
                TransferFinishedEvent.WaitOne();
                ms.Seek(0, SeekOrigin.Begin);
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    ms = new MemoryStream(ms.ToArray());
                    ms.CopyTo(fs);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        static void transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            Console.WriteLine("Transfer running. Progress: " + progress);
        }

        static void transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            Console.WriteLine("Transfer failed: " + error);
            TransferFinishedEvent.Set();
        }

        static void transfer_OnFinshed(ITftpTransfer transfer)
        {
            Console.WriteLine("Transfer succeeded.");
            TransferFinishedEvent.Set();
        }
    }
}
