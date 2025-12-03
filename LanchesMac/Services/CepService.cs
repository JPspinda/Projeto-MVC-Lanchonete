
using LanchesMac.Models;
using System.Text.Json;

namespace LanchesMac.Services
{
    public class CepService : ICepService
    {
        private readonly HttpClient _httpClient;

        public CepService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ViaCepResponse> GetCepByAPI(string cep)
        {
            var url = $@"https://viacep.com.br/ws/{cep}/json/";
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);

                var responseObj = JsonSerializer.Deserialize<ViaCepResponse>(responseBody);

                return responseObj;
            }
            catch (Exception ex)
            {
                return new ViaCepResponse()
                {
                    Message = $"Erro ao obter o CEP: {ex.Message}"
                };
            }
        }
    }
}
