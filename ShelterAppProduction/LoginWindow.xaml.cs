using System.Windows;
using ShelterAppProduction.Services;

namespace ShelterAppProduction
{
    public partial class LoginWindow : Window
    {
        private AuthService authService = new AuthService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = "";

            var username = UsernameTextBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "Введите логин и пароль";
                return;
            }

            var user = authService.Login(username, password);

            if (user != null)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorTextBlock.Text = "Неверный логин или пароль";
            }
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ErrorTextBlock.Text = "Введите логин для сброса пароля";
                return;
            }

            var result = MessageBox.Show(
                $"Сбросить пароль для пользователя {username} на 'admin'?",
                "Сброс пароля",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                authService.ResetPassword(username, "admin");
                MessageBox.Show("Пароль успешно сброшен на 'admin'", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ErrorTextBlock.Text = "";
            }
        }
    }
}
