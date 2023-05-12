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

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    public partial class PatientAnamnesisWindow : Window
    {
        public Anamnesis Anamnesis;
        public PatientAnamnesisWindow(Anamnesis anamnesis)
        {
            InitializeComponent();
            Anamnesis = anamnesis;
            InitializeAnamnesis(anamnesis);
        }

        public void InitializeAnamnesis(Anamnesis anamnesis)
        {
            anamnesisTitle.Content += " " + anamnesis.ExaminationId.ToString();
            symptomsBox.Text = anamnesis.Symptoms;
            symptomsBox.IsEnabled = false;
            alergiesBox.Text = anamnesis.Alergies;
            alergiesBox.IsEnabled = false;
            illnessesBox.Text = anamnesis.EarlierIllnesses;
            illnessesBox.IsEnabled = false;
            diagnosisBox.Text = anamnesis.Diagnosis;
            diagnosisBox.IsEnabled = false;
        }
    }
}
