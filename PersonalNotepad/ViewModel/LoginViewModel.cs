using PersonalNotepad.Common;
using PersonalNotepad.Model;
using System.Windows;

namespace PersonalNotepad.ViewModel
{
    public class LoginViewModel
    {
        public LoginModel LoginModel { get; set; } 

        public CommandBase CloseWindowCommand { get; set; }

        public LoginViewModel()
        {
            this.LoginModel = new LoginModel();
            this.LoginModel.UserName = "aaa";
            this.LoginModel.Password = "123";

            this.CloseWindowCommand = new CommandBase();
            this.CloseWindowCommand.DoExecute = new Action<object>((o) =>
            {
                (o as Window).Close();
            });
            this.CloseWindowCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }
    }
}
