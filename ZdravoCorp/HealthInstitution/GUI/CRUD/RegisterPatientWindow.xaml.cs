using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xaml.Permissions;
using ZdravoCorp.HealthInstitution.Services.Register;
using ZdravoCorp.HealthInstitution.Users;

namespace ZdravoCorp.HealthInstitution.GUI.Register
{
    /// <summary>
    /// Interaction logic for RegisterPatientWindow.xaml
    /// </summary>
    public partial class RegisterPatientWindow : Window
    {
        public RegisterPatientWindow()
        {
            InitializeComponent();
        }

        private void TryToRegister(object sender, EventArgs e)
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
                PatientCRUD.RegisterNewPatient(username, password, name, surname, birthday, height, weight, medicalHistory);
                MessageBox.Show("Paient is successfully registered.");
                this.Close();
                NurseMainWindow.UpdatePatientsCollection();
            }
        }

        private bool isInputValid(string username, string password, string name, string surname, string birthday, string height , string weight, string medicalHistory)
        {
            if (!isUsernameValid(username)) return false;
            if (!isPasswordValid(password)) return false;
            if (!isMedicalRecordValid(name, surname, birthday, height, weight, medicalHistory)) return false;
            
            return true;
        }

        private bool isUsernameValid(string username)
        {
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
        
        public static bool isPasswordValid(string password) { 
            if (password.Length < 5)
            {
                MessageBox.Show("Password must contain at least 5 characters.");
                return false;
            }
            return true;
        }

        public static bool isMedicalRecordValid(string name, string surname, string birthday, string height, string weight, string medicalHistory)
        {
            if (name == "" || surname == "" || birthday == "" || height == "" || weight == "" || medicalHistory == "")
            {
                MessageBox.Show("All fields are required.");
                return false;
            }
            if (!isBirthdayValid(birthday)) return false;
            if (!isWeightValid(weight)) return false;
            if (!isHeightValid(height)) return false;

            return true;
        }

        public static bool isBirthdayValid(string birthday) {
            DateTime date;
            if (!DateTime.TryParseExact(birthday, "dd.MM.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                MessageBox.Show("Date must be given in this format: dd.MM.yyyy.");
                return false;
            }
            return true;
        }
        public static bool isHeightValid(string height)
        {
            double number;
           if (!double.TryParse(height, out number) || number <= 0)
            {
                MessageBox.Show("Height must be a positive number.");
                return false;
            }
            return true;
        }
        public static bool isWeightValid(string weight)
        {
            double number;
            if (!double.TryParse(weight, out number) || number <= 0)
            {
                MessageBox.Show("Weight must be a positive number.");
                return false;
            }
            return true;
        }
    }
}
