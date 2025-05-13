using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WarehouseEquipmentManager.Entity;
using System.IO;

namespace WarehouseEquipmentManager
{
    public partial class AddEquipmentWindow : Window
    {
        private string _imagePath;
        private const string DefaultImagePath = "pack://application:,,,/images/image.png";

        public AddEquipmentWindow()
        {
            InitializeComponent();
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            using (var context = new WarehouseDBEntities())
            {
                cbType.ItemsSource = context.EquipmentTypes.ToList();
                cbStatus.ItemsSource = context.EquipmentStatuses.ToList();
                cbWarehouse.ItemsSource = context.Warehouses.ToList();
                cbResponsible.ItemsSource = context.Users.ToList();
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите изображение оборудования"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _imagePath = openFileDialog.FileName;
                imgEquipment.Source = new BitmapImage(new Uri(_imagePath));
            }
            else
            {
                _imagePath = null;
                imgEquipment.Source = new BitmapImage(new Uri(DefaultImagePath));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название оборудования");
                return;
            }

            try
            {
                using (var context = new WarehouseDBEntities())
                {
                    var newEquipment = new Equipment
                    {
                        Name = txtName.Text,
                        SerialNumber = txtSerialNumber.Text,
                        TypeId = (int)cbType.SelectedValue,
                        StatusId = (int)cbStatus.SelectedValue,
                        PurchaseDate = dpPurchaseDate.SelectedDate ?? DateTime.Now,
                        Description = txtDescription.Text,
                        WarehouseId = (int)cbWarehouse.SelectedValue,
                        CreatedBy = (int?)cbResponsible.SelectedValue,
                        CreatedDate = DateTime.Now
                    };

                    context.Equipment.Add(newEquipment);
                    context.SaveChanges();

                    string relativePath = "image.png";

                    if (!string.IsNullOrEmpty(_imagePath))
                    {
                        string imageFileName = System.IO.Path.GetFileName(_imagePath);
                        relativePath = imageFileName;

                        string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

                        if (!Directory.Exists(imagesDir))
                            Directory.CreateDirectory(imagesDir);

                        string destinationPath = System.IO.Path.Combine(imagesDir, imageFileName);

                        File.Copy(_imagePath, destinationPath, overwrite: true);
                    }

                    var photo = new EquipmentPhotos
                    {
                        EquipmentId = newEquipment.Id,
                        FilePath = relativePath
                    };

                    context.EquipmentPhotos.Add(photo);
                    context.SaveChanges();

                    MessageBox.Show("Оборудование успешно добавлено!");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void ldExit_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }

    public partial class EquipmentPhoto
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public string FilePath { get; set; }

        public virtual Equipment Equipment { get; set; }
    }
}
