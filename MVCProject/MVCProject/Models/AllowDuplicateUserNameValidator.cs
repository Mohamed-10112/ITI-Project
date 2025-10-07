using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MVCProject.Models
{
    public class AllowDuplicateUserNameValidator<TUser> : IUserValidator<TUser>
        where TUser : class
    {
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
