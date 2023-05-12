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
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    public partial class PatientExaminationCRUDWindow : Window
    {
        public int PatientId;
        public int DoctorId;
        public PatientExaminationCRUDWindow(Examination exam, int id)
        {
            PatientId = id;
            InitializeComponent();
            InitilizeData(exam);
        }

        public void InitilizeData(Examination exam)
        {
            if (exam != null)
            {
                titleCRUD.Title = "Update examination";
                createButton.IsEnabled = false;
                updateButton.IsEnabled = true;
                idTxtBox.Text = exam.Id.ToString();
                idTxtBox.IsEnabled = false;
                if (exam.DoctorId == 1)
                {
                    doc1.IsChecked = true;
                }
                else if (exam.DoctorId == 2)
                {
                    doc2.IsChecked = true;
                }
                else if (exam.DoctorId == 3)
                {
                    doc3.IsChecked = true;
                }
                if (exam.IsOperation)
                {
                    trueRbtn.IsChecked = true;
                }
                else
                {
                    falseRbtn.IsChecked = true;
                }
                DateTime dt = Convert.ToDateTime(exam.TimeSlot.Date);
                pickDate.SelectedDate = dt;
                timeTxtBox.Text = exam.TimeSlot.StartTime.ToString();
                durationTxtBox.Text = exam.TimeSlot.Duration.ToString();
            }
            else
            {
                titleCRUD.Title = "Create examination";
                updateButton.IsEnabled = false;
                createButton.IsEnabled = true;
            }
        }
        private void TrueRbtnChecked(object sender, RoutedEventArgs e)
        {
            durationTxtBox.IsEnabled = true;
        }

        private void FalseRbtnChecked(object sender, RoutedEventArgs e)
        {
            durationTxtBox.IsEnabled = false;
            durationTxtBox.Text = "00:15:00";
        }
        private void Doc1Checked(object sender, RoutedEventArgs e)
        {
            DoctorId = 1;
        }

        private void Doc2Checked(object sender, RoutedEventArgs e)
        {
            DoctorId = 2;
        }

        private void Doc3Checked(object sender, RoutedEventArgs e)
        {
            DoctorId = 3;
        }
        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            if (idTxtBox.Text == "" || (doc1.IsChecked != true && doc2.IsChecked != true && doc3.IsChecked != true)
                || (trueRbtn.IsChecked != true && falseRbtn.IsChecked != true)
                || pickDate.SelectedDate == null || timeTxtBox.Text == "" || durationTxtBox.Text == "")
            {
                MessageBox.Show("Not everything is filled out!");
            }
            else
            {
                try
                {
                    string date = pickDate.SelectedDate.Value.Date.ToString("dd.MM.yyyy.");
                    TimeSlot timeslot = new TimeSlot(date, timeTxtBox.Text, durationTxtBox.Text);
                    bool ex;
                    if (trueRbtn.IsChecked == true)
                    {
                        ex = Examination.Create(int.Parse(idTxtBox.Text), true, DoctorId,
                        PatientId, timeslot);
                    }
                    else
                    {
                        ex = Examination.Create(int.Parse(idTxtBox.Text), false, DoctorId,
                        PatientId, timeslot);
                    }
                    if (ex != false)
                    {
                        MessageBox.Show("Examinaton created!", "Confirmation");
                        this.Close();
                    }
                }
                catch
                {
                    MessageBox.Show("Invalid input types", "Warning");
                }
            }
        }
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            if (idTxtBox.Text != "")
            {
                try
                {
                    if ((doc1.IsChecked != true && doc2.IsChecked != true && doc3.IsChecked != true)
                    || (trueRbtn.IsChecked != true && falseRbtn.IsChecked != true)
                    || pickDate.SelectedDate == null || timeTxtBox.Text == "" || durationTxtBox.Text == "")
                    {
                        MessageBox.Show("Not everything is filled out!", "Warning");
                    }
                    else
                    {
                        string date = pickDate.SelectedDate.Value.Date.ToString("dd.MM.yyyy.");
                        TimeSlot timeslot = new TimeSlot(date, timeTxtBox.Text, durationTxtBox.Text);
                        bool ex;
                        if (trueRbtn.IsChecked == true)
                        {
                            ex = Examination.Update(int.Parse(idTxtBox.Text), true, DoctorId,
                            PatientId, timeslot);
                        }
                        else
                        {
                            ex = Examination.Update(int.Parse(idTxtBox.Text), false, DoctorId,
                              PatientId, timeslot);
                        }
                        if (ex == true)
                        {
                            MessageBox.Show("Examinaton updated!", "Confirmation");
                            this.Close();
                        }

                    }
                }
                catch
                {
                    MessageBox.Show("Invalid input types", "Warning");
                }
            }
            else
            {
                MessageBox.Show("Enter the examination id!", "Warning");
            }
        }
    }
}
