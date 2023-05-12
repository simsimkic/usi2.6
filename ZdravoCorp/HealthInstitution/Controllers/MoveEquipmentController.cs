using EquipmentMovingNS;
using EquipmentServicesNS;
using Newtonsoft.Json;
using RoomServicesNS;
using RoomsNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ZdravoCorp.HealthInstitution.Controllers
{
    internal class MoveEquipmentController
    {
        public static TableRow MakeRowForEquipmentDisplay(EquipmentStorageItem item, ref List<EquipmentMovingRequest> equipmentScheduledForMoving)
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
            cell.Blocks.Add(new Paragraph(new Run(item.Quantity.ToString())));
            row.Cells.Add(cell);

            int quantityForMove = 0;
            int quantityToCome = 0;
            foreach (EquipmentMovingRequest request in equipmentScheduledForMoving)
            {
                if (request.EquipmentName == item.StoredEquipmentName)
                {
                    if (item.ContainingRoomId == request.OriginRoomId)
                    {
                        quantityForMove += request.Quantity;
                    }
                    else if (item.ContainingRoomId == request.DestinationRoomId)
                    {
                        quantityToCome += request.Quantity;
                    }
                }
            }
            if (quantityForMove != 0 || quantityToCome != 0)
            {
                cell = new TableCell();
                cell.Blocks.Add(new Paragraph(new Run(quantityForMove.ToString())));
                row.Cells.Add(cell);
                cell = new TableCell();
                cell.Blocks.Add(new Paragraph(new Run(quantityToCome.ToString())));
                row.Cells.Add(cell);
            }
            else
            {
                cell = new TableCell();
                cell.Blocks.Add(new Paragraph(new Run("/")));
                row.Cells.Add(cell);
                cell = new TableCell();
                cell.Blocks.Add(new Paragraph(new Run("/")));
                row.Cells.Add(cell);
            }

            if (item.Quantity == 0)
            {
                Color rowAccentColor = Color.FromRgb(150, 137, 127);      //#96897f
                row.Background = new SolidColorBrush(rowAccentColor);
            }

            return row;
        }

        private static bool ValidateRooms(string equipmentName, int fromRoom, int toRoom)
        {
            if (fromRoom == toRoom)
            {
                MessageBox.Show("Moving has to happen between two rooms!");
                return false;
            }
            Room.Type fromRoomType = RoomSevice.GetRoomTypeById(fromRoom);
            Room.Type toRoomType = RoomSevice.GetRoomTypeById(toRoom);
            List<Room.Type> possibleRoomTypes = EquipmentService.GetValidRoomTypesForEquipment(equipmentName);
            if (!possibleRoomTypes.Contains(fromRoomType))
            {
                string message = "This equipment can not be in " + fromRoomType + "(room id " + fromRoom + ")!";
                MessageBox.Show(message);
                return false;
            }
            if (!possibleRoomTypes.Contains(toRoomType))
            {
                string message = "This equipment can not be in " + toRoomType + "(room id " + toRoom + ")!";
                MessageBox.Show(message);
                return false;
            }
            return true;
        }

        public static bool ValidateQuantity(ref int quantity, string equipmentName, string quantityString, TextBox equipmentQuantity, int fromRoom) 
        {
            if (int.TryParse(quantityString, out quantity))
            {
                string fileName = "../../../Data/EquipmentStorage/EquipmentStorage.json";
                List<EquipmentStorageItem> allItems = EquipmentStorageService.GetAllItems(fileName);
                EquipmentStorageItem chosenItem = EquipmentStorageService.FindItem(equipmentName, fromRoom, ref allItems);
                string jsontext = File.ReadAllText("../../../Data/EquipmentStorage/EquipmentMovingSchedule.json");
                List<EquipmentMovingRequest> equipmentScheduledForMoving = JsonConvert.DeserializeObject<List<EquipmentMovingRequest>>(jsontext)!;

                int quantityForMove = 0;
                int quantityToCome = 0;
                foreach (EquipmentMovingRequest request in equipmentScheduledForMoving)
                {
                    if (request.EquipmentName == chosenItem.StoredEquipmentName)
                    {
                        if (chosenItem.ContainingRoomId == request.OriginRoomId)
                        {
                            quantityForMove += request.Quantity;
                        }
                        else if (chosenItem.ContainingRoomId == request.DestinationRoomId)
                        {
                            quantityToCome += request.Quantity;
                        }
                    }
                }
                if (quantity < 1)
                {
                    MessageBox.Show("Quantity can not be negative!");
                    equipmentQuantity.Text = "";
                    return false;
                }
                if (quantity > chosenItem.Quantity - quantityForMove)
                {
                    MessageBox.Show("You cannot move more than what is avaliable for that room!");
                    equipmentQuantity.Text = "";
                    return false;
                }
                return true;
             }
            else
            {
                MessageBox.Show("Quantity must be a numeric value!");
                equipmentQuantity.Text = "";
                return false;
            }
        }

        public static bool ValidateTime(string equipmentName, string hoursAndMinutes, ref DateTime moveMoment)
        {
            string hourString;
            string minuteString;
            if (hoursAndMinutes != "hh:mm")
            {
                try
                {
                    hourString = hoursAndMinutes.Split(':')[0];
                    minuteString = hoursAndMinutes.Split(':')[1];
                    int hours = int.Parse(hourString);
                    int minutes = int.Parse(minuteString);
                    if ((hours > 23) || (hours < 0) || (minutes < 0) || (minutes > 59))
                    {
                        MessageBox.Show("Please input valid time!");
                        return false;
                    }
                    TimeSpan timeOfDay = new TimeSpan(hours, minutes, 0);
                    moveMoment = moveMoment.Date + timeOfDay;
                    if (moveMoment < DateTime.Now)
                    {
                        MessageBox.Show("Please input date and time in the future!");
                        return false;
                    }
                    return true;
                }
                catch
                {
                    MessageBox.Show("Please input valid time and date!");
                    return false ;
                }
            }
            else
            {
                if(!EquipmentService.IsEquipmentDinamicByName(equipmentName))
                {
                    MessageBox.Show("Please input time!");
                    return false;
                }
            }
            return true;
        }

        public static void MoveEquipment(string equipmentName, int fromRoom, int toRoom, string quantityString,
            string hoursAndMinutes, DateTime moveMoment, TextBox equipmentQuantity, ComboBox fromRoomCombo, ComboBox toRoomCombo, ComboBox movingSelectEquipmentCombo)
        {
            bool roomsValidated = ValidateRooms(equipmentName, fromRoom, toRoom);
            int quantity = 0;
            bool quantityValidated = ValidateQuantity(ref quantity, equipmentName, quantityString, equipmentQuantity, fromRoom);

            bool timeValidated = ValidateTime(equipmentName, hoursAndMinutes, ref moveMoment);
            if(!roomsValidated || !quantityValidated || !timeValidated)
            {
                return;
            }
            EquipmentMovingService.RequestMoving(equipmentName, fromRoom, toRoom, quantity, moveMoment);
            string moveConfirmationString;
            if (EquipmentService.IsEquipmentDinamicByName(equipmentName))
            {
                moveConfirmationString = EquipmentServicesNS.EquipmentService.Format(equipmentName) + " move is successful!";
            }
            else
            {
                moveConfirmationString = EquipmentServicesNS.EquipmentService.Format(equipmentName) + " move is scheduled!";
            }
            MessageBox.Show(moveConfirmationString);
            equipmentQuantity.Text = "";
            fromRoomCombo.SelectedIndex = -1;
            toRoomCombo.SelectedIndex = -1;
            movingSelectEquipmentCombo.SelectedIndex = -1;
        }
    }
}