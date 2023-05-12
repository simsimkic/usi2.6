using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.GUI.CRUD;
using ZdravoCorp.HealthInstitution.Schedules;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.Services.ExaminationServices
{
    public class EmergencyExaminationService
    {
        public static bool TryToSchedule(Patient patient, Doctor.DoctorsSpeciality speciality)
        {
            bool wasScheduled = false;

            List<Doctor> specialisedDoctors = Doctor.FindSpecialisedDoctors(speciality);
            if (specialisedDoctors == null)
            {
                MessageBox.Show("There are no doctors with that speciality");
                return wasScheduled;
            }
            
            for(int i = 0; i < specialisedDoctors.Count(); i++)
            {
                wasScheduled = TryToScheduleInFreeTimeSlots(specialisedDoctors[i], patient);
            }

            return wasScheduled;
        }

        public static bool TryToScheduleInFreeTimeSlots(Doctor doctor, Patient patient)
        {
            bool wasScheduled = false;
            List<DateTime> freeStartTimes = GetFreeStartTimesInTimeWindow(doctor, 2);
            for (int j = 0; j < freeStartTimes.Count(); j++)
            {

                string date = freeStartTimes[j].ToString("dd.MM.yyyy.");
                string time = freeStartTimes[j].ToString("HH:mm:ss");
                TimeSlot freeTimeSlot = new TimeSlot(date, time, "00:15:00");

                Examination exam = CreateExamination(patient, doctor, freeTimeSlot);

                wasScheduled = Schedule.IsFree(null, doctor.Id, freeTimeSlot, "doctor");
                if (wasScheduled)
                {
                    Examination.Create(exam.Id, true, doctor.Id, patient.Id, freeTimeSlot);
                    return wasScheduled;
                }
            }
            return wasScheduled;
        }


        //for each specialised doctor, finds all of his examinations in the next two hours, and for each examination finds how far in the future it would have to be moved
        //return the 5 examinations that would be moved to a soonest time
        public static Dictionary<Examination, TimeSpan> GetFiveLeastImportantExaminations(Doctor.DoctorsSpeciality speciality)
        {
            Dictionary<Examination, TimeSpan> examinationsAndOffset = new Dictionary<Examination, TimeSpan>();
            List<Doctor> specialisedDoctors = Doctor.FindSpecialisedDoctors(speciality);
            for (int i = 0; i < specialisedDoctors.Count(); i++)
            {
                List<Examination> doctorsExams = FindExaminationsInTimeWindow(specialisedDoctors[i], 2);
                for (int j = 0; j < doctorsExams.Count(); j++)
                {
                    List<DateTime> freeStartTimes = GetSortedStartTimes(specialisedDoctors[i]);
                    for (int k = 0; k < freeStartTimes.Count(); k++)
                    {
                        TimeSlot currentTimeSlot = new TimeSlot(doctorsExams[j].TimeSlot.Date, doctorsExams[j].TimeSlot.StartTime, doctorsExams[j].TimeSlot.Duration);

                        TimeSlot goalTimeSlot = new TimeSlot(doctorsExams[j].TimeSlot.Date, doctorsExams[j].TimeSlot.StartTime, doctorsExams[j].TimeSlot.Duration);
                        goalTimeSlot.StartTime = freeStartTimes[k].ToString("HH:mm:ss");

                        bool canBeScheduled = Schedule.IsFree(null, specialisedDoctors[i].Id, goalTimeSlot, "doctor");
                        if (canBeScheduled)
                        {
                            DateTime currentExaminationDateTime = DateTime.ParseExact(currentTimeSlot.Date + " " + currentTimeSlot.StartTime, "dd.MM.yyyy. HH:mm:ss", null);
                            DateTime movedExaminationDateTime = DateTime.ParseExact(goalTimeSlot.Date + " " + goalTimeSlot.StartTime, "dd.MM.yyyy. HH:mm:ss", null);
                            TimeSpan difference = movedExaminationDateTime - currentExaminationDateTime;
                            examinationsAndOffset[doctorsExams[j]] = difference;
                            break;
                        }
                    }
                }
            }

            var sortedDict = from entry in examinationsAndOffset orderby entry.Value ascending select entry;
            int fiveCounter = 0;
            Dictionary<Examination, TimeSpan> firstFiveExaminations = new Dictionary<Examination, TimeSpan>();
            foreach (KeyValuePair<Examination, TimeSpan> pair in sortedDict)
            {
                firstFiveExaminations[pair.Key] = pair.Value;
                fiveCounter++;
                if (fiveCounter == 5) break;
            }

            return firstFiveExaminations;
        }


        public static Examination CreateExamination(Patient patient, Doctor doctor, TimeSlot freeTimeSlot)
        {
            Random random = new Random();
            int id = random.Next(10000, 999999);


            Examination exam = new Examination(id, true, patient.Id, doctor.Id, freeTimeSlot, "created");
            return exam;
        }

        public static List<DateTime> GetFreeStartTimesInTimeWindow (Doctor doctor, int timeWindow)
        {
            List<DateTime> startTimes = new List<DateTime>();
            List<TimeSlot> busyTimeSlots = doctor.GetBusyTimeSlots();

            DateTime now = DateTime.Now;
            now = now.AddSeconds(10);
            DateTime maxStartTime = now.AddHours(timeWindow);

            bool isNowCovered = false;
            for (int i = 0; i < busyTimeSlots.Count(); i++)
            {
                if (TimeSlot.GetStartTime(busyTimeSlots[i]).CompareTo(now) <= 0 && now.CompareTo(TimeSlot.GetEndTime(busyTimeSlots[i])) <= 0)
                {
                    isNowCovered = true;
                    break;
                }
            }
            if(!isNowCovered) { startTimes.Add(now); }

            for (int i = 0; i < busyTimeSlots.Count(); i++)
            {
                if (maxStartTime.CompareTo(TimeSlot.GetEndTime(busyTimeSlots[i])) <= 0)
                {
                    continue;
                }
                if (now.CompareTo(TimeSlot.GetEndTime(busyTimeSlots[i])) <= 0)
                {
                    startTimes.Add(TimeSlot.GetEndTime(busyTimeSlots[i]));
                }
            }
            return startTimes;
        }

        public static List<Examination> FindExaminationsInTimeWindow(Doctor doctor, int timeWindow)
        {
            List<Examination> examinations = new List<Examination>();
            List<TimeSlot> busyTimeSlots = doctor.GetBusyTimeSlots();

            DateTime now = DateTime.Now;
            DateTime maxStartTime = now.AddHours(timeWindow);

            for (int i = 0; i < busyTimeSlots.Count(); i++)
            {
                if (maxStartTime.CompareTo(TimeSlot.GetStartTime(busyTimeSlots[i])) <= 0)
                {
                    continue;
                }
                if (now.CompareTo(TimeSlot.GetStartTime(busyTimeSlots[i])) <= 0)
                {
                    examinations.Add(Examination.getExaminationByTimeSlot(doctor, busyTimeSlots[i]));
                }
            }

            return examinations;
        }

        public static List<DateTime> GetSortedStartTimes(Doctor doctor)
        {
            List<DateTime> freeStartTimes = GetFreeStartTimesInTimeWindow(doctor, 168);
            freeStartTimes.Sort();
            return freeStartTimes;
        }


        public static bool UpdateExeminationInEmergency(int id, bool isOperation, int doctorId, int patientId, TimeSlot timeSlot)
        {
            Examination[] examinations = Examination.LoadExaminations("../../../Data/Examinations.json");
            Examination exam = Examination.FindExamination(id, examinations);
            Patient newPatient = Patient.Find(patientId);
            Doctor newDoctor = Doctor.Find(doctorId);
            DateTime examDateTime = DateTime.ParseExact(exam.TimeSlot.Date + " " + exam.TimeSlot.StartTime,
                "dd.MM.yyyy. HH:mm:ss", null);
            DateTime newDateTime = DateTime.ParseExact(timeSlot.Date + " " + timeSlot.StartTime,
                "dd.MM.yyyy. HH:mm:ss", null);
            if (newDateTime.CompareTo(DateTime.Now) > 0)
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
                                    File.WriteAllText(@"../../../Data/Examinations.json", JsonConvert.SerializeObject(examinations, Formatting.Indented));
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
                if (newDateTime.CompareTo(DateTime.Now) <= 0)
                {
                    MessageBox.Show("Examination cannot be created in the past", "Warning");
                }
            }
            return false;
        }
    }
}
