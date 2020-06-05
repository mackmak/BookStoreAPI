using Blazored.LocalStorage;
using BookStoreUI.Contracts;
using BookStoreUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Utils.Contracts;

namespace BookStoreUI.Services
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        private readonly IHttpClientFactory _client;
        private readonly ILocalStorageService _localStorage;
        private readonly ILoggerService _logger;
        public AuthorRepository(IHttpClientFactory client,
            ILocalStorageService localStorage,
            ILoggerService logger) :base(client, localStorage, logger)
        {
            _client = client;
            _localStorage = localStorage;
            _logger = logger;
        }



    }
}
