using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace AdministrationPanel
{
    /// <summary>
    /// Логика взаимодействия для AppointmentDialog.xaml
    /// </summary>
    public partial class AppointmentDialog : Window
    {
        public event EventHandler? DataChaged;

        public Consultation consultation { get; set; }

        public Appointment? CurrentAppointment { get; set; }

        public AppointmentDialog(Consultation consultation)
        {
            InitializeComponent();
            this.consultation = consultation;
           

            foreach (var user in DatabaseManager.Select<User>("SELECT * FROM USER WHERE IsAdmin = 0").ToList())
            {
                this.UsersComboBox.Items.Add(user);
            }
            this.UsersComboBox.SelectedValue = CurrentAppointment?.User != null ? CurrentAppointment.User : "None";

        }

        public AppointmentDialog(Consultation consultation, Appointment appointment)
        {
            InitializeComponent();
            this.consultation = consultation;
            this.CurrentAppointment = appointment;

            foreach (var user in DatabaseManager.Select<User>("SELECT * FROM USER WHERE IsAdmin = 0").ToList())
            {
                this.UsersComboBox.Items.Add(user);
            }
            
            this.Title = "Edit appointment";
            this.AddOrEditButton.Content = "Edit";

            this.AppointmentDatePicker.Text = CurrentAppointment.Time.ToString("dd.MM.yyyy");

            this.TimeEntry.Text = CurrentAppointment.Time.ToString("HH:mm");

           
            this.UsersComboBox.SelectedValue = CurrentAppointment.User != null ? CurrentAppointment.User : "None";
            Console.Write("");
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex(@"(?:[0-1][0-9]|2[0-3]):[0-5][0-9]");

            if (!regex.IsMatch(this.TimeEntry.Text) || this.AppointmentDatePicker.Text == "")
            {
                MessageBox.Show("Please enter date and time in correct format!");
                return;
            }

            User? user = null;
            
            if (UsersComboBox.SelectedItem is User)
            {
                user = (User) UsersComboBox.SelectedItem;
            }

            string[] splittedDate = AppointmentDatePicker.Text.Split('.');
            string day = splittedDate[0];
            string month = splittedDate[1];
            string year = splittedDate[2];


            if (this.CurrentAppointment != null)
            {
                Appointment appointment = new Appointment(CurrentAppointment.Id, $"{year}-{month}-{day} {this.TimeEntry.Text}:00", this.consultation.Id != null ? this.consultation.Id.Value : 0, user?.Id);
                DatabaseManager.Update(appointment);
                MainWindow.WriteLog($"{appointment} updated");
            }
            else
            {
                Appointment appointment = new Appointment(null, $"{year}-{month}-{day} {this.TimeEntry.Text}:00", this.consultation.Id != null ? this.consultation.Id.Value : 0, user?.Id);
                DatabaseManager.Insert(appointment);
                MainWindow.WriteLog($"{appointment} inserted");
            }

            

            
            this.DataChaged?.Invoke(this, new EventArgs());
            this.Close();
           
        }

    }
}
