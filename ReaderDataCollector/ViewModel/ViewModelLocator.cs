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
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}