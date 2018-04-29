using Custom_Computers.Models.Data;
using Custom_Computers.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;

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
                if (db.Category.Any(x => x.Name == catName))


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

        public ActionResult DeleteCategory(int id)
        {
            using (DBLayer db = new DBLayer())
            {

                //this is used to get the correct page
                CategoryDTO categorydto = db.Category.Find(id);

                //this is used to remove page that is selected
                db.Category.Remove(categorydto);

                //this is to save the changes that the user has done
                db.SaveChanges();
            }
            return RedirectToAction("Category");
        }

        [HttpPost]
        public string RenameCategories(string newCategoryName, int id)
        {
            using (DBLayer db = new DBLayer())
            {
                if (db.Category.Any(x => x.Name == newCategoryName))

                    return "titletaken";

                CategoryDTO catdto = db.Category.Find(id);

                catdto.Name = newCategoryName;
                catdto.Slug = newCategoryName.Replace(" ", "-").ToLower();



                db.SaveChanges();

            }

            return "ok";
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            //this is used to initialise the model
            ProductView model = new ProductView();

            //this is used to select the list of categories to add a product
            using (DBLayer db = new DBLayer())
            {

                model.Categories = new SelectList(db.Category.ToList(), "Id", "Name");
            }
            return View(model);
        }
        
        [HttpPost]
        public ActionResult AddProduct(ProductView model, HttpPostedFileBase file)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                using (DBLayer db = new DBLayer())
                {
                    model.Categories = new SelectList(db.Category.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // this part is used to make the product name unique
            using (DBLayer db = new DBLayer())
            {
                if (db.Product.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Category.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Sorry this product name is already taken");
                    return View(model);
                }
            }

            // //this is used to declare the product id 
            int id;

            // this is used to intialise the product class and save it
            using (DBLayer db = new DBLayer())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.Quantity = model.Quantity;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Category.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Product.Add(product);
                db.SaveChanges();

                // Get the id
                id = product.Id;
            }

            // Set TempData message
            TempData["SM"] = "You have added a product!";

            #region Upload Image

            // Create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check if a file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (DBLayer db = new DBLayer())
                    {
                        model.Categories = new SelectList(db.Category.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Sorry the image was not uploaded wrong file type");
                        return View(model);
                    }
                }

                // Init image name
                string imageName = file.FileName;

                // Save image name to DTO
                using (DBLayer db = new DBLayer())
                {
                    ProductDTO dto = db.Product.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                // Save original
                file.SaveAs(path);

                // Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion

            // Redirect
            return RedirectToAction("AddProduct");


        }

        public ActionResult ViewProducts(int? page, int? categoryId)
        {
            //this part is used to declare a list for the product views
            List<ProductView> productViewList;

            //this part sets a page with a page number
            var pageNo = page ?? 1;

            using (DBLayer db = new DBLayer())
            {
                //this part is used to initilise the list

                productViewList = db.Product.ToArray().Where(x => categoryId == null || categoryId == 0 || x.CategoryId == categoryId)
                                  .Select(x => new ProductView(x))
                                  .ToList();



                //this sets the selected category
                ViewBag.Categories = new SelectList(db.Category.ToList(), "Id", "Name");

                //this part of the code is used to poplulate the category selection list
                ViewBag.SelectedCat = categoryId.ToString();
            }




            //this sets a  pagination
            var onePageOfProducts = productViewList.ToPagedList(pageNo, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts;

            //this aprt is used to return the view with the list



            return View(productViewList);
        }
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //this part is used to declare the product view
            ProductView model;

            using (DBLayer db = new DBLayer())
            {
                ProductDTO prodto = db.Product.Find(id);

                if (prodto == null)
                {

                    return Content("The product you have selected does not exist");
                }

                //this code is used to intilise the model
                model = new ProductView(prodto);

                //this creates a list of products to select
                model.Categories = new SelectList(db.Category.ToList(), "Id", "Name");

                //this recives and allows the user to view all the images associated with the product
                model.Images = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }



                return View(model);
        }

        [HttpPost]
        public ActionResult EditProduct(ProductView model, HttpPostedFileBase file)
        {
            //this gets the product id
            int id = model.Id;

            //this populates the category list and the images for the product
            using (DBLayer db = new DBLayer())
            {
                model.Categories = new SelectList(db.Category.ToList(), "Id", "Name");

            }
            model.Images = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                              .Select(fn => Path.GetFileName(fn));

            // this part is used to the check the model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //this checks if the user has input a inquie product name
            using (DBLayer db = new DBLayer())
            {
                if (db.Product.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Sorry the product name is already taken");
                    return View(model);
                }

            }
            // Update product
            using (DBLayer db = new DBLayer())
            {
                ProductDTO dto = db.Product.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.Quantity = model.Quantity;

                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Category.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }
            // Set TempData message
            TempData["SM"] = "You have edited the product!";

            #region Image Upload

            // this checks if the file is uploaded
            if (file != null && file.ContentLength > 0)
            {

                // this gets the extension fo the file
                string ext = file.ContentType.ToLower();

                // checks and verifys if teh extenson acn be used on the site
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (DBLayer db = new DBLayer())
                    {
                        ModelState.AddModelError("", "Sorry the images was not uploaded please check the file type before uploading");
                        return View(model);
                    }
                }

                // Set uplpad directory paths
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // Delete files from directories

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();

                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();

                // Save image name

                string imageName = file.FileName;

                using (DBLayer db = new DBLayer())
                {
                    ProductDTO dto = db.Product.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Save original and thumb images

                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            

            #endregion


            return RedirectToAction("EditProduct");


        }

      
        public ActionResult DeleteProduct(int id)
        {
            // this deletes the product form the database
            using (DBLayer db = new DBLayer())
            {
                ProductDTO dto = db.Product.Find(id);
                db.Product.Remove(dto);

                db.SaveChanges();
            }

            // Delete product folder
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            // Redirect
            return RedirectToAction("ViewProducts");
        }

        [HttpPost]
        public void SaveGalleryImages(int id)
        {
           //this part of the code loops through all the files
            foreach (string fileName in Request.Files)
            {
                // Init the file
                HttpPostedFileBase file = Request.Files[fileName];

                // Check it's not null
                if (file != null && file.ContentLength > 0)
                {
                    // Set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Set image paths
                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    // Save original and thumb

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);
                }

            }

        }

        // POST: Admin/Shop/DeleteImage
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {

            //this finds all images addreess and deletes
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }

    }

}
