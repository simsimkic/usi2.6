using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZdravoCorp.HealthInstitution.Users.Patient;

namespace ZdravoCorp.HealthInstitution.Examinations
{
    public class Anamnesis
    {
        private int examinationId;
        private int patientId;
        private string symptoms;
        private string alergies;
        private string earlierIllnesses;
        private string diagnosis;

        public Anamnesis(int examinationId, int patientId, string symptoms, string alergies, string earlierIllnesses)
        {
            this.examinationId = examinationId; 
            this.patientId = patientId;
            this.symptoms = symptoms;
            this.alergies = alergies;
            this.earlierIllnesses = earlierIllnesses;
            this.diagnosis = "";
        }

        public int ExaminationId { get => examinationId; set => examinationId = value; }
        public int PatientId { get => patientId; set => patientId = value; }
        public string Symptoms { get => symptoms; set => symptoms = value; }
        public string Alergies { get => alergies; set => alergies = value; }
        public string EarlierIllnesses { get => earlierIllnesses; set => earlierIllnesses = value; }
        public string Diagnosis { get => diagnosis; set => diagnosis = value; }


        public static bool DoesAnamnesisExist(int examId)
        {
            Anamnesis[] anamneses = LoadAnamnesis();
            if (anamneses == null) return false;
            foreach (Anamnesis anamnesis in anamneses)
            {
                if (anamnesis.examinationId == examId)
                    return true;
            }
            return false;
        }
        public static Anamnesis Find(int examinationId)
        {
            Anamnesis[] anamneses = LoadAnamnesis();
            if (anamneses == null) { return null; }
            foreach (Anamnesis anamnesis in anamneses)
            {
                if (anamnesis.examinationId == examinationId)
                    return anamnesis;
            }
            return null;
        }
        public static Anamnesis[] LoadAnamnesis()
        {
            var jsontext = File.ReadAllText("../../../Data/Anamnesis.json");
            Anamnesis[] allAnamnesis = JsonConvert.DeserializeObject<Anamnesis[]>(jsontext)!;
            return allAnamnesis;
        }
        public static Anamnesis FindAnamnesis(int examId)
        {
            Anamnesis[] anamneses = LoadAnamnesis();
            foreach(Anamnesis anamnesis in anamneses)
            {
                if (anamnesis.examinationId == examId)
                {
                    return anamnesis;
                }
            }
            return null;
        }

        public static void SaveAnamnesis(Anamnesis[] anamnesis)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string anamnesisJson = System.Text.Json.JsonSerializer.Serialize(anamnesis, options);
            File.WriteAllText("../../../Data/Anamnesis.json", anamnesisJson);
        }
    }
}
