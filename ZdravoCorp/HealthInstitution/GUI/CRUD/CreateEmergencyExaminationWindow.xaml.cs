using System;
using System.Collections;
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
using ZdravoCorp.HealthInstitution.Services.ExaminationServices;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    /// <summary>
    /// Interaction logic for CreateEmergencyExaminationWindow.xaml
    /// </summary>
    public partial class CreateEmergencyExaminationWindow : Window
    {
        Dictionary<Examination, TimeSpan> FiveExaminations;
        Patient patient;
        public CreateEmergencyExaminationWindow(Dictionary<Examination, TimeSpan> fiveExaminations, Patient p)
        {
            patient = p;
            FiveExaminations = fiveExaminations;
            InitializeComponent();
            InitializeExaminationsList(fiveExaminations);
        }

        public void InitializeExaminationsList(Dictionary<Examination, TimeSpan> fiveExaminations)
        {
            List<Examination> examinations = new List<Examination>();
            foreach (KeyValuePair<Examination, TimeSpan> pair in fiveExaminations)
            {
                examinations.Add(pair.Key);
            }
            listExaminations.ItemsSource = examinations;
        }

        public void PickExamination(object sender, EventArgs e)
        {
            if (listExaminations.SelectedItem != null)
            {
                Examination selectedExamination = ((Examination)listExaminations.SelectedItem);
                TimeSpan offset = FiveExaminations[selectedExamination];

                Doctor doctor = Doctor.Find(selectedExamination.DoctorId);
                TimeSlot emergencyTimeSlot = new TimeSlot(selectedExamination.TimeSlot.Date, selectedExamination.TimeSlot.StartTime, selectedExamination.TimeSlot.Duration);
                Examination emergencyExam = EmergencyExaminationService.CreateExamination(patient, doctor, emergencyTimeSlot);

                DateTime startTime = TimeSlot.GetStartTime(selectedExamination.TimeSlot);
                startTime = startTime + offset;
                TimeSlot newTimeSlot = new TimeSlot(startTime.ToString("dd.MM.yyyy."), startTime.ToString("HH:mm:ss"), emergencyExam.TimeSlot.Duration);
                bool ExamSuccesfullyMoved = EmergencyExaminationService.UpdateExeminationInEmergency(selectedExamination.Id, selectedExamination.IsOperation, selectedExamination.DoctorId, selectedExamination.PatientId, newTimeSlot);
                if (!ExamSuccesfullyMoved)
                {
                    MessageBox.Show("Exam could not be rescheduled.");
                    return;
                }

                bool ExamSuccesfullyCreated = Examination.Create(emergencyExam.Id, emergencyExam.IsOperation, emergencyExam.DoctorId, emergencyExam.PatientId, emergencyExam.TimeSlot);
                if (!ExamSuccesfullyCreated)
                {
                    MessageBox.Show("Emergency exam could not be created.");
                    return;
                }
                MessageBox.Show("Emergency exam was successfully created.");
                this.Close();
                return;
            }
            else
            {
                MessageBox.Show("Select an examination");
            }
        }
    } 
}
