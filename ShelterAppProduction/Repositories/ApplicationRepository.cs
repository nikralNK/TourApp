using ShelterAppProduction.Models;
using ShelterAppProduction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShelterAppProduction.Repositories
{
    public class ApplicationRepository
    {
        private readonly GuardianRepository _guardianRepository;

        public ApplicationRepository()
        {
            _guardianRepository = new GuardianRepository();
        }

        public async Task CreateApplication(int animalId, string guardianFullName, string guardianPhone, string guardianEmail, string comments)
        {
            try
            {
                var guardianId = await _guardianRepository.GetOrCreateGuardian(guardianFullName, guardianPhone, guardianEmail);

                var request = new ApplicationCreateRequest
                {
                    IdAnimal = animalId,
                    IdGuardian = guardianId,
                    Comments = comments
                };

                await ApiService.PostAsync<ApplicationResponse>("applications/", request);
            }
            catch
            {
            }
        }

        public async Task<List<Application>> GetAllApplications()
        {
            try
            {
                var response = await ApiService.GetAsync<List<ApplicationResponse>>("applications/");
                var guardians = await _guardianRepository.GetAllGuardians();

                return response.Select(app =>
                {
                    var guardian = guardians.FirstOrDefault(g => g.Id == app.IdGuardian);
                    var fullName = guardian?.FullName ?? "";
                    var shortName = GetShortName(fullName);

                    return new Application
                    {
                        Id = app.Id,
                        IdAnimal = app.IdAnimal,
                        IdGuardian = app.IdGuardian,
                        ApplicationDate = app.ApplicationDate,
                        Status = app.Status ?? "На рассмотрении",
                        Comments = app.Comments,
                        GuardianShortName = shortName
                    };
                }).ToList();
            }
            catch
            {
                return new List<Application>();
            }
        }

        private string GetShortName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "Не указано";

            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return "Не указано";
            if (parts.Length == 1)
                return parts[0];
            if (parts.Length == 2)
                return $"{parts[0]} {parts[1][0]}.";

            return $"{parts[0]} {parts[1][0]}. {parts[2][0]}.";
        }

        public async Task UpdateApplicationStatus(int applicationId, string status)
        {
            try
            {
                var request = new ApplicationUpdateRequest
                {
                    Status = status
                };

                await ApiService.PutAsync<ApplicationResponse>($"applications/{applicationId}", request);
            }
            catch
            {
            }
        }

        public async Task<Application> GetApplicationByEmailAndAnimal(string email, int animalId)
        {
            try
            {
                var guardians = await _guardianRepository.GetAllGuardians();
                var guardian = guardians.FirstOrDefault(g => g.Email == email);

                if (guardian == null)
                    return null;

                var applications = await ApiService.GetAsync<List<ApplicationResponse>>("applications/");

                var application = applications
                    .Where(a => a.IdGuardian == guardian.Id && a.IdAnimal == animalId)
                    .OrderByDescending(a => a.ApplicationDate)
                    .FirstOrDefault();

                if (application == null)
                    return null;

                return new Application
                {
                    Id = application.Id,
                    IdAnimal = application.IdAnimal,
                    IdGuardian = application.IdGuardian,
                    ApplicationDate = application.ApplicationDate,
                    Status = application.Status ?? "На рассмотрении",
                    Comments = application.Comments
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
