using GalaSoft.MvvmLight;
using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.ViewModel
{
    public class OnlineReadingDetailsViewModel : ViewModelBase
    {
        private ObservableCollection<Read> _reads;
        public ObservableCollection<Read> Reads
        {
            get { return _reads; }
            set { _reads = value; RaisePropertyChanged("Reads"); }
        }

        public OnlineReadingDetailsViewModel(ObservableCollection<Read> reads)
        {
            Reads = reads;
        }
    }
}
