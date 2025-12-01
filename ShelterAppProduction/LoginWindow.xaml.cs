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

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = "";

            var username = UsernameTextBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "Введите логин и пароль";
                return;
            }

            var user = await authService.Login(username, password);

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

            MessageBox.Show("Функция сброса пароля доступна только через API администратора", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
        }
    }
}
