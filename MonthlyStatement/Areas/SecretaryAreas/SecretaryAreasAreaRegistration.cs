using System.Web.Mvc;

namespace MonthlyStatement.Areas.SecretaryAreas
{
    public class SecretaryAreasAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SecretaryAreas";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SecretaryAreas_default",
                "SecretaryAreas/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}