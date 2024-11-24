using HandyControl.Controls;
using Microsoft.Data.Sqlite;
using PersonalNotepad.Common;
using PersonalNotepad.Model;
using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PersonalNotepad.ViewModel
{
    public class LoginViewModel : NotifyBase
    {
        public LoginModel LoginModel { get; set; } = new LoginModel();

        public CommandBase CloseWindowCommand { get; set; }
        public CommandBase LoginCommand { get; set; }

        string databasePath = DatabaseHelper.GetDatabasePath();

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; this.DoNotify(); }
        }

        private int _stepIndex;
        public int StepIndex
        {
            get => _stepIndex;
            set
            {
                _stepIndex = value;
                this.DoNotify();
            }
        }

        public string DateStr { get; set; }
        public void GetCurrentTime(object sender, EventArgs e)
        {
            DateStr = DateTime.Now.ToString("MM-dd");
        }

        public LoginViewModel()
        {
            DispatcherTimer ShowTimer = new DispatcherTimer();
            ShowTimer.Interval = new TimeSpan(0, 0, 1); // 每秒触发一次
            ShowTimer.Tick += new EventHandler(GetCurrentTime); // 绑定事件
            ShowTimer.Start();

            this.CloseWindowCommand = new CommandBase();
            this.CloseWindowCommand.DoExecute = new Action<object>((o) =>
            {
                (o as System.Windows.Window).Close();
            });
            this.CloseWindowCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            this.LoginCommand = new CommandBase();
            this.LoginCommand.DoExecute = new Action<object>(DoLogin);
            this.LoginCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            // 初始化 StepIndex
            this.StepIndex = 0;

            // 监听属性变化
            LoginModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(LoginModel.UserName) && !string.IsNullOrEmpty(LoginModel.UserName))
                {
                    StepIndex = 1;
                }
                else if (args.PropertyName == nameof(LoginModel.Password) && !string.IsNullOrEmpty(LoginModel.Password))
                {
                    StepIndex = 2;
                }
                else if (args.PropertyName == nameof(LoginModel.ValidationCode) && !string.IsNullOrEmpty(LoginModel.ValidationCode))
                {
                    StepIndex = 3;
                }
            };
        }

        private async void DoLogin(object o)
        {
            this.ErrorMessage = "";
            if (string.IsNullOrEmpty(LoginModel.UserName))
            {
                this.ErrorMessage = "用户名不能为空";
                await Task.Run(() => Growl.WarningGlobal("用户名不能为空"));
                return;
            }
            if (string.IsNullOrEmpty(LoginModel.Password))
            {
                this.ErrorMessage = "密码不能为空";
                await Task.Run(() => Growl.WarningGlobal("密码不能为空"));
                return;
            }
            if (string.IsNullOrEmpty(LoginModel.ValidationCode))
            {
                this.ErrorMessage = "验证码不能为空";
                await Task.Run(() => Growl.WarningGlobal("验证码不能为空"));
                return;
            }
            if (LoginModel.ValidationCode.ToLower() != DateStr)
            {
                this.ErrorMessage = "验证码错误";
                await Task.Run(() => Growl.WarningGlobal("验证码错误"));
                return;
            }

            // 获取数据库路径
            using (var conn = new SqliteConnection($"Data Source={databasePath}"))
            {
                try
                {
                    conn.Open();
                    // 检查用户名是否存在
                    string checkUserSql = "SELECT * FROM users WHERE user_name=@user_name";
                    using (var command = new SqliteCommand(checkUserSql, conn))
                    {
                        command.Parameters.Add(new SqliteParameter("@user_name", LoginModel.UserName));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 用户名存在，检查密码是否正确
                                string storedPassword = reader["password"].ToString();
                                string inputPasswordHash = ComputeMD5Hash(LoginModel.Password);
                                if (storedPassword == inputPasswordHash)
                                {
                                    await Task.Run(() => Growl.SuccessGlobal("登录成功！"));
                                    await Task.Run(() => Growl.SuccessGlobal("等待跳转！"));

                                    await Task.Delay(2000);
                                    Application.Current.Dispatcher.Invoke(new Action(() =>
                                    {
                                        var window = o as System.Windows.Window;
                                        if (window != null)
                                        {
                                            window.DialogResult = true;
                                        }
                                        else
                                        {
                                            Growl.ErrorGlobal("无法设置 DialogResult，因为窗口对象为 null。");
                                        }
                                    }));
                                }
                                else
                                {
                                    this.ErrorMessage = "密码错误";
                                    await Task.Run(() => Growl.ErrorGlobal("密码错误"));
                                }
                            }
                            else
                            {
                                // 用户名不存在，弹出MessageBox询问是否新建用户
                                var result = System.Windows.MessageBox.Show("用户名不存在，是否新建用户？", "提示", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    // 用户选择是，创建新用户
                                    string insertUserSql = "INSERT INTO users (user_name, password) VALUES (@user_name, @password)";
                                    using (var insertCommand = new SqliteCommand(insertUserSql, conn))
                                    {
                                        string hashedPassword = ComputeMD5Hash(LoginModel.Password);
                                        insertCommand.Parameters.Add(new SqliteParameter("@user_name", LoginModel.UserName));
                                        insertCommand.Parameters.Add(new SqliteParameter("@password", hashedPassword));
                                        insertCommand.ExecuteNonQuery();
                                        await Task.Run(() => Growl.SuccessGlobal("用户创建成功！"));
                                    }
                                }
                                else
                                {
                                    // 用户选择否，不创建新用户
                                    this.ErrorMessage = "用户未创建";
                                    await Task.Run(() => Growl.WarningGlobal("用户未创建"));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.ErrorMessage = "数据库连接失败: " + ex.Message;
                    await Task.Run(() => Growl.ErrorGlobal("数据库连接失败: " + ex.Message));
                }
                finally
                {
                    conn.Close(); // 确保每次操作后关闭数据库连接
                }
            }

            ClearCache(); // 清理缓存
        }

        private void ClearCache()
        {
            LoginModel.UserName = string.Empty;
            LoginModel.Password = string.Empty;
            LoginModel.ValidationCode = string.Empty;
            this.ErrorMessage = string.Empty;
        }

        public static string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
