using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseEquipmentManager.Classes
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ResponsiblePerson { get; set; } // Ответственный
        public ObservableCollection<Equipment> Equipment { get; set; }
    }
}
