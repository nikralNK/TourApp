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

                    if (veterinarian.UserId.HasValue)
                    {
                        AccountStatusTextBlock.Text = "✓ Учетная запись создана";
                        AccountStatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                        AccountPanel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        AccountStatusTextBlock.Text = "⚠ Учетная запись не создана. Заполните поля ниже для создания.";
                        AccountStatusTextBlock.Foreground = System.Windows.Media.Brushes.Orange;
                        AccountPanel.Visibility = Visibility.Visible;
                    }
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

            bool needCreateAccount = false;
            if (!veterinarianId.HasValue || (veterinarianId.HasValue && AccountPanel.Visibility == Visibility.Visible))
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
                needCreateAccount = true;
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
                var existingVet = veterinarianRepository.GetById(veterinarianId.Value);
                veterinarian.UserId = existingVet.UserId;

                if (needCreateAccount)
                {
                    var authService = new AuthService();
                    var userId = authService.RegisterUser(UsernameTextBox.Text.Trim(), PasswordBox.Password, FullNameTextBox.Text.Trim(), "veterinarian");

                    if (userId.HasValue)
                    {
                        veterinarian.UserId = userId.Value;
                    }
                    else
                    {
                        StatusTextBlock.Text = "Ошибка при создании учетной записи. Возможно, логин уже занят";
                        return;
                    }
                }

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
