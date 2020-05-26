using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookStoreAPI.Contracts;
using BookStoreAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _configuration;

        public UsersController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILoggerService logger, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// UserLogin Endpoint
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO user)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Login attempt from user {user.UserName}");
                var signIn = await _signInManager.PasswordSignInAsync(user.UserName,
                user.Password, false, false);

                if (signIn != null && signIn.Succeeded)
                {
                    _logger.LogInfo($"{location} : {user.UserName} successfully authenticated");
                    var loggedInUser = await _userManager.FindByNameAsync(user.UserName);
                    var generatedToken = await GenerateJSONWebToken(loggedInUser);

                    return Accepted("User logged in", new { token = generatedToken});
                }

                _logger.LogInfo($"{location} : {user.UserName} Not Authenticated");
                return Unauthorized(user);
            }
            catch (Exception ex)
            {
                return new Utils().ShowInternalServerError(ex, _logger);
            }
        }

        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            //assiging security key from appsettings.json
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            //hashing up security key
            var credentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            //assigning user info
            var claims = new List<Claim> 
            { 
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            //retrieving all user roles
            var roles = await _userManager.GetRolesAsync(user);

            //adding roles to user info
            claims.AddRange(roles.Select(role => 
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role
            )));

            //creating token based on user info - expiry time: 2 hours from issue time
            var token = new JwtSecurityToken(_configuration["Jwt:issuer"],
                _configuration["Jwt:issuer"], claims, null, expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

    }
}