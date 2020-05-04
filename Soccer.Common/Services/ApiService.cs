using Newtonsoft.Json;
using Plugin.Connectivity;
using Soccer.Common.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Soccer.Common.Services
{
    public class ApiService : IApiService
    {
        public async Task<bool> CheckConnectionAsync(string url)
        {
            if (!CrossConnectivity.Current.IsConnected) //valida si en modo aviòn tiene datos
            {
                return false;
            }

            return await CrossConnectivity.Current.IsRemoteReachable(url); //ping a la url
        }


        public async Task<Response> GetListAsync<T>(string urlBase, string servicePrefix, string controller)
        {
			try
			{
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase) //BaseAddress tiene la direcciòn de la urlbase
                };

                string url = $"{servicePrefix}{controller}"; //armamos la peticiòn
                HttpResponseMessage response = await client.GetAsync(url);//realiza un GetAsync a la peticiòn url
                string result = await response.Content.ReadAsStringAsync(); //ReadAsStringAsync la lee como cadena

                if (!response.IsSuccessStatusCode) //si no es success
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                List<T> list = JsonConvert.DeserializeObject<List<T>>(result); //deserializa como una lista genèrica

                return new Response
                {
                    IsSuccess = true,
                    Result = list
                };
            }
			catch (Exception ex)
			{
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
			}
        }

        public async Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request)
        {
            try
            {
                //serialize el requerimiento y lo codifica en UTF8
                string requeststring = JsonConvert.SerializeObject(request);
                StringContent content = new StringContent(requeststring, Encoding.UTF8, "application/json");

                //realiza la petición al postman
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

                string url = $"{servicePrefix}{controller}";
                HttpResponseMessage response = await client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = token
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> GetUserByEmail(string urlBase,
                                                   string servicePrefix,
                                                   string controller,
                                                   string tokenType,
                                                   string accessToken,
                                                   EmailRequest request)
        {
            try
            {
                string requeststring = JsonConvert.SerializeObject(request);
                StringContent content = new StringContent(requeststring, Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                string url = $"{servicePrefix}{controller}";
                HttpResponseMessage response = await client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = userResponse
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> RegisterUserAsync(string urlBase, string servicePrefix, string controller, UserRequest userRequest)
        {
            try
            {
                string request = JsonConvert.SerializeObject(userRequest);
                StringContent content = new StringContent(request, Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

                string url = $"{servicePrefix}{controller}";
                HttpResponseMessage response = await client.PostAsync(url, content);
                string answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = answer
                    };
                }

                Response obj = JsonConvert.DeserializeObject<Response>(answer);

                return obj;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
