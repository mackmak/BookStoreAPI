using Blazored.LocalStorage;
using BookStoreUI.Contracts;
using BookStoreUI.Models;
using BookStoreUI.Providers;
using BookStoreUI.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUI.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;
        private static readonly string _applicationTypeJson = "application/json";
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private static string tokenName = "authToken";

        public AuthenticationRepository(IHttpClientFactory client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var url = Endpoints.LoginEndpoint;

            //settiing up Login request 
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            //assigning user info to the request
            request.Content = new StringContent(JsonConvert.SerializeObject(user),
                Encoding.UTF8, _applicationTypeJson);

            var client = _client.CreateClient();

            //response based on request result
            HttpResponseMessage httpResponse = await client.SendAsync(request);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await httpResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<string>(content);

            //Store Token
            await _localStorage.SetItemAsync(tokenName, token);

            //Change auth state
            var apiAuthenticationStateProvider =
               (ApiAuthenticationStateProvider)_authenticationStateProvider;

            //Log user in
            await apiAuthenticationStateProvider.Login();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", token);
            return true;


        }

        public async Task Logout()
        {
            //removing token from storage
            await _localStorage.RemoveItemAsync(tokenName);


            var apiAuthenticationStateProvider =
               (ApiAuthenticationStateProvider)_authenticationStateProvider;

            apiAuthenticationStateProvider.Logout();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var url = Endpoints.RegisterEndpoint;

            //setting up register request 
            var request = new HttpRequestMessage(HttpMethod.Post,
                url);

            //assigning user information to the request
            request.Content = new StringContent(JsonConvert.SerializeObject(user),
                Encoding.UTF8, _applicationTypeJson);

            var client = _client.CreateClient();

            //response based on request result
            HttpResponseMessage httpResponse = await client.SendAsync(request);

            return httpResponse.IsSuccessStatusCode;

        }
    }
}
