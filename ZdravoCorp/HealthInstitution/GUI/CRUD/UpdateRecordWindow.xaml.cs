using System;
using System.Collections.Generic;
using System.Globalization;
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
using ZdravoCorp.HealthInstitution.Services.DoctorServices;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    /// <summary>
    /// Interaction logic for UpdateRecordWindow.xaml
    /// </summary>
    public partial class UpdateRecordWindow : Window
    {
        public int DoctorId;
        public int PatientId;
        public bool IsCurrentExam;
       
        public UpdateRecordWindow(MedicalRecord record,int doctorId,int patientId,bool isCurrentExam)
        {
            DoctorId= doctorId;
            PatientId = patientId;
            IsCurrentExam= isCurrentExam;
            InitializeComponent();
            InitializeData(record);

        }

        private void InitializeData(MedicalRecord record)
        {
            txtName.Text = record.Name;
            txtSurname.Text = record.Surname;
            DateTime dt = Convert.ToDateTime(record.Birthday);
            dtpBirthday.SelectedDate = dt;
            txtHeight.Text = record.Height;
            txtWeight.Text = record.Weight;
            txtHistory.Text = record.MedicalHistory;
        }

        private void BtnUpdateClick(object sender, RoutedEventArgs e)
        {
            if (txtName.Text != "" && txtSurname.Text != "" && txtHeight.Text != ""
                   && txtWeight.Text != "" && txtHistory.Text != "" && dtpBirthday.SelectedDate != null)
            {
                MedicalRecord newRecord = new MedicalRecord(txtName.Text, txtSurname.Text, dtpBirthday.SelectedDate.Value.ToShortDateString(),
                              txtHeight.Text, txtWeight.Text, txtHistory.Text);
                PerformExaminationService.UpdateMedicalRecord(newRecord,DoctorId, PatientId,IsCurrentExam);

            }
            else
            {
                MessageBox.Show("Not everything is filled out", "Warning");
            }
               
        }
    }
}
