using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("useres/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> logger;
        private readonly IUsersService usersService;

        public UserController(ILogger<UserController> logger, IUsersService usersService)
        {
            this.logger = logger;
            this.usersService = usersService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DtoUser>>> GetAllUsers()
        {
            var users = await usersService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]//------לבדוק בכל השאר 
        public async Task<ActionResult<DtoUser>> GetUserById(int id)
        {
            try
            {
                var user = await usersService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("basket/{id}")]
        public async Task<List<Basket>> Basket(int id)
        {
            if (id == null)
            {
               
            }
            var basket = await usersService.Basket(id);
            return basket;
        }

    }
}
