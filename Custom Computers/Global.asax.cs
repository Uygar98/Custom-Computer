using Custom_Computers.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Custom_Computers
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            //this checks if the user has logged in
            if(User == null)
            {
                return;

            }

            //thsi gets the username
            string username = Context.User.Identity.Name;


            //this part of the code is used to declare the array of roles
            string[] roles = null;

            using (DBLayer db = new DBLayer())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                roles = db.UserRole.Where(x => x.UserId == dto.Id).Select(x => x.Role.name).ToArray();
            }

            //this is used to bulid the ipriciple object
            IIdentity userID = new GenericIdentity(username);
            IPrincipal newUser = new GenericPrincipal(userID, roles);

            //update context user
            Context.User = newUser;
        }

    }
}
