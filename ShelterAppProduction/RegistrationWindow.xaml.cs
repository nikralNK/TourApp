using System.Windows;
using ShelterAppProduction.Services;

namespace ShelterAppProduction
{
    public partial class RegistrationWindow : Window
    {
        private AuthService authService = new AuthService();

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";

            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                StatusTextBlock.Text = "Введите логин";
                return;
            }

            if (UsernameTextBox.Text.Length < 3)
            {
                StatusTextBlock.Text = "Логин должен содержать минимум 3 символа";
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                StatusTextBlock.Text = "Введите пароль";
                return;
            }

            if (PasswordBox.Password.Length < 6)
            {
                StatusTextBlock.Text = "Пароль должен содержать минимум 6 символов";
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                StatusTextBlock.Text = "Пароли не совпадают";
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                StatusTextBlock.Text = "Введите email";
                return;
            }

            if (!EmailTextBox.Text.Contains("@"))
            {
                StatusTextBlock.Text = "Введите корректный email";
                return;
            }

            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                StatusTextBlock.Text = "Введите полное имя";
                return;
            }

            var result = authService.Register(
                UsernameTextBox.Text.Trim(),
                PasswordBox.Password,
                EmailTextBox.Text.Trim(),
                FullNameTextBox.Text.Trim()
            );

            if (result)
            {
                MessageBox.Show("Регистрация успешно завершена! Теперь вы можете войти в систему.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                StatusTextBlock.Text = "Ошибка регистрации. Возможно, пользователь с таким логином уже существует.";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
