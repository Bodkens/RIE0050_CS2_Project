using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DatabaseLayer;
using System.Threading;
using System.IO;
using System.ComponentModel;

namespace AdministrationPanel
{

    class DataChangedEventArgs : EventArgs
    {
        public object? OldData { get; set; }
        public object? NewData { get; set; }
        public DataChangedEventArgs(object? oldData, object? newData) : base() 
        {
            this.OldData = oldData;
            this.NewData = newData;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<User> Users { get; set; } 
        public ObservableCollection<Consultation> Consultations { get; set; }

        private static object lockObject = new object();

        public static string LogPath { get; set; } = "../../../../log/log.txt";

        public User CurrentAdmin { get; set; }
        
        public static void WriteLog(string line, string filePath= "../../../../log/log.txt")
        {
            Thread t = new Thread(() =>
            {

                lock (lockObject)
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Append))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(line);
                        }
                    }
                }

            });
            t.Start();
            
        }

        

        public void RefreshUsers()
        {
            this.Users.Clear();
            foreach (var user in DatabaseManager.Select<User>("SELECT * FROM USER WHERE IsAdmin = 0").ToList())
            {
                this.Users.Add(user);
            }
        }

        public void RefreshConsultations()
        {
            this.Consultations.Clear();
            foreach (var consultation in DatabaseManager.SelectAll<Consultation>())
            {
                this.Consultations.Add(consultation);
            }
        }

        public MainWindow(List<User> users, List<Consultation> consultations, User currentAdmin)
        {
            this.DataContext = this;

            this.Users = new ObservableCollection<User>(users);
            this.Consultations = new ObservableCollection<Consultation>(consultations);

            this.CurrentAdmin = currentAdmin;

            this.Closing += ((object? sender, CancelEventArgs e) => { MainWindow.WriteLog($"Administrator with email {CurrentAdmin.Email} signed out at {DateTime.Now.ToString("yyyy-M-d H:m:s")}", MainWindow.LogPath); });

            

            InitializeComponent();
        }

        private void AddNewUser(object sender, RoutedEventArgs e)
        {
            UserDialog userDialog = new UserDialog();

            userDialog.DataChaged += (object? sender, EventArgs e) => { RefreshUsers(); };
            userDialog.ShowDialog();
        }

        private void EditUser(object sender, RoutedEventArgs e)
        {

            Button? button = sender as Button;

            User? user = button?.DataContext as User;

            if(user != null)
            {
                UserDialog userDialog = new UserDialog(user);
                userDialog.DataChaged += (object? sender, EventArgs e) => { RefreshUsers(); };
                userDialog.ShowDialog();
            }

            
        }
        private void DeleteUser(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete user? Appointments assigned to this user will be freed", "Delete User", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {

                Button? button = sender as Button;

                User? user = button?.DataContext as User;

                if (user != null)
                {

                    foreach (var appointment in DatabaseManager.SelectAll<Appointment>().Where(a => a.User?.Id == user.Id))
                    {
                        appointment.User = null;
                        DatabaseManager.Update<Appointment>(appointment);
                    }

                    DatabaseManager.Delete<User>(user);

                    RefreshUsers();
                    
                }

            }
        }

        private void AddNewConsultation(object sender, RoutedEventArgs e)
        {
            ConsultationDialog consultation = new ConsultationDialog();

            consultation.DataChaged += (object? sender, EventArgs e) => { RefreshConsultations(); };
            consultation.ShowDialog();
        }

        private void EditConsultation(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            Consultation? consultation = button?.DataContext as Consultation;
            


            if (consultation != null)
            {
                ConsultationDialog consultationDialog = new ConsultationDialog(consultation);
                consultationDialog.DataChaged += (object? sender, EventArgs e) => { RefreshConsultations(); };
                consultationDialog.ShowDialog();
            }

            
        }

        private void DeleteConsultation(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete consultation? Appointments assigned to this consultation will be deleted", "Delete consultation", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {

                Button? button = sender as Button;

                Consultation? consultation = button?.DataContext as Consultation;

                if (consultation != null)
                {
                    foreach (var appointment in DatabaseManager.SelectAll<Appointment>().Where(a => a.Consultation.Id == consultation.Id))
                    {
                        DatabaseManager.Delete<Appointment>(appointment);
                    }


                    DatabaseManager.Delete<Consultation>(consultation);
                    
                    
                    RefreshConsultations();

                }

            }

        }
    }
}

