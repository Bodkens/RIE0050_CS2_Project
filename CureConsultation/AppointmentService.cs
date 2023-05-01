using DatabaseLayer;
using Microsoft.AspNetCore.Mvc;

namespace CureConsultation
{
    public class AppointmentService
    {
        public AppointmentService() {  }

        

        public List<Appointment> LoadAppointments(User user)
        {
            if (user.Id != null)
            {
                return DatabaseManager.Select<Appointment>("SELECT * FROM Appointment WHERE userId = @id", 
                    new Dictionary<string, object?>() { { "@id", user.Id} });
            }

            return new List<Appointment>();
        }

        public List<Appointment> LoadAppointments(int id)
        {
                return DatabaseManager.Select<Appointment>("SELECT * FROM Appointment WHERE userId = @id",
                    new Dictionary<string, object?>() { { "@id", id } });
        }
    }
}
