using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using static WebApplication1.Dto.DtoAuth;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("auth/[controller]")]
    public class AuthController:Controller
    {
        private readonly IUsersService usersService;
        private readonly ILogger<AuthController> logger;

        public AuthController(IUsersService usersService, ILogger<AuthController> logger)
        {
            this.usersService = usersService;
            this.logger = logger;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var result = await usersService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserCreateDto>> Register([FromBody] User user)
        {
            try
            {
                var u = await usersService.CreateUserAsync(user);
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
