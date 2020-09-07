using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Helpers
{
    public class RoleChecker
    {
        private readonly ILogger _logger;
        public LogLevel _accessDeniedLogLevel;
        public RoleChecker(ILogger logger, LogLevel accessDeniedLogLevel = LogLevel.Information) 
        {
            _logger = logger;
            _accessDeniedLogLevel = accessDeniedLogLevel;
        }
        public struct AccessRule
        {
            public string Role;
            public Func<string, string, bool> Rule;
            public LogLevel LogLevel;
        }
        public bool CheckAccessList(ClaimsPrincipal user,
            string resource,
            IEnumerable<AccessRule> accessList, 
            string message)
        {
            var senderId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
            {
                _logger.LogError(Helpers.LogEvents.InvalidToken,
                        $"{message} {resource} invalid token");
                return false;
            }

            foreach (AccessRule rule in accessList)
            {
                if (user.IsInRole(rule.Role) && rule.Rule(senderId, resource))
                {
                    _logger.Log(rule.LogLevel, LogEvents.AccessAllowed,
                        $"{message}, access allowed resource {resource} by user {senderId}");
                    return true;
                }
            }

            _logger.Log(_accessDeniedLogLevel, LogEvents.Forbiden,
                        $"{message}, access denied resource {resource} by user {senderId}");
            return false;
        }

        public static bool AccessAllowed() => true;
        public static bool AccessDenied() => false;
        public static bool CompareId(string user, string resource) => user == resource;
    }
}
