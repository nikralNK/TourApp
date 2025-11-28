using System.Windows;
using System.Windows.Controls;
using ShelterAppProduction.Repositories;
using ShelterAppProduction.Services;

namespace ShelterAppProduction.Pages
{
    public partial class AdminPage : Page
    {
        private ApplicationRepository applicationRepository = new ApplicationRepository();

        public AdminPage()
        {
            InitializeComponent();

            if (!SessionManager.IsAdmin)
            {
                MessageBox.Show("Доступ запрещен. Только для администраторов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Manager.MainFrame.GoBack();
                return;
            }

            LoadApplications();
        }

        private void LoadApplications()
        {
            var applications = applicationRepository.GetAllApplications();
            ApplicationsDataGrid.ItemsSource = applications;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadApplications();
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var applicationId = (int)button.Tag;

            var result = MessageBox.Show(
                "Одобрить эту заявку?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                applicationRepository.UpdateApplicationStatus(applicationId, "Approved");
                LoadApplications();
                MessageBox.Show("Заявка одобрена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var applicationId = (int)button.Tag;

            var result = MessageBox.Show(
                "Отклонить эту заявку?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                applicationRepository.UpdateApplicationStatus(applicationId, "Rejected");
                LoadApplications();
                MessageBox.Show("Заявка отклонена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
