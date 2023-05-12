using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.Schedules;

namespace ZdravoCorp.HealthInstitution.Users
{
    public class Doctor
    {
        public enum DoctorsSpeciality { Cardiology, GeneralMedicine, Surgery };

        [JsonProperty]
        public string Username;
        [JsonProperty]
        public string Password;
        [JsonProperty]
        public int Id;
        [JsonProperty]
        public DoctorsSpeciality Speciality;
        [JsonProperty]
        public List<TimeSlot> BusyTimeSlots;

        public Doctor(string username, string password)    //constructor for login
        {
            Username = username;
            Password = password;
        }
        [JsonConstructor]
        public Doctor(string username, string password, int id, DoctorsSpeciality speciality, List<TimeSlot> busyTimeSlots)
        {
            Username = username;
            Password = password;
            Id = id;
            Speciality = speciality;
            BusyTimeSlots = busyTimeSlots;
        }
        public static int FindByUsername(string username)
        {
            Doctor[] doctors = LoadDoctors("../../../Data/DoctorData.json");
            foreach (Doctor doctor in doctors)
            {
                if (doctor.Username == username)
                {
                    return doctor.Id;
                }
            }
            return 0;
        }
        public static Doctor Find(int doctorId)
        {
            Doctor[] doctors = LoadDoctors("../../../Data/DoctorData.json");
            foreach (Doctor doctor in doctors)
            {
                if (doctor.GetId() == doctorId)
                {
                    return doctor;
                }
            }
            return null;

        }
        public static List<Doctor> FindSpecialisedDoctors(Doctor.DoctorsSpeciality speciality)
        {
            Doctor[] allDoctors = Doctor.LoadDoctors("../../../Data/DoctorData.json");
            List<Doctor> specialisedDoctors = new List<Doctor>();
            foreach (Doctor d in allDoctors)
            {
                if (d.Speciality == speciality)
                {
                    specialisedDoctors.Add(d);
                }
            }
            return specialisedDoctors;

        }

        public int GetId()
        {
            return Id;
        }
        public List<TimeSlot> GetBusyTimeSlots()
        {
            return BusyTimeSlots;
        }
        public void SetBusyTimeSlots(List<TimeSlot> newList)
        {
            BusyTimeSlots = newList;
        }
        public void SetData(Doctor doctor)
        {
            this.Username = doctor.Username;
            this.Password = doctor.Password;
            this.Id = doctor.Id;
            this.BusyTimeSlots = doctor.BusyTimeSlots;
        }
        public static Doctor[] LoadDoctors(string filename)
        {
            var jsontext = File.ReadAllText(filename);
            Doctor[] doctors = JsonConvert.DeserializeObject<Doctor[]>(jsontext)!;
            return doctors;
        }



        public static void WriteFile(string filename, Doctor oldDoctor, Doctor newDoctor)
        {
            Doctor[] doctors = LoadDoctors(filename);
            foreach (Doctor doctor in doctors)
            {
                if (doctor.GetId() == oldDoctor.GetId())
                {
                    doctor.SetData(oldDoctor);

                }
                else if (doctor.GetId() == newDoctor.GetId())
                {
                    doctor.SetData(newDoctor);
                }
            }
            File.WriteAllText(@"../../../Data/DoctorData.json", JsonConvert.SerializeObject(doctors, Formatting.Indented));

        }

    }
}
