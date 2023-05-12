using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.Users.Patient;
using ZdravoCorp.HealthInstitution.Users;
using Newtonsoft.Json.Linq;

namespace ZdravoCorp.HealthInstitution.Services.DoctorServices
{
    internal class PerformExaminationService
    {
        public static void UpdateMedicalRecord(MedicalRecord newRecord, int doctorId, int patientId,bool isCurrnentExam)
        {   
            if (isCurrnentExam || HasDoctorExaminedPatient(doctorId,patientId))
            {
                Patient oldPatient = Patient.Find(patientId); // record is being updated for this patient
                Patient newPatient = oldPatient;
                newPatient.SetMedicalRecord(newRecord);
                Patient.WriteFile("../../../Data/PatientData.json", oldPatient, newPatient);
                MessageBox.Show("Medical record updated");
            } 
            else
            {
                MessageBox.Show("You cannot updated this record");
            }
        }

        private static bool HasDoctorExaminedPatient(int doctorId, int patientId)
        {
            Examination[] examinations = Examination.LoadExaminations("../../../Data/Examinations.json");
            foreach (Examination exam in examinations)
            {
                if (exam.DoctorId == doctorId && exam.PatientId == patientId)  //this doctor and this patient
                {
                    DateTime date = Convert.ToDateTime(exam.TimeSlot.Date + exam.TimeSlot.StartTime);
                    if (date < DateTime.Now)  //exam date has passed
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void UpdateAnamnesis(Anamnesis newAnamnesis)
        {
            Anamnesis[] anamneses = Anamnesis.LoadAnamnesis();
            foreach(Anamnesis anamnesis in anamneses)
            {
                if (anamnesis.ExaminationId == newAnamnesis.ExaminationId)
                {
                    anamnesis.Symptoms = newAnamnesis.Symptoms;
                    anamnesis.EarlierIllnesses = newAnamnesis.EarlierIllnesses;
                    anamnesis.Alergies = newAnamnesis.Alergies;
                    anamnesis.Diagnosis = newAnamnesis.Diagnosis;
                    break;
                }
            }
            Anamnesis.SaveAnamnesis(anamneses);
            MessageBox.Show("Anamnesis updated");
        }
        internal static void CreateAnamnesis(Anamnesis anamnesis)
        {
            Anamnesis[] anamneses = Anamnesis.LoadAnamnesis();
            anamneses = anamneses.Append(anamnesis).ToArray();
            Anamnesis.SaveAnamnesis(anamneses);
            MessageBox.Show("Anamnesis created");
        }

        internal static bool TryPerformingExam(Examination exam)
        {
            DateTime now=DateTime.Now;
            DateTime examTime = DateTime.Parse(exam.TimeSlot.Date+exam.TimeSlot.StartTime);
            if(examTime.Date==now.Date && exam.State != "deleted" &&
                now>= examTime.Subtract(new TimeSpan(0, 15, 0))&& now<= examTime)
            {
                return true;
            }
            return false;
        }

  
    }
}
