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
            WarehousesPanel.Children.Clear();

            using (var context = new WarehouseDBEntities())
            {
                var warehouses = context.Warehouses
                    .OrderBy(w => w.Name)
                    .Select(w => new Warehouse
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Address = w.Address,
                        ResponsiblePerson = w.ResponsiblePerson
                    })
                    .ToList();

                foreach (var warehouse in warehouses)
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
                        Text = $"ID: {warehouse.Id}\n" +
                               $"Название: {warehouse.Name}\n" +
                               $"Адрес: {warehouse.Address}\n" +
                               $"Ответственный: {warehouse.ResponsiblePerson ?? "не назначен"}",
                        TextWrapping = TextWrapping.Wrap
                    };

                    // Добавляем обработчик клика
                    /*border.MouseLeftButtonDown += (sender, e) =>
                    {
                        ShowWarehouseDetails(warehouse.Id);
                    };*/ 

                    border.Child = textBlock;
                    WarehousesPanel.Children.Add(border);
                }
            }
        }

        // доп метод для показа деталей склада
        /*private void ShowWarehouseDetails(int warehouseId)
        {
            using (var context = new WarehouseDBEntities())
            {
                var warehouse = context.Warehouses
                    .FirstOrDefault(w => w.Id == warehouseId);

                if (warehouse != null)
                {
                    MessageBox.Show($"Детали склада:\n\n" +
                                  $"ID: {warehouse.Id}\n" +
                                  $"Название: {warehouse.Name}\n" +
                                  $"Адрес: {warehouse.Address}\n" +
                                  $"Ответственный: {warehouse.ResponsiblePerson ?? "не назначен"}",
                                  "Информация о складе");
                }
            }
        }*/
    }
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ResponsiblePerson { get; set; }
    }
}
