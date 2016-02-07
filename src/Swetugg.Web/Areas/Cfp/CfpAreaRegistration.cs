using System.Web.Mvc;

namespace Swetugg.Web.Areas.Cfp
{
    public class CfpAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "cfp";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

        }
    }
}