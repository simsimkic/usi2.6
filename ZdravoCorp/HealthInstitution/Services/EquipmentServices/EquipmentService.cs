using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomsNS;

namespace EquipmentServicesNS
{
    public class EquipmentService
    {
        public static string[] OperationEquipmentOptions = {
            "Scissors",
            "Mask",
            "OperationTray",
            "Scalpel",
            "Tweezers",
            "RubberGlove",
            "OxygenMask"
        };

        public static string[] ExaminationEquipmentOptions = {
            "Stethoscope",
            "Plasters",
            "Thermometer",
            "Bandage",
            "Injection",
            "Gauze",
            "Syringe",
            "TongueDepressor"
        };

        public static string[] StationeryOptions = {
            "Pencil",
            "Pen",
            "Notepad",
            "PatientDatailForm",
            "PrescriptionPad"
        };

        public static string[] OperationFurnitureOptions = {
            "SurgicalTable",
            "AnesthesiaMachine",
            "PatientMonitor",
            "Trolley"
        };

        public static string[] RoomFurnitureOptions = {
            "Wheelchair",
            "HospitalBed",
            "BedsideScreen",
            "Locker",
            "Table",
            "Chair"
        };

        public static string Format(string name)
        {
            switch(name)
            {
                case "HospitalBed":
                    return "Hospital bed";
                case "BedsideScreen":
                    return "Bedside screen";
                case "PatientMonitor":
                    return "Patient monitor";
                case "AnesthesiaMachine":
                    return "Anesthesia machine";
                case "SurgicalTable":
                    return "Surgical table";
                case "PrescriptionPad":
                    return "Prescription pad";
                case "PatientDatailForm":
                    return "Patient detail form";
                case "TongueDepressor":
                    return "Tongue depressor";
                case "OxygenMask":
                    return "Oxygen mask";
                case "RubberGlove":
                    return "Rubber glove";
                case "OperationTray":
                    return "Operation tray";
                default:
                    return name;

            }
        }
        public static string UndoFormat(string name)
        {
            switch (name)
            {
                case "Hospital bed":
                    return "HospitalBed";
                case "Bedside screen":
                    return "BedsideScreen";
                case "Patient monitor":
                    return "PatientMonitor";
                case "Anesthesia machine":
                    return "AnesthesiaMachine";
                case "Surgical table":
                    return "SurgicalTable";
                case "Prescription pad":
                    return "PrescriptionPad";
                case "Patient detail form":
                    return "PatientDatailForm";
                case "Tongue depressor":
                    return "TongueDepressor";
                case "Oxygen mask":
                    return "OxygenMask";
                case "Rubber glove":
                    return "RubberGlove";
                case "Operation tray":
                    return "OperationTray";
                default:
                    return name;

            }
        }
        public static EquipmentNS.Equipment.EquipmentCategory GetEquipmentCategoryByName(string name)
        {
            name = UndoFormat(name);
            if (OperationEquipmentOptions.Contains(name))
            {
                return EquipmentNS.Equipment.EquipmentCategory.OperationEquipment;
            }
            else if (ExaminationEquipmentOptions.Contains(name))
            {
                return EquipmentNS.Equipment.EquipmentCategory.ExaminationEquipment;
            }
            else if (StationeryOptions.Contains(name))
            {
                return EquipmentNS.Equipment.EquipmentCategory.Stationery;
            }
            else if (OperationFurnitureOptions.Contains(name))
            {
                return EquipmentNS.Equipment.EquipmentCategory.OperationFurniture;
            }
            else
            {
                return EquipmentNS.Equipment.EquipmentCategory.RoomFurniture;
            }
        }

        public static bool IsEquipmentDinamicByName(string name)
        {
            name = UndoFormat(name);
            if (OperationEquipmentOptions.Contains(name))
            {
                return true;
            } else if (ExaminationEquipmentOptions.Contains(name))
            {
                return true;
            } else if (StationeryOptions.Contains(name))
            {
                return true;
            } else
            {
                return false;
            }
        }

        public static List<RoomsNS.Room.Type> GetValidRoomTypesForEquipment(string equipmentName)
        {
            equipmentName = UndoFormat(equipmentName);
            EquipmentNS.Equipment.EquipmentCategory equipmentCategory = EquipmentService.GetEquipmentCategoryByName(equipmentName);
            var validRooms = new List<RoomsNS.Room.Type>();
            //anything can be in the warehouse
            validRooms.Add(RoomsNS.Room.Type.Warehouse);
            switch(equipmentCategory)
            {
                case EquipmentNS.Equipment.EquipmentCategory.OperationEquipment:
                    validRooms.Add(RoomsNS.Room.Type.OperatingRoom);
                    break;
                case EquipmentNS.Equipment.EquipmentCategory.ExaminationEquipment:
                    validRooms.Add(RoomsNS.Room.Type.ConsultingRoom);
                    break;
                case EquipmentNS.Equipment.EquipmentCategory.Stationery:
                    validRooms.Add(RoomsNS.Room.Type.OperatingRoom);
                    validRooms.Add(RoomsNS.Room.Type.PatientCareRoom);
                    validRooms.Add(RoomsNS.Room.Type.ConsultingRoom);
                    validRooms.Add(RoomsNS.Room.Type.WaitingRoom);
                    break;
                case EquipmentNS.Equipment.EquipmentCategory.OperationFurniture:
                    validRooms.Add(RoomsNS.Room.Type.OperatingRoom);
                    break;
                case EquipmentNS.Equipment.EquipmentCategory.RoomFurniture:
                    validRooms.Add(RoomsNS.Room.Type.OperatingRoom);
                    validRooms.Add(RoomsNS.Room.Type.ConsultingRoom);
                    validRooms.Add(RoomsNS.Room.Type.PatientCareRoom);
                    validRooms.Add(RoomsNS.Room.Type.WaitingRoom);
                    break;
            }
            return validRooms;
        }

        public static List<string> GetAllEquipmentTypes()
        {
            List<string> allEquipmentTypes = new List<string>();
            foreach(string equipment in OperationEquipmentOptions)
            {
                allEquipmentTypes.Add(Format(equipment));
            }
            foreach (string equipment in OperationFurnitureOptions)
            {
                allEquipmentTypes.Add(Format(equipment));
            }
            foreach (string equipment in ExaminationEquipmentOptions)
            {
                allEquipmentTypes.Add(Format(equipment));
            }
            foreach (string equipment in StationeryOptions)
            {
                allEquipmentTypes.Add(Format(equipment));
            }
            foreach (string equipment in RoomFurnitureOptions)
            {
                allEquipmentTypes.Add(Format(equipment));
            }
            return allEquipmentTypes;
        }
    }
}
