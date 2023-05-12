using System;
using System.Collections.Generic;
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
using ZdravoCorp.HealthInstitution.GUI.Register;
using ZdravoCorp.HealthInstitution.Services.Register;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    /// <summary>
    /// Interaction logic for UpdatePatientWindow.xaml
    /// </summary>
    public partial class UpdatePatientWindow : Window
    {
        private Patient _patient;
        public UpdatePatientWindow(Patient selectedPatient)
        {
            InitializeComponent();
            _patient = selectedPatient;
            usernameBox.Text = selectedPatient.Username;
            passwordBox.Password = selectedPatient.Password;
            nameBox.Text = selectedPatient.MedicalRecord.Name;
            surnameBox.Text = selectedPatient.MedicalRecord.Surname;
            birthdayBox.Text = selectedPatient.MedicalRecord.Birthday;
            heightBox.Text = selectedPatient.MedicalRecord.Height;
            weightBox.Text = selectedPatient.MedicalRecord.Weight;
            medicalHistoryBox.Text = selectedPatient.MedicalRecord.MedicalHistory;
        }

        private void TryToUpdate(object sender, EventArgs e)
        {
            string username = usernameBox.Text;
            string password = passwordBox.Password.ToString();
            string name = nameBox.Text;
            string surname = surnameBox.Text;
            string birthday = birthdayBox.Text;
            string height = heightBox.Text;
            string weight = weightBox.Text;
            string medicalHistory = medicalHistoryBox.Text;

            if (isInputValid(username, password, name, surname, birthday, height, weight, medicalHistory))
            {
                PatientCRUD.UpdatePatient(_patient, username, password, name, surname, birthday, height, weight, medicalHistory);
                MessageBox.Show("Paient is successfully updated.");
                this.Close();
            }
        }

        private bool isInputValid(string username, string password, string name, string surname, string birthday, string height, string weight, string medicalHistory)
        {
            if (!isUsernameValid(username)) return false;
            if (!RegisterPatientWindow.isPasswordValid(password)) return false;
            if (!RegisterPatientWindow.isMedicalRecordValid(name, surname, birthday, height, weight, medicalHistory)) return false;

            return true;
        }

        private bool isUsernameValid(string username)
        {
            if (username == _patient.Username) return true;

            Profile[] profiles = Profile.LoadProfiles();
            foreach (Profile profile in profiles)
            {
                if (profile.Username == username)
                {
                    MessageBox.Show("Username already exists.");
                    return false;
                }
            }
            if (username == "")
            {
                MessageBox.Show("Username cannot be an empty string.");
                return false;
            }
            return true;
        }
    }
}
