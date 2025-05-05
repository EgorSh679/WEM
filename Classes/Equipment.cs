using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseEquipmentManager.Classes
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; }          // Наименование
        public string SerialNumber { get; set; }  // Серийный номер
        public EquipmentType Type { get; set; }   // Тип
        public EquipmentStatus Status { get; set; }   // Статус
        public DateTime ReceiptDate { get; set; }   // Дата получения
        public string PhotoPath { get; set; }     // Путь к изображению
        public int WarehouseId { get; set; }      // Привязка к складу
        public Warehouse Warehouse { get; set; }
    }
}
namespace WarehouseEquipmentManager.Classes
{
    public enum EquipmentType { Production, Office, Special }
    public enum EquipmentStatus { InUse, InStock, Maintenance, WrittenOff }
}
