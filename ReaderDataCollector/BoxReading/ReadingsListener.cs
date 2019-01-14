using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
using ReaderDataCollector.Utils;
using ReaderDataCollector.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReaderDataCollector.BoxReading
{
    class ReadingsListener
    {
        private System.Net.Sockets.TcpClient _tcpClient;

        private readonly string _host;
        private readonly int _port;
        private readonly List<string> _readLines;
        private ObservableCollection<Read> _uiReads;
        private CancellationTokenSource _cancellationToken;
        private Reading _reading;
        private Task backgoundWorker;

        public ReadingsListener(string host, int port, ObservableCollection<Read> uiReads, CancellationTokenSource cancellationToken, Reading reading = null)
        {
            _cancellationToken = cancellationToken;
            _uiReads = uiReads;
            _readLines = new List<string>();
            _host = host;
            _port = port;
            _reading = reading;
        }

        private void SetRecoveryFileName(string recoveryFileName)
        {
            _reading.FileName = recoveryFileName;
        }

        private void CheckStringForReadings(string stringToCheck)
        {
            try
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
                                _uiReads.Add(read);
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(CheckStringForReadings), ex.Message, ex.StackTrace));
            }
        }

        public static Read MappRead(string stringToMap)
        {
            try
            {
                var array = stringToMap.Replace("@", "").Split('#');
                if (array.Length == 9)
                {
                    return new Read()
                    {
                        ID = 0,
                        EPC = array[1],
                        Time = DateTime.Parse(array[2]),
                        PeakRssiInDbm = array[3],
                        AntennaNumber = array[4],
                        ReaderNumber = array[5],
                        IpAddress = array[6],
                        UniqueReadID = array[7],
                        TimingPoint = array[8]
                    };
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(MappRead), ex.Message, ex.StackTrace));
            }
            return null;
        }

        public Task StartReading()
        {
            try
            {
                bool isPingable = false;
                while (!isPingable && !_cancellationToken.IsCancellationRequested)
                {
                    isPingable = PingUtil.PingHostViaTcp(_reading.Reader.Host, int.Parse(_reading.Reader.Port));
                }

                if (isPingable)
                    return backgoundWorker = Task.Run(() => DoReadingWork(_cancellationToken.Token), _cancellationToken.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(StartReading), ex.Message, ex.StackTrace));
            }

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
                    ChangeReaderStatus(true);
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ns.Flush();
                        data = new byte[10000];
                        recvieveLength = ns.Read(data, 0, data.Length);
                        stringData = Encoding.ASCII.GetString(data, 0, recvieveLength);
                        if (stringData.Contains("$_"))
                        {
                            SetRecoveryFileName(stringData);
                            continue;
                        }
                        if (stringData == Consts.STOP)
                        {
                            ChangeReaderStatus(false);
                            break;
                        }

                        CheckStringForReadings(stringData);
                    } while (!cancellationToken.IsCancellationRequested);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Unable to connect to server");
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(DoReadingWork), ex.Message, ex.StackTrace));
                ChangeReaderStatus(null);
                Thread.Sleep(250);
                DoReadingWork(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(DoReadingWork), ex.Message, ex.StackTrace));
                ChangeReaderStatus(false);
                DisposeTcpClient();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(DoReadingWork), ex.Message, ex.StackTrace));
                ChangeReaderStatus(false);
                DisposeTcpClient();
            }
        }

        private void DisposeTcpClient()
        {
            try
            {
                _tcpClient.Close();
                _tcpClient.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}:  {1}\n{2}", nameof(DisposeTcpClient), ex.Message, ex.StackTrace));
            }
        }

        private void ChangeReaderStatus(bool? status)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (_reading != null)
                    _reading.IsConnected = status;
            }));
        }

        private void SetStartTime(DateTime dateTime)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (_reading != null)
                    _reading.StartedDateTime = dateTime;
            }));
        }
    }
}
