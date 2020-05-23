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
    /// <summary>
    /// Interacts with the Authors in th eBook Store database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;

        public AuthorsController(IAuthorRepository authorRepository,
            ILoggerService logger)
        {
            _authorRepository = authorRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets All authors
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Get Authors requested");

                var authors = await _authorRepository.FindAll();

                _logger.LogInfo("Authors successfully retrieved");
                return Accepted(authors);
            }
            catch (Exception ex)
            {
                return new Utils().ShowInternalServerError(ex, _logger);
            }
        }

        /// <summary>
        /// Gets author based on requested ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>author</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            try
            {

                _logger.LogInfo("Get Author By Id requested");

                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"No Authors found for id {id}");
                    return NotFound("Author requested not found");
                }

                _logger.LogInfo("Author by id successfully retrieved");

                return Accepted(author);
            }
            catch (Exception ex)
            {
                return new Utils().ShowInternalServerError(ex, _logger);
            }
        }


        /// <summary>
        /// Basic data format:
        /// {
        ///     "firstName": "First Name",
        ///     "lastName": "Last Name",
        ///     "bio": "Biography"
        /// }
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Author author)
        {
            try
            {
                _logger.LogInfo("Create Author requested");
                if (author == null)
                {
                    _logger.LogWarn("Empty request for author creation");
                    return BadRequest(ModelState);
                }

                if(!ModelState.IsValid)
                {
                    _logger.LogError("Author data invalid");
                    return BadRequest(ModelState);
                }

                var isInsertionSuccessful = await _authorRepository.Create(author);
                if(!isInsertionSuccessful)
                {
                    return new Utils().ShowInternalServerError("Author creation failed", 
                        _logger);
                }

                _logger.LogInfo("Author successfully created");

                return Created("Authors Controller Create",  author);
            }
            catch (Exception ex)
            {
                return new Utils().ShowInternalServerError(ex, _logger);
            }
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Author author)
        {
            try
            {
                _logger.LogInfo($"Author id {author.Id} Update requested");
                if (author == null)
                {
                    _logger.LogWarn("Empty Author requested");
                    return BadRequest("Author not provided");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Author data");
                    return BadRequest(ModelState);
                }

                var isUpdateSusscessful = await _authorRepository.Update(author);
                if(!isUpdateSusscessful)
                {
                    return new Utils().ShowInternalServerError("Author update failed", _logger);
                }

                _logger.LogInfo("Author Update successful");

                return Created("Author Controller Update", author);
            }
            catch (Exception ex)
            {
                return new Utils().ShowInternalServerError(ex, _logger);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo($"Author id {id} Delete requested");
                if(id < 1)
                {
                    _logger.LogWarn("Invalid id requested");
                    return BadRequest("Invalid id requested");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Author data");
                    return BadRequest(ModelState);
                }

                var author = await _authorRepository.FindById(id);

                if(author == null)
                {
                    _logger.LogError("Requested Author not found");
                    return NotFound("Requested Author not found");
                }

                var isDeleteSuccessful = await _authorRepository.Delete(author);

                if (!isDeleteSuccessful)
                {
                    return new Utils().ShowInternalServerError("Author Delete failed", _logger);
                }

                _logger.LogInfo("Author delete successful");

                return Accepted("Author Controller Update", author);
                
            }
            catch (Exception ex)
            {
                return new Utils().ShowInternalServerError(ex, _logger);
            }
        }
    }
}