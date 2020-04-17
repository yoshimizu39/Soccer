using Newtonsoft.Json;
using Soccer.Common.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Soccer.Common.Services
{
    public class ApiService : IApiService
    {
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
    }
}
