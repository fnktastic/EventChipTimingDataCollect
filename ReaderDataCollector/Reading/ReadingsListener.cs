using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector.Reading
{
    class ReadingsListener
    {
        private TcpClient _tcpClient;

        private readonly string _host;
        private readonly int _port;
        private readonly List<string> _readLines;
        private ObservableCollection<Read> _uiReads;
        private readonly IReadRepository _readRepository;
        private CancellationTokenSource _cancellationToken;
        private Task backgoundWorker;

        public ReadingsListener(string host, int port, ObservableCollection<Read> uiReads, IReadRepository readRepository, CancellationTokenSource cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _uiReads = uiReads;
            _readLines = new List<string>();
            _readRepository = readRepository;
            _host = host;
            _port = port;
        }

        private void CheckStringForReadings(string stringToCheck)
        {
            var readings = stringToCheck.Split('@');
            foreach (var reading in readings)
            {
                if (reading.Length > 0)
                {
                    var read = MappRead(reading);
                    if (read != null)
                    {
                        _readLines.Add(reading);
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            _uiReads.Insert(0,read);
                        }));

                        //_readRepository.SaveRead(read);
                    }
                }

            }

        }

        public static Read MappRead(string stringToMap)
        {            
            var array = stringToMap.Replace("@","").Split('#');
            if (array.Length == 9)
            {
                Console.WriteLine(stringToMap);
                return new Read()
                {
                    ID = 0,
                    EPC = array[1],
                    Time = array[2],
                    PeakRssiInDbm = array[3],
                    AntennaNumber = array[4],
                    ReaderNumber = array[5],
                    IpAddress = array[6],
                    UniqueReadingID = array[7],
                    TimingPoint = array[8]
                };
            }

            return null;
        }

        public Task StartReading()
        {
            _cancellationToken = new CancellationTokenSource();
            backgoundWorker = Task.Run(() => DoReadingWork(_cancellationToken.Token), _cancellationToken.Token);
            return backgoundWorker;
        }

        public void StopReading()
        {
            Thread.Sleep(300);
            _cancellationToken.Cancel();
        }

        private void DoReadingWork(CancellationToken cancellationToken)
        {
            byte[] data = new byte[10000];
            string stringData;
            try
            {
                using (_tcpClient = new TcpClient(_host, _port))
                using (NetworkStream ns = _tcpClient.GetStream())
                {
                    int recvieveLength;
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        ns.Flush();
                        data = new byte[10000];
                        recvieveLength = ns.Read(data, 0, data.Length);
                        stringData = Encoding.ASCII.GetString(data, 0, recvieveLength);
                        CheckStringForReadings(stringData);
                    } while (!cancellationToken.IsCancellationRequested);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Unable to connect to server");
                Thread.Sleep(250);
                DoReadingWork(cancellationToken);
                //DisposeTcpClient();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation canceled");
                DisposeTcpClient();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                DisposeTcpClient();
            }
        }

        private void DisposeTcpClient()
        {
            _tcpClient.Close();
            _tcpClient.Dispose();
        }
    }
}
