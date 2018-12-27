using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.Model
{
    public class Reader : ViewModelBase
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("ID"); }
        }

        private string _host;
        public string Host
        {
            get { return _host; }
            set { _host = value; RaisePropertyChanged("Host"); }
        }

        private string _port;
        public string Port
        {
            get { return _port; }
            set { _port = value; RaisePropertyChanged("Port"); }
        }

        public IEnumerable<Reading> Readings { get; set; }
    }
}
