using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Custom_Computers.Models.Data
{
    //this allows this class to pull data from th table page
    [Table("tblPages")]
    public class PageDTO
    {
        //declaring variables and giving them getters/setters
        //the ID variable is the primary key
        [Key]
        public int Id  { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body { get; set; }
        public int Sorting { get; set; }
        public Boolean Sidebar { get; set; }


    }
}