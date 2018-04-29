using Custom_Computers.Models.Data;
using Custom_Computers.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Custom_Computers.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        public ActionResult Index(string page = "")
        {

            //this part of the code gets and sets the slug for the page
            if (page == "")
                page = "home";

            //this is used to decalere the model and the class
            PagesView model;
            PageDTO pagedto;

            //this checks if the pages being accessed exists
            using (DBLayer db = new DBLayer())
            {
                if (! db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }

            }

            //this pulls all the data from the page class
            using (DBLayer db = new DBLayer())
            {
                pagedto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }


            // Set page title
            ViewBag.PageTitle = pagedto.Title;

            // Check for sidebar
            if (pagedto.Sidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            // Init model
            model = new PagesView(pagedto);

            // Return view with model
            return View(model);








      
        }

        public ActionResult PagesMenuPartial()
        {
            // Declare a list of PageVM
            List<PagesView> pageViewList;

            // Get all pages except home
            using (DBLayer db = new DBLayer())
            {
                pageViewList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PagesView(x)).ToList();
            }
            // Return partial view with list
            return PartialView(pageViewList);
        }

        public ActionResult SidebarPartial()
        {
            // this part delcares the dto
            SidebarView model;

            // this part of the code iniilises the mode
            using (DBLayer db = new DBLayer())
            {
                SidebarDTOcs dto = db.Sidebar.Find(1);

                model = new SidebarView(dto);
            }

            // Return partial view with model
            return PartialView(model);
        }
    }

}






