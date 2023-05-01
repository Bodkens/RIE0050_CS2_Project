using System;
using System.Collections.Generic;
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
using DatabaseLayer;
using System.Text.RegularExpressions;
namespace AdministrationPanel
{
    /// <summary>
    /// Логика взаимодействия для UserDialog.xaml
    /// </summary>
    /// 

    


    public partial class UserDialog : Window
    {
        public User? CurrentUser { get; set; }

        public event EventHandler? DataChaged;

        public UserDialog()
        {
            InitializeComponent();
            this.AddOrEditButton.Content = "Add";
            this.Title = "Add new user";
        }

        public UserDialog(User user)
        {
            this.CurrentUser = user;
           
            InitializeComponent();
            this.AddOrEditButton.Content = "Edit";
            this.Title = "Edit user";

            this.EmailEntry.Text = user.Email;
            this.PasswordEntry.Text = user.Password;
            this.FirstNameEntry.Text = user.FirstName;
            this.LastNameEntry.Text = user.LastName;
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            Regex emailFormat = new Regex(@"^[a-z\.0-9]+@[a-z]+\.[a-z]+$");

            if (!emailFormat.IsMatch(this.EmailEntry.Text))
            {
                MessageBox.Show("Please enter email in correct format!");
                return;
            }

            if (this.PasswordEntry.Text.Length < 5)
            {
                MessageBox.Show("Password is too short! Minimum length: 6");
                return;
            }

            if (this.FirstNameEntry.Text == "" || this.LastNameEntry.Text == "")
            {
                MessageBox.Show("Please enter first and last name");
                return;
            }
            

            if(this.CurrentUser == null)
            {
                User user = new User(null, this.EmailEntry.Text, this.PasswordEntry.Text, this.FirstNameEntry.Text, this.LastNameEntry.Text, false);
                try
                {
                    DatabaseManager.Insert<User>(user);
                    MainWindow.WriteLog($"{user} inserted");
                }
                catch 
                {
                    MessageBox.Show("Unknown error occured");
                    return;
                }
                         
            }
            else
            {
                User user = new User(this.CurrentUser.Id, this.EmailEntry.Text, this.PasswordEntry.Text, this.FirstNameEntry.Text, this.LastNameEntry.Text, false);

                try
                {
                    DatabaseManager.Update<User>(user);
                    MainWindow.WriteLog($"{user} updated");
                }
                catch
                {
                    MessageBox.Show("Unknown error occured");
                    return;
                }
            }
            this.DataChaged?.Invoke(this, new EventArgs());
            this.Close();
        }
    }
}
