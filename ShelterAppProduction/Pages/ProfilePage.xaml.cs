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
                FullNameTextBlock.Text = user.FullName ?? "Не указано";
                EmailTextBlock.Text = user.Email ?? "Не указано";
                RoleTextBlock.Text = user.Role ?? "User";
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
    }
}
