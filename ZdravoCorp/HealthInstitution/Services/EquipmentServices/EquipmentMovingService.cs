using EquipmentOrderNS;
using EquipmentServicesNS;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentMovingNS
{
    public class EquipmentMovingService
    {
        public static bool RequestMoving(string equipmentName, int originRoomId, int destinationRoomId, int quantity, DateTime time)
        {
            equipmentName = EquipmentService.UndoFormat(equipmentName);
            EquipmentMovingRequest request = new EquipmentMovingRequest(equipmentName, originRoomId, destinationRoomId, quantity, time);
            if(EquipmentService.IsEquipmentDinamicByName(equipmentName))
            {
                MoveEquipment(request);
            } else
            {
                ScheduleRequest(request);
            }
            return true;
        }

        public static void MoveEquipment(EquipmentMovingRequest request)
        {
            EquipmentStorageService.ChangeItemQuantityInRoom(request.EquipmentName, request.OriginRoomId, request.Quantity * -1);
            EquipmentStorageService.ChangeItemQuantityInRoom(request.EquipmentName, request.DestinationRoomId, request.Quantity);
        }

        private static List<EquipmentMovingRequest> GetAllRequests(string fileName)
        {
            var jsontext = File.ReadAllText(fileName);
            List<EquipmentMovingRequest> allRequests = JsonConvert.DeserializeObject<List<EquipmentMovingRequest>>(jsontext)!;
            return allRequests;
        }

        private static void SerializeRequests(ref List<EquipmentMovingRequest> requests, string fileName)
        {
            string text = JsonConvert.SerializeObject(requests, Formatting.Indented);
            File.WriteAllText(fileName, text);
        }

        private static void ScheduleRequest(EquipmentMovingRequest request)
        {
            string fileName = "../../../Data/EquipmentStorage/EquipmentMovingSchedule.json";
            List<EquipmentMovingRequest> allRequests = GetAllRequests(fileName);
            allRequests.Add(request);
            SerializeRequests(ref allRequests, fileName);
        }

        public static bool IsUpdateNeeded()
        {
            string fileName = "../../../Data/EquipmentStorage/EquipmentMovingSchedule.json";
            List<EquipmentMovingRequest> allRequests = GetAllRequests(fileName);
            if (allRequests.Count == 0)
            {
                return false;
            }
            foreach (EquipmentMovingRequest request in allRequests)
            {
                DateTime requestMovingMoment = DateTime.ParseExact(request.MovingDateTime, "MM.dd.yyyy HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture); ;
                if (requestMovingMoment <= DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }

        public static void UpdateMoveRequests()
        {
            string fileName = "../../../Data/EquipmentStorage/EquipmentMovingSchedule.json";
            List<EquipmentMovingRequest> allRequests = GetAllRequests(fileName);
            if (allRequests.Count == 0)
            {
                return;
            }
            List<EquipmentMovingRequest> unprocessedRequests = GetAllRequests(fileName);
            foreach (EquipmentMovingRequest request in allRequests)
            {
                DateTime requestMovingMoment = DateTime.ParseExact(request.MovingDateTime, "MM.dd.yyyy HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture); ;
                if (requestMovingMoment <= DateTime.Now)
                {
                    EquipmentStorageService.ChangeItemQuantityInRoom(request.EquipmentName, request.OriginRoomId, request.Quantity*-1);
                    EquipmentStorageService.ChangeItemQuantityInRoom(request.EquipmentName, request.DestinationRoomId, request.Quantity);                    
                    var toBeRemoved = unprocessedRequests.Find(x => x.EquipmentName == request.EquipmentName && x.DestinationRoomId == request.DestinationRoomId && x.OriginRoomId == request.OriginRoomId && x.Quantity == request.Quantity);
                    unprocessedRequests.Remove(toBeRemoved);
                }
            }
            SerializeRequests(ref unprocessedRequests, fileName);
        }
    }
}
