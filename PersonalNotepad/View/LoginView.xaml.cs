using PersonalNotepad.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PersonalNotepad.View
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : Window
    {
        public string DateStr { get; set; }

        public LoginView()
        {
            InitializeComponent();

            this.DataContext = new LoginViewModel();

            DispatcherTimer ShowTimer = new DispatcherTimer();
            ShowTimer.Interval = new TimeSpan(0, 0, 1); // 每秒触发一次
            ShowTimer.Tick += new EventHandler(GetCurrentTime); // 绑定事件
            ShowTimer.Start();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        public void GetCurrentTime(object sender, EventArgs e)
        {
            DateStr = DateTime.Now.ToString("HH-mm");

            this.Date.Text = DateStr;//WPF中有控件名为Date
        }
    }
}
