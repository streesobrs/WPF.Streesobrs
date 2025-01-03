﻿using PersonalNotepad.Common;

namespace PersonalNotepad.Model
{
    public class LoginModel : NotifyBase
    {
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                this.DoNotify();
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                this.DoNotify();
            }
        }

        private string _validationCode;

        public string ValidationCode
        {
            get { return _validationCode; }
            set
            {
                _validationCode = value;
                this.DoNotify();
            }
        }
    }
}
