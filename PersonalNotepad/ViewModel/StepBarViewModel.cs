using HandyControl.Controls;
using System.ComponentModel;

namespace PersonalNotepad.ViewModel
{
    public class StepBarViewModel : INotifyPropertyChanged
    {
        private int _stepIndex;
        public int StepIndex
        {
            get => _stepIndex;
            set
            {
                _stepIndex = value;
                OnPropertyChanged(nameof(StepIndex));
            }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
                if (!string.IsNullOrEmpty(UserName))
                {
                    StepIndex = 1;
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                if (!string.IsNullOrEmpty(Password))
                {
                    StepIndex = 2;
                }
            }
        }

        private string _validationCode;
        public string ValidationCode
        {
            get => _validationCode;
            set
            {
                _validationCode = value;
                OnPropertyChanged(nameof(ValidationCode));
                if (!string.IsNullOrEmpty(ValidationCode))
                {
                    StepIndex = 3;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
