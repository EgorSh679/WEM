using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WarehouseEquipmentManager.Entity;
using System.IO;

namespace WarehouseEquipmentManager
{
    public partial class MainWindow : Window
    {
        private static readonly string ProjectRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        public MainWindow()
        {
            InitializeComponent();
            ShowEquipmentView(0);
            OnEquipmentSearchRequested(null, null, null, null, null, null);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void ldMinimize_MouseDown(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void ldMaximize_MouseDown(object sender, MouseButtonEventArgs e)
            => WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        private void ldExit_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private void EquipmentButton_Click(object sender, RoutedEventArgs e) => ShowEquipmentView(0);
        private void WarehousesButton_Click(object sender, RoutedEventArgs e) => ShowEquipmentView(1);
        private void UsersButton_Click(object sender, RoutedEventArgs e) => ShowEquipmentView(2);

        private void ShowEquipmentView(int key)
        {
            switch (key)
            {
                case 0:
                    var navigationView = new EquipmentNavigationView();
                    navigationView.SearchRequested += OnEquipmentSearchRequested;
                    contentControl.Content = navigationView;
                    break;
                case 1:
                    contentControl.Content = new WarehouseNavigationView();
                    break;
                case 2:
                    contentControl.Content = new UserNavigationView();
                    break;
            }
        } 

        private void OnEquipmentSearchRequested(string name, string serial, int? typeId, int? statusId, DateTime? date, int? warehouseId)
        {
            EquipmentPanel.Children.Clear();

            List<EquipmentItem> equipmentList = LoadEquipmentFromDatabase(name, serial, typeId, statusId, date, warehouseId);
 
            ResultsCountText.Content = $"Найдено: {equipmentList.Count}";

            foreach (var item in equipmentList)
            {
                var card = CreateEquipmentCard(item);
                EquipmentPanel.Children.Add(card);
            }
        }

        private void AddNewEquipment_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEquipmentWindow();
            addWindow.Owner = this;
            addWindow.ShowDialog();
            OnEquipmentSearchRequested(null, null, null, null, null, null);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа 'Учет оборудования' была создана в результате производственной практики в МВД\n\n" +
                           "Разработчик: Шатровой Е.С. и Назаров А.Р.\n" +
                           "Версия: 1.0\n" +
                           "© 2025 Все права защищены",
                           "О программе");
        }

        private void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new DeleteEquipmentWindow();
            addWindow.Owner = this;
            addWindow.ShowDialog();
            OnEquipmentSearchRequested(null, null, null, null, null, null);
        }

        private void UpdateEquipment_Click(object sender, RoutedEventArgs e)
            => OnEquipmentSearchRequested(null, null, null, null, null, null);

        private List<EquipmentItem> LoadEquipmentFromDatabase(string name, string serial, int? typeId, int? statusId, DateTime? date, int? warehouseId)
        {
            using (var context = new WarehouseDBEntities())
            {
                var query = context.Equipment.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(e => e.Name.Contains(name));

                if (!string.IsNullOrEmpty(serial))
                    query = query.Where(e => e.SerialNumber.Contains(serial));

                if (typeId.HasValue && typeId.Value > 0)
                    query = query.Where(e => e.TypeId == typeId.Value);

                if (statusId.HasValue && statusId.Value > 0)
                    query = query.Where(e => e.StatusId == statusId.Value);

                if (date.HasValue)
                    query = query.Where(e => e.PurchaseDate == date.Value);

                if (warehouseId.HasValue && warehouseId.Value > 0)
                    query = query.Where(e => e.WarehouseId == warehouseId.Value);

                var result = query
                    .Join(context.EquipmentTypes,
                        e => e.TypeId,
                        t => t.Id,
                        (e, t) => new { e, TypeName = t.Name })
                    .Join(context.EquipmentStatuses,
                        temp => temp.e.StatusId,
                        s => s.Id,
                        (temp, s) => new { temp.e, temp.TypeName, StatusName = s.Name })
                    .Join(context.Warehouses,
                        temp => temp.e.WarehouseId,
                        w => w.Id,
                        (temp, w) => new EquipmentItem
                        {
                            Id = temp.e.Id,
                            Name = temp.e.Name,
                            SerialNumber = temp.e.SerialNumber,
                            TypeId = temp.e.TypeId,
                            TypeName = temp.TypeName,
                            StatusId = temp.e.StatusId,
                            StatusName = temp.StatusName,
                            PurchaseDate = temp.e.PurchaseDate,
                            WarehouseId = temp.e.WarehouseId,
                            WarehouseName = w.Name,
                            CreatedBy = temp.e.CreatedBy,
                            Description = temp.e.Description,
                            ImagePath = context.EquipmentPhotos
                                .Where(p => p.EquipmentId == temp.e.Id)
                                .Select(p => p.FilePath)
                                .FirstOrDefault()
                        })
                    .ToList();

                return result;
            }
        }

