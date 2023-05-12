using EquipmentServicesNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ZdravoCorp.HealthInstitution.Controllers
{
    public class EquipmentGuiUtils
    {
        public static TableRowGroup GetRowGroup(Table table)
        {
            TableRowGroup rowGroup;
            try
            {
                rowGroup = table.RowGroups[1];
            }
            catch
            {
                table.RowGroups.Add(new TableRowGroup());
                rowGroup = table.RowGroups[1];
            }
            return rowGroup;
        }

        public static void SetEquipmentComboBox(ComboBox comboBox, ref List<string> equipment, bool justDynamic)
        {
            foreach (string equipmentType in equipment)
            {
                if (justDynamic)
                {
                    if (EquipmentService.IsEquipmentDinamicByName(equipmentType))
                    {
                        comboBox.Items.Add(equipmentType);
                    }
                }
                else
                {
                    comboBox.Items.Add(equipmentType);
                }
            }
        }
    }
}
