using Microsoft.Data.Sqlite;
using PersonalNotepad.View;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace PersonalNotepad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeDatabase();

            if (new LoginView().ShowDialog() == true)
            {
                new MainView().ShowDialog();
            }
            Application.Current.Shutdown();
        }


        // 获取数据库路径的方法
        private string GetDatabasePath()
        {
            // 获取用户文档目录的路径
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // 在文档目录下创建一个名为StreeDB的文件夹
            string folderPath = Path.Combine(documentsPath, "StreeDB");

            // 如果文件夹不存在，则创建它
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 根据编译条件（Debug或Release）设置数据库文件的名称
#if DEBUG
            return Path.Combine(folderPath, "debug_PersonalNotepadDatabase.db");
#else
                return Path.Combine(folderPath, "PersonalNotepadDatabase.db");
#endif
        }

        // 初始化数据库的方法
        private void InitializeDatabase()
        {
            // 获取数据库文件的完整路径  
            string databasePath = GetDatabasePath();
            // 判断数据库文件是否存在  
            bool isNewDatabase = !File.Exists(databasePath);

            using (var connection = new SqliteConnection($"Data Source={databasePath}"))
            {
                connection.Open();

                try
                {
                    // 如果是新数据库，则创建表格并插入初始数据  
                    if (isNewDatabase)
                    {
                        CreateUserTable(connection); // 创建用户表
                    }
                    else
                    {
                        // 确保用户表存在
                        CreateUserTable(connection);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database initialization failed: " + ex.Message);
                }
            }
        }

        // 创建用户表的方法
        private void CreateUserTable(SqliteConnection connection)
        {
            string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS users (
            user_name TEXT NOT NULL,
            password TEXT NOT NULL
        )";
            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

    }
}
