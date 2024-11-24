using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalNotepad.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public LoginViewModel LoginViewModel { get; set; }
        public StepBarViewModel StepBarViewModel { get; set; }

        public MainViewModel()
        {
            LoginViewModel = new LoginViewModel();
            StepBarViewModel = new StepBarViewModel();
            CurrentDateTime = DateTime.Now;
        }

        private DateTime _currentDateTime;
        public DateTime CurrentDateTime
        {
            get { return _currentDateTime; }
            set
            {
                _currentDateTime = value;
                OnPropertyChanged(nameof(CurrentDateTime));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
