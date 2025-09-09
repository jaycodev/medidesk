using Newtonsoft.Json;
using Shared.DTOs.Account.Requests;
using Web.Models.Account;

namespace Web.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _http;

        public ProfileService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _http.PutAsync($"api/account/{model.UserId}/password", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProfilePictureAsync(int userId, string pictureUrl)
        {
            var userUpdate = new { ProfilePictureUrl = pictureUrl };
            using var updateContent = new StringContent(JsonConvert.SerializeObject(userUpdate), System.Text.Encoding.UTF8, "application/json");

            var response = await _http.PutAsync($"api/account/{userId}/profile-picture", updateContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProfileAsync(int userId, string phone)
        {
            var request = new UpdateProfileRequest
            {
                Phone = phone
            };

            var json = JsonConvert.SerializeObject(request);
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _http.PutAsync($"api/account/{userId}/profile", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> UploadProfilePictureAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            using var content = new MultipartFormDataContent
            {
                { new StreamContent(file.OpenReadStream()), "file", file.FileName },
                { new StringContent("MedicalAppointmentsDB/UserPhotos"), "folder" },
                { new StringContent(file.FileName), "fileName" }
            };

            var response = await _http.PostAsync("api/cloudinary/upload", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json)!;
            return result["url"];
        }
    }
}
