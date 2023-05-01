using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdministrationPanel;
using DatabaseLayer;

namespace Administration
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            DatabaseManager.ConnectionString = @"Data Source=F:\VP\RIE0050_CS2_Project\Database\database.db;";
            InitializeComponent();
        }

        private async void BeginLogin(object sender, RoutedEventArgs e)
        {
            List<User> users = new List<User>();
            List<Consultation> consultations = new List<Consultation>();

            //if (users.Where(x => x.Email == EmailEntry.Text && x.Password == PasswordEntry.Password && x.IsAdmin).ToList().Count == 0)
            //{
            //    MessageBox.Show("Email or password is incorrect");

            //    return;
            //}


            List<User> adminList = DatabaseManager.Select<User>("SELECT * FROM user WHERE IsAdmin = 1 AND Email = @email AND Password = @password",
                new Dictionary<string, object?>() { { "@email", EmailEntry.Text }, { "@password", PasswordEntry.Password } });
            if (adminList.Count == 0)
            {
                MessageBox.Show("Email or password is incorrect");

                return;
            }

            User admin = adminList[0];

            Task loadUsers = new Task(() =>
            {
                users = DatabaseManager.Select<User>("SELECT * FROM USER WHERE IsAdmin = 0").ToList();
            });
            loadUsers.Start();
            Task loadConsultations = new Task(() =>
            {
                consultations = DatabaseManager.SelectAll<Consultation>();
            });

            loadConsultations.Start();


            MainWindow.WriteLog($"Administrator with email {EmailEntry.Text} signed in at {DateTime.Now.ToString("yyyy-M-d H:m:s")}", MainWindow.LogPath);
            await loadConsultations;
            await loadUsers;
            MainWindow mainWindow = new MainWindow(users, consultations, admin);
            mainWindow.Show();
            this.Close();
            }
        }
    }
