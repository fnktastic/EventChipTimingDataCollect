using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ReaderDataCollector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.ViewModel
{
    public class SetClockViewModel : ViewModelBase
    {
        public SetClockViewModel(Reader reader)
        {

        }

        [PreferredConstructor]
        public SetClockViewModel() { }
    }
}
