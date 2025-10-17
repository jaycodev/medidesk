using Shared.DTOs.Account.Requests;
using Shared.DTOs.Account.Responses;
using Web.Models.Account;

namespace Web.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _http;

        public AccountService(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("ApiClient");
        }

        public async Task<(bool Success, LoggedUserResponse? User, string Message)> LoginAsync(LoginRequest request)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("api/account/login", request);

                if (!resp.IsSuccessStatusCode)
                {
                    string content = await resp.Content.ReadAsStringAsync();
                    var message = string.IsNullOrWhiteSpace(content) ? "Correo y/o contraseña incorrectos" : content;
                    return (false, null, message);
                }

                var apiUser = await resp.Content.ReadFromJsonAsync<LoggedUserResponse>();
                if (apiUser == null)
                    return (false, null, "Error al procesar el login");

                return (true, apiUser, "Login correcto");
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
