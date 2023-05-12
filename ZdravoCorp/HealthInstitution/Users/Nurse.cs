using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZdravoCorp.HealthInstitution.Users
{
    public class Nurse
    {
        public string Username;
        public string Password;
        public string Name;
        public string Email;
        public string Surname;
        public string Birthday;

        public Nurse() { }
        public Nurse(string username, string password)
        {
            Nurse nurse = FindNurseByUsername(username);
            this.Username = nurse.Username;
            this.Password = nurse.Password;
            this.Name = nurse.Name;
            this.Surname = nurse.Surname;
            this.Email = nurse.Email;
            this.Birthday = nurse.Birthday;
        }
        public Nurse(string username, string password, string name, string email, string surname, string birthday)
        {
            this.Username = username;
            this.Password = password;
            this.Name = name;
            this.Surname = surname;
            this.Email = email;
            this.Birthday = birthday;
        }

        public static Nurse FindNurseByUsername(string username)
        {
            Nurse[] nurses = LoadNurses();
            foreach (Nurse nurse in nurses)
            {
                if (nurse.Username == username) return nurse;
            }
            return null;
        }
        public static Nurse[] LoadNurses()
        {
            var nursesJson = File.ReadAllText("../../../Data/Users/nurses.json");
            Nurse[] allNurses = JsonConvert.DeserializeObject<Nurse[]>(nursesJson)!;
            return allNurses;
        }
    }
}
