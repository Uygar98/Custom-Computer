using Custom_Computers.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Custom_Computers.Models.ViewModels.Shop
{
    public class CategoryView
    {
        public CategoryView()
        {

        }

        public CategoryView(CategoryDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}