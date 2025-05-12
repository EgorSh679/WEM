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
using WarehouseEquipmentManager.Entity;

namespace WarehouseEquipmentManager
{
    /// <summary>
    /// Логика взаимодействия для DeleteEquipmentWindow.xaml
    /// </summary>
    public partial class DeleteEquipmentWindow : Window
    {
        public DeleteEquipmentWindow()
        {
            InitializeComponent();
            LoadEquipment();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ldExit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
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
            // Создаем карточку оборудования
            var card = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 10),
                Background = Brushes.White,
                Padding = new Thickness(10),
                Tag = equipment.Id // Сохраняем ID оборудования в Tag
            };

            var stack = new StackPanel();

            // ID оборудования
            var idText = new TextBlock
            {
                Text = $"ID: {equipment.Id}",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Название оборудования
            var nameText = new TextBlock
            {
                Text = equipment.Name,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Серийный номер
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

            // Обработчик клика по карточке
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
                    // Удаляем связанные фотографии
                    var photos = context.EquipmentPhotos.Where(p => p.EquipmentId == equipmentId);
                    context.EquipmentPhotos.RemoveRange(photos);

                    // Удаляем само оборудование
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
