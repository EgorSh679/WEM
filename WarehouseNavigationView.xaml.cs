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
    /// Логика взаимодействия для WarehouseNavigationView.xaml
    /// </summary>
    public partial class WarehouseNavigationView : UserControl
    {
        public WarehouseNavigationView()
        {
            InitializeComponent();
            LoadWarehouses();
        }

        private void LoadWarehouses()
        {
            // Мок-данные (замените на загрузку из БД)
            var warehouses = new ObservableCollection<Warehouse>
            {
                new Warehouse
                {
                    Id = 1,
                    Name = "Склад первого отделения",
                    Address = "г. Новосибирск, ул. Ленина, 1",
                    ResponsiblePerson = "Машуков А.Г."
                },
                new Warehouse
                {
                    Id = 2,
                    Name = "Центральный склад",
                    Address = "г. Новосибирск, пр. Карла Маркса, 10",
                    ResponsiblePerson = null
                }
            };

            // Очистка панели
            WarehousesPanel.Children.Clear();

            // Создание блоков для каждого склада
            foreach (var warehouse in warehouses)
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
                    Text = $"ID: {warehouse.Id}\n" +
                           $"Название: {warehouse.Name}\n" +
                           $"Адрес: {warehouse.Address}\n" +
                           $"Ответственный: {warehouse.ResponsiblePerson ?? "не назначен"}",
                    TextWrapping = System.Windows.TextWrapping.Wrap
                };

                border.Child = textBlock;
                WarehousesPanel.Children.Add(border);
            }
        }
    }
}
