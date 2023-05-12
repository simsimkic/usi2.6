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
using ZdravoCorp.HealthInstitution.Schedules;
using ZdravoCorp.HealthInstitution.Users;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    /// <summary>
    /// Interaction logic for UpdateExaminationWindow.xaml
    /// </summary>
    public partial class UpdateExaminationDoctorWindow : Window
    {
        public int DocId;
        public UpdateExaminationDoctorWindow(Examination exam, int docId)
        {
            DocId = docId;
            InitializeComponent();
            InitilizeData(exam);
        }

        private void InitilizeData(Examination exam)
        {
            if (exam != null)
            {
                btnCreate.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                txtDocId.Text = exam.DoctorId.ToString();
                txtDocId.IsEnabled = false;
                txtExamId.Text = exam.Id.ToString();
                txtExamId.IsEnabled = false;
                txtPatientId.Text = exam.PatientId.ToString();
                if (exam.IsOperation)
                {
                    rbtnTrue.IsChecked = true;
                }
                else
                {
                    rbtnFalse.IsChecked = true;
                }
                DateTime dt = Convert.ToDateTime(exam.TimeSlot.Date);
                dtp.SelectedDate = dt;
                txtTime.Text = exam.TimeSlot.StartTime.ToString();
                txtDuration.Text = exam.TimeSlot.Duration.ToString();
            }
            else  //for create, exam is null, only docId is known
            {
                btnUpdate.IsEnabled = false;
                btnCreate.IsEnabled = true;
                txtDocId.Text = DocId.ToString();
                txtDocId.IsEnabled = false;

            }

        }

        private void BtnCreateClick(object sender, RoutedEventArgs e)
        {

            if (txtExamId.Text == "" || txtPatientId.Text == "" || (rbtnTrue.IsChecked != true && rbtnFalse.IsChecked != true)
                || dtp.SelectedDate == null || txtTime.Text == "" || txtDuration.Text == "")
            {
                MessageBox.Show("Not everything is filled out!");
            }
            else
            {
                try
                {
                    string date = dtp.SelectedDate.Value.Date.ToString("dd.MM.yyyy.");
                    TimeSlot timeslot = new TimeSlot(date, txtTime.Text, txtDuration.Text);
                    bool ex;
                    if (rbtnTrue.IsChecked == true)
                    {
                        ex = Examination.Create(int.Parse(txtExamId.Text), true, int.Parse(txtDocId.Text),
                        int.Parse(txtPatientId.Text), timeslot);
                    }
                    else
                    {
                        ex = Examination.Create(int.Parse(txtExamId.Text), false, int.Parse(txtDocId.Text),
                        int.Parse(txtPatientId.Text), timeslot);
                    }
                    if (ex != false)
                    {
                        MessageBox.Show("Examinaton created");

                    }

                }
                catch
                {
                    MessageBox.Show("Invalid input types", "Warning");
                }
            }

        }

        private void BtnUpdateClick(object sender, RoutedEventArgs e)
        {
            if (txtExamId.Text != "")
            {
                try
                {

                    if (txtPatientId.Text == "" || (rbtnTrue.IsChecked != true && rbtnFalse.IsChecked != true)
               || dtp.SelectedDate == null || txtTime.Text == "" || txtDuration.Text == "")
                    {
                        MessageBox.Show("Not everything is filled out!");
                    }
                    else
                    {
                        string date = dtp.SelectedDate.Value.Date.ToString("dd.MM.yyyy.");
                        TimeSlot timeslot = new TimeSlot(date, txtTime.Text, txtDuration.Text);
                        bool ex;
                        if (rbtnTrue.IsChecked == true)
                        {
                            ex = Examination.Update(int.Parse(txtExamId.Text), true, int.Parse(txtDocId.Text),
                            int.Parse(txtPatientId.Text), timeslot);
                        }
                        else
                        {
                            ex = Examination.Update(int.Parse(txtExamId.Text), false, int.Parse(txtDocId.Text),
                              int.Parse(txtPatientId.Text), timeslot);
                        }
                        if (ex == true)
                        {
                            MessageBox.Show("Examinaton updated");
                        }

                    }
                }
                catch
                {
                    MessageBox.Show("Invelid input types", "Warning");
                }
            }
            else
            {
                MessageBox.Show("Enter the examination id");
            }
        }
    }

}
