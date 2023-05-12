using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EquipmentServicesNS;
using RoomServicesNS;
using EquipmentNS;
using System.Security.Policy;
using System.Net;
using static EquipmentNS.Equipment;
using System.Xml.Linq;

namespace EquipmentServicesNS
{
    public enum QuantityRange { None, OneToTen, MoreThanTen, NoneToTen, NoneOrTenPlus, OneToInfinity }

    internal class EquipmentStorageService
    {
        public static bool IsChanged = false;
        private static void Serialize(ref List<EquipmentStorageItem> items, string fileName)
        {
            dynamic text = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(fileName, text);
        }

        private static List<EquipmentStorageItem> Deserialize(string fileName)
        {
            var jsontext = File.ReadAllText(fileName);
            List<EquipmentStorageItem> items = JsonConvert.DeserializeObject<List<EquipmentStorageItem>>(jsontext)!; ;
            return items;
        }

        public static List<EquipmentStorageItem> GetAllItems(string fileName)
        {
            return Deserialize(fileName);
        }
        public static string GetEquipmentCategory(EquipmentStorageItem item)
        {
            return EquipmentServicesNS.EquipmentService.GetEquipmentCategoryByName(item.StoredEquipmentName).ToString();
        }
        public static bool IsEquipmentStatic(EquipmentStorageItem item)
        {
            if (EquipmentServicesNS.EquipmentService.IsEquipmentDinamicByName(item.StoredEquipmentName))
            {
                return false;
            }
            return true;
        }

        public static string GetEquipmentType(EquipmentStorageItem item)
        {
            if (EquipmentServicesNS.EquipmentService.IsEquipmentDinamicByName(item.StoredEquipmentName))
            {
                return "dynamic";
            }
            return "static";
        }

        public static RoomsNS.Room.Type GetRoomType(EquipmentStorageItem item)
        {
            return RoomServicesNS.RoomSevice.GetRoomTypeById(item.ContainingRoomId);
        }

        public static EquipmentStorageItem FindItem(string name, int roomId, ref List<EquipmentStorageItem> allItems)
        {
            name = EquipmentService.UndoFormat(name);
            foreach (EquipmentStorageItem item in allItems)
            {
                if ((item.StoredEquipmentName == name) && (item.ContainingRoomId == roomId))
                {
                    return item;
                }
            }
            return null;
        }

        public static void ChangeItemQuantityInRoom(string itemName, int roomId, int quantityChange)
        {
            itemName = EquipmentService.UndoFormat(itemName);
            string fileName = "../../../Data/EquipmentStorage/EquipmentStorage.json";
            List<EquipmentStorageItem> allItems = GetAllItems(fileName);
            foreach (EquipmentStorageItem item in allItems)
            {
                if ((item.StoredEquipmentName == itemName) && (item.ContainingRoomId == roomId))
                {
                    item.Quantity += quantityChange;
                    Serialize(ref allItems, fileName);
                    IsChanged = true;
                    return;
                }
            }
        }
    }
}