using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog_Website.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // List of controllers to exclude
            var excludedControllers = new[] { "UserManage" };

            // Get the current controller name
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            if (excludedControllers.Contains(controllerName))
            {
                base.OnActionExecuting(context);
                return; // Skip the filter for excluded controllers
            }

            // Check if the user session exists
            if (context.HttpContext.Session.GetString("UserId") == null)
            {
                context.Result = new RedirectToActionResult("Login", "UserManage", null);
            }

            base.OnActionExecuting(context);
        }

    }
}
