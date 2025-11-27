using LanchesMac.Models;

namespace LanchesMac.Services
{
    public interface ICepService
    {
        public Task<ViaCepResponse> GetCepByAPI(string cep);
    }
}
