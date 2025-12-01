using ShelterAppProduction.Models;
using ShelterAppProduction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShelterAppProduction.Repositories
{
    public class AnimalRepository
    {
        public async Task<List<Animal>> GetAll()
        {
            try
            {
                var response = await ApiService.GetAsync<List<AnimalResponse>>("animals/");
                return response.Select(MapFromApiResponse).ToList();
            }
            catch
            {
                return new List<Animal>();
            }
        }

        public async Task<List<Animal>> GetFiltered(string type = null, string gender = null, string size = null, int? minAge = null, int? maxAge = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(type))
                    queryParams.Add($"type={Uri.EscapeDataString(type)}");
                if (!string.IsNullOrEmpty(gender))
                    queryParams.Add($"gender={Uri.EscapeDataString(gender)}");
                if (!string.IsNullOrEmpty(size))
                    queryParams.Add($"size={Uri.EscapeDataString(size)}");

                var endpoint = "animals/";
                if (queryParams.Count > 0)
                    endpoint += "?" + string.Join("&", queryParams);

                var response = await ApiService.GetAsync<List<AnimalResponse>>(endpoint);
                var animals = response.Select(MapFromApiResponse).ToList();

                if (minAge.HasValue || maxAge.HasValue)
                {
                    animals = animals.Where(a =>
                    {
                        if (minAge.HasValue && a.Age < minAge.Value)
                            return false;
                        if (maxAge.HasValue && a.Age > maxAge.Value)
                            return false;
                        return true;
                    }).ToList();
                }

                return animals;
            }
            catch
            {
                return new List<Animal>();
            }
        }

        public async Task<Animal> GetById(int id)
        {
            try
            {
                var response = await ApiService.GetAsync<AnimalResponse>($"animals/{id}");
                return MapFromApiResponse(response);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AddAnimal(Animal animal)
        {
            try
            {
                var request = new AnimalCreateRequest
                {
                    Name = animal.Name,
                    Type = animal.Type,
                    Breed = animal.Breed,
                    DateOfBirth = animal.DateOfBirth,
                    Gender = animal.Gender,
                    Size = animal.Size,
                    Temperament = animal.Temperament,
                    IdEnclosure = animal.IdEnclosure
                };

                await ApiService.PostAsync<AnimalResponse>("animals/", request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAnimal(Animal animal)
        {
            try
            {
                var request = new AnimalUpdateRequest
                {
                    Name = animal.Name,
                    Type = animal.Type,
                    Breed = animal.Breed,
                    DateOfBirth = animal.DateOfBirth,
                    Gender = animal.Gender,
                    Size = animal.Size,
                    Temperament = animal.Temperament,
                    IdEnclosure = animal.IdEnclosure,
                    IdGuardian = animal.IdGuardian,
                    CurrentStatus = animal.CurrentStatus,
                    Photo = animal.Photo
                };

                await ApiService.PutAsync<AnimalResponse>($"animals/{animal.Id}", request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Animal MapFromApiResponse(AnimalResponse response)
        {
            return new Animal
            {
                Id = response.Id,
                Name = response.Name,
                Type = response.Type,
                Breed = response.Breed,
                DateOfBirth = response.DateOfBirth ?? DateTime.Today,
                IdEnclosure = response.IdEnclosure,
                IdGuardian = response.IdGuardian,
                CurrentStatus = response.CurrentStatus ?? "Доступен",
                Gender = response.Gender,
                Size = response.Size,
                Temperament = response.Temperament,
                Photo = response.Photo
            };
        }
    }
}
