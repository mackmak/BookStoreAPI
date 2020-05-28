using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStoreUI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        private string tokenName = "authToken";


        public ApiAuthenticationStateProvider(ILocalStorageService localStorage,
            JwtSecurityTokenHandler tokenHandler)
        {
            _localStorage = localStorage;
            _tokenHandler = tokenHandler;
        }

        private AuthenticationState GenerateNewToken()
        {
            return new AuthenticationState(
                        new ClaimsPrincipal(
                            new ClaimsIdentity()));
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedToken = await _localStorage.GetItemAsync<string>(tokenName);

                if (string.IsNullOrWhiteSpace(savedToken))
                {
                    return GenerateNewToken();
                }
                var tokenContent = _tokenHandler.ReadJwtToken(savedToken);

                //token expired, generate a new one
                if (tokenContent.ValidTo < DateTime.Now)
                {
                    //removing expired token from storage
                    await _localStorage.RemoveItemAsync(tokenName);

                    return GenerateNewToken();
                }

                //create user based on claims from token
                var claims = ParseClaims(tokenContent);
                var user = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "jwt"));

                return new AuthenticationState(user);

            }
            catch (Exception ex)
            {
                return GenerateNewToken();
            }
        }

        public async Task Login()
        {
            var savedToken = await _localStorage.GetItemAsync<string>(tokenName);

            var tokenContent = _tokenHandler.ReadJwtToken(savedToken);

            //create user based on claims from token
            var claims = ParseClaims(tokenContent);

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));

            NotifyAuthenticationStateChanged(authState);

        }

        public void Logout()
        {
            var emptyUser = new ClaimsPrincipal(new ClaimsIdentity());

            var authState = Task.FromResult(new AuthenticationState(emptyUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList();

            //adding the subject claim to the end of the list
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));

            return claims;
        }
    }
}
