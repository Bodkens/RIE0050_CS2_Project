using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer
{
    public class Consultation : INotifyPropertyChanged
    {
        [Key, Order(0)]
        public int? Id { get; set; }
        [Order(1)]
        public string Name { get; set; }

        public Consultation(int? id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"Consultation Id: {Id}; Consultation Name: {Name}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
