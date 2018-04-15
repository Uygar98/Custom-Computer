using Custom_Computers.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Custom_Computers.Models.ViewModels.Pages
{
    public class SidebarView
    {
        public SidebarView()
        {

        }
        public SidebarView(SidebarDTOcs rowposition)
        {
            Id= rowposition.Id;
            Body = rowposition.Body;

        }
        
        public int Id { get; set; }
        [AllowHtml]
        public string Body { get; set; }
    }
}
