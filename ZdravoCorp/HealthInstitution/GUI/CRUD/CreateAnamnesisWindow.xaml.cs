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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;
using ZdravoCorp.HealthInstitution.Examinations;
using ZdravoCorp.HealthInstitution.Services.Register;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    /// <summary>
    /// Interaction logic for CreateAnamnesisWindow.xaml
    /// </summary>
    public partial class CreateAnamnesisWindow : Window
    {
        public int _examinationId;
        public int _patientId;
        public CreateAnamnesisWindow(int examinationId, int patientId)
        {
            _examinationId = examinationId;
            _patientId = patientId;
            InitializeComponent();
        }

        private void TryToCreateAnamnesis(object sender, EventArgs e)
        {

            string symptoms = symptomsBox.Text;
            string alergies = alergiesBox.Text;
            string earlierIllnesses = earlierIllnessesBox.Text;

            if (isInputValid(symptoms, alergies, earlierIllnesses))
            {

                Anamnesis newAnamnesis = new Anamnesis(_examinationId, _patientId, symptoms, alergies, earlierIllnesses);
                Anamnesis[] anamneses = Anamnesis.LoadAnamnesis();
                anamneses = AppendAnamnesis(newAnamnesis, anamneses);
                Anamnesis.SaveAnamnesis(anamneses);

                MessageBox.Show("Anamnesis is successfully created.");
                this.Close();
            }
        }

        bool isInputValid(string symptoms, string alergies, string earlierIllnesses)
        {
            if (symptoms == "" || alergies == "" || earlierIllnesses == "")
            {
                MessageBox.Show("All fields are required.");
                return false;
            }
            return true;
        }

        private static Anamnesis[] AppendAnamnesis(Anamnesis anamnesis, Anamnesis[] anamneses)
        {
            if (anamneses == null)
            {
                Anamnesis[] oneAnamenesis = new Anamnesis[1];
                oneAnamenesis[0] = anamnesis;
                return oneAnamenesis;
            }

            Anamnesis[] anamnesesPlusOne = new Anamnesis[anamneses.Length + 1];
            for (int i = 0; i < anamneses.Length; i++)
            {
                anamnesesPlusOne[i] = anamneses[i];
            }
            anamnesesPlusOne[anamnesesPlusOne.Length - 1] = anamnesis;
            return anamnesesPlusOne;
        }
    }
}
