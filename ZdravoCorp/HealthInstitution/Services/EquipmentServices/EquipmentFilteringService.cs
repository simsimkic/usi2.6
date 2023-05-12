using EquipmentServicesNS;
using RoomsNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentServicesNS
{
    internal class EquipmentFilteringService
    { 
        public static List<EquipmentStorageItem> FilterEquipment(string fileName,
            bool byRoomType, List<RoomsNS.Room.Type> roomTypes,
            bool byEquipmentType, bool isStatic, bool isDynamic,
            bool byQuantity, QuantityRange quantity)
        {
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();
            if (byRoomType)
            {
                List<EquipmentStorageItem> allItems = EquipmentStorageService.GetAllItems(fileName);
                foreach (RoomsNS.Room.Type roomType in roomTypes)
                {
                    adequateItems.AddRange(FilterByRoomType(ref allItems, roomType));
                }
            }
            else
            {
                adequateItems = EquipmentStorageService.GetAllItems(fileName);
            }
            if (byEquipmentType)
            {
                adequateItems = FilterByEquipmentType(ref adequateItems, isStatic, isDynamic);

            }
            if (byQuantity)
            {
                adequateItems = FilterByQuantity(ref adequateItems, quantity);
            }
            return adequateItems;
        }

        public static List<EquipmentStorageItem> FilterByEquipmentType(ref List<EquipmentStorageItem> items, bool isStatic, bool isDynamic)
        {
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();
            foreach (EquipmentStorageItem item in items)
            {
                if (isStatic || isDynamic)
                {
                    if (EquipmentStorageService.IsEquipmentStatic(item) == isStatic)
                    {
                        adequateItems.Add(item);
                    }
                    else if (!EquipmentStorageService.IsEquipmentStatic(item) == isDynamic)
                    {
                        adequateItems.Add(item);
                    }
                }
            }
            return adequateItems;
        }

        public static List<EquipmentStorageItem> FilterByRoomType(ref List<EquipmentStorageItem> items, RoomsNS.Room.Type roomType)
        {
            List<EquipmentStorageItem> adequateItems = new List<EquipmentStorageItem>();
            foreach (EquipmentStorageItem item in items)
            {
                if (EquipmentStorageService.GetRoomType(item) == roomType)
                {
                    adequateItems.Add(item);
                }
            }
            return adequateItems;
        }


        public static List<EquipmentStorageItem> FilterByQuantity(ref List<EquipmentStorageItem> items, QuantityRange quantity)
        {
            List<EquipmentStorageItem> foundItems = new List<EquipmentStorageItem>();
            switch (quantity)
            {
                case QuantityRange.None:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if (item.Quantity == 0)
                        {
                            foundItems.Add(item);
                        }
                    }
                    break;
                case QuantityRange.OneToTen:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if ((item.Quantity > 0) && (item.Quantity <= 10))
                        {
                            foundItems.Add(item);
                        }
                    }
                    break;
                case QuantityRange.NoneToTen:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if (item.Quantity <= 10)
                        {
                            foundItems.Add(item);
                        }
                    }
                    break;
                case QuantityRange.MoreThanTen:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if (item.Quantity > 10)
                        {
                            foundItems.Add(item);
                        }
                    }
                    break;
                case QuantityRange.NoneOrTenPlus:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if ((item.Quantity > 10) || (item.Quantity == 0))
                        {
                            foundItems.Add(item);
                        }
                    }
                    break;
                case QuantityRange.OneToInfinity:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if (item.Quantity > 1)
                        {
                            foundItems.Add(item);
                        }
                    }
                    break;
            }
            return foundItems;
        }

        public static Dictionary<string, int> FindItemSumByFilteringRoomType(ref List<EquipmentStorageItem> items, RoomsNS.Room.Type roomType)
        {
            Dictionary<string, int> adequateItems = new Dictionary<string, int>();
            foreach (EquipmentStorageItem item in items)
            {
                if (EquipmentStorageService.GetRoomType(item) == roomType)
                {
                    if (adequateItems.ContainsKey(item.StoredEquipmentName))
                    {
                        adequateItems[item.StoredEquipmentName] += item.Quantity;
                    }
                    else
                    {
                        adequateItems.Add(item.StoredEquipmentName, item.Quantity);
                    }
                }
            }
            return adequateItems;
        }

        public static Dictionary<string, int> FindItemSumByFiteringQuantity(ref List<EquipmentStorageItem> items, QuantityRange quantity)
        {
            Dictionary<string, int> foundItems = new Dictionary<string, int>();
            switch (quantity)
            {
                case QuantityRange.None:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if (item.Quantity == 0)
                        {
                            if (foundItems.ContainsKey(item.StoredEquipmentName))
                            {
                                foundItems[item.StoredEquipmentName] += item.Quantity;
                            }
                            else
                            {
                                foundItems.Add(item.StoredEquipmentName, item.Quantity);
                            }
                        }
                    }
                    break;
                case QuantityRange.OneToTen:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if ((item.Quantity > 0) && (item.Quantity <= 10))
                        {
                            if (foundItems.ContainsKey(item.StoredEquipmentName))
                            {
                                foundItems[item.StoredEquipmentName] += item.Quantity;
                            }
                            else
                            {
                                foundItems.Add(item.StoredEquipmentName, item.Quantity);
                            }
                        }
                    }
                    break;
                case QuantityRange.NoneToTen:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if (item.Quantity <= 10)
                        {
                            if (foundItems.ContainsKey(item.StoredEquipmentName))
                            {
                                foundItems[item.StoredEquipmentName] += item.Quantity;
                            }
                            else
                            {
                                foundItems.Add(item.StoredEquipmentName, item.Quantity);
                            }
                        }
                    }
                    break;
                case QuantityRange.MoreThanTen:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if (item.Quantity > 10)
                        {
                            if (foundItems.ContainsKey(item.StoredEquipmentName))
                            {
                                foundItems[item.StoredEquipmentName] += item.Quantity;
                            }
                            else
                            {
                                foundItems.Add(item.StoredEquipmentName, item.Quantity);
                            }
                        }
                    }
                    break;
                case QuantityRange.NoneOrTenPlus:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if ((item.Quantity > 10) || (item.Quantity == 0))
                        {
                            if (foundItems.ContainsKey(item.StoredEquipmentName))
                            {
                                foundItems[item.StoredEquipmentName] += item.Quantity;
                            }
                            else
                            {
                                foundItems.Add(item.StoredEquipmentName, item.Quantity);
                            }
                        }
                    }
                    break;
                case QuantityRange.OneToInfinity:
                    foreach (EquipmentStorageItem item in items)
                    {
                        if (item.Quantity > 1)
                        {
                            if (foundItems.ContainsKey(item.StoredEquipmentName))
                            {
                                foundItems[item.StoredEquipmentName] += item.Quantity;
                            }
                            else
                            {
                                foundItems.Add(item.StoredEquipmentName, item.Quantity);
                            }
                        }
                    }
                    break;
            }
            return foundItems;
        }

        public static List<EquipmentStorageItem> GetLowQuantityItems()
        {
            string fileName = "../../../Data/EquipmentStorage/EquipmentStorage.json";
            List<EquipmentStorageItem> items = EquipmentStorageService.GetAllItems(fileName);
            foreach (EquipmentStorageItem item in items)
            {
                if(item.Quantity > 5)
                {
                    items.Remove(item);
                }
            }
            return items;
        }

        public static Dictionary<string, int> GetDynamicEquipmentSumItems()
        {
            string fileName = "../../../Data/EquipmentStorage/EquipmentStorage.json";
            List<EquipmentStorageItem> allItems = EquipmentStorageService.GetAllItems(fileName);
            Dictionary<string, int> adequateItems = new Dictionary<string, int>();
            foreach (EquipmentStorageItem item in allItems)
            {
                if (EquipmentService.IsEquipmentDinamicByName(item.StoredEquipmentName))
                {
                    if (adequateItems.ContainsKey(item.StoredEquipmentName))
                    {
                        adequateItems[item.StoredEquipmentName] += item.Quantity;
                    }
                    else
                    {
                        adequateItems.Add(item.StoredEquipmentName, item.Quantity);
                    }
                }
            }
            return adequateItems;
        }
    }
}
