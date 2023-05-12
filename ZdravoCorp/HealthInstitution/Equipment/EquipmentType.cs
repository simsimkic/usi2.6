using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentNS
{
    internal class OperationEquipment : Equipment
    { 
        public OperationEquipment(string name)
        {
            this.Category = EquipmentCategory.OperationEquipment;
            this.IsDynamic = true;
            this.Name = name;
        }
    }

    internal class ExaminationEquipment : Equipment
    {
        public ExaminationEquipment()
        {
            this.Category = EquipmentCategory.ExaminationEquipment;
            this.IsDynamic = true;
        }
    }

    internal class Stationery : Equipment
    {
        public Stationery()
        {
            this.Category = EquipmentCategory.Stationery;
            this.IsDynamic = true;
        }
    }

    internal class OperationFurniture : Equipment
    {
        public OperationFurniture()
        {
            this.Category = EquipmentCategory.OperationFurniture;
            this.IsDynamic = false;
        }
    }

    internal class RoomFurniture : Equipment
    {
        public RoomFurniture()
        {
            this.Category = EquipmentCategory.RoomFurniture;
            this.IsDynamic = false;
        }
    }
}
