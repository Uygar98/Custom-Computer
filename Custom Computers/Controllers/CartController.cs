using Custom_Computers.Models.Data;
using Custom_Computers.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Custom_Computers.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {

            // this part of the code intilises the cart list 
            var cart = Session["cart"] as List<CartView> ?? new List<CartView>();

            // this checks if the cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "The cart is empty.";
                return View();
            }

            // this calculates the total and sends it to viewbag

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Return view with list
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // thsi part of the code is used to initilise the cart view
            CartView model = new CartView();

            // this is used to initilise the quantity
            int qty = 0;

            // this is used to initilise the price
            decimal price = 0m;

            // this checks if the carts session
            if (Session["cart"] != null)
            {
                // Get total qty and price
                var list = (List<CartView>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;

            }
            else
            {
                // this sets the quanity to 0 and price
                model.Quantity = 0;
                model.Price = 0m;
            }

            // Return partial view with model
            return PartialView(model);
       
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Init CartVM list
            List<CartView> cart = Session["cart"] as List<CartView> ?? new List<CartView>();

            // Init CartVM
            CartView model = new CartView();

            using (DBLayer db = new DBLayer())
            {
                // Get the product
                ProductDTO product = db.Product.Find(id);

                // Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // If not, add new
                if (productInCart == null)
                {
                    cart.Add(new CartView()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // If it is, increment
                    productInCart.Quantity++;
                }
            }

            // Get total qty and price and add to model

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return PartialView(model);
        }
        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // this part fo the code initilises the list
            List<CartView> cart = Session["cart"] as List<CartView>;

            using (DBLayer db = new DBLayer())
            {
                // this gets all content in the cart view to be passed to list
                CartView model = cart.FirstOrDefault(x => x.ProductId == productId);

                // this increments the quanitity of items ordered 
                model.Quantity++;

                // Store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                // Return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            // Init cart
            List<CartView> cart = Session["cart"] as List<CartView>;

            using (DBLayer db = new DBLayer())
            {
                // Get model from list
                CartView model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Decrement qty
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Store needed data
                var result = new { qty = model.Quantity, price = model.Price };

                // Return json
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // Init cart list
            List<CartView> cart = Session["cart"] as List<CartView>;

            using (DBLayer db = new DBLayer())
            {
                // Get model from list
                CartView model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Remove model from list
                cart.Remove(model);
            }

        }

        public ActionResult PaypalPartial()
        {
            List<CartView> cart = Session["cart"] as List<CartView>;

            return PartialView(cart);
        }

        [HttpPost]
        public void PlaceOrder()
        {
            //this part is used to get the cart list
            List<CartView> cart = Session["cart"] as List<CartView>;


            //this is used to get the username
            string username = User.Identity.Name;

            int orderid = 0;


            //this declares the order id
            using (DBLayer db = new DBLayer())
            {
                //this is used to initilise the order class
                OrderDTO orderdto = new OrderDTO();

                //thsi part of the code is used to get the user id 
                var query = db.Users.FirstOrDefault(x => x.Username == username);
                int userId = query.Id;

                //this adds to the order class and saves
                orderdto.UserId = userId;
                orderdto.Orderdate = DateTime.Now;

                db.Orders.Add(orderdto);
                db.SaveChanges();

                //get inserted id 
                orderid = orderdto.OrderId;

                //this is used ot initlise order details class
                OrderDetailsDTO orderdetailsdto = new OrderDetailsDTO();

                //adds to order details class
                foreach(var item in cart)
                {
                    orderdetailsdto.OrderId = orderid;
                    orderdetailsdto.UserId = userId;
                    orderdetailsdto.ProductId = item.ProductId;
                    orderdetailsdto.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderdetailsdto);

                    db.SaveChanges();


                }

            }


        }


    }
}