using Custom_Computers.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Custom_Computers.Models.ViewModels.Shop
{
    public class ProductView
    {

        public ProductView()
        {

        }

        public ProductView(ProductDTO position)
        {
            Id = position.Id;
            Name = position.Name;
            Slug = position.Slug;
            Description = position.Description;
            Price = position.Price;
            Quantity = position.Quantity;
            CategoryName = position.CategoryName;
            CategoryId = position.CategoryId;
            ImageName = position.ImageName;

        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string CategoryName { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> Images { get; set; }



    }
}