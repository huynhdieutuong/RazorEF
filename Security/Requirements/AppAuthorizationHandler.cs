using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Security.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RazorEF.Models;

namespace RazorEF.Security.Requirements
{
    // 22.4 Create AppAuthorizationHandler
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            _logger.LogInformation("context.Resource ~ " + context.Resource?.GetType().Name);
            foreach (var requirement in requirements)
            {
                if (requirement is GenZRequirement)
                {
                    if (IsGenZ(context.User, (GenZRequirement)requirement))
                    {
                        // 22.4.1 If meet GenZRequirement => Succeed
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }

        // 22.4.2 Create IsGenZ to verify
        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement requirement)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;

            if (appUser.BirthDate == null)
            {
                _logger.LogInformation($"{appUser.UserName} don't have BirthDate.");
                return false;
            }
            int year = appUser.BirthDate.Value.Year;

            var success = (year >= requirement.FromYear && year <= requirement.ToYear);
            if (success)
            {
                _logger.LogInformation($"{appUser.UserName} meets GenZRequirement.");
            }
            else
            {
                _logger.LogInformation($"{appUser.UserName} doesn't meet GenZRequirement.");
            }

            return success;
        }
    }
}