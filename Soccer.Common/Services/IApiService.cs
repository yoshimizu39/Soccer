using Soccer.Common.Models;
using System.Threading.Tasks;

namespace Soccer.Common.Services
{
    public interface IApiService
    {
        Task<Response> GetPredictionsForUserAsync(string urlBase, string servicePrefix, string controller, PredictionForUserRequest predictionsForUserRequest, string tokenType, string accessToken);
        Task<Response> MakePredictionAsync(string urlBase, string servicePrefix, string controller, PredictionRequest predictionRequest, string tokenType, string accessToken);
        Task<Response> ChangePasswordAsync(string urlBase, string servicePrefix, string controller, ChangePasswordRequest changePasswordRequest, string tokenType, string accessToken);

        Task<Response> PutAsync<T>(string urlBase, string servicePrefix, string controller, T model, string tokenType, string accessToken);

        Task<Response> RecoverPasswordAsync(string urlBase, string servicePrefix, string controller, EmailRequest emailRequest);

        Task<Response> RegisterUserAsync(string urlBase, string servicePrefix, string controller, UserRequest userRequest);

        Task<Response> GetListAsync<T>(string urlBase, string servicePrefix, string controller);

        Task<bool> CheckConnectionAsync(string url);

        //enviamos un TokenRequest para ver si es correcto o nos devuelve un error
        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);
        
        //devuelve el email que enviò la aplicación        
        Task<Response> GetUserByEmail(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, EmailRequest request);

    }
}
