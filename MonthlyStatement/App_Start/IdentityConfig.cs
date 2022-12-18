using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MonthlyStatement.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MonthlyStatement
{

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationAccountManager : UserManager<ApplicationUser>
    {
        public ApplicationAccountManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationAccountManager Create(IdentityFactoryOptions<ApplicationAccountManager> options, IOwinContext context)
        {
            var manager = new ApplicationAccountManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationAccountManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationAccountManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationAccountManager>(), context.Authentication);
        }
    }
}