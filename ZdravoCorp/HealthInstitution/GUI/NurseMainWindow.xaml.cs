using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.GUI.CRUD;
using ZdravoCorp.HealthInstitution.GUI.Register;
using ZdravoCorp.HealthInstitution.Schedules;
using ZdravoCorp.HealthInstitution.Services.Register;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;
using ZdravoCorp.HealthInstitution.Services.ExaminationServices;

namespace ZdravoCorp.HealthInstitution.GUI
{
    /// <summary>
    /// Interaction logic for NurseMainWindow.xaml
    /// </summary>
    public partial class NurseMainWindow : Window
    {
        public static ObservableCollection<Patient> Patients { get; set; }
        public Patient SelectedPatient { get; set; }

        public Doctor.DoctorsSpeciality SelectedSpecialty { get; set; }

        public NurseMainWindow(Nurse loggedInNurse)
        {
            InitializeComponent();
            DataContext = this;
            InitializeProfile(loggedInNurse);
            InitializePatientTable();
            InitializeExaminationsForToday();
        }

        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        public void RegisterPatient(object sender, RoutedEventArgs e)
        {
            RegisterPatientWindow register = new RegisterPatientWindow();
            register.Show();
        }

        public void DeletePatient(object sender, EventArgs e)
        {
            if (SelectedPatient != null)
            {
                PatientCRUD.DeletePatient(SelectedPatient);
                MessageBox.Show("Patient successfully deleted.");
                UpdatePatientsCollection();
            } 
            else
            {
                MessageBox.Show("Pick the patient you want to delete.");
            }
        }

        public void UpdatePatient(object sender, EventArgs e)
        {
            if (SelectedPatient != null)
            {
                UpdatePatientWindow update = new UpdatePatientWindow(SelectedPatient);
                update.Show();
            }
            else
            {
                MessageBox.Show("Pick the patient you want to update.");
            }
        }
        public void ScheduleEmergencyExamination(object sender, EventArgs e)
        {
            if (SelectedPatient != null)
            {
                if(WasSpecialitySelected(sender, e))
                {
                    bool wasScheduled = EmergencyExaminationService.TryToSchedule(SelectedPatient, SelectedSpecialty);
                    if (wasScheduled)
                    {
                        MessageBox.Show("Emergency examination was successfuly scheduled.");
                    } else
                    {
                        Dictionary<Examination, TimeSpan> fiveExaminations = EmergencyExaminationService.GetFiveLeastImportantExaminations(SelectedSpecialty);
                        CreateEmergencyExaminationWindow examWindow = new CreateEmergencyExaminationWindow(fiveExaminations, SelectedPatient);
                        examWindow.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("Pick the patient you want to update.");
            }
        }

        public bool WasSpecialitySelected(object sender, EventArgs e)
        {
            if (surgeryOption.IsChecked == true)
            {
                SelectedSpecialty = Doctor.DoctorsSpeciality.Surgery;
            }
            else if (generalMedicineOption.IsChecked == true)
            {
                SelectedSpecialty = Doctor.DoctorsSpeciality.GeneralMedicine;
            }
            else if (cardiologyOption.IsChecked == true)
            {
                SelectedSpecialty = Doctor.DoctorsSpeciality.Cardiology;
            }
            else {
                MessageBox.Show("Pick a speciality for the examination.");
                return false;
            }
            return true;
        }

        public void InitializeProfile(Nurse loggedInNurse)
        {
            name.Content += "    " + loggedInNurse.Name;
            surname.Content += "    " + loggedInNurse.Surname;
            username.Content += "    " + loggedInNurse.Username;
            password.Content += "    " + loggedInNurse.Password;
            birthday.Content += "    " + loggedInNurse.Birthday;
            email.Content += "    " + loggedInNurse.Email;
            role.Content += "    " + "Nurse";
        }

        public void InitializePatientTable()
        {
            Patient[] patients = Patient.LoadPatients("../../../Data/PatientData.json");
            Patients = new ObservableCollection<Patient>(Patient.LoadPatients("../../../Data/PatientData.json"));
        }

        public void InitializeExaminationsForToday()
        {
            listExaminations.ItemsSource = Examination.GetActiveExaminationsForToday();
        }

        public void AdmitPatient(object sender, EventArgs e)
        {
            if (listExaminations.SelectedItem != null)
            {
                Examination selectedExamination = ((Examination)listExaminations.SelectedItem);
                if (CanAdmitionBeStarted(selectedExamination))
                {
                    CreateAnamnesisWindow anamnesisWindow = new CreateAnamnesisWindow(selectedExamination.Id, selectedExamination.PatientId);
                    anamnesisWindow.Show();
                }
            }
            else
            {
                MessageBox.Show("Select an examination");
            }
        }

        private bool CanAdmitionBeStarted(Examination examination)
        {
            DateTime now = DateTime.Now;
            now = now.AddMinutes(15);
            DateTime startTime = TimeSlot.GetStartTime(examination.TimeSlot);
            if (now.CompareTo(startTime) <= 0)
            {
                MessageBox.Show("Admission can be started only 15 minutes before examination");
                return false;
            }
            if (Anamnesis.DoesAnamnesisExist(examination.Id))
            {
                MessageBox.Show("Anamnesis was already created");
                return false;
            }
            return true;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public static void UpdatePatientsCollection()
        {
            Patient[] patients = Patient.LoadPatients("../../../Data/PatientData.json");
            Patients.Clear();
            foreach (var pat in patients)
            {
                Patients.Add(pat);
            }
        }
    }
}
