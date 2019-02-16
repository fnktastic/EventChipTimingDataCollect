using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ReaderDataCollector.DataAccess;
using ReaderDataCollector.Model;
using ReaderDataCollector.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaderDataCollector.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        #region fields
        private readonly Context _context;
        private readonly ISettingRepository _settingRepository;
        #endregion
        #region properties
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; RaisePropertyChanged("Value"); }
        }

        private ObservableCollection<Setting> _allSettings;
        public ObservableCollection<Setting> AllSettings
        {
            get { return _allSettings; }
            set { _allSettings = value; RaisePropertyChanged("AllSettings"); }
        }

        private Setting _selectedSetting;
        public Setting SelectedSetting
        {
            get { return _selectedSetting; }
            set
            {
                _selectedSetting = value;
                if (_selectedSetting != null)
                {
                    Name = _selectedSetting.Name;
                    Value = _selectedSetting.Value;
                }

                RaisePropertyChanged("SelectedSetting");
            }
        }
        #endregion
        #region constructor
        public SettingsViewModel()
        {
            _context = new Context();
            _settingRepository = new SettingRepository(_context);
            _allSettings = new ObservableCollection<Setting>(_settingRepository.Settings);
            SelectedSetting = new Setting();
        }
        #endregion
        #region commands
        private RelayCommand _addOrUpdateSettingCommand;
        public RelayCommand AddOrUpdateSettingCommand
        {
            get
            {
                return _addOrUpdateSettingCommand ?? (_addOrUpdateSettingCommand = new RelayCommand(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(_name) && !string.IsNullOrWhiteSpace(_value))
                    {
                        var settingToAdd = new Setting()
                        {
                            Name = this.Name,
                            Value = this.Value
                        };

                        await _settingRepository.SaveSetting(settingToAdd);
                        AllSettings = new ObservableCollection<Setting>(_settingRepository.Settings);
                        Name = string.Empty;
                        Value = string.Empty;
                    }
                }));
            }
        }
        #endregion
    }
}
