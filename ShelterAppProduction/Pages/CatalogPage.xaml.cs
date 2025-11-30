using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ShelterAppProduction.Models;
using ShelterAppProduction.Repositories;

namespace ShelterAppProduction.Pages
{
    public partial class CatalogPage : Page
    {
        private AnimalRepository animalRepository = new AnimalRepository();

        public CatalogPage()
        {
            InitializeComponent();
            LoadAnimals();
        }

        private void LoadAnimals()
        {
            var type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var size = (SizeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            type = type == "Все" ? null : type;
            gender = gender == "Все" ? null : gender;
            size = size == "Все" ? null : size;

            var animals = animalRepository.GetFiltered(type, gender, size);
            AnimalsItemsControl.ItemsSource = animals.Where(a => a.CurrentStatus == "Доступен");
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnimalsItemsControl != null)
            {
                LoadAnimals();
            }
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            TypeComboBox.SelectedIndex = 0;
            GenderComboBox.SelectedIndex = 0;
            SizeComboBox.SelectedIndex = 0;
            LoadAnimals();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var animalId = (int)button.Tag;
            Manager.MainFrame.Navigate(new AnimalDetailPage(animalId));
        }

        private void AnimalCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var animal = border.DataContext as Animal;
            if (animal != null)
            {
                Manager.MainFrame.Navigate(new AnimalDetailPage(animal.Id));
            }
        }
    }
}
