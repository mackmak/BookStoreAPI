using Blazored.LocalStorage;
using BookStoreUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Contracts
{
    public interface IAuthenticationRepository
    {

         Task<bool> Register(RegistrationModel user);

         Task<bool> Login(LoginModel user);

         Task Logout();
    }
}
