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
using Utils.Contracts;

namespace BookStoreUI.Services
{
    public class AuthenticationRepository : AuthenticationBaseRepository, 
        IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;
        private static readonly string _applicationTypeJson = "application/json";
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private static string tokenName = "authToken";

        public AuthenticationRepository(IHttpClientFactory client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider,
            ILoggerService logger):base(logger)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            try
            {
                var url = Endpoints.LoginEndpoint;

                //setting up Login request 
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
                var token = JsonConvert.DeserializeObject<TokenResponse>(content);

                //Store Token
                await _localStorage.SetItemAsync(tokenName, token.Token);

                //Change auth state
                await ((ApiAuthenticationStateProvider)_authenticationStateProvider)
                .Login();

                /*client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", token.Token);*/


                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return false;
        }

        public async Task Logout()
        {
            try
            {
                //removing token from storage
                await _localStorage.RemoveItemAsync(tokenName);


                var apiAuthenticationStateProvider =
                   (ApiAuthenticationStateProvider)_authenticationStateProvider;

                apiAuthenticationStateProvider.Logout();
            }
            catch (Exception ex)
            {
                LogError(ex); 
            }
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            try
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
            catch (Exception ex)
            {
                LogError(ex);
            }

            return false;
        }
    }
}
