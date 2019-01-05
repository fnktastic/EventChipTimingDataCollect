using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ReaderDataCollector.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _selectedView;
        public string SelectedView
        {
            get { return _selectedView; }
            set { _selectedView = value; RaisePropertyChanged("SelectedView"); }
        }

        private int _windowWidth;
        public int WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; RaisePropertyChanged("WindowWidth"); }
        }

        #region constructor
        public MainViewModel()
        {
            WindowWidth = 750;
        }
        #endregion

        #region commands
        private RelayCommand _openReadingsCollectorCommand;
        public RelayCommand OpenReadingsCollectorCommand
        {
            get
            {
                return _openReadingsCollectorCommand ?? (_openReadingsCollectorCommand = new RelayCommand(() =>
                {
                    SelectedView = "A";
                    WindowWidth = 750;
                }));
            }
        }

        private RelayCommand _openStoredReadingsCommand;
        public RelayCommand OpenStoredReadingsCommand
        {
            get
            {
                return _openStoredReadingsCommand ?? (_openStoredReadingsCommand = new RelayCommand(() =>
                {
                    SelectedView = "B";
                    WindowWidth = 1000;
                }));
            }
        }
        #endregion
    }
}