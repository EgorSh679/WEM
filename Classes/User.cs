using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseEquipmentManager.Classes
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public string FullName { get; set; }
    }

    public enum UserRole
    {
        Admin,         // Полные права
        Storekeeper,   // Кладовщик (редактирование)
        Viewer         // Только просмотр
    }
}
