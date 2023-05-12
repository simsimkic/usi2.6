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
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.Schedules;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    public partial class PatientFindAnamnesesWindow : Window
    {
        public Patient Patient;
        public Anamnesis[] GlobalAnamneses;
        public Anamnesis SelectedAnamnesis { get; set; }
        public PatientFindAnamnesesWindow(Patient patient)
        {
            Patient = patient;
            InitializeComponent();
            DataContext = this;
            InitializeAnamnesesTable();
        }

        public void InitializeAnamnesesTable()
        {
            dataGrid.ItemsSource = GlobalAnamneses;
        }

        public void SearchBtnClick(object sender, RoutedEventArgs e)
        {
            if (keywordBox.Text != "")
            {
                string characters = keywordBox.Text.ToLower();
                Anamnesis[] allAnamneses = Anamnesis.LoadAnamnesis();
                List<Anamnesis> anamneses = new List<Anamnesis>();
                if (allAnamneses != null)
                {
                    foreach (Anamnesis anamnesis in allAnamneses)
                    {
                        string diagnosis = anamnesis.Diagnosis.ToLower();
                        if (anamnesis.PatientId == Patient.Id && diagnosis.Contains(characters, StringComparison.CurrentCultureIgnoreCase))
                        {
                            anamneses.Add(anamnesis);
                        }
                    }
                    GlobalAnamneses = anamneses.ToArray();
                    InitializeAnamnesesTable();
                    keywordBox.Text = "";
                }
                else
                {
                    MessageBox.Show("Anamneses do not exist. \nSorry.", "Error");
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Insert a keyword to find!", "Warning");
            }
        }

        private void SortByDateChecked(object sender, RoutedEventArgs e)
        {
            Anamnesis[] allAnamneses = GlobalAnamneses;
            if (allAnamneses != null)
            {
                SortedDictionary<DateTime, Anamnesis> dict = new SortedDictionary<DateTime, Anamnesis>();
                foreach (Anamnesis anamnesis in allAnamneses)
                {
                    Examination[] examinations = Examination.LoadExaminations("../../../Data/Examinations.json");
                    TimeSlot ts = Examination.FindExamination(anamnesis.ExaminationId, examinations).TimeSlot;
                    DateTime examinationDateTime = DateTime.ParseExact(ts.Date + " " + ts.StartTime, "dd.MM.yyyy. HH:mm:ss", null);
                    // sorting dates
                    dict.Add(examinationDateTime, anamnesis);
                }
                GlobalAnamneses = dict.Values.ToArray();
            }
            else
            {
                MessageBox.Show("Anamneses do not exist. \nSorry.");
            }
        }
        private void SortByDoctorChecked(object sender, RoutedEventArgs e)
        {
            Anamnesis[] allAnamneses = GlobalAnamneses;
            if (allAnamneses != null)
            {
                SortedDictionary<String, Anamnesis> dict = new SortedDictionary<String, Anamnesis>();
                foreach (Anamnesis anamnesis in allAnamneses)
                {
                    Examination[] examinations = Examination.LoadExaminations("../../../Data/Examinations.json");
                    Doctor doctor = Doctor.Find(Examination.FindExamination(anamnesis.ExaminationId, examinations).DoctorId);
                    String username = doctor.Username;
                    // sorting doctors by username
                    dict.Add(username, anamnesis);
                }
                GlobalAnamneses = dict.Values.ToArray();
                InitializeAnamnesesTable();
            }
            else
            {
                MessageBox.Show("Anamneses do not exist. \nSorry.");
            }
        }
        private void SortByDocSpecChecked(object sender, RoutedEventArgs e)
        {
            Anamnesis[] allAnamneses = GlobalAnamneses;
            if (allAnamneses != null)
            {
                SortedDictionary<Doctor.DoctorsSpeciality, Anamnesis> dict = new SortedDictionary<Doctor.DoctorsSpeciality, Anamnesis>();
                foreach (Anamnesis anamnesis in allAnamneses)
                {
                    Examination[] examinations = Examination.LoadExaminations("../../../Data/Examinations.json");
                    Doctor doctor = Doctor.Find(Examination.FindExamination(anamnesis.ExaminationId, examinations).DoctorId);
                    Doctor.DoctorsSpeciality speciality = doctor.Speciality;
                    // sorting doctors by speciality
                    dict.Add(speciality, anamnesis);
                }
                GlobalAnamneses = dict.Values.ToArray();
                InitializeAnamnesesTable();
            }
            else
            {
                MessageBox.Show("Anamneses do not exist. \nSorry.");
            }

        }

        public void ShowMoreBtnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedAnamnesis != null)
            {
                Anamnesis anamnesis = SelectedAnamnesis;
                PatientAnamnesisWindow window = new PatientAnamnesisWindow(anamnesis);
                window.Top = 100;
                window.Left = 800;
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("Select an anamnesis to show!", "Warning");
            }
        }
    }
}
