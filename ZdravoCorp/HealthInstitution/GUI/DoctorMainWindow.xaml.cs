using RoomServicesNS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.GUI.CRUD;
using ZdravoCorp.HealthInstitution.Schedules;
using ZdravoCorp.HealthInstitution.Services.DoctorServices;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.GUI
{
    public partial class DoctorMainWindow : Window
    {
        public int DocId;
        public DoctorMainWindow(string docUsername)
        {
            DocId = Doctor.FindByUsername(docUsername);
            InitializeComponent();
            InitializeExamGrid();
            InitializePatientGrid();
        }

        public void InitializeExamGrid()
        {
            examDataGrid.ItemsSource = Examination.GetDoctorExaminations(DocId);
            scheduleDataGrid.ItemsSource = Examination.GetDoctorExaminations(DocId);

        }
        public void InitializePatientGrid()
        {
            Patient[]patients = Patient.LoadPatients("../../../Data/PatientData.json");
            patientDataGrid.ItemsSource = patients;

        }
        private void BtnThreeDaysClick(object sender, RoutedEventArgs e)
        {
            List<Examination> examinationsAll;
            List<Examination> examinations1;
            List<Examination> examinations2;
            List<Examination> examinations3;
            DateTime now = DateTime.Now;
            string date = now.ToString("dd.MM.yyyy.");
            int doctorId = DocId;
            examinations1 = Examination.GetDoctorExaminationsForDate(doctorId, date);
            now = now.AddDays(1);
            date = now.ToString("dd.MM.yyyy.");
            examinations2 = Examination.GetDoctorExaminationsForDate(doctorId, date);
            now = now.AddDays(1);
            date = now.ToString("dd.MM.yyyy.");
            examinations3 = Examination.GetDoctorExaminationsForDate(doctorId, date);
            examinationsAll = examinations1.Concat(examinations2).Concat(examinations3).ToList();
            lstExaminations.ItemsSource = examinationsAll;
        }
        private void BtnViewPatientClick(object sender, RoutedEventArgs e)
        {
            if (lstExaminations.SelectedItem != null)
            {
                int row = lstExaminations.SelectedIndex;
                String[] data = lstExaminations.Items[row].ToString().Split(", patientId: ");
                int id = int.Parse(data[1].Split(" doctorId: ")[0]);
                Patient p = Patient.Find(id);
                MedicalRecord record = p.GetMedicalRecord();
                string info = "First name: " + record.Name + ", Last name: " + record.Surname + ", Date of birth: " + record.Birthday +
                    "\nHeight: " + record.Height + ", Weight: " + record.Weight + ", Medical history: " + record.MedicalHistory;
                MessageBox.Show(info, "Patient medical record");
            }
            else
            {
                MessageBox.Show("Select a patient", "Warning");
            }
        }
        private void BtnDateClick(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate != null)
            {
                string date = datePicker.SelectedDate.Value.Date.ToString("dd.MM.yyyy.");
                lstExaminations.ItemsSource = Examination.GetDoctorExaminationsForDate(DocId, date);
            }
            else
            {
                MessageBox.Show("Select a date in the datePicker component", "Warning");
            }

        }

        public void BtnUpdateClick(object sender, RoutedEventArgs e)
        {
            if (examDataGrid.SelectedItem != null)
            {
                Examination selectedExam = examDataGrid.SelectedItem as Examination;
                UpdateExaminationDoctorWindow window = new UpdateExaminationDoctorWindow(selectedExam, DocId);
                window.ShowDialog();
                InitializeExamGrid();
            }
            else
            {
                MessageBox.Show("Select an examination to update", "Warning");
            }
        }
        private void BtnCreateClick(object sender, RoutedEventArgs e)
        {
            UpdateExaminationDoctorWindow window = new UpdateExaminationDoctorWindow(null, DocId);
            window.ShowDialog();
            InitializeExamGrid();
        }

        private void BtnDeleteClick(object sender, RoutedEventArgs e)
        {
            Examination selectedExam = examDataGrid.SelectedItem as Examination;
            if (selectedExam != null)
            {
                try
                {
                    if (Examination.IsDoctors(DocId, selectedExam.DoctorId))
                    {
                        if (Examination.Delete(selectedExam.Id))
                        {
                            MessageBox.Show("Examination deleted");
                        }

                    }
                    else
                    {
                        MessageBox.Show("This examination doesn't belong to this doctor", "Warning");
                    }
                }
                catch
                {
                    MessageBox.Show("Invalid input type", "Warning");
                }
            }
            else
            {
                MessageBox.Show("Select an examination", "Warning");
            }
            InitializeExamGrid();
        }

        private void BtnViewRecordClick(object sender, RoutedEventArgs e)
        {
            if(patientDataGrid.SelectedItem != null)
            {
                Patient patient = patientDataGrid.SelectedItem as Patient;
                UpdateRecordWindow recordWindow = new UpdateRecordWindow(patient.GetMedicalRecord(),DocId,patient.GetId(),false);
                recordWindow.ShowDialog();
                InitializePatientGrid();
            }
            else
            {
                MessageBox.Show("Select a patient", "Warning");
            }
            
        }

        private void TxtIdTextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtId.Text != "")
            {
                Patient patient = Patient.Find(int.Parse(txtId.Text));
                if (patient != null)
                {
                    List<Patient> patients = new List<Patient>();
                    patients.Add(patient);
                    patientDataGrid.ItemsSource = patients;
                }
                else
                {
                    patientDataGrid.ItemsSource = null;
                }
            }
            else
            {
                InitializePatientGrid();
            }

        }
        private void BtnStartExamClick(object sender, RoutedEventArgs e)
        {
            if (scheduleDataGrid.SelectedItem != null)
            {
                if (cmbRoomId.SelectedItem != null)
                {
                    Examination exam = scheduleDataGrid.SelectedItem as Examination;
                    if (PerformExaminationService.TryPerformingExam(exam))
                    {
                        PerformExamWindow performExamWindow = new PerformExamWindow(exam,int.Parse(cmbRoomId.SelectedItem.ToString()));
                        performExamWindow.ShowDialog();
                        InitializeExamGrid();
                    }
                    else
                    {
                        MessageBox.Show("Cant perform this examination", "Warning");
                    }
                }
                else
                {
                    MessageBox.Show("Select room id", "Warning");
                }
            
            }
            else
            {
                MessageBox.Show("Select an exam", "Warning");
            }
        }

        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void InitializeRooms(bool isOperation)
        {
            List<int> allRoomIds = RoomSevice.GetAllRoomIds();
            List<int> filteredRoomIds = new List<int>();
            foreach (int roomId in allRoomIds)
            {
                if (isOperation)
                {
                    if (RoomSevice.GetRoomTypeById(roomId).Equals( RoomsNS.Room.Type.OperatingRoom))
                    {
                        filteredRoomIds.Add(roomId);
                    }
                }
                else
                {
                    if (RoomSevice.GetRoomTypeById(roomId).Equals( RoomsNS.Room.Type.PatientCareRoom) ||
                    RoomSevice.GetRoomTypeById(roomId).Equals(RoomsNS.Room.Type.ConsultingRoom))
                    {
                        filteredRoomIds.Add(roomId);
                    }
                }
            }
            cmbRoomId.ItemsSource= filteredRoomIds;
        }

        private void ScheduleDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (scheduleDataGrid.SelectedItem != null)
            {
                Examination exam = scheduleDataGrid.SelectedItem as Examination;
                InitializeRooms(exam.IsOperation);
            }
        }

    }

}
