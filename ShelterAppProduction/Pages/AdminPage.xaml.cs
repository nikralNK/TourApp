using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ShelterAppProduction.Repositories;
using ShelterAppProduction.Services;

namespace ShelterAppProduction.Pages
{
    public partial class AdminPage : Page
    {
        private ApplicationRepository applicationRepository = new ApplicationRepository();
        private AnimalRepository animalRepository = new AnimalRepository();
        private GuardianRepository guardianRepository = new GuardianRepository();

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

        private void AddAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddAnimalPage());
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
                applicationRepository.UpdateApplicationStatus(applicationId, "Одобрена");
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
                applicationRepository.UpdateApplicationStatus(applicationId, "Отклонена");
                LoadApplications();
                MessageBox.Show("Заявка отклонена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AnimalId_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock != null && textBlock.Tag != null)
            {
                var animalId = (int)textBlock.Tag;
                var animal = animalRepository.GetById(animalId);

                if (animal != null)
                {
                    PopupAnimalName.Text = $"Кличка: {animal.Name}";
                    PopupAnimalType.Text = $"Тип: {animal.Type ?? "Не указано"}";
                    PopupAnimalBreed.Text = $"Порода: {animal.Breed ?? "Не указано"}";
                    PopupAnimalGender.Text = $"Пол: {animal.Gender ?? "Не указано"}";
                    PopupAnimalAge.Text = $"Возраст: {animal.Age} лет";
                    PopupAnimalSize.Text = $"Размер: {animal.Size ?? "Не указано"}";
                    PopupAnimalTemperament.Text = $"Темперамент: {animal.Temperament ?? "Не указано"}";
                    PopupAnimalStatus.Text = $"Статус: {animal.CurrentStatus}";

                    AnimalInfoPopup.IsOpen = true;
                }
            }
        }

        private void AnimalId_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimalInfoPopup.IsOpen = false;
        }

        private void GuardianName_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock != null && textBlock.Tag != null)
            {
                var guardianId = (int)textBlock.Tag;
                var guardian = guardianRepository.GetById(guardianId);

                if (guardian != null)
                {
                    PopupGuardianFullName.Text = $"ФИО: {guardian.FullName}";
                    PopupGuardianPhone.Text = $"Телефон: {guardian.PhoneNumber ?? "Не указано"}";
                    PopupGuardianEmail.Text = $"Email: {guardian.Email ?? "Не указано"}";
                    PopupGuardianAddress.Text = $"Адрес: {guardian.Address ?? "Не указано"}";
                    PopupGuardianRegistrationDate.Text = $"Дата регистрации: {guardian.RegistrationDate:dd.MM.yyyy}";

                    GuardianInfoPopup.IsOpen = true;
                }
            }
        }

        private void GuardianName_MouseLeave(object sender, MouseEventArgs e)
        {
            GuardianInfoPopup.IsOpen = false;
        }
    }
}
