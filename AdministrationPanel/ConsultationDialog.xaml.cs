using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
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

namespace AdministrationPanel
{
    /// <summary>
    /// Логика взаимодействия для ConsultationDialog.xaml
    /// </summary>
    public partial class ConsultationDialog : Window
    {
        public Consultation? CurrentConsultation { get; set; } 

        public ObservableCollection<Appointment> Appointments { get; set; } = new ObservableCollection<Appointment>();
        public event EventHandler? DataChaged;

        public ConsultationDialog()
        {
            this.DataContext = this;
            InitializeComponent();
            this.AddOrEditButton.Content = "Add";
            this.Title = "Add new consultation";
            this.AppointmentsPanel.Visibility = Visibility.Hidden;        
        }

        public ConsultationDialog(Consultation consultation)
        {
            this.DataContext = this;
                        
            //Appointments = new ObservableCollection<Appointment>();
           

            InitializeComponent();

            this.CurrentConsultation = consultation;
            this.AddOrEditButton.Content = "Edit";
            this.Title = "Edit consultation";
            this.NameEntry.Text = consultation.Name;

            RefreshAppointments();

        }

        public void RefreshAppointments()
        {
            Appointments.Clear();
            foreach (var appointment in DatabaseManager.SelectAll<Appointment>().Where(a => a.Consultation.Id == this.CurrentConsultation?.Id))
            {
                Appointments.Add(appointment);
            }
        }

        private void AddAppointment(object sender, RoutedEventArgs e)
        {

            if (CurrentConsultation == null)
            {
                return;
            }

            AppointmentDialog appointmentDialog = new AppointmentDialog(CurrentConsultation);
            
            
            appointmentDialog.DataChaged += (object? sender, EventArgs e) => { RefreshAppointments(); };

            appointmentDialog.ShowDialog();
        }

        private void EditAppointment(object sender, EventArgs e)
        {
            Button? button = sender as Button;

            Appointment? appointment = button?.DataContext as Appointment;



            if (appointment != null && CurrentConsultation != null)
            {
                AppointmentDialog appointmentDialog = new AppointmentDialog(CurrentConsultation, appointment);
                appointmentDialog.DataChaged += (object? sender, EventArgs e) => { RefreshAppointments(); };
                appointmentDialog.ShowDialog();
            }
        }

        private void DeleteAppointment(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            Appointment? appointment = button?.DataContext as Appointment;



            if (appointment != null && CurrentConsultation != null)
            {

                DatabaseManager.Delete<Appointment>(appointment);

                RefreshAppointments();

            }
        }

        private void AddOrEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NameEntry.Text == "")
            {
                MessageBox.Show("Please enter a name");
                return;
            }

            if (CurrentConsultation != null)
            {
                Consultation consultation = new Consultation(CurrentConsultation.Id, this.NameEntry.Text);
                DatabaseManager.Update(consultation);
                MainWindow.WriteLog($"{consultation} updated");

            }
            else
            {
                Consultation consultation = new Consultation(null, this.NameEntry.Text);
                DatabaseManager.Insert(consultation);
                MainWindow.WriteLog($"{consultation} inserted");
            }


            this.DataChaged?.Invoke(this, new EventArgs());
            this.Close();
        }
    }
}
