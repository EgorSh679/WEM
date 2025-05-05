using System;
using System.Collections.Generic;
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

namespace WarehouseEquipmentManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowEquipmentView(0);
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

        private void EquipmentButton_Click(object sender, RoutedEventArgs e) => ShowEquipmentView(0);
        private void WarehousesButton_Click(object sender, RoutedEventArgs e) => ShowEquipmentView(1);
        private void UsersButton_Click(object sender, RoutedEventArgs e) => ShowEquipmentView(2);
        private void ShowEquipmentView(int key)
        {
            switch (key)
            {
                case 0:
                    contentControl.Content = new EquipmentNavigationView();
                    break;
                case 1:
                    contentControl.Content = new WarehouseNavigationView();
                    break;
                case 2:
                    contentControl.Content = new UserNavigationView();
                    break;
            }
        }
    }
}