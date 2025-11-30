using System.Windows;
using ShelterAppProduction.Pages;
using ShelterAppProduction.Services;

namespace ShelterAppProduction
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Manager.MainFrame = MainFrame;

            if (SessionManager.IsAuthenticated)
            {
                UserNameTextBlock.Text = $"Добро пожаловать, {SessionManager.CurrentUser.FullName ?? SessionManager.CurrentUser.Username}";

                MessageBox.Show($"Роль: '{SessionManager.CurrentUser.Role}'\nДлина: {SessionManager.CurrentUser.Role?.Length}\nIsAdmin: {SessionManager.IsAdmin}", "Отладка MainWindow");

                if (SessionManager.IsAdmin)
                {
                    AdminButton.Visibility = Visibility.Visible;
                    VeterinarianButton.Visibility = Visibility.Visible;
                }
            }

            MainFrame.Navigate(new CatalogPage());
        }

        private void CatalogButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CatalogPage());
        }

        private void FavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new FavoritesPage());
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage());
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AdminPage());
        }

        private void VeterinarianButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new VeterinarianListPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentUser = null;
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
