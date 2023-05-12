using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.HealthInstitution.Users.Patient;
using ZdravoCorp.HealthInstitution.Users;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZdravoCorp.HealthInstitution.Schedules
{
    public class TimeSlot : INotifyPropertyChanged
    {
        private string _date;
        private string _startTime;
        private string _duration;

        public string Date
        {
            get => _date;
            set
            {
                if (value != _date)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }
        public string StartTime
        {
            get => _startTime;
            set
            {
                if (value != _startTime)
                {
                    _startTime = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Duration
        {
            get => _duration;
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSlot(string date, string startTime, string duration)
        {
            _date = date;
            _startTime = startTime;
            _duration = duration;
        }

        public static DateTime GetStartTime(TimeSlot timeSlot)
        {
            string combinedTimeSlot = timeSlot._date + " " + timeSlot._startTime;
            DateTime startTimeSlot = DateTime.ParseExact(combinedTimeSlot, "dd.MM.yyyy. HH:mm:ss", null);
            return startTimeSlot;
        }

        public static DateTime GetEndTime(TimeSlot timeSlot)
        {
            TimeSpan duration = TimeSpan.Parse(timeSlot._duration);
            DateTime endTimeSlot = GetStartTime(timeSlot).Add(duration);
            return endTimeSlot;
        }

        public static TimeSlot GetNextTimeSlot(TimeSlot timeSlot)
        {
            DateTime oldDT = GetStartTime(timeSlot);
            DateTime newDT = oldDT.AddMinutes(5);
            return new TimeSlot(newDT.ToString("dd.MM.yyyy."), newDT.ToString("HH:mm:ss"), "00:15:00");
        }

        public static void Remove(Doctor doctor, Patient patient, TimeSlot oldTimeSlot)
        {
            foreach (TimeSlot timeSlot in doctor.GetBusyTimeSlots())
            {
                if (timeSlot.Date == oldTimeSlot.Date && timeSlot.StartTime == oldTimeSlot.StartTime &&
                    timeSlot.Duration == oldTimeSlot.Duration)
                {
                    doctor.GetBusyTimeSlots().Remove(timeSlot);
                    break;
                }
            }
            foreach (TimeSlot timeSlot in patient.GetBusyTimeSlots())
            {
                if (timeSlot.Date == oldTimeSlot.Date && timeSlot.StartTime == oldTimeSlot.StartTime &&
                    timeSlot.Duration == oldTimeSlot.Duration)
                {
                    patient.GetBusyTimeSlots().Remove(timeSlot);
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
