using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ShelterAppProduction.Models;
using ShelterAppProduction.Repositories;
using ShelterAppProduction.Services;

namespace ShelterAppProduction.Pages
{
    public class MedicalRecordViewModel
    {
        public DateTime VisitDate { get; set; }
        public string VeterinarianName { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
    }

    public partial class AnimalDetailPage : Page
    {
        private int animalId;
        private Animal animal;
        private AnimalRepository animalRepository = new AnimalRepository();
        private FavoriteRepository favoriteRepository = new FavoriteRepository();
        private ApplicationRepository applicationRepository = new ApplicationRepository();
        private MedicalRecordRepository medicalRecordRepository = new MedicalRecordRepository();

        public AnimalDetailPage(int id)
        {
            InitializeComponent();
            animalId = id;
            LoadAnimalData();
        }

        private void LoadAnimalData()
        {
            animal = animalRepository.GetById(animalId);

            if (animal != null)
            {
                AnimalNameTextBlock.Text = animal.Name;
                TypeTextBlock.Text = animal.Type ?? "Не указано";
                BreedTextBlock.Text = animal.Breed ?? "Не указано";
                DateOfBirthTextBlock.Text = animal.DateOfBirth.ToString("dd.MM.yyyy");
                AgeTextBlock.Text = $"{animal.Age} лет";
                GenderTextBlock.Text = animal.Gender ?? "Не указано";
                SizeTextBlock.Text = animal.Size ?? "Не указано";
                TemperamentTextBlock.Text = animal.Temperament ?? "Не указано";
                StatusTextBlock.Text = animal.CurrentStatus ?? "Не указано";

                if (!string.IsNullOrWhiteSpace(animal.Photo) && File.Exists(animal.Photo))
                {
                    LoadPhoto(animal.Photo);
                }

                if (SessionManager.IsAuthenticated && favoriteRepository.IsFavorite(SessionManager.CurrentUser.Id, animalId))
                {
                    FavoriteButton.Content = "♥ Удалить из избранного";
                }

                if (SessionManager.IsAuthenticated && (SessionManager.CurrentUser.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) || SessionManager.CurrentUser.Role.Equals("veterinarian", StringComparison.OrdinalIgnoreCase)))
                {
                    ApplicationBorder.Visibility = Visibility.Collapsed;
                }

                if (SessionManager.IsAuthenticated && SessionManager.CurrentUser.Role.Equals("veterinarian", StringComparison.OrdinalIgnoreCase))
                {
                    AddRecordBorder.Visibility = Visibility.Visible;
                }

                if (SessionManager.IsAdmin)
                {
                    EditButton.Visibility = Visibility.Visible;
                }
            }

            VisitDatePicker.SelectedDate = DateTime.Now;
            LoadMedicalRecords();
        }

        private void LoadMedicalRecords()
        {
            var records = medicalRecordRepository.GetByAnimalId(animalId);

            if (records.Count == 0)
            {
                NoMedicalRecordsTextBlock.Visibility = Visibility.Visible;
                MedicalRecordsItemsControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                var viewModels = new List<MedicalRecordViewModel>();
                foreach (var record in records)
                {
                    viewModels.Add(new MedicalRecordViewModel
                    {
                        VisitDate = record.VisitDate,
                        VeterinarianName = medicalRecordRepository.GetVeterinarianName(record.IdVeterinarian),
                        Diagnosis = record.Diagnosis ?? "Не указан",
                        Treatment = record.Treatment ?? "Не указано",
                        Notes = record.Notes ?? "Нет примечаний"
                    });
                }
                MedicalRecordsItemsControl.ItemsSource = viewModels;
                NoMedicalRecordsTextBlock.Visibility = Visibility.Collapsed;
                MedicalRecordsItemsControl.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SessionManager.IsAuthenticated)
            {
                MessageBox.Show("Необходимо войти в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (FavoriteButton.Content.ToString().Contains("Добавить"))
            {
                favoriteRepository.AddFavorite(SessionManager.CurrentUser.Id, animalId);
                FavoriteButton.Content = "♥ Удалить из избранного";
                MessageBox.Show("Добавлено в избранное", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                favoriteRepository.RemoveFavorite(SessionManager.CurrentUser.Id, animalId);
                FavoriteButton.Content = "♡ Добавить в избранное";
                MessageBox.Show("Удалено из избранного", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SubmitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationStatusTextBlock.Text = "";
            ApplicationStatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;

            var fullName = FullNameTextBox.Text.Trim();
            var phone = PhoneTextBox.Text.Trim();
            var email = EmailTextBox.Text.Trim();
            var comments = CommentsTextBox.Text.Trim();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email))
            {
                ApplicationStatusTextBlock.Text = "Заполните все обязательные поля";
                return;
            }

            applicationRepository.CreateApplication(animalId, fullName, phone, email, comments);

            ApplicationStatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
            ApplicationStatusTextBlock.Text = "Заявка успешно отправлена! Мы свяжемся с вами в ближайшее время.";

            FullNameTextBox.Clear();
            PhoneTextBox.Clear();
            EmailTextBox.Clear();
            CommentsTextBox.Clear();
        }

        private void LoadPhoto(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
                PhotoImage.Source = bitmap;
            }
            catch { }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddAnimalPage(animal));
        }

        private void AddMedicalRecordButton_Click(object sender, RoutedEventArgs e)
        {
            MedicalRecordStatusTextBlock.Text = "";
            MedicalRecordStatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;

            if (!VisitDatePicker.SelectedDate.HasValue)
            {
                MedicalRecordStatusTextBlock.Text = "Выберите дату визита";
                return;
            }

            if (string.IsNullOrWhiteSpace(DiagnosisTextBox.Text))
            {
                MedicalRecordStatusTextBlock.Text = "Введите диагноз";
                return;
            }

            var vetId = medicalRecordRepository.GetVeterinarianIdByUserId(SessionManager.CurrentUser.Id);
            if (!vetId.HasValue)
            {
                MedicalRecordStatusTextBlock.Text = "Ошибка: у вас нет профиля ветеринара. Только ветеринары могут добавлять записи.";
                return;
            }
            int veterinarianId = vetId.Value;

            var record = new MedicalRecord
            {
                IdAnimal = animalId,
                IdVeterinarian = veterinarianId,
                VisitDate = VisitDatePicker.SelectedDate.Value,
                Diagnosis = DiagnosisTextBox.Text.Trim(),
                Treatment = string.IsNullOrWhiteSpace(TreatmentTextBox.Text) ? null : TreatmentTextBox.Text.Trim(),
                Notes = string.IsNullOrWhiteSpace(NotesTextBox.Text) ? null : NotesTextBox.Text.Trim()
            };

            string errorMessage;
            if (medicalRecordRepository.AddMedicalRecord(record, out errorMessage))
            {
                MedicalRecordStatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                MedicalRecordStatusTextBlock.Text = "Запись успешно добавлена!";

                DiagnosisTextBox.Clear();
                TreatmentTextBox.Clear();
                NotesTextBox.Clear();
                VisitDatePicker.SelectedDate = DateTime.Now;

                LoadMedicalRecords();
            }
            else
            {
                MedicalRecordStatusTextBlock.Text = $"Ошибка при сохранении записи: {errorMessage}";
            }
        }
    }
}
