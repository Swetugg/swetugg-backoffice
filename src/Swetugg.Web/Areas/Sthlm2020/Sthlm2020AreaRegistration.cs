using System.Web.Mvc;

namespace Swetugg.Web.Areas.Sthlm2020
{
    public class Sthlm2020AreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "sthlm-2020";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}