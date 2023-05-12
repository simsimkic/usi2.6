using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class RecommendExaminationWindow : Window
    {
        public Patient Patient;
        public bool DoctorPriority;
        public Examination[] allExaminations;
        public RecommendExaminationWindow(Patient patient)
        {
            Patient = patient;
            InitializeComponent();
            InitializeTable(allExaminations);
        }

        public void InitializeTable(Examination[] examinations)
        {
            dataGrid.ItemsSource = examinations;
        }

        public void GetPriority()
        {
            if (doctorPriority.IsChecked == true) { DoctorPriority = true; }
            else if (timePriority.IsChecked == true) { DoctorPriority = false; }
        }

        public bool IsLastDateValid()
        {
            try
            {
                DateTime lastDate = DateTime.Parse(pickLastDate.Text);
                if (lastDate < DateTime.Now.AddDays(1))
                {
                    MessageBox.Show("Pick a date in more than 1 day.", "Warning");
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Invalid date input.", "Warning");
                return false;
            }
            return true;
        }

        public bool IsTimeSpanValid()
        {
            try
            {
                DateTime startTime = DateTime.ParseExact(startTimeBox.Text, "HH:mm:ss", null);
                DateTime endTime = DateTime.ParseExact(endTimeBox.Text, "HH:mm:ss", null);
                if (startTime > (endTime.AddMinutes(-15)))
                {
                    MessageBox.Show("End time must be at least 15 minuts after start time.", "Warning");
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Invalid time input.", "Warning");
                return false;
            }
            return true;
        }

        public bool IsDoctorValid()
        {
            if (doctorBox.Text != "doctor1" && doctorBox.Text != "doctor2" && doctorBox.Text != "doctor3")
            {
                MessageBox.Show("Doctor does not exist.", "Warning");
                return false;
            }
            return true;
        }

        public bool IsDataValid()
        {
            if ((doctorPriority.IsChecked != true && timePriority.IsChecked != true) || doctorBox.Text == "" ||
                startTimeBox.Text == "" || endTimeBox.Text == "" || pickLastDate.SelectedDate == null)
            {
                MessageBox.Show("Not everything is filled out!");
                return false;
            }
            else
            {
                if (IsDoctorValid() && IsTimeSpanValid() && IsLastDateValid()) { return true; }
                return false;
            }
        }

        public void SearchClick(object sender, RoutedEventArgs e)
        {
            if (IsDataValid())
            {
                GetPriority();
                RecommendTimeSlot();
            }
            else
            {
                MessageBox.Show("Try input again :)");
            }
        }

        public void RecommendTimeSlot()
        {
            int doctorId = Doctor.FindByUsername(doctorBox.Text);
            string firstDate = DateTime.Now.AddHours(24).ToString("dd.MM.yyyy.");
            DateTime lastDate = (DateTime)pickLastDate.SelectedDate;
            List<Examination> recommendedExamList = new List<Examination>();
            TimeSlot ts = new TimeSlot(firstDate, startTimeBox.Text, "00:15:00");
            Examination[] examinations = Examination.LoadExaminations("../../../Data/Examinations.json");
            if (DoctorPriority)
            {

                recommendedExamList = RecommendTimeSlotByDoctor(doctorId, ts, firstDate, DateTime.Now, lastDate, recommendedExamList, examinations);
            }
            else
            {
                recommendedExamList = RecommendTimeSlotByTimeSpan(doctorId, ts, firstDate, DateTime.Now, lastDate, recommendedExamList, examinations);
            }
            if (recommendedExamList.Count == 0)
            {
                recommendedExamList = RecommendOtherTimeSlot(doctorId, ts, firstDate, DateTime.Now, lastDate, recommendedExamList, examinations);
            }
            InitializeTable(recommendedExamList.ToArray());
        }

        public List<Examination> RecommendOtherTimeSlot(int doctorId, TimeSlot ts, string firstDateString, DateTime firstDate, DateTime lastDate,
            List<Examination> recommendedExamList, Examination[] examinations)
        {
            while (recommendedExamList.Count < 3 && DateTime.Parse(ts.Date) < lastDate)
            {
                if (Schedule.IsFree(null, doctorId, ts, "doctor") && Schedule.IsFree(null, Patient.Id, ts, "patient"))
                {
                    Examination exam = new Examination((examinations[examinations.Length - 1].Id * 10 - 2),
                        false, Patient.Id, doctorId, ts, "created");
                    recommendedExamList.Add(exam);
                }
                else { ts = TimeSlot.GetNextTimeSlot(ts); }
            }
            return recommendedExamList;
        }

        public List<Examination> RecommendTimeSlotByDoctor(int doctorId, TimeSlot ts, string firstDateString, DateTime firstDate, DateTime lastDate,
            List<Examination> recommendedExamList, Examination[] examinations)
        {
            TimeOnly start = TimeOnly.Parse(startTimeBox.Text);
            TimeOnly end = TimeOnly.Parse(endTimeBox.Text);
            // searching for timeslots untill last day
            while (recommendedExamList.Count == 0 && DateTime.Parse(ts.Date) < lastDate)
            {
                // searching for doctors timeslots in timespan for every day
                while (TimeOnly.Parse(ts.StartTime) >= start && TimeOnly.Parse(ts.StartTime).AddMinutes(15) <= end)
                {
                    if (Schedule.IsFree(null, doctorId, ts, "doctor") && Schedule.IsFree(null, Patient.Id, ts, "patient"))
                    {
                        Examination exam = new Examination((examinations[examinations.Length - 1].Id * 10 - 2),
                            false, Patient.Id, doctorId, ts, "created");
                        recommendedExamList.Add(exam);
                        break;
                    }
                    else { ts = TimeSlot.GetNextTimeSlot(ts); }
                }
                // moving to next day
                if (TimeOnly.Parse(ts.StartTime).AddMinutes(15) > end)
                {
                    firstDate.AddHours(24);
                    ts = new TimeSlot(firstDate.ToString("dd.MM.yyyy."), startTimeBox.Text, "00:15:00");
                }
            }
            // repeat the process, but without timespan
            if (recommendedExamList.Count == 0)
            {
                ts = new TimeSlot(firstDateString, startTimeBox.Text, "00:15:00");
                while (recommendedExamList.Count == 0 && DateTime.Parse(ts.Date) < lastDate)
                {
                    if (Schedule.IsFree(null, doctorId, ts, "doctor") && Schedule.IsFree(null, Patient.Id, ts, "patient"))
                    {
                        Examination exam = new Examination((examinations[examinations.Length - 1].Id * 10 - 2),
                            false, Patient.Id, doctorId, ts, "created");
                        recommendedExamList.Add(exam);
                        break;
                    }
                    else { ts = TimeSlot.GetNextTimeSlot(ts); }
                }
            }
            return recommendedExamList;
        }

        public List<Examination> RecommendTimeSlotByTimeSpan(int doctorId, TimeSlot ts, string firstDateString, DateTime firstDate, DateTime lastDate,
            List<Examination> recommendedExamList, Examination[] examinations)
        {
            TimeOnly start = TimeOnly.Parse(startTimeBox.Text);
            TimeOnly end = TimeOnly.Parse(endTimeBox.Text);
            // searching for timeslots untill last day
            while (recommendedExamList.Count == 0 && DateTime.Parse(ts.Date) < lastDate)
            {
                // searching for doctors timeslots in timespan for every day
                while (TimeOnly.Parse(ts.StartTime) >= start && TimeOnly.Parse(ts.StartTime).AddMinutes(15) <= end)
                {
                    if (Schedule.IsFree(null, doctorId, ts, "doctor") && Schedule.IsFree(null, Patient.Id, ts, "patient"))
                    {
                        Examination exam = new Examination((examinations[examinations.Length - 1].Id * 10 - 2),
                            false, Patient.Id, doctorId, ts, "created");
                        recommendedExamList.Add(exam);
                        break;
                    }
                    else { ts = TimeSlot.GetNextTimeSlot(ts); }
                }
                // moving to next day
                if (TimeOnly.Parse(ts.StartTime).AddMinutes(15) > end)
                {
                    firstDate.AddHours(24);
                    ts = new TimeSlot(firstDate.ToString("dd.MM.yyyy."), startTimeBox.Text, "00:15:00");
                }
            }
            // repeat the process, but for other doctor
            if (recommendedExamList.Count == 0)
            {
                Doctor[] docs = Doctor.LoadDoctors("../../../Data/DoctorData.json");
                List<Doctor> newDocs = docs.ToList();
                if (newDocs.Contains(Doctor.Find(doctorId))) { newDocs.Remove(Doctor.Find(doctorId)); }
                foreach (Doctor doc in newDocs)
                {
                    ts = new TimeSlot(firstDateString, startTimeBox.Text, "00:15:00");
                    while (recommendedExamList.Count == 0 && DateTime.Parse(ts.Date) < lastDate)
                    {
                        while (TimeOnly.Parse(ts.StartTime) >= start && TimeOnly.Parse(ts.StartTime).AddMinutes(15) <= end)
                        {
                            if (Schedule.IsFree(null, doc.Id, ts, "doctor") && Schedule.IsFree(null, Patient.Id, ts, "patient"))
                            {
                                Examination exam = new Examination((examinations[examinations.Length - 1].Id * 10 - 2),
                                    false, Patient.Id, doc.Id, ts, "created");
                                recommendedExamList.Add(exam);
                                break;
                            }
                            else { ts = TimeSlot.GetNextTimeSlot(ts); }
                        }
                        // moving to next day
                        if (TimeOnly.Parse(ts.StartTime).AddMinutes(15) > end)
                        {
                            firstDate.AddHours(24);
                            ts = new TimeSlot(firstDate.ToString("dd.MM.yyyy."), startTimeBox.Text, "00:15:00");
                        }
                    }
                }
            }
            return recommendedExamList;
        }

        private void CreateExaminationClick(object sender, RoutedEventArgs e)
        {
            Examination examination = dataGrid.SelectedItem as Examination;
            if (examination != null)
            {
                Examination.Create(examination.Id, false, examination.DoctorId, Patient.Id, examination.TimeSlot);
                MessageBox.Show("Examination created!", "Confirmation");
                this.Close();
            }
            else
            {
                MessageBox.Show("Select an examination to create!", "Warning");
            }
        }
    }
}
