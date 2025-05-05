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
using WarehouseEquipmentManager.Classes;

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
            // Мок-данные (замените на загрузку из БД)
            var users = new System.Collections.Generic.List<User>
            {
                new User
                {
                    Id = 1,
                    Login = "Никола",
                    Role = UserRole.Admin,
                    FullName = "Николай Николаев Николаевич"
                },
                new User
                {
                    Id = 2,
                    Login = "Иван123",
                    Role = UserRole.Storekeeper,
                    FullName = "Иванов Иван Иванович"
                },
                new User
                {
                    Id = 3,
                    Login = "Гость",
                    Role = UserRole.Viewer,
                    FullName = "Петров Петр Петрович"
                }
            };

            // Очистка панели
            UsersPanel.Children.Clear();

            // Создание блоков для каждого пользователя
            foreach (var user in users)
            {
                var border = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(10),
                    Background = Brushes.White
                };

                var textBlock = new TextBlock
                {
                    Text = $"ID: {user.Id}\n" +
                           $"Логин: {user.Login}\n" +
                           $"Роль: {GetRoleName(user.Role)}\n" +
                           $"Полное имя: {user.FullName}",
                    TextWrapping = System.Windows.TextWrapping.Wrap
                };

                border.Child = textBlock;
                UsersPanel.Children.Add(border);
            }
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
