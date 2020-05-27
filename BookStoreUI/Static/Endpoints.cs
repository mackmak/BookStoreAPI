using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Static
{
    public static class Endpoints
    {
        //TODO: grab environment from configuration file

        private readonly static string BaseUrl = GetBaseUrl();



        public static string AuthorsEndpoint = $"{BaseUrl}api/authors/";
        public static string BooksEndpoint = $"{BaseUrl}api/books/";
        public static string RegisterEndpoint = $"{BaseUrl}api/users/register/";
        public static string LoginEndpoint = $"{BaseUrl}api/users/login/";


        private static string GetBaseUrl()
        {
            switch (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            {
                case "Development":
                    return "https://localhost:44375/";
                case "Production":
                    return "http://bookstore.com/";
                default:
                    return "https://localhost:44375/"; ;
            }
        }

    }
}