        private FrameworkElement CreateEquipmentCard(dynamic equipment)
        {
            var border = new Border
            {
                Width = 200,
                Height = 200,
                Margin = new Thickness(10),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5)
            };

            var stackPanel = new StackPanel();

            var image = new System.Windows.Controls.Image
            {
                Height = 120,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(10) 
            };

            if (!string.IsNullOrEmpty(equipment.ImagePath))
            {
                try { image.Source = new BitmapImage(new Uri($"{ProjectRoot}/WEM/images/{equipment.ImagePath}")); }
                catch { image.Source = new BitmapImage(new Uri($"{ProjectRoot}/WEM/images/image.png")); }
            }
            else
            {
                image.Source = new BitmapImage(new Uri($"{ProjectRoot}/WEM/images/image.png"));
            }

            var nameText = new TextBlock
            {
                Text = equipment.Name,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10, 0, 10, 0),
                TextWrapping = TextWrapping.Wrap
            };

            var serialText = new TextBlock
            {
                Text = $"Серийный номер: {equipment.SerialNumber}",
                Margin = new Thickness(10, 0, 10, 5),
                TextWrapping = TextWrapping.Wrap
            };

            stackPanel.Children.Add(image);
            stackPanel.Children.Add(nameText);
            stackPanel.Children.Add(serialText);
             
            border.Child = stackPanel;

            border.MouseLeftButtonDown += (sender, e) =>
            {
                OpenEquipmentDetails(equipment);
            };

            return border;
        }

        private void OpenEquipmentDetails(EquipmentItem equipment)
        {
            var detailsView = new EquipmentDetailsView(equipment);

            LoadDetailsComboBoxes(detailsView, equipment);

            detailsView.DataContext = equipment;

            contentControl2.Content = detailsView;
        }

        private void LoadDetailsComboBoxes(EquipmentDetailsView detailsView, EquipmentItem equipment)
        {
            using (var context = new WarehouseDBEntities())
            {
                detailsView.cbType.ItemsSource = context.EquipmentTypes.ToList();
                detailsView.cbStatus.ItemsSource = context.EquipmentStatuses.ToList();
                detailsView.cbWarehouse.ItemsSource = context.Warehouses.ToList();
                detailsView.cbResponsible.ItemsSource = context.Users.ToList();

                detailsView.cbType.SelectedValue = equipment.TypeId;
                detailsView.cbStatus.SelectedValue = equipment.StatusId;
                detailsView.cbWarehouse.SelectedValue = equipment.WarehouseId;
                detailsView.cbResponsible.SelectedValue = equipment.CreatedBy;
                detailsView.lblEquipmentId.Content = $"ID в базе данных: {equipment.Id}";
                detailsView.txtName.Text = equipment.Name;
                detailsView.txtSerialNumber.Text = equipment.SerialNumber;
                detailsView.dpPurchaseDate.SelectedDate = equipment.PurchaseDate;
                detailsView.txtDescription.Text = equipment.Description;

                var photo = context.EquipmentPhotos.FirstOrDefault(p => p.EquipmentId == equipment.Id);
                try
                {
                    if (photo != null)
                    {
                        detailsView.imgEquipment.Source = new BitmapImage(new Uri($"{ProjectRoot}/WEM/images/{equipment.ImagePath}"));
                    }
                }
                catch
                {
                    detailsView.imgEquipment.Source = new BitmapImage(new Uri($"{ProjectRoot}/WEM/images/image.png"));
                }
            }
        }
    }
}