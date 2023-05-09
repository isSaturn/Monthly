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
            return View();
        }
    }
}