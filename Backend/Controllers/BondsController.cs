using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Backend.Helpers.Extension;
using System.Diagnostics;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BondsController : ControllerBase
    {
        private readonly Models.IdentityApplicationContext _context;
        private readonly ILogger<UserBondsController> _logger;
        private readonly Helpers.RoleChecker _roleChecker;

        private readonly Helpers.RoleChecker.AccessRule[] _adminAccessList =
            new Helpers.RoleChecker.AccessRule[]
            {
                new Helpers.RoleChecker.AccessRule
                {
                    Role = Helpers.Roles.Admin,
                    Rule = (user, resourse) => Helpers.RoleChecker.AccessAllowed(),
                    LogLevel = LogLevel.Warning
                },
            };

        public BondsController(Models.IdentityApplicationContext context,
            ILogger<UserBondsController> logger)
        {
            _context = context;
            _logger = logger;
            _roleChecker = new Helpers.RoleChecker(_logger);
        }

        // GET: api/DBBonds/5
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ViewModels.Bond>>> GetBonds()
        {
            return await _context.Bonds
                .Include(bond => bond.Company)
                .AsNoTracking()
                .Select(bond => bond.ToViewBond())
                .ToListAsync();
        }

        // GET: api/DBBonds/5
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ViewModels.Bond>>> GetDefoultBonds()
        {
            var bonds = await _context.Bonds
                .Include(bond => bond.Company)
                .AsNoTracking()
                .Select(bond => bond.ToViewBond())
                .ToListAsync();
            bonds.Find(bond => bond.Id == 64).Count = 50;
            bonds.Find(bond => bond.Id == 65).Count = 50;
            bonds.Find(bond => bond.Id == 73).Count = 50;
            bonds.Find(bond => bond.Id == 74).Count = 50;
            return bonds;
        }

        // POST: api/DBBonds
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("update")]
        [Authorize(Roles = Helpers.Roles.Admin)]
        public async Task<ActionResult<IEnumerable<ViewModels.Bond>>> PostBond(IEnumerable<ViewModels.Bond> bonds)
        {
            var accessAllowed = _roleChecker.CheckAccessList(User, null, _adminAccessList, "PostBond");
            if (!accessAllowed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "Access denied"
                });
            }
            foreach (ViewModels.Bond bond in bonds)
            {
                var company = await _context.Companies.FindAsync(bond.Company.Name);
                if (company is null)
                {
                    _context.Add(bond.Company);
                }

                if (bond.Id is null)
                {
                    var bdBond = new Models.DBBond();
                    bdBond.UpdateDBBond(bond);
                    _context.Bonds.Add(bdBond);
                }
                else
                {
                    var bdBond = await _context.Bonds.FindAsync(bond.Id);
                    if (bdBond is null)
                    {
                        bdBond = new Models.DBBond();
                        bdBond.UpdateDBBond(bond);
                        _context.Bonds.Add(bdBond);
                    }
                    else
                    {
                        bdBond.UpdateDBBond(bond);
                    }
                }
            }
            await _context.SaveChangesAsync();

            return Ok(bonds);
        }

        // DELETE: api/DBBonds/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Helpers.Roles.Admin)]
        public async Task<ActionResult<Models.DBBond>> DeleteDBBond(int id)
        {
            var accessAllowed = _roleChecker.CheckAccessList(User, null, _adminAccessList, "PostBond");
            if (!accessAllowed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "Access denied"
                });
            }

            var dBBond = await _context.Bonds.FindAsync(id);
            if (dBBond == null)
            {
                return NotFound();
            }

            _context.Bonds.Remove(dBBond);
            await _context.SaveChangesAsync();

            return dBBond;
        }
    }
}
