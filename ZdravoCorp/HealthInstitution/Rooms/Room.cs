using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomsNS
{
    public class Room
    {
        public int Id;
        public enum Type { OperatingRoom, PatientCareRoom, ConsultingRoom, WaitingRoom, Warehouse }

        public Type RoomType { get; set; }

        public int MaxPatientNumber { get; set; }

        public int CurrentPatientNumber { get; set; }
        public Room(Type type, int id) 
        {
            this.Id = id;
            this.RoomType = type;
            InitializeNumOfPatients(type);
        }

        public void InitializeNumOfPatients(Type type)
        {
            switch (type)
            {
                case Type.OperatingRoom:
                    this.MaxPatientNumber = 1;
                    break;
                case Type.PatientCareRoom:
                    this.MaxPatientNumber = 3;
                    break;
                case Type.ConsultingRoom:
                    this.MaxPatientNumber = 1;
                    break;
                case Type.WaitingRoom:
                    this.MaxPatientNumber = 100;
                    break;
                case Type.Warehouse:
                    this.MaxPatientNumber = 0;
                    break;
            }
            this.CurrentPatientNumber = 0;
        }
    }
}
