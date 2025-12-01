using ShelterAppProduction.Models;
using ShelterAppProduction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShelterAppProduction.Repositories
{
    public class FavoriteRepository
    {
        private readonly AnimalRepository _animalRepository;

        public FavoriteRepository()
        {
            _animalRepository = new AnimalRepository();
        }

        public async Task<List<Animal>> GetFavoritesByUser(int userId)
        {
            try
            {
                var favorites = await ApiService.GetAsync<List<FavoriteResponse>>("favorites/");
                var animalIds = favorites.Select(f => f.IdAnimal).ToList();

                var allAnimals = await _animalRepository.GetAll();
                return allAnimals.Where(a => animalIds.Contains(a.Id)).ToList();
            }
            catch
            {
                return new List<Animal>();
            }
        }

        public async Task AddFavorite(int userId, int animalId)
        {
            try
            {
                var request = new FavoriteCreateRequest
                {
                    IdAnimal = animalId
                };

                await ApiService.PostAsync<FavoriteResponse>("favorites/", request);
            }
            catch
            {
            }
        }

        public async Task RemoveFavorite(int userId, int animalId)
        {
            try
            {
                await ApiService.DeleteAsync($"favorites/animal/{animalId}");
            }
            catch
            {
            }
        }

        public async Task<bool> IsFavorite(int userId, int animalId)
        {
            try
            {
                var favorites = await ApiService.GetAsync<List<FavoriteResponse>>("favorites/");
                return favorites.Any(f => f.IdAnimal == animalId);
            }
            catch
            {
                return false;
            }
        }
    }
}
