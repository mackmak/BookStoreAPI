﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BookStoreAPI.Contracts;

namespace BookStoreAPI.Controllers
{
    public class Utils : ControllerBase
    {

        public const string strAdmin = "Administrator";
        public const string strCustomer = "Customer";
        public ObjectResult ShowInternalServerError(Exception ex, ILoggerService logger)
        {
            logger.LogError(ex.Message);

            if (ex.InnerException != null)
                logger.LogError($"Inner Exception: {ex.InnerException}");

            var errorMsg = "Something went wrong, please contact the administrator.";
            return StatusCode(StatusCodes.Status500InternalServerError, errorMsg);
        }

        public ObjectResult ShowInternalServerError(string message, ILoggerService logger)
        {
            logger.LogError(message);

            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }
}
