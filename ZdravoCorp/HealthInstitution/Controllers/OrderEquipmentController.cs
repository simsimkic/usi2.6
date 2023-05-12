using EquipmentServicesNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ZdravoCorp.HealthInstitution.GUI;

namespace ZdravoCorp.HealthInstitution.Controllers
{
    internal class OrderEquipmentController
    {
        public static TableRow MakeRowForEquipmentDisplay(KeyValuePair<string, int> item)
        {
            TableRow row = new TableRow();

            row.FontSize = 15;

            TableCell cell = new TableCell();
            cell.Blocks.Add(new Paragraph(new Run(EquipmentServicesNS.EquipmentService.Format(item.Key))));
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Blocks.Add(new Paragraph(new Run(item.Value.ToString())));
            row.Cells.Add(cell);

            if (item.Value <= 5)
            {
                Color rowAccentColor = Color.FromRgb(150, 137, 127);      //#96897f
                row.Background = new SolidColorBrush(rowAccentColor);
            }
            return row;
        }

        public static bool EvaluateOrder(string equipmentName, string quantityString)
        {
            int quantity = 0;
            if (int.TryParse(quantityString, out quantity))
            {
                if (quantity > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static void PlaceOrder(string equipmentName, string quantityString, TextBox quantityTextBox, ComboBox equipmentComboBox)
        {
            OrderEquipmentController.EvaluateOrder(equipmentName, quantityString);
            int quantity = 0;
            if (int.TryParse(quantityString, out quantity))
            {
                EquipmentOrderNS.EquipmentOrderingService.PlaceOrder(equipmentName, quantity);
                string orderConfirmationString = EquipmentServicesNS.EquipmentService.Format(equipmentName) + " order is successfuly placed!";
                MessageBox.Show(orderConfirmationString);
                quantityTextBox.Text = "";
                equipmentComboBox.SelectedIndex = -1;
                return;
            }
            MessageBox.Show("Enter valid quantity!");
            quantityTextBox.Text = "";
        }
    }
}
