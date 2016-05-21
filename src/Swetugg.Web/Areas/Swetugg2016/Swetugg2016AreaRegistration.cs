using System;
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
        }
    }
}