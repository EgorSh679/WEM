using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseEquipmentManager.Entity;

namespace WarehouseEquipmentManager
{
    /// <summary>
    /// Логика взаимодействия для UserNavigationView.xaml
    /// </summary>
    public partial class UserNavigationView : UserControl
    {
        public UserNavigationView()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void LoadUsers()
        {
            UsersPanel.Children.Clear();

            using (var context = new WarehouseDBEntities())
            {
                // Загрузка пользователей с JOIN к таблице ролей
                var users = context.Users
                    .Join(context.UserRoles,
                          user => user.RoleId,
                          role => role.Id,
                          (user, role) => new User
                          {
                              Id = user.Id,
                              Login = user.Login,
                              Role = (UserRole)user.RoleId,
                              RoleName = role.Name, // Добавляем название роли
                              FullName = user.FullName,
                              Email = user.Email
                          })
                    .OrderBy(u => u.FullName)
                    .ToList();

                foreach (var user in users)
                {
                    var border = new Border
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(0, 0, 0, 10),
                        Padding = new Thickness(10),
                        Background = Brushes.White,
                        Cursor = Cursors.Hand
                    };

                    var textBlock = new TextBlock
                    {
                        Text = $"ID: {user.Id}\n" +
                               $"Логин: {user.Login}\n" +
                               $"Роль: {user.RoleName}\n" + // Используем RoleName из JOIN
                               $"Полное имя: {user.FullName}\n" +
                               $"Email: {user.Email ?? "не указан"}",
                        TextWrapping = TextWrapping.Wrap
                    };

                    border.Child = textBlock;
                    UsersPanel.Children.Add(border);
                }
            }
        }

        // Enum для ролей пользователей
        public enum UserRole
        {
            Admin = 1,
            Storekeeper = 2,
            Viewer = 3
            // Добавьте другие роли при необходимости
        }

        // Класс User (должен быть объявлен в вашем проекте)
        public class User
        {
            public int Id { get; set; }
            public string Login { get; set; }
            public UserRole Role { get; set; }
            public string RoleName { get; set; } // Добавлено для хранения названия роли
            public string FullName { get; set; }
            public string Email { get; set; }
        }

        // Метод для получения названия роли
        private string GetRoleName(UserRole role)
        {
            switch (role)
            {
                case UserRole.Admin:
                    return "Администратор";
                case UserRole.Storekeeper:
                    return "Кладовщик";
                case UserRole.Viewer:
                    return "Наблюдатель";
                default:
                    return "Неизвестная роль";
            }
        }
    }
}
