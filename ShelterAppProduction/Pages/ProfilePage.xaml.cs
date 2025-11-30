using System.Windows;
using System.Windows.Controls;
using ShelterAppProduction.Services;

namespace ShelterAppProduction.Pages
{
    public partial class ProfilePage : Page
    {
        private AuthService authService = new AuthService();

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
                AvatarTextBox.Text = user.Avatar ?? "";
                AvatarTextBlock.Text = user.Avatar ?? "👤";
            }
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

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SessionManager.IsAuthenticated)
                return;

            var fullName = FullNameTextBox.Text.Trim();
            var avatar = AvatarTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Введите полное имя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = authService.UpdateProfile(SessionManager.CurrentUser.Id, fullName, avatar);

            if (result)
            {
                AvatarTextBlock.Text = string.IsNullOrWhiteSpace(avatar) ? "👤" : avatar;
                MessageBox.Show("Профиль успешно обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
