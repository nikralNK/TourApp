using ShelterAppProduction.Models;
using ShelterAppProduction.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShelterAppProduction.Repositories
{
    public class VeterinarianRepository
    {
        public async Task<List<Veterinarian>> GetAll()
        {
            try
            {
                var response = await ApiService.GetAsync<List<VeterinarianResponse>>("veterinarians/");
                return response.Select(MapFromApiResponse).ToList();
            }
            catch
            {
                return new List<Veterinarian>();
            }
        }

        public async Task<Veterinarian> GetById(int id)
        {
            try
            {
                var response = await ApiService.GetAsync<VeterinarianResponse>($"veterinarians/{id}");
                return MapFromApiResponse(response);
            }
            catch
            {
                return null;
            }
        }

        public async Task<Veterinarian> AddVeterinarian(string username, string password, string email, string fullName, string specialization, string phoneNumber, string licenseNumber)
        {
            try
            {
                var request = new VeterinarianCreateRequest
                {
                    Username = username,
                    Password = password,
                    Email = email,
                    Fullname = fullName,
                    Specialization = specialization,
                    Phonenumber = phoneNumber,
                    Licensenumber = licenseNumber
                };

                var response = await ApiService.PostAsync<VeterinarianResponse>("veterinarians/", request);
                return MapFromApiResponse(response);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateVeterinarian(int id, string fullName, string specialization, string phoneNumber, string licenseNumber)
        {
            try
            {
                var request = new VeterinarianUpdateRequest
                {
                    Fullname = fullName,
                    Specialization = specialization,
                    Phonenumber = phoneNumber,
                    Licensenumber = licenseNumber
                };

                await ApiService.PutAsync<VeterinarianResponse>($"veterinarians/{id}", request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteVeterinarian(int id)
        {
            try
            {
                await ApiService.DeleteAsync($"veterinarians/{id}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Veterinarian MapFromApiResponse(VeterinarianResponse response)
        {
            return new Veterinarian
            {
                Id = response.Id,
                FullName = response.Fullname,
                Specialization = response.Specialization,
                PhoneNumber = response.Phonenumber,
                LicenseNumber = response.Licensenumber,
                UserId = response.Iduser
            };
        }
    }
}
