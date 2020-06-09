using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels
{
    public class CategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
