using System.Threading.Tasks;
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
            Loaded += async (s, e) => await LoadVeterinarianData();
        }

        private async Task LoadVeterinarianData()
        {
            if (veterinarianId.HasValue)
            {
                var veterinarian = await veterinarianRepository.GetById(veterinarianId.Value);
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";
            StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;

            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                StatusTextBlock.Text = "Введите ФИО ветеринара";
                return;
            }

            string fullName = FullNameTextBox.Text.Trim();
            string specialization = string.IsNullOrWhiteSpace(SpecializationTextBox.Text) ? null : SpecializationTextBox.Text.Trim();
            string phoneNumber = string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text) ? null : PhoneNumberTextBox.Text.Trim();
            string licenseNumber = string.IsNullOrWhiteSpace(LicenseNumberTextBox.Text) ? null : LicenseNumberTextBox.Text.Trim();

            bool success = false;

            if (veterinarianId.HasValue)
            {
                var existingVet = await veterinarianRepository.GetById(veterinarianId.Value);

                if (existingVet != null && !existingVet.UserId.HasValue && AccountPanel.Visibility == Visibility.Visible)
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

                    MessageBox.Show("Функция создания аккаунта для существующего ветеринара временно недоступна через API", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                success = await veterinarianRepository.UpdateVeterinarian(veterinarianId.Value, fullName, specialization, phoneNumber, licenseNumber);
            }
            else
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

                var veterinarian = await veterinarianRepository.AddVeterinarian(
                    UsernameTextBox.Text.Trim(),
                    PasswordBox.Password,
                    "",
                    fullName,
                    specialization,
                    phoneNumber,
                    licenseNumber
                );

                success = veterinarian != null;
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
