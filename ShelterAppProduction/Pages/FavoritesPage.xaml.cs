using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ShelterAppProduction.Models;
using ShelterAppProduction.Repositories;
using ShelterAppProduction.Services;

namespace ShelterAppProduction.Pages
{
    public partial class FavoritesPage : Page
    {
        private FavoriteRepository favoriteRepository = new FavoriteRepository();

        public FavoritesPage()
        {
            InitializeComponent();
            LoadFavorites();
        }

        private void LoadFavorites()
        {
            if (!SessionManager.IsAuthenticated)
            {
                EmptyMessageTextBlock.Text = "Необходимо войти в систему для просмотра избранного";
                EmptyMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            var favorites = favoriteRepository.GetFavoritesByUser(SessionManager.CurrentUser.Id);

            if (favorites.Count == 0)
            {
                EmptyMessageTextBlock.Visibility = Visibility.Visible;
                FavoritesItemsControl.ItemsSource = null;
            }
            else
            {
                EmptyMessageTextBlock.Visibility = Visibility.Collapsed;
                FavoritesItemsControl.ItemsSource = favorites;
            }
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
