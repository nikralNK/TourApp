using ShelterAppProduction.Models;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ShelterAppProduction.Services
{
    public class AuthService
    {
        public async Task<User> Login(string username, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Username = username,
                    Password = password
                };

                var response = await ApiService.PostAsync<LoginResponse>("auth/login", loginRequest);

                ApiService.SetAuthToken(response.AccessToken);

                var userResponse = await ApiService.GetAsync<UserResponse>("users/me");

                var user = new User
                {
                    Id = userResponse.Id,
                    Username = userResponse.Username,
                    Email = userResponse.Email,
                    FullName = userResponse.Fullname,
                    Role = userResponse.Role,
                    Avatar = userResponse.Avatar
                };

                SessionManager.CurrentUser = user;
                return user;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<bool> Register(string username, string password, string email, string fullName)
        {
            try
            {
                var registerRequest = new UserRegisterRequest
                {
                    Username = username,
                    Password = password,
                    Email = email,
                    Fullname = fullName
                };

                await ApiService.PostAsync<UserResponse>("auth/register", registerRequest);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> UpdateProfile(int userId, string fullName, string avatar)
        {
            try
            {
                var updateRequest = new UserUpdateRequest
                {
                    Fullname = fullName,
                    Avatar = avatar
                };

                var userResponse = await ApiService.PutAsync<UserResponse>("users/me", updateRequest);

                if (SessionManager.CurrentUser != null && SessionManager.CurrentUser.Id == userId)
                {
                    SessionManager.CurrentUser.FullName = userResponse.Fullname;
                    SessionManager.CurrentUser.Avatar = userResponse.Avatar;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении профиля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task Logout()
        {
            try
            {
                var token = ApiService.GetAuthToken();
                if (!string.IsNullOrEmpty(token))
                {
                    await ApiService.PostAsync<object>("auth/logout", new { token });
                }
            }
            catch
            {
            }
            finally
            {
                ApiService.SetAuthToken(null);
                SessionManager.CurrentUser = null;
            }
        }
    }
}
