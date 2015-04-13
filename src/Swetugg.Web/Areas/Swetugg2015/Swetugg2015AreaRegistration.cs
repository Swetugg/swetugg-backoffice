using System.Web.Mvc;

namespace Swetugg.Web.Areas.Swetugg2015
{
    public class Swetugg2015AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "swetugg-2015";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

/*            context.MapRoute(
                "Swetugg2015_default",
                "swetugg-2015/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );*/
        }
    }
}