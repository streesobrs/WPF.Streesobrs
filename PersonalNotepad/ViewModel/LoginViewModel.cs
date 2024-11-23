using PersonalNotepad.Common;
using System.Windows;

namespace PersonalNotepad.ViewModel
{
    public class LoginViewModel
    {
        public CommandBase CloseWindowCommand { get; set; }

        public LoginViewModel()
        {
            this.CloseWindowCommand = new CommandBase();
            this.CloseWindowCommand.DoExecute = new Action<object>((o) =>
            {
                (o as Window).Close();
            });
            this.CloseWindowCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }
    }
}
