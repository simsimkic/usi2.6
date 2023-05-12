using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EquipmentServicesNS;
using Newtonsoft.Json;
using RoomsNS;

namespace RoomServicesNS
{
    public class RoomSevice
    {
        public static void Serialize(List<Room> items, string fileName)
        {
            dynamic text = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(fileName, text);
        }

        public static List<Room> Deserialize(string fileName)
        {
            var jsontext = File.ReadAllText(fileName);
            Console.WriteLine(jsontext);
            List<Room> items = JsonConvert.DeserializeObject<List<Room>>(jsontext)!; ;
            return items;
        }

        public static Room.Type GetRoomTypeById(int id)
        {
            string fileName = "../../../Data/Rooms/Rooms.json";
            List<Room> rooms = Deserialize(fileName);
            foreach (Room room in rooms)
            {
                if(room.Id == id)
                {
                    return room.RoomType;
                }
            }
            return RoomsNS.Room.Type.WaitingRoom;
        }

        public static string Format(RoomsNS.Room.Type type)
        {
            switch (type)
            {
                case Room.Type.WaitingRoom:
                    return "Waiting Room";
                case Room.Type.ConsultingRoom:
                    return "Consulting Room";
                case Room.Type.OperatingRoom:
                    return "Operating Room";
                case Room.Type.PatientCareRoom:
                    return "Patient Care Room";
                default:
                    return "Warehouse";
            }
        }

        public static List<int> GetAllRoomIds()
        {
            string fileName = "../../../Data/Rooms/Rooms.json";
            List<Room> rooms = Deserialize(fileName);
            List<int> roomIds = new List<int>();
            foreach (Room room in rooms)
            {
                roomIds.Add(room.Id);
            }
            return roomIds;
        }
    }
}
