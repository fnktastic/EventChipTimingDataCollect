using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tftp.Net;

namespace ReaderDataCollector.Reading
{
    public class FTPClient
    {
        private static AutoResetEvent TransferFinishedEvent = new AutoResetEvent(false);

        public static void Download(string fileName, string host)
        {
            //Setup a TftpClient instance
            var client = new TftpClient(host);

            //Prepare a simple transfer (GET test.dat)
            var transfer = client.Download(fileName);

            //Capture the events that may happen during the transfer
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinshed);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);

            //Start the transfer and write the data that we're downloading into a memory stream
            MemoryStream stream = new MemoryStream();
            transfer.Start(stream);

            //Wait for the transfer to finish
            TransferFinishedEvent.WaitOne();

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                stream.WriteTo(fileStream);
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
