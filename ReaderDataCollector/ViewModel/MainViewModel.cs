using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.DataAccess;
using System.Data.Entity;

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
            Database.SetInitializer(new Initializer());
            WindowWidth = 675;
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
                    WindowWidth = 675;
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

        private RelayCommand _openOnlineReadingsCommand;
        public RelayCommand OpenOnlineReadingsCommand
        {
            get
            {
                return _openOnlineReadingsCommand ?? (_openOnlineReadingsCommand = new RelayCommand(() =>
                {
                    SelectedView = "E";
                    WindowWidth = 925;
                }));
            }
        }

        private RelayCommand _openReportsCommand;
        public RelayCommand OpenReportsCommand
        {
            get
            {
                return _openReportsCommand ?? (_openReportsCommand = new RelayCommand(() =>
                {
                    SelectedView = "C";
                    WindowWidth = 675;
                }));
            }
        }

        private RelayCommand _openStatisticssCommand;
        public RelayCommand OpenStatisticssCommand
        {
            get
            {
                return _openStatisticssCommand ?? (_openStatisticssCommand = new RelayCommand(() =>
                {
                    SelectedView = "D";
                    WindowWidth = 675;
                }));
            }
        }

        private RelayCommand _openSettingsCommand;
        public RelayCommand OpenSettingsCommand
        {
            get
            {
                return _openSettingsCommand ?? (_openSettingsCommand = new RelayCommand(() =>
                {
                    SelectedView = "F";
                    WindowWidth = 575;
                }));
            }
        }
        #endregion
    }
}