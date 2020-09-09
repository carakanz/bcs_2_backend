using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Helpers.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Models.IdentityApplicationContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly Helpers.RoleChecker _roleChecker;

        private readonly Helpers.RoleChecker.AccessRule[] _defaultAccessList =
            new Helpers.RoleChecker.AccessRule[]
            {
                new Helpers.RoleChecker.AccessRule
                {
                    Role = Helpers.Roles.Admin,
                    Rule = (user, resourse) => Helpers.RoleChecker.AccessAllowed(),
                    LogLevel = LogLevel.Warning
                },
                new Helpers.RoleChecker.AccessRule
                {
                    Role = Helpers.Roles.User,
                    Rule = (user, resourse) => Helpers.RoleChecker.CompareId(user, resourse),
                    LogLevel = LogLevel.Information
                }
            };

        public UsersController(Models.IdentityApplicationContext context,
            ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
            _roleChecker = new Helpers.RoleChecker(_logger);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewModels.User>> Get(string id)
        {
            var accessAllowed = _roleChecker.CheckAccessList(User, id, _defaultAccessList, "Get user");
            if (!accessAllowed)
            {
                return Forbid("Access denied");
            }

            var user = await _context.Users
                .Include(user => user.Bonds)
                .ThenInclude(userBond => userBond.Bond)
                .Where(user => user.Id == id)
                .Select(user => user.ToViewUser())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (user is null)
            {
                _logger.LogInformation(Helpers.LogEvents.GetItem,
                        $"User id {id} not fount");
                return NotFound($"User id {id} not fount");
            }

            return user;
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<ActionResult<ViewModels.User>> Update(ViewModels.User user)
        {
            var accessAllowed = _roleChecker.CheckAccessList(User, user.Id, _defaultAccessList, "Update user");
            if (!accessAllowed)
            {
                return Forbid("Access denied");
            }

            var dbUser = await _context.Users
                .FirstOrDefaultAsync(user => user.Id == user.Id);
            if (dbUser is null)
            {
                _logger.LogInformation(Helpers.LogEvents.GetItem,
                        $"User id {user.Id} not fount");
                return NotFound($"User id {user.Id} not fount");
            }

            dbUser.UpdateDBUser(user);
            await _context.SaveChangesAsync();

            return dbUser.ToViewUser();
        }
    }
}
