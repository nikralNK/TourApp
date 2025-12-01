using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ShelterAppProduction.Services;

namespace ShelterAppProduction.Pages
{
    public partial class ProfilePage : Page
    {
        private AuthService authService = new AuthService();
        private string selectedAvatarPath = null;

        public ProfilePage()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (SessionManager.IsAuthenticated)
            {
                var user = SessionManager.CurrentUser;
                UsernameTextBlock.Text = user.Username;
                FullNameTextBox.Text = user.FullName ?? "";
                EmailTextBlock.Text = user.Email ?? "Не указано";
                RoleTextBlock.Text = user.Role ?? "User";

                if (!string.IsNullOrWhiteSpace(user.Avatar) && File.Exists(user.Avatar))
                {
                    LoadAvatar(user.Avatar);
                }
            }
        }

        private void LoadAvatar(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
                AvatarImage.Source = bitmap;
            }
            catch { }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordStatusTextBlock.Text = "";
            PasswordStatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;

            var newPassword = NewPasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                PasswordStatusTextBlock.Text = "Заполните все поля";
                return;
            }

            if (newPassword != confirmPassword)
            {
                PasswordStatusTextBlock.Text = "Пароли не совпадают";
                return;
            }

            if (newPassword.Length < 4)
            {
                PasswordStatusTextBlock.Text = "Пароль должен содержать минимум 4 символа";
                return;
            }

            authService.ResetPassword(SessionManager.CurrentUser.Username, newPassword);

            PasswordStatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
            PasswordStatusTextBlock.Text = "Пароль успешно изменен";

            NewPasswordBox.Clear();
            ConfirmPasswordBox.Clear();
        }

        private void SelectAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Выберите изображение для аватара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedAvatarPath = openFileDialog.FileName;
                LoadAvatar(selectedAvatarPath);
            }
        }

        private async void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SessionManager.IsAuthenticated)
                return;

            var fullName = FullNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Введите полное имя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string avatarPathToSave = selectedAvatarPath ?? SessionManager.CurrentUser.Avatar;

            if (!string.IsNullOrWhiteSpace(selectedAvatarPath))
            {
                try
                {
                    var avatarsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatars");
                    if (!Directory.Exists(avatarsFolder))
                    {
                        Directory.CreateDirectory(avatarsFolder);
                    }

                    var fileName = $"{SessionManager.CurrentUser.Id}_{Path.GetFileName(selectedAvatarPath)}";
                    var destPath = Path.Combine(avatarsFolder, fileName);

                    File.Copy(selectedAvatarPath, destPath, true);
                    avatarPathToSave = destPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении аватара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var result = await authService.UpdateProfile(SessionManager.CurrentUser.Id, fullName, avatarPathToSave);

            if (result)
            {
                selectedAvatarPath = null;
                MessageBox.Show("Профиль успешно обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
