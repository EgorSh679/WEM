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
using System.Windows.Shapes;
using System.Xml.Linq;
using WarehouseEquipmentManager.Entity;

namespace WarehouseEquipmentManager
{
    /// <summary>
    /// Логика взаимодействия для EquipmentDetailsView.xaml
    /// </summary>
    public partial class EquipmentDetailsView : UserControl
    {
        private EquipmentItem  _originalValues;
        public EquipmentDetailsView(EquipmentItem equipment)
        {
            InitializeComponent();
            _originalValues = equipment;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) 
        {
            // Восстанавливаем оригинальные значения
            txtName.Text = _originalValues.Name;
            txtSerialNumber.Text = _originalValues.SerialNumber;
            txtDescription.Text = _originalValues.Description;
            dpPurchaseDate.SelectedDate = _originalValues.PurchaseDate;

            cbType.SelectedValue = _originalValues.TypeId;
            cbStatus.SelectedValue = _originalValues.StatusId;
            cbWarehouse.SelectedValue = _originalValues.WarehouseId;
            cbResponsible.SelectedValue = _originalValues.CreatedBy;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            // Проверяем, есть ли изменения
            bool hasChanges =
                txtName.Text != _originalValues.Name ||
                txtSerialNumber.Text != _originalValues.SerialNumber ||
                txtDescription.Text != _originalValues.Description ||
                dpPurchaseDate.SelectedDate != _originalValues.PurchaseDate ||
                (int?)cbType.SelectedValue != _originalValues.TypeId ||
                (int?)cbStatus.SelectedValue != _originalValues.StatusId ||
                (int?)cbWarehouse.SelectedValue != _originalValues.WarehouseId ||
                (int?)cbResponsible.SelectedValue != _originalValues.CreatedBy;

            MessageBox.Show($"{txtDescription.Text} != {_originalValues.Description}");

            if (!hasChanges)
            {
                MessageBox.Show("Изменений не обнаружено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Создаем объект для обновления
            var updatedEquipment = new EquipmentItem
            {
                Id = _originalValues.Id,
                Name = txtName.Text,
                SerialNumber = txtSerialNumber.Text,
                Description = txtDescription.Text,
                PurchaseDate = dpPurchaseDate.SelectedDate ?? _originalValues.PurchaseDate, // Обработка null для DateTime
                TypeId = cbType.SelectedValue != null ? Convert.ToInt32(cbType.SelectedValue) : _originalValues.TypeId,
                StatusId = cbStatus.SelectedValue != null ? Convert.ToInt32(cbStatus.SelectedValue) : _originalValues.StatusId,
                WarehouseId = cbWarehouse.SelectedValue != null ? Convert.ToInt32(cbWarehouse.SelectedValue) : _originalValues.WarehouseId,
                CreatedBy = cbResponsible.SelectedValue != null ? Convert.ToInt32(cbResponsible.SelectedValue) : _originalValues.CreatedBy
            };

            try
            {
                using (var context = new WarehouseDBEntities())
                {
                    // Находим запись в базе
                    var equipmentInDb = context.Equipment.FirstOrDefault(eq => eq.Id == _originalValues.Id);

                    if (equipmentInDb != null)
                    {
                        // Обновляем поля
                        equipmentInDb.Name = updatedEquipment.Name;
                        equipmentInDb.SerialNumber = updatedEquipment.SerialNumber;
                        equipmentInDb.Description = updatedEquipment.Description;
                        equipmentInDb.PurchaseDate = updatedEquipment.PurchaseDate;
                        equipmentInDb.TypeId = updatedEquipment.TypeId;
                        equipmentInDb.StatusId = updatedEquipment.StatusId;
                        equipmentInDb.WarehouseId = updatedEquipment.WarehouseId;
                        equipmentInDb.CreatedBy = updatedEquipment.CreatedBy;

                        // Сохраняем изменения
                        context.SaveChanges();

                        MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Обновляем оригинальные значения
                        _originalValues = updatedEquipment;
                    }
                    else
                    {
                        MessageBox.Show("Оборудование не найдено в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

    public class EquipmentItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Description { get; set; }

        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public int WarehouseId { get; set; }
        public int? CreatedBy { get; set; }

        public string TypeName { get; set; }
        public string StatusName { get; set; }
        public string WarehouseName { get; set; }
        public string ImagePath { get; set; }
    }
}
