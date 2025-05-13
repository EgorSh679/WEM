using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseEquipmentManager.Entity;

namespace WarehouseEquipmentManager
{
    public partial class EquipmentDetailsView : UserControl
    {
        private EquipmentItem  _originalValues;
        private readonly int _equipmentId;


        public EquipmentDetailsView(EquipmentItem equipment)
        {
            InitializeComponent();
            _originalValues = equipment;
            _equipmentId = equipment.Id;
            LoadEquipmentHistory();
        }

        private void LoadEquipmentHistory()
        {
            using (var context = new WarehouseDBEntities())
            {
                var history = context.AuditLogs
                    .Where(log => log.TableName == "Equipment" && log.RecordId == _equipmentId)
                    .OrderByDescending(log => log.ChangeDate)
                    .Select(log => new AuditLogViewModel
                    {
                        ChangeDate = log.ChangeDate ?? DateTime.Now,
                        ActionType = log.ActionType == "I" ? "Создание" : 
                                    log.ActionType == "U" ? "Изменение" : "Удаление",
                        OldData = log.OldData,
                    })
                    .ToList();
                dgAuditLogs.ItemsSource = history;
            }
        }

        // Класс для отображения истории
        private class AuditLogViewModel
        {
            public DateTime ChangeDate { get; set; }
            public string ActionType { get; set; }
            public string OldData { get; set; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) 
        {
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
            bool hasChanges =
                txtName.Text != _originalValues.Name ||
                txtSerialNumber.Text != _originalValues.SerialNumber ||
                txtDescription.Text != _originalValues.Description ||
                dpPurchaseDate.SelectedDate != _originalValues.PurchaseDate ||
                (int?)cbType.SelectedValue != _originalValues.TypeId ||
                (int?)cbStatus.SelectedValue != _originalValues.StatusId ||
                (int?)cbWarehouse.SelectedValue != _originalValues.WarehouseId ||
                (int?)cbResponsible.SelectedValue != _originalValues.CreatedBy;

            if (!hasChanges)
            {
                MessageBox.Show("Изменений не обнаружено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var updatedEquipment = new EquipmentItem
            {
                Id = _originalValues.Id,
                Name = txtName.Text,
                SerialNumber = txtSerialNumber.Text,
                Description = txtDescription.Text,
                PurchaseDate = dpPurchaseDate.SelectedDate ?? _originalValues.PurchaseDate,
                TypeId = cbType.SelectedValue != null ? Convert.ToInt32(cbType.SelectedValue) : _originalValues.TypeId,
                StatusId = cbStatus.SelectedValue != null ? Convert.ToInt32(cbStatus.SelectedValue) : _originalValues.StatusId,
                WarehouseId = cbWarehouse.SelectedValue != null ? Convert.ToInt32(cbWarehouse.SelectedValue) : _originalValues.WarehouseId,
                CreatedBy = cbResponsible.SelectedValue != null ? Convert.ToInt32(cbResponsible.SelectedValue) : _originalValues.CreatedBy
            };

            try
            {
                using (var context = new WarehouseDBEntities())
                {
                    var equipmentInDb = context.Equipment.FirstOrDefault(eq => eq.Id == _originalValues.Id);

                    if (equipmentInDb != null)
                    {
                        equipmentInDb.Name = updatedEquipment.Name;
                        equipmentInDb.SerialNumber = updatedEquipment.SerialNumber;
                        equipmentInDb.Description = updatedEquipment.Description;
                        equipmentInDb.PurchaseDate = updatedEquipment.PurchaseDate;
                        equipmentInDb.TypeId = updatedEquipment.TypeId;
                        equipmentInDb.StatusId = updatedEquipment.StatusId;
                        equipmentInDb.WarehouseId = updatedEquipment.WarehouseId;
                        equipmentInDb.CreatedBy = updatedEquipment.CreatedBy;

                        context.SaveChanges();

                        MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

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
