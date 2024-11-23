using System.ComponentModel;

namespace PersonalNotepad.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string _defaultAction;
        public string DefaultAction
        {
            get { return _defaultAction; }
            set
            {
                _defaultAction = value;
                OnPropertyChanged(nameof(DefaultAction));
            }
        }

        public SettingsViewModel()
        {
            // 默认值可以是 "导入" 或 "导出"
            DefaultAction = "导入";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
