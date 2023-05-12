using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomsNS;
using EquipmentNS;

namespace EquipmentServicesNS
{
    //a class that links equipment with room
    //contains information about quontity of said equipment in a specific room
    public class EquipmentStorageItem
    {
        public string StoredEquipmentName { get; set; }

        public int ContainingRoomId { get; set; }

        public int Quantity { get; set; }
        public EquipmentStorageItem(string equipmentName, int roomId, int quantity=0) 
        {
            this.StoredEquipmentName = equipmentName;
            this.ContainingRoomId = roomId;
            this.Quantity = quantity;
        }

    }
}
