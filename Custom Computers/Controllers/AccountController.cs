using Custom_Computers.Models.Data;
using Custom_Computers.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Custom_Computers.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            // Confirm user is not logged in

            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("user-profile");

            // Return view
            return View();
        }

      
        [HttpPost]
        public ActionResult Login(LoginUserView model)
        {
            //this cecks the model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if the user is valid

            bool isValid = false;

            using (DBLayer db = new DBLayer())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
            }
        }



    

        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserView model)
        {
            // this checks the models state
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            // this checks if the password matchs
            if (!model.Password.Equals(model.ConfrimPassword))
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View("CreateAccount", model);
            }

            using (DBLayer db = new DBLayer())
            {
                // thsi is used to make uure the username is unique
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", "Username " + model.Username + " is taken.");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                // Create userDTO
                UserDTO userDTO = new UserDTO()
                {
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    EmailAddress = model.EmailAddress,
                    Username = model.Username,
                    Password = model.Password
                };

                // Add the DTO
                db.Users.Add(userDTO);

                // Save
                db.SaveChanges();

                // Add to UserRolesDTO
                int id = userDTO.Id;

                UserRoleDTO userRolesDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRole.Add(userRolesDTO);
                db.SaveChanges();
            }

            // Create a TempData message
            TempData["SM"] = "You are now registered and can login.";

            // Redirect
            return Redirect("~/account/login");
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/account/login");
        }

        public ActionResult UserDetailsPartial()
        {
            // Get username
            string username = User.Identity.Name;

            // Declare model
            UserDetailsPartial model;

            using (DBLayer db = new DBLayer())
            {
                // Get the user
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);

                // Build the model
                model = new UserDetailsPartial()
                {
                    Firstname = dto.Firstname,
                    Lastname = dto.Lastname
                };
            }

            // Return partial view with model
            return PartialView(model);
        }

         [HttpGet]
          [ActionName("user-profile")]
        public ActionResult UserProfile()
          {

              //thsi gets the username 
              string username = User.Identity.Name;

            //this is used to delcare the model
            UserProfile model;

            using (DBLayer db = new DBLayer())
            {
                //this gets the user 
                UserDTO userdto = db.Users.FirstOrDefault(x => x.Username == username);


                //thsi is used to build the model
                model = new UserProfile(userdto);
            }


                return View("UserProfile", model);
        }

        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfile model)
        {

            // this checks the models state
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            // this checks if the password matchs
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if(!model.Password.Equals(model.ConfrimPassword))
                {
                    ModelState.AddModelError("", "The passwords entered do not match ");
                    return View("UserProfile", model);
                }
            }

            using (DBLayer db = new DBLayer())
            {

                //thsi gets the username 
                string username = User.Identity.Name;

                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == username))
                    {

                    ModelState.AddModelError("", "Username" + model.Username + " is already taken");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                //this allows the user to edit the deatils
                UserDTO userdto = db.Users.Find(model.Id);

                userdto.Firstname = model.Firstname;
                userdto.Lastname = model.Lastname;
                userdto.EmailAddress = model.EmailAddress;
                userdto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))

                {
                userdto.Password = model.Password;

                }

                //thsi saves all changes made to the details
                db.SaveChanges();
            }

            // Create a TempData message
            TempData["SM"] = "You have changed your details.";

            return Redirect("~/account/user-profile");



        }
    }
}