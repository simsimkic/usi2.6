using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.Schedules;

namespace ZdravoCorp.HealthInstitution.Users.Patient
{
    public class Patient : INotifyPropertyChanged
    {
        [JsonProperty]
        private string _username;
        [JsonProperty]
        private string _password;
        [JsonProperty]
        private int _id;
        [JsonProperty]
        private Boolean _isBlocked;
        [JsonProperty]
        private List<TimeSlot> _busyTimeSlots;
        [JsonProperty]
        private MedicalRecord _medicalRecord;

        public Patient(string username, string password)
        {
            _username = username;
            _password = password;
        }
        [JsonConstructor]
        public Patient(string username, string password, int id, Boolean isBlocked, List<TimeSlot> busyTimeSlots, MedicalRecord medicalRecord)
        {
            _username = username;
            _password = password;
            _id = id;
            _busyTimeSlots = busyTimeSlots;
            _medicalRecord = medicalRecord;
            _isBlocked = isBlocked;
        }

        public string Username
        {
            get => _username;
            set
            {
                if (value != _username)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Password { get => _password; set => _password = value; }
        public int Id { get => _id; set => _id = value; }
        public Boolean IsBlocked { get => _isBlocked; set => _isBlocked = value; }
        public List<TimeSlot> BusyTimeSlots { get => _busyTimeSlots; set => _busyTimeSlots = value; }
        public MedicalRecord MedicalRecord
        {
            get => _medicalRecord;
            set
            {
                if (value != _medicalRecord)
                {
                    _medicalRecord = value;
                    OnPropertyChanged();
                }

            }
        }
        public static Patient Find(int patientId)
        {
            Patient[] patients = LoadPatients("../../../Data/PatientData.json");
            foreach (Patient patient in patients)
            {
                if (patient.GetId() == patientId)
                {
                    return patient;
                }
            }
            return null;
        }
        public int GetId()
        {
            return _id;
        }

        public List<TimeSlot> GetBusyTimeSlots()
        {
            return _busyTimeSlots;
        }
        public void SetBusyTimeSlots(List<TimeSlot> newList)
        {
            _busyTimeSlots = newList;
        }
        public MedicalRecord GetMedicalRecord()
        {
            return _medicalRecord;
        }
        public void SetMedicalRecord(MedicalRecord medicalRecord)
        {
            _medicalRecord = medicalRecord;
        }
        public void SetData(Patient patient)
        {
            _username = patient._username;
            _password = patient._password;
            _id = patient._id;
            _busyTimeSlots = patient._busyTimeSlots;
            _medicalRecord = patient._medicalRecord;
            _isBlocked = patient._isBlocked;
        }

        public static Patient[] LoadPatients(string filename)
        {
            var jsontext = File.ReadAllText(filename);
            Patient[] allPatients = JsonConvert.DeserializeObject<Patient[]>(jsontext)!;
            return allPatients;
        }

        public static void SavePatients(Patient[] patients)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string patientsJson = System.Text.Json.JsonSerializer.Serialize(patients, options);
            File.WriteAllText("../../../Data/PatientData.json", patientsJson);
        }

        public static int FindByUsername(string username)
        {
            Patient[] patients = LoadPatients("../../../Data/PatientData.json");
            foreach (Patient patient in patients)
            {
                if (patient._username == username)
                {
                    return patient.GetId();
                }
            }
            return 0;
        }

        public static bool CheckIfBlocked(int patientId)
        {
            int createdNumber = 0;
            int updatedNumber = 0;
            ExaminationHistory[] allExaminations = ExaminationHistory.LoadExaminationHistory("../../../Data/ExaminationsHistoryData.json");
            foreach (ExaminationHistory examinationHistory in allExaminations)
            {
                DateTime modificationDate = DateTime.ParseExact(examinationHistory.ModificationDate,
                "dd.MM.yyyy. HH:mm:ss", null);
                //checking if action happend in last 30 days
                if (modificationDate.CompareTo(DateTime.Now.AddDays(-30)) > 0)
                {
                    if (examinationHistory.Examination.PatientId == patientId
                    && (examinationHistory.Examination.State == "updated"
                    || examinationHistory.Examination.State == "deleted"))
                    {
                        updatedNumber += 1;
                    }
                    else if (examinationHistory.Examination.PatientId == patientId
                        && examinationHistory.Examination.State == "created")
                    {
                        createdNumber += 1;
                    }
                }
            }
            if (createdNumber > 8)
            {
                return true;
            }
            if (updatedNumber >= 5)
            {
                return true;
            }
            return false;
        }

        public static void Block(Patient patient)
        {
            if (CheckIfBlocked(patient.GetId()))
            {
                patient.IsBlocked = true;
            }
            else
            {
                patient.IsBlocked = false;
            }
            patient.SetData(patient);
            WriteFile("../../../Data/PatientData.json", patient, patient);
        }

        public static void WriteFile(string filename, Patient oldPatient, Patient newPatient)
        {
            Patient[] patients = LoadPatients(filename);
            foreach (Patient patient in patients)
            {
                if (patient.GetId() == oldPatient.GetId())
                {
                    patient.SetData(oldPatient);

                }
                else if (patient.GetId() == newPatient.GetId())
                {
                    patient.SetData(newPatient);
                }
            }
            SavePatients(patients);
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
