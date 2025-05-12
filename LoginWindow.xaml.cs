using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        // txtFullName txtPassword txtLogin
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;
            //string fullName = txtFullName.Text.Trim();

            // Проверка заполнения полей
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            //if (string.IsNullOrEmpty(fullName))
            //{
            //    MessageBox.Show("Введите ФИО");
            //    return;
            //}

            try
            {
                // Хешируем введенный пароль для сравнения с базой
                string hashedPassword = HashPassword(password);

                using (var context = new WarehouseDBEntities())
                {
                    // Ищем пользователя с совпадающим логином, паролем и ФИО
                    var user = context.Users
                        .FirstOrDefault(u => u.Login == login &&
                                            u.PasswordHash == hashedPassword); // &&
                                            //u.FullName == fullName);

                    if (user != null)
                    {
                        // Авторизация успешна
                        var mainWindow = new MainWindow();

                        // Сохраняем информацию о текущем пользователе (можно через статический класс или свойства) 
                        CurrentUser.Id = user.Id;
                        CurrentUser.Login = user.Login;
                        CurrentUser.RoleId = user.RoleId;
                        CurrentUser.FullName = user.FullName;

                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный данные для входа");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}");
            }
        }

        // Класс для хранения данных текущего пользователя
        public static class CurrentUser
        {
            public static int Id { get; set; }
            public static string Login { get; set; }
            public static int RoleId { get; set; }
            public static string FullName { get; set; }
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ldExit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        private string HashPassword(string password) // password принимает методом в виде аргумента
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] sourceBytePassw = Encoding.UTF8.GetBytes(password);
                byte[] hashSourceBytePassw = sha256Hash.ComputeHash(sourceBytePassw);
                string hashPassw = BitConverter.ToString(hashSourceBytePassw).Replace("-", string.Empty);
                return hashPassw; // возвращаемое методом строковое значение
            }
        }
    }
}