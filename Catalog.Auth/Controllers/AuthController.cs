namespace Catalog.Auth.Controllers
{
    using Catalog.Auth.Extensions;
    using Catalog.Auth.Infrastructure;
    using Catalog.Auth.Model;
    using Catalog.Auth.Services;
    using Catalog.Auth.ViewModel;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuth _auth;

        private readonly AuthContext authContext;

        public AuthController(ILogger<AuthController> logger, IAuth auth, AuthContext authContext)
        {
            _logger = logger;
            _auth = auth;
            this.authContext = authContext;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var login = await _auth.Authenticate(model.Email, model.Password);
            if (login is null)
            {
                return BadRequest(new
                {
                    Succeeded = false,
                    Message = "User not found"
                });
            }
            return Ok(new
            {
                Result = login,
                Succeeded = true,
                Message = "User logged in successfully"
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {

            authContext.Users.Add(new User
            {
                Id = Ulid.NewUlid(),
                Email = model.Email,
                Fullname = model.Fullname,
                Password = model.Password.Hash(),

            });
            
            await authContext.SaveChangesAsync();

            return Ok(new
            {
                Succeeded = true,
                Message = "User created successfully"
            });
        }       
    }
}