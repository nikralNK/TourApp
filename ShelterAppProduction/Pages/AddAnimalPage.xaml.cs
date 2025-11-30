using System;
using System.Windows;
using System.Windows.Controls;
using ShelterAppProduction.Models;
using ShelterAppProduction.Repositories;

namespace ShelterAppProduction.Pages
{
    public partial class AddAnimalPage : Page
    {
        private AnimalRepository animalRepository = new AnimalRepository();

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
                CurrentStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
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

        private void ClearForm()
        {
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
