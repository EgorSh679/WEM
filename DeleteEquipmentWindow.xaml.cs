using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WarehouseEquipmentManager.Entity;

namespace WarehouseEquipmentManager
{
    public partial class DeleteEquipmentWindow : Window
    {
        public DeleteEquipmentWindow()
        {
            InitializeComponent();
            LoadEquipment();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void ldExit_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private void LoadEquipment()
        {
            try
            {
                using (var context = new WarehouseDBEntities())
                {
                    var equipmentList = context.Equipment
                        .OrderBy(e => e.Name)
                        .ToList();

                    foreach (var equipment in equipmentList)
                    {
                        var card = CreateEquipmentCard(equipment);
                        EquipmentPanel.Children.Add(card);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки оборудования: {ex.Message}");
            }
        }

        private Border CreateEquipmentCard(Equipment equipment)
        {
            var card = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 10),
                Background = Brushes.White,
                Padding = new Thickness(10),
                Tag = equipment.Id
            };

            var stack = new StackPanel();

            var idText = new TextBlock
            {
                Text = $"ID: {equipment.Id}",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var nameText = new TextBlock
            {
                Text = equipment.Name,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var serialText = new TextBlock
            {
                Text = $"Серийный номер: {equipment.SerialNumber ?? "не указан"}",
                FontSize = 12,
                Foreground = Brushes.Gray
            };

            stack.Children.Add(idText);
            stack.Children.Add(nameText);
            stack.Children.Add(serialText);

            card.Child = stack;

            card.MouseLeftButtonDown += (sender, e) =>
            {
                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить оборудование:\n\n" +
                    $"ID: {equipment.Id}\n" +
                    $"Название: {equipment.Name}\n" +
                    $"Серийный номер: {equipment.SerialNumber}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteEquipment(equipment.Id);
                    EquipmentPanel.Children.Remove(card);
                }
            };

            return card;
        }

        private void DeleteEquipment(int equipmentId)
        {
            try
            {
                using (var context = new WarehouseDBEntities())
                {
                    var photos = context.EquipmentPhotos.Where(p => p.EquipmentId == equipmentId);
                    context.EquipmentPhotos.RemoveRange(photos);

                    var equipment = context.Equipment.FirstOrDefault(e => e.Id == equipmentId);
                    if (equipment != null)
                    {
                        context.Equipment.Remove(equipment);
                        context.SaveChanges();
                        MessageBox.Show("Оборудование успешно удалено!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении оборудования: {ex.Message}");
            }
        }
    }
}
