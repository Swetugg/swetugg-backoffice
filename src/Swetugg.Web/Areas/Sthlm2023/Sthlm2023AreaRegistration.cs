using System.Web.Mvc;

namespace Swetugg.Web.Areas.Sthlm2023
{
    public class Sthlm2023AreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "sthlm-2023";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}