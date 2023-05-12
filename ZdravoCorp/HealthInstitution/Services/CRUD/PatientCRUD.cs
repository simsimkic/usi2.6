using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.Schedules;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.Services.Register
{
    internal class PatientCRUD
    {

        public static void RegisterNewPatient(string username, string password, string name, string surname, string birthday, string height, string weight, string medicalHistory)
        {
            MedicalRecord medicalRecord = new MedicalRecord(name, surname, birthday, height, weight, medicalHistory);

            Random random = new Random();
            int id = random.Next(10000, 999999);
            List<TimeSlot> busyTimeSlots = new List<TimeSlot>();

            Patient newPatient = new Patient(username, password, id, false, busyTimeSlots, medicalRecord);
            Patient[] patients = Patient.LoadPatients("../../../Data/PatientData.json");
            patients = AppendPatient(newPatient, patients);
            Patient.SavePatients(patients);

            Profile newProfile = new Profile(username, password, Profile.Type.patient);
            Profile[] profiles = Profile.LoadProfiles();
            profiles = AppendProfile(newProfile, profiles);
            Profile.SaveProfiles(profiles);
        }
        public static Patient[] AppendPatient(Patient patient, Patient[] patients)
        {
            Patient[] patientsPlusOne = new Patient[patients.Length + 1];
            for (int i = 0; i < patients.Length; i++)
            {
                patientsPlusOne[i] = patients[i];
            }
            patientsPlusOne[patientsPlusOne.Length - 1] = patient;
            return patientsPlusOne;
        }
        public static Profile[] AppendProfile(Profile profile, Profile[] profiles)
        {
            Profile[] profilePlusOne = new Profile[profiles.Length + 1];
            for (int i = 0; i < profiles.Length; i++)
            {
                profilePlusOne[i] = profiles[i];
            }
            profilePlusOne[profilePlusOne.Length - 1] = profile;
            return profilePlusOne;
        }


        public static void DeletePatient(Patient patientToBeDeleted)
        {
            DeleteInExaminations(patientToBeDeleted);

            DeleteInPatientData(patientToBeDeleted);
            DeleteInProfileData(patientToBeDeleted);
        }

        public static void DeleteInExaminations(Patient patientToBeDeleted)
        {
            Examination[] examinations = Examination.LoadExaminations("../../../data/examinations.json");
            foreach (var examination in examinations)
            {
                if (examination.PatientId == patientToBeDeleted.Id)
                {
                    Examination.Delete(examination.Id);
                }
            }
        }

        public static void DeleteInPatientData(Patient patientToBeDeleted)
        {

            Patient[] patients = Patient.LoadPatients("../../../Data/PatientData.json");
            Patient[] patientsMinusOne = new Patient[patients.Length - 1];
            int j = 0;
            for (int i = 0; i < patients.Length; i++)
            {
                if (patients[i].Id != patientToBeDeleted.Id)
                {
                    patientsMinusOne[j++] = patients[i];
                }
            }
            Patient.SavePatients(patientsMinusOne);
        }

        public static void DeleteInProfileData(Patient patientToBeDeleted)
        {
            Profile[] profiles = Profile.LoadProfiles();
            Profile[] profilesMinusOne = new Profile[profiles.Length - 1];
            int k = 0;
            for (int i = 0; i < profiles.Length; i++)
            {
                if (profiles[i].Username != patientToBeDeleted.Username)
                {
                    profilesMinusOne[k++] = profiles[i];
                }
            }
            Profile.SaveProfiles(profilesMinusOne);
        }


        public static void UpdatePatient(Patient PatientToBeUpdated, string username, string password, string name, string surname, string birthday, string height, string weight, string medicalHistory)
        {
            string oldUsername = PatientToBeUpdated.Username;
            PatientToBeUpdated.Username = username;
            PatientToBeUpdated.Password = password;
            PatientToBeUpdated.MedicalRecord.Name = name;
            PatientToBeUpdated.MedicalRecord.Surname = surname;
            PatientToBeUpdated.MedicalRecord.Birthday = birthday;
            PatientToBeUpdated.MedicalRecord.Height = height;
            PatientToBeUpdated.MedicalRecord.Weight = weight;
            PatientToBeUpdated.MedicalRecord.MedicalHistory = medicalHistory;

            UpdateInPatientData(PatientToBeUpdated);
            UpdateInProfileData(PatientToBeUpdated, oldUsername);
        }

        public static void UpdateInPatientData(Patient PatientToBeUpdated)
        {

            Patient[] patients = Patient.LoadPatients("../../../Data/PatientData.json");
            for (int i = 0; i < patients.Length; i++)
            {
                if (patients[i].Id == PatientToBeUpdated.Id)
                {
                    patients[i] = PatientToBeUpdated;
                }
            }
            Patient.SavePatients(patients);
        }

        public static void UpdateInProfileData(Patient PatientToBeUpdated, string oldUsername)
        {

            Profile[] profiles = Profile.LoadProfiles();
            for (int i = 0; i < profiles.Length; i++)
            {
                if (profiles[i].Username == oldUsername)
                {
                    profiles[i].Username = PatientToBeUpdated.Username;
                    profiles[i].Password = PatientToBeUpdated.Password;
                }
            }
            Profile.SaveProfiles(profiles);
        }
    }
}
