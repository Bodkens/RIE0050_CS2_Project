using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DatabaseLayer
{
    public class User : INotifyPropertyChanged
    {

        private int? id;
        [Key, Order(0)]
        public int? Id { get { return id; } set { id = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id))); } }
        private string email;
        [Order(1)]
        public string Email { get { return email; } set { email = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email))); } }
        private string password;
        [Order(2)]
        public string Password { get { return password; } set { password = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password))); } }
        private string firstName;
        [Order(3)]
        public string FirstName { get { return firstName; } set { firstName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName))); } }
        private string lastName;
        [Order(4)]
        public string LastName { get { return lastName; } set { lastName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName))); } }
        private bool isAdmin;
        [Order(5)]
        public bool IsAdmin { get { return isAdmin; } set { isAdmin = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAdmin))); } }

        public User(int? id, string email, string password, string firstName, string lastName, bool isAdmin)
        {
            this.id = id;
            this.email = email;
            this.password = password;
            this.firstName = firstName;
            this.lastName = lastName;
            this.isAdmin = isAdmin;
        }

        public User(int? id, string email, string password, string firstName, string lastName, int isAdmin)
        {
            this.id = id;
            this.email = email;
            this.password = password;
            this.firstName = firstName;
            this.lastName = lastName;
            this.isAdmin = isAdmin == 0 ? false : true;
        }
        public override string ToString()
        {
            return $"{this.Id} - {this.Email} - {this.FirstName} {this.LastName}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
