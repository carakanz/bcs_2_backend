using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Helpers.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBondsController : ControllerBase
    {
        private readonly Models.IdentityApplicationContext _context;
        private readonly ILogger<UserBondsController> _logger;
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

        public UserBondsController(Models.IdentityApplicationContext context,
            ILogger<UserBondsController> logger)
        {
            _context = context;
            _logger = logger;
            _roleChecker = new Helpers.RoleChecker(_logger);
        }

        // POST: api/UserBonds
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ViewModels.Bond>> Buy(ViewModels.UserBondBuyRequest userBond)
        {
            var accessAllowed = _roleChecker.CheckAccessList(User, userBond.UserId, _defaultAccessList, "Buy bond");
            if (!accessAllowed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "Access denied"
                });
            }

            var bond = await _context.Bonds
                .FirstOrDefaultAsync(bond => bond.Id == userBond.BondId);
            if (bond is null)
            {
                _logger.LogInformation(Helpers.LogEvents.GetItem,
                        $"BondId {userBond.BondId} not fount");
                return NotFound(
                    new
                    {
                        error = "BondId not found",
                        userBond
                    });
            }

            if (bond.CurrentPurchasePrice is null)
            {
                _logger.LogInformation(Helpers.LogEvents.GetItem,
                        $"BondId {userBond.BondId} not fount");
                return StatusCode(StatusCodes.Status410Gone,
                    new
                    {
                        error = "Bond is not for sale",
                        userBond
                    });
            }

            for (int i = 1; i < 6; ++i)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var dbUser = await _context.Users
                        .Where(user => user.Id == userBond.UserId)
                        .Include(user => user.Bonds
                            .Where(bond => bond.BondId == userBond.BondId))
                    .FirstOrDefaultAsync();

                    if (dbUser is null)
                    {
                        _logger.LogInformation(Helpers.LogEvents.GetItem,
                                $"UserId {userBond.UserId} not fount");
                        return NotFound(
                        new
                        {
                            error = "UserId not found",
                            userBond
                        });
                    }

                    var requiredMoney = bond.CurrentPurchasePrice * userBond.Count;
                    if (dbUser.Money < requiredMoney)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new
                        {
                            error = "Payment required",
                            correntMoney = dbUser.Money,
                            requiredMoney = userBond.Count * bond.CurrentPurchasePrice
                        });
                    }

                    dbUser.Money -= (long)requiredMoney;

                    Models.DBUserBond result = null;

                    if (dbUser.Bonds.Count == 0)
                    {
                        result = new Models.DBUserBond
                        {
                            UserId = userBond.UserId,
                            Count = userBond.Count,
                            Bond = bond
                        };
                        _context.UserBonds.Add(result);
                    } 
                    else
                    {
                        result = dbUser.Bonds.First();
                        result.Count += userBond.Count;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return CreatedAtAction("Buy", result.ToViewUser());
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogWarning(Helpers.LogEvents.UpdateItem,
                        $"Rollback {i} transaction buy bond {userBond}, error: {ex}");
                }
            }
            _logger.LogError(Helpers.LogEvents.UpdateItem,
                        $"Rollback 5 transaction buy bond {userBond}");

            return Conflict(new
            {
                error = "Rollback 5 transaction buy bond",
                userBond
            });
        }

        // POST: api/UserBonds
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ViewModels.Bond>> Sell(ViewModels.UserBondBuyRequest userBond)
        {
            var accessAllowed = _roleChecker.CheckAccessList(User, userBond.UserId, _defaultAccessList, "Buy bond");
            if (!accessAllowed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "Access denied"
                });
            }

            var bond = await _context.Bonds
                .FirstOrDefaultAsync(bond => bond.Id == userBond.BondId);
            if (bond is null)
            {
                _logger.LogInformation(Helpers.LogEvents.GetItem,
                        $"BondId {userBond.BondId} not fount");
                return NotFound(
                    new
                    {
                        error = "BondId not found",
                        userBond
                    });
            }

            if (bond.CurrentSellingPrice is null)
            {
                _logger.LogInformation(Helpers.LogEvents.GetItem,
                        $"BondId {userBond.BondId} not fount");
                return StatusCode(StatusCodes.Status410Gone,
                    new
                    {
                        error = "Bond is not for purchase",
                        userBond
                    });
            }

            for (int i = 1; i < 6; ++i)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var dbUser = await _context.Users
                        .Where(user => user.Id == userBond.UserId)
                        .Include(user => user.Bonds
                            .Where(bond => bond.BondId == userBond.BondId))
                    .FirstOrDefaultAsync();

                    if (dbUser is null)
                    {
                        _logger.LogInformation(Helpers.LogEvents.GetItem,
                                $"UserId {userBond.UserId} not fount");
                        return NotFound(
                        new
                        {
                            error = "UserId not found",
                            userBond
                        });
                    }

                    if (dbUser.Bonds.Count == 0)
                    {
                        _logger.LogInformation(Helpers.LogEvents.GetItem,
                                $"UserId {userBond.UserId} not fount");
                        return NotFound(
                        new
                        {
                            error = "Too few bonds",
                            userBond
                        });
                    }

                    var dbUserBond = dbUser.Bonds.First();

                    if (dbUserBond.Count < userBond.Count)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new
                        {
                            error = "Too few bonds",
                            correntBonds = dbUserBond.Count,
                            requiredBonds = userBond.Count
                        });
                    }
                    dbUserBond.Count -= userBond.Count;
                    dbUser.Money += (long)bond.CurrentPurchasePrice * userBond.Count;

                    if (dbUserBond.Count == 0)
                    {
                        _context.UserBonds.Remove(dbUserBond);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return CreatedAtAction("Buy", dbUserBond.ToViewUser());
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogWarning(Helpers.LogEvents.UpdateItem,
                        $"Rollback {i} transaction buy bond {userBond}, error: {ex}");
                }
            }
            _logger.LogError(Helpers.LogEvents.UpdateItem,
                        $"Rollback 5 transaction buy bond {userBond}");

            return Conflict(new
            {
                error = "Rollback 5 transaction buy bond",
                userBond
            });
        }
    }
}
