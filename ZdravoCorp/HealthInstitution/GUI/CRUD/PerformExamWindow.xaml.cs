using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using ZdravoCorp.HealthInstitution.Services.DoctorServices;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    /// <summary>
    /// Interaction logic for PerformExamWindow.xaml
    /// </summary>
    public partial class PerformExamWindow : Window
    {
        public Examination Exam;
        public int RoomId;
        public PerformExamWindow(Examination exam, int roomId)
        {
            Exam = exam;
            RoomId = roomId;
            InitializeComponent();
            InitializePatientGrid();
            InitializeAnamnesisData();
        }

        private void InitializeAnamnesisData()
        {
            if (Anamnesis.DoesAnamnesisExist(Exam.Id))   //if the anamnesis exist,load data
            {
                Anamnesis anamnesis = Anamnesis.FindAnamnesis(Exam.Id);
                txtSymptoms.Text = anamnesis.Symptoms;
                txtAllergies.Text = anamnesis.Alergies;
                txtIllness.Text = anamnesis.EarlierIllnesses;
                txtDiagnosis.Text = anamnesis.Diagnosis;
            }
            
        }

        private void InitializePatientGrid()
        {
            List<Patient> patients = new List<Patient>();
            patients.Add(Patient.Find(Exam.PatientId));
            patientDataGrid.ItemsSource=patients;
        }

        private void BtnViewRecordClick(object sender, RoutedEventArgs e)
        {
            Patient patient = Patient.Find(Exam.PatientId);
            UpdateRecordWindow recordWindow = new UpdateRecordWindow(patient.GetMedicalRecord(), Exam.DoctorId, Exam.PatientId,true);
            recordWindow.ShowDialog();
            InitializePatientGrid();
           
        }

        private void BtnUpdateAnamnesisClick(object sender, RoutedEventArgs e)
        {
           
            if (txtSymptoms.Text !="" && txtIllness.Text!="" && txtAllergies.Text!="" &&
                txtDiagnosis.Text!="")
            {
                Anamnesis anamnesis = new Anamnesis(Exam.Id, Exam.PatientId, txtSymptoms.Text, txtAllergies.Text,txtIllness.Text);
                anamnesis.Diagnosis=txtDiagnosis.Text;
                if (Anamnesis.DoesAnamnesisExist(Exam.Id))
                {
                    PerformExaminationService.UpdateAnamnesis(anamnesis);
                }
                else
                {
                    PerformExaminationService.CreateAnamnesis(anamnesis);  
                }
                
            }
            else
            {
                MessageBox.Show("Not all data is entered", "Warning");
            }
        }

        private void BtnEndExamClick(object sender, RoutedEventArgs e)
        {
            DynamicEquipmentWindow equipmentWindow = new DynamicEquipmentWindow(RoomId);
            equipmentWindow.Show();
            this.Close();
        }
    }
}
