using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ShelterAppProduction.Models;
using ShelterAppProduction.Repositories;

namespace ShelterAppProduction.Pages
{
    public partial class AddAnimalPage : Page
    {
        private AnimalRepository animalRepository = new AnimalRepository();
        private string selectedPhotoPath = null;

        public AddAnimalPage()
        {
            InitializeComponent();
            LoadEnclosures();
        }

        private void LoadEnclosures()
        {
            try
            {
                var enclosures = animalRepository.GetAllEnclosures();
                EnclosureComboBox.ItemsSource = enclosures;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке вольеров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";
            StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                StatusTextBlock.Text = "Введите кличку животного";
                return;
            }

            if (TypeComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "Выберите тип животного";
                return;
            }

            if (GenderComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "Выберите пол животного";
                return;
            }

            if (SizeComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = "Выберите размер животного";
                return;
            }

            string photoPathToSave = null;

            if (!string.IsNullOrWhiteSpace(selectedPhotoPath))
            {
                try
                {
                    var photosFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AnimalPhotos");
                    if (!Directory.Exists(photosFolder))
                    {
                        Directory.CreateDirectory(photosFolder);
                    }

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(selectedPhotoPath)}";
                    var destPath = Path.Combine(photosFolder, fileName);

                    File.Copy(selectedPhotoPath, destPath, true);
                    photoPathToSave = destPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении фото: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var animal = new Animal
            {
                Name = NameTextBox.Text.Trim(),
                Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Breed = string.IsNullOrWhiteSpace(BreedTextBox.Text) ? null : BreedTextBox.Text.Trim(),
                DateOfBirth = DateOfBirthPicker.SelectedDate ?? DateTime.Today,
                Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Size = (SizeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Temperament = string.IsNullOrWhiteSpace(TemperamentTextBox.Text) ? null : TemperamentTextBox.Text.Trim(),
                IdEnclosure = EnclosureComboBox.SelectedValue != null ? (int?)EnclosureComboBox.SelectedValue : null,
                CurrentStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Photo = photoPathToSave
            };

            try
            {
                bool success = animalRepository.AddAnimal(animal);

                if (success)
                {
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                    StatusTextBlock.Text = $"Животное '{animal.Name}' успешно добавлено!";

                    ClearForm();

                    MessageBox.Show($"Животное '{animal.Name}' успешно добавлено в базу данных!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    StatusTextBlock.Text = "Ошибка при добавлении животного в базу данных";
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Выберите фото животного"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedPhotoPath = openFileDialog.FileName;
                LoadPhoto(selectedPhotoPath);
            }
        }

        private void LoadPhoto(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
                PhotoImage.Source = bitmap;
            }
            catch { }
        }

        private void ClearForm()
        {
            selectedPhotoPath = null;
            PhotoImage.Source = null;
            NameTextBox.Text = "";
            TypeComboBox.SelectedIndex = -1;
            BreedTextBox.Text = "";
            DateOfBirthPicker.SelectedDate = DateTime.Today;
            GenderComboBox.SelectedIndex = -1;
            SizeComboBox.SelectedIndex = -1;
            TemperamentTextBox.Text = "";
            EnclosureComboBox.SelectedIndex = -1;
            StatusComboBox.SelectedIndex = 0;
        }
    }
}
