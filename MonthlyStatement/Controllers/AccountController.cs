using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using MonthlyStatement.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using System.DirectoryServices.AccountManagement;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MonthlyStatement.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationAccountManager _userManager;
        private CP25Team04Entities db = new CP25Team04Entities();


        public AccountController()
        {
        }

        public AccountController(ApplicationAccountManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationAccountManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationAccountManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (!Request.IsAuthenticated)
            {
                ViewBag.ReturnUrl = "/";
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public void SignIn()
        {

            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = Url.Action("SignInCallBack") },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        //
        // GET: /Account/SignInCallBack
        public async Task<ActionResult> SignInCallBack()
        {
            // Get user information
            var user = new ApplicationUser
            {
                Email = User.Identity.Name,
                UserName = User.Identity.Name,
            };
            // Check if user exists
            var currentUser = await UserManager.FindByEmailAsync(user.Email);
            if (currentUser != null)
            {
                if (currentUser.Roles.Count != 0)
                {

                    //var email = User.Identity.Name;
                    //var id = db.AspNetUsers.FirstOrDefault(f => f.Email == email).Id;

                    //Session["Avt"] = db.Profiles.FirstOrDefault(a => a.account_id == id).avatar;
                    //Session["ID_User"] = id;
                    //Session["Email"] = email;

                    //Session["ID_User"] = currentUser.Id;
                    //var profile = db.Profiles.FirstOrDefault(f => f.account_id == currentUser.Id);
                    //Session["Avt"] = profile.avatar;

                    // Add role claim to user
                    ClaimsIdentity identity = (ClaimsIdentity)User.Identity;

                    var currentRole = await UserManager.GetRolesAsync(currentUser.Id);
                    identity.AddClaim(new Claim(ClaimTypes.Role, currentRole[0]));
                    IOwinContext context = HttpContext.GetOwinContext();

                    context.Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    context.Authentication.SignIn(identity);
                }
            }
            else
            {
                // Create new user
                await UserManager.CreateAsync(user);
                var aspNetRole = db.AspNetRoles.Find("5");
                var aspNetUser = db.AspNetUsers.Find(user.Id);
                aspNetRole.AspNetUsers.Add(aspNetUser);
                //Add profile
                Profile profile = new Profile();
                profile.account_id = aspNetUser.Id;
                db.Profiles.Add(profile);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/SignOut
        [HttpPost]
        public ActionResult SignOut()
        {
            /// Send an OpenID Connect sign-out request.
            /// 
            HttpContext.GetOwinContext()
                        .Authentication
                        .SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Redirect("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}