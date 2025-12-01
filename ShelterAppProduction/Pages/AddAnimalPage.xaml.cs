using System;
using System.IO;
using System.Threading.Tasks;
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
        private Animal editingAnimal = null;

        public AddAnimalPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadEnclosures();
        }

        public AddAnimalPage(Animal animal) : this()
        {
            editingAnimal = animal;
            PageTitle.Text = "Редактирование животного";
            AddAnimalButton.Content = "Сохранить изменения";
            LoadAnimalData();
        }

        private void LoadAnimalData()
        {
            if (editingAnimal == null) return;

            NameTextBox.Text = editingAnimal.Name;
            BreedTextBox.Text = editingAnimal.Breed;
            DateOfBirthPicker.SelectedDate = editingAnimal.DateOfBirth;
            TemperamentTextBox.Text = editingAnimal.Temperament;

            if (!string.IsNullOrWhiteSpace(editingAnimal.Type))
            {
                foreach (ComboBoxItem item in TypeComboBox.Items)
                {
                    if (item.Content.ToString() == editingAnimal.Type)
                    {
                        TypeComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(editingAnimal.Gender))
            {
                foreach (ComboBoxItem item in GenderComboBox.Items)
                {
                    if (item.Content.ToString() == editingAnimal.Gender)
                    {
                        GenderComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(editingAnimal.Size))
            {
                foreach (ComboBoxItem item in SizeComboBox.Items)
                {
                    if (item.Content.ToString() == editingAnimal.Size)
                    {
                        SizeComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(editingAnimal.CurrentStatus))
            {
                foreach (ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Content.ToString() == editingAnimal.CurrentStatus)
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (editingAnimal.IdEnclosure.HasValue)
            {
                EnclosureComboBox.SelectedValue = editingAnimal.IdEnclosure.Value;
            }

            if (!string.IsNullOrWhiteSpace(editingAnimal.Photo) && File.Exists(editingAnimal.Photo))
            {
                selectedPhotoPath = editingAnimal.Photo;
                LoadPhoto(editingAnimal.Photo);
            }
        }

        private async Task LoadEnclosures()
        {
            try
            {
                var enclosures = await animalRepository.GetAllEnclosures();
                EnclosureComboBox.ItemsSource = enclosures;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке вольеров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddAnimalButton_Click(object sender, RoutedEventArgs e)
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
                Id = editingAnimal?.Id ?? 0,
                Name = NameTextBox.Text.Trim(),
                Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Breed = string.IsNullOrWhiteSpace(BreedTextBox.Text) ? null : BreedTextBox.Text.Trim(),
                DateOfBirth = DateOfBirthPicker.SelectedDate ?? DateTime.Today,
                Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Size = (SizeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Temperament = string.IsNullOrWhiteSpace(TemperamentTextBox.Text) ? null : TemperamentTextBox.Text.Trim(),
                IdEnclosure = EnclosureComboBox.SelectedValue != null ? (int?)EnclosureComboBox.SelectedValue : null,
                CurrentStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Photo = photoPathToSave ?? editingAnimal?.Photo
            };

            try
            {
                bool success;
                string message;

                if (editingAnimal != null)
                {
                    success = await animalRepository.UpdateAnimal(animal);
                    message = success ? $"Животное '{animal.Name}' успешно обновлено!" : "Ошибка при обновлении животного в базе данных";
                }
                else
                {
                    success = await animalRepository.AddAnimal(animal);
                    message = success ? $"Животное '{animal.Name}' успешно добавлено!" : "Ошибка при добавлении животного в базе данных";
                }

                if (success)
                {
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                    StatusTextBlock.Text = message;

                    MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (editingAnimal == null)
                    {
                        ClearForm();
                    }
                    else
                    {
                        Manager.MainFrame.GoBack();
                    }
                }
                else
                {
                    StatusTextBlock.Text = message;
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
