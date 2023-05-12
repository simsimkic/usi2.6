using System;
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
using ZdravoCorp.HealthInstitution.Users;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    public partial class PatientExaminationWindow : Window
    {
        public Examination Examination;
        public PatientExaminationWindow(Examination examination)
        {
            Examination = examination;
            InitializeComponent();
            InitializeExamination(examination);
        }

        public void InitializeExamination(Examination examination)
        {
            exIdTitle.Content += " " + examination.Id.ToString();
            doctorBox.Text = Doctor.Find(examination.DoctorId).Username;
            doctorBox.IsEnabled = false;
            switch (Doctor.Find(examination.DoctorId).Speciality)
            {
                case 0:
                    docSpecBox.Text = "Cardiology";
                    break;
                case (Doctor.DoctorsSpeciality)1:
                    docSpecBox.Text = "General Medicine";
                    break;
                case (Doctor.DoctorsSpeciality)2:
                    docSpecBox.Text = "Surgery";
                    break;
                default:
                    break;
            }
            docSpecBox.IsEnabled = false;
            if (examination.IsOperation)
            {
                operationBox.Text = "true";
            }
            else { operationBox.Text = "false"; }
            operationBox.IsEnabled = false;
            dateBox.Text = examination.TimeSlot.Date;
            dateBox.IsEnabled = false;
            timeBox.Text = examination.TimeSlot.StartTime;
            timeBox.IsEnabled = false;
            durationBox.Text = examination.TimeSlot.Duration;
            durationBox.IsEnabled = false;
            stateBox.Text = examination.State;
            stateBox.IsEnabled = false;
        }
    }
}
