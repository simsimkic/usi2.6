using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentMovingNS
{
    public class EquipmentMovingRequest
    {
        public string EquipmentName { get; set; }
        public int OriginRoomId { get; set; }
        public int DestinationRoomId { get; set; }
        public int Quantity { get; set; }
        public string MovingDateTime { get; set; }

        public enum Status { Created, Completed }
        public Status RequestStatus { get; set; }

        public EquipmentMovingRequest(string name, int containingRoomId, int destinationRoomId, int quantity, DateTime movingMoment) 
        { 
            EquipmentName = name;
            OriginRoomId = containingRoomId;
            DestinationRoomId = destinationRoomId;
            Quantity = quantity;
            MovingDateTime = movingMoment.ToString("MM/dd/yyyy HH:mm");
            RequestStatus = Status.Created;
        }
    }
}
