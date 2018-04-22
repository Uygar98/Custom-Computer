using Custom_Computers.Models.Data;
using Custom_Computers.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Custom_Computers.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Category()
        {
            //this declares the list
            List<CategoryView> CategoryViewList;

            using (DBLayer db = new DBLayer())
            {
                CategoryViewList = db.Category.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryView(x)).ToList();

            }
            return View(CategoryViewList);
        }

        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using (DBLayer db = new DBLayer())
            {
                if(db.Category.Any(x => x.Name == catName))
                

                return "titletaken";

                CategoryDTO catDTO = new CategoryDTO();

                catDTO.Name = catName;
                catDTO.Slug = catName.Replace(" ", "-").ToLower();
                catDTO.Sorting = 100;

                db.Category.Add(catDTO);
                db.SaveChanges();

                id = catDTO.Id.ToString();


                
            }

            return id;

            
        }

        [HttpPost]
        public ActionResult ReorderCategories(int[] id)
        {

            using (DBLayer db = new DBLayer())
            {

                int array = 1;

                CategoryDTO pagedto;

                foreach (var categoryId in id)
                {
                    pagedto = db.Category.Find(categoryId);
                    pagedto.Sorting = array;

                    db.SaveChanges();

                    array++;

                }


            }




            return View();
        }

    }
}