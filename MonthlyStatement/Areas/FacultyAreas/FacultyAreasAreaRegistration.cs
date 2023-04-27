using System.Web.Mvc;

namespace MonthlyStatement.Areas.FacultyAreas
{
    public class FacultyAreasAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FacultyAreas";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FacultyAreas_default",
                "FacultyAreas/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}