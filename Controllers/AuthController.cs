
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Globaltec.Models;
using Globaltec.Services;
using Globaltec.Repositories;
using Microsoft.AspNetCore.Http;

namespace Globaltec.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public ITokenService TokenService { get; }
        public AuthController(ITokenService tokenService)
        {
            TokenService = tokenService;
        }


        /// <summary>
        /// authentication users
        /// </summary>
        /// <remarks>
        /// This request generates the token to access the other endpoints. Sample request:
        ///
        ///     {
        ///        "username": "admin",
        ///        "password": "admin"
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<dynamic> Authenticate([FromBody] User model)
        {
            var user = UserRepository.Get(model.Username, model.Password);
            if (user == null) return NotFound(new { message = "Invalid user or password" });

            var token = TokenService.generateToken(user);
            user.Password = "";

            return new
            {
                user = user,
                token = "Bearer " + token
            };
        }
    }
}
