using System.Windows;
using System.Windows.Controls;
using ShelterAppProduction.Models;
using ShelterAppProduction.Repositories;
using ShelterAppProduction.Services;

namespace ShelterAppProduction.Pages
{
    public partial class VeterinarianProfilePage : Page
    {
        private VeterinarianRepository veterinarianRepository = new VeterinarianRepository();
        private int? veterinarianId = null;

        public VeterinarianProfilePage()
        {
            InitializeComponent();
            TitleTextBlock.Text = "Добавить ветеринара";
            SaveButton.Content = "Добавить";
        }

        public VeterinarianProfilePage(int id)
        {
            InitializeComponent();
            veterinarianId = id;
            TitleTextBlock.Text = "Редактировать ветеринара";
            SaveButton.Content = "Сохранить";
            LoadVeterinarianData();
        }

        private void LoadVeterinarianData()
        {
            if (veterinarianId.HasValue)
            {
                var veterinarian = veterinarianRepository.GetById(veterinarianId.Value);
                if (veterinarian != null)
                {
                    FullNameTextBox.Text = veterinarian.FullName;
                    SpecializationTextBox.Text = veterinarian.Specialization ?? "";
                    PhoneNumberTextBox.Text = veterinarian.PhoneNumber ?? "";
                    LicenseNumberTextBox.Text = veterinarian.LicenseNumber ?? "";
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";
            StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;

            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                StatusTextBlock.Text = "Введите ФИО ветеринара";
                return;
            }

            if (!veterinarianId.HasValue)
            {
                if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
                {
                    StatusTextBlock.Text = "Введите логин";
                    return;
                }

                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    StatusTextBlock.Text = "Введите пароль";
                    return;
                }
            }

            var veterinarian = new Veterinarian
            {
                FullName = FullNameTextBox.Text.Trim(),
                Specialization = string.IsNullOrWhiteSpace(SpecializationTextBox.Text) ? null : SpecializationTextBox.Text.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text) ? null : PhoneNumberTextBox.Text.Trim(),
                LicenseNumber = string.IsNullOrWhiteSpace(LicenseNumberTextBox.Text) ? null : LicenseNumberTextBox.Text.Trim()
            };

            bool success;
            if (veterinarianId.HasValue)
            {
                veterinarian.Id = veterinarianId.Value;
                success = veterinarianRepository.UpdateVeterinarian(veterinarian);
            }
            else
            {
                var authService = new AuthService();
                var userId = authService.RegisterUser(UsernameTextBox.Text.Trim(), PasswordBox.Password, FullNameTextBox.Text.Trim(), "veterinarian");

                if (userId.HasValue)
                {
                    veterinarian.UserId = userId.Value;
                    success = veterinarianRepository.AddVeterinarian(veterinarian);
                }
                else
                {
                    StatusTextBlock.Text = "Ошибка при создании учетной записи. Возможно, логин уже занят";
                    return;
                }
            }

            if (success)
            {
                StatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                StatusTextBlock.Text = veterinarianId.HasValue
                    ? "Данные ветеринара успешно обновлены!"
                    : "Ветеринар успешно добавлен!";

                MessageBox.Show(
                    veterinarianId.HasValue ? "Данные ветеринара обновлены" : "Ветеринар добавлен",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Manager.MainFrame.GoBack();
            }
            else
            {
                StatusTextBlock.Text = "Ошибка при сохранении данных";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
    }
}
