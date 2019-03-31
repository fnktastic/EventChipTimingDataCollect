using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;

namespace ReaderDataCollector.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ReadingViewModel>();
            SimpleIoc.Default.Register<ReaderViewModel>();
            SimpleIoc.Default.Register<RewindViewModel>();
            SimpleIoc.Default.Register<PingViewModel>();
            SimpleIoc.Default.Register<SetClockViewModel>();
            SimpleIoc.Default.Register<StoredReadingsViewModel>();
            SimpleIoc.Default.Register<OnlineReadingsViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<LocalReadingsViewModel>();
            SimpleIoc.Default.Register<StatisticsViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ReadingViewModel Reading
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReadingViewModel>();
            }
        }

        public ReaderViewModel Reader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReaderViewModel>();
            }
        }

        public RewindViewModel Rewind
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RewindViewModel>();
            }
        }

        public PingViewModel Ping
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PingViewModel>();
            }
        }
        
        public SetClockViewModel SetClock
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SetClockViewModel>();
            }
        }

        public StoredReadingsViewModel StoredReadings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StoredReadingsViewModel>();
            }
        }

        public OnlineReadingsViewModel OnlineReadings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<OnlineReadingsViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public LocalReadingsViewModel LocalReadings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LocalReadingsViewModel>();
            }
        }

        public StatisticsViewModel Statistics
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StatisticsViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}