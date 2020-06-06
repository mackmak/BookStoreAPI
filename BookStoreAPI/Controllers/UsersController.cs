﻿using BookStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utils.Contracts;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UsersController : ApplicationController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public UsersController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILoggerService logger, IConfiguration configuration) : base(logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="requestUser"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("register")]//to avoid endpoint name conflicts due to the same http verb
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]User requestUser) 
        {
            try
            {
                var location = GetControllerActionNames();

                var newUser = new IdentityUser
                {
                    Email = requestUser.EmailAddress,
                    UserName = requestUser.EmailAddress
                };

                //creates a new user in the system
                var result = await _userManager.CreateAsync(newUser, requestUser.Password);

                if(!result.Succeeded)
                {
                    _logger.LogError($"{location}: {requestUser.EmailAddress} User registration attempt failed");

                    var errorMessage = new StringBuilder();

                    foreach (var error in result.Errors)
                    {
                        ShowInternalServerError($"{location}: {error.Code} {error.Description}");

                        errorMessage.AppendLine($"{location}: {error.Code} {error.Description}");
                    }

                    return ValidationProblem(new ValidationProblemDetails() 
                    { 
                        Detail = errorMessage.ToString() 
                    });
                }

                await _userManager.AddToRoleAsync(newUser, "Administrator");//Customer

                return Created("login", new { result.Succeeded });
            }
            catch (Exception ex)
            {
                return ShowInternalServerError(ex);
            }
        }

        /// <summary>
        /// UserLogin Endpoint
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("login")]//to avoid endpoint name conflicts due to the same http verb
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            try
            {
                var location = GetControllerActionNames();

                _logger.LogInfo($"{location}: Login attempt from user {user.EmailAddress}");
                var signIn = await _signInManager.PasswordSignInAsync(user.EmailAddress,
                user.Password, false, false);

                if (signIn != null && signIn.Succeeded)
                {
                    _logger.LogInfo($"{location} : {user.EmailAddress} successfully authenticated");
                    var loggedInUser = await _userManager.FindByNameAsync(user.EmailAddress);
                    var generatedToken = await GenerateJSONWebToken(loggedInUser);

                    return Accepted("User logged in", new { token = generatedToken});
                }

                _logger.LogInfo($"{location} : {user.EmailAddress} Not Authenticated");
                return Unauthorized(user);
            }
            catch (Exception ex)
            {
                return ShowInternalServerError(ex);
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