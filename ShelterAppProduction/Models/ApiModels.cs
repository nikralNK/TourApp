using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShelterAppProduction.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }

    public class UserRegisterRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }
    }

    public class UserResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }

    public class UserUpdateRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }

    public class AnimalResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("breed")]
        public string Breed { get; set; }

        [JsonPropertyName("dateofbirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("temperament")]
        public string Temperament { get; set; }

        [JsonPropertyName("idenclosure")]
        public int? IdEnclosure { get; set; }

        [JsonPropertyName("idguardian")]
        public int? IdGuardian { get; set; }

        [JsonPropertyName("currentstatus")]
        public string CurrentStatus { get; set; }

        [JsonPropertyName("photo")]
        public string Photo { get; set; }
    }

    public class AnimalCreateRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("breed")]
        public string Breed { get; set; }

        [JsonPropertyName("dateofbirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("temperament")]
        public string Temperament { get; set; }

        [JsonPropertyName("idenclosure")]
        public int? IdEnclosure { get; set; }
    }

    public class AnimalUpdateRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("breed")]
        public string Breed { get; set; }

        [JsonPropertyName("dateofbirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("temperament")]
        public string Temperament { get; set; }

        [JsonPropertyName("idenclosure")]
        public int? IdEnclosure { get; set; }

        [JsonPropertyName("idguardian")]
        public int? IdGuardian { get; set; }

        [JsonPropertyName("currentstatus")]
        public string CurrentStatus { get; set; }

        [JsonPropertyName("photo")]
        public string Photo { get; set; }
    }

    public class ApplicationResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idanimal")]
        public int IdAnimal { get; set; }

        [JsonPropertyName("idguardian")]
        public int IdGuardian { get; set; }

        [JsonPropertyName("applicationdate")]
        public DateTime ApplicationDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
    }

    public class ApplicationCreateRequest
    {
        [JsonPropertyName("idanimal")]
        public int IdAnimal { get; set; }

        [JsonPropertyName("idguardian")]
        public int IdGuardian { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
    }

    public class ApplicationUpdateRequest
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
    }

    public class FavoriteResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("iduser")]
        public int IdUser { get; set; }

        [JsonPropertyName("idanimal")]
        public int IdAnimal { get; set; }
    }

    public class FavoriteCreateRequest
    {
        [JsonPropertyName("idanimal")]
        public int IdAnimal { get; set; }
    }
}
