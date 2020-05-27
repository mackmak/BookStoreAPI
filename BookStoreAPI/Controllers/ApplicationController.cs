using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BookStoreAPI.Contracts;

namespace BookStoreAPI.Controllers
{
    public class ApplicationController : ControllerBase
    {

        protected const string strAdmin = "Administrator";
        protected const string strCustomer = "Customer";
        protected readonly ILoggerService _logger;

        public ApplicationController(ILoggerService logger)
        {
            _logger = logger;
        }

        public ObjectResult ShowInternalServerError(Exception ex)
        {
            _logger.LogError(ex.Message);

            if (ex.InnerException != null)
                _logger.LogError($"Inner Exception: {ex.InnerException}");

            var errorMsg = "Something went wrong, please contact the administrator.";
            return StatusCode(StatusCodes.Status500InternalServerError, errorMsg);
        }

        public ObjectResult ShowInternalServerError(string message)
        {
            _logger.LogError(message);

            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }
}
