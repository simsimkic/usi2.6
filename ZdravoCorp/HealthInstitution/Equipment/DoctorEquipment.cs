using EquipmentServicesNS;
using RoomsNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZdravoCorp.HealthInstitution.Equipment
{
    internal class DoctorEquipment
    {
        public static void EnterEquipment(string itemName,int quantity,int roomId)
        {
            List<EquipmentStorageItem> items = EquipmentStorageService.GetAllItems("../../../Data/EquipmentStorage/EquipmentStorage.json");
            EquipmentStorageItem item = EquipmentStorageService.FindItem(itemName, roomId, ref (items));
            if (item.Quantity >= quantity)
            {
                EquipmentStorageService.ChangeItemQuantityInRoom(itemName,roomId,-quantity);
                MessageBox.Show("Entered used equipment");
            }
            else
            {
                MessageBox.Show("The quantity is too big", "Warning");
            }
        }

        internal static List<EquipmentStorageItem> GetRoomEquipment(int roomId)
        {
            List<EquipmentStorageItem> roomEquipment=new List<EquipmentStorageItem>();

            List<EquipmentStorageItem> items = EquipmentStorageService.GetAllItems("../../../Data/EquipmentStorage/EquipmentStorage.json");
            foreach (EquipmentStorageItem item in items)
            {
                if (item.ContainingRoomId == roomId && item.Quantity>0)
                {
                    roomEquipment.Add(item);
                }
            }
            return roomEquipment;
        }
    }
}
