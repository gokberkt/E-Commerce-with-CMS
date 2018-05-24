using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eticaret.Areas.Admin.Controllers
{
    public class ShopAuthorizeAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CustomerData.AdminInfo == null)
            {
                filterContext.Result = new RedirectResult("/Admin/Admin/Login");
            }
        }
    }
}