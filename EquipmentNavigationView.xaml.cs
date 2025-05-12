using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WarehouseEquipmentManager.Entity;

namespace WarehouseEquipmentManager
{
    /// <summary>
    /// Логика взаимодействия для EquipmentNavigationView.xaml
    /// </summary>
    public partial class EquipmentNavigationView : UserControl
    {
        public event Action<string, string, int?, int?, DateTime?, int?> SearchRequested;

        public EquipmentNavigationView()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBoxes ();
        }

        private void LoadComboBoxes()
        {
            cbType.ItemsSource = GetEquipmentTypes();
            cbStatus.ItemsSource = GetEquipmentStatuses();
            cbWarehouse.ItemsSource = GetWarehouses();

            // Настройка отображения для всех ComboBox
            foreach (var comboBox in new[] { cbType, cbStatus, cbWarehouse })
            {
                comboBox.DisplayMemberPath = "Name";
                comboBox.SelectedValuePath = "Id";
            }

            // Добавление пункта "Все" и выбор его по умолчанию
            AddAllItemAndSelect(cbType);
            AddAllItemAndSelect(cbStatus);
            AddAllItemAndSelect(cbWarehouse);
        }

        // Методы для загрузки данных
        private List<ComboBoxItem> GetEquipmentTypes()
        {
            using (var context = new WarehouseDBEntities())
            {
                return context.EquipmentTypes
                    .OrderBy(t => t.Name)
                    .Select(t => new ComboBoxItem
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList();
            }
        }

        private List<ComboBoxItem> GetEquipmentStatuses()
        {
            using (var context = new WarehouseDBEntities())
            {
                return context.EquipmentStatuses
                    .OrderBy(s => s.Name)
                    .Select(s => new ComboBoxItem
                    {
                        Id = s.Id,
                        Name = s.Name
                    })
                    .ToList();
            }
        }

        private List<ComboBoxItem> GetWarehouses()
        {
            using (var context = new WarehouseDBEntities())
            {
                return context.Warehouses
                    .OrderBy(w => w.Name)
                    .Select(w => new ComboBoxItem
                    {
                        Id = w.Id,
                        Name = w.Name
                    })
                    .ToList();
            }
        }

        // Вспомогательный метод для добавления пункта "Все"
        private void AddAllItemAndSelect(ComboBox comboBox)
        {
            var items = comboBox.ItemsSource as List<ComboBoxItem>;
            items?.Insert(0, new ComboBoxItem { Id = 0, Name = "Все" });   
            comboBox.SelectedIndex = 0;
        }

        // Класс для элементов ComboBox
        public class ComboBoxItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Получаем значения из полей
            string name = tbName.Text;
            string serial = tbSerialNumber.Text;
            int? typeId = cbType.SelectedValue as int?;
            int? statusId = cbStatus.SelectedValue as int?;
            DateTime? date = dpData.SelectedDate;
            int? warehouseId = cbWarehouse.SelectedValue as int?;

            // Вызываем событие с параметрами поиска
            SearchRequested?.Invoke(name, serial, typeId, statusId, date, warehouseId);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            // Сброс всех фильтров
            tbName.Text = string.Empty;
            tbSerialNumber.Text = string.Empty;
            cbType.SelectedIndex = 0;
            cbStatus.SelectedIndex = 0;
            dpData.SelectedDate = null;
            cbWarehouse.SelectedIndex = 0;
        }
    }
}
