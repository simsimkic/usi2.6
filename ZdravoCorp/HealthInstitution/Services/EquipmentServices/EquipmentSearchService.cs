using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentServicesNS
{
    internal class EquipmentSearchService
    {
        public static List<EquipmentStorageItem> SearchEquipment(string characters, ref List<EquipmentStorageItem> allItems)
        {
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();
            adequateItems.AddRange(FindByNameSubstring(characters, ref allItems));
            adequateItems.AddRange(FindByEquipmentTypeSubstring(characters, ref allItems));
            adequateItems.AddRange(FindByEquipmentCategorySubstring(characters, ref allItems));
            adequateItems.AddRange(FindByRoomTypeSubstring(characters, ref allItems));
            adequateItems = adequateItems.Distinct().ToList();
            return adequateItems;
        }

        private static List<EquipmentStorageItem> FindByNameSubstring(string characters, ref List<EquipmentStorageItem> allItems)
        {
            characters = characters.ToLower();
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();
            foreach (EquipmentStorageItem item in allItems)
            {
                string equipmentName = item.StoredEquipmentName.ToLower();
                if (equipmentName.Contains(characters, StringComparison.CurrentCultureIgnoreCase))
                {
                    adequateItems.Add(item);
                }
            }
            return adequateItems;
        }

        private static List<EquipmentStorageItem> FindByEquipmentTypeSubstring(string characters, ref List<EquipmentStorageItem> allItems)
        {
            characters = characters.ToLower();
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();
            foreach (EquipmentStorageItem item in allItems)
            {
                string equipmentType = EquipmentStorageService.GetEquipmentType(item).ToLower();
                if (equipmentType.Contains(characters, StringComparison.CurrentCultureIgnoreCase))
                {
                    adequateItems.Add(item);
                }
            }
            return adequateItems;
        }


        private static List<EquipmentStorageItem> FindByEquipmentCategorySubstring(string characters, ref List<EquipmentStorageItem> allItems)
        {
            characters = characters.ToLower();
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();
            foreach (EquipmentStorageItem item in allItems)
            {
                string equipmentCategory = EquipmentStorageService.GetEquipmentCategory(item).ToLower();
                if (equipmentCategory.Contains(characters, StringComparison.CurrentCultureIgnoreCase))
                {
                    adequateItems.Add(item);
                }
            }
            return adequateItems;
        }

        private static List<EquipmentStorageItem> FindByRoomTypeSubstring(string characters, ref List<EquipmentStorageItem> allItems)
        {
            characters = characters.ToLower();
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();
            foreach (EquipmentStorageItem item in allItems)
            {
                string roomType = EquipmentStorageService.GetRoomType(item).ToString().ToLower();
                if (roomType.Contains(characters, StringComparison.CurrentCultureIgnoreCase))
                {
                    adequateItems.Add(item);
                }
            }
            return adequateItems;
        }

    }
}
