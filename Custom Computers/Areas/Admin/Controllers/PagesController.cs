using Custom_Computers.Models.Data;
using Custom_Computers.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Custom_Computers.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PagesView> listPages;

            using (DBLayer db = new DBLayer())
            {
                //this initialises the list
                listPages = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PagesView(x)).ToList();
            }
                return View(listPages);
        }


        // GET: Admin/Pages/CreatePage
        [HttpGet]
        public ActionResult CreatePage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePage(PagesView model)
        {
            //this part checks the model state of this page
            if (! ModelState.IsValid)
            {
                return View(model);
            }

            using (DBLayer db = new DBLayer())
            {
                //this part is used to declare the slug for the page
                string slug;

                //this part intialises the PageDTO
                PageDTO dto = new PageDTO();

                //the part dtos the title for the page
                dto.Title = model.Title;
                
                //check if the slug has been entered when creating a page
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                     
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //this part is identify if the tile and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title)  || db.Pages.Any(x => x.Slug == slug))
                {

                    ModelState.AddModelError("", "Either the title or slug already exists please choose an alternative");
                    return View(model);
                }

                //data transfer object the rest of the objects
                dto.Body = model.Body;
                dto.Sorting = 100;
                dto.Slug = slug;
                dto.Sidebar = model.Sidebar;

                //this part saves all the data transfer objects that occured
                db.Pages.Add(dto);
                db.SaveChanges();
            }
           
            //this part creates a tempdata message
            TempData["SM"] = "You have successfully created a new page!";


            //redirect user 
            return RedirectToAction("CreatePage");
        }

        [HttpGet ]
        public ActionResult EditPage(int id)
        {
            PagesView model;

            using (DBLayer db = new DBLayer())
            {
                //looks at the table and selects the correct page to edit
                PageDTO pagedto = db.Pages.Find(id);

                //checks if the page exists to change
                if (pagedto == null)
                {
                    //displays error message to user
                    return Content("The page you are trying to edit does not exist");
                }
                //this part intials the page view
                model = new PagesView(pagedto);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditPage(PagesView model)
        {
            //checkk the state of the model
            if(! ModelState.IsValid)
            {
                return View(model);

            }
            using (DBLayer db = new DBLayer())
            {
                //this part gets the id of the page that is being edited
                int id = model.Id;

                //this part is used to initials the slug of the page
                string slug = "home" ;

                //this part is used to get the correct page that is being edited
                PageDTO pagedto = db.Pages.Find(id);

                //this part is for dto the title
                pagedto.Title = model.Title;

                //this part is to check if a slug has been entered and if not then set one automatically
                if (model.Slug != "home")
                {

                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                         
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //this part is identify if the tile and slug are unique
                if (db.Pages.Where(x => x.Id != id ).Any(x => x.Title  == model.Title) ||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                    {

                    ModelState.AddModelError("", "Either the title or slug already exists please choose an alternative");
                    return View(model);
                }
                //dto the rest
                pagedto.Body = model.Body;
                pagedto.Slug = slug;
                pagedto.Sidebar = model.Sidebar;

                //this part saves the dto that occured
                db.SaveChanges();

            }

            //this part is used to set temp data message

            TempData["SM"] = "You have successfully edited page!";

            //redirect back to edit page area

            return RedirectToAction("EditPage");

        }

        public ActionResult PageDetails(int id)
        {
            //this part declares the pages view
            PagesView model;

            using (DBLayer db = new DBLayer())
            {
                //this gets the correct page
                PageDTO pagedto = db.Pages.Find(id);


                //this part is used to confirm the page is the correct one
                if(pagedto == null)
                {
                    return Content("This page does not exist sorry");

                }

                //this part is used to intialise the page
                model = new PagesView(pagedto);
            }

            //this retruns the model with view
                return View(model);
        }

        public ActionResult DeletePage(int id)
        {
            using (DBLayer db = new DBLayer())
            {

                //this is used to get the correct page
                PageDTO pagedto = db.Pages.Find(id);

                //this is used to remove page that is selected
                db.Pages.Remove(pagedto);

                //this is to save the changes that the user has done
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ReorderPages(int[] id)
        {

            using (DBLayer db = new DBLayer())
            {

                int array = 1;

                PageDTO pagedto;

                foreach (var PageID in id)
                {
                    pagedto = db.Pages.Find(PageID);
                    pagedto.Sorting = array;

                    db.SaveChanges();

                    array++;

                }


            }




                return View();
        }

        [HttpGet]
        public ActionResult EditSidebar()
            
        {
            SidebarView model;


            using (DBLayer db = new DBLayer())
            {
                //this part access the sidebar class
                SidebarDTOcs sidebardto = db.Sidebar.Find(1);

                //this intialiess the model
                model = new SidebarView(sidebardto);
            }
                return View(model);
        }

        [HttpPost]
        public ActionResult EditSidebar(SidebarView model)
        {
            using (DBLayer db = new DBLayer())
            {
                SidebarDTOcs sidebardto = db.Sidebar.Find(1);
                sidebardto.Body = model.Body;
                db.SaveChanges();
            }
            TempData["SM"] = "You have made changes to the sidebar";

            return RedirectToAction("EditSidebar");
        }
    }
}