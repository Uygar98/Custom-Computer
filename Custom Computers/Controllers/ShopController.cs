using Custom_Computers.Models.Data;
using Custom_Computers.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Custom_Computers.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // this pat of of the code delcares a alist of the category view
            List<CategoryView> categoryViewList;

            // Init the list
            using (DBLayer db = new DBLayer())
            {
                categoryViewList = db.Category.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryView(x)).ToList();
            }

            // Return partial with list
            return PartialView(categoryViewList);
        }

        // GET: /shop/category/name
        public ActionResult Category(string name)
        {
            // this part of the code is used to declare the product view list
            List<ProductView> productViewList;

            using (DBLayer db = new DBLayer())
            {
                // this is used to get the catergory id 
                CategoryDTO categoryDTO = db.Category.Where(x => x.Slug == name).FirstOrDefault();

                int catId = categoryDTO.Id;

                // this is used to initilise the list
                productViewList = db.Product.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductView(x)).ToList();

                // Get category name

                var productCat = db.Product.Where(x => x.CategoryId == catId).FirstOrDefault();

                ViewBag.CategoryName = productCat.CategoryName;
            }

            // Return view with list
            return View(productViewList);
        }

        // GET: /shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //this part of the code is used to declare the product class and view
            ProductView model;
            ProductDTO prodto;

            //this part is used to initilise the product id

            int id = 0;


            using (DBLayer db = new DBLayer())
            {
                //this checks if the product exissts in the database
                if (!db.Product.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }


                // this part of the code is used to initlise the product class
                prodto = db.Product.Where(x => x.Slug == name).FirstOrDefault();

                //this gets the id 
                id = prodto.Id;

                // this initlises the product class
                model = new ProductView(prodto);
            }

            // Get gallery images
         
            model.Images = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Return view with model
            return View("ProductDetails", model);

            
        }
    }
}