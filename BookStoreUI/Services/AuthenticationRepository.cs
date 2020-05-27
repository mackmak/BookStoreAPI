using BookStoreUI.Contracts;
using BookStoreUI.Models;
using BookStoreUI.Static;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUI.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;


        public AuthenticationRepository(IHttpClientFactory client)
        {
            _client = client;
        }
        public async Task<bool> Register(RegistrationModel user)
        {
            try
            {
                var url = Endpoints.RegisterEndpoint;

                //setting up register request to the server
                var request = new HttpRequestMessage(HttpMethod.Post,
                    url);

                //assigning user information to the request
                request.Content = new StringContent(JsonConvert.SerializeObject(user),
                    Encoding.UTF8, "application/json");

                var client = _client.CreateClient();

                //response based on request result
                HttpResponseMessage httpResponse = await client.SendAsync(request);

                return httpResponse.IsSuccessStatusCode;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
