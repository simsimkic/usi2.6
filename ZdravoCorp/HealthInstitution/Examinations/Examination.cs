using Microsoft.Windows.Themes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using ZdravoCorp.HealthInstitution.GUI;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;
using ZdravoCorp.HealthInstitution.Schedules;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace ZdravoCorp.HealthInstitution.Examinations
{
    public class Examination : INotifyPropertyChanged
    {
        [JsonProperty]
        private int _id;
        [JsonProperty]
        private bool _isOperation;
        [JsonProperty]
        private int _patientId;
        [JsonProperty]
        private int _doctorId;
        [JsonProperty]
        private TimeSlot _timeSlot;
        [JsonProperty]
        private string _state;

        public Examination(int id, bool isOperation, int patientId, int doctorId, TimeSlot timeSlot, string state)
        {
            Id = id;
            IsOperation = isOperation;
            PatientId = patientId;
            DoctorId = doctorId;
            TimeSlot = timeSlot;
            State = state;
        }
        public int Id
        {
            get => _id;
            set
            {
                if (value != _id)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsOperation { get => _isOperation; set => _isOperation = value; }
        public int PatientId { get => _patientId; set => _patientId = value; }
        public int DoctorId
        {
            get => _doctorId;
            set
            {
                if (value != _doctorId)
                {
                    _doctorId = value;
                    OnPropertyChanged();
                }
            }
        }
        public TimeSlot TimeSlot
        {
            get => _timeSlot;
            set
            {
                if (value != _timeSlot)
                {
                    _timeSlot = value;
                    OnPropertyChanged();
                }
            }
        }
        public string State
        {
            get => _state;
            set
            {
                if (value != _state)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }
        public override string? ToString()
        {
            return "[id: " + Id + ", isOperation: " + IsOperation + ", patientId: " + PatientId + " doctorId: " + DoctorId
                + ", date: " + TimeSlot.Date + ", startTime: " + TimeSlot.StartTime + ", duration: " + TimeSlot.Duration + ", state: " + State + "]";
        }

        public static List<Examination> GetActiveExaminationsForToday()
        {
            DateTime now = DateTime.Now;
            string date = now.ToString("dd.MM.yyyy.");
            List<Examination> todaysExaminations = Examination.GetAllExaminationsForDate(date);
            List<Examination> activeExaminations = new List<Examination>();
            foreach (Examination examination in todaysExaminations)
            {
                DateTime startTime = TimeSlot.GetStartTime(examination.TimeSlot);
                if (now.CompareTo(startTime) <= 0)
                {
                    activeExaminations.Add(examination);
                }
            }
            return activeExaminations;

        }

        public static List<Examination> GetAllExaminationsForDate(string date)
        {
            Examination[] allExaminations = LoadExaminations("../../../Data/Examinations.json");
            List<Examination> filteredExaminations = new List<Examination>();
            for (int i = 0; i < allExaminations.Length; i++)
            {
                if (allExaminations[i].TimeSlot.Date == date && allExaminations[i].State != "deleted")
                {
                    filteredExaminations.Add(allExaminations[i]);
                }
            }
            return filteredExaminations;
        }

        public static Examination getExaminationByTimeSlot(Doctor doctor, TimeSlot timeSlot)
        {
            Examination[] allExaminations = LoadExaminations("../../../Data/Examinations.json");
            for (int i = 0; i < allExaminations.Length; i++)
            {
                if (doctor.Id == allExaminations[i].DoctorId && allExaminations[i].TimeSlot.Date == timeSlot.Date && allExaminations[i].TimeSlot.StartTime == timeSlot.StartTime)
                {
                    return allExaminations[i];
                }
            }
            return null;
        }


        public static List<Examination> GetDoctorExaminations(int doctorId)
        {
            Examination[] allExaminations = LoadExaminations("../../../Data/Examinations.json");
            List<Examination> doctorsExaminations = new List<Examination>();
            for (int i = 0; i < allExaminations.Length; i++)
            {
                if (allExaminations[i].DoctorId == doctorId)
                {
                    doctorsExaminations.Add(allExaminations[i]);
                }
            }
            return doctorsExaminations;
        }
        public static List<Examination> GetDoctorExaminationsForDate(int doctorId, string date)
        {
            Examination[] allExaminations = LoadExaminations("../../../Data/Examinations.json");
            List<Examination> filteredExaminations = new List<Examination>();
            for (int i = 0; i < allExaminations.Length; i++)
            {
                if (allExaminations[i].DoctorId == doctorId && allExaminations[i].TimeSlot.Date == date
                    && allExaminations[i].State != "deleted")
                {
                    filteredExaminations.Add(allExaminations[i]);
                }
            }
            return filteredExaminations;

        }

        public static Examination[] LoadExaminations(string filename)
        {
            var jsontext = File.ReadAllText(filename);
            Examination[] examinations = JsonConvert.DeserializeObject<Examination[]>(jsontext)!;
            return examinations;
        }


        public static bool Create(int id, bool isOperation, int doctorId, int patientId, TimeSlot timeSlot)
        {
            Examination[] examinations = LoadExaminations("../../../Data/Examinations.json");
            Examination exam = FindExamination(id, examinations);
            Patient patient = Patient.Find(patientId);
            Doctor doctor = Doctor.Find(doctorId);
            DateTime examinationDateTime = DateTime.ParseExact(timeSlot.Date + " " + timeSlot.StartTime,
                "dd.MM.yyyy. HH:mm:ss", null);
            if (examinationDateTime.CompareTo(DateTime.Now) > 0) // checking if examination is in future
            {
                if (exam == null && patient != null && doctor != null)  //if the examination isn't found and doctor and patient are found
                {
                    if (Schedule.IsFree(exam, patientId, timeSlot, "patient"))
                    {
                        if (Schedule.IsFree(exam, doctorId, timeSlot, "doctor"))
                        {
                            exam = new Examination(id, isOperation, patientId, doctorId, timeSlot, "created");

                            patient.GetBusyTimeSlots().Add(timeSlot);
                            doctor.GetBusyTimeSlots().Add(timeSlot);

                            examinations = examinations.Concat(new Examination[] { exam }).ToArray();

                            //exams file changed examination data
                            Save(examinations);
                            //File.WriteAllText(@"../../../Data/Examinations.json", JsonConvert.SerializeObject(examinations, Formatting.Indented));
                            //doctors file changed timeslots
                            Doctor.WriteFile("../../../Data/DoctorData.json", doctor, doctor);
                            //patients file changed timeslots
                            Patient.WriteFile("../../../Data/PatientData.json", patient, patient);
                            //adding action to history
                            ExaminationHistory[] allExaminations = ExaminationHistory.LoadExaminationHistory("../../../Data/ExaminationsHistoryData.json").
                                Concat(new ExaminationHistory[] { new ExaminationHistory(DateTime.Now.ToString("dd.MM.yyyy. HH:mm:ss"), exam) }).ToArray();
                            ExaminationHistory.SaveExaminationHistory(allExaminations);

                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Doctor isn't free for this date", "Warning");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Patient isn't free for this date", "Warning");
                    }
                }
                else
                {
                    if (exam != null)
                    {
                        MessageBox.Show("Exam id already exists", "Warning");
                    }
                    if (patient == null)
                    {
                        MessageBox.Show("Patient id doesn't exist", "Warning");
                    }
                    if (doctor == null)
                    {
                        MessageBox.Show("Doctor id dosen't exist", "Warning");
                    }
                }
            }
            else
            {
                MessageBox.Show("Examination cannot be created in the past", "Warning");
            }
            return false;
        }

        public static bool Update(int id, bool isOperation, int doctorId, int patientId, TimeSlot timeSlot)
        {
            Examination[] examinations = LoadExaminations("../../../Data/Examinations.json");
            Examination exam = FindExamination(id, examinations);
            Patient newPatient = Patient.Find(patientId);
            Doctor newDoctor = Doctor.Find(doctorId);
            DateTime examDateTime = DateTime.ParseExact(exam.TimeSlot.Date + " " + exam.TimeSlot.StartTime,
                "dd.MM.yyyy. HH:mm:ss", null);
            DateTime newDateTime = DateTime.ParseExact(timeSlot.Date + " " + timeSlot.StartTime,
                "dd.MM.yyyy. HH:mm:ss", null);
            if (DateTime.Now.CompareTo(examDateTime.AddDays(-1)) < 0 && newDateTime.CompareTo(DateTime.Now) > 0)
            {
                if (exam != null && newPatient != null && newDoctor != null && exam.State != "deleted")  //if the examination,doctor and patient are found and the examination isn't deleted
                {
                    if (Schedule.IsFree(exam, patientId, timeSlot, "patient"))
                    {
                        if (Schedule.IsFree(exam, doctorId, timeSlot, "doctor"))
                        {
                            foreach (Examination examination in examinations)
                            {
                                if (examination.Id == id)
                                {
                                    examination.IsOperation = isOperation;

                                    int oldDoctorId = examination.DoctorId;
                                    int oldPatientId = examination.PatientId;
                                    Doctor oldDoctor = Doctor.Find(oldDoctorId);
                                    Patient oldPatient = Patient.Find(oldPatientId);

                                    TimeSlot.Remove(oldDoctor, oldPatient, examination.TimeSlot);

                                    examination.DoctorId = doctorId;
                                    examination.PatientId = patientId;

                                    if (doctorId == oldDoctorId)
                                    {
                                        oldDoctor.GetBusyTimeSlots().Add(timeSlot);
                                        Doctor.WriteFile("../../../Data/DoctorData.json", oldDoctor, oldDoctor);

                                    }
                                    else
                                    {
                                        newDoctor.GetBusyTimeSlots().Add(timeSlot);
                                        Doctor.WriteFile("../../../Data/DoctorData.json", oldDoctor, newDoctor);
                                    }
                                    if (patientId == oldPatientId)
                                    {
                                        oldPatient.GetBusyTimeSlots().Add(timeSlot);
                                        Patient.WriteFile("../../../Data/PatientData.json", oldPatient, oldPatient);
                                    }
                                    else
                                    {
                                        newPatient.GetBusyTimeSlots().Add(timeSlot);
                                        Patient.WriteFile("../../../Data/PatientData.json", oldPatient, newPatient);
                                    }

                                    examination.TimeSlot = timeSlot;
                                    examination.State = "updated";

                                    //exams file changed examination data
                                    Save(examinations);
                                    //File.WriteAllText(@"../../../Data/Examinations.json", JsonConvert.SerializeObject(examinations, Formatting.Indented));
                                    //adding action to history
                                    ExaminationHistory[] allExaminations = ExaminationHistory.LoadExaminationHistory("../../../Data/ExaminationsHistoryData.json").
                                        Concat(new ExaminationHistory[] { new ExaminationHistory(DateTime.Now.ToString("dd.MM.yyyy. HH:mm:ss"), examination) }).ToArray();
                                    ExaminationHistory.SaveExaminationHistory(allExaminations);

                                    return true;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Doctor isn't free for this date", "Warning");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Patient isn't free for this date", "Warning");
                    }
                }
                else
                {
                    if (exam == null)
                    {
                        MessageBox.Show("Exam id doesn't exist", "Warning");
                    }
                    else if (exam.State == "deleted")
                    {
                        MessageBox.Show("Exam under entered id has been deleted");
                    }
                    if (newPatient == null)
                    {
                        MessageBox.Show("Patient id doesn't exist", "Warning");
                    }
                    if (newDoctor == null)
                    {
                        MessageBox.Show("Doctor id dosen't exist", "Warning");
                    }
                }
            }
            else
            {
                if (DateTime.Now.CompareTo(examDateTime.AddDays(-1)) >= 0)
                {
                    MessageBox.Show("Examination cannot be updated less then one day before it's date", "Warning");
                }
                else if (newDateTime.CompareTo(DateTime.Now) <= 0)
                {
                    MessageBox.Show("Examination cannot be created in the past", "Warning");
                }
            }
            return false;
        }
        public static bool IsPatients(int patientId, int examinationId)
        {
            Examination[] allExaminations = Examination.LoadExaminations("../../../Data/Examinations.json");
            foreach (Examination exam in allExaminations)
            {
                if (exam.PatientId == patientId && exam.Id == examinationId)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsDoctors(int doctorId, int examinationId)
        {
            Examination[] allExaminations = Examination.LoadExaminations("../../../Data/Examinations.json");
            foreach (Examination exam in allExaminations)
            {
                if (exam.DoctorId == doctorId && exam.Id == examinationId)
                {
                    return true;
                }
            }
            return false;
        }

        public static Examination FindExamination(int id, Examination[] examinations)
        {
            foreach (Examination exam in examinations)
            {
                if (exam.Id == id)
                {
                    return exam;
                }
            }
            return null;
        }

        public static bool Delete(int id)
        {
            Examination[] examinations = LoadExaminations("../../../Data/Examinations.json");
            Examination exam = FindExamination(id, examinations);
            DateTime examDateTime = DateTime.ParseExact(exam.TimeSlot.Date + " " + exam.TimeSlot.StartTime,
                "dd.MM.yyyy. HH:mm:ss", null);
            if (DateTime.Now.CompareTo(examDateTime.AddDays(-1)) < 0)
            {
                if (exam != null && exam.State != "deleted")
                {
                    Patient patient = Patient.Find(exam.PatientId);
                    Doctor doctor = Doctor.Find(exam.DoctorId);
                    TimeSlot.Remove(doctor, patient, exam.TimeSlot);
                    exam.State = "deleted";

                    //exams file changed examination data
                    Save(examinations);
                    //File.WriteAllText(@"../../../Data/Examinations.json", JsonConvert.SerializeObject(examinations, Formatting.Indented));
                    //doctors file changed timeslots
                    Doctor.WriteFile("../../../Data/DoctorData.json", doctor, doctor);
                    //patients file changed timeslots
                    Patient.WriteFile("../../../Data/PatientData.json", patient, patient);
                    //adding action to history
                    ExaminationHistory[] allExaminations = ExaminationHistory.LoadExaminationHistory("../../../Data/ExaminationsHistoryData.json").
                        Concat(new ExaminationHistory[] { new ExaminationHistory(DateTime.Now.ToString("dd.MM.yyyy. HH:mm:ss"), exam) }).ToArray();
                    ExaminationHistory.SaveExaminationHistory(allExaminations);

                    return true;
                }
                else
                {
                    if (exam == null)
                    {
                        MessageBox.Show("Examination id doesn't exist", "Warning");
                    }
                    else if (exam.State == "deleted")
                    {
                        MessageBox.Show("Examination has already been deleted", "Warning");
                    }
                }
            }
            else
            {
                MessageBox.Show("Examination cannot be deleted less then one day before it's date", "Warning");
            }

            return false;
        }

        public static void Save(Examination[] examinations)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string examsJson = System.Text.Json.JsonSerializer.Serialize(examinations, options);
            File.WriteAllText("../../../Data/Examinations.json", examsJson);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
