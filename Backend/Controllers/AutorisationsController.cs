using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorisationsController : ControllerBase
    {
        private readonly Startup.AppSettings _appSettings;
        private readonly UserManager<Models.DBUser> _userManager;
        private readonly SignInManager<Models.DBUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AutorisationsController> _logger;

        public AutorisationsController(IOptions<Startup.AppSettings> appSettings,
            UserManager<Models.DBUser> userManager,
            SignInManager<Models.DBUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AutorisationsController> logger)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost("InitDefault")]
        public async Task<ActionResult> InitDefault()
        {
            string[] roles = { Helpers.Roles.Admin,
                Helpers.Roles.User };
            foreach (var role in roles)
            {
                _logger.LogInformation(Helpers.LogEvents.GetItem,
                    $"Getting role {role}");
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogInformation(Helpers.LogEvents.InsertItem,
                        $"Insert role {role}");
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            _logger.LogInformation(Helpers.LogEvents.GetItem,
                    $"Getting user in role {Helpers.Roles.Admin}. Change password!");
            var admins = await _userManager.GetUsersInRoleAsync("admin");
            if (admins.Count == 0)
            {
                _logger.LogInformation(Helpers.LogEvents.InsertItem,
                        $"Insert user admin");
                var admin = new Models.DBUser
                {
                    UserName = "admin",
                    Email = "admin@admin",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(admin, "One_of_the_most_cunning_and_ruthless_warriors_in_the_history_of_the_Galactic_Empire");
                await _userManager.AddToRoleAsync(admin, Helpers.Roles.Admin);
            }
            return Ok();
        }

        [HttpGet("status")]
        public string Status()
        {
            return ModelState.IsValid ? "Ok" : "Error";
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create(ViewModels.AutorisationCreateRequest user)
        {
            _logger.LogInformation(Helpers.LogEvents.InsertItem,
                        $"Create user {user.Name}");
            var newUser = new Models.DBUser
            {
                UserName = user.Name,
                Email = user.Email,
                EmailConfirmed = false,
            };
            
            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded)
            {
                _logger.LogInformation(Helpers.LogEvents.InsertItemFaild,
                        $"Insert item {newUser} faild, errors: {result.Errors}");
                return StatusCode(403, result.Errors.ToArray());
            }

            var userInfo = await _userManager.FindByNameAsync(user.Name);
            result = await _userManager.AddToRoleAsync(userInfo, Helpers.Roles.User);

            if (!result.Succeeded)
            {
                _logger.LogError(Helpers.LogEvents.InsertItemFaild,
                        $"Insert user {user.Name} to role {Helpers.Roles.User} faild, errors: {result.Errors}");
                return StatusCode(403, result.Errors.ToArray());
            }
            _logger.LogInformation(Helpers.LogEvents.InsertItem,
                        $"Created user {userInfo.UserName}:{userInfo.Id}");
            return Ok("Ok");
        }

        [HttpPost("login")]
        public async Task<ActionResult<ViewModels.AutorisationLoginResponse>> Login(ViewModels.AutorisationLoginRequest model)
        {
            _logger.LogInformation(Helpers.LogEvents.GetItem,
                    $"Login user {model.Name}");

            var user = await _userManager.FindByNameAsync(model.Name);
            if (user is null)
            {
                _logger.LogInformation(Helpers.LogEvents.InsertItem,
                        $"Get user {model.Name} not fount");
                return NotFound("Invalid username or password.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (!signInResult.Succeeded)
            {
                _logger.LogInformation(Helpers.LogEvents.InsertItem,
                        $"Get user {model.Name} not fount: {signInResult}");
                return NotFound("Invalid username or password.");
            }
            

            if (user.EmailConfirmed == false)
            {
                // TODO
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            _logger.LogInformation(Helpers.LogEvents.GetItem,
                    $"Getting JWT {model.Name}");

            if (identity == null)
            {
                _logger.LogError(Helpers.LogEvents.GetItem,
                    $"Getting JWT faild {model.Name}");
                return NotFound("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: _appSettings.ValidIssuer,
                    audience: _appSettings.ValidAudience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(_appSettings.Expires),
                    signingCredentials: new SigningCredentials(_appSettings.IssuerSigningKey, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new ViewModels.AutorisationLoginResponse
            {
                Id = user.Id,
                Role = roles[0],
                Token = encodedJwt
            };
        }
    }
}
