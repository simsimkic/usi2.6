using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZdravoCorp.HealthInstitution.Users
{
    public class Profile
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public enum Type { doctor, nurse, patient, manager };

        public Type role { get; set; }
        public Profile(string username, string password, Type role)
        {
            this.Username = username;
            this.Password = password;
            this.role = role;
        }
        public override string ToString()
        {
            return "User: [ username: " + Username + ", password: " + Password + ", role: " + role + "]";
        }

        public static Profile[] LoadProfiles()
        {
            var profilesJson = File.ReadAllText("../../../Data/Login/LoginData.json");
            Profile[] allProfiles = JsonConvert.DeserializeObject<Profile[]>(profilesJson)!;
            return allProfiles;
        }

        public static void SaveProfiles(Profile[] profiles)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string profilesJosn = System.Text.Json.JsonSerializer.Serialize(profiles, options);
            File.WriteAllText("../../../Data/Login/LoginData.json", profilesJosn);
        }
    }
}
