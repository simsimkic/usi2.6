using EquipmentServicesNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentOrderNS
{
    internal class EquipmentOrderingService
    {
        public static bool PlaceOrder(string equipmentName, int quantity)
        {
            equipmentName = EquipmentService.UndoFormat(equipmentName);
            EquipmentOrder order = new EquipmentOrder(equipmentName, quantity);
            ScheduleOrder(order);
            return true;
        }

        private static List<EquipmentOrder> GetAllOrders(string fileName) 
        {
            var jsontext = File.ReadAllText(fileName);
            List<EquipmentOrder> allOrders = JsonConvert.DeserializeObject<List<EquipmentOrder>>(jsontext)!;
            return allOrders;
        }

        private static void SerializeOrders(ref List<EquipmentOrder> orders, string fileName)
        {
            dynamic text = JsonConvert.SerializeObject(orders, Formatting.Indented);
            File.WriteAllText(fileName, text);
        }

        private static void ScheduleOrder(EquipmentOrder order)
        {
            string fileName = "../../../Data/Orders/EquipmentOrders.json";
            List<EquipmentOrder> allOrders = GetAllOrders(fileName);
            allOrders.Add(order);
            SerializeOrders(ref allOrders, fileName);
        }

        public static void UpdateDeliveredOrders()
        {
            string fileName = "../../../Data/Orders/EquipmentOrders.json";
            List<EquipmentOrder> allOrders = GetAllOrders(fileName);
            List<EquipmentOrder> unprocessedOrders = GetAllOrders(fileName);
            const int WAREHOUSE_ID = 11;
            if (allOrders.Count == 0) 
            { 
                return;
            }
            foreach(EquipmentOrder order in allOrders)
            {
                if(order.DayOfDelivery <= DateTime.Now.Date)
                {
                    EquipmentStorageService.ChangeItemQuantityInRoom(order.EquipmentName, WAREHOUSE_ID, order.Quantity);
                    var toBeRemoved = unprocessedOrders.Find(x => x.EquipmentName == order.EquipmentName && x.DayOfDelivery == order.DayOfDelivery && x.Quantity == order.Quantity);
                    unprocessedOrders.Remove(toBeRemoved);
                }
            }
            SerializeOrders(ref unprocessedOrders, fileName);
        }
    }
}
