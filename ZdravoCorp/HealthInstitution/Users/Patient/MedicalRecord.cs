using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.HealthInstitution.Users.Patient
{
    public class MedicalRecord : INotifyPropertyChanged
    {
        private string name;
        private string surname;
        private string birthday;
        private string height;
        private string weight;
        private string medicalHistory;


        public MedicalRecord(string name, string surname, string birthday, string height, string weight, string medicalHistory)
        {
            this.name = name;
            this.surname = surname;
            this.birthday = birthday;
            this.height = height;
            this.weight = weight;
            this.medicalHistory = medicalHistory;
        }
        public string Name {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged();
                }

            }
        }
        public string Surname {
            get => surname;
            set
            {
                if (value != surname)
                {
                    surname = value;
                    OnPropertyChanged();
                }

            }
        }
        public string Birthday {
            get => birthday;
            set
            {
                if (value != birthday)
                {
                    birthday = value;
                    OnPropertyChanged();
                }

            }
        }
        public string Height { get => height; set => height = value; }
        public string Weight { get => weight; set => weight = value; }
        public string MedicalHistory { get => medicalHistory; set => medicalHistory = value; }




        public event PropertyChangedEventHandler PropertyChanged;

        internal static void Update(object recordId, MedicalRecord record)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
