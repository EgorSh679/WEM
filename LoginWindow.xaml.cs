using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WarehouseEquipmentManager.Entity;

namespace WarehouseEquipmentManager
{
    public partial class LoginWindow : Window
    {
        public static class CurrentUser
        {
            public static int Id { get; set; }
            public static string Login { get; set; }
            public static int RoleId { get; set; }
            public static string FullName { get; set; }
        }

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

            try
            {
                string hashedPassword = HashPassword(password);

                using (var context = new WarehouseDBEntities())
                {
                    var user = context.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == hashedPassword);
                    if (user != null)
                    {
                        var mainWindow = new MainWindow();

                        CurrentUser.Id = user.Id;
                        CurrentUser.Login = user.Login;
                        CurrentUser.RoleId = user.RoleId;
                        CurrentUser.FullName = user.FullName;

                        mainWindow.Show();
                        this.Close();
                    }
                    else
                        MessageBox.Show("Неверные данные для входа");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}");
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void ldMinimize_MouseDown(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void ldExit_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] sourceBytePassw = Encoding.UTF8.GetBytes(password);
                byte[] hashSourceBytePassw = sha256Hash.ComputeHash(sourceBytePassw);
                string hashPassw = BitConverter.ToString(hashSourceBytePassw).Replace("-", string.Empty);
                return hashPassw;
            }
        }
    }
}