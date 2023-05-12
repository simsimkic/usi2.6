using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentOrderNS
{
    internal class EquipmentOrder
    {
        public enum OrderStatus { Created, Completed };

        public OrderStatus Status { get; set; }

        public DateTime DayOfDelivery { get; set; }

        public string EquipmentName;

        public int Quantity;

        public EquipmentOrder(string name, int quantity) 
        {
            Status = OrderStatus.Created;
            DayOfDelivery = DateTime.Now.Date.AddDays(1);
            EquipmentName = name;
            Quantity = quantity;
        }

    }
}
