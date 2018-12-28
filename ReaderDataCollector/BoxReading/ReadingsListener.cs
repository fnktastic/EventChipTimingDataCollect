﻿using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
using ReaderDataCollector.Utils;
using ReaderDataCollector.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private void CheckStringForReadings(string stringToCheck, string tsk)
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
                            Console.WriteLine("{0} -> {1}", tsk, reading);
                        }));
                    }
                }
            }
        }

        public static Read MappRead(string stringToMap)
        {
            var array = stringToMap.Replace("@", "").Split('#');
            if (array.Length == 9)
            {
                Console.WriteLine(stringToMap);
                return new Read()
                {
                    ID = 0,
                    EPC = array[1],
                    Time = DateTime.Parse(array[2]),
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

        public Task StartReading(string tsk)
        {
            bool isPingable = false;
            while (!isPingable && !_cancellationToken.IsCancellationRequested)
            {
                isPingable = PingUtil.PingHostViaTcp(_reading.Reader.Host, int.Parse(_reading.Reader.Port));
            }

            if (isPingable)
                return backgoundWorker = Task.Run(() => DoReadingWork(_cancellationToken.Token, tsk), _cancellationToken.Token);

            return backgoundWorker;
        }

        public void StopReading()
        {
            Thread.Sleep(300);
            _cancellationToken.Cancel();
        }

        private void DoReadingWork(CancellationToken cancellationToken, string tsk)
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

                        CheckStringForReadings(stringData, tsk);
                    } while (!cancellationToken.IsCancellationRequested);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Unable to connect to server");
                ChangeReaderStatus(null);
                Thread.Sleep(250);
                DoReadingWork(cancellationToken, tsk);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation canceled");
                ChangeReaderStatus(false);
                DisposeTcpClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ChangeReaderStatus(false);
                DisposeTcpClient();
            }
        }

        private void DisposeTcpClient()
        {
            _tcpClient.Close();
            _tcpClient.Dispose();
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
