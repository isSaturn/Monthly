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
using System.Web.Helpers;

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
                    //Add profile
                    var pro = db.Profiles.FirstOrDefault(p => p.email.ToLower().Equals(user.UserName.ToLower()));
                    if (pro != null)
                    {
                        if (pro.account_id == null)
                        {
                            var aspNetUser = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(user.Email.ToLower()));

                            pro.account_id = aspNetUser.Id;
                            pro.email = aspNetUser.UserName;

                            db.Entry(pro).State = System.Data.Entity.EntityState.Modified;

                            db.SaveChanges();
                        }
                        

                    }

                    // Add role claim to user
                    ClaimsIdentity identity = (ClaimsIdentity)User.Identity;

                    var currentRole = await UserManager.GetRolesAsync(currentUser.Id);
                    identity.AddClaim(new Claim(ClaimTypes.Role, currentRole[0]));
                    IOwinContext context = HttpContext.GetOwinContext();

                    context.Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    context.Authentication.SignIn(identity);

                    var userdb = db.Profiles.FirstOrDefault(p => p.account_id == currentUser.Id);
                    userdb.last_login = DateTime.Now;
                    db.SaveChanges();

                }
            }
            else
            {

                await UserManager.CreateAsync(user);
                var aspNetRole = db.AspNetRoles.Find("6");
                var aspNetUser = db.AspNetUsers.Find(user.Id);
                aspNetRole.AspNetUsers.Add(aspNetUser);

                //Add profile
                var pro = db.Profiles.FirstOrDefault(p => p.email.ToLower().Equals(user.UserName.ToLower()));
                if (pro != null)
                {
                    var aspNetUser1 = db.AspNetUsers.FirstOrDefault(a => a.Email.ToLower().Equals(user.Email.ToLower()));

                    pro.account_id = aspNetUser1.Id;
                    pro.email = aspNetUser1.UserName;
                    db.Entry(pro).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    // Create new user
                    Profile profile = new Profile();
                    profile.account_id = aspNetUser.Id;
                    profile.email = aspNetUser.UserName;
                    db.Profiles.Add(profile);
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/SignOut
        [HttpPost]
        public ActionResult SignOut()
        {
            // Get user information
            var user = new ApplicationUser
            {
                Email = User.Identity.Name,
                UserName = User.Identity.Name,
            };
            // Check if user exists
            var currentUser = UserManager.FindByEmail(user.Email);
            /// Send an OpenID Connect sign-out request.
            /// 
            HttpContext.GetOwinContext()
                        .Authentication
                        .SignOut(CookieAuthenticationDefaults.AuthenticationType);

            var userdb = db.Profiles.FirstOrDefault(p => p.account_id == currentUser.Id);
            userdb.last_logout = DateTime.Now;
            db.SaveChanges();

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