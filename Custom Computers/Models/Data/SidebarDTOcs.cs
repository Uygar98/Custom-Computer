using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Custom_Computers.Models.Data
{
    [Table("tblSidebar")]
    public class SidebarDTOcs
    {
        [Key]
        public int Id { get; set; }
        public string Body { get; set; }
    }
}