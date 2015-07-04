using System.Web.Mvc;

namespace Swetugg.Web.Areas.Swetugg2016
{
    public class Swetugg2016AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "swetugg-2016";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
/*            context.MapRoute(
                "Swetugg2016_default",
                "swetugg-2016/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
  */      }
    }
}