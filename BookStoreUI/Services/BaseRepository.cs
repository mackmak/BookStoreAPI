using BookStoreUI.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Utils.Contracts;

namespace BookStoreUI.Services
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory _client;
        private readonly ILocalStorageService _localStorage;
        private readonly ILoggerService _logger;
        public BaseRepository(IHttpClientFactory client,
            ILocalStorageService localStorage, ILoggerService logger)
        {
            _client = client;
            _localStorage = localStorage;
            _logger = logger;
        }
        public async Task<bool> Create(string url, T entity)
        {
            try
            {


                if (entity == null)
                {
                    return false;
                }

                //method is POST as this is an Insertion method
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                //serializing entity into JSON format
                request.Content = new StringContent(JsonConvert.SerializeObject(entity));

                var client = _client.CreateClient();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", await GetBearerToken());

                //response based on request result
                HttpResponseMessage httpResponse = await client.SendAsync(request);

                //case request result == Created
                if (httpResponse.StatusCode == HttpStatusCode.Created)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            try
            {


                if (id < 1)
                {
                    return false;
                }

                var request = new HttpRequestMessage(HttpMethod.Delete, url);

                var client = _client.CreateClient();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", await GetBearerToken());

                //response based on request result
                HttpResponseMessage httpResponse = await client.SendAsync(request);

                //case request result == Delete
                if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return false;
        }

        public async Task<T> Get(string url, int id)
        {
            try
            {


                if (id < 1)
                {
                    return null;
                }

                //notice that id is being concatenated to the url
                var request = new HttpRequestMessage(HttpMethod.Get, url + id);

                var client = _client.CreateClient();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", await GetBearerToken());

                //response based on request result
                HttpResponseMessage httpResponse = await client.SendAsync(request);

                if (httpResponse.StatusCode == HttpStatusCode.Accepted)
                {
                    //reading content from request attached to response
                    var content = await httpResponse.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return null;

        }

        public async Task<IList<T>> Get(string url)
        {
            try
            {


                var request = new HttpRequestMessage(HttpMethod.Get, url);

                var client = _client.CreateClient();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", await GetBearerToken());

                //response based on request result
                HttpResponseMessage httpResponse = await client.SendAsync(request);

                if (httpResponse.StatusCode == HttpStatusCode.Accepted)
                {
                    //reading content from request attached to response
                    var content = await httpResponse.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IList<T>>(content);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }


        public async Task<bool> Update(string url, T entity, int id)
        {
            try
            {
                if (entity == null)
                {
                    return false;
                }

                var request = new HttpRequestMessage(HttpMethod.Put, url + id);

                //acquiring entity to be updated
                request.Content = new StringContent(JsonConvert.SerializeObject(entity),
                    Encoding.UTF8, "application/json");

                var client = _client.CreateClient();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", await GetBearerToken());

                //response based on request result
                HttpResponseMessage httpResponse = await client.SendAsync(request);

                if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return false;
        }

        private async Task<string> GetBearerToken()
        {
            string tokenName = "authToken";
            return await _localStorage.GetItemAsync<string>(tokenName);
        }


        public void LogError(Exception ex)
        {
            _logger.LogError(ex.Message);

            if (ex.InnerException != null)
                _logger.LogError($"Inner Exception: {ex.InnerException}");
        }
    }
}
