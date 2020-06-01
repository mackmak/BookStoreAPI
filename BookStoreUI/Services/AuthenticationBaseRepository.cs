using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Contracts;

namespace BookStoreUI.Services
{
    public class AuthenticationBaseRepository
    {
        protected readonly ILoggerService _logger;
        public AuthenticationBaseRepository(ILoggerService logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex)
        {
            _logger.LogError(ex.Message);

            if (ex.InnerException != null)
                _logger.LogError($"Inner Exception: {ex.InnerException}");

        }
    }
}
