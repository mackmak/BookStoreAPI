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

namespace BookStoreUI.Services
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory _client;
        public BaseRepository(IHttpClientFactory client)
        {
            _client = client;
        }
        public async Task<bool> Create(string url, T entity)
        {

            if(entity == null)
            {
                return false;
            }

            //method is POST as this is an Insertion method
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            //serializing entity into JSON format
            request.Content = new StringContent(JsonConvert.SerializeObject(entity));

            var client = _client.CreateClient();

            //response based on request result
            HttpResponseMessage httpResponse = await client.SendAsync(request);

            //case request result == Created
            if(httpResponse.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if(id < 1)
            {
                return false;
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, url);

            var client = _client.CreateClient();

            //response based on request result
            HttpResponseMessage httpResponse = await client.SendAsync(request);

            //case request result == Delete
            if(httpResponse.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;
        }

        public async Task<T> Get(string url, int id)
        {
            if(id < 1)
            {
                return null;
            }

            //notice that id is being concatenated to the url
            var request = new HttpRequestMessage(HttpMethod.Get, url + id);

            var client = _client.CreateClient();

            //response based on request result
            HttpResponseMessage httpResponse = await client.SendAsync(request);

            if(httpResponse.StatusCode == HttpStatusCode.OK)
            {
                //reading content from request attached to response
                var content = await httpResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }

            return null;

        }

        public async Task<IList<T>> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _client.CreateClient();

            //response based on request result
            HttpResponseMessage httpResponse = await client.SendAsync(request);

            if(httpResponse.StatusCode == HttpStatusCode.OK)
            {
                //reading content from request attached to response
                var content = await httpResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<T>>(content);
            }

            return null;
        }

        public async Task<bool> Update(string url, T entity)
        {
            if(entity == null)
            {
                return false;
            }

            var request = new HttpRequestMessage(HttpMethod.Put, url);

            //acquiring entity to be updated
            request.Content = new StringContent(JsonConvert.SerializeObject(entity),
                Encoding.UTF8, "application/json");


            var client = _client.CreateClient();

            //response based on request result
            HttpResponseMessage httpResponse = await client.SendAsync(request);

            if(httpResponse.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;
        }
    }
}
