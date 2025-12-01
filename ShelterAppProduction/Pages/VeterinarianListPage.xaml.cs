using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ShelterAppProduction.Repositories;

namespace ShelterAppProduction.Pages
{
    public partial class VeterinarianListPage : Page
    {
        private VeterinarianRepository veterinarianRepository = new VeterinarianRepository();

        public VeterinarianListPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadVeterinarians();
        }

        private async Task LoadVeterinarians()
        {
            var veterinarians = await veterinarianRepository.GetAll();
            VeterinariansDataGrid.ItemsSource = veterinarians;
        }

        private void AddVeterinarianButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new VeterinarianProfilePage());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var veterinarianId = (int)button.Tag;
            Manager.MainFrame.Navigate(new VeterinarianProfilePage(veterinarianId));
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var veterinarianId = (int)button.Tag;

            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить этого ветеринара?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (await veterinarianRepository.DeleteVeterinarian(veterinarianId))
                {
                    MessageBox.Show("Ветеринар успешно удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadVeterinarians();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении ветеринара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
