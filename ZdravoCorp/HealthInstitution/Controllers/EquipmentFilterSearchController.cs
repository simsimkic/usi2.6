using EquipmentServicesNS;
using RoomsNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ZdravoCorp.HealthInstitution.Controllers
{
    internal class EquipmentFilterSearchController
    {
        public static TableRow MakeRowForEquipmentDisplay(EquipmentStorageItem item)
        {

            TableRow row = new TableRow();

            row.FontSize = 15;

            TableCell cell = new TableCell();
            cell.Blocks.Add(new Paragraph(new Run(EquipmentServicesNS.EquipmentService.Format(item.StoredEquipmentName))));
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Blocks.Add(new Paragraph(new Run(item.ContainingRoomId.ToString())));
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Blocks.Add(new Paragraph(new Run(RoomServicesNS.RoomSevice.Format(RoomServicesNS.RoomSevice.GetRoomTypeById(item.ContainingRoomId)))));
            row.Cells.Add(cell);

            cell = new TableCell();
            bool isDynamic = EquipmentServicesNS.EquipmentService.IsEquipmentDinamicByName(item.StoredEquipmentName);
            if (isDynamic)
            {
                cell.Blocks.Add(new Paragraph(new Run("Dynamic")));
            }
            else
            {
                cell.Blocks.Add(new Paragraph(new Run("Static")));
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Blocks.Add(new Paragraph(new Run(EquipmentServicesNS.EquipmentService.GetEquipmentCategoryByName(item.StoredEquipmentName).ToString())));
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Blocks.Add(new Paragraph(new Run(item.Quantity.ToString())));
            row.Cells.Add(cell);

            return row;
        }

        public static List<EquipmentStorageItem> ApplyFilters(bool warehouseChecked, bool waitingChecked, bool consultingChecked,
                    bool patientCareChecked, bool operatingChecked, bool staticChecked, bool dynamicChecked,
                    bool moreThanTenChecked, bool upToTenChecked, bool zeroChecked)
        {
            //filtering by room type
            bool byRoomType = !(warehouseChecked && waitingChecked && consultingChecked && patientCareChecked && operatingChecked);
            if (!warehouseChecked && !waitingChecked && !consultingChecked && !patientCareChecked && !operatingChecked)
            {
                byRoomType = false;
            }
            List<RoomsNS.Room.Type> roomTypes = new List<Room.Type>();
            if (warehouseChecked)
            {
                roomTypes.Add(Room.Type.Warehouse);
            }
            if (waitingChecked)
            {
                roomTypes.Add(Room.Type.WaitingRoom);
            }
            if (consultingChecked)
            {
                roomTypes.Add(Room.Type.ConsultingRoom);
            }
            if (patientCareChecked)
            {
                roomTypes.Add(Room.Type.PatientCareRoom);
            }
            if (operatingChecked)
            {
                roomTypes.Add(Room.Type.OperatingRoom);
            }

            //equipment filtering by equipment type
            bool byEquipmentType = !(staticChecked && dynamicChecked);
            if (!staticChecked && !dynamicChecked)
            {
                byEquipmentType = false;
            }

            //filtering by equipment quantity
            bool byQuantity = !(moreThanTenChecked && upToTenChecked && zeroChecked);
            if (!moreThanTenChecked && !upToTenChecked && !zeroChecked)
            {
                byQuantity = false;
            }
            QuantityRange quantity = QuantityRange.None;
            if (byQuantity)
            {
                if (zeroChecked && upToTenChecked)
                {
                    quantity = QuantityRange.OneToTen;
                }
                else if (zeroChecked && moreThanTenChecked)
                {
                    quantity = QuantityRange.NoneOrTenPlus;
                }
                else if (upToTenChecked && moreThanTenChecked)
                {
                    quantity = QuantityRange.OneToInfinity;
                }
                else if (moreThanTenChecked)
                {
                    quantity = QuantityRange.MoreThanTen;
                }
                else if (upToTenChecked)
                {
                    quantity = QuantityRange.OneToTen;
                }
            }

            bool allChecked = warehouseChecked && waitingChecked && consultingChecked &&
                patientCareChecked && operatingChecked && staticChecked && dynamicChecked &&
                moreThanTenChecked && upToTenChecked && zeroChecked;
            bool nothingChecked = !warehouseChecked && !waitingChecked && !consultingChecked &&
                !patientCareChecked && !operatingChecked && !staticChecked && !dynamicChecked &&
                !moreThanTenChecked && !upToTenChecked && !zeroChecked;
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();

            string fileName = "../../../Data/EquipmentStorage/EquipmentStorage.json";
            if (allChecked || nothingChecked)
            {
                return EquipmentStorageService.GetAllItems(fileName);
            }
            return EquipmentFilteringService.FilterEquipment(fileName,
            byRoomType, roomTypes, byEquipmentType, staticChecked, dynamicChecked, byQuantity, quantity);
        }
    }
}
