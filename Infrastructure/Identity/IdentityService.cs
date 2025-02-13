using ESMART.Application.Common.Models;
using ESMART.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ESMART.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> CreatUserAsync(string userName, string password)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName,
                };

                var result = await _userManager.CreateAsync(user, password);
                return (result.ToApplicationResult());
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when creating user account. " + ex.Message);
            }
        }
    }
}
