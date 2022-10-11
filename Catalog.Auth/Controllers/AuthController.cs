namespace Catalog.Auth.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuth _auth;
        private readonly IArgonService argonService;
        private readonly AuthContext authContext;
        
        public AuthController(ILogger<AuthController> logger, IAuth auth, IArgonService argonService, AuthContext authContext)
        {
            _logger = logger;
            _auth = auth;
            this.argonService = argonService;
            this.authContext = authContext;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var login = _auth.Authenticate(model.Email, model.Password);
            if (login is null)
            {
                return Unauthorized(new
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
            var saltHash = argonService.CreateHashAndSalt(model.Password);

            authContext.Users.Add(new User
            {
                Id = Ulid.NewUlid(),
                Email = model.Email,
                Fullname = model.Fullname,
                Salt = saltHash.Salt,
                Password = saltHash.Hash,
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