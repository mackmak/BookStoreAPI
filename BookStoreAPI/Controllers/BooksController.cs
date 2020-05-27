using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreAPI.Contracts;
using BookStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ApplicationController
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository, 
            ILoggerService logger) : base(logger)
        {
            _bookRepository = bookRepository;
        }


        /// <summary>
        /// Gets All Books
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                _logger.LogInfo("Get All Books requested");

                var books = await _bookRepository.FindAll();

                _logger.LogInfo("Books successfully retrieved");

                return Accepted("Books Controller Accepted", books);
            }
            catch (Exception ex)
            {
                return ShowInternalServerError(ex);
            }
        }

        /// <summary>
        /// Gets a Book based on provided ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            try
            {
                _logger.LogInfo("Get Book By Id requested");

                if(id < 1)
                {
                    _logger.LogWarn($"Invalid ID {id} provided for Book");
                    BadRequest($"Invalid ID {id} provided for Book");
                }

                var book = await _bookRepository.FindById(id);

                if(book == null)
                {
                    _logger.LogError($"No Book found with id {id}");
                    NotFound($"No Book found with id {id}");
                }

                _logger.LogInfo("Book successfully retrieved");


                return Accepted("Book successfully retrieved", book);
            }
            catch (Exception ex)
            {
                return ShowInternalServerError(ex);
            }
        }


        /// <summary>
        /// Updates the provided Book
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Book book)
        {
            try
            {
                _logger.LogInfo($"Book Update requested");
                if (book == null)
                {
                    _logger.LogWarn("Empty Book requested");
                    return BadRequest("Empty Book requested");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Book data");
                    return BadRequest(ModelState);
                }

                var isUpdateSuccessful = await _bookRepository.Update(book);

                if (!isUpdateSuccessful)
                {
                    return ShowInternalServerError("Book update failed");
                }

                _logger.LogInfo("Book update successful");

                return Accepted("Book Update", book);
            }
            catch (Exception ex)
            {
                return ShowInternalServerError(ex);
            }
        }


        /// <summary>
        /// Inserts the provided Book into the database
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            try
            {
                _logger.LogInfo("Create Book requested");
                if (book == null)
                {
                    _logger.LogWarn("Empty request for Book creation");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Book data invalid");
                    return BadRequest(ModelState);
                }

                var isCreationSuccessful = await _bookRepository.Create(book);

                if (!isCreationSuccessful)
                {
                    return ShowInternalServerError("Book create failed");
                }

                _logger.LogInfo("Book successfully created");

                return Created("Book Create", book);
            }
            catch (Exception ex)
            {
                return ShowInternalServerError(ex);
            }
        }

        /// <summary>
        /// Removes the requested Book from database based on ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo($"Book id {id} Delete requested");
                if (id < 1)
                {
                    _logger.LogWarn($"Invalid ID requested: {id}");
                    BadRequest($"Invalid ID requested: {id}");
                }


                var book = await _bookRepository.FindById(id);

                if(book == null)
                {
                    _logger.LogError($"Requested Book ID not found: {id}");
                    return NotFound($"Requested Book ID not found: {id}");
                }

                var isDeleteSuccessful = await _bookRepository.Delete(book);

                if (!isDeleteSuccessful)
                {
                    return ShowInternalServerError("Book Delete failed");
                }

                _logger.LogInfo($"Book id {id} succcessfully deleted");

                return Accepted("Book Controller Update", book);
            }
            catch (Exception ex)
            {
                return ShowInternalServerError(ex);
            }

        }
    }
}
