using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Custom_Computers.Models.Data
{
    public class DBLayer : DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }
        public DbSet<SidebarDTOcs> Sidebar { get; set; }
        public DbSet<CategoryDTO> Category { get; set; }
        public DbSet<ProductDTO> Product { get; set; }
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<RoleDTO> Roles { get; set; }
        public DbSet<UserRoleDTO> UserRole { get; set; }
        public DbSet<OrderDTO> Orders { get; set; }

        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }










    }
}