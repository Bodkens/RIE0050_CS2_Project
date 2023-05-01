using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer
{
    public class Appointment : INotifyPropertyChanged
    {
        [Key, Order(0)]
        public int? Id { get; set; }
        [Order(1)]
        public DateTime Time { get; set; }
        [Order(2), ForeignKey("consultationId", "Id")]
        public Consultation Consultation { get; set; }

        [Order(3), ForeignKey("userId", "Id")]
        public User? User { get; set; }


       

        public Appointment(int? id, string time, int consultationId, int? userId)
        {
            this.Id = id;
            this.Time = DateTime.ParseExact(time, "yyyy-M-d H:m:s", CultureInfo.CurrentCulture);
          
            this.Consultation = DatabaseManager.Select<Consultation>("SELECT * FROM consultation WHERE Id = @id", new Dictionary<string, object?>(){ { "@id", consultationId } } ).First();
            
            if (userId != null)
            {
                this.User = DatabaseManager.Select<User>("SELECT * FROM user WHERE Id = @id", new Dictionary<string, object?>() { { "@id", userId } }).First();
            }
        }

        public override string ToString()
        {
            return $"Appointment Id: {Id}; Appointment Time {Time.ToString("yyyy-M-d H:m:s")} Appointed to consultaltation with ID {Consultation.Id}, {(User?.Id != null ? $"appointed to user with Id {User?.Id}" : "does not have an appointed user")}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
