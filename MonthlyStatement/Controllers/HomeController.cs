using MonthlyStatement.Models;
using System.Linq;
using System.Web.Mvc;


namespace MonthlyStatement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private CP25Team04Entities db = new CP25Team04Entities();

        public ActionResult Index()
        {
            var id = User.Identity.Name;
            if (Session["Faculty-exist"] == null)
            {
                if (string.IsNullOrEmpty(id))
                {
                    Session["Faculty-exist"] = false;
                }
                else
                {
                    var khoa = db.Profiles.FirstOrDefault(p => p.email.ToLower().Trim().Equals(id.ToLower().Trim()));
                    if (khoa == null)
                    {
                        Session["Faculty-exist"] = false;
                    }
                    else
                    {
                        int? fac = khoa.faculty_id;
                        if (fac == null)

                        {
                            Session["Faculty-exist"] = false;
                        }
                        else
                        {
                            Session["Faculty-exist"] = true;
                        }
                    }
                }
            }

            return View();
        }
    }
}