using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WarehouseEquipmentManager.Entity;

namespace WarehouseEquipmentManager
{
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
                var users = context.Users
                    .Join(context.UserRoles,
                          user => user.RoleId,
                          role => role.Id,
                          (user, role) => new User
                          {
                              Id = user.Id,
                              Login = user.Login,
                              Role = (UserRole)user.RoleId,
                              RoleName = role.Name,
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
                               $"Роль: {user.RoleName}\n" +
                               $"Полное имя: {user.FullName}\n" +
                               $"Email: {user.Email ?? "не указан"}",
                        TextWrapping = TextWrapping.Wrap
                    };

                    border.Child = textBlock;
                    UsersPanel.Children.Add(border);
                }
            }
        }

        public enum UserRole
        {
            Admin = 1,
            Storekeeper = 2,
            Viewer = 3
        }

        public class User
        {
            public int Id { get; set; }
            public string Login { get; set; }
            public UserRole Role { get; set; }
            public string RoleName { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
        }

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
