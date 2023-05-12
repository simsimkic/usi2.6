using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using ZdravoCorp.HealthInstitution.Users;
using Newtonsoft.Json;
using System.IO;
using ZdravoCorp.HealthInstitution.GUI;
using ZdravoCorp.HealthInstitution.Users.Patient;
using ZdravoCorp.HealthInstitution.Examinations;

namespace ZdravoCorp.HealthInstitution.Services.Login
{
    public class Login
    {
        public static bool LoginLogic(string username, string password, string fileName)
        {
            Profile[] users = LoadUsers(fileName);
            bool loggedIn = false;
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i].Username == username && users[i].Password == password)
                {
                    loggedIn = true;
                    FindUserType(users[i]);
                    break;
                }
            }
            return loggedIn;
        }

        private static Profile[] LoadUsers(string filename)
        {
            var jsontext = File.ReadAllText(filename);
            Profile[] users = JsonConvert.DeserializeObject<Profile[]>(jsontext)!;
            return users;
        }

        private static void FindUserType(Profile user)
        {
            switch (user.role)
            {
                case Profile.Type.doctor:
                    LoginDoctor(user);
                    break;
                case Profile.Type.nurse:
                    LoginNurse(user);
                    break;
                case Profile.Type.patient:
                    LoginPatient(user);
                    break;
                case Profile.Type.manager:
                    LoginManager(user);
                    break;
            }
        }

        private static void LoginManager(Profile user)
        {
            ManagerMainWindow managerView = new ManagerMainWindow(user.Username);
            managerView.Show();
        }

        private static void LoginPatient(Profile user)
        {
            Patient patient = Patient.Find(Patient.FindByUsername(user.Username));
            if (Patient.CheckIfBlocked(patient.Id))
            {
                Patient.Block(patient);
                MessageBox.Show("Oops, you are blocked!\nTry later.", "Warning");
            }
            else
            {
                PatientMainWindow patientView = new PatientMainWindow(patient);
                patientView.Show();
            }
        }

        private static void LoginNurse(Profile user)
        {
            Nurse nurse = new Nurse(user.Username, user.Password);
            NurseMainWindow nurseView = new NurseMainWindow(nurse);
            nurseView.Show();
        }

        private static void LoginDoctor(Profile user)
        {
            Doctor doctor = new Doctor(user.Username, user.Password);
            DoctorMainWindow doctorView = new DoctorMainWindow(user.Username);
            doctorView.Show();
        }
    }
}
