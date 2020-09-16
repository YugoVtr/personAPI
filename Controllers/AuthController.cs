
using Microsoft.AspNetCore.Mvc;
using Globaltec.Models;
using Globaltec.Services;
using Globaltec.Repositories;
using Microsoft.AspNetCore.Authorization;
namespace Globaltec.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public ActionResult<dynamic> Authenticate([FromBody] User model)
        {
            var user = UserRepository.Get(model.Username, model.Password);
            if (user == null) return NotFound(new { message = "Invalid user or password" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";

            return new
            {
                user = user,
                token = token
            };
        }
    }
}
